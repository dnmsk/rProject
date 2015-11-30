using System.Collections.Generic;
using System.Net;

namespace CommonUtils.Code.WebRequestData {
    public class WebRequestParamWebHeaderCollection : WebRequestParamBase {
        public WebRequestParamWebHeaderCollection(WebHeaderCollection value) {
            Value = value;
        }

        public override object Clone() {
            return new WebRequestParamWebHeaderCollection((WebHeaderCollection) Value);
        }
    }
}
