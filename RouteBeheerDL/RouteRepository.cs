using Microsoft.Data.SqlClient;
using RouteBeheerBL.Interfaces;
using RouteBeheerBL.Model;
using System.Drawing;
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
            string queryRouteSegments = "INSERT INTO Route_Segments(route_id, segment_id, sequenceNo, isReverse) VALUES(@routeId, @segmentId, @sequenceNo, @isReverse)";
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
                        cmd.Parameters.AddWithValue("@segmentId", route.Segments[i].Item1.Id);
                        cmd.Parameters.AddWithValue("@sequenceNo", i + 1);
                        cmd.Parameters.AddWithValue("@isReverse", route.Segments[i].Item2);
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

        public bool CheckForExistingConnectionsWithinRoutes(NetworkPoint point) {
            string query = "SELECT COUNT(*) AS count FROM Route_NetworkPoints WHERE networkpoint_id=@id";
            using (SqlConnection connection = new(connectionstring))
            using (SqlCommand cmd = connection.CreateCommand()) {
                try {
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@id", point.Id);
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read() && (int)reader["count"] != 0 ? true : false;
                } catch (SqlException) {
                    throw new ApplicationException("An error occured while checking for existing connections between networkpoints and routes");
                } catch (Exception) {
                    throw new Exception();
                }
            }
        }

        public bool CheckForExistingConnectionsWithinRoutes(Segment segment) {
            string query = "SELECT COUNT(*) AS count FROM Route_Segments WHERE segment_id=@id";
            using (SqlConnection connection = new(connectionstring))
            using (SqlCommand cmd = connection.CreateCommand()) {
                try {
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@id", segment.Id);
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read() && (int)reader["count"] != 0 ? true : false;
                } catch (SqlException) {
                    throw new ApplicationException("An error occured while checking for existing connections between segments and routes");
                } catch (Exception) {
                    throw new Exception();
                }
            }
        }

        public void Delete(Route route) {
            string queryRouteNetworkPoints = "DELETE FROM Route_NetworkPoints WHERE route_id=@routeId";
            string queryRouteSegments = "DELETE FROM Route_Segments WHERE route_id=@routeId";
            string queryRoute = "DELETE FROM Routes WHERE id=@routeId";
            using (SqlConnection connection = new(connectionstring))
            using (SqlCommand cmd = connection.CreateCommand()) {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                cmd.Transaction = transaction;
                try {
                    cmd.CommandText = queryRouteNetworkPoints;
                    cmd.Parameters.AddWithValue("@routeId", route.Id);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = queryRouteSegments;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@routeId", route.Id);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = queryRoute;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@routeId", route.Id);
                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                } catch (SqlException) {
                    transaction.Rollback();
                    throw new ApplicationException("An error occurred while deleting the route.");
                }
            }
        }

        public List<Route> GetAllRoutes() {
            string query = @"
            SELECT 
                r.id AS RouteID,
                r.name AS RouteName,
                rs.sequenceNo AS SegmentSequence,
                rs.isReverse AS IsReverse,
                s.id AS SegmentID,
    
                npStart.id AS StartPointID,
                npStart.x_coordinate AS StartPoint_X,
                npStart.y_coordinate AS StartPoint_Y,
                rnpStart.isStop AS StartPointIsStop,
    
                npStop.id AS StopPointID,
                npStop.x_coordinate AS StopPoint_X,
                npStop.y_coordinate AS StopPoint_Y,
                rnpStop.isStop AS StopPointIsStop
            FROM Routes r
            JOIN Route_Segments rs ON rs.route_id = r.id
            JOIN Segments s ON s.id = rs.segment_id
            JOIN NetworkPoints npStart ON npStart.id = s.start_id
            JOIN NetworkPoints npStop ON npStop.id = s.stop_id
            JOIN Route_NetworkPoints rnpStart ON rnpStart.route_id = r.id AND rnpStart.networkpoint_id = s.start_id
            JOIN Route_NetworkPoints rnpStop ON rnpStop.route_id = r.id AND rnpStop.networkpoint_id = s.stop_id
            ORDER BY r.id, rs.sequenceNo;"; // query om alles van een route te bemachtigen => points/segmenten/facilities

            using (SqlConnection connection = new(connectionstring))
            using (SqlCommand cmd = connection.CreateCommand()) {
                try {
                    List<Route> routes = new();
                    cmd.CommandText = query;
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    int currentRouteId = -1;
                    Route route = null;
                    List<(Segment, bool)> segments = new(); // lijst om de uitgelezen segmenten bij te houden
                    List<(NetworkPoint, bool)> stops = new(); //lijst om de uitgelezen punten bij te houden

                    while (reader.Read()) {
                        int routeId = (int)reader["RouteID"]; // de route id uitlezen

                        if (routeId != currentRouteId) { //indien de route id niet dezelfde is als de huidige id schrijven we de route weg en beginnen we aan een nieuwe
                            currentRouteId = routeId;
                            if (route != null) {
                                route.Segments = segments;
                                route.Stops = stops;
                                routes.Add(new Route(route.Id, route.Name, [.. route.Segments], [.. route.Stops]));
                                segments.Clear();
                                stops.Clear();
                            }

                            route = new Route { // de nieuwe route aanmaken met volgende id en naam
                                Id = routeId,
                                Name = (string)reader["RouteName"]
                            };
                        }

                        NetworkPoint npStart = new((int)reader["StartPointID"], (double)reader["StartPoint_X"], (double)reader["StartPoint_Y"]); //het startpunt van het segment
                        NetworkPoint npStop = new((int)reader["StopPointID"], (double)reader["StopPoint_X"], (double)reader["StopPoint_Y"]); //het eindpunt van het segment
                        Segment segment = new((int)reader["SegmentID"], npStart, npStop); //het segment aanmaken met start en stop
                        bool isReverse = (bool)reader["IsReverse"]; //uitlezen of het over een omgedraaid segment gaat

                        NetworkPoint first = isReverse ? npStop : npStart; //indien omgedraaid wissel ik het start en eindpunt even om
                        NetworkPoint second = isReverse ? npStart : npStop;

                        bool firstIsStop = isReverse ? (bool)reader["StopPointIsStop"] : (bool)reader["StartPointIsStop"]; //ik pas de boolean waarde van stops aan op basis van of het over een reverse gaat of niet
                        bool secondIsStop = isReverse ? (bool)reader["StartPointIsStop"] : (bool)reader["StopPointIsStop"];

                        if (stops.Count == 0 || stops[^1].Item1.Id != first.Id) //ik voeg ze toe op basis van welke eerst komt in het segment. Indien ik dit niet doe komen er problemen met de requirements van een route
                            stops.Add((first, firstIsStop));
                        if (stops.Count == 0 || stops[^1].Item1.Id != second.Id)
                            stops.Add((second, secondIsStop));

                        segments.Add((segment, isReverse));
                    }

                    if (route != null) { //in geval van de laatste route deze nog toevoegen
                        route.Segments = segments;
                        route.Stops = stops;
                        routes.Add(new Route(route.Id, route.Name, [.. route.Segments], [.. route.Stops]));
                    }

                    return routes;
                } catch (SqlException ex) {
                    throw new ApplicationException("An error occurred while retrieving all routes.", ex);
                }
            }
        }

        public void Update(Route route) {
            throw new NotImplementedException();
        }
    }
}
