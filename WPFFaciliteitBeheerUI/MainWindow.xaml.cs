using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RouteBeheerBL.Managers;
using RouteBeheerBL.Model;
using RouteBeheerDL;
using WPFFaciliteitBeheerUI.Model;
using WPFFaciliteitBeheerUI.Mappers;

namespace WPFFaciliteitBeheerUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private NetworkManager networkManager;
        private readonly string connectionString = @"Data Source=NATHAN\SQLExpress;Initial Catalog=NetworkControlTesting;Integrated Security=True;Trust Server Certificate=True";
        public ObservableCollection<FacilityUI> Facilities;
        public MainWindow() {
            InitializeComponent();
            networkManager = new NetworkManager(new NetworkRepository(connectionString));
            Facilities = new(networkManager.GetAllFacilities().Select( s => FacilityMapper.MapFromDomain(s)));
            DataGridFacilities.ItemsSource = Facilities;
        }

        private void UpdateFacility_Click(object sender, RoutedEventArgs e) {
            FacilityWindow window = new((FacilityUI)DataGridFacilities.SelectedItem, true);
            window.ShowDialog();
        }

        private void AddFacility_Click(object sender, RoutedEventArgs e) {
            FacilityWindow window = new(null, false);
            window.ShowDialog();
        }

        private void DeleteFacility_Click(object sender, RoutedEventArgs e) {
            
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (e.ButtonState == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e) {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e) {
            if (this.WindowState == WindowState.Normal) {
                this.WindowState = WindowState.Maximized;
            } else {
                this.WindowState = WindowState.Normal;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}