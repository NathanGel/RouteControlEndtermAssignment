using RouteBeheerBL.Interfaces;
using RouteBeheerBL.Model;

namespace RouteBeheerBL.Managers {
    public class RouteManager {
        private IRouteRepository _repo;
        public RouteManager(IRouteRepository repo) {
            _repo = repo;
        }

        public int AddRoute(Route route) {
            return _repo.Add(route);
        }

        public void UpdateRoute(Route route) {
            _repo.Update(route);
        }

        public void DeleteRoute(int routeId) {
            _repo.Delete(routeId);
        }

        public List<Route> GetAllRoutes() {
            return _repo.GetAll();
        }
    }
}
