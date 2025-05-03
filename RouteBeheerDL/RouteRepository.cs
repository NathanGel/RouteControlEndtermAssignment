using RouteBeheerBL.Interfaces;
using RouteBeheerBL.Model;

namespace RouteBeheerDL {
    public class RouteRepository : IRouteRepository{
        private readonly string connectionstring;

        public RouteRepository(string connectionstring) {
            this.connectionstring = connectionstring;
        }

        public int Add(Route route) {
            throw new NotImplementedException();
        }

        public void Delete(int routeId) {
            throw new NotImplementedException();
        }

        public List<Route> GetAll() {
            throw new NotImplementedException();
        }

        public void Update(Route route) {
            throw new NotImplementedException();
        }
    }
}
