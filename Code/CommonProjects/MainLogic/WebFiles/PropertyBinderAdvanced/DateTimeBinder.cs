using System;
using System.ComponentModel;
using System.Web.Mvc;
using CommonUtils.Code;
using CommonUtils.ExtendedTypes;

namespace MainLogic.WebFiles.PropertyBinderAdvanced {
    public class DateTimeBinder :IPropertyBinder {
        public object BindModel(object bindParam, ControllerContext controllerContext, ModelBindingContext bindingContext, MemberDescriptor memberDescriptor) {
            var timeString = controllerContext.HttpContext.Request.Params[memberDescriptor.Name];
            var def = default(DateTime);
            if (timeString.IsNullOrWhiteSpace()) {
                return DateTime.MinValue;
            }
            var formats = bindParam as string[];
            DateTime dateTimeParsed;
            if (formats != null) {
                foreach (var format in formats) {
                    dateTimeParsed = StringParser.ToDateTime(timeString, def, format);
                    if (dateTimeParsed != def) {
                        return dateTimeParsed;
                    }
                }
            }
            dateTimeParsed = StringParser.ToDateTime(timeString, def);
            return dateTimeParsed == def ? DateTime.MinValue : dateTimeParsed;
        }
    }
}
