using System;
namespace MainLogic.WebFiles.PropertyBinderAdvanced {
    public class PropertyBinderAttribute : Attribute {
        public PropertyBinderAttribute(Type binderType) : this(binderType, null) {}
        public PropertyBinderAttribute(Type binderType, object bindParam) {
            BinderType = binderType;
            BindParam = bindParam;
        }

        public Type BinderType { get; private set; }
        public object BindParam { get; private set; }
    }
}
