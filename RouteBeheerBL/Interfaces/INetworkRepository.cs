using RouteBeheerBL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteBeheerBL.Interfaces {
    public interface INetworkRepository {
        int AddNetworkPoint(NetworkPoint point);
        void RemoveNetworkPoint(NetworkPoint point);
        void ConnectNetworkPoint(NetworkPoint p1, NetworkPoint p2);
        void DisconnectNetworkPoint(NetworkPoint p1, NetworkPoint p2);
        void UpdateNetworkPoint(NetworkPoint point);
        List<NetworkPoint> GetNetworkPoints();
        List<Segment> GetSegments();
    }
}
