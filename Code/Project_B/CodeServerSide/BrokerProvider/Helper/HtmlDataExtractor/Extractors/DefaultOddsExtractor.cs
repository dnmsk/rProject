using System;
using System.Collections.Generic;
using CommonUtils.Code;
using CommonUtils.ExtendedTypes;
using Project_B.CodeServerSide.BrokerProvider.Helper.Configuration;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.BrokerProvider.Helper.HtmlDataExtractor.Extractors {
    public class DefaultOddsExtractor<T> : DefaultExtractor<List<OddParsed>, T> {
        private readonly Func<T, SportType, object> _customParser;

        public DefaultOddsExtractor(BrokerConfiguration brokerConfiguration, Func<T, SportType, object> customParser = null) : base(brokerConfiguration) {
            _customParser = customParser;
        }

        protected override void SetToMatchParsed(MatchParsed matchParsed, T container) {
            matchParsed.Odds.AddRange(ExtractData(container, matchParsed.SportType));
        }

        protected override List<OddParsed> ExtractData(T container, SportType sportType, Func<string, List<OddParsed>> customCreator = null) {
            var obj = _customParser?.Invoke(container, sportType) ?? container;
            if (obj is List<OddParsed>) {
                return obj as List<OddParsed>;
            }
            if (obj is string[]) {
                return ParsePromStringArray(obj as string[]);
            }
            return null;
        }

        private List<OddParsed> ParsePromStringArray(string[] odds) {
            var res = new List<OddParsed>();
            foreach (var odd in odds) {
                if (odd.IsNullOrWhiteSpace()) {
                    continue;
                }
                var splitted = odd.Split(new [] { " - ", "(", ")" }, StringSplitOptions.RemoveEmptyEntries);
                var oddType = BetOddType.Unknown;
                var factor = default(float);
                var advancedParam = (float?) null;
                switch (splitted[0].Trim().ToUpper()) {
                    case "1":
                        oddType = BetOddType.Win1;
                        factor = StringParser.ToFloat(splitted[1]);
                        break;
                    case "X":
                        oddType = BetOddType.Draw;
                        factor = StringParser.ToFloat(splitted[1]);
                        break;
                    case "2":
                        oddType = BetOddType.Win2;
                        factor = StringParser.ToFloat(splitted[1]);
                        break;
                    case "1X":
                        oddType = BetOddType.Win1Draw;
                        factor = StringParser.ToFloat(splitted[1]);
                        break;
                    case "12":
                        oddType = BetOddType.Win1Win2;
                        factor = StringParser.ToFloat(splitted[1]);
                        break;
                    case "X2":
                        oddType = BetOddType.DrawWin2;
                        factor = StringParser.ToFloat(splitted[1]);
                        break;
                    case "H1":
                        oddType = BetOddType.Handicap1;
                        factor = StringParser.ToFloat(splitted[2]);
                        break;
                    case "H2":
                        oddType = BetOddType.Handicap2;
                        advancedParam = StringParser.ToFloat(splitted[1]);
                        factor = StringParser.ToFloat(splitted[2]);
                        break;
                    case "TU":
                        oddType = BetOddType.TotalUnder;
                        factor = StringParser.ToFloat(splitted[2]);
                        break;
                    case "TO":
                        oddType = BetOddType.TotalOver;
                        advancedParam = StringParser.ToFloat(splitted[1]);
                        factor = StringParser.ToFloat(splitted[2]);
                        break;
                }
                if (oddType != BetOddType.Unknown) {
                    res.Add(new OddParsed {
                        Type = oddType,
                        Factor = factor,
                        AdvancedParam = advancedParam
                    });
                }
            }

            return res;
        }
    }
}