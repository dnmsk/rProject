using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Project_B.CodeClientSide;
using Project_B.CodeClientSide.Enums;
using Project_B.CodeClientSide.TransportType.ModerateTransport;
using Project_B.CodeServerSide.DataProvider;
using Project_B.CodeServerSide.Enums;
using Project_B.Models;

namespace Project_B.Areas.Moderate.Controllers {
    [CheckCredential(UserPolicyLocal.IsPageEditor, true)]
    public class SystemStateController : ProjectControllerBase {
        private static readonly SystemStateProvder _provider = new SystemStateProvder();
        private const FilterSettings _filterSettings = FilterSettings.BtnAll | FilterSettings.ToDate;


        public override SubNavigationType SubNavigationType => SubNavigationType.Empty;
        
        public ActionResult Index(FilterModel<int> filter) {
            var dateTime = DateTime.UtcNow.Date;
            if (filter.date == DateTime.MinValue) {
                filter.FixDates(dateTime, dateTime);
            }
            filter.FixDates(new DateTime(2014, 01, 01), dateTime.AddDays(14));
            var fixUserTimeToSystem = FixUserTimeToSystem(filter.date);
            return View(new StaticPageBaseModel<WithFilterModel<List<SystemStateSummaryStatus>>>(this) {
                ControllerModel = new WithFilterModel<List<SystemStateSummaryStatus>>(new FilterModel<int>("Index", "SystemState", CurrentLanguage, _filterSettings, filter)) {
                    Data = filter.all ? _provider.SummarySystemState() : _provider.SummarySystemState(fixUserTimeToSystem, fixUserTimeToSystem.AddDays(1))
                }
            });
        }

        public ActionResult CompetitionItem(LanguageType languagetype, FilterModel<BrokerType> filter, StateFilter state = StateFilter.All) {
            var dateTime = DateTime.UtcNow.Date;
            if (filter.date == DateTime.MinValue) {
                filter.FixDates(dateTime, dateTime);
            }
            filter.FixDates(new DateTime(2014, 01, 01), dateTime.AddDays(14));
            return View(new StaticPageBaseModel<WithFilterModel<BrokerType, List<RawCompetitionTransport>>>(this) {
                ControllerModel = new WithFilterModel<BrokerType, List<RawCompetitionTransport>>(new FilterModel<BrokerType>("CompetitionItem", "SystemState", CurrentLanguage, FilterSettings.ToDate, filter)) {
                    Data = _provider.GetCompetitionItems(filter.id, languagetype, filter.date, state)
                }
            });
        }
    }
}