using System;
using System.Linq;
using CommonUtils.Code;
using HtmlAgilityPack;
using Project_B.CodeServerSide.BrokerProvider.Helper.Configuration;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.BrokerProvider.Helper.HtmlDataExtractor.Extractors {
    public class DefaultDateUtcExtractor<T> : DefaultExtractor<DateTime, T> {
        private readonly DateTimeToGmtFixer _dateTimeToGmtFixer;
        private readonly Func<T, object> _customCreator;
        private readonly Func<string> _additionalDatePart;

        public DefaultDateUtcExtractor(BrokerConfiguration brokerConfiguration, DateTimeToGmtFixer dateTimeToGmtFixer, bool foo, Func<T, object> customCreator = null) : base(brokerConfiguration) {
            _dateTimeToGmtFixer = dateTimeToGmtFixer;
            _customCreator = customCreator;
        }
        
        public DefaultDateUtcExtractor(BrokerConfiguration brokerConfiguration, DateTimeToGmtFixer dateTimeToGmtFixer, Func<string> additionalDatePart = null) : base(brokerConfiguration) {
            _dateTimeToGmtFixer = dateTimeToGmtFixer;
            _additionalDatePart = additionalDatePart;
        }

        private DateTime ParseDateTime(string date) {
            date = date.ToLower().Replace("мая", "май");
            var defaultDateTime = DateTime.MinValue;
            foreach (var dateTimeFormat in BrokerConfiguration.StringArray[SectionName.ArrayDateTimeFormat]
                                           ?? ConfigurationContainer.Instance.BrokerConfiguration[BrokerType.Default].StringArray[SectionName.ArrayDateTimeFormat]) {
                var dateTime = StringParser.ToDateTime(date, defaultDateTime, dateTimeFormat);
                if (!dateTime.Equals(defaultDateTime)) {
                    return dateTime;
                }
            }
            return defaultDateTime;
        }

        protected override void SetToMatchParsed(MatchParsed matchParsed, T container) {
            matchParsed.DateUtc = ExtractData(container, matchParsed.SportType);
        }

        protected override DateTime ExtractData(T arg, SportType sportType, Func<string, DateTime> customCreator = null) {
            var obj = _customCreator?.Invoke(arg) ?? arg;
            if (obj is HtmlNode) {
                obj = HtmlBlockHelper.ExtractBlock(arg as HtmlNode, BrokerConfiguration.XPath[SectionName.XPathToDate]).FirstOrDefault()?.InnerText;
            }
            if (obj is string) {
                obj = obj + (_additionalDatePart?.Invoke() ?? string.Empty);
                return _dateTimeToGmtFixer.FixToGmt(ParseDateTime(obj as string));
            }
            if (obj is int) {
                return ProjectBConsts.DefaultLinuxUtc.AddSeconds((int)obj);
            }
            return DateTime.MinValue;
        }
    }
}