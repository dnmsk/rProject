using System;
using System.Collections.Generic;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using MainLogic;
using Project_B.Code.Entity;
using Project_B.Code.Enums;

namespace Project_B.Code.DataProvider.DataHelper {
    public class ScoreHelper : Singleton<ScoreHelper> {
        private Dictionary<string, ResultModeType> _nameToGender = new Dictionary<string, ResultModeType>();

        public ScoreHelper() {
            UpdateSportMap();
            MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(UpdateSportMap, new TimeSpan(0, 30, 0), null));
        }

        private object UpdateSportMap() {
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

        public ResultType GetResultType(short score1, short score2) {
            if (score1 > score2) {
                return ResultType.Win1;
            }
            if (score1 < score2) {
                return ResultType.Win2;
            }
            return ResultType.Draw;
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