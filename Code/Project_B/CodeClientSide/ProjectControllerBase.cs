using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Code;
using CommonUtils.ExtendedTypes;
using MainLogic.WebFiles;
using Project_B.CodeClientSide.Enums;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeServerSide.DataProvider.DataHelper;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeClientSide {
    public abstract class ProjectControllerBase : ApplicationControllerBase {
        public const string GMT_COOKIE_NAME = "GMT";
        private int? _gmtDeltaMinutes;

        private int GmtDeltaMinutes {
            get {
                if (!_gmtDeltaMinutes.HasValue) {
                    var gmtCookie = Request.Cookies[GMT_COOKIE_NAME];
                    _gmtDeltaMinutes = gmtCookie != null ? StringParser.ToInt(gmtCookie.Value, default(int)) : default(int);
                }
                return _gmtDeltaMinutes.Value;
            }
        }
        public void FixToUserTime(CompetitionTransport competitionTransport) {
            var delta = GmtDeltaMinutes;
            competitionTransport.CompetitionItems.Each(ci => {
                ci.DateUtc = ci.DateUtc.AddMinutes(_gmtDeltaMinutes.Value);
                ci.CurrentBets?.Values.Each(cb => cb.DateTimeUtc = cb.DateTimeUtc.AddMinutes(delta));
                ci.HistoryMaxBets?.Values.Each(cb => cb.DateTimeUtc = cb.DateTimeUtc.AddMinutes(delta));
                ci.HistoryMinBets?.Values.Each(cb => cb.DateTimeUtc = cb.DateTimeUtc.AddMinutes(delta));
            });
        }

        protected static DateTime FixDateTime(DateTime dateTime, DateTime minDate, DateTime maxDate) {
            if (dateTime < minDate) {
                dateTime = minDate;
            }
            if (dateTime > maxDate) {
                dateTime = maxDate;
            }
            return dateTime;
        }

        public DateTime FixUserTimeToSystem(DateTime dateTime) {
            return dateTime.AddMinutes(-GmtDeltaMinutes);
        }

        private LanguageType _currentLanguage = LanguageType.Default;
        public LanguageType CurrentLanguage {
            get {
                if (_currentLanguage != LanguageType.Default) {
                    return _currentLanguage;
                }
                return _currentLanguage = LanguageTypeHelper.Instance.GetLanguageByIsoOrDefault(RouteData.Values["language"] as string);
            }
        }

        public Tuple<string, string> GetPreviousControllerAction() {
            var referrer = Request.UrlReferrer;
            if (referrer != null && SiteConfiguration.ProductionHostName.IndexOf(referrer.Host, StringComparison.InvariantCultureIgnoreCase) >= 0 && referrer.Segments.Length > 1) {
                var segments = referrer.Segments;
                return new Tuple<string, string>(
                    segments.Length == 2 ? "Home" : segments[2].Trim('/'), 
                    segments.Length <= 3 ? "Index" : segments[3].Trim('/'));
            }
            return null;
        }

        public virtual SubNavigationType SubNavigationType => SubNavigationType.None;
        
        protected DateTime TryGetNotModifiedResultForItems(List<CompetitionTransport> competitions, DateTime minModifiedDate) {
            var utcNow = DateTime.UtcNow;
            return competitions.MaxOrDefault(c =>
                c.CompetitionItems.SelectMany(ci => new[] { ci.DateUtc }.Union(ci.CurrentBets?.Select(b => b.Value.DateTimeUtc) ?? new DateTime[0])).Where(d => d < utcNow && d > minModifiedDate).MaxOrDefault(d => d, minModifiedDate), minModifiedDate);
        }
        protected DateTime TryGetNotModifiedResultForGame(CompetitionAdvancedTransport competitionAdvanced, DateTime minModifiedDate) {
            var utcNow = DateTime.UtcNow;
            return competitionAdvanced.CompetitionTransport.
                CompetitionItems.MaxOrDefault(ci => new[] { ci.DateUtc }.Union(ci.CurrentBets?.Select(b => b.Value.DateTimeUtc) ?? new DateTime[0]).Where(d => d < utcNow && d > minModifiedDate).MaxOrDefault(d => d, minModifiedDate), minModifiedDate);
        }

        private static readonly string[] _dateTimeFormats = { "dd.MM.yyyy", "MM/dd/yyyy" };
        protected DateTime ParseToUserTime(string date, DateTime dateDef, DateTime minDate, DateTime maxDate) {
            var dateParsed = dateDef;
            foreach (var dateTimeFormat in _dateTimeFormats) {
                if ((dateParsed = StringParser.ToDateTime(date, dateDef, dateTimeFormat)) != dateDef) {
                    break;
                }
            }
            return FixUserTimeToSystem(FixDateTime(dateParsed.Date, minDate, maxDate));
        }
    }
}