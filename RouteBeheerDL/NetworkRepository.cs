using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using RouteBeheerBL.Interfaces;
using RouteBeheerBL.Model;

namespace RouteBeheerDL {
    public class NetworkRepository : INetworkRepository {
        private readonly string connectionString;

        public NetworkRepository(string connectionString) {
            this.connectionString = connectionString;
        }

        public int AddNetworkPoint(NetworkPoint point) {
            int id;
            string query = "INSERT INTO NetworkPoints(x_coordinate, y_coordinate) OUTPUT INSERTED.ID VALUES(@x, @y)";
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand cmd = connection.CreateCommand()) {
                try {
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@x", point.X);
                    cmd.Parameters.AddWithValue("@y", point.Y);
                    connection.Open();
                    id = (int)cmd.ExecuteScalar();
                } catch (SqlException ex) {
                    throw new Exception("Error adding network point", ex);
                }
            }
            return id;
        }

        public void ConnectNetworkPoint(NetworkPoint p1, NetworkPoint p2) {
            string queryStretch = "INSERT INTO Stretches OUTPUT INSERTED.id";
            string queryStretchNetworkPoint = "INSERT INTO StretchNetworkPoints(stretch_id, networkpoint_id) VALUES(@stretchId, @networkPointId)";

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
