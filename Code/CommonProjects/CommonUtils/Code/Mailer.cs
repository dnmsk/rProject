using System;
using System.Net.Mail;
using System.Text;
using CommonUtils.Core.Config;
using CommonUtils.Core.Logger;

namespace CommonUtils.Code {
    public class Mailer {

        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof(Mailer).FullName);

        /// <summary>
        /// Хост для smtp сервера.
        /// </summary>
        protected string _host = "192.168.0.100";

        /// <summary>
        /// Порт для smtp сервера.
        /// </summary>
        protected int _port = 25;

        /// <summary>
        /// Конструктор - создает объект для отправки писем, подключаться будет к localhost:25.
        /// </summary>
        public Mailer() { }

        /// <summary>
        /// Конструктор - создает объект для отправки писем.
        /// </summary>
        /// <param name="host">Хост smtp.</param>
        /// <param name="port">Порт smtp.</param>
        public Mailer(string host, int port) {
            _host = host != string.Empty ? host : _host;
            _port = port != default(int) ? port : _port;
        }

        /// <summary>
        /// Отправка письма.
        /// </summary>
        /// <param name="message">Письмо.</param>
        public void Send(MailMessage message) {
            new SmtpClient(_host, _port).Send(message);
        }

        /// <summary>
        /// Отправка простого письма в html формате, в кодировке Utf8, без проброса исключений выше.
        /// </summary>
        /// <param name="from">Адрес - от кого.</param>
        /// <param name="to">Адрес - кому.</param>
        /// <param name="subject">Тема письма.</param>
        /// <param name="body">Тело письма.</param>
        /// <returns>Возвращает true в случае успешной отправки письма, иначе false.</returns>
        public bool SendSafe(string from, string to, string subject, string body) {
            try {
                if (ConfigHelper.TestMode || !/*ProductionPolicy.IsProduction()*/true) {
                    _logger.Info("Is test mode: {0}\r\n{1}\r\n{2}", to, subject, body);
                    return true;
                }

                Send(new MailMessage(from, to) {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                    HeadersEncoding = Encoding.UTF8,
                    SubjectEncoding = Encoding.UTF8,
                    BodyEncoding = Encoding.UTF8
                });
                _logger.Info(body);
            } catch (Exception e) {
                _logger.Error(e);
                return false;
            }

            return true;
        }
    }
}