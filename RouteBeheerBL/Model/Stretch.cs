using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteBeheerBL.Model {
    public class Stretch {
        public Stretch() {
        }

        public Stretch(int id) {
            this.Id = id;
        }

        public Stretch(List<NetworkPoint> points) {
            NetworkPoints = points;
        }

        public Stretch(int id, List<NetworkPoint> points) {
            Id = id;
            NetworkPoints = points;
        }

        public int Id { get; set; }
        public List<NetworkPoint> NetworkPoints { get; set; } = new();

    }
}
