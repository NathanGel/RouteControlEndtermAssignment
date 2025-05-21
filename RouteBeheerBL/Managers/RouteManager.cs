using RouteBeheerBL.Interfaces;
using RouteBeheerBL.Model;
using RouteBeheerBL.Exceptions;

namespace RouteBeheerBL.Managers {
    public class RouteManager {
        private IRouteRepository _repo;
        public RouteManager(IRouteRepository repo) {
            _repo = repo;
        }

        public int AddRoute(Route route) {
            if (_repo.DoesRouteNameExist(route.Name, route.Id)) throw new InvalidOperationException("Route name already exists");
            if (route == null) throw new RouteException("Route invalid cannot be null");
            return _repo.Add(route);
        }

        public void UpdateRoute(Route route) {
            if (_repo.DoesRouteNameExist(route.Name, route.Id)) throw new InvalidOperationException("Route name already exists");
            if (route == null) throw new RouteException("Route invalid cannot be null");
            _repo.Update(route);
        }

        public void DeleteRoute(Route route) {
            if (route == null) throw new InvalidOperationException();
            _repo.Delete(route);
        }

        public List<Route> GetAllRoutes() {
            return _repo.GetAllRoutes();
        }
    }
}
