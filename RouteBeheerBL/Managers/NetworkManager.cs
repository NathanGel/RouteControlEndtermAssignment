using RouteBeheerBL.Exceptions;
using RouteBeheerBL.Interfaces;
using RouteBeheerBL.Model;
using System.Data.SqlTypes;

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

        public void UpdateNetworkPoint(NetworkPoint point) {
            if (point == null) throw new NetworkException("Network point cannot be null");
            if (point.X == default || point.Y == default) throw new NetworkException("Network point coordinates cannot be default");
            if (point.X <= 0 || point.Y <= 0) throw new InvalidOperationException("Coordinates do not match requirements.");
            repo.UpdateNetworkPoint(point);
        }

        public int AddConnection(Segment segment) {
           
            return repo.AddConnection(segment);
        }

        public void RemoveConnection(Segment segment) {
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
    }
}
