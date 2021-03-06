﻿using System;
using System.Collections.Generic;
using MainLogic.Transport;
using MainLogic.WebFiles;
using MainLogic.WebFiles.UserPolicy;
using Project_B.CodeClientSide;
using Project_B.CodeClientSide.Enums;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeServerSide.DataProvider;
using Project_B.CodeServerSide.Enums;

namespace Project_B.Models {
    public class StaticPageBaseModel : BaseModel {
        public StaticPageTransport StaticPageTransport { get; }
        public SubNavigationType SubNavigationType { get; }
        public ProjectBActions PageKey { get; set; }
        private static readonly StaticPageWebCache<ProjectBActions, StaticPageTransport> _staticPagesCache = new StaticPageWebCache<ProjectBActions, StaticPageTransport>(
            () => ProjectProvider.Instance.StaticPageProvider.GetCurrentStaticPageModels(true),
            type => type);

        public LanguageType CurrentLanguage { get; }
        public List<string> AdditionHtmlAssets { get; }
        public StaticPageBaseModel(ProjectControllerBase projectController) : base(projectController.GetBaseModel()) {
            StaticPageTransport = (StaticPageTransport) (_staticPagesCache.GetPage(
                    CurrentLanguage = projectController.CurrentLanguage, 
                    PageKey = ActionLogAttribute.GetPageActionId(projectController)
                ) ?? new StaticPageTransport()).Clone();
            SubNavigationType = projectController.SubNavigationType;
            AdditionHtmlAssets = new List<string>();
        }

        public StaticPageBaseModel(BaseModel baseModel) : base(baseModel) {
            CurrentLanguage = LanguageType.Default;
            StaticPageTransport = new StaticPageTransport();
            SubNavigationType = SubNavigationType.None;
        }

        public Func<DateTime> GetLastModifiedFunc(Func<DateTime> dataDateFunc) {
            return () => {
                var accountDetailsDate = GetUserPolicyState<AccountDetailsTransport>(UserPolicyGlobal.AccountDetails)?.DateLastLogin ?? DateTime.MinValue;
                var dataDate = dataDateFunc();
                return accountDetailsDate > dataDate ? accountDetailsDate : dataDate;
            };
        } 
    }

    public class StaticPageBaseModel<T> : StaticPageBaseModel {
        public T ControllerModel { get; set; }
        public StaticPageBaseModel(ProjectControllerBase projectController) : base(projectController) {}
    }
}