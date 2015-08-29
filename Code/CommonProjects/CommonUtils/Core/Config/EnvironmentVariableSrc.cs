using System;
using System.Configuration;

namespace CommonUtils.Core.Config {
    public class EnvironmentVariableSrc : PathSrcBase {
        private readonly string _environmentVariable;

        public EnvironmentVariableSrc() {
            var fromConfig = ConfigurationManager.AppSettings["EnvironmentVariable"];
            _environmentVariable = string.IsNullOrEmpty(fromConfig) ? null : fromConfig;
        }

        public override string GetLocalPathValue {
            get { return Environment.GetEnvironmentVariable(_environmentVariable); }
        }
    }
}