using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading;

namespace CommonUtils.Code {
    public class WebRequestHelper : IDisposable {
        public string Proxy { get; set; }

        /// <summary>
        ///     Юзер-агенты для подставновкив запросы
        /// </summary>
        private static readonly string[] _userAgents = {
            "Mozilla/5.0 (Windows; U; Windows NT 5.2; ru) AppleWebKit/534.10 (KHTML, like Gecko) Chrome/8.0.558.0 Safari/534.10",
            "Mozilla/5.0 (X11; U; CrOS i686 0.9.128; ru) AppleWebKit/534.10 (KHTML, like Gecko) Chrome/8.0.552.343 Safari/534.10",
            "Mozilla/5.0 (Macintosh; U; Intel Mac OS X 10_5_8; ru) AppleWebKit/533.18.1 (KHTML, like Gecko) Version/5.0.2 Safari/533.18.5",
            "Mozilla/5.0 (Windows; U; Windows NT 5.2; ru) AppleWebKit/533.17.8 (KHTML, like Gecko) Version/5.0.1 Safari/533.17.8",
            "Mozilla/5.0 (Windows; U; Windows NT 5.1; ru) AppleWebKit/534.14 (KHTML, like Gecko) Chrome/9.0.600.0 Safari/534.14",
            "Mozilla/5.0 (Windows; U; Windows NT 6.1; ru; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13",
            "Mozilla/5.0 (Windows; U; Windows NT 5.1; ru; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13 GTB7.1 ( .NET CLR 3.5.30729)",
            "Mozilla/5.0 (Windows; U; Windows NT 6.1; ru; rv:1.9.2.12) Gecko/20101026 Firefox/3.6.12 ( .NET CLR 3.5.30729; .NET4.0E)",
            "Mozilla/5.0 (Windows; U; Windows NT 6.0; ru; rv:1.9.2) Gecko/20100115 Firefox/3.6",
            "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:2.0b8pre) Gecko/20101213 Firefox/4.0b8pre",
            "Mozilla/5.0 (Windows; U; Windows NT 6.1; ru; rv:1.9.1.8) Gecko/20100202 Firefox/3.5.8",
            "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; Media Center PC 4.0; SLCC1; .NET CLR 3.0.04320)",
            "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; SLCC1; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET CLR 1.1.4322)",
            "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; Zune 3.0)",
            "Mozilla/5.0 (Windows; U; Windows NT 5.1; ru; rv:1.9) Gecko/2008052906 Firefox/3.0",
            "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.58 Safari/535.2",
            "Mozilla/5.0 (Windows; U; Windows NT 5.1; de; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13",
            "Mozilla/5.0 (X11; Linux i686; rv:6.0.2) Gecko/20100101 Firefox/6.0.2",
            "Mozilla/5.0 (Windows NT 6.1; rv:6.0.2) Gecko/20100101 Firefox/6.0.2",
            "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.835.187 Safari/535.1",
            "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET4.0C; .NET4.0E)",
            "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.835.187 Safari/535.1",
            "MSIE (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 2.0.50727)",
            "Opera/9.80 (Windows NT 5.2; U; zh-cn) Presto/2.9.211 Version/12.00",
            "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.835.187 Safari/535.1",
            "Mozilla/5.0 (Linux; U; Android 2.2; ja-jp; L-04C Build/FRF91) AppleWebKit/533.1 (KHTML,like Gecko) Version/4.0 Mobile Safari/533.1"
        };

        static WebRequestHelper() {
            ServicePointManager.Expect100Continue = false;
        }

        public readonly CookieContainer Cookies;
        public TimeSpan? MinRequestDelay = null;
        private readonly string _userAgent;
        private readonly Action<string, CookieContainer> _onDispose;
        public WebRequestHelper(string userAgent = null, CookieContainer cookies = null, Action<string, CookieContainer> onDispose = null) {
            Proxy = null;
            _userAgent = userAgent ?? RandomUserAgent();
            Cookies = cookies ?? new CookieContainer();
            _onDispose = onDispose;
        }

