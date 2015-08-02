using System.Web;

namespace Project_R.Code {
    public static class SessionDataManager {
        public static T GetObject<T>() {
            var session = HttpContext.Current.Session;
            var nameObject = typeof (T).ToString();
            return (T)session[nameObject];
        }
        public static T SetObject<T>(T value) {
            var session = HttpContext.Current.Session;
            var nameObject = typeof (T).ToString();
            session[nameObject] = value;
            return value;
        }
    }
}