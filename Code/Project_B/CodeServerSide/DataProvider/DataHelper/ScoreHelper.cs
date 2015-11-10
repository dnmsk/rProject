using System;
using System.Collections.Generic;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using MainLogic;
using Project_B.CodeServerSide.Entity;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.DataHelper {
    public class ScoreHelper : Singleton<ScoreHelper> {
        private Dictionary<string, ResultModeType> _nameToGender = new Dictionary<string, ResultModeType>();

        public ScoreHelper() {
            UpdateResultModeMap();
            MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(UpdateResultModeMap, new TimeSpan(0, 30, 0), null));
        }

        private object UpdateResultModeMap() {
            var genders = ResultModeAdvanced.DataSource.AsList();
            var newMap = new Dictionary<string, ResultModeType>();
            foreach (var genderAdvanced in genders) {
                newMap[genderAdvanced.Name.ToLower()] = genderAdvanced.Resultmodetype;
            }
            _nameToGender = newMap;
            return null;
        }
        
        public short GenerateScoreID(short score1, short score2) {
            return (short) (((short) (score1 << 8)) | score2);
        }
        
        public Tuple<short, short> GenerateScore(short scoreID) {
            var score1 = (byte) (scoreID >> 8);
            var score2 = (byte) (((short)(scoreID << 8)) >> 8);
            return new Tuple<short, short>(score1, score2);
        }

        public BetOddType GetResultType(short score1, short score2) {
            if (score1 > score2) {
                return BetOddType.Win1;
            }
            if (score1 < score2) {
                return BetOddType.Win2;
            }
            return BetOddType.Draw;
        }

        public ResultModeType GetResultModeType(SportType sportType, int subResultIndex, string modeTypeString) {
            switch (sportType) {
                case SportType.Tennis:
                case SportType.Volleyball:
                    switch (subResultIndex) {
                        case 0:
                            return ResultModeType.GamePlay1;
                        case 1:
                            return ResultModeType.GamePlay2;
                        case 2:
                            return ResultModeType.GamePlay3;
                        case 3:
                            return ResultModeType.GamePlay4;
                        case 4:
                            return ResultModeType.GamePlay5;
                    }
                    break;
                case SportType.Football:
                    switch (subResultIndex) {
                        case 0:
                            return ResultModeType.GamePlay1;
                        case 1:
                            return ResultModeType.GamePlay2;
                    }
                    break;
                case SportType.Basketball:
                    switch (subResultIndex) {
                        case 0:
                            return ResultModeType.GamePlay1;
                        case 1:
                            return ResultModeType.GamePlay2;
                        case 2:
                            return ResultModeType.GamePlay3;
                        case 3:
                            return ResultModeType.GamePlay4;
                    }
                    break;
                case SportType.IceHockey:
                    switch (subResultIndex) {
                        case 0:
                            return ResultModeType.GamePlay1;
                        case 1:
                            return ResultModeType.GamePlay2;
                        case 2:
                            return ResultModeType.GamePlay3;
                    }
                    break;
            }
            return ResultModeType.OverTime;
        }
    }
}