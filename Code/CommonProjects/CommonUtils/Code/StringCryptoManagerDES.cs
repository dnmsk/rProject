using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;

namespace CommonUtils.Code {
    public class StringCryptoManagerDES : IDisposable {
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof(StringCryptoManagerDES).FullName);
        private readonly ICryptoTransform _encryptorStream;
        private readonly ICryptoTransform _decryptorStream;

        public StringCryptoManagerDES(string key = null) {
            if (key.IsNullOrEmpty()) {
                key = "EmptyKey";
            }
            var bytes = Encoding.ASCII.GetBytes(key);
            _encryptorStream = new DESCryptoServiceProvider().CreateEncryptor(bytes, bytes);
            _decryptorStream = new DESCryptoServiceProvider().CreateDecryptor(bytes, bytes);
        }

        public string EncryptString(string str) {
            try {
                return HttpUtility.UrlEncode(EncryptByte(Encoding.UTF8.GetBytes(str)));
            } catch (Exception ex) {
                _logger.Error(ex);
            }
            return string.Empty;
        }

        public string DecryptString(string str) {
            try {
                return Encoding.UTF8.GetString(DecryptBytes(HttpUtility.UrlDecode(str)));
            } catch (Exception ex) {
                _logger.Error(ex);
            }
            return string.Empty;
        }

        private string EncryptByte(byte[] array) {
            try {
                return Convert.ToBase64String(EncryptContent(array));
            } catch (Exception ex) {
                _logger.Error(ex);
            }
            return string.Empty;
        }

        private byte[] DecryptBytes(string str) {
            try {
                return DecriptString(Convert.FromBase64String(str));
            } catch (Exception ex) {
                _logger.Info(string.Format("String: {0}", str), ex);
            }
            return new byte[0];
        }
        
        private byte[] EncryptContent(byte[] content) {
            using (var memoryStream = new MemoryStream()) {
                using (var cryptoStream = new CryptoStream(memoryStream, _encryptorStream, CryptoStreamMode.Write)) {
                    cryptoStream.Write(content, 0, content.Length);
                }
                return memoryStream.ToArray();
            }
        }
        
        private byte[] DecriptString(byte[] content) {
            using (var memoryStream = new MemoryStream()) {
                using (var cryptoStream = new CryptoStream(memoryStream, _decryptorStream, CryptoStreamMode.Write)) {
                    cryptoStream.Write(content, 0, content.Length);
                }
                return memoryStream.ToArray();
            }
        }

        public void Dispose() {
            _encryptorStream.Dispose();
            _decryptorStream.Dispose();
        }
    }
}
