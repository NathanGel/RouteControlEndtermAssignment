using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RouteBeheerBL.Model;

namespace WPFRouteBeheerUI.Model {
    public class RouteUI : INotifyPropertyChanged {
        public RouteUI(string name, ObservableCollection<(Segment, bool)> segments, ObservableCollection<(NetworkPoint, bool)> stops) {
            Name = name;
            Segments = segments;
            Stops = stops;
        }

        public RouteUI(int id, string name, ObservableCollection<(Segment, bool)> segments, ObservableCollection<(NetworkPoint, bool)> stops) {
            Id = id;
            Name = name;
            Segments = segments;
            Stops = stops;
        }

        public RouteUI() {
        }

        public int Id { get; set; }
        private string _name;
        public string Name { 
            get { return _name; }
            set {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        private ObservableCollection<(Segment, bool)> _segments = new();
        public ObservableCollection<(Segment, bool)> Segments { 
            get { return _segments; }
            set {
                _segments = value;
                OnPropertyChanged(nameof(Segments));
            }
        }
        private ObservableCollection<(NetworkPoint, bool)> _stops = new();
        public ObservableCollection<(NetworkPoint, bool)> Stops { 
            get { return _stops; }
            set {
                _stops = value;
                OnPropertyChanged(nameof(Stops));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
