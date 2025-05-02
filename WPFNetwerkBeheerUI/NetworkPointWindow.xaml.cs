using RouteBeheerBL.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WPFNetwerkBeheerUI.Model;

namespace WPFNetwerkBeheerUI {
    /// <summary>
    /// Interaction logic for NetworkPointWindow.xaml
    /// </summary>
    public partial class NetworkPointWindow : Window {
        public NetworkPointUI point;
        private ObservableCollection<Facility> facilities;
        public NetworkPointWindow(NetworkPointUI point) {
            InitializeComponent();
            facilities = point.Facilities;
            this.point = point;
            textBoxId.Text = point.Id.ToString();
            textBoxX.Text = point.X.ToString();
            textBoxY.Text = point.Y.ToString();
            listBoxFacilities.ItemsSource = facilities;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e) {
            point.X = Convert.ToDouble(textBoxX.Text);
            point.Y = Convert.ToDouble(textBoxY.Text);
            point.Facilities = facilities;
            DialogResult = true;
            Close();
        }

        private void BtnFacilities_Click(object sender, RoutedEventArgs e) {
            FacilitiesWindow fWindow = new(facilities);
            bool? result = fWindow.ShowDialog();
            if (result == true) { 
                facilities = fWindow.selectedFacilities;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) {
            this.Close();
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
    }
}
