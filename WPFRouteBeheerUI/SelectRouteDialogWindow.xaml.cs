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

namespace WPFRouteBeheerUI
{
    /// <summary>
    /// Interaction logic for SelectRouteDialogWindow.xaml
    /// </summary>
    public partial class SelectRouteDialogWindow : Window
    {
        public Route route;
        private List<Route> routes = new();
        public SelectRouteDialogWindow(List<Route> routes)
        {
            InitializeComponent();
            this.routes = routes;
            DataGridRoutes.ItemsSource = routes;
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
            route = (Route)DataGridRoutes.SelectedItem;
            DialogResult = true;
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
    }
}
