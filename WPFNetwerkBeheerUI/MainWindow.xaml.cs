using System.Drawing;
using System.Net;
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
using RouteBeheerBL.Interfaces;
using RouteBeheerBL.Managers;
using RouteBeheerBL.Model;
using RouteBeheerDL;
using Point = System.Windows.Point;
// Or remove using System.Drawing; if you have it

namespace WPFNetwerkBeheerUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        double minX = 0;
        double minY = 0;
        double maxX = 1000;
        double maxY = 1000;
        List<Point> points = new();
        private Point selectedPoint;
        private Point clickedLocation;
        private readonly string connectionString = @"Data Source=NATHAN\SQLExpress;Initial Catalog=NetworkControlTesting;Integrated Security=True;Trust Server Certificate=True";
        
        public MainWindow() {
            InitializeComponent();
            ReadFromDatabase();
        }

        public void ReadFromDatabase() {
            INetworkRepository repo = new NetworkRepository(connectionString);
            NetworkManager nm = new(repo);
            List<RouteBeheerBL.Model.Stretch> stretches = nm.ReadNetwork();
            foreach(var stretch in stretches) {
                foreach(var point in stretch.NetworkPointSequence){
                    points.Add(new(point.Value.X, point.Value.Y));
                }
            }
            DrawAllLines(stretches);
            DrawPoints(points);
        }

        public void DrawPoints(List<Point> points) {
            foreach (var point in points) {
                Ellipse ellipse = new Ellipse {
                    Fill = Brushes.MediumTurquoise,
                    Width = 8,
                    Height = 8,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };
                Canvas.SetLeft(ellipse, point.X - (ellipse.Width / 2)); // de berekening die hier in plaats vind zorgt ervoor dat
                Canvas.SetTop(ellipse, point.Y - (ellipse.Width /2));   // het midden van de ellipse overeenstemt met de exacte coordinaten
                                                                        // van het punt eerder zorgde dit voor problemen met de lijnen 
                                                                        // die net naast de punten liepen

                // Fix: Ensure that the Canvas object is referenced correctly
                if (this.FindName("canvas") is Canvas canvas) {
                    canvas.Children.Add(ellipse);
                } else {
                    throw new InvalidOperationException("Canvas not found in the current context.");
                }
            }
        }

        public void DrawLine(Point p1, Point p2) {
            // Create a new Line
            Line line = new Line {
                Stroke = Brushes.OrangeRed,
                StrokeThickness = 1,
                X1 = p1.X,                // Start at the left of the canvas
                Y1 = p1.Y,                // Start at the top of the canvas
                X2 = p2.X,      // End at the right of the canvas
                Y2 = p2.Y      // End at the bottom of the canvas
            };

            // Add the line to the canvas
            if (this.FindName("canvas") is Canvas canvas) {
                canvas.Children.Add(line);
            } else {
                throw new InvalidOperationException("Canvas not found in the current context.");
            }
        }
        public void DrawAllLines(List<RouteBeheerBL.Model.Stretch> stretches) {
            foreach (var stretch in stretches) {
                for(int i = 1; i<stretch.NetworkPointSequence.Count; i++) {
                    Point p1 = new(stretch.NetworkPointSequence[i - 1].X, stretch.NetworkPointSequence[i-1].Y);
                    Point p2 = new(stretch.NetworkPointSequence[i].X, stretch.NetworkPointSequence[i].Y);
                    DrawLine(p1, p2);
                }
            }
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Point mousePos = e.GetPosition(canvas);
            Point nearbyPoint = FindNearbyPoint(mousePos);

            if (nearbyPoint != default) {
                HighlightPoint(selectedPoint);
            } else {
                clickedLocation = mousePos;
                HighlightPoint(clickedLocation);
            }
        }

        private Point FindNearbyPoint(Point p) {
            double tolerance = 5; 
            foreach(var point in points) {
                if (Math.Abs(point.X - p.X) < tolerance && Math.Abs(point.Y - p.Y) < tolerance) {
                    selectedPoint = point;
                    //MessageBox.Show($"Selected Point: {selectedPoint}");
                    return selectedPoint;
                }
            }
            return default;
        }

        private void HighlightPoint(Point p) {
            Ellipse highlightCircle = new Ellipse {
                Width = 10,
                Height = 10,
                Fill = Brushes.Red,
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };

            Canvas.SetLeft(highlightCircle, p.X - highlightCircle.Width / 2);
            Canvas.SetTop(highlightCircle, p.Y - highlightCircle.Height / 2);

            // Remove any previous highlight
            RemovePreviousHighlight();

            // Add the highlight to canvas
            canvas.Children.Add(highlightCircle);
            highlightElement = highlightCircle;
        }

        private UIElement highlightElement;

        private void RemovePreviousHighlight() {
            if (highlightElement != null && canvas.Children.Contains(highlightElement)) {
                canvas.Children.Remove(highlightElement);
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

        private void AddNetworkPoint_Click(object sender, RoutedEventArgs e) {
            if (clickedLocation != default) {
                MessageBox.Show($"Add Network Point clicked at {clickedLocation}");
                clickedLocation = default;
            } else {
                MessageBox.Show("No location selected for new network point");
            }
        }

        private void AddConnection_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("Add Connection clicked");
        }

        private void UpdateNetworkPoint_Click(object sender, RoutedEventArgs e) {
            if (selectedPoint != default) {
                MessageBox.Show("Update Network Point clicked");
                selectedPoint = default;
            } else {
                MessageBox.Show("No network point selected");
            }
        }

        private void RemoveNetworkPoint_Click(object sender, RoutedEventArgs e) {
            if (selectedPoint != default) {
                MessageBox.Show("Remove Network Point clicked");
                selectedPoint = default;
            } else {
                MessageBox.Show("No network point selected");
            }
        }

        private void RemoveConnection_Click(object sender, RoutedEventArgs e) {
            if (selectedPoint != default) {
                MessageBox.Show("Remove Connection clicked");
                selectedPoint = default;
            } else {
                MessageBox.Show("No network point selected");
            }
        }
    }
}