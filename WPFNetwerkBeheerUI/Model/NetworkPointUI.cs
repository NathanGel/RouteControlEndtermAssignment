using RouteBeheerBL.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WPFNetwerkBeheerUI.Model {
    public class NetworkPointUI : INotifyPropertyChanged {
        public NetworkPointUI(double x, double y) {
            X = x;
            Y = y;
        }

        public NetworkPointUI(double x, double y, ObservableCollection<Facility> facilities) {
            X = x;
            Y = y;
            Facilities = facilities;
        }

        public NetworkPointUI(int id, double x, double y) {
            Id = id;
            X = x;
            Y = y;
        }

        public NetworkPointUI(int id, double x, double y, ObservableCollection<Facility> facilities) {
            Id = id;
            X = x;
            Y = y;
            Facilities = facilities;
        }

        public int Id { get; set; }
        private double _x;
        public double X { 
            get { return _x; }
            set {
                _x = value;
                OnPropertyChanged(nameof(X));
            }
        }
        private double _y;
        public double Y { 
            get { return _y; }
            set {
                _y = value;
                OnPropertyChanged(nameof(Y));
            }
        }
        private ObservableCollection<Facility> _facilities = new(); // dit veranderd naar een observable collection om te zorgen dat de UI altijd aanpast wanneer er een verwijdert wordt/toegevoegd
        public ObservableCollection<Facility> Facilities { 
            get { return _facilities; }
            set {
                _facilities = value;
                OnPropertyChanged(nameof(Facilities));
            }
        } 
        public void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override bool Equals(object? obj) {
            if (obj is not NetworkPointUI other)
                return false;

            return Id == other.Id;
        }

        public override int GetHashCode() {
            return Id.GetHashCode();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
