using RouteBeheerBL.Exceptions;
using RouteBeheerBL.Interfaces;
using RouteBeheerBL.Model;

namespace RouteBeheerBL.Managers {
    public class NetworkManager {
        private INetworkRepository repo;

        public NetworkManager(INetworkRepository repo) {
            this.repo = repo;
        }

        public int AddNetworkPoint(NetworkPoint point) {
            if (point == null) throw new NetworkException("Network point cannot be null");
            if(point.X == default || point.Y == default) throw new NetworkException("Network point coordinates cannot be default");
            return repo.AddNetworkPoint(point);
        }

        public void RemoveNetworkPoint(NetworkPoint point) {
            if (point == null) throw new NetworkException("Network point cannot be null");
            repo.RemoveNetworkPoint(point);
        }

        public int AddConnection(Segment segment) {
           
            return repo.AddConnection(segment);
        }

        public void DisconnectNetworkPoint(NetworkPoint p1, NetworkPoint p2) {
            if (p1 == null || p2 == null) throw new NetworkException("Network points cannot be null");
            if (p1.X == default || p1.Y == default || p2.X == default || p2.Y == default) throw new NetworkException("Network point coordinates cannot be default");
            repo.DisconnectNetworkPoint(p1, p2);
        }

        public void UpdateNetworkPoint(NetworkPoint point) {
            if (point == null) throw new NetworkException("Network point cannot be null");
            if (point.X == default || point.Y == default) throw new NetworkException("Network point coordinates cannot be default");
            repo.UpdateNetworkPoint(point);
        }

        public List<NetworkPoint> GetNetworkPoints() {
            return repo.GetNetworkPoints();
        }

        public List<Segment> GetSegments() {
            return repo.GetSegments();
        }

        public int AddFacility(Facility facility) {
            if (facility == null) throw new NetworkException("AddFacility Invalid Null");
            return repo.AddFacility(facility);
        }

        public void RemoveFacility(Facility facility) {
            if (facility == null) throw new NetworkException("RemoveFacility Invalid Null");
            repo.RemoveFacility(facility);
        }

        public void UpdateFacility(Facility facility) {
            if (facility == null) throw new NetworkException("UpdateFacility Invalid Null");
            repo.UpdateFacility(facility);
        }

        public void GetFacility(int id) {
            if (id <= 0) throw new NetworkException("UpdateFacility Invalid Null");
            repo.GetFacility(id);
        }

        public List<Facility> GetAllFacilities() {
            return repo.GetAllFacilities();
        }

        public bool CheckForExistingConnectionsFacility(Facility facility) {
            if (facility == null) throw new NetworkException("CheckForExistingConnectionsFacility Invalid Null");
            return repo.CheckForExistingConnectionsFacility(facility);
        }
    }
}
