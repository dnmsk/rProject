using System;
using System.Web;
using System.Web.Script.Serialization;
using CommonUtils.Core.Logger;

namespace MainLogic.WebFiles {
    public class SessionModule {
        private static readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof(SessionModule).FullName);

        public int AccountID { get; set; }

        public int GuestID { get; set; }

        public SessionModule() {}

        public SessionModule(int guestID) : this(guestID, default(int)) {}

        public SessionModule(int guestID, int accountID) {
            AccountID = accountID;
            GuestID = guestID;
        }

        public bool IsAuthenticated() {
            return AccountID != default(int);
        }

        public string ModuleToString() {
            return _serializer.Serialize(this);
        }

        public static SessionModule CreateSessionModule(int guestID, HttpContextBase httpContext) {
            if (httpContext.User == null) {
                return new SessionModule(guestID);
            }

            if (httpContext.User.Identity.IsAuthenticated) {
                try {
                    return Deserialize(httpContext.User.Identity.Name);
                } catch (ArgumentException ex) {
                    _logger.Error(ex);
                }
            }
            return new SessionModule(guestID);
        }

        public static SessionModule Deserialize(string name) {
            var sessionModule = _serializer.Deserialize<SessionModule>(name);
            return sessionModule;
        }
    }
}
