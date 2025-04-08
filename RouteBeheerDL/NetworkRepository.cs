using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RouteBeheerBL.Interfaces;
using RouteBeheerBL.Model;

namespace RouteBeheerDL {
    public class NetworkRepository : INetworkRepository {
        private readonly string connectionString;

        public NetworkRepository(string connectionString) {
            this.connectionString = connectionString;
        }

        public void AddNetworkPoint(NetworkPoint point) {
            throw new NotImplementedException();
        }

        public void ConnectNetworkPoint(NetworkPoint p1, NetworkPoint p2) {
            throw new NotImplementedException();
        }

        public void DisconnectNetworkPoint(NetworkPoint p1, NetworkPoint p2) {
            throw new NotImplementedException();
        }

        public List<NetworkPoint> GetNetworkPoints() {
            throw new NotImplementedException();
        }

        public List<Segment> GetSegments() {
            throw new NotImplementedException();
        }

        public void RemoveNetworkPoint(NetworkPoint point) {
            throw new NotImplementedException();
        }

        public void UpdateNetworkPoint(NetworkPoint point) {
            throw new NotImplementedException();
        }
    }
}
