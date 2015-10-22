using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Project_B.Code.Data.Result;

namespace Project_B.Models {
    public class CompetitionResultItem : CompetitionItemShortModel {
        public FullResult FullResult { get; set; }
    }
}