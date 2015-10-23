using System;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace MainLogic.WebFiles {
    public class ExpectedValuesConstraint : IRouteConstraint {
        private readonly string[] _values;

        public ExpectedValuesConstraint(params string[] values) {
            _values = values;
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection) {
            return _values.Contains(values[parameterName].ToString(), StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
