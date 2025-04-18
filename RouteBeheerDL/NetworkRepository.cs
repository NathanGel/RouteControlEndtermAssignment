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

        public void InitializeNetwork(List<Facility> fs, List<NetworkPoint> nps, List<Stretch> ss, Dictionary<int, int> npMappings, Dictionary<int, int> fMappings) {
            string queryFacility = "INSERT INTO Facilities(name) OUTPUT INSERTED.id VALUES(@name)";
            string queryNetworkPoint = "INSERT INTO NetworkPoints(x_coordinate, y_coordinate) OUTPUT INSERTED.id VALUES(@x, @y)";
            string queryStretch = "INSERT INTO Stretches OUTPUT INSERTED.id DEFAULT VALUES";
            string queryStretchNetworkPoint = "INSERT INTO Stretch_NetworkPoints(stretch_id, networkpoint_id) VALUES(@stretchId, @networkpointId)";
            string queryNetworkPointFacility = "INSERT INTO NetworkPoint_Facilities(networkpoint_id, facility_id) VALUES(@networkpointId, @facilityId)";
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand cmd = connection.CreateCommand()) {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                cmd.Transaction = transaction;
                try {
                    cmd.CommandText = queryFacility;
                    foreach (Facility f in fs) {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@name", f.Name);
                        int id = (int)cmd.ExecuteScalar();
                        fMappings[f.Id] = id; // write away facility and map the new id to the old one in the dictionary
                    }
                    cmd.CommandText = queryNetworkPoint;
                    foreach (NetworkPoint np in nps) {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@x", np.X);
                        cmd.Parameters.AddWithValue("@y", np.Y);
                        int id = (int)cmd.ExecuteScalar();
                        npMappings[np.Id] = id; // write away networkpoint and map the new id to the old one in the dictionary
                    }
                    foreach (Stretch s in ss) {
                        cmd.CommandText = queryStretch;
                        cmd.Parameters.Clear();
                        int id = (int)cmd.ExecuteScalar();
                        foreach (NetworkPoint np in s.NetworkPoints) {
                            Console.WriteLine(id);
                            cmd.CommandText = queryStretchNetworkPoint;
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@stretchId", id);
                            cmd.Parameters.AddWithValue("@networkpointId", npMappings[np.Id]); // use the new id of the networkpoint
                            cmd.ExecuteNonQuery();
                        }
                    }
                    foreach (NetworkPoint np in nps) {
                        cmd.CommandText = queryNetworkPointFacility;
                        foreach (Facility f in np.Facilities) {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@networkpointId", npMappings[np.Id]); // use the new id of the networkpoint
                            cmd.Parameters.AddWithValue("@facilityId", fMappings[f.Id]); // use the new id of the facility
                            cmd.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                } catch (Exception ex) {
                    transaction.Rollback();
                    throw new Exception("InitializeNetwork", ex);
                }
            }
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
        public void AddFacility(Facility facility) {
            string query = "INSERT INTO Facilities(name) VALUES(@name)";
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand cmd = connection.CreateCommand()) {
                try {
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@name", facility.Name);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                } catch (Exception ex) {
                    throw new Exception("AddFacility", ex);
                }
            }
        }

        public List<Facility> GetAllFacilities() {
            string query = "SELECT * FROM Facilities";
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand cmd = connection.CreateCommand()) {
                List<Facility> facilities = new();
                try {
                    cmd.CommandText = query;
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read()) {
                        facilities.Add(new((int)reader["id"], (string)reader["name"]));
                    }
                    return facilities;
                } catch (Exception ex) {
                    throw new Exception("GetAllFacilities", ex);
                }
            }
        }

        public Facility GetFacility(int id) {
            string query = "SELECT * FROM Facilities WHERE id=@id";
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand cmd = connection.CreateCommand()) {
                try {
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    Facility f = new((int)reader["id"], (string)reader["name"]);
                    return f;
                } catch (Exception ex) {
                    throw new Exception("GetFacility", ex);
                }
            }
        }

        public void RemoveFacility(int id) {
            string query = "DELETE FROM Facilities WHERE id=@id AND id IN" +
                "(SELECT f.id FROM Facilities f " +
                "LEFT JOIN NetworkPoint_Facilities npf ON f.id=npf.facility_id " +
                "WHERE id=@id AND npf.facility_id IS NULL)";
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand cmd = connection.CreateCommand()) {
                try {
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                } catch (Exception ex) {
                    throw new Exception("RemoveFacility", ex);
                }
            }

        }

        public void UpdateFacility(Facility facility) {
            string query = "UPDATE Facilities SET name = @name WHERE id=@id";
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand cmd = connection.CreateCommand()) {
                try {
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@id", facility.Id);
                    cmd.Parameters.AddWithValue("@name", facility.Name);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                } catch (Exception ex) {
                    throw new Exception("UpdateFacility", ex);
                }
            }
        }

    }
}
