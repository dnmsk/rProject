using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Filters;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeServerSide.Entity;
using Project_B.CodeServerSide.Entity.Interface;
using Project_B.CodeServerSide.Enums;
using Project_B.Models;

namespace Project_B.CodeServerSide.DataProvider {
    public class StaticPageProvider : SafeInvokerBase {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (StaticPageProvider).FullName);

        public StaticPageProvider() : base(_logger) {}

        public Dictionary<PageType, List<StaticPageTransport>> GetCurrentStaticPageModels(bool onlyTop) {
            return InvokeSafe(() => {
                var result = new Dictionary<PageType, List<StaticPageTransport>>();
                var dbDataSource = StaticPage.DataSource;
                if (onlyTop) {
                    dbDataSource = dbDataSource
                        .WhereEquals(StaticPage.Fields.Istop, true);
                }
                var entities = dbDataSource
                        .Sort(StaticPage.Fields.ID, SortDirection.Asc)
                        .AsList();
                foreach (var staticPage in entities) {
                    List<StaticPageTransport> list;
                    if (!result.TryGetValue(staticPage.Pagetype, out list)) {
                        list = new List<StaticPageTransport>();
                        result[staticPage.Pagetype] = list;
                    }
                    list.Add(StaticPageToModel<StaticPageTransport>(staticPage));
                }
                return result;
            }, null);
        }

        public List<StaticPageTransport> GetAllStaticPageModelsForType(LanguageType languageType, PageType pageType) {
            return InvokeSafe(() => {
                var entities = StaticPage.DataSource
                    .WhereEquals(StaticPage.Fields.Languagetype, (short)languageType)                     
                    .WhereEquals(StaticPage.Fields.Pagetype, (short)pageType)                     
                    .Sort(StaticPage.Fields.ID, SortDirection.Asc)
                    .AsList();
                return entities.Select(StaticPageToModel<StaticPageTransport>).ToList();
            }, null);
        }

        public StaticPageTransport EditStaticPage(StaticPageTransport staticPageTransport, bool isTop) {
            return InvokeSafe(() => {
                var staticPage = staticPageTransport.ID == default(int)
                    ? new StaticPage {
                        Datecreatedutc = DateTime.UtcNow
                    }
                    : StaticPage.DataSource.GetByKey(staticPageTransport.ID);
                var entity = ModelToStaticPage(staticPage, staticPageTransport, isTop);
                return StaticPageToModel<StaticPageTransport>(entity);
            }, null);
        }

        public Dictionary<string, List<BrokerPageTransport>> GetCurrentBrokerPageModels(bool onlyTop) {
            return InvokeSafe(() => {
                var result = new Dictionary<string, List<BrokerPageTransport>>();
                var dbDataSource = BrokerPage.DataSource;
                if (onlyTop) {
                    dbDataSource = dbDataSource
                        .WhereEquals(BrokerPage.Fields.Istop, true);
                }
                var entities = dbDataSource
                        .Sort(BrokerPage.Fields.ID, SortDirection.Asc)
                        .AsList();
                foreach (var staticPage in entities) {
                    List<BrokerPageTransport> list;
                    var key = (staticPage.Pageurl ?? string.Empty).ToLowerInvariant();
                    if (!result.TryGetValue(key, out list)) {
                        list = new List<BrokerPageTransport>();
                        result[key] = list;
                    }
                    list.Add(BuildBrokerPageModel(staticPage));
                }
                return result;
            }, null);
        }

        public List<BrokerPageTransport> GetCurrentBrokerPageModels(LanguageType languageType, string shortUrl) {
            return InvokeSafe(() => {
                var entities = BrokerPage.DataSource
                        .WhereEquals(BrokerPage.Fields.Languagetype, (short)languageType)
                        .Where(BrokerPage.Fields.Pageurl, Oper.Ilike, shortUrl)
                        .Sort(BrokerPage.Fields.ID, SortDirection.Asc)
                        .AsList();
                return entities.Select(BuildBrokerPageModel).ToList();
            }, null);
        }

        public StaticPageTransport EditBrokerPage(BrokerPageTransport staticPageTransport, bool isTop) {
            return InvokeSafe(() => {
                var staticPage = staticPageTransport.ID == default(int)
                    ? new BrokerPage {
                        Datecreatedutc = DateTime.UtcNow
                    }
                    : BrokerPage.DataSource.GetByKey(staticPageTransport.ID);
                var entity = ModelToStaticPage(staticPage, staticPageTransport, isTop);
                entity.Faviconclass = staticPageTransport.Faviconclass;
                entity.Largeiconclass = staticPageTransport.Largeiconclass;
                entity.Orderindex = staticPageTransport.Orderindex;
                entity.Pageurl = staticPageTransport.Pageurl;
                return BuildBrokerPageModel(entity);
            }, null);
        }

        private static BrokerPageTransport BuildBrokerPageModel(BrokerPage staticPage) {
            var staticPageToModel = StaticPageToModel<BrokerPageTransport>(staticPage);
            staticPageToModel.Faviconclass = staticPage.Faviconclass;
            staticPageToModel.Largeiconclass = staticPage.Largeiconclass;
            staticPageToModel.Orderindex = staticPage.Orderindex;
            staticPageToModel.Pageurl = staticPage.Pageurl;
            return staticPageToModel;
        }

        private static T StaticPageToModel<T>(IStaticPage entity) where T : StaticPageTransport, new() {
            return new T {
                ID = entity.ID,
                Languagetype = entity.Languagetype,
                Content = entity.Content,
                Description = entity.Description,
                Keywords = entity.Keywords,
                Title = entity.Title
            };
        }

        private static I ModelToStaticPage<I, T>(I entity, T staticPageModel, bool isTop) where T : StaticPageTransport where I : IStaticPage {
            entity.Datemodifiedutc = DateTime.UtcNow;
            entity.Content = staticPageModel.Content;
            entity.Languagetype = staticPageModel.Languagetype;
            entity.Description = staticPageModel.Description;
            entity.Keywords = staticPageModel.Keywords;
            entity.Title = staticPageModel.Title;
            entity.Content = staticPageModel.Content;
            if (!entity.Istop && isTop) {
                entity.Datepublishedutc = DateTime.UtcNow;
                entity.Istop = isTop;
            }
            entity.Save();
            return entity;
        }
    }
}