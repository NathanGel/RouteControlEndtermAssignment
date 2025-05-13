using RouteBeheerBL.Exceptions;
using RouteBeheerBL.Model;

namespace UnitTestsModelRouteBeheer {
    public class UnitTestsNetworkPoint {
        private List<Facility> _facilities = new() {
            new Facility("F1"),
            new Facility("F2")
        };

        [Fact]
        public void Constructor_XY_Valid() {
            var np = new NetworkPoint(10, 20);
            Assert.Equal(10, np.X);
            Assert.Equal(20, np.Y);
            Assert.Empty(np.Facilities);
        }

        [Theory]
        [InlineData(-1, 50)]
        [InlineData(50, -1)]
        [InlineData(1001, 50)]
        [InlineData(50, 1001)]
        public void Constructor_XY_Invalid(double x, double y) {
            Assert.Throws<NetworkException>(() => new NetworkPoint(x, y));
        }

        [Fact]
        public void Constructor_XYFacilities_Valid() {
            var np = new NetworkPoint(100, 200, _facilities);
            Assert.Equal(100, np.X);
            Assert.Equal(200, np.Y);
            Assert.Equal(_facilities, np.Facilities);
        }

        [Theory]
        [InlineData(-5, 20)]
        [InlineData(20, -5)]
        [InlineData(1500, 50)]
        [InlineData(50, 1500)]
        public void Constructor_XYFacilities_Invalid(double x, double y) {
            Assert.Throws<NetworkException>(() => new NetworkPoint(x, y, _facilities));
        }

        [Fact]
        public void Constructor_IdXY_Valid() {
            var np = new NetworkPoint(1, 100, 200);
            Assert.Equal(1, np.Id);
            Assert.Equal(100, np.X);
            Assert.Equal(200, np.Y);
            Assert.Empty(np.Facilities);
        }

        [Theory]
        [InlineData(0, 10, 10)]
        [InlineData(-1, 10, 10)]
        public void Constructor_IdXY_InvalidId(int id, double x, double y) {
            Assert.Throws<NetworkException>(() => new NetworkPoint(id, x, y));
        }

        [Theory]
        [InlineData(1, -1, 50)]
        [InlineData(1, 50, -1)]
        [InlineData(1, 2000, 50)]
        [InlineData(1, 50, 2000)]
        public void Constructor_IdXY_InvalidXY(int id, double x, double y) {
            Assert.Throws<NetworkException>(() => new NetworkPoint(id, x, y));
        }

        [Fact]
        public void Constructor_IdXYFacilities_Valid() {
            var np = new NetworkPoint(2, 300, 400, _facilities);
            Assert.Equal(2, np.Id);
            Assert.Equal(300, np.X);
            Assert.Equal(400, np.Y);
            Assert.Equal(_facilities, np.Facilities);
        }

        [Theory]
        [InlineData(0, 100, 100)]
        [InlineData(-10, 100, 100)]
        [InlineData(1, -100, 100)]
        [InlineData(1, 100, -100)]
        [InlineData(1, 1001, 100)]
        [InlineData(1, 100, 1001)]
        public void Constructor_IdXYFacilities_Invalid(int id, double x, double y) {
            Assert.Throws<NetworkException>(() => new NetworkPoint(id, x, y, _facilities));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(3000)]
        public void Test_Id_Valid(int id) {
            NetworkPoint point = new(12.12, 124.1234);
            point.Id = id;
            Assert.Equal(id, point.Id);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Test_Id_Invalid(int id) {
            NetworkPoint point = new(12.12, 124.1234);
            Assert.Throws<NetworkException>(() => point.Id = id);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(0.1)]
        [InlineData(1000)]
        [InlineData(999.9)]
        public void Test_X_Valid(double x) {
            NetworkPoint point = new(12.12, 124.1234);
            point.X = x;
            Assert.Equal(x, point.X);
        }


        [Theory]
        [InlineData(-0.1)]
        [InlineData(-1)]
        [InlineData(-1000)]
        [InlineData(1000.1)]
        [InlineData(1001)]
        [InlineData(1100)]
        public void Test_X_Invalid(double x) {
            NetworkPoint point = new(12.12, 124.1234);
            Assert.Throws<NetworkException>(() => point.X = x);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(0.1)]
        [InlineData(1000)]
        [InlineData(999.9)]
        public void Test_Y_Valid(double y) {
            NetworkPoint point = new(12.12, 124.1234);
            point.Y = y;
            Assert.Equal(y, point.Y);
        }


        [Theory]
        [InlineData(-0.1)]
        [InlineData(-1)]
        [InlineData(-1000)]
        [InlineData(1000.1)]
        [InlineData(1001)]
        [InlineData(1100)]
        public void Test_Y_Invalid(double y) {
            NetworkPoint point = new(12.12, 124.1234);
            Assert.Throws<NetworkException>(() => point.Y = y);
        }

        [Fact]
        public void Test_Facilities_Valid() {
            NetworkPoint point = new(12.01, 123.1234);
            List<Facility> facilities = new() {
                new Facility("Test"),
                new Facility("Test2")
            };
            point.Facilities = facilities;
            Assert.Equal(facilities, point.Facilities);
        }

        [Fact]
        public void Test_Equals_SameObject() {
            var networkPoint1 = new NetworkPoint(1, 10, 20);
            var networkPoint2 = new NetworkPoint(1, 10, 20);
            Assert.True(networkPoint1.Equals(networkPoint2));
        }

        [Fact]
        public void Test_Equals_DifferentId() {
            var networkPoint1 = new NetworkPoint(1, 10, 20);
            var networkPoint2 = new NetworkPoint(2, 10, 20);
            Assert.False(networkPoint1.Equals(networkPoint2));
        }

        [Fact]
        public void Test_Equals_DifferentObjectType() {
            var networkPoint = new NetworkPoint(1, 10, 20);
            var differentObject = new object();
            Assert.False(networkPoint.Equals(differentObject));
        }

        [Fact]
        public void Test_Equals_SameReference() {
            var networkPoint = new NetworkPoint(1, 10, 20);
            Assert.True(networkPoint.Equals(networkPoint));  // Same reference should be equal
        }

        [Fact]
        public void Test_Equals_Null() {
            var networkPoint = new NetworkPoint(1, 10, 20);
            Assert.False(networkPoint.Equals(null));  // Null is not equal to any object
        }

        [Fact]
        public void Test_GetHashCode_SameId() {
            var networkPoint1 = new NetworkPoint(1, 10, 20);
            var networkPoint2 = new NetworkPoint(1, 30, 40);  // Same Id, different coordinates
            Assert.Equal(networkPoint1.GetHashCode(), networkPoint2.GetHashCode());
        }

        [Fact]
        public void Test_GetHashCode_DifferentId() {
            var networkPoint1 = new NetworkPoint(1, 10, 20);
            var networkPoint2 = new NetworkPoint(2, 10, 20);  // Different Id
            Assert.NotEqual(networkPoint1.GetHashCode(), networkPoint2.GetHashCode());
        }

        [Fact]
        public void Test_GetHashCode_Consistent() {
            var networkPoint = new NetworkPoint(1, 10, 20);
            var hashCode1 = networkPoint.GetHashCode();
            var hashCode2 = networkPoint.GetHashCode();
            Assert.Equal(hashCode1, hashCode2);  // GetHashCode should be consistent for the same object
        }
    }
}
