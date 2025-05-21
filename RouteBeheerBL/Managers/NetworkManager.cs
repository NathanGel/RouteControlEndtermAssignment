using RouteBeheerBL.Exceptions;
using RouteBeheerBL.Interfaces;
using RouteBeheerBL.Model;

namespace RouteBeheerBL.Managers {
    public class NetworkManager {
        private INetworkRepository repo;
        private IRouteRepository routeRepo;

        public NetworkManager(INetworkRepository repo, IRouteRepository routeRepo) {
            this.repo = repo;
            this.routeRepo = routeRepo;
        }

        public int AddNetworkPoint(NetworkPoint point) {
            if (point == null) throw new NetworkException("Network point cannot be null");
            if (repo.CheckForNetworkPointWithSameCoordinates(point)) throw new InvalidOperationException("Cannot add this point because one with the same x and y coordinate already exists");
            if (point.X == default || point.Y == default) throw new NetworkException("Network point coordinates cannot be default");
            if (point.X <= 0 || point.Y <= 0 || point.X > 1000 || point.Y > 1000) throw new NetworkException("Coordinates do not match requirements.");
            return repo.AddNetworkPoint(point);
        }

        public void RemoveNetworkPoint(NetworkPoint point) {
            if (repo.CheckForExistingConnectionsWithinSegments(point)) throw new InvalidOperationException("Cannot delete this point because it's connected to one or more segments");
            if (routeRepo.CheckForExistingConnectionsWithinRoutes(point)) throw new InvalidOperationException("Cannot delete this point because it's connected to one or more routes");
            if (point == null) throw new NetworkException("Network point cannot be null");
            repo.RemoveNetworkPoint(point);
        }

        public void UpdateNetworkPoint(NetworkPoint point, bool coordinatesAltered) {
            if (point == null) throw new NetworkException("Network point cannot be null");
            if (coordinatesAltered)
                if (repo.CheckForNetworkPointWithSameCoordinates(point)) throw new InvalidOperationException("Cannot alter this point because one with the same x and y coordinate already exists");
            if (point.X == default || point.Y == default) throw new NetworkException("Network point coordinates cannot be default");
            if (point.X <= 0 || point.Y <= 0 || point.X > 1000 || point.Y > 1000) throw new NetworkException("Coordinates do not match requirements.");
            repo.UpdateNetworkPoint(point);
        }

        public int AddConnection(Segment segment) {
            if (segment == null) throw new NetworkException("Segment cannot be null");
            if (repo.CheckForSegmentWithSameNetworkPoints(segment)) throw new InvalidOperationException("Cannot add segment because one with the same start and endopoint already exists");
            if (segment.StartPoint.Equals(segment.EndPoint)) throw new NetworkException("Segment start and endpoint cannot be the same");
            return repo.AddConnection(segment);
        }

        public void RemoveConnection(Segment segment) {
            if (routeRepo.CheckForExistingConnectionsWithinRoutes(segment)) throw new InvalidOperationException("Cannot delete this segment because it's connected to one or more routes");
            if (segment == null) throw new NetworkException("Segment cannot be null");
            repo.RemoveConnection(segment);
        }

        public List<NetworkPoint> GetNetworkPoints() {
            return repo.GetNetworkPoints();
        }

        public List<Segment> GetSegments() {
            return repo.GetSegments();
        }

        public int AddFacility(Facility facility) {
            if (repo.DoesFacilityNameExist(facility.Name, 0)) throw new InvalidOperationException("Cannot add facility because one with the same name already exists");
            if (facility == null) throw new NetworkException("AddFacility Invalid Null");
            return repo.AddFacility(facility);
        }

        public void RemoveFacility(Facility facility) {
            if (repo.CheckForExistingConnectionsBetweenFacilitiesAndNetworkPoints(facility)) throw new InvalidOperationException("Cannot remove facility because it's connected to one or more networkpoints");
            if (facility == null) throw new NetworkException("RemoveFacility Invalid Null");
            repo.RemoveFacility(facility);
        }

        public void UpdateFacility(Facility facility) {
            if (repo.DoesFacilityNameExist(facility.Name, facility.Id)) throw new InvalidOperationException("Cannot update facility because one with the same name already exists");
            if (facility == null) throw new NetworkException("UpdateFacility Invalid Null");
            repo.UpdateFacility(facility);
        }

        public List<Facility> GetAllFacilities() {
            return repo.GetAllFacilities();
        }
    }
}
