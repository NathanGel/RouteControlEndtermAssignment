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
using RouteBeheerBL.Model;
using WPFRouteBeheerUI.Model;

namespace WPFRouteBeheerUI {
    /// <summary>
    /// Interaction logic for RouteManagmentWindow.xaml
    /// </summary>
    public partial class RouteManagmentWindow : Window {
        private ObservableCollection<RouteUI> routes;
        public RouteManagmentWindow(ObservableCollection<RouteUI> routes) {
            InitializeComponent();
            this.routes = routes;
            DataGridRoutes.ItemsSource = this.routes;
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
