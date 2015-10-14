using Project_B.Code.DataProvider.DataHelper.LiveResultToDbProc.Proc;
using Project_B.Code.Enums;

namespace Project_B.Code.DataProvider.DataHelper.LiveResultToDbProc {
    public static class LiveResultProcFactory {
        public static ILiveResultProc GetLiveResultProc(SportType sportType) {
            switch (sportType) {
                case SportType.Tennis:
                    return new TennisLiveResultProcessor();
                case SportType.Volleyball:
                    return new VolleyballLiveResultProcessor();
                default: 
                    return new DefaultLiveResultProcessor();
            }
        }
    }
}