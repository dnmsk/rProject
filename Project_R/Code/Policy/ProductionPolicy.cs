using System;
using System.Web;

namespace Project_R.Code.Policy {
    public class ProductionPolicy {
        public static bool IsProduction() {
            return (SessionDataManager.GetObject<ProductionPolicy>()
                   ?? SessionDataManager.SetObject(new ProductionPolicy()))
                   .IsProductionValue;
        }

        private ProductionPolicy() {
            IsProductionValue = SiteConfiguration.ProductionHostName.Equals(HttpContext.Current.Request.Url.Host,
                StringComparison.InvariantCultureIgnoreCase);
        }

        private bool IsProductionValue { get; set; }
    }
}