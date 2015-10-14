using Project_B.Code.Data.Result;
using Project_B.Code.Entity;

namespace Project_B.Code.DataProvider.DataHelper.LiveResultToDbProc.Proc {
    public interface ILiveResultProc {
        void Process(CompetitionResultLive lastResult, CompetitionResultLiveAdvanced lastAdvancedResult, FullResult result);
    }
}
