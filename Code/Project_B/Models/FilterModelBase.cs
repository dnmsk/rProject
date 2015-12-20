using System;
using MainLogic.WebFiles.PropertyBinderAdvanced;
using Project_B.CodeServerSide.Enums;

namespace Project_B.Models {
    [Flags]
    public enum FilterSettings : short {
        Default = 0,
        FromDate = 0x01,
        ToDate = 0x02,
        BtnAll = 0x04,
        BtnWithOdds = 0x08
    }
    public abstract class FilterModelBase {
        [PropertyBinder(typeof(DateTimeBinder), new[] { "dd.MM.yyyy", "MM/dd/yyyy" })]
        public DateTime date { get; set; }
        [PropertyBinder(typeof(DateTimeBinder), new[] { "dd.MM.yyyy", "MM/dd/yyyy" })]
        public DateTime from { get; set; }
        public bool all { get; set; }
        public string ActionName { get; protected set; }
        public string ControllerName { get; protected set; }
        public LanguageType LanguageType { get; protected set; }
        public FilterSettings FilterSettings { get; set; }
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }
    }

    public class FilterModel<T> : FilterModelBase {
        public T id { get; set; }

        public FilterModel() {
            date = DateTime.MinValue;
            LanguageType = LanguageType.English;
            ActionName = null;
            ControllerName = null;
            id = default(T);
            all = false;
        }

        public FilterModel(string action, string controller, LanguageType languageType, FilterSettings filterSettings, FilterModel<T> filter) : this() {
            ActionName = action;
            ControllerName = controller;
            LanguageType = languageType;
            FilterSettings = filterSettings;

            date = filter.date;
            from = filter.date;
            all = filter.all;
            id = filter.id;
        }
    }
}