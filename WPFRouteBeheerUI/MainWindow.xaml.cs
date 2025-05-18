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
using RouteBeheerBL.Managers;
using RouteBeheerDL;
using RouteBeheerBL.Model;
using System.Collections.ObjectModel;
using System.Net;
using RouteBeheerBL.Exceptions;
using WPFRouteBeheerUI.Model;
using WPFRouteBeheerUI.Mappers;

namespace WPFRouteBeheerUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private List<NetworkPoint> points;
        private Dictionary<Ellipse, NetworkPoint> pointElements = new();
        private Dictionary<NetworkPoint, Ellipse> elementPoints = new();
        private List<Segment> segments;
        private Dictionary<Segment, Line> segmentElements = new();
        private ObservableCollection<RouteUI> routes;
        private readonly string connectionString = @"Data Source=nathan\SQLExpress;Initial Catalog=NetworkControlTesting;Integrated Security=True;Trust Server Certificate=True";
        private NetworkManager nm;
        private RouteManager rm;
        private bool addRouteClicked;
        private List<(NetworkPoint, bool)> selectedPoints = new();
        private List<(Segment, bool)> selectedSegments = new();
        private Ellipse selectedPoint;

        public MainWindow() {
            rm = new(new RouteRepository(connectionString));
            points = new List<NetworkPoint>();
            segments = new List<Segment>();
            InitializeComponent();
            ReadFromDatabase();
            DrawNetwork();
            try {
                routes = new ObservableCollection<RouteUI>(rm.GetAllRoutes().Select(r => RouteMapper.MapFromDomain(r)));
                foreach (var route in routes) {
                    foreach (var (point, _) in route.Stops) {
                        var matchingPoint = points.FirstOrDefault(p => p.Equals(point));
                        if (matchingPoint != null) {
                            point.Facilities = matchingPoint.Facilities;
                        }
                    }
                }

            } catch (RouteException ex) {
                MessageBox.Show("An error occured because the route does not meet the specified requirements", ex.Message, MessageBoxButton.OK, MessageBoxImage.Warning);
            } catch (ApplicationException ex) {
                MessageBox.Show("An error occured while retrieving the routes for initialization", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            } catch (Exception ex) { 
                MessageBox.Show("An unexpected error occured:  " + ex.Message, "System Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ReadFromDatabase() {
            nm = new(new NetworkRepository(connectionString), new RouteRepository(connectionString));
            try {
                List<Segment> segmentsFromDb = new(nm.GetSegments());
                foreach (var segment in segmentsFromDb) {
                    segments.Add(segment);
                }

                List<NetworkPoint> pointsFromDb = new(nm.GetNetworkPoints());
                foreach (var point in pointsFromDb) {
                    points.Add(new NetworkPoint(point.Id, point.X, point.Y, point.Facilities));
                }
            } catch (ApplicationException ex) { // dit catch-blok vangt elke mogelijke sqlexception die gegooid werden in de repo. De SqlException werd vertaald naar een ApplicationException
                MessageBox.Show("An error occured while retrieving the network for initialization", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            } catch (Exception ex) { // dit catch-blok dient als fallback indien er ergens een exception gegooid word die onverwacht is  
                MessageBox.Show("An unexpected error occured:  " + ex.Message, "System Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DrawNetwork() {
            foreach (var segment in segments) {
                // Create a new Line
                Line line = new Line {
                    Stroke = Brushes.OrangeRed,
                    StrokeThickness = 1,
                    X1 = segment.StartPoint.X * 0.55,
                    Y1 = segment.StartPoint.Y * 0.55,
                    X2 = segment.EndPoint.X * 0.55,
                    Y2 = segment.EndPoint.Y * 0.55
                };

                canvas.Children.Add(line);
                segmentElements.Add(segment, line);
            }
            foreach (var point in points) {
                Ellipse ellipse = new Ellipse {
                    Fill = Brushes.MediumTurquoise,
                    Width = 7,
                    Height = 7,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                };

                Canvas.SetLeft(ellipse, point.X * 0.55 - (ellipse.Width / 2)); // de berekening die hier in plaats vind zorgt ervoor dat
                Canvas.SetTop(ellipse, point.Y * 0.55 - (ellipse.Width / 2));   // het midden van de ellipse overeenstemt met de exacte coordinaten
                                                                         // van het punt eerder zorgde dit voor problemen met de lijnen 
                ellipse.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;
                ellipse.MouseRightButtonDown += Ellipse_MouseRightButtonDown;

                canvas.Children.Add(ellipse);
                pointElements[ellipse] = point;
                elementPoints[point] = ellipse;
            }
        }

        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (sender is Ellipse ellipse) {
                selectedPoint = ellipse;
            }
        }

        private void Ellipse_MouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            if (sender is Ellipse ellipse) {
                selectedPoint = ellipse;
            }
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            //rechtermuisklik in combinatie met addrouteclicked betekent dat je het punt niet wenst te selecteren als een stopplaats
            // indien het gaat over het eerste punt is dit standaard ingebouwd dat dit wel een stopplaats is
            if (addRouteClicked && selectedPoint != default) { //nakijken of er gelikt is op addroute en er effectief een punt geselecteerd is
                if (selectedPoints.Count == 0) { // indien het het eerste punt is voeg ik dit toe als een stopplaats
                    selectedPoints.Add((pointElements[selectedPoint], true));
                    HighlightPoint(selectedPoint, true);
                } else {
                    var lastPoint = selectedPoints[^1].Item1; //ik kijk na op basis van het vorig punt en het geselecteerde punt of deze ergens samen in een segment bestaan
                    var currentPoint = pointElements[selectedPoint];

                    Segment? segment = segments.FirstOrDefault(
                        s =>
                            (s.StartPoint.Equals(lastPoint) && s.EndPoint.Equals(currentPoint)) ||
                            (s.StartPoint.Equals(currentPoint) && s.EndPoint.Equals(lastPoint))
                    );

                    if (segment != null && !selectedSegments.Any(s => s.Item1.Equals(segment))) { // zoja kijk ik of het gaat over een omgekeerd segment of niet
                        bool isReverse = segment.StartPoint.Equals(currentPoint);

                        selectedSegments.Add((segment, isReverse)); //toevoegen aan de lijst van segmenten
                        selectedPoints.Add((currentPoint, false)); //toevoegen aan de lijst van stops
                        HighlightPoint(selectedPoint, false); // aanduiden op canvas
                    }
                }
                selectedPoint = default;
            }
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            //rechtermuisklik in combinatie met addrouteclicked betekent dat je het punt wenst te selecteren als een stopplaats
            if (addRouteClicked && selectedPoint != default) { //nakijken of er gelikt is op addroute en er effectief een punt geselecteerd is
                if (selectedPoints.Count == 0) { // indien het het eerste punt is voeg ik dit toe als een stopplaats
                    selectedPoints.Add((pointElements[selectedPoint], true));
                    HighlightPoint(selectedPoint, true);
                } else {
                    var lastPoint = selectedPoints[^1].Item1; //ik kijk na op basis van het vorig punt en het geselecteerde punt of deze ergens samen in een segment bestaan
                    var currentPoint = pointElements[selectedPoint];

                    Segment? segment = segments
                        .FirstOrDefault(
                        s =>
                            (s.StartPoint.Equals(selectedPoints[^1].Item1) && s.EndPoint.Equals(pointElements[selectedPoint])) ||
                            (s.StartPoint.Equals(pointElements[selectedPoint]) && s.EndPoint.Equals(selectedPoints[^1].Item1))
                    );


                    if (segment != null && !selectedSegments.Any(s => s.Item1.Equals(segment))) { // zoja kijk ik of het gaat over een omgekeerd segment of niet
                        bool isReverse = segment.StartPoint.Equals(currentPoint);

                        selectedSegments.Add((segment, isReverse));  //toevoegen aan de lijst van segmenten
                        selectedPoints.Add((currentPoint, true)); //toevoegen aan de lijst van stops
                        HighlightPoint(selectedPoint, true); // aanduiden op canvas
                    }
                }
                selectedPoint = default;
            }
        }

        private List<Ellipse> highlightedPoints = new();

        private void HighlightPoint(Ellipse ellipse, bool isStop) {
            if (ellipse != null) {
                if (isStop) {
                    ellipse.Stroke = Brushes.LightGreen;
                    ellipse.Fill = Brushes.LightGreen;
                    highlightedPoints.Add(ellipse);
                } else {
                    ellipse.Stroke = Brushes.Red;
                    ellipse.Fill = Brushes.Red;
                    highlightedPoints.Add(ellipse);
                }
            }
        }

        private void BtnManageRoutes_Click(object sender, RoutedEventArgs e) {
            RemoveAllCurrentHighLights();
            SelectRouteDialogWindow window = new SelectRouteDialogWindow(routes, true, rm);
            window.Show();
        }

        private void BtnAddRoute_Click(object sender, RoutedEventArgs e) {
            RemoveAllCurrentHighLights();
            if (!addRouteClicked) {
                addRouteClicked = true;
                buttonConfirmSelection = new() {
                    Content = "Confirm Selection",
                    Foreground = Brushes.White,
                    Background = Brushes.Green,
                    BorderBrush = Brushes.Green,
                    Height = 30
                };

                buttonConfirmSelection.Click += BtnConfirmSelection_Click;

                top.Children.Add(buttonConfirmSelection);
                MessageBox.Show("• Left click to select a point \n• Right click to select it as a stop", "Select Points for Route", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private Button buttonConfirmSelection;

        private void BtnConfirmSelection_Click(object sender, RoutedEventArgs e) {
            RouteNameDialogWindow dialog = new();
            bool? result = dialog.ShowDialog();
            if (result == true) {
                try {
                    RouteUI route = new(dialog.RouteName, [.. selectedSegments], [.. selectedPoints]);
                    route.Id = rm.AddRoute(RouteMapper.MapToDomainWithoutId(route));
                    routes.Add(route);
                    //maybe something that shows the route was added???
                } catch (RouteException ex) {
                    MessageBox.Show("An error occured because the route does not meet the specified requirements", ex.Message, MessageBoxButton.OK, MessageBoxImage.Warning);
                } catch (ApplicationException ex) {
                    MessageBox.Show("An error occured while adding a route", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                } catch (Exception ex) {
                    MessageBox.Show("An uncexpected error occured: ", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            RemoveAllCurrentHighLights();
        }

        private void BtnRemoveAllCurrentHighlights_Click(object sender, RoutedEventArgs e) {
            RemoveAllCurrentHighLights();
        }

        private void RemoveAllCurrentHighLights() {
            canvas.Children.Clear();
            pointElements.Clear();
            segmentElements.Clear();
            highlightedPoints.Clear();
            addRouteClicked = false;
            top.Children.Remove(buttonConfirmSelection);
            selectedPoints.Clear();
            selectedSegments.Clear();
            selectedPoint = null;
            contextTop.Children.Clear();
            DrawNetwork();
        }

        private void BtnSelectRoute_Click(object sender, RoutedEventArgs e) {
            RemoveAllCurrentHighLights();
            SelectRouteDialogWindow window = new(routes, false, rm);
            bool? result = window.ShowDialog();
            if (result == true) {
                HighLightSelectedRoute(window.route);
                //selectedRoute = window.route;
            }
        }

        private void HighLightSelectedRoute(RouteUI route) {
            foreach (var point in route.Stops) {
                Ellipse e = pointElements.FirstOrDefault(p => p.Value.Equals(point.Item1)).Key;
                HighlightPoint(e, point.Item2);
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