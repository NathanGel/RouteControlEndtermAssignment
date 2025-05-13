using RouteBeheerBL.Model;
namespace RouteBeheerBL.Interfaces {
    public interface IRouteRepository {
        int Add(Route route);
        bool CheckForExistingConnectionsWithinRoutes(NetworkPoint point);
        bool CheckForExistingConnectionsWithinRoutes(Segment segment);
        void Update(Route route);
        void Delete(Route route);
        List<Route> GetAllRoutes();
    }
}
