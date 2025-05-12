using RouteBeheerBL.Model;

namespace RouteBeheerBL.Interfaces {
    public interface INetworkRepository {
        void InitializeNetwork(Dictionary<int, NetworkPoint> networkPoints, Dictionary<int, Facility> facilities, List<Segment> segmenten);
        int AddNetworkPoint(NetworkPoint point);
        void RemoveNetworkPoint(NetworkPoint point);
        bool CheckForExistingConnectionsWithinSegments(NetworkPoint point);
        bool CheckForExistingConnectionsBetweenFacilitiesAndNetworkPoints(Facility facility);
        int AddConnection(Segment segment);
        void RemoveConnection(Segment segment);
        void UpdateNetworkPoint(NetworkPoint point);
        List<NetworkPoint> GetNetworkPoints();
        List<Segment> GetSegments();
        int AddFacility(Facility facility);
        void RemoveFacility(Facility facility);
        void UpdateFacility(Facility facility);
        List<Facility> GetAllFacilities();
    }
}
