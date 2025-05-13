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
            if (route == null) throw new RouteException("Route invalid cannot be null");
            return _repo.Add(route);
        }

        public void UpdateRoute(Route route) {
            _repo.Update(route);
        }

        public void DeleteRoute(Route route) {
            _repo.Delete(route);
        }

        public List<Route> GetAllRoutes() {
            return _repo.GetAllRoutes();
        }
    }
}
