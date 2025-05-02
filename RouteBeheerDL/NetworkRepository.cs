using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
            string query = "SELECT s.id, s.start_id, s.stop_id, nStart.x_coordinate AS startX, nStart.y_coordinate AS startY, nStop.x_coordinate AS stopX, nStop.y_coordinate AS stopY FROM Segments s JOIN NetworkPoints nStart ON s.start_id=nStart.id JOIN NetworkPoints nStop  ON s.stop_id=nStop.id;";
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand cmd = connection.CreateCommand()) {
                List<Segment> segments = new();
                try {
                    cmd.CommandText = query;
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read()) {
                        segments.Add(new((int)reader["id"], new((int)reader["start_id"], (double)reader["startX"], (double)reader["startY"]), new((int)reader["stop_id"], (double)reader["stopX"], (double)reader["stopY"])));
                    }
                    return segments;
                } catch (Exception ex) {
                    throw new Exception("GetSegments", ex);
                }
            }
        }

        public List<NetworkPoint> GetNetworkPoints() {
            string query = "SELECT * FROM NetworkPoints n LEFT JOIN NetworkPoint_Facilities nf ON n.id = nf.networkpoint_id LEFT JOIN Facilities f ON nf.facility_id = f.id";
            using (SqlConnection connection = new(connectionString))
            using(SqlCommand cmd = connection.CreateCommand()) {
                List<NetworkPoint> networkPoints = new();
                try {
                    cmd.CommandText = query;
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    NetworkPoint np = null;
                    int lastId = -1;
                    while (reader.Read()) {
                        int currentId = (int)reader["id"];

                        if (np == null || currentId != lastId) {
                            np = new NetworkPoint(currentId, (double)reader["x_coordinate"], (double)reader["y_coordinate"]);
                            networkPoints.Add(np);
                            lastId = currentId;
                        }
                        if (reader["facility_id"] == DBNull.Value)
                            continue;
                        else
                            np.Facilities.Add(new Facility((int)reader["facility_id"], (string)reader["name"]));
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

        public void RemoveNetworkPoint(NetworkPoint point) {
            string query = "DELETE FROM NetworkPoints WHERE id=@id";
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand cmd = connection.CreateCommand()) {
                try {
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@id", point.Id);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                } catch (SqlException ex) when (ex.Number == 547) { // dit exception getal is het getal voor een foreign key constraint
                    throw new InvalidOperationException("Cannot delete this point because it's connected to one or more segments"); // deze exception vertaal ik naar een InvalidOperationsException en
                                                                                                                                    // vang ik in de UI op zo weet ik dat het gaat over een bestaande link
                } catch (SqlException) {
                    throw new ApplicationException("An error occured while deleting the networkpoint"); // deze error vertaal ik naae een ApplicationException en bevat eender welke ander
                                                                                                        // sql error en vang ik ook op in de ui
                }
            }
        }

        public void UpdateNetworkPoint(NetworkPoint point) {
            string queryNetworkPoint = "UPDATE NetworkPoints SET x_coordinate = @x, y_coordinate = @y where id = @id";
            string queryClearPreviousFacilities = "DELETE FROM NetworkPoint_Facilities WHERE networkpoint_id = @id";
            string queryUpdatedFacilities = "INSERT INTO NetworkPoint_Facilities(networkpoint_id, facility_id) VALUES(@id, @facilityId)";
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand cmd = connection.CreateCommand()) {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                cmd.Transaction = transaction;
                try {
                    // command en logica om networkpoint aan te passen
                    cmd.CommandText = queryNetworkPoint;
                    cmd.Parameters.AddWithValue("@id", point.Id);
                    cmd.Parameters.AddWithValue("@x", point.X);
                    cmd.Parameters.AddWithValue("@y", point.Y);
                    cmd.ExecuteNonQuery();

                    //command en logica om de oude facilities te clearen
                    cmd.CommandText = queryClearPreviousFacilities;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@id", point.Id);
                    cmd.ExecuteNonQuery();

                    //command en logica om de geupdatete lijst toe te voegen
                    cmd.CommandText = queryUpdatedFacilities;
                    cmd.Parameters.Clear();
                    foreach (var facility in point.Facilities) {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@id", point.Id);
                        cmd.Parameters.AddWithValue("@facilityId", facility.Id);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                } catch (SqlException) {
                    transaction.Rollback();
                    throw new ApplicationException("An error occured while updating the networkpoint");
                }
            }
        }

        public int AddConnection(Segment segment) {
            string querySegment = "INSERT INTO Segments(start_id, stop_id) OUTPUT INSERTED.id VALUES(@startId, @endId)";
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand cmd = connection.CreateCommand()) {
                try {
                    cmd.CommandText = querySegment;
                    cmd.Parameters.AddWithValue("@startId", segment.StartPoint.Id);
                    cmd.Parameters.AddWithValue("@endId", segment.EndPoint.Id);
                    connection.Open();
                    int id = (int)cmd.ExecuteScalar();
                    return id;
                } catch (SqlException ex) {
                    throw new Exception("AddConnection", ex);
                }
            }
        }

        public void RemoveConnection(Segment segment) {
            string query = "DELETE FROM Segments WHERE id=@id";
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand cmd = connection.CreateCommand()) {
                try {
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@id", segment.Id);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                } catch (SqlException ex) when (ex.Number == 547) {
                    throw new InvalidOperationException("Cannot delete segment because it's part of one or more routes");// deze exception vertaal ik naar een InvalidOperationsException en
                                                                                                                         // vang ik in de UI op zo weet ik dat het gaat over een bestaande link
                } catch (SqlException ex) {
                    throw new ApplicationException("An error occured while deleting the segment");// deze error vertaal ik naae een ApplicationException en bevat eender welke ander
                                                                                                  // sql error en vang ik ook op in de ui
                }
            }
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

                } catch (SqlException ex) when (ex.Number == 547) { // // dit exception getal is het getal voor een foreign key constraint
                    throw new InvalidOperationException("Cannot delete this facility because it's connected to one or more networkpoints", ex); // deze exception vertaal ik naar een InvalidOperationsException en
                                                                                                                                                // vang ik in de UI op zo weet ik dat het gaat over een bestaande link
                } catch (SqlException) {
                    throw new ApplicationException("An error occured while deleting the facility"); // deze error vertaal ik naae een ApplicationException en bevat eender welke ander
                                                                                                    // sql error en vang ik ook op in de ui 
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
