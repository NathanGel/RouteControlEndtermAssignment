using RouteBeheerBL.Exceptions;
using RouteBeheerBL.Model;
using System.Drawing;

namespace UnitTestsModelRouteBeheer {
    public class UnitTestsRoute {
        private List<(Segment, bool)> _segmenten;
        private List<Facility> _facilities;
        private List<NetworkPoint> _networkPoints;
        private List<(NetworkPoint, bool)> _stops;

        public UnitTestsRoute() {
            _facilities = new() {
                new Facility("Facility1"),
                new Facility("Facility2"),
                new Facility("Facility3")
            };
            _networkPoints = new() {
                new NetworkPoint(1, 2, _facilities),
                new NetworkPoint(2, 3, _facilities),
                new NetworkPoint(3, 4, _facilities),
                new NetworkPoint(4, 5, _facilities),
                new NetworkPoint(5, 6, _facilities)
            };
            _stops = new() {
                (_networkPoints[0], true),
                (_networkPoints[1], false),
                (_networkPoints[2], true),
                (_networkPoints[3], false),
                (_networkPoints[4], true)
            };
            _segmenten = new() {
                (new Segment(1, _networkPoints[0], _networkPoints[1]), true),
                (new Segment(2, _networkPoints[1], _networkPoints[2]), false),
                (new Segment(3, _networkPoints[2], _networkPoints[3]), true),
                (new Segment(4, _networkPoints[3], _networkPoints[4]), false),
            };
        }

        [Fact]
        public void Test_EmptyConstructor_Valid() {
            Route route = new Route();
            Assert.Equal(0, route.Id);
            Assert.Equal(null, route.Name);
            Assert.Empty(route.Segments);
            Assert.Empty(route.Stops);
        }

        [Fact]
        public void Test_ConstructorWithoutId_Valid() {
            Route route = new Route("Route1", _segmenten, _stops);
            Assert.Equal("Route1", route.Name);
            Assert.Equal(_segmenten, route.Segments);
            Assert.Equal(_stops, route.Stops);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("Rt")]

        public void Test_ConstructorWithoutId_InvalidName(string name) {
            Assert.Throws<RouteException>( () => new Route(name, _segmenten, _stops));
        }

        [Fact]
        public void Test_ConstructorWithoutId_InvalidSegmentsNull() {
            Assert.Throws<RouteException>(() => new Route("Route1", null, _stops));
        }

        [Fact]
        public void Test_ConstructorWithoutId_InvalidSegmentsEmpty() {
            List<(Segment, bool)> invalidSegments = new();
            Assert.Throws<RouteException>(() => new Route("Route1", invalidSegments, _stops));
        }

        [Fact]
        public void Test_ConstructorWithoutId_InvalidSegmentsContainsNullValue() {
            List<(Segment, bool)> invalidSegments = new() { (null, true) };
            Assert.Throws<RouteException>(() => new Route("Route1", invalidSegments, _stops));
        }

        [Fact]
        public void Test_ConstructorWithoutId_InvalidStops_Null() {
            Assert.Throws<RouteException>(() => new Route("Route1", _segmenten, null));
        }

        [Fact]
        public void Test_ConstructorWithoutId_InvalidStops_Empty() {
            var invalidStops = new List<(NetworkPoint, bool)>();
            Assert.Throws<RouteException>(() => new Route("Route1", _segmenten, invalidStops));
        }

        [Fact]
        public void Test_ConstructorWithoutId_InvalidStops_ContainsNull() {
            var invalidStops = new List<(NetworkPoint, bool)>
            {
                (null, true),
                (new NetworkPoint(0, 0), true),
                (new NetworkPoint(1, 1), true),
                (new NetworkPoint(2, 2), true),
                (new NetworkPoint(3, 3), true),
            };
            Assert.Throws<RouteException>(() => new Route("Route1", _segmenten, invalidStops));
        }

        [Fact]
        public void Test_ConstructorWithoutId_InvalidStops_LessThanFive() {
            var invalidStops = new List<(NetworkPoint, bool)>
            {
                (new NetworkPoint(0, 0), true),
                (new NetworkPoint(1, 1), true),
                (new NetworkPoint(2, 2), true),
                (new NetworkPoint(3, 3), true),
            };
            Assert.Throws<RouteException>(() => new Route("Route1", _segmenten, invalidStops));
        }