        private DateTime? _lastQueryTime;
        public Tuple<HttpStatusCode, string> GetContent(string url, string postData = null, string contentType = "application/x-www-form-urlencoded", Encoding encoding = null) {
            if (MinRequestDelay.HasValue) {
                if (!_lastQueryTime.HasValue) {
                    _lastQueryTime = DateTime.MinValue;
                }
                var requestDealy = DateTime.UtcNow - _lastQueryTime.Value;
                if (MinRequestDelay.Value > requestDealy) {
                    Thread.Sleep(MinRequestDelay.Value - requestDealy);
                }
            }
            var webResponse = GetResponse(url, Cookies, postData, encoding, contentType, _userAgent, Proxy);
            return new Tuple<HttpStatusCode, string>(((HttpWebResponse) webResponse).StatusCode, GetContent(webResponse));
        }

        /// <summary>
        ///     Выбор случайного юзер-агента
        /// </summary>
        /// <returns></returns>
        private static string RandomUserAgent() {
            return _userAgents[new Random().Next(_userAgents.Length - 1)];
        }

        public static Tuple<HttpStatusCode, string> GetContentWithStatus(string url, CookieContainer cookies = null) {
            var webResponse = GetResponse(url, cookies);
            return new Tuple<HttpStatusCode, string>(((HttpWebResponse) webResponse).StatusCode, GetContent(webResponse));
        }
        
        private static string GetContent(WebResponse response, Encoding encoding = null) {
            var contentRaw = GetContentRaw(response);
            encoding = encoding ?? EncodingDetector.DetectFromStatistic(contentRaw);
            return encoding.GetString(contentRaw);
        }

        private static byte[] GetContentRaw(WebResponse response) {
            byte[] result;
            Stream stream = null;
            const int BYTE_BLOCK_SIZE = 102400;
            try {
                stream = response.GetResponseStream();
                if ((response.Headers["Content-Encoding"] == "gzip" ||
                     response.Headers["Content-Type"] == "application/x-gzip") && stream.CanRead) {
                    stream = new GZipStream(stream, CompressionMode.Decompress);
                }
                else if (response.Headers["Content-Encoding"] == "deflate" && stream.CanRead) {
                    stream = new DeflateStream(stream, CompressionMode.Decompress);
                }

                var blocks = new LinkedList<byte[]>();
                var count = 0;
                using (var reader = new BinaryReader(stream)) {
                    while (stream.CanRead) {
                        byte[] buff = reader.ReadBytes(BYTE_BLOCK_SIZE);
                        if (buff.Length == 0) {
                            break;
                        }
                        blocks.AddLast(buff);
                        count += buff.Length;
                    }
                }
                result = new byte[count];
                var step = 0;
                foreach (var block in blocks) {
                    Array.Copy(block, 0, result, step, block.Length);
                    step += block.Length;
                }
            }
            finally {
                if (stream != null) {
                    stream.Close();
                }
            }
            return result;
        }

        private static WebResponse GetResponse(string url, CookieContainer cookieContainer = null, string postData = null, Encoding encoding = null, string contentType = "text/xml", string userAgent = null, string proxy = null) {
            var readWriteTimeout = 15000;
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Accept =
                "text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";
            request.Headers["Accept-Language"] = "ru-ru,ru;q=0.5";
            request.Headers["Accept-Encoding"] = "gzip,deflate";
            request.Headers["Accept-Charset"] = "utf-8,windows-1251;q=0.7,*;q=0.7";
            request.KeepAlive = false;
            request.Timeout = readWriteTimeout;
            request.ReadWriteTimeout = readWriteTimeout;
            request.UserAgent = userAgent ?? RandomUserAgent();
            request.AllowAutoRedirect = true;
            request.CookieContainer = cookieContainer ?? new CookieContainer();
            if (proxy != null) {
                request.Proxy = new WebProxy(new Uri(proxy.StartsWith("http") ? proxy : "http://" + proxy));
            }
            if (!string.IsNullOrEmpty(postData)) {
                encoding = encoding ?? Encoding.UTF8;
                request.Method = "POST";
                request.ContentType = contentType;
                var bytes = encoding.GetBytes(postData);
                request.ContentLength = bytes.Length;
                try {
                    Stream stream = request.GetRequestStream();
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Close();
                }
                catch (WebException ex) {
                    return null;
                }
            }

            try {
                return request.GetResponse();
            }
            catch (Exception e) {
                return null;
            }
        }

        public void PushToConfigurationFile() {
            if(_onDispose != null) {
                _onDispose(_userAgent, Cookies);
            }
        }

        public void Dispose() {
            PushToConfigurationFile();
        }
    }
}