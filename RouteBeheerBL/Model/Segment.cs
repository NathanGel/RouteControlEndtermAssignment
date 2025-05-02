namespace RouteBeheerBL.Model {
    public class Segment {
        public Segment( NetworkPoint startPoint, NetworkPoint endPoint) {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public Segment(int id, NetworkPoint startPoint, NetworkPoint endPoint) {
            Id = id;
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public int Id { get; set; }
        public NetworkPoint StartPoint { get; set; }
        public NetworkPoint EndPoint { get; set; }
    }
}
