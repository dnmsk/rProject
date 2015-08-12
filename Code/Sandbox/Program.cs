using CommonUtils.Code;
using Project_R.Code;

namespace Sandbox {
    class Program {
        static void Main(string[] args) {
            new Mailer().SendSafe("admin@re-dan.ru", "malhaz@list.ru", "test theme", "test subject");
        }
    }
}
