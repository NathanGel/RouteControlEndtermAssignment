using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Data.SqlClient;
using RouteBeheerBL.Interfaces;
using RouteBeheerBL.Model;

namespace RouteBeheerDL {
    public class NetworkRepository : INetworkRepository {
        private readonly string connectionString;

        public NetworkRepository(string connectionString) {
            this.connectionString = connectionString;
        }

        public void InitializeNetwork(Dictionary<int, NetworkPoint> networkPoints, Dictionary<int, Facility> facilities, List<Segment> segmenten) {
            string queryNetworkPoints = "INSERT INTO NetworkPoints(x_coordinate, y_coordinate) OUTPUT INSERTED.id VALUES(@X, @Y)";
            string queryFacilities = "INSERT INTO Facilities(name) OUTPUT INSERTED.id VALUES(@name)";
            string queryNetworkPointFacilities = "INSERT INTO NetworkPoint_Facilities(networkpoint_id, facility_id) VALUES(@npId, @fId)";
            string querySegments = "INSERT INTO Segments(start_id, stop_id) VALUES(@startId, @stopId)";
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand cmd = connection.CreateCommand()) {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                cmd.Transaction = transaction;
                try {
                    cmd.CommandText = queryNetworkPoints;
                    for(int i = 1; i<=networkPoints.Count; i++) { // start vanaf 1 want de keys starten vanaf 1
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@X", networkPoints[i].X);
                        cmd.Parameters.AddWithValue("@Y", networkPoints[i].Y);
                        int id = (int)cmd.ExecuteScalar();
                        networkPoints[i].Id = id; // de door de databank gegenereerde id opslaan in het networkpoint voor NetworkPoint_Facilities en Segmenten
                    }

                    cmd.CommandText = queryFacilities;
                    cmd.Parameters.Clear();
                    for(int i = 1; i<=facilities.Count; i++) { // start vanaf 1 want de keys starten vanaf 1
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@name", facilities[i].Name);
                        int id = (int)cmd.ExecuteScalar();
                        facilities[i].Id = id; // de door de databank gegenereerde id opslaan in de facility voor NetworkPoint_Facilities
                    }

                    cmd.CommandText = queryNetworkPointFacilities;
                    cmd.Parameters.Clear();
                    foreach(var networkpoint in networkPoints.Values) {
                        foreach(var facility in networkpoint.Facilities) {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@npId", networkpoint.Id);
                            cmd.Parameters.AddWithValue("@fId", facility.Id);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    cmd.CommandText = querySegments;
                    cmd.Parameters.Clear();
                    foreach (var segment in segmenten) {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@startId", segment.StartPoint.Id);
                        cmd.Parameters.AddWithValue("@stopId", segment.EndPoint.Id);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                } catch (Exception ex) {
                    transaction.Rollback();
                    throw new Exception("InitializeNetwork Invalid Transaction Rolled Back", ex);
                }
            }
        }

        public List<Segment> GetSegments() {
            string query = "SELECT s.start_id, s.stop_id, nStart.x_coordinate AS startX, nStart.y_coordinate AS startY, nStop.x_coordinate AS stopX, nStop.y_coordinate AS stopY FROM Segments s JOIN NetworkPoints nStart ON s.start_id=nStart.id JOIN NetworkPoints nStop  ON s.stop_id=nStop.id;";
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand cmd = connection.CreateCommand()) {
                List<Segment> segments = new();
                try {
                    cmd.CommandText = query;
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read()) {
                        segments.Add(new(new((int)reader["start_id"], (double)reader["startX"], (double)reader["startY"]), new((int)reader["stop_id"], (double)reader["stopX"], (double)reader["stopY"])));
                    }
                    return segments;
                } catch (Exception ex) {
                    throw new Exception("GetSegments", ex);
                }
            }
        }
        public List<NetworkPoint> GetNetworkPoints() {
            string query = "SELECT * FROM NetworkPoints";
            using (SqlConnection connection = new(connectionString))
            using(SqlCommand cmd = connection.CreateCommand()) {
                List<NetworkPoint> networkPoints = new();
                try {
                    cmd.CommandText = query;
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read()) {
                        networkPoints.Add(new((int)reader["id"], (double)reader["x_coordinate"], (double)reader["y_coordinate"]));
                    }
                    return networkPoints;
                } catch (Exception ex) {
                    throw new Exception("GetNetworkPoints", ex);
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


        public void RemoveNetworkPoint(NetworkPoint point) {
            string query = "DELETE FROM NetworkPoints WHERE x_coordinate=@X AND y_coordinate=@Y";
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand cmd = connection.CreateCommand()) {
                try {
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@X", point.X);
                    cmd.Parameters.AddWithValue("@Y", point.Y);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                } catch (Exception ex) {
                    throw new Exception("RemoveNetworkPoint", ex);
                }
            }

        }

        public void UpdateNetworkPoint(NetworkPoint point) {
            throw new NotImplementedException();
        }
        public int AddFacility(Facility facility) {
            string query = "INSERT INTO Facilities(name) OUTPUT INSERTED.id VALUES(@name)";
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand cmd = connection.CreateCommand()) {
                try {
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@name", facility.Name);
                    connection.Open();
                    int id = (int)cmd.ExecuteScalar();
                    return id;
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

        public void RemoveFacility(Facility facility) {
            string query = "DELETE FROM Facilities WHERE id=@id";
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand cmd = connection.CreateCommand()) {
                try {
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@id", facility.Id);
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

        public bool CheckForExistingConnectionsFacility(Facility facility) {
            string query = "SELECT COUNT(*) FROM NetworkPoint_Facilities WHERE facility_id=@id";
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand cmd = connection.CreateCommand()) {
                try {
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@id", facility.Id);
                    connection.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                } catch (Exception ex) {
                    throw new Exception("CheckForExistingConnectionsFacility", ex);
                }
            }
        }
    }
}
