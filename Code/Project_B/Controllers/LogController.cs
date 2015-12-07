using System;
using System.Web;
using System.Web.Mvc;
using CommonUtils.Code;
using CommonUtils.Core.Logger;
using MainLogic.WebFiles.UserPolicy.Enum;
using Project_B.CodeClientSide;

namespace Project_B.Controllers {
    public class LogController : ProjectControllerBase {
        [ValidateInput(false)]
        public ActionResult JsError(string message, string target = "", string location = "") {
            var baseModel = GetBaseModel();
            if (!baseModel.GetUserPolicyState<bool>(UserPolicyGlobal.IsBot)) {
                LoggerManager.GetLogger("JavaScriptError")
                    .Info("GuestID {0} ({1}): {2}, in {3}", baseModel.SessionModule.GuestID, TryParseMessage(message), location, target);
            }
            return new EmptyResult();
        }

        public ActionResult Feature(string featureID, string objectID) {
            ProjectBActions feature;
            if (Enum.TryParse(featureID, out feature)) {
                LogAction(feature, StringParser.ToInt(objectID, default(int)));
            }
            return new EmptyResult();
        }
        private static string TryParseMessage(string message) {
            var splitedParams = message.Split('&');
            string result = string.Empty;
            if (splitedParams.Length > 0) {
                foreach (var splitedParam in splitedParams) {
                    var splittedValues = splitedParam.Split('=');
                    if (splittedValues.Length == 2) {
                        result += string.Format("{0}: {1} | ", splittedValues[0], splittedValues[1]);
                    } else {
                        result += splitedParam;
                    }
                }
            } else {
                result = message;
            }
            return HttpUtility.UrlDecode(result);
        }
    }
}