using System.Collections.ObjectModel;
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
using WPFNetwerkBeheerUI.Mappers;
using WPFNetwerkBeheerUI.Model;
using System.Collections.Specialized;

namespace WPFNetwerkBeheerUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private ObservableCollection<Point> points = new(); 
        private ObservableCollection<SegmentUI> segments;
        private Dictionary<Point, UIElement> pointElements = new Dictionary<Point, UIElement>(); // dictionary om de punten te koppelen aan hun UI elementen
                                                                                                 // om ze makkelijk te kunnen verwijderen zonder dat ik telkens
                                                                                                 // na elke verandering het hele canvas opnieuw moet tekenen

        private Point selectedPoint;
        private Point clickedLocation;
        private readonly string connectionString = @"Data Source=NATHAN\SQLExpress;Initial Catalog=NetworkControlTesting;Integrated Security=True;Trust Server Certificate=True";
        private NetworkManager nm;

        public MainWindow() {
            InitializeComponent();
            points.CollectionChanged += Points_CollectionChanged; // de points collection abboneren op de CollectionChanged event

            ReadFromDatabase();
        }

        private void ReadFromDatabase() {
            nm = new(new NetworkRepository(connectionString));
            segments = new ObservableCollection<SegmentUI>(nm.GetSegments().Select(sm => SegmentMapper.MapFromDomain(sm)));

            foreach (var segment in segments) {
                Point p1 = new(segment.StartPoint.X, segment.StartPoint.Y);
                Point p2 = new(segment.EndPoint.X, segment.EndPoint.Y);
                DrawLine(p1, p2);
            }
            DrawAllLines();

            List<NetworkPointUI> pointsUI = new(nm.GetNetworkPoints().Select( np => NetworkPointMapper.MapFromDomain(np)));
            foreach (var point in pointsUI) {
                this.points.Add(new Point(point.X, point.Y));
            }
        }

        private void Points_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.Action == NotifyCollectionChangedAction.Add) {
                foreach (Point point in e.NewItems) {
                    DrawPoint(point);
                }
            } else if (e.Action == NotifyCollectionChangedAction.Remove) {
                foreach (Point point in e.OldItems) {
                    // Remove the visual element for this point
                    if (pointElements.TryGetValue(point, out UIElement element)) {
                        canvas.Children.Remove(element);
                        pointElements.Remove(point);
                    }
                }
            }
        }
        public void DrawPoint(Point point) {
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
            canvas.Children.Add(ellipse);
            pointElements[point] = ellipse;
        }

        private void DrawLine(Point p1, Point p2) {
            // Create a new Line
            Line line = new Line {
                Stroke = Brushes.OrangeRed,
                StrokeThickness = 1,
                X1 = p1.X,     
                Y1 = p1.Y,     
                X2 = p2.X,     
                Y2 = p2.Y      
            };

            canvas.Children.Add(line);
        }

        private void DrawAllLines() {
            foreach (var segment in segments) {
                Point p1 = new(segment.StartPoint.X, segment.StartPoint.Y);
                Point p2 = new(segment.EndPoint.X, segment.EndPoint.Y);
                DrawLine(p1, p2);
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

        private void AddNetworkPoint_Click(object sender, RoutedEventArgs e) {
            if (clickedLocation != default) {
                MessageBox.Show($"Add Network Point clicked at {clickedLocation}");
                NetworkPointUI newPoint = new(clickedLocation.X, clickedLocation.Y);
                nm.AddNetworkPoint(NetworkPointMapper.MapToDomain(newPoint));
                points.Add(clickedLocation);
                RemovePreviousHighlight();
                clickedLocation = default;
            } else {
                MessageBox.Show("No location selected for new network point");
            }
        }
        private void RemoveNetworkPoint_Click(object sender, RoutedEventArgs e) {
            if (selectedPoint != default) {
                MessageBox.Show("Remove Network Point clicked");
                nm.RemoveNetworkPoint(NetworkPointMapper.MapToDomain(new NetworkPointUI(selectedPoint.X, selectedPoint.Y)));
                points.Remove(selectedPoint);
                RemovePreviousHighlight();
                selectedPoint = default;
            } else {
                MessageBox.Show("No network point selected");
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


        private void RemoveConnection_Click(object sender, RoutedEventArgs e) {
            if (selectedPoint != default) {
                MessageBox.Show("Remove Connection clicked");
                selectedPoint = default;
            } else {
                MessageBox.Show("No network point selected");
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
    }
}