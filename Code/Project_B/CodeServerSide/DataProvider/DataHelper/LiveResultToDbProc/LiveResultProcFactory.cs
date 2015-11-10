using Project_B.CodeServerSide.DataProvider.DataHelper.LiveResultToDbProc.Proc;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.DataHelper.LiveResultToDbProc {
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