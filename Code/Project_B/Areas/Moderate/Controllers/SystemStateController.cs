using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using CommonUtils.ExtendedTypes;
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
                ControllerModel = new WithFilterModel<BrokerType, List<RawCompetitionTransport>>(new FilterModel<BrokerType>("CompetitionItem", "SystemState", CurrentLanguage, FilterSettings.ToDate, filter, new RouteValueDictionary(new { languagetype }))) {
                    Data = _provider.GetCompetitionItems(filter.id, languagetype, filter.date, state)
                }
            });
        }

        public ActionResult EntityLinker(BrokerEntityType type, FilterModel<int> filter, int cid = default(int), int targetID = default(int)) {
            switch (Request.RequestType.ToUpper()) {
                case "GET":
                    return PartialView(new Tuple<RawEntityWithLink, List<RawEntityWithLink>>(_provider.GetEntity(filter.id, type), 
                        _provider.EntityLinkerGet(cid, filter.id, type, filter.date)));
                case "PUT":
                    _provider.EntityLinkerPut(filter.id, type);
                    break;
                case "POST":
                    _provider.EntityLinkerPost(filter.id, type, targetID);
                    break;
                case "DELETE":
                    _provider.EntityLinkerDelete(filter.id, type);
                    break;
            }
            return PartialView("_RawEntityWriter", new Tuple<RawEntityWithLink, string>(_provider.GetEntity(filter.id, type), string.Empty));
        }

        public ActionResult LiveSearch(BrokerEntityType type, int id, string search, bool all = true) {
            var data = _provider.LiveSearch(type, id, search, all);
            return PartialView("_ListRawEntityWriter", data);
        }

        public ActionResult LiveSearchJoin(BrokerEntityType type, int id, string search, bool all = true) {
            var data = _provider.LiveSearch(type, id, search, all);
            return PartialView("_ListRawEntityWriterJoin", data);
        }

        public ActionResult JoinEntity(BrokerEntityType type, int[] ids) {
            _provider.EntityJoin(type, ids);
            return new EmptyResult();
        }

        public ActionResult GetTooltip(BrokerEntityType type, int uniqueid) {
            List<string> res = _provider.GetTooltip(type, uniqueid);
            return new ContentResult {
                Content = res.StrJoin("<br/>")
            };
        }
    }
}