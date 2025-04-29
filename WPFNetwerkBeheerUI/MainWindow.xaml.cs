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
        private ObservableCollection<NetworkPointUI> points = new();
        // de lijst van punten die ik gebruik om op het canvas te tekenen.
        // Observable collection omdat ik de UI wil updaten wanneer er iets
        // veranderd in de lijst

        private ObservableCollection<SegmentUI> segments; 
        // de lijst van segmenten die ik gebruik om op het canvas te tekenen
        // Observable collection idem met points

        private Dictionary<NetworkPointUI, UIElement> pointElements = new Dictionary<NetworkPointUI, UIElement>(); 
        // dictionary om de punten te koppelen aan hun UI elementen
        // om ze makkelijk te kunnen verwijderen zonder dat ik telkens
        // na elke verandering het hele canvas opnieuw moet tekenen

        private NetworkPointUI selectedPoint; 
        //ik sla dit punt op om in de RemoveLocation/UpdateLocation en RemoveConnection/AddConnection
        // dit punt te gebruiken en vast te stellen of er wel een punt geselecteerd is na de click events op de knoppen

        private NetworkPointUI clickedLocation; 
        // dit punt sla ik op om te gebruiken in de AddLocation  en om te
        // kijken of er wel een locatie geselecteerd is na het click event op de knop

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
                DrawLine(segment.StartPoint, segment.EndPoint);
            }
            DrawAllLines();

            List<NetworkPointUI> pointsUI = new(nm.GetNetworkPoints().Select( np => NetworkPointMapper.MapFromDomain(np)));
            foreach (var point in pointsUI) {
                points.Add(new NetworkPointUI(point.X, point.Y));
            }
        }

        private void Points_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.Action == NotifyCollectionChangedAction.Add) {
                foreach (NetworkPointUI point in e.NewItems) {
                    DrawPoint(point);
                }
            } else if (e.Action == NotifyCollectionChangedAction.Remove) {
                foreach (NetworkPointUI point in e.OldItems) {
                    if (pointElements.TryGetValue(point, out UIElement element)) {
                        canvas.Children.Remove(element);
                        pointElements.Remove(point);
                    }
                }
            }
        }

        public void DrawPoint(NetworkPointUI point) {
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

        private void DrawLine(NetworkPointUI p1, NetworkPointUI p2) {
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
                DrawLine(segment.StartPoint, segment.EndPoint);
            }
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Point mousePosPoint = e.GetPosition(canvas);
            NetworkPointUI mousePos = new(mousePosPoint.X, mousePosPoint.Y);
            NetworkPointUI nearbyPoint = FindNearbyPoint(mousePos);

            if (nearbyPoint != default) {
                HighlightPoint(selectedPoint);
            } else {
                clickedLocation = mousePos;
                HighlightPoint(clickedLocation);
            }
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            Point mousePosPoint = e.GetPosition(canvas);
            NetworkPointUI mousePos = new(mousePosPoint.X, mousePosPoint.Y);
            NetworkPointUI nearbyPoint = FindNearbyPoint(mousePos);
            // deze checks bepalen of er een bestaand punt geselescteerd is. Dit bepaald dan weer welke opties er zichtbaar zijn het contextmenu
            if(nearbyPoint != default) {
                selectedPoint = nearbyPoint;
                HighlightPoint(selectedPoint);
                MenuItemAddNetworkPoint.Visibility = Visibility.Collapsed;
                MenuItemRemoveNetworkPoint.Visibility = Visibility.Visible;
                MenuItemUpdateNetworkPoint.Visibility = Visibility.Visible;
            } else {
                clickedLocation = mousePos;
                HighlightPoint(clickedLocation);
                MenuItemRemoveNetworkPoint.Visibility = Visibility.Collapsed;
                MenuItemUpdateNetworkPoint.Visibility = Visibility.Collapsed;
                MenuItemAddNetworkPoint.Visibility = Visibility.Visible;
            }
        }

        private NetworkPointUI FindNearbyPoint(NetworkPointUI p) {
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

        private void HighlightPoint(NetworkPointUI p) {
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

        private UIElement highlightElement; // Ik gebruik hier UIElement om het highlight punt op te slaan en achteraf makkelijk te kunnen verwijderen

        private void RemovePreviousHighlight() {
            if (highlightElement != null && canvas.Children.Contains(highlightElement)) {
                canvas.Children.Remove(highlightElement);
            }
        }

        private void AddNetworkPoint_Click(object sender, RoutedEventArgs e) {
            if (clickedLocation != default) {
                NetworkPointUI newPoint = new(clickedLocation.X, clickedLocation.Y);
                newPoint.Id = nm.AddNetworkPoint(NetworkPointMapper.MapToDomain(newPoint));
                points.Add(clickedLocation);
                RemovePreviousHighlight();
                clickedLocation = default;
            } else {
                MessageBox.Show("No location selected for new network point");
            }
        }

        private void RemoveNetworkPoint_Click(object sender, RoutedEventArgs e) {
            if (selectedPoint != default) {
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