using System.Web;
using System.Web.SessionState;

namespace MainLogic.WebFiles {
    public static class SessionDataManager {
        public static T GetObject<T>() {
            var session = GetSessionState();
            if (session == null) {
                return default(T);
            }
            var nameObject = typeof (T).ToString();
            return (T)session[nameObject];
        }
        public static T SetObject<T>(T value) {
            var session = GetSessionState();
            if (session == null) {
                return value;
            }
            var nameObject = typeof (T).ToString();
            session[nameObject] = value;
            return value;
        }

        public static HttpSessionState GetSessionState() {
            var ctx = HttpContext.Current;
            var session = ctx?.Session;
            return session;
        }
    }
}