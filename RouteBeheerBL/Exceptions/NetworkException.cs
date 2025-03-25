using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RouteBeheerBL.Exceptions {
    public class NetworkException : Exception {
        public NetworkException(string? message) : base(message) {
        }
        public NetworkException(string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}
