﻿using System.Collections.Generic;
using Project_B.CodeServerSide.Data.Result;
using Project_B.CodeServerSide.Entity;

namespace Project_B.CodeServerSide.DataProvider.DataHelper.LiveResultToDbProc.Proc {
    public class DefaultLiveResultProcessor : ILiveResultProc {
        public void Process(List<CompetitionResultLive> lastResultList, FullResult result) {}
    }
}