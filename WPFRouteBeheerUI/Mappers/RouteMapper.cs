using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RouteBeheerBL.Model;
using WPFRouteBeheerUI.Model;

namespace WPFRouteBeheerUI.Mappers {
    public static class RouteMapper {
        public static RouteUI MapFromDomain(Route route) {
            return new RouteUI(route.Id, route.Name, route.Segments, route.Stops);
        }
        public static Route MapToDomain(RouteUI routeUI) {
            return new Route(routeUI.Id, routeUI.Name, routeUI.Segments, routeUI.Stops);
        }
        public static Route MapToDomainWithoutId(RouteUI routeUI) {
            return new Route(routeUI.Name, routeUI.Segments, routeUI.Stops);
        }
    }
}
