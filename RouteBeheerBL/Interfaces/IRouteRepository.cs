using RouteBeheerBL.Model;
namespace RouteBeheerBL.Interfaces {
    public interface IRouteRepository {
        int Add(Route route);
        bool CheckForExistingConnectionsWithinRoutes(NetworkPoint point);
        bool CheckForExistingConnectionsWithinRoutes(Segment segment);
        bool DoesRouteNameExist(string name, int excludedId);
        void Update(Route route);
        void Delete(Route route);
        List<Route> GetAllRoutes();
    }
}
