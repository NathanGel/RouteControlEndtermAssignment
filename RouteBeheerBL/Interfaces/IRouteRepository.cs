using RouteBeheerBL.Model;
namespace RouteBeheerBL.Interfaces {
    public interface IRouteRepository {
        int Add(Route route);
        void Update(Route route);
        void Delete(int routeId);
        List<Route> GetAllRoutes();
    }
}
