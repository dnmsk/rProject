using System;

namespace Project_B.CodeServerSide.Enums {
    [Flags]
    public enum RunTaskMode : short {
        Default = 0,
        AllTasks = HistoryTasks | OddsTasks,
        HistoryTasks = RunTodayHistoryTask | RunPastDateHistoryTask,
        OddsTasks = RunLiveOddsTask | RunRegularOddsTask,
        RunTodayHistoryTask = 0x01,
        RunPastDateHistoryTask = 0x02,
        RunLiveOddsTask = 0x04,
        RunRegularOddsTask = 0x08
    }
}