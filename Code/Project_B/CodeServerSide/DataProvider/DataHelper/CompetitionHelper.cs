using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Filters;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeClientSide.TransportType.SubData;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.DataProvider.Transport;
using Project_B.CodeServerSide.Entity.BrokerEntity;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.DataHelper {
    internal static class CompetitionHelper {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (CompetitionHelper).FullName);

        private static readonly object _lockObject = new object();

        private static readonly List<string> _stopListWithInclude = new List<string> {
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
        }; 
        private static readonly List<string> _stopList = new List<string> {
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
        };

        public static CompetitionSpecifyTransport CreateCompetitionSpecify(BrokerType brokerType, LanguageType language, SportType sportType, GenderType genderDetected, List<string> nameOrigin, CompetitionParsed competitionToSave, GatherBehaviorMode algoMode) {
            var nameOriginShort = GetShortCompetitionName(nameOrigin);
            var competitionParsedFromRaw = RawCompetitionHelper.CreateCompetitionSpecify(brokerType, language, sportType, genderDetected, nameOrigin, nameOriginShort, competitionToSave, algoMode);
            if (competitionParsedFromRaw.CompetitionSpecifyUniqueID != default(int)) {
                if (algoMode.HasFlag(GatherBehaviorMode.CreateNewLanguageName)) {
                    if (competitionParsedFromRaw.CompetitionUniqueID != default(int) && !Competition.DataSource
                        .WhereEquals(Competition.Fields.CompetitionuniqueID, competitionParsedFromRaw.CompetitionUniqueID)
                        .WhereEquals(Competition.Fields.Languagetype, (short)language)
                        .IsExists()) {
                        new Competition {
                            CompetitionuniqueID = competitionParsedFromRaw.CompetitionUniqueID,
                            Datecreatedutc = DateTime.UtcNow,
                            Gendertype = genderDetected,
                            SportType = sportType,
                            Name = ListStringToName(nameOriginShort),
                            Languagetype = language
                        }.Save();
                    }
                    if (competitionParsedFromRaw.CompetitionSpecifyUniqueID != default(int) && 
                        competitionParsedFromRaw.CompetitionUniqueID != default(int) && !CompetitionSpecify.DataSource
                            .WhereEquals(CompetitionSpecify.Fields.CompetitionSpecifyUniqueID, competitionParsedFromRaw.CompetitionSpecifyUniqueID)
                            .WhereEquals(CompetitionSpecify.Fields.Languagetype, (short) language)
                            .IsExists()) {
                        new CompetitionSpecify {
                            Languagetype = language,
                            Name = ListStringToName(nameOrigin),
                            CompetitionSpecifyUniqueID = competitionParsedFromRaw.CompetitionSpecifyUniqueID,
                            CompetitionuniqueID = competitionParsedFromRaw.CompetitionUniqueID,
                            Datecreatedutc = DateTime.UtcNow,
                            Gendertype = genderDetected,
                            SportType = sportType
                        }.Save();
                    }
                }
                return competitionParsedFromRaw;
            }
            if (!algoMode.HasFlag(GatherBehaviorMode.CreateOriginal)) {
                return null;
            }
            lock(_lockObject) { 
                if (competitionParsedFromRaw.CompetitionUniqueID == default(int)) {
                    var competition = Competition.DataSource
                            .WhereEquals(Competition.Fields.Gendertype, (short)genderDetected)
                            .WhereEquals(Competition.Fields.Languagetype, (short)language)
                            .WhereEquals(Competition.Fields.Sporttype, (short)sportType)
                            .Where(QueryHelper.GetFilterByWordsForField(nameOriginShort, Competition.Fields.Name))
                            .First(Competition.Fields.CompetitionuniqueID);
                    if (competition == null) {
                        var competitionUnique = new CompetitionUnique {
                            IsUsed = true
                        };
                        competitionUnique.Save();
                        competition = new Competition {
                            Datecreatedutc = DateTime.UtcNow,
                            Languagetype = language,
                            SportType = sportType,
                            Name = ListStringToName(nameOriginShort),
                            Gendertype = genderDetected,
                            CompetitionuniqueID = competitionUnique.ID
                        };
                        competition.Save();
                    }
                    competitionParsedFromRaw.CompetitionUniqueID = competition.CompetitionuniqueID;
                }

                var competitionSpecify = CompetitionSpecify.DataSource
                    .WhereEquals(CompetitionSpecify.Fields.Gendertype, (short)genderDetected)
                    .WhereEquals(CompetitionSpecify.Fields.Languagetype, (short)language)
                    .WhereEquals(CompetitionSpecify.Fields.Sporttype, (short)sportType)
                    .Where(QueryHelper.GetFilterByWordsForField(nameOrigin, CompetitionSpecify.Fields.Name))
                    .Where(new DaoFilterOr(
                        new DaoFilterNull(CompetitionSpecify.Fields.CompetitionuniqueID, true),
                        new DaoFilterEq(CompetitionSpecify.Fields.CompetitionuniqueID, competitionParsedFromRaw.CompetitionUniqueID)
                    ))
                    .Sort(CompetitionSpecify.Fields.ID)
                    .First(
                        CompetitionSpecify.Fields.CompetitionuniqueID,
                        CompetitionSpecify.Fields.CompetitionSpecifyUniqueID
                    )
                    ?? new CompetitionSpecify {
                        Datecreatedutc = DateTime.UtcNow,
                        Languagetype = language,
                        SportType = sportType,
                        Name = ListStringToName(nameOrigin),
                        Gendertype = genderDetected,
                        CompetitionuniqueID = competitionParsedFromRaw.CompetitionUniqueID
                    };
                if (competitionParsedFromRaw.CompetitionSpecifyUniqueID == default(int)) {
                    if (competitionSpecify.CompetitionSpecifyUniqueID == default(int)) {
                        var competitionSpecifyUnique = new CompetitionSpecifyUnique {
                            CompetitionuniqueID = competitionParsedFromRaw.CompetitionUniqueID
                        };
                        competitionSpecifyUnique.Save();
                        competitionSpecify.CompetitionSpecifyUniqueID = competitionSpecifyUnique.ID;
                        competitionSpecify.Save();
                    }
                    competitionParsedFromRaw.CompetitionSpecifyUniqueID = competitionSpecify.CompetitionSpecifyUniqueID;
                    if (competitionSpecify.CompetitionuniqueID != competitionParsedFromRaw.CompetitionUniqueID) {
                        _logger.Error("{0} != {1}. {2}. SKIP", competitionSpecify.CompetitionuniqueID, competitionParsedFromRaw.CompetitionUniqueID, ListStringToName(nameOrigin));
                        return null;
                    }
                }
            }
            return RawCompetitionHelper.UpdateCompetitionParsedForUniqueIDs(competitionParsedFromRaw, LinkEntityStatus.Original);
        }

        public static CompetitionUnique TryDetectCompetitionUniqueFromMatches(SportType sportType, List<string> nameOrigin, CompetitionParsed competitionToSave) {
            var dates = competitionToSave.Matches.Select(c => c.DateUtc).Where(d => d != DateTime.MinValue).ToArray();
            var minDate = dates.Any() ? dates.Min().Date : DateTime.MinValue;
            var maxdate = dates.Any() ? dates.Max().Date.AddDays(1) : DateTime.MinValue;
            if (minDate < DateTime.UtcNow.Date) {
                var resultModelEqualityComparer = new ResultTransportEqualityComparer();
                var mapResults = CompetitionResult.DataSource
                    .Join(JoinType.Inner, CompetitionItem.Fields.ID, CompetitionResult.Fields.CompetitionitemID, RetrieveMode.Retrieve)
                    .Join(JoinType.Inner, CompetitionResultAdvanced.Fields.CompetitionresultID, CompetitionResult.Fields.ID, RetrieveMode.Retrieve)
                    .WhereEquals(CompetitionItem.Fields.Sporttype, (short)sportType)
                    .Where(new DaoFilterAnd(
                        new DaoFilter(CompetitionItem.Fields.Dateeventutc, Oper.GreaterOrEq, minDate),
                        new DaoFilter(CompetitionItem.Fields.Dateeventutc, Oper.Less, maxdate)
                    ))
                    .Sort(CompetitionItem.Fields.CompetitionuniqueID)
                    .Sort(CompetitionItem.Fields.ID)
                    .Sort(CompetitionResult.Fields.ID)
                    .Sort(CompetitionResultAdvanced.Fields.ID)
                    .AsList(CompetitionItem.Fields.CompetitionuniqueID, CompetitionResult.Fields.ScoreID, CompetitionResultAdvanced.Fields.ScoreID)
                    .GroupBy(e => e.GetJoinedEntity<CompetitionItem>().CompetitionuniqueID)
                    .ToDictionary(g => g.Key, g => g.GroupBy(gr => gr.ID).Select(gr => new ResultTransport {
                        ScoreID = gr.First().ScoreID,
                        SubScore = gr.Select(gra => gra.GetJoinedEntity<CompetitionResultAdvanced>().ScoreID).ToArray()
                    }).Distinct(resultModelEqualityComparer).ToList());
                var mapCoefficients = new Dictionary<int, float>();
                var hashResults = competitionToSave.Matches
                    .Where(m => m.Result != null)
                    .Select(m => new ResultTransport {
                        ScoreID = ScoreHelper.Instance.GenerateScoreID(m.Result.CompetitorResultOne, m.Result.CompetitorResultTwo),
                        SubScore = m.Result.SubResult.Any()
                            ? m.Result.SubResult.Select(sr => ScoreHelper.Instance.GenerateScoreID(sr.CompetitorResultOne, sr.CompetitorResultTwo)).ToArray()
                            : new short[0]
                    })
                    .Distinct(resultModelEqualityComparer)
                    .ToArray();
                foreach (var suitableСompetitionItem in mapResults) {
                    List<ResultTransport> resultsForCompetition;
                    if (!mapResults.TryGetValue(suitableСompetitionItem.Key, out resultsForCompetition)) {
                        mapCoefficients[suitableСompetitionItem.Key] = 0;
                        continue;
                    }
                    var successMatches = resultsForCompetition.Count(res => hashResults.Any(h => resultModelEqualityComparer.Equals(h, res)));
                    mapCoefficients[suitableСompetitionItem.Key] = (float)successMatches / competitionToSave.Matches.Count;
                }
                var orderedCompetitionCoeffs = mapCoefficients.OrderByDescending(kv => kv.Value).ToList();
                if (orderedCompetitionCoeffs.Count == 0) {
                    return null;
                }
                if (orderedCompetitionCoeffs.First().Value > .5 && 
                        (orderedCompetitionCoeffs.Count == 1 || 
                         orderedCompetitionCoeffs.Count > 1 && (
                            (orderedCompetitionCoeffs[0].Value - orderedCompetitionCoeffs[1].Value) > .3) || orderedCompetitionCoeffs[0].Key == orderedCompetitionCoeffs[1].Key)) {
                    var key = orderedCompetitionCoeffs.First().Key;
                    _logger.Info("Для '{0}' поставляю CompetitionUniqueID {1} ({2}). K={3}", nameOrigin.StrJoin(". "), key,
                        CompetitionSpecify.DataSource.WhereEquals(CompetitionSpecify.Fields.CompetitionuniqueID, key).Sort(CompetitionSpecify.Fields.ID).First().Name, orderedCompetitionCoeffs.First().Value);
                    return CompetitionUnique.DataSource.GetByKey(key);
                }
                return null;
            }
            return null;
        }


        private static List<string> GetShortCompetitionName(List<string> names) {
            var result = new List<string>();
            foreach (var name in names) {
                if (_stopListWithInclude.Any(slw => name.IndexOf(slw, StringComparison.InvariantCultureIgnoreCase) >= 0)) {
                    result.Add(name);
                    break;
                }

                if (_stopList.Any(sl => name.IndexOf(sl, StringComparison.InvariantCultureIgnoreCase) >= 0) && result.Count > 1) {
                    break;
                }
                result.Add(name);
            }
            return result;
        }

        public static string ListStringToName(List<string> names) {
            return names.StrJoin(". ");
        }
    }
}