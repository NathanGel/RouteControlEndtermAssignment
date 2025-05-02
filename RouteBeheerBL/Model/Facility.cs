using RouteBeheerBL.Exceptions;

namespace RouteBeheerBL.Model {
    public class Facility {
        public Facility(string name) {
            Name = name;
        }

        public Facility(int id, string name) {
            Id = id;
            Name = name;
        }
        private int _id;
        public int Id {
            get {return _id;}
            set {
                if (value <= 0) throw new NetworkException("Id Invalid");
                _id = value;
            }
        }
        private string _name;
        public string Name { 
            get {  return _name;}
            set {
                if (string.IsNullOrWhiteSpace(value)) throw new NetworkException("Name Invalid");
                _name = value;
            }
        }
    }
}
