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
        private readonly string connectionString = "Data Source=nathans-laptop\\sqlexpress;Initial Catalog=NetworkControlTesting;Integrated Security=True;Trust Server Certificate=True";
        
        public MainWindow() {
            InitializeComponent();
            ReadFromDatabase();
        }

        public void ReadFromDatabase() {
            INetworkRepository repo = new NetworkRepository(connectionString);
            NetworkManager nm = new(repo);
            List<RouteBeheerBL.Model.Stretch> stretches = nm.ReadNetwork();
            foreach(var stretch in stretches) {
                foreach(var point in stretch.NetworkPoints){
                    points.Add(new(point.X / 2, point.Y / 2));
                }
                Draw(stretch);
            }
            //DrawAllLines(stretches);
            //DrawPoints(points);
        }

        public void Draw(RouteBeheerBL.Model.Stretch stretch) {
            foreach (var point in stretch.NetworkPoints) {
                Ellipse ellipse = new() {
                    Fill = Brushes.MediumTurquoise,
                    Width = 4,
                    Height = 4,
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.5
                };
                Canvas.SetLeft(ellipse, point.X / 2 - (ellipse.Width / 2)); // de berekening die hier in plaats vind zorgt ervoor dat
                Canvas.SetTop(ellipse, point.Y / 2 - (ellipse.Width / 2));   // het midden van de ellipse overeenstemt met de exacte coordinaten

                if (this.FindName("canvas") is Canvas canvas) {
                    canvas.Children.Add(ellipse);
                } else {
                    throw new InvalidOperationException("Canvas not found in the current context.");
                }
            }
            for (int i = 1; i < stretch.NetworkPoints.Count; i++) {
                Point p1 = new(stretch.NetworkPoints[i - 1].X / 2, stretch.NetworkPoints[i - 1].Y / 2 );
                Point p2 = new(stretch.NetworkPoints[i].X / 2 , stretch.NetworkPoints[i].Y / 2);
                Line line = new() {
                    Stroke = Brushes.OrangeRed,
                    StrokeThickness = 1,
                    X1 = p1.X,
                    Y1 = p1.Y,
                    X2 = p2.X,
                    Y2 = p2.Y
                };
                // Add the line to the canvas
                if (this.FindName("canvas") is Canvas canvas) {
                    canvas.Children.Add(line);
                    System.Diagnostics.Debug.WriteLine($"Drawing line from {stretch.NetworkPoints[i-1].Id} ({p1.X}, {p1.Y}) to {stretch.NetworkPoints[i].Id} ({p2.X}, {p2.Y})");
                } else {
                    throw new InvalidOperationException("Canvas not found in the current context.");
                }
            }
            TextBlock stretchLabel = new TextBlock {
                Text = $"S{stretch.Id}",
                
                FontSize = 10,
                FontWeight = FontWeights.Bold
            };

            // Position label at first point of the stretch
            Point labelPoint = new(stretch.NetworkPoints[0].X / 2, stretch.NetworkPoints[0].Y / 2);
            Canvas.SetLeft(stretchLabel, labelPoint.X);
            Canvas.SetTop(stretchLabel, labelPoint.Y - 15); // Position slightly above the start point

            if (this.FindName("canvas") is Canvas canvas2) {
                canvas2.Children.Add(stretchLabel);
            }
        }
        public void DrawPoints(List<Point> points) {
            foreach (var point in points) {
                Ellipse ellipse = new Ellipse {
                    Fill = Brushes.MediumTurquoise,
                    Width = 4,
                    Height = 4,
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.5
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
                for (int i = 1; i < stretch.NetworkPoints.Count; i++) {
                    Point p1 = new(stretch.NetworkPoints[i - 1].X/2, stretch.NetworkPoints[i - 1].Y / 2);
                    Point p2 = new(stretch.NetworkPoints[i].X / 2, stretch.NetworkPoints[i].Y / 2);
                    DrawLine(p1, p2);
                }
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
            MessageBox.Show("Add Network Point clicked");
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