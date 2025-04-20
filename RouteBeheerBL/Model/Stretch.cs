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

        public Stretch(Dictionary<int, NetworkPoint> networkPointSequence) {
            NetworkPointSequence = networkPointSequence;
        }

        public Stretch(int id, List<NetworkPoint> points) {
            Id = id;
            NetworkPoints = points;
        }

        public int Id { get; set; }
        public List<NetworkPoint> NetworkPoints { get; set; } = new();
        public Dictionary<int, NetworkPoint> NetworkPointSequence { get; set; } = new();

    }
}
