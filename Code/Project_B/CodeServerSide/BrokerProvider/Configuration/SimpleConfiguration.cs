using System;
using System.Collections.Generic;
using CommonUtils.Core.Logger;

namespace Project_B.CodeServerSide.BrokerProvider.Configuration {
    public class SimpleConfiguration<K, V> : Dictionary<K, V> {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger("SimpleConfiguration");
        
        public new V this[K key] {
            get {
                try {
                    return base[key];
                }
                catch (Exception ex) {
                    _logger.Error("key={0}\r\n{1}", key, ex);
                }
                return default(V);
            }
            set { base[key] = value; }
        }
    }
}