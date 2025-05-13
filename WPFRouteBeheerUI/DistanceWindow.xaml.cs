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

namespace WPFRouteBeheerUI {
    /// <summary>
    /// Interaction logic for DistanceWindow.xaml
    /// </summary>
    public partial class DistanceWindow : Window {
        public DistanceWindow(Route route, Segment segment) {
            InitializeComponent();

            double distance = Route.GetDistance(segment.StartPoint, segment.EndPoint);

            Point1Text.Text = $"Point 1: ({segment.StartPoint.X:0.##}, {segment.StartPoint.Y:0.##})";
            Point2Text.Text = $"Point 2: ({segment.StartPoint.X:0.##} ,  {segment.StartPoint.Y:0.##})";
            DistanceText.Text = $"Distance: {distance:0.##}";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            this.DragMove();
        }
    }
}
