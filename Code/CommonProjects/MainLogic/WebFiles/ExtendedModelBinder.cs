using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using MainLogic.WebFiles.PropertyBinderAdvanced;

namespace MainLogic.WebFiles {
    public class ExtendedModelBinder : DefaultModelBinder {
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType) {
            if (IsChild(modelType, typeof(BaseModel))) {
                return Activator.CreateInstance(modelType, ((ApplicationControllerBase)controllerContext.Controller).GetBaseModel());
            }
            return base.CreateModel(controllerContext, bindingContext, modelType);
        }

        protected override void BindProperty(
            ControllerContext controllerContext,
            ModelBindingContext bindingContext,
            PropertyDescriptor propertyDescriptor) {
            var propertyBinderAttribute = TryFindPropertyBinderAttribute(propertyDescriptor);
            if (propertyBinderAttribute != null) {
                var binder = CreateBinder(propertyBinderAttribute);
                var value = binder.BindModel(propertyBinderAttribute.BindParam, controllerContext, bindingContext, propertyDescriptor);
                propertyDescriptor.SetValue(bindingContext.Model, value);
            } else // revert to the default behavior.
              {
                base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
            }
        }

        private static IPropertyBinder CreateBinder(PropertyBinderAttribute propertyBinderAttribute) {
            return (IPropertyBinder)DependencyResolver.Current.GetService(propertyBinderAttribute.BinderType);
        }

        private static PropertyBinderAttribute TryFindPropertyBinderAttribute(PropertyDescriptor propertyDescriptor) {
            return propertyDescriptor.Attributes
              .OfType<PropertyBinderAttribute>()
              .FirstOrDefault();
        }

        private static bool IsChild(Type modelType, Type type) {
            return modelType == type || modelType.IsSubclassOf(type);
        }
    }
}
