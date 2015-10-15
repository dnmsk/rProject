using System.Collections.Generic;
using System.Linq;
using Project_B.Code.Enums;

namespace Project_B.Code.Data.Result {
    public static class ResultBuilder {
        public static FullResult BuildResultFromString(SportType sportType, string resString) {
            FullResult result ;
            switch (sportType) {
                case SportType.Tennis:
                    var buildTennisResult = BuildTennisResult(resString);
                    if (buildTennisResult.SubResult.Count == 1) {
                        var tempTennisResult = new FullResult {
                            CompetitorResultOne = 0,
                            CompetitorResultTwo = 0
                        };
                        tempTennisResult.SubResult.Add(new SimpleResult {
                            CompetitorResultOne = buildTennisResult.CompetitorResultOne,
                            CompetitorResultTwo = buildTennisResult.CompetitorResultTwo
                        });
                        tempTennisResult.SubResult.AddRange(buildTennisResult.SubResult);
                        buildTennisResult = tempTennisResult;
                    }
                    result = buildTennisResult;
                    break;
                case SportType.IceHockey:
                    result = BuildIceHockeyResult(resString);
                    break;
                case SportType.Basketball:
                    result = BuildBasketballResult(resString);
                    break;
                case SportType.Football:
                    result = BuildFootballResult(resString);
                    break;
                case SportType.Volleyball:
                    result = BuildVolleyballResult(resString);
                    break;
                default:
                    result = null;
                    break;
            }
            return result;
        }

        private static FullResult BuildVolleyballResult(string resString) {
            return BuildFullResult(resString);
        }

        private static FullResult BuildFootballResult(string resString) {
            return BuildFullResult(resString);
        }

        private static FullResult BuildBasketballResult(string resString) {
            return BuildFullResult(resString);
        }

        private static FullResult BuildIceHockeyResult(string resString) {
            return BuildFullResult(resString);
        }

        private static FullResult BuildTennisResult(string resString) {
            return BuildFullResult(resString);
        }

        private static FullResult BuildFullResult(string resString) {
            var tempResult = ParseStringToSimpleResults(resString).ToList();
            var fullResult = new FullResult {
                CompetitorResultOne = tempResult[0].CompetitorResultOne,
                CompetitorResultTwo = tempResult[0].CompetitorResultTwo
            };
            for (var i = 1; i < tempResult.Count; i++) {
                fullResult.SubResult.Add(tempResult[i]);
            }
            return fullResult;
        }

        private static readonly Dictionary<char, short> _charToDigit = new Dictionary<char, short> {
            {'0', 0 },
            {'1', 1 },
            {'2', 2 },
            {'3', 3 },
            {'4', 4 },
            {'5', 5 },
            {'6', 6 },
            {'7', 7 },
            {'8', 8 },
            {'9', 9 },
            { 'A', 45 } //костыль
        };

        private static IEnumerable<SimpleResult> ParseStringToSimpleResults(string resStr) {
            var simpleResult = new SimpleResult();
            var isFirstDigitCollect = false;
            var canDetectServe = false;
            var canDetectDidit = true;
            var serve = Serve.Unknown;
            var hasAnyResult = false;
            foreach (var ch in resStr) {
                short digit;
                if (canDetectDidit && _charToDigit.TryGetValue(ch, out digit)) {
                    if (isFirstDigitCollect) {
                        simpleResult.CompetitorResultTwo = (short) (simpleResult.CompetitorResultTwo * 10 + digit);
                    } else {
                        simpleResult.CompetitorResultOne = (short) (simpleResult.CompetitorResultOne * 10 + digit);
                    }
                    continue;
                }
                switch (ch) {
                    case '(':
                        canDetectServe = true;
                        break;
                    case ')':
                        canDetectServe = false;
                        break;
                    case '<':
                        canDetectDidit = false;
                        break;
                    case '>':
                        canDetectDidit = true;
                        break;
                    case ':':
                        if (canDetectDidit) {
                            isFirstDigitCollect = true;
                        }
                        continue;
                }
                if (canDetectServe && ch == '<') {
                    if (serve == Serve.Unknown) {
                        serve = isFirstDigitCollect ? Serve.Serve2Player : Serve.Serve1Player;
                    } else {
                        serve = Serve.Unknown;
                    }
                    continue;
                }
                if (isFirstDigitCollect) {
                    hasAnyResult = true;
                    isFirstDigitCollect = false;
                    simpleResult.Serve = serve;
                    yield return simpleResult;
                    simpleResult = new SimpleResult();
                    serve = Serve.Unknown;
                }
            }
            if (!hasAnyResult) {
                yield return simpleResult;
            }
        } 
    }
}