using RouteBeheerBL.Exceptions;
namespace RouteBeheerBL.Model {
    public class NetworkPoint {
        public NetworkPoint(double x, double y) {
            X = x;
            Y = y;
        }

        public NetworkPoint(double x, double y, List<Facility> facilities) {
            X = x;
            Y = y;
            Facilities = facilities;
        }

        public NetworkPoint(int id, double x, double y) {
            Id = id;
            X = x;
            Y = y;
        }

        public NetworkPoint(int id, double x, double y, List<Facility> facilities) {
            Id = id;
            X = x;
            Y = y;
            Facilities = facilities;
        }

        private int _id;
        public int Id {
            get {  return _id; }
            set {
                if (value <= 0) throw new NetworkException("Id invalid equal to or smaller than 0"); //een id kan niet kleiner of gelijk zijn dan 0 ivm DB identity spec
                _id = value;
            }
        }

        private double _x;
        public double X { 
            get { return _x; }
            set {
                if (value < 0 || value > 1000) throw new NetworkException("X Invalid values don't lie within required range"); //coordinaten dienen tussen 0 en 1000 te zitten
                _x = value;
            }
        }

        private double _y;
        public double Y {
            get { return _y; }
            set {
                if (value < 0 || value > 1000) throw new NetworkException("X Invalid values don't lie within required range"); //coordinaten dienen tussen 0 en 1000 te zitten
                _y = value;
            }
        }
        public List<Facility> Facilities { get; set; } = new();

        public override bool Equals(object? obj) {
            if (obj is not NetworkPoint other)
                return false;

            return Id == other.Id;
        }

        public override int GetHashCode() {
            return Id.GetHashCode();
        }
    }
}
