using System.Collections.Generic;
using MainLogic.WebFiles;
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
        public List<string> AdditionHtmlAssets { get; set; }
        public StaticPageBaseModel(ProjectControllerBase projectController) : base(projectController.GetBaseModel()) {
            StaticPageTransport = _staticPagesCache.GetPage(
                    CurrentLanguage = projectController.CurrentLanguage, 
                    PageKey = ActionLogAttribute.GetPageActionId(projectController)
                ) ?? new StaticPageTransport();
            SubNavigationType = projectController.SubNavigationType;
            AdditionHtmlAssets = new List<string> {
                "bk"
            };
        }

        public StaticPageBaseModel(BaseModel baseModel) : base(baseModel) {
            CurrentLanguage = LanguageType.Default;
            StaticPageTransport = new StaticPageTransport();
            SubNavigationType = SubNavigationType.None;
        }
    }

    public class StaticPageBaseModel<T> : StaticPageBaseModel {
        public T ControllerModel { get; set; }
        public StaticPageBaseModel(ProjectControllerBase projectController) : base(projectController) {}
        public StaticPageBaseModel(BaseModel baseModel) : base(baseModel) {}
    }
}