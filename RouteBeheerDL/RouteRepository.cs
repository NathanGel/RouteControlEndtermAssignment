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
            int routeId;
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
                    routeId = (int)cmd.ExecuteScalar();

                    cmd.CommandText = queryRouteSegments;
                    cmd.Parameters.Clear();
                    for (int i = 0; i<route.Segments.Count; i++) {
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
            return routeId;
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
            string query = "SELECT r.id AS RouteID,r.name AS RouteName, " +
                           "rs.sequenceNo AS SegmentSequence, " +
                           "s.id AS SegmentID, " +
                           "npStart.id AS StartPointID, npStart.x_coordinate AS StartPoint_X, npStart.y_coordinate AS StartPoint_Y, rnpStart.isStop AS StartPointIsStop, " +
                           "npStop.id AS StopPointID, npStop.x_coordinate AS StopPoint_X, npStop.y_coordinate AS StopPoint_Y, rnpStop.isStop AS StopPointIsStop " +
                           "FROM Routes r " +
                           "JOIN Route_Segments rs ON rs.route_id = r.id " +
                           "JOIN Segments s ON s.id = rs.segment_id " +
                           "JOIN NetworkPoints npStart ON npStart.id = s.start_id " +
                           "JOIN NetworkPoints npStop ON npStop.id = s.stop_id " +
                           "JOIN Route_NetworkPoints rnpStart ON rnpStart.route_id = r.id " +
                           "AND rnpStart.networkpoint_id = s.start_id " +
                           "JOIN Route_NetworkPoints rnpStop ON rnpStop.route_id = r.id " +
                           "AND rnpStop.networkpoint_id = s.stop_id " +
                           "ORDER BY r.id, rs.sequenceNo;";
            using (SqlConnection connection = new(connectionstring))
            using (SqlCommand cmd = connection.CreateCommand()) {
                try {
                    List<Route> routes = new ();
                    cmd.CommandText = query;
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    int currentRouteId = -1;
                    Route route = null;
                    List<Segment> segments = new ();
                    List < (NetworkPoint, bool)> stops = new ();
                    HashSet<int> addedStops = new();
                    while (reader.Read()) {
                        if ((int)reader["RouteID"] != currentRouteId) {
                            currentRouteId = (int)reader["RouteId"];
                            if (route != null) {
                                route.Segments = segments;
                                route.Stops = stops;
                                routes.Add(new Route(route.Id, route.Name, [.. route.Segments], [.. route.Stops]));
                                route = null;
                                segments.Clear();
                                stops.Clear();
                            }
                            route = new();
                            route.Id = currentRouteId;
                            route.Name = (string)reader["RouteName"];
                        }
                        NetworkPoint start = new NetworkPoint((int)reader["StartPointID"], (double)reader["StartPoint_X"], (double)reader["StartPoint_Y"]);
                        NetworkPoint stop = new NetworkPoint((int)reader["StopPointID"], (double)reader["StopPoint_X"], (double)reader["StopPoint_Y"]);
                        
                        if (addedStops.Add(start.Id))
                            stops.Add((start, (bool)reader["StartPointIsStop"]));
                        if (addedStops.Add(stop.Id))
                            stops.Add((stop, (bool)reader["StopPointIsStop"]));

                        segments.Add(new Segment((int)reader["SegmentID"], start, stop));
                    }

                    if (route != null) {
                        route.Segments = segments;
                        route.Stops = stops;
                        routes.Add(new Route(route.Id, route.Name, [.. route.Segments], [.. route.Stops]));
                    }

                    return routes;
                } catch (SqlException ex) {
                    throw new ApplicationException("An error occured while retrieving all routes.");
                }
            }
        }

        public void Update(Route route) {
            throw new NotImplementedException();
        }
    }
}
