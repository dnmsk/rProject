using System;
using System.Web;

namespace MainLogic.WebFiles.Policy {
    public class ProductionPolicy {
        public static bool IsProduction() {
            return (SessionDataManager.GetObject<ProductionPolicy>()
                   ?? SessionDataManager.SetObject(new ProductionPolicy()))
                   .IsProductionValue;
        }

        private ProductionPolicy() {
            var session = SessionDataManager.GetSessionState();
            IsProductionValue = session != null && SiteConfiguration.ProductionHostName.Equals(HttpContext.Current.Request.Url.Host,
                StringComparison.InvariantCultureIgnoreCase);
        }

        private bool IsProductionValue { get; set; }
    }
}