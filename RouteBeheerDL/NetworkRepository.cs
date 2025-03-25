using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RouteBeheerBL.Interfaces;

namespace RouteBeheerDL {
    public class NetworkRepository : INetworkRepository {
        private readonly string connectionString;

        public NetworkRepository(string connectionString) {
            this.connectionString = connectionString;
        }
    }
}
