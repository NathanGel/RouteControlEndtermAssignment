using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RouteBeheerBL.Model;

namespace UnitTestsModelRouteBeheer {
    public class UnitTestsFacility {
        [Fact]
        public void TestFacility() {
            Facility facility = new("Facility");
            Assert.Equal("Facility", facility.Name);
        }
    }
}
