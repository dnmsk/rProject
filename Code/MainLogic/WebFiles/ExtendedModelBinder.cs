using System;
using System.Web.Mvc;

namespace MainLogic.WebFiles {
    public class ExtendedModelBinder : DefaultModelBinder {
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType) {
            if (IsChild(modelType, typeof(BaseModel))) {
                return Activator.CreateInstance(modelType, ((ApplicationControllerBase)controllerContext.Controller).GetBaseModel());
            }
            return base.CreateModel(controllerContext, bindingContext, modelType);
        }
        
        private static bool IsChild(Type modelType, Type type) {
            return modelType == type || modelType.IsSubclassOf(type);
        }
    }
}
