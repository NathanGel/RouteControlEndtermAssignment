using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RouteBeheerBL.Interfaces;

namespace RouteBeheerDL {
    public class RouteRepository : IRouteRepository{
        private readonly string connectionstring;

        public RouteRepository(string connectionstring) {
            this.connectionstring = connectionstring;
        }
    }
}
