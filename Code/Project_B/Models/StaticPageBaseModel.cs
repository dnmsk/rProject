﻿using MainLogic.WebFiles;
using Project_B.CodeClientSide;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeServerSide.DataProvider;
using Project_B.CodeServerSide.Enums;

namespace Project_B.Models {
    public class StaticPageBaseModel<T> : BaseModel {
        private static readonly StaticPageWebCache<PageType, StaticPageTransport> _staticPagesCache = new StaticPageWebCache<PageType, StaticPageTransport>(
            () => MainProvider.Instance.StaticPageProvider.GetCurrentStaticPageModels(true),
            type => type);

        public LanguageType CurrentLanguage { get; set; }
        public StaticPageTransport StaticPageTransport { get; set; }

        public T ControllerModel { get; set; }

        public StaticPageBaseModel(LanguageType languageType, PageType pageType, BaseModel baseModel) : base(baseModel) {
            StaticPageTransport = _staticPagesCache.GetPage(languageType, pageType) ?? new StaticPageTransport();
            CurrentLanguage = languageType;
        }
    }
}