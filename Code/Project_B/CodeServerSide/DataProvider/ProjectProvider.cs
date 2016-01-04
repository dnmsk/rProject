﻿using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeServerSide.Entity;

namespace Project_B.CodeServerSide.DataProvider {
    public class ProjectProvider : Singleton<ProjectProvider> {
        public readonly BetProvider BetProvider = new BetProvider();
        public readonly CompetitionProvider CompetitionProvider = new CompetitionProvider();
        public readonly CompetitorProvider CompetitorProvider =  new CompetitorProvider();
        public readonly ResultProvider ResultProvider = new ResultProvider();
        public readonly StaticPageProvider StaticPageProvider = new StaticPageProvider();
        public readonly WebFileProvider WebFileProvider = new WebFileProvider();
        public readonly FrontCompetitionProvider FrontCompetitionProvider = new FrontCompetitionProvider();

        public List<SiteMapItem> GetSiteMapItems() {
            var result = new List<SiteMapItem>();
            new[] {
                "", "/competition", "/competitionlive", "/history", "/bookmaker"
            }.Each(s => result.Add(new SiteMapItem {
                ChangeFreq = SiteMapChangeFreq.Hourly,
                LastMod = DateTime.Now,
                Location = s,
                Priority = 1
            }));
            SiteBrokerPage.DataSource
                .WhereEquals(SiteBrokerPage.Fields.Istop, true)
                .WhereNotEquals(SiteBrokerPage.Fields.Pageurl, string.Empty)
                .AsList(SiteBrokerPage.Fields.Pageurl, SiteBrokerPage.Fields.Datemodifiedutc)
                .GroupBy(bp => bp.Pageurl.ToLowerInvariant())
                .Each(brokerPages => result.Add(new SiteMapItem {
                    ChangeFreq = SiteMapChangeFreq.Weekly,
                    LastMod = brokerPages.Max(bp => bp.Datemodifiedutc),
                    Location = "/bookmaker/index/" + brokerPages.Key,
                    Priority = 0.8f
                }));
            for (var dateTime = new DateTime(2014, 01, 01); dateTime < DateTime.Today; dateTime = dateTime.AddDays(1)) {
                result.Add(new SiteMapItem {
                    ChangeFreq = SiteMapChangeFreq.Monthly,
                    LastMod = dateTime,
                    Location = "/history?date=" + dateTime.ToString("dd.MM.yyyy"),
                    Priority = 0.4f
                });
            }
            return result;
        } 
    }
}