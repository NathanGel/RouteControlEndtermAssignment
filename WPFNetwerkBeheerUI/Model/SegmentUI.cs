using System.ComponentModel;

namespace WPFNetwerkBeheerUI.Model {
    public class SegmentUI : INotifyPropertyChanged {
        public SegmentUI( NetworkPointUI startPoint, NetworkPointUI endPoint) {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public SegmentUI(int id, NetworkPointUI startPoint, NetworkPointUI endPoint) {
            Id = id;
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public SegmentUI() {
        }

        public int Id { get; set; }

        private NetworkPointUI _startPoint;
        public NetworkPointUI StartPoint {
            get { return _startPoint; }
            set {
                _startPoint = value;
                OnPropertyChanged(nameof(StartPoint));
            }
        }

        private NetworkPointUI _endPoint;
        public NetworkPointUI EndPoint {
            get { return _endPoint; }
            set {
                _endPoint = value;
                OnPropertyChanged(nameof(EndPoint));
            }
        }

        public void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
