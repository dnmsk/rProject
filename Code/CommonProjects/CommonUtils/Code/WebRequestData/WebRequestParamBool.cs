namespace CommonUtils.Code.WebRequestData {
    public class WebRequestParamBool : WebRequestParamBase {
        public WebRequestParamBool(bool value){
            Value = value;
        }

        public override object Clone() {
            return new WebRequestParamBool((bool)Value);
        }
    }
}
