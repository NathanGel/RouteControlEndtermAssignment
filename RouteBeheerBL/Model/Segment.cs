using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteBeheerBL.Model {
    public class Segment {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public int StartId { get; set; }
        public int StopId { get; set; }
    }
}
