using RouteBeheerBL.Interfaces;
using RouteBeheerBL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Net;

namespace RouteBeheerDL {
    public class FaciltyRepository : IFacilityRepository {
        private readonly string connectionString;
        public FaciltyRepository(string connectionString) {
            this.connectionString = connectionString;
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
                }catch(Exception ex) {
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
                }catch(Exception ex) {
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
