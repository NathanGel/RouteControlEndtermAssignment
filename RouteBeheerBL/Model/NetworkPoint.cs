using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteBeheerBL.Model {
    public class NetworkPoint {
        public NetworkPoint(int x, int y, List<Facility> facilities) {
            X = x;
            Y = y;
            Facilities = facilities;
        }

        public NetworkPoint(int id, int x, int y, List<Facility> facilities) {
            Id = id;
            X = x;
            Y = y;
            Facilities = facilities;
        }

        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public List<Facility> Facilities { get; set; } = new();
    }
}
