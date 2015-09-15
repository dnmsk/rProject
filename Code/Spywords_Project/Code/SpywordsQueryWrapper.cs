using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using CommonUtils.Code;
using CommonUtils.Core.Logger;

namespace Spywords_Project.Code {
    public class SpywordsQueryWrapper {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof(SpywordsQueryWrapper).FullName);

        private static readonly Random _rnd = new Random();

        private readonly string _login;
        private readonly string _password;
        private readonly WebRequestHelper _requestHelper;
        private readonly TimeSpan _minRequestDelay;
        private static readonly Encoding  _encoding = Encoding.GetEncoding(1251);
        private DateTime _lastQueryTime = DateTime.MinValue;

        public SpywordsQueryWrapper(string login, string password, WebRequestHelper requestHelper, TimeSpan minRequestDelay) {
            _login = login;
            _password = password;
            _requestHelper = requestHelper;
            _minRequestDelay = minRequestDelay;
            Authenticate();
        }

        public string GetDomainsForPhrase(string phrase) {
            return LoadSpywordsContent("http://spywords.ru/sword.php?sword=" + HttpUtility.UrlEncode(phrase, _encoding));
        }

        /*http://spywords.ru/sword.php?do=advSite&word=%EF%EB%E0%F1%F2%E8%EA%EE%E2%FB%E5%20%EE%EA%ED%E0&tot=200*/
        public string GetDomainsForPhraseYandex(string phrase) {
            return LoadSpywordsContent("http://spywords.ru/sword.php?do=advSite&tot=200&?word=" + HttpUtility.UrlEncode(phrase, _encoding));
        }

        public string GetDomainInfo(string domain) {
            return LoadSpywordsContent("http://spywords.ru/sword.php?site=" + DomainExtension.PunycodeDomain(domain));
        }

        /*http://spywords.ru/sword.php?do=advWord&site=oknastroypro.ru&tot=200&page=1*/
        public string GetQueriesForDomain(string domain, int page = 1) {
            return LoadSpywordsContent(string.Format("http://spywords.ru/sword.php?do=advWord&site={0}&tot=200&page={1}", DomainExtension.PunycodeDomain(domain), page));
        }

        private string LoadSpywordsContent(string url) {
            lock (this) {
                _logger.Info("Пошли с запросом " + url);
                var requestDealy = DateTime.Now - _lastQueryTime;
                if (_minRequestDelay > requestDealy) {
                    Thread.Sleep(_minRequestDelay - requestDealy);
                }
                var pageContent = _requestHelper.GetContent(url);
                _lastQueryTime = DateTime.Now;
                _logger.Debug(string.Format("{0}\r\n{1}", url, pageContent.Item2));
                TryLogError(url, pageContent);
                if (IsAuthenticated(pageContent.Item2)) {
                    return pageContent.Item2;
                }
                _logger.Info("Не авторизован" + url);
                Authenticate();
                pageContent = _requestHelper.GetContent(url);

                return pageContent.Item2;
            }
        }

        private static void TryLogError(string msg, Tuple<HttpStatusCode, string> pageContent) {
            if (pageContent.Item1 != HttpStatusCode.OK) {
                _logger.Error("Пришел неверный статус код {0}, {1} \r\n {2}", pageContent.Item1, msg, pageContent.Item2);
            }
        }

        private static bool IsAuthenticated(string pageContent) {
            _logger.Info("Проверяю авторизацию");
            return pageContent.IndexOf("Настройки аккаунта", StringComparison.InvariantCultureIgnoreCase) > 0;
        }

        private void Authenticate() {
            _logger.Info("Авторизуюсь");
            var postData = string.Format("email_address={0}&password={1}&x={2}&y={3}", HttpUtility.UrlEncode(_login), HttpUtility.UrlEncode(_password), _rnd.Next(110), _rnd.Next(35));

            var loginPageContent = _requestHelper.GetContent("http://spywords.ru/login.php");
            TryLogError("GetLoginPage", loginPageContent);
            var loginRequestContent = _requestHelper.GetContent("http://spywords.ru/login.php?action=process", postData);
            /*http://spywords.ru/login.php*/
            TryLogError("SendLogin", loginRequestContent);
            if (!IsAuthenticated(loginRequestContent.Item2)) {
                _logger.Info("Не смогли авторизоваться \r\n {0}", loginPageContent.Item2);
            }
        }
    }
}