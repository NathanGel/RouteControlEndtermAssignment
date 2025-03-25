using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteBeheerBL.Model {
    public class Facility {
        public Facility(string name) {
            Name = name;
        }

        public Facility(int id, string name) {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
    }
}
