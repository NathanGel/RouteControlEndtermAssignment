using RouteBeheerBL.Model;
using System.ComponentModel;

namespace WPFRouteBeheerUI.Model {
    public class NetworkPointStopsUI : INotifyPropertyChanged {
        public NetworkPoint point { get; set; }
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        private bool _isStop;
        public bool IsStop {
            get => _isStop;
            set {
                if (_isStop != value) {
                    _isStop = value;
                    OnPropertyChanged(nameof(IsStop));
                }
            }
        }

        public NetworkPointStopsUI(NetworkPoint point, bool isStop) {
            this.point = point;
            Id = point.Id;
            X = point.X;
            Y = point.Y;
            IsStop = isStop;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
