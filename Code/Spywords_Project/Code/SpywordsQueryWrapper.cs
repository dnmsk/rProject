using System;
using System.Net;
using System.Text;
using System.Web;
using CommonUtils.Code;
using CommonUtils.Core.Logger;

namespace Spywords_Project.Code {
    public class SpywordsQueryWrapper {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof(SpywordsQueryWrapper).FullName);

        private static Random _rnd = new Random();

        private readonly string _login;
        private readonly string _password;
        private static readonly WebRequestHelper _requestHelper = new WebRequestHelper();
        private static readonly Encoding  _encoding = Encoding.GetEncoding(1251);

        public SpywordsQueryWrapper(string login, string password) {
            _login = login;
            _password = password;
            Authenticate();
        }

        public string GetDomainsForPhrase(string phrase) {
            return LoadSpywordsContent("http://spywords.ru/sword.php?sword=" + HttpUtility.UrlEncode(phrase, _encoding));
        }

        public string GetDomainInfo(string domain) {
            return LoadSpywordsContent("http://spywords.ru/sword.php?site=" + HttpUtility.UrlEncode(domain, _encoding));
        }

        private string LoadSpywordsContent(string url) {
            var pageContent = _requestHelper.GetContent(url);
            TryLogError(url, pageContent);
            if (IsAuthenticated(pageContent.Item2)) {
                return pageContent.Item2;
            }
            Authenticate();
            pageContent = _requestHelper.GetContent(url);

            return pageContent.Item2;
        }

        private static void TryLogError(string msg, Tuple<HttpStatusCode, string> pageContent) {
            if (pageContent.Item1 != HttpStatusCode.OK) {
                _logger.Error("Пришел неверный статус код {0}, {1} \r\n {2}", pageContent.Item1, msg, pageContent.Item2);
            }
        }

        private static bool IsAuthenticated(string pageContent) {
            return pageContent.IndexOf("Настройки аккаунта", StringComparison.InvariantCultureIgnoreCase) > 0;
        }

        private void Authenticate() {
            var postData = string.Format("email_address={0}&password={1}&x={2}&y={3}", HttpUtility.UrlEncode(_login), HttpUtility.UrlEncode(_password), _rnd.Next(110), _rnd.Next(35));

            var loginPageContent = _requestHelper.GetContent("http://spywords.ru/login.php");
            TryLogError("GetLoginPage", loginPageContent);
            var loginRequestContent = _requestHelper.GetContent("http://spywords.ru/login.php?action=process", postData);
            /*http://spywords.ru/login.php*/
            TryLogError("SendLogin", loginRequestContent);
            if (!IsAuthenticated(loginRequestContent.Item2)) {
                _logger.Error("Не смогли авторизоваться \r\n {0}", loginPageContent.Item2);
            }
        }
    }
}