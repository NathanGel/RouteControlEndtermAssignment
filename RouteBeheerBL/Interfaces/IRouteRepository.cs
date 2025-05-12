using RouteBeheerBL.Model;
namespace RouteBeheerBL.Interfaces {
    public interface IRouteRepository {
        int Add(Route route);
        bool CheckForExistingConnectionsWithinRoutes(NetworkPoint point);
        bool CheckForExistingConnectionsWithinRoutes(Segment segment);
        void Update(Route route);
        void Delete(int routeId);
        List<Route> GetAllRoutes();
    }
}
