using System;
using System.Net;
using System.Web.Mvc;
using MainLogic.WebFiles;
using MainLogic.WebFiles.UserPolicy;
using Project_B.CodeClientSide.Enums;

namespace Project_B.CodeClientSide {
    public class CheckCredentialAttribute : ActionFilterAttribute {
        private readonly Enum _policyName;
        private readonly bool _successValue;

        private CheckCredentialAttribute(Enum policyName, bool successValue) {
            _policyName = policyName;
            _successValue = successValue;
        }
        public CheckCredentialAttribute(UserPolicyLocal policyName, bool successValue) : this((Enum)policyName, successValue) {}
        public CheckCredentialAttribute(UserPolicyGlobal policyName, bool successValue) : this((Enum)policyName, successValue) { }

        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            var controller = filterContext.Controller as ApplicationControllerBase;
            if (controller == null || controller.GetBaseModel().GetUserPolicyState<bool>(_policyName) != _successValue) {
                filterContext.Result = new HttpNotFoundResult();
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}