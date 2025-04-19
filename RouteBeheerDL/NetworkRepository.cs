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

        public List<Stretch> ReadNetwork() { //Ik kies hier voor een stretch omdat een stretch points heeft en points facilities.
            List<Stretch> stretches = new();         //Zo hoef ik maar 1 keer op te roepen om alles uit de DB te halen dat ik zoek
            string queryFacilities = "SELECT * FROM Facilities";
            string queryNetworkPoints = "SELECT * FROM NetworkPoints";
            string queryFacilitiesLocations = "SELECT * FROM NetworkPoint_Facilities";
            string queryStretches = "SELECT * FROM Stretches";
            string queryStretchNetworkPoint = "SELECT * FROM Stretch_NetworkPoints";
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand cmd = connection.CreateCommand()) {
                connection.Open();
                cmd.CommandText = queryFacilities;
                List<Facility> facilities = new();
                using (SqlDataReader reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        facilities.Add(new((int)reader["id"], (string)reader["name"]));
                    }
                }

                cmd.CommandText = queryNetworkPoints;
                List<NetworkPoint> points = new();
                using (SqlDataReader reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        points.Add(new((int)reader["id"], (double)reader["x_coordinate"], (double)reader["y_coordinate"]));
                    }
                }

                cmd.CommandText = queryFacilitiesLocations;
                Dictionary<int, List<Facility>> facilitiesLocations = new();
                using (SqlDataReader reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        int LocationIdKey = (int)reader["networkpoint_id"];
                        if(!facilitiesLocations.ContainsKey(LocationIdKey)) // als de dictionary het nog niet bevat schrijf ik het weg en maak ik het lijst object aan
                            facilitiesLocations.Add(LocationIdKey, new List<Facility>());

                        int facilityId = (int)reader["facility_id"];
                        int indexFacility = facilities.FindIndex( fc => fc.Id == facilityId); // facility opzoeken in de reeds verkregen lijst van facilities
                        facilitiesLocations[LocationIdKey].Add(facilities[indexFacility]); // facility toevoegen op basis van de gevonden index
                    }
                }

                foreach(var location in facilitiesLocations) {
                    int indexPoint = points.FindIndex(p => p.Id == location.Key);
                    points[indexPoint].Facilities = location.Value;
                }

                cmd.CommandText = queryStretches;
                using (SqlDataReader reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        stretches.Add(new((int)reader["id"]));
                    }
                }

                cmd.CommandText = queryStretchNetworkPoint;
                using (SqlDataReader reader = cmd.ExecuteReader()) {
                    int previousStretchId = default;
                    int stretchIndex = default;
                    while (reader.Read()) {
                        int stretchId = (int)reader["stretch_id"];
                        int networkPointId = (int)reader["networkpoint_id"];

                        if(stretchId != previousStretchId) // check om niet telkens opnieuw de index van een stretch in de lijst op te zoeken
                            stretchIndex = stretches.FindIndex( st => st.Id == stretchId);

                        int networkpointIndex = points.FindIndex( p => p.Id == networkPointId); //index van point zoeken op basis van id
                        stretches[stretchIndex].NetworkPoints.Add(points[networkpointIndex]); // point toevoegen aan stretch
                    }
                }
            }
            return stretches;
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
