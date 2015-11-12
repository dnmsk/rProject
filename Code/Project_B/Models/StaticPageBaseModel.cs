using MainLogic.WebFiles;
using Project_B.CodeClientSide;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeServerSide.DataProvider;
using Project_B.CodeServerSide.Enums;

namespace Project_B.Models {
    public class StaticPageBaseModel : BaseModel {
        public StaticPageTransport StaticPageTransport { get; }
        private static readonly StaticPageWebCache<ProjectBActions, StaticPageTransport> _staticPagesCache = new StaticPageWebCache<ProjectBActions, StaticPageTransport>(
            () => ProjectProvider.Instance.StaticPageProvider.GetCurrentStaticPageModels(true),
            type => type);

        public LanguageType CurrentLanguage { get; }
        public StaticPageBaseModel(ProjectControllerBase projectController) : base(projectController.GetBaseModel()) {
            CurrentLanguage = projectController.CurrentLanguage;
            StaticPageTransport = _staticPagesCache.GetPage(projectController.CurrentLanguage, ActionLogAttribute.GetPageActionId(projectController)) 
                ?? new StaticPageTransport();
        }

        public StaticPageBaseModel(BaseModel baseModel) : base(baseModel) {
            CurrentLanguage = LanguageType.Default;
            StaticPageTransport = new StaticPageTransport();
        }
    }

    public class StaticPageBaseModel<T> : StaticPageBaseModel {
        public T ControllerModel { get; set; }
        public StaticPageBaseModel(ProjectControllerBase projectController) : base(projectController) {}
        public StaticPageBaseModel(BaseModel baseModel) : base(baseModel) {}
    }
}