using Microsoft.Data.SqlClient;
using RouteBeheerBL.Interfaces;
using RouteBeheerBL.Model;
using System.Globalization;

namespace RouteBeheerDL {
    public class RouteRepository : IRouteRepository{
        private readonly string connectionstring;

        public RouteRepository(string connectionstring) {
            this.connectionstring = connectionstring;
        }

        public int Add(Route route) {
            string queryRoute = "INSERT INTO Routes(name) OUTPUT INSERTED.id VALUES(@name)";
            string queryRouteSegments = "INSERT INTO Route_Segments(route_id, segment_id, sequenceNo) VALUES(@routeId, @segmentId, @sequenceNo)";
            string queryRouteNetworkPoints = "INSERT INTO Route_NetworkPoints(route_id, networkpoint_id, isStop) VALUES(@routeId, @networkpointId, @isStop)";
            using (SqlConnection connection = new(connectionstring))
            using (SqlCommand cmd = connection.CreateCommand()) {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                cmd.Transaction = transaction;
                try {
                    cmd.CommandText = queryRoute;
                    cmd.Parameters.AddWithValue("@name", route.Name);
                    int routeId = (int)cmd.ExecuteScalar();

                    cmd.CommandText = queryRouteSegments;
                    cmd.Parameters.Clear();
                    for (int i = 0; i<route.Segments.Count - 1; i++) {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@routeId", routeId);
                        cmd.Parameters.AddWithValue("@segmentId", route.Segments[i].Id);
                        cmd.Parameters.AddWithValue("@sequenceNo", i + 1);
                        cmd.ExecuteNonQuery();
                    }

                    cmd.CommandText = queryRouteNetworkPoints;
                    cmd.Parameters.Clear();
                    foreach (var point in route.Stops) {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@routeId", routeId);
                        cmd.Parameters.AddWithValue("@networkpointId", point.Item1.Id);
                        cmd.Parameters.AddWithValue("@isStop", point.Item2);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                } catch (SqlException) {
                    transaction.Rollback();
                    throw new ApplicationException();
                }
            }
            return default;
        }

        public void Delete(int routeId) {
            string queryRouteSegments = "DELETE FROM Route_Segments WHERE route_id=@routeId";
            string queryRouteNetworkPoints = "DELETE FROM Route_NetworkPoints WHERE route_id=@routeId";
            string queryRoute = "DELETE FROM Routes WHERE id=@routeId";
            using (SqlConnection connection = new(connectionstring))
            using (SqlCommand cmd = connection.CreateCommand()) {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                cmd.Transaction = transaction;
                try {
                    cmd.CommandText = queryRouteSegments;
                    cmd.Parameters.AddWithValue("@routeId", routeId);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = queryRouteNetworkPoints;
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = queryRoute;
                    cmd.ExecuteNonQuery();

                    transaction.Commit();

                } catch (SqlException) {
                    transaction.Rollback();
                    throw new ApplicationException();
                }
            }
        }

        public List<Route> GetAllRoutes() {
            throw new NotImplementedException();
        }

        public void Update(Route route) {
            throw new NotImplementedException();
        }
    }
}
