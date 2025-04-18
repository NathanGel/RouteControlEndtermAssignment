using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RouteBeheerBL.Exceptions {
    public class NetworkInitializationException : Exception {

        public NetworkInitializationException(string? message) : base(message) {
        }

        public NetworkInitializationException(string? message, Exception? innerException) : base(message, innerException) {
        }

    }
}
