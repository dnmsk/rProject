using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeClientSide.TransportType.SubData;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Entity.BrokerEntity;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.DataHelper {
    public static class CompetitionHelper {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (CompetitionHelper).FullName);

        private static readonly string[] _stopListWithGetNext = {
            "games",
            "tour",
            "турнир",
            "competition"
        };

        private static readonly string[] _stopListWithIncludeCurrent = {
            "copa",
            "cup",
            " кап",
            "кубок",
            "trophy",
            "трофей",
            "league",
            "liga",
            "лига",
            "singles",
            "doubles",
            "разряд",
            "tournament",
            "чемпионат",
            "champion"
        }; 
        private static readonly string[] _stopListWithExcludeCurrent = {
            "pool ",
            "группа",
            "group",
            
            "stage",
            "этап",
            "матчи",

            "round",
            "final",
            "раунд",
            "финал",

            "playof",
            "play of",
            "play-of",
            "плэй-оф",
            "плэйоф",
            "плэй оф",
            "1/",

            "play-out",
            "play out",
            "плей аут",
            "плэй аут",
            "плей-аут",
            "плэй-аут",

            "bowl",

            "cезон",
            "season",
        };

        public static CompetitionUnique TryDetectCompetitionUniqueFromMatches(SportType sportType, IEnumerable<string> nameOrigin, CompetitionParsed competitionToSave) {
            var dates = competitionToSave.Matches.Select(c => c.DateUtc).Where(d => d != DateTime.MinValue).ToArray();
            var minDate = dates.Any() ? dates.Min().AddHours(-4) : DateTime.MinValue;
            var maxdate = dates.Any() ? dates.Max().AddHours(4) : DateTime.MinValue;
            if (minDate < DateTime.UtcNow.Date) {
                var resultModelEqualityComparer = new ResultTransportEqualityComparer();
                var mapResults = CompetitionResult.DataSource
                    .Join(JoinType.Inner, CompetitionItem.Fields.ID, CompetitionResult.Fields.CompetitionitemID, RetrieveMode.Retrieve)
                    .Join(JoinType.Left, CompetitionResultAdvanced.Fields.CompetitionresultID, CompetitionResult.Fields.ID, RetrieveMode.Retrieve)
                    .WhereEquals(CompetitionItem.Fields.Sporttype, (short)sportType)
                    .WhereBetween(CompetitionItem.Fields.Dateeventutc, minDate, maxdate, BetweenType.Inclusive)
                    .Sort(CompetitionItem.Fields.CompetitionuniqueID)
                    .Sort(CompetitionItem.Fields.ID)
                    .Sort(CompetitionResult.Fields.ID)
                    .Sort(CompetitionResultAdvanced.Fields.ID)
                    .AsList(CompetitionItem.Fields.CompetitionuniqueID, CompetitionResult.Fields.ScoreID, CompetitionResultAdvanced.Fields.ScoreID)
                    .GroupBy(e => e.GetJoinedEntity<CompetitionItem>().CompetitionuniqueID)
                    .ToDictionary(g => g.Key, g => g.GroupBy(gr => gr.ID).Select(gr => {
                        var competitionResultAdvanceds = gr.Select(gra => gra.GetJoinedEntity<CompetitionResultAdvanced>()).Where(je => je != null).ToArray();
                        var resultAdvanceds = competitionResultAdvanceds.Any() ? competitionResultAdvanceds.ToArray() : null;
                        return new ResultTransport {
                            ScoreID = gr.First().ScoreID,
                            SubScore = resultAdvanceds != null && resultAdvanceds.Any() ? resultAdvanceds.Select(cra => cra.ScoreID).ToArray() : null
                        };
                    }).ToList());
                var mapCoefficients = new Dictionary<int, float>();
                var hashResults = competitionToSave.Matches
                    .Where(m => m.Result != null)
                    .Select(m => new ResultTransport {
                        ScoreID = ScoreHelper.Instance.GenerateScoreID(m.Result.CompetitorResultOne, m.Result.CompetitorResultTwo),
                        SubScore = m.Result.SubResult.Any()
                            ? m.Result.SubResult.Select(sr => ScoreHelper.Instance.GenerateScoreID(sr.CompetitorResultOne, sr.CompetitorResultTwo)).ToArray()
                            : null
                    })
                    .Distinct(resultModelEqualityComparer)
                    .ToArray();
                foreach (var suitableСompetitionItem in mapResults) {
                    List<ResultTransport> resultsForCompetition;
                    if (!mapResults.TryGetValue(suitableСompetitionItem.Key, out resultsForCompetition)) {
                        mapCoefficients[suitableСompetitionItem.Key] = 0;
                        continue;
                    }
                    var successMatches = Math.Max(
                        hashResults.Count(h => resultsForCompetition.Any(res => resultModelEqualityComparer.Equals(h, res))),
                        hashResults.Count(h => resultsForCompetition.Any(res => resultModelEqualityComparer.Equals(res, h)))
                    );
                    mapCoefficients[suitableСompetitionItem.Key] =
                        (successMatches / (float) resultsForCompetition.Count)
                        * (successMatches / (float) competitionToSave.Matches.Count);
                }
                var orderedCompetitionCoeffs = mapCoefficients.OrderByDescending(kv => kv.Value).ToList();
                if (orderedCompetitionCoeffs.Count == 0) {
                    return null;
                }
                if (orderedCompetitionCoeffs.First().Value > .35 && 
                        (orderedCompetitionCoeffs.Count == 1 || 
                         orderedCompetitionCoeffs.Count > 1 && orderedCompetitionCoeffs[1].Value / orderedCompetitionCoeffs[0].Value < .65)) {
                    var key = orderedCompetitionCoeffs.First().Key;
                    _logger.Info("Для '{0}' поставляю CompetitionUniqueID {1} ({2}). K={3}", nameOrigin.StrJoin(". "), key,
                        CompetitionSpecify.DataSource.WhereEquals(CompetitionSpecify.Fields.CompetitionuniqueID, key).Sort(CompetitionSpecify.Fields.ID).First().Name, orderedCompetitionCoeffs.First().Value);
                    return CompetitionUnique.DataSource.GetByKey(key);
                }
                return null;
            }
            return null;
        }
        
        public static string[] GetShortCompetitionName(string[] names, SportType sportType) {
            var result = new List<string>();
            for (var i = 0; i < names.Length; i++) {
                var name = names[i];
                if (sportType != SportType.Tennis && _stopListWithGetNext.Any(slw => name.IndexOf(slw, StringComparison.InvariantCultureIgnoreCase) >= 0)) {
                    result.Add(name);
                    var nextIdx = i + 1;
                    if (nextIdx < names.Length) {
                        result.Add(names[nextIdx]);
                    }
                    break;
                }

                if (_stopListWithIncludeCurrent.Any(slw => name.IndexOf(slw, StringComparison.InvariantCultureIgnoreCase) >= 0)) {
                    result.Add(name);
                    break;
                }

                if (_stopListWithExcludeCurrent.Any(sl => name.IndexOf(sl, StringComparison.InvariantCultureIgnoreCase) >= 0) && result.Count > 0) {
                    break;
                }
                result.Add(name);
            }
            return result.ToArray();
        }

        public static string ListStringToName(IEnumerable<string> names) {
            return names.StrJoin(". ");
        }
    }
}