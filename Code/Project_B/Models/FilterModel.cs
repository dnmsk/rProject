using System;
using Project_B.CodeClientSide.Enums;
using Project_B.CodeServerSide.Enums;

namespace Project_B.Models {
    public class FilterModel {
        public int LimitToDisplayInGroup = int.MaxValue;
        public SportType SportType { get; set; }
        public DateTime DateUtc { get; set; }
        public DisplayColumnType DisplayColumn { get; set; }

        public FilterModel() {
            DateUtc = DateTime.MinValue;
            DisplayColumn = DisplayColumnType.All;
        }
    }
}