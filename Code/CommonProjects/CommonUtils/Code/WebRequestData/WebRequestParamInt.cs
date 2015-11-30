namespace CommonUtils.Code.WebRequestData {
    public class WebRequestParamInt : WebRequestParamBase {
        public WebRequestParamInt(int value) {
            Value = value;
        }

        public override object Clone() {
            return new WebRequestParamInt((int)Value);
        }
    }
}
