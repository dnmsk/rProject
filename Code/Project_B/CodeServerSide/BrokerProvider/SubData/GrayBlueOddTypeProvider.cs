using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using CommonUtils.Code;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.BrokerProvider.SubData {
    internal class GrayBlueOddTypeProvider {
        private readonly string _targetOddTypeUrl;
        private readonly WebRequestHelper _webRequestHelper;
        private readonly JavaScriptSerializer _javaScriptSerializer;
        private readonly Func<object, object[]> _toA;
        private readonly Func<object, Dictionary<string, object>> _toD;
        private Dictionary<int, Func<OddParsed>> _oddMapper = new Dictionary<int, Func<OddParsed>>();
        private int _timestamp;

        public GrayBlueOddTypeProvider(string targetOddTypeUrl, WebRequestHelper webRequestHelper, JavaScriptSerializer javaScriptSerializer, Func<object, object[]> toA, Func<object, Dictionary<string, object>> toD) {
            _targetOddTypeUrl = targetOddTypeUrl;
            _webRequestHelper = webRequestHelper;
            _javaScriptSerializer = javaScriptSerializer;
            _toA = toA;
            _toD = toD;
            RefreshMap();
        }

        public Dictionary<int, Func<OddParsed>> GetOddMapCreator(int timestamp) {
            if (timestamp != _timestamp) {
                RefreshMap();
            }
            return _oddMapper;
        }

        private void RefreshMap() {
            var deserializedKab = _toD(_javaScriptSerializer.DeserializeObject(_webRequestHelper.GetContent(_targetOddTypeUrl).Item2));
            _timestamp = (int) deserializedKab["catalogVersion"];
            var oddMapper = new Dictionary<int, Func<OddParsed>>();
            var goodNames = new[] { "1x2", "hand", "total" };
            var dictNameToInternalType = new Dictionary<string, Func<object[], int, BetOddType>> {
                {"1", (objects, i) => BetOddType.Win1 },
                {"x", (objects, i) => BetOddType.Draw },
                {"2", (objects, i) => BetOddType.Win2 },
                {"1x", (objects, i) => BetOddType.Win1Draw },
                {"12", (objects, i) => BetOddType.Win1Win2 },
                {"x2", (objects, i) => BetOddType.DrawWin2 },
                {"hand", (objects, i) => ((string)(_toD(_toA(objects[0])[i + 1]))["eng"]).Equals("1", StringComparison.InvariantCultureIgnoreCase)
                                    ? BetOddType.Handicap1
                                    : BetOddType.Handicap2},
                {"u", (objects, i) => BetOddType.TotalUnder },
                {"o", (objects, i) => BetOddType.TotalOver },
            };
            foreach (Dictionary<string, object> map in _toA(_toD(_toA(deserializedKab["catalog"])
                                                                          .First(m => ((string)_toD(m)["name_eng"]).Equals("main", StringComparison.InvariantCultureIgnoreCase))
                                                                )["grids"])) {
                object objKey;
                if (!map.TryGetValue("name_eng", out objKey)) {
                    continue;
                }
                var key = ((string)objKey).ToLower();
                if (goodNames.Any(gn => gn.Equals(key, StringComparison.InvariantCultureIgnoreCase))) {
                    var array = _toA(map["grid"]);
                    var arrayList = _toA(array[0]);
                    for (var i = 0; i < arrayList.Length; i++) {
                        var name = ((string)_toD(arrayList[i])["eng"]).ToLower();
                        if (dictNameToInternalType.ContainsKey(name)) {
                            var factorId = (int)_toD(_toA(array[1])[i])["factorId"];
                            if (!oddMapper.ContainsKey(factorId)) {
                                var oddType = dictNameToInternalType[name](array, i);
                                oddMapper[factorId] = () => new OddParsed {
                                    Type = oddType
                                };
                            }
                        }
                    }
                }
            }
            _oddMapper = oddMapper;
        }
    }
}