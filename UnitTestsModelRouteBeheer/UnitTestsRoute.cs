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
            double distance = Route.GetDistance(new Point(1, 1), new Point(4, 5));
            Assert.Equal(5, distance);
        }
    }
}
