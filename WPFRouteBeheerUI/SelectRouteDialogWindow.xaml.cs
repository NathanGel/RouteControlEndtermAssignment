using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using RouteBeheerBL.Managers;
using RouteBeheerBL.Model;
using WPFRouteBeheerUI.Mappers;
using WPFRouteBeheerUI.Model;

namespace WPFRouteBeheerUI
{
    /// <summary>
    /// Interaction logic for SelectRouteDialogWindow.xaml
    /// </summary>
    public partial class SelectRouteDialogWindow : Window
    {
        public RouteUI route;
        private RouteManager _routeManager;
        private ObservableCollection<RouteUI> routes = new();
        private List<Segment> segments;
        private List<NetworkPoint> points;
        public SelectRouteDialogWindow(ObservableCollection<RouteUI> routes, bool isRouteInfo, RouteManager mn){
            InitializeComponent();
            this.routes = routes;
            _routeManager = mn;
            DataGridRoutes.ItemsSource = routes;

            if (isRouteInfo) {
                SelectRoute.Visibility = Visibility.Collapsed;
            } else {
                MoreInfo.Visibility = Visibility.Collapsed;
                RemoveRoute.Visibility = Visibility.Collapsed;
            }
        }
        public SelectRouteDialogWindow(ObservableCollection<RouteUI> routes, bool isRouteInfo, RouteManager mn, List<Segment> segments, List<NetworkPoint> points) {
            InitializeComponent();
            this.routes = routes;
            _routeManager = mn;
            DataGridRoutes.ItemsSource = routes;
            this.segments = segments;
            this.points = points;

            if (isRouteInfo) {
                SelectRoute.Visibility = Visibility.Collapsed;
            } else {
                MoreInfo.Visibility = Visibility.Collapsed;
                RemoveRoute.Visibility = Visibility.Collapsed;
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

        private void CloseButton_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void TitleBar_MouseMove(object sender, MouseEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                DragMove();
            }
        }

        private void MoreInfo_Click(object sender, RoutedEventArgs e) {
            if (DataGridRoutes.SelectedItem != null) {
                RouteWindow routeWindow = new RouteWindow(_routeManager, (RouteUI)DataGridRoutes.SelectedItem, segments, points);
                routeWindow.ShowDialog();
            } else {
                MessageBox.Show("Please select a route to view it's info", "No route selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        } 

        private void RemoveRoute_Click(object sender, RoutedEventArgs e) {
            try {
                if (DataGridRoutes.SelectedItem == null) {
                    MessageBox.Show("Please select a route to remove", "No route selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                _routeManager.DeleteRoute(RouteMapper.MapToDomain((RouteUI)DataGridRoutes.SelectedItem));
                routes.Remove((RouteUI)DataGridRoutes.SelectedItem);
            } catch (ApplicationException ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            } catch (Exception ex) {
                MessageBox.Show("An unexpected error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