        [Fact]
        public void Test_ConstructorWithoutId_InvalidStops_StartNotStop() {
            var invalidStops = new List<(NetworkPoint, bool)>
            {
                (new NetworkPoint(0, 0), false), // invalid start
                (new NetworkPoint(1, 1), true),
                (new NetworkPoint(2, 2), true),
                (new NetworkPoint(3, 3), true),
                (new NetworkPoint(4, 4), true),
            };
            Assert.Throws<RouteException>(() => new Route("Route1", _segmenten, invalidStops));
        }

        [Fact]
        public void Test_ConstructorWithoutId_InvalidStops_EndNotStop() {
            var invalidStops = new List<(NetworkPoint, bool)>
            {
                (new NetworkPoint(0, 0), true),
                (new NetworkPoint(1, 1), true),
                (new NetworkPoint(2, 2), true),
                (new NetworkPoint(3, 3), true),
                (new NetworkPoint(4, 4), false), // invalid end
            };
            Assert.Throws<RouteException>(() => new Route("Route1", _segmenten, invalidStops));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-99)]
        public void Test_ConstructorWithId_InvalidId(int id) {
            Assert.Throws<RouteException>(() => new Route(id, "Route1", _segmenten, _stops));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("Rt")]
        public void Test_ConstructorWithId_InvalidName(string name) {
            Assert.Throws<RouteException>(() => new Route(1, name, _segmenten, _stops));
        }

        [Fact]
        public void Test_ConstructorWithId_InvalidSegmentsNull() {
            Assert.Throws<RouteException>(() => new Route(1, "Route1", null, _stops));
        }

        [Fact]
        public void Test_ConstructorWithId_InvalidSegmentsEmpty() {
            List<(Segment, bool)> invalidSegments = new();
            Assert.Throws<RouteException>(() => new Route(1, "Route1", invalidSegments, _stops));
        }

        [Fact]
        public void Test_ConstructorWithId_InvalidSegmentsContainsNull() {
            List<(Segment, bool)> invalidSegments = new() { (null, true) };
            Assert.Throws<RouteException>(() => new Route(1, "Route1", invalidSegments, _stops));
        }

        [Fact]
        public void Test_ConstructorWithId_InvalidStopsNull() {
            Assert.Throws<RouteException>(() => new Route(1, "Route1", _segmenten, null));
        }

        [Fact]
        public void Test_ConstructorWithId_InvalidStopsEmpty() {
            List<(NetworkPoint, bool)> invalidStops = new();
            Assert.Throws<RouteException>(() => new Route(1, "Route1", _segmenten, invalidStops));
        }

        [Fact]
        public void Test_ConstructorWithId_InvalidStopsContainsNull() {
            List<(NetworkPoint, bool)> invalidStops = new() {
        (null, true),
        (new NetworkPoint(1,1), true),
        (new NetworkPoint(2,2), true),
        (new NetworkPoint(3,3), true),
        (new NetworkPoint(4,4), true),
    };
            Assert.Throws<RouteException>(() => new Route(1, "Route1", _segmenten, invalidStops));
        }

        [Fact]
        public void Test_ConstructorWithId_InvalidStops_LessThanFive() {
            List<(NetworkPoint, bool)> invalidStops = new() {
        (new NetworkPoint(1,1), true),
        (new NetworkPoint(2,2), true),
        (new NetworkPoint(3,3), true),
        (new NetworkPoint(4,4), true),
    };
            Assert.Throws<RouteException>(() => new Route(1, "Route1", _segmenten, invalidStops));
        }


        [Fact]
        public void Test_ConstructorWithId_Valid() {
            Route route = new Route(1, "Route1", _segmenten, _stops);
            Assert.Equal(1, route.Id);
            Assert.Equal("Route1", route.Name);
            Assert.Equal(_segmenten, route.Segments);
            Assert.Equal(_stops, route.Stops);
        }

