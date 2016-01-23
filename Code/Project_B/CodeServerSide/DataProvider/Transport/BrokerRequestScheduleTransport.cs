using System;
using System.Collections.Generic;
using Project_B.CodeServerSide.Entity;
using Project_B.CodeServerSide.Entity.Interface.NameConstraint;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.Transport {
    public class BrokerRequestScheduleTransport : IBrokerTyped, ILanguageTyped, ISportTyped {
        public BrokerType BrokerID { get; set; }
        public LanguageType Languagetype { get; set; }
        public SportType SportType { get; set; }
        public GatherBehaviorMode Behaviormode { get; set; }
        public short Pasthistorydays { get; set; }
        public Dictionary<RunTaskMode, TimeSpan> RepeatSchedule { get; set; }
        public Dictionary<RunTaskMode, DateTime> LastTaskRun { get; set; }
        public Enum BrokerField => SystemBrokerRequest.Fields.BrokerID;
        public Enum SportTypeField => SportName.Fields.Sporttype;
        public Enum LanguageTypeField => SiteBrokerPage.Fields.Languagetype;
    }
}