using RouteBeheerBL.Exceptions;
using System.Drawing;

namespace RouteBeheerBL.Model {
    public class Route {
        public Route() {
        }
        public Route(string name, List<(Segment, bool)> segments, List<(NetworkPoint, bool)> stops) {
            Name = name;
            Segments = segments;
            Stops = stops;
        }

        public Route(int id, string name, List<(Segment, bool)> segments, List<(NetworkPoint, bool)> stops) {
            Id = id;
            Name = name;
            Segments = segments;
            Stops = stops;
        }

        private int _id;
        public int Id { 
            get { return _id; }
            set {
                if (value <= 0) throw new RouteException("Id must be greater than 0"); //een id kan niet kleiner of gelijk zijn dan 0 ivm DB identity spec
                _id = value;
            }
        }

        private string _name;
        public string Name { 
            get { return _name; }
            set {
                if (string.IsNullOrWhiteSpace(value)) throw new RouteException("Name Invalid Null");
                if (value.Length < 3) throw new RouteException("Name Invalid Length Must Be Longer Than 3 Characters"); //naam mag niet minder dan 3 karakters bevatten
                _name = value;
            }
        }

        private List<(Segment, bool)> _segments = new();
        public List<(Segment, bool)> Segments { 
            get { return _segments;}
            set {
                if (value == null || value.Count == 0) throw new RouteException("Segments Invalid Null"); //segmenten mogen niet leeg of 0 segmenten bevatten
                if (value.Any(s => s.Item1 == null)) throw new RouteException("Segments Includes Null Value"); // er mag geen segment in segmenten zitten dat null is
                _segments = value;
            }
        }

        private List<(NetworkPoint, bool)> _stops = new();
        public List<(NetworkPoint, bool)> Stops {
            get { return _stops; }
            set {
                if (value == null || value.Count == 0) throw new RouteException("Stops Invalid Null"); //stops mogen niet leeg of null zijn
                if (value.Any(s => s.Item1 == null)) throw new RouteException("Stops Invalid Null"); //er mag geen networkpoint in stops zitten dat null is
                if (value.Count < 5) throw new RouteException("Stops Invalid Less Than 5"); //er moeten minstens 5 networkpoints in de route zitten
                if (value[0].Item2 == false) throw new RouteException("Stops Invalid StartPoint Not A Stop"); //het startpunt van een route moet een stopplaats zijn
                if (value[^1].Item2 == false) throw new RouteException("Stops Invalid EndPoint Not A Stop"); //het eindpunt van een route moet een stopplaats zijn
                _stops = value;
            }
        }

        public static double GetDistance(NetworkPoint p1, NetworkPoint p2) {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2)); //method om de afstand tussen punten te berekenen
        }
    }
}
