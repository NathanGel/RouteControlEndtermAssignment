using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RouteBeheerBL.Model;

namespace WPFRouteBeheerUI.Model {
    public class RouteUI : INotifyPropertyChanged {
        public RouteUI(string name, List<(Segment, bool)> segments, List<(NetworkPoint, bool)> stops) {
            Name = name;
            Segments = segments;
            Stops = stops;
        }

        public RouteUI(int id, string name, List<(Segment, bool)> segments, List<(NetworkPoint, bool)> stops) {
            Id = id;
            Name = name;
            Segments = segments;
            Stops = stops;
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
        private List<(Segment, bool)> _segments = new();
        public List<(Segment, bool)> Segments { 
            get { return _segments; }
            set {
                _segments = value;
                OnPropertyChanged(nameof(Segments));
            }
        }
        private List<(NetworkPoint, bool)> _stops = new();
        public List<(NetworkPoint, bool)> Stops { 
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
