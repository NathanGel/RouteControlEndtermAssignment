using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RouteBeheerBL.Exceptions {
    public class FacilityException : Exception {
        public FacilityException(string? message) : base(message) {
        }

        public FacilityException(string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}
