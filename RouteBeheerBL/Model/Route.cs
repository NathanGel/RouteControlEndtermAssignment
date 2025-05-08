using RouteBeheerBL.Exceptions;
using System.Drawing;

namespace RouteBeheerBL.Model {
    public class Route {
        public Route() {
        }
        public Route(string name, List<Segment> segments, List<(NetworkPoint, bool)> stops) {
            Name = name;
            Segments = segments;
            Stops = stops;
        }

        public Route(int id, string name, List<Segment> segments, List<(NetworkPoint, bool)> stops) {
            Id = id;
            Name = name;
            Segments = segments;
            Stops = stops;
        }

        private int _id;
        public int Id { 
            get { return _id; }
            set {
                if (value <= 0) throw new RouteException("Id must be greater than 0");
                _id = value;
            }
        }

        private string _name;
        public string Name { 
            get { return _name; }
            set {
                if (string.IsNullOrWhiteSpace(value)) throw new RouteException("Name Invalid Null");
                if (value.Length < 3) throw new RouteException("Name Invalid Length Must Be Longer Than 3 Characters");
                _name = value;
            }
        }

        private List<Segment> _segments = new();
        public List<Segment> Segments { 
            get { return _segments;}
            set {
                if (value == null || value.Count == 0) throw new RouteException("Segments Invalid Null");
                if (value.Any(s => s == null)) throw new RouteException("Segments Invalid Null");
                _segments = value;
            }
        }

        private List<(NetworkPoint, bool)> _stops = new();
        public List<(NetworkPoint, bool)> Stops {
            get { return _stops; }
            set {
                if (value == null || value.Count == 0) throw new RouteException("Stops Invalid Null");
                if (value.Any(s => s.Item1 == null)) throw new RouteException("Stops Invalid Null");
                if (value.Count < 5) throw new RouteException("Stops Invalid Less Than 5");
                if (value[0].Item2 == false) throw new RouteException("Stops Invalid StartPoint Not A Stop");
                if (value[^1].Item2 == false) throw new RouteException("Stops Invalid EndPoint Not A Stop");
                _stops = value;
            }
        }

        public static double GetDistance(Point p1, Point p2) {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }
    }
}
