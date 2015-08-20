using System.Configuration;

namespace CommonUtils.Core.Config {
    public class ApplicationConfigSrc : PathSrcBase {

        private readonly string _path;
        public ApplicationConfigSrc() {
            _path = ConfigurationManager.AppSettings["ConfigSource"] ?? ".\\Config\\";
        }

        public override string GetLocalPathValue {
            get { return _path; }
        }
    }
}