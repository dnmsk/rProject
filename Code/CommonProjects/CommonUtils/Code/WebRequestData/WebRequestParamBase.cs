using System;

namespace CommonUtils.Code.WebRequestData {
    public abstract class WebRequestParamBase : ICloneable {
        public  object Value { get; protected set; }
        public abstract object Clone();
    }
}
