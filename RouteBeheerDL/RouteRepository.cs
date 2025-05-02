using RouteBeheerBL.Interfaces;

namespace RouteBeheerDL {
    public class RouteRepository : IRouteRepository{
        private readonly string connectionstring;

        public RouteRepository(string connectionstring) {
            this.connectionstring = connectionstring;
        }
    }
}
