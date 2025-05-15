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
using RouteBeheerBL.Model;
using WPFRouteBeheerUI.Model;

namespace WPFRouteBeheerUI
{
    /// <summary>
    /// Interaction logic for SelectRouteDialogWindow.xaml
    /// </summary>
    public partial class SelectRouteDialogWindow : Window
    {
        public RouteUI route;
        private List<RouteUI> routes = new();
        public SelectRouteDialogWindow(List<RouteUI> routes, bool isRouteInfo )
        {
            InitializeComponent();
            this.routes = routes;
            DataGridRoutes.ItemsSource = routes;

            if (isRouteInfo) {
                SelectRoute.Visibility = Visibility.Collapsed;
            } else {
                MoreInfo.Visibility = Visibility.Collapsed;
                RemoveRoute.Visibility = Visibility.Collapsed;
            }
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

        private void SelectRoute_Click(object sender, RoutedEventArgs e) {
            if (DataGridRoutes.SelectedItem != null) {
                route = (RouteUI)DataGridRoutes.SelectedItem;
                DialogResult = true;
                Close();
            } else {
                MessageBox.Show("Please select a route to display", "No route selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void TitleBar_MouseMove(object sender, MouseEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                DragMove();
            }
        }

        private void MoreInfo_Click(object sender, RoutedEventArgs e) {
            RouteWindow routeWindow = new RouteWindow((RouteUI)DataGridRoutes.SelectedItem);
            routeWindow.ShowDialog();
        }

        private void RemoveRoute_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("OULEH DE KNOP WERKT INSHALLAH");
        }
    }
}
