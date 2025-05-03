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
using RouteBeheerDL;
using RouteBeheerBL.Model;
using System.Collections.ObjectModel;

namespace WPFRouteBeheerUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private List<NetworkPoint> points;
        private List<Segment> segments;
        private ObservableCollection<Route> routes;
        private readonly string connectionString = @"Data Source=nathans-laptop\SQLExpress;Initial Catalog=NetworkControlTesting;Integrated Security=True;Trust Server Certificate=True";
        private NetworkManager nm;
        private RouteManager rm;
        public MainWindow() {
            rm = new(new RouteRepository(connectionString));
            points = new List<NetworkPoint>();
            segments = new List<Segment>();
            routes = new ObservableCollection<Route>();
            InitializeComponent();
            ReadFromDatabase();
        }

        public void ReadFromDatabase() {
            nm = new(new NetworkRepository(connectionString));
            try {
                List<Segment> segmentsFromDb = new(nm.GetSegments());
                foreach (var segment in segmentsFromDb) {
                    segments.Add(segment);
                }

                List<NetworkPoint> pointsFromDb = new(nm.GetNetworkPoints());
                foreach (var point in pointsFromDb) {
                    points.Add(new NetworkPoint(point.Id, point.X, point.Y, point.Facilities));
                }
            } catch (ApplicationException ex) { // dit catch-blok vangt elke mogelijke sqlexception die gegooid werden in de repo. De SqlException werd vertaald naar een ApplicationException
                MessageBox.Show("An error occured while retrieving the network for initialization", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            } catch (Exception ex) { // dit catch-blok dient als fallback indien er ergens een exception gegooid word die onverwacht is  
                MessageBox.Show("An unexpected error occured:  " + ex.Message, "System Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            MessageBox.Show("Not implemented");
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            MessageBox.Show("Not implemented");
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount == 2) {
                if (WindowState == WindowState.Maximized) {
                    WindowState = WindowState.Normal;
                } else {
                    WindowState = WindowState.Maximized;
                }
            } else {
                DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e) {
            if (WindowState == WindowState.Maximized) {
                WindowState = WindowState.Normal;
                MaxRestoreButton.Content = "\uE922";
            } else {
                WindowState = WindowState.Maximized;
                MaxRestoreButton.Content = "\uE922";
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void TitleBar_MouseMove(object sender, MouseEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                DragMove();
            }
        }

        private void BtnManageRoutes_Click(object sender, RoutedEventArgs e) {

        }

        private void BtnAddRoute_Click(object sender, RoutedEventArgs e) {

        }

        private void BtnRemoveAllCurrentHighlights_Click(object sender, RoutedEventArgs e) {

        }

        private void BtnSelectRoute_Click(object sender, RoutedEventArgs e) {

        }
    }
}