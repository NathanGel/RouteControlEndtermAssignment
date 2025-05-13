using RouteBeheerBL.Model;
using RouteBeheerBL.Exceptions;

namespace UnitTestsModelRouteBeheer {
    public class UnitTestsSegment {
        [Fact]
        public void Test_ConstructorWithoutId_Valid() {
            NetworkPoint startPoint = new(12, 13);
            NetworkPoint endPoint = new(13, 14);
            Segment segment = new( startPoint, endPoint);
            Assert.NotNull(segment);
            Assert.Equal(startPoint, segment.StartPoint);
            Assert.Equal(endPoint, segment.EndPoint);
        }

        [Fact]
        public void Test_ConstructorWithoutId_InvalidStartPointNull() {
            NetworkPoint endPoint = new(13, 14);
            Assert.Throws<NetworkException>(() => new Segment(null, endPoint));
        }

        [Fact]
        public void Test_ConstructorWithoutId_InvalidEndPointNull() {
            NetworkPoint startPoint = new(12, 13);
            Assert.Throws<NetworkException>(() => new Segment(startPoint, null));
        }

        [Fact]
        public void Test_ConstructorWithId_Valid() {
            NetworkPoint startPoint = new(12, 13);
            NetworkPoint endPoint = new(13, 14);
            Segment segment = new(1, startPoint, endPoint);
            Assert.NotNull(segment);
            Assert.Equal(startPoint, segment.StartPoint);
            Assert.Equal(endPoint, segment.EndPoint);
            Assert.Equal(1, segment.Id);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Test_ConstructorWithId_Invalid(int id) {
            NetworkPoint startPoint = new(12, 13);
            NetworkPoint endPoint = new(13, 14);
            Assert.Throws<NetworkException>(() => new Segment(id, startPoint, endPoint));
        }

        [Fact]
        public void Test_ConstructorWithId_InvalidStartPointNull() {
            NetworkPoint startPoint = new(12, 13);
            Assert.Throws<NetworkException>(() => new Segment(1, startPoint, null));
        }

        [Fact]
        public void Test_ConstructorWithId_InvalidEndPointNull() {
            NetworkPoint endPoint = new(13, 14);
            Assert.Throws<NetworkException>(() => new Segment(1, null, endPoint));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(200)]
        [InlineData(3000)]
        public void Test_Id_Valid(int id) {
            Segment segment = new(new(12,13), new(13,14));
            segment.Id = id;
            Assert.Equal(id, segment.Id);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Test_Id_Invalid(int id) {
            Segment segment = new(new(12, 13), new(13, 14));
            Assert.Throws<NetworkException>( () => segment.Id = id);
        }

        [Fact]
        public void Test_StartPoint_Valid() {
            Segment segment = new(new(12, 13), new(13, 14));
            NetworkPoint startPoint = new(20, 21);
            segment.StartPoint = startPoint;
            Assert.Equal(startPoint, segment.StartPoint);
        }

        [Fact]
        public void Test_StartPoint_Invalid() {
            Segment segment = new(new(12, 13), new(13, 14));
            Assert.Throws<NetworkException>(() => segment.StartPoint = null);
        }

        [Fact]
        public void Test_EndPoint_Valid() {
            Segment segment = new(new(12, 13), new(13, 14));
            NetworkPoint endPoint = new(30, 31);
            segment.EndPoint = endPoint;
            Assert.Equal(endPoint, segment.EndPoint);
        }

        [Fact]
        public void Test_EndPoint_Invalid() {
            Segment segment = new(new(12, 13), new(13, 14));
            Assert.Throws<NetworkException>(() => segment.EndPoint = null);
        }
    }
}
