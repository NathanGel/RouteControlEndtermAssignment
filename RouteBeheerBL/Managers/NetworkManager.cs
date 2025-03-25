using RouteBeheerBL.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RouteBeheerBL.Model;

namespace RouteBeheerBL.Managers {
    public class NetworkManager {
        private INetworkRepository repo;

        public NetworkManager(INetworkRepository repo) {
            this.repo = repo;
        }

    }
}
