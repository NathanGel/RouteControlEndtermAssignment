using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public List<Facility> Facilities { get; set; } = new();
    }
}
