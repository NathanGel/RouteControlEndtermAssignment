using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RouteBeheerBL.Model;
using WPFRouteBeheerUI.Model;
using System.Collections.ObjectModel;

namespace WPFRouteBeheerUI.Mappers {
    public static class RouteMapper {
        public static RouteUI MapFromDomain(Route route) {
            ObservableCollection <(Segment, bool)>  segments = new(route.Segments);
            ObservableCollection<(NetworkPoint, bool)> stops = new(route.Stops);
            return new RouteUI(route.Id, route.Name, segments, stops);
        }
        public static Route MapToDomain(RouteUI routeUI) {
            return new Route(routeUI.Id, routeUI.Name, routeUI.Segments.ToList(), routeUI.Stops.ToList());
        }
        public static Route MapToDomainWithoutId(RouteUI routeUI) {
            return new Route(routeUI.Name, routeUI.Segments.ToList(), routeUI.Stops.ToList());
        }
    }
}
