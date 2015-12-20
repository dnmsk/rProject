using System.ComponentModel;
using System.Web.Mvc;

namespace MainLogic.WebFiles.PropertyBinderAdvanced {
    interface IPropertyBinder {
        object BindModel(object bindParam, ControllerContext controllerContext, ModelBindingContext bindingContext, MemberDescriptor memberDescriptor);
    }
}
