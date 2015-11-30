using System.Net;
using System.Web.ModelBinding;

namespace CommonUtils.Code.WebRequestData {
    public class WebRequestParamCookieContainer : WebRequestParamBase {
        public WebRequestParamCookieContainer(CookieContainer value) {
            Value = value;
        }

        public override object Clone() {
            return new WebRequestParamCookieContainer((CookieContainer)Value);
        }
    }
}
