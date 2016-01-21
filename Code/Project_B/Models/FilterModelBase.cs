using System;
using System.Web.Routing;
using CommonUtils.ExtendedTypes;
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
        [PropertyBinder(typeof(DateTimeBinder), new[] { "dd.MM.yyyy", "MM/dd/yyyy", "dd.MM hh:mm", "dd.MM.yyyy hh:mm:ss" })]
        public DateTime date { get; set; }
        [PropertyBinder(typeof(DateTimeBinder), new[] { "dd.MM.yyyy", "MM/dd/yyyy", "dd.MM hh:mm", "dd.MM.yyyy hh:mm:ss" })]
        public DateTime from { get; set; }
        public bool all { get; set; }
        public string ActionName { get; protected set; }
        public string ControllerName { get; protected set; }
        public LanguageType LanguageType { get; protected set; }
        public FilterSettings FilterSettings { get; protected set; }
        public DateTime MinDate { get; protected set; }
        public DateTime MaxDate { get; protected set; }
        
        public void FixDates(DateTime minDate, DateTime maxDate) {
            MinDate = minDate;
            MaxDate = maxDate;

            date = FixDate(date, MaxDate, minDate, maxDate);
            from = FixDate(from, MinDate, minDate, maxDate);
            if (date < from) {
                var tmp = from;
                from = date;
                date = tmp;
            }
        }

        public virtual RouteValueDictionary ToRoute() {
            var result = new RouteValueDictionary();
            if (from != DateTime.MinValue && from != MinDate) {
                result["from"] = from.ToString("dd.MM.yyyy");
            }
            if (date != DateTime.MinValue && date != MaxDate) {
                result["date"] = date.ToString("dd.MM.yyyy");
            }
            if (FilterSettings.HasFlag(FilterSettings.BtnAll)) {
                result["all"] = all;
            }
            return result;
        }

        public virtual RouteValueDictionary GetHiddenFilter() {
            return new RouteValueDictionary();
        }

        private static DateTime FixDate(DateTime dateTime, DateTime ifDefault, DateTime minDate, DateTime maxDate) {
            if (dateTime == DateTime.MinValue) {
                return ifDefault;
            }
            if (dateTime < minDate) {
                dateTime = minDate;
            }
            if (dateTime > maxDate) {
                dateTime = maxDate;
            }
            return dateTime;
        }
    }

    public class FilterModel<T> : FilterModelBase {
        private readonly RouteValueDictionary _additionalRouteValueDictionary;
        public T id { get; set; }

        public FilterModel() {
            LanguageType = LanguageType.English;
            ActionName = null;
            ControllerName = null;
            id = default(T);
        }

        public FilterModel(string action, string controller, LanguageType languageType, FilterSettings filterSettings, FilterModel<T> filter, RouteValueDictionary additionalRouteValueDictionary = null) : this() {
            _additionalRouteValueDictionary = additionalRouteValueDictionary ?? new RouteValueDictionary();
            ActionName = action;
            ControllerName = controller;
            LanguageType = languageType;
            FilterSettings = filterSettings;

            date = filter.date;
            from = filter.from;
            all = filter.all;
            id = filter.id;
            MaxDate = filter.MaxDate;
            MinDate = filter.MinDate;
        }

        public override RouteValueDictionary GetHiddenFilter() {
            return _additionalRouteValueDictionary;
        }

        public override RouteValueDictionary ToRoute() {
            var route =  base.ToRoute();
            route["id"] = id;
            _additionalRouteValueDictionary.Each(kv => {
                route[kv.Key] = kv.Value;
            });
            return route;
        }
    }
}