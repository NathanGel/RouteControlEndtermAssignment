namespace RouteBeheerBL.Exceptions {
    public class RouteException : Exception {
        public RouteException(string message) : base(message) {
        }
        public RouteException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}
