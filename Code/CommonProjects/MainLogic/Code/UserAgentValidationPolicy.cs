using System;
using System.Collections.Generic;
using System.Linq;

namespace MainLogic.Code {
    public static class UserAgentValidationPolicy {
        /// <summary>
        /// Гуид ботов
        /// </summary>
        public const int BOT_GUID = -1;

        public static readonly List<string> BotNames = new List<string> {
            "bot",
            "NewRelicPinger",
            "YandexMetrika/",
            "YandexAntivirus/",
            "YaDirectFetcher/",
            "Nigma.ru/",
            "http://itrack.ru/cmsrate/",
            "LoadImpactPageAnalyzer/",
            "magpie-crawler/",
            "MailRuConnect/",
            "JoeDog/",
            "LoadImpactRload/",
            "W3C_Validator/",
            "YandexDirect/",
            "vkShare;",
            "facebookexternalhit/",
            "Baiduspider/",
            "Web-Monitoring/",
            "UnwindFetchor/",
            "Microsoft-WebDAV-MiniRedir",
            "Scrapy",
            "Wotbox",
            "URLGrabber",
            "IE6",
            "MSIE 4",
            "MSIE 5",
            "MSIE 6",
            "MSIE 7",
            "MSIE 8",
            "WebIndex",
            "Subscribe.ru",
            "NetLyzer",
            "site-verification",
            "curl/",
            "skypeuripreview",
            "favicon",
            "slurp",
            "crawler",
            "python-urllib",
            "http-client",
            "package http",
            "google web preview",
        };

        /// <summary>
        /// Является ли данный юзер агент - ботом
        /// </summary>
        public static bool IsBot(string userAgent) {
            var isBot = string.IsNullOrEmpty(userAgent);
            if (!isBot && BotNames.Any(botName => userAgent.IndexOf(botName, StringComparison.OrdinalIgnoreCase) >= 0)) {
                isBot = true;
            }
            return isBot;
        }
    }
}
