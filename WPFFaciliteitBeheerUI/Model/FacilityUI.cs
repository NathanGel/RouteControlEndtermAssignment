using System.ComponentModel;

namespace WPFFaciliteitBeheerUI.Model {
    public class FacilityUI : INotifyPropertyChanged {
        public FacilityUI(string name) {
            Name = name;
        }

        public FacilityUI(int id, string name) {
            Id = id;
            Name = name;
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
        public void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
