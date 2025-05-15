using RouteBeheerBL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFRouteBeheerUI.Model;
namespace WPFRouteBeheerUI {
    /// <summary>
    /// Interaction logic for RouteWindow.xaml
    /// </summary>
    public partial class RouteWindow : Window {
        private RouteUI _route;
        public RouteWindow( RouteUI route) {
            InitializeComponent();
            this._route = route;
            TxtId.Text = _route.Id.ToString();
            TxtName.Text = _route.Name;
            DataGridStops.ItemsSource = _route.Stops;
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) {
            Close();
        }
        private void StopCell_Click(object sender, MouseButtonEventArgs e) {
            if (sender is Border border && border.DataContext is NetworkPoint point) {
                // Toggle the IsStop property
                //point.IsStop = !point.IsStop;

                // Refresh DataGrid to update visuals
                //DataGridLocations.Items.Refresh();
            }
        }

    }
}
