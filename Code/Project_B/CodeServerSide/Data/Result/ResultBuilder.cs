using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Data.Result {
    public static class ResultBuilder {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (ResultBuilder).FullName);

        private const char _resLeftSide = '(';
        private const char _resRightSide = ')';
        private const char _resDelim = '-';
        private static readonly string _resultSerializeTpl = _resLeftSide + "{0}" + _resDelim + "{1}" + _resRightSide;

        public static string BuildStringFromResult(FullResult res) {
            return string.Format(_resultSerializeTpl, res.CompetitorResultOne, res.CompetitorResultTwo) + 
                (res.SubResult != null && res.SubResult.Any()
                    ? res.SubResult.Where(sr => sr != null).Select(sr => string.Format(_resultSerializeTpl, sr.CompetitorResultOne, sr.CompetitorResultTwo)).StrJoin(string.Empty)
                    : string.Empty);
        }

        public static FullResult BuildResultFromString(SportType sportType, string resString) {
            FullResult result ;
            switch (sportType) {
                case SportType.Tennis:
                case SportType.Volleyball:
                    var buildTennisResult = BuildFullResult(resString);
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
                    result = BuildFullResult(resString);
                    break;
                case SportType.Basketball:
                    result = BuildFullResult(resString);
                    break;
                case SportType.Football:
                    result = BuildFullResult(resString);
                    break;
                default:
                    result = BuildFullResult(resString);
                    break;
            }
            return result;
        }
        
        private static FullResult BuildFullResult(string resString) {
            try {
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
            catch (Exception ex) {
                _logger.Error(ex);
            }
            return null;
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
            resStr = resStr ?? string.Empty;
            var simpleResult = new SimpleResult();
            var isFirstDigitCollect = false;
            var canDetectServe = false;
            var canDetectDidit = true;
            var serve = Serve.Unknown;
            var resultReturned = false;
            foreach (var ch in resStr) {
                short digit;
                if (canDetectDidit && _charToDigit.TryGetValue(ch, out digit)) {
                    if (isFirstDigitCollect) {
                        simpleResult.CompetitorResultTwo = (short) (simpleResult.CompetitorResultTwo * 10 + digit);
                    } else {
                        simpleResult.CompetitorResultOne = (short) (simpleResult.CompetitorResultOne * 10 + digit);
                    }
                    resultReturned = false;
                    continue;
                }
                switch (ch) {
                    case _resLeftSide:
                        canDetectServe = true;
                        break;
                    case _resRightSide:
                        canDetectServe = false;
                        break;
                    case '<':
                        canDetectDidit = false;
                        break;
                    case '>':
                        canDetectDidit = true;
                        break;
                    case _resDelim:
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
                    resultReturned = true;
                    isFirstDigitCollect = false;
                    simpleResult.Serve = serve;
                    yield return simpleResult;
                    simpleResult = new SimpleResult();
                    serve = Serve.Unknown;
                }
            }
            if (!resultReturned) {
                yield return simpleResult;
            }
        } 
    }
}