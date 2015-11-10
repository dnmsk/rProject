using Project_B.CodeServerSide.Data.Result;
using Project_B.CodeServerSide.Entity;

namespace Project_B.CodeServerSide.DataProvider.DataHelper.LiveResultToDbProc.Proc {
    public interface ILiveResultProc {
        void Process(CompetitionResultLive lastResult, CompetitionResultLiveAdvanced lastAdvancedResult, FullResult result);
    }
}
