namespace CommonUtils.Code.WebRequestData {
    public class WebRequestParamString : WebRequestParamBase {
        public WebRequestParamString(string value) {
            Value = value;
        }

        public override object Clone() {
            return new WebRequestParamString((string) Value);
        }
    }
}
