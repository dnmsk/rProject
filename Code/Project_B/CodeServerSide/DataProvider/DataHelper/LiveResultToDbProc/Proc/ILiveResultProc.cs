using System.Collections.Generic;
using Project_B.CodeServerSide.Data.Result;
using Project_B.CodeServerSide.Entity;
using Project_B.CodeServerSide.Entity.BrokerEntity;

namespace Project_B.CodeServerSide.DataProvider.DataHelper.LiveResultToDbProc.Proc {
    public interface ILiveResultProc {
        void Process(List<CompetitionResultLive> lastResultList, FullResult result);
    }
}
