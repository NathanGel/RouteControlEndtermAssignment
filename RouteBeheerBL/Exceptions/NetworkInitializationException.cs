namespace RouteBeheerBL.Exceptions {
    public class NetworkInitializationException : Exception {

        public NetworkInitializationException(string? message) : base(message) {
        }

        public NetworkInitializationException(string? message, Exception? innerException) : base(message, innerException) {
        }

    }
}
