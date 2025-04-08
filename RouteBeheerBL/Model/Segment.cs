using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteBeheerBL.Model {
    public class Segment {
        public Segment(int routeId, NetworkPoint startPoint, NetworkPoint endPoint) {
            RouteId = routeId;
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public int Id { get; set; }
        public int RouteId { get; set; }
        public NetworkPoint StartPoint { get; set; }
        public NetworkPoint EndPoint { get; set; }
    }
}
