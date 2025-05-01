using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RouteBeheerBL.Model;
using RouteBeheerBL.Exceptions;

namespace UnitTestsModelRouteBeheer {
    public class UnitTestsFacility {
        [Fact]
        public void Test_ConstructorWithId_Valid() {
            Facility f = new(1, "Test");
            Assert.NotNull(f);
            Assert.Equal(1, f.Id);
            Assert.Equal("Test", f.Name);
        }

        [Fact]
        public void Test_ConstructorWithoutId_Valid() {
            Facility f = new("test");
            Assert.NotNull(f);
            Assert.Equal("test", f.Name);
        }

        [Fact]
        public void Test_FacilityId_Valid() {
            Facility f = new("Test");
            f.Id = 2;
            Assert.Equal(2, f.Id);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Test_FacilityId_Invalid(int id) {
            Facility f = new("Test");
            Assert.Throws<NetworkException>(() => f.Id=id);
        }

        [Fact]
        public void Test_FacilityName_Valid() {
            Facility f = new("Test");
            f.Name = "Name";
            Assert.Equal("Name", f.Name);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Test_FacilityName_Invalid(string name) {
            Facility f = new("Test");
            Assert.Throws<NetworkException>(() => f.Name = name);
        }
    }
}
