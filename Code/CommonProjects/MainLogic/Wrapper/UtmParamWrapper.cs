using System.Web;
using CommonUtils.ExtendedTypes;

namespace MainLogic.Wrapper {
    public struct UtmParamWrapper {
        public string UtmSource { get; }
        public string UtmCampaign { get; }
        public string UtmMedium { get; }
        public bool IsNew { get; }

        public UtmParamWrapper(string utmSource, string utmCampaign, string utmMedium, bool isNew) {
            UtmSource = utmSource;
            UtmCampaign = utmCampaign;
            UtmMedium = utmMedium;
            IsNew = isNew;
        }

        public string SerializeStruct() {
            return HttpUtility.UrlEncode(string.Join("&", UtmSource, UtmCampaign, UtmMedium));
        }

        public static UtmParamWrapper DeserializeParamWrapper(string str) {
            var urlDecode = HttpUtility.UrlDecode(str);
            if (urlDecode != null) {
                var data = urlDecode.Split('&');
                if (data.Length == 3) {
                    return new UtmParamWrapper(data[0], data[1], data[2], false);
                }
            }
            return new UtmParamWrapper();
        }

        public bool Any() {
            return !UtmMedium.IsNullOrWhiteSpace() || 
                   !UtmCampaign.IsNullOrWhiteSpace() ||
                   !UtmSource.IsNullOrWhiteSpace();
        }
    }
}