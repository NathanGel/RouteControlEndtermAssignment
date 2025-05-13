using RouteBeheerBL.Exceptions;
namespace RouteBeheerBL.Model {
    public class Segment {
        public Segment( NetworkPoint startPoint, NetworkPoint endPoint) {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public Segment(int id, NetworkPoint startPoint, NetworkPoint endPoint) {
            Id = id;
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        private int _id;
        public int Id { 
            get { return _id; }
            set {
                if (value <= 0) throw new NetworkException("Id invalid equal to or smaller than 0"); //een id kan niet kleiner of gelijk zijn dan 0 ivm DB identity spec
                _id = value;
            }
        }
        private NetworkPoint _startPoint;
        public NetworkPoint StartPoint { 
            get { return _startPoint; }
            set {
                if (value == null) throw new NetworkException("Startpoint cannot be null");
                _startPoint = value;
            }
        }
        private NetworkPoint _endPoint;
        public NetworkPoint EndPoint { 
            get { return _endPoint; }
            set {
                if (value == null) throw new NetworkException("Endpoint cannot be null");
                _endPoint = value;
            }
        }
    }
}