        [Fact]
        public void Test_Id_Valid() {
            Route r = new("Route1", _segmenten, _stops);
            r.Id = 1;
            Assert.Equal(1, r.Id);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Test_Id_Invalid(int id) {
            Route r = new("Route1", _segmenten, _stops);
            Assert.Throws<RouteException>(() => r.Id = id);
        }

        [Theory]
        [InlineData("Route1")]
        [InlineData("Rt1")]
        public void Test_Name_Valid(string name) {
            Route r = new(name, _segmenten, _stops);
            Assert.Equal(name, r.Name);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("Rt")]
        public void Test_Name_Invalid(string name) {
            Assert.Throws<RouteException>(() => new Route(name, _segmenten, _stops));
        }

        [Fact]
        public void Test_Segments_Valid() {
            Route r = new("Route1", _segmenten, _stops);
            Assert.Equal(_segmenten, r.Segments);
        }

        [Fact]
        public void Test_Segments_Invalid_SegmentsEmpty() {
            Route r = new("Route1", _segmenten, _stops);
            List<(Segment, bool)> invalidSegments = new();
            Assert.Throws<RouteException>(() => r.Segments = invalidSegments);
        }

        [Fact]
        public void Test_Segments_Invalid_SegmentsNull() {
            Route r = new("Route1", _segmenten, _stops);
            List<(Segment, bool)> invalidSegments = null;
            Assert.Throws<RouteException>(() => r.Segments = invalidSegments);
        }

        [Fact]
        public void Test_Segments_Invalid_SegmentInListNull() {
            Route r = new("Route1", _segmenten, _stops);
            List<(Segment, bool)> invalidSegments = new() { (null, true) };
            Assert.Throws<RouteException>(() => r.Segments = invalidSegments);
        }


        [Fact]
        public void Test_Stops_Valid() {
            Route r = new("Route1", _segmenten, _stops);
            Assert.Equal(_stops, r.Stops);
            Assert.True(r.Stops[0].Item2);
            Assert.True(r.Stops[_stops.Count - 1].Item2);
        }

        [Fact]
        public void Test_Stops_Invalid_StartPointNotAStop() {
            _stops[0] = (_networkPoints[1], false);
            Assert.Throws<RouteException>(() => new Route("Route1", _segmenten, _stops));
        }

        [Fact]
        public void Test_Stops_Invalid_EndPointNotAStop() {
            _stops[^1] = (_networkPoints[3], false);
            Assert.Throws<RouteException>(() => new Route("Route1", _segmenten, _stops));
        }

        [Fact]
        public void Test_Stops_Invalid_StopsEmpty() { 
            Route r = new("Route1", _segmenten, _stops);
            List<(NetworkPoint, bool)> invalidStops = new();
            Assert.Throws<RouteException>(() => r.Stops = invalidStops);
        }

        [Fact]
        public void Test_Stops_Invalid_StopsNull() {
            Route r = new("Route1", _segmenten, _stops);
            List<(NetworkPoint, bool)> invalidStops = null;
            Assert.Throws<RouteException>(() => r.Stops = invalidStops);
        }

        [Fact]
        public void Test_Stops_Invalid_StopInListNull() {
            Route r = new("Route1", _segmenten, _stops);
            List<(NetworkPoint, bool)> invalidStops = new() { (null, true) };
            Assert.Throws<RouteException>(() => r.Stops = invalidStops);
        }

        [Fact]
        public void Test_Route_Valid() {
            Route r = new("Route1", _segmenten, _stops);
            Assert.NotNull(r);
            Assert.Equal("Route1", r.Name);
            Assert.Equal(_segmenten, r.Segments);
            Assert.Equal(_stops, r.Stops);
        }

        [Fact]
        public void Test_Route_Invalid_NotEnoughNetworkPoints() {
            _stops.RemoveAt(0);
            Assert.Throws<RouteException>(() => new Route("Route1", _segmenten, _stops));
        }

        [Fact]
        public void Test_GetDistance_Valid() {
            double distance = Route.GetDistance(new NetworkPoint(1, 1), new NetworkPoint(4, 5));
            Assert.Equal(5, distance);
        }
    }
}
