using RouteBeheerBL.Managers;
using RouteBeheerDL;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WPFNetwerkBeheerUI.Mappers;
using WPFNetwerkBeheerUI.Model;
using Point = System.Windows.Point;

namespace WPFNetwerkBeheerUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private ObservableCollection<NetworkPointUI> points = new();
        // de lijst van punten die ik gebruik om op het canvas te tekenen.
        // Observable collection omdat ik de UI wil updaten wanneer er iets
        // veranderd in de lijst

        private ObservableCollection<SegmentUI> segments = new();
        // de lijst van segmenten die ik gebruik om op het canvas te tekenen
        // Observable collection idem met points

        private Dictionary<NetworkPointUI, Ellipse> pointElements = new Dictionary<NetworkPointUI, Ellipse>();
        // dictionary om de punten te koppelen aan hun UI elementen
        // om ze makkelijk te kunnen verwijderen zonder dat ik telkens
        // na elke verandering het hele canvas opnieuw moet tekenen

        private Dictionary<SegmentUI, Line> segmentElements = new Dictionary<SegmentUI, Line>();

        private NetworkPointUI selectedPoint; 
        //ik sla dit punt op om in de RemoveLocation/UpdateLocation en RemoveConnection/AddConnection
        // dit punt te gebruiken en vast te stellen of er wel een punt geselecteerd is na de click events op de knoppen

        private NetworkPointUI clickedLocation; 
        // dit punt sla ik op om te gebruiken in de AddLocation  en om te
        // kijken of er wel een locatie geselecteerd is na het click event op de knop

        private bool addConnectionClicked = false;

        private SegmentUI newConnection;

        private readonly string connectionString = @"Data Source=NATHAN\SQLExpress;Initial Catalog=NetworkControlTesting;Integrated Security=True;Trust Server Certificate=True";

        private NetworkManager nm;

        public MainWindow() {
            InitializeComponent();
            points.CollectionChanged += Points_CollectionChanged; // de points collection abboneren op de CollectionChanged event
            segments.CollectionChanged += Segments_CollectionChanged;
            ReadFromDatabase();
        }

        private void ReadFromDatabase() {
            nm = new(new NetworkRepository(connectionString));
            
            List<SegmentUI> segmentsUI = new(nm.GetSegments().Select(sm => SegmentMapper.MapFromDomain(sm)));
            foreach (var segment in segmentsUI) {
                segments.Add(segment);
            }

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
                    if (pointElements.TryGetValue(point, out Ellipse ellipse)) {
                        canvas.Children.Remove(ellipse);
                        pointElements.Remove(point);
                    }
                }
            }
        }

        private void Segments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if(e.Action == NotifyCollectionChangedAction.Add) {
                foreach(SegmentUI segment in e.NewItems) {
                    DrawLine(segment);
                }
            } else if(e.Action == NotifyCollectionChangedAction.Remove) {
                foreach(SegmentUI segment in e.OldItems) {
                    if(segmentElements.TryGetValue(segment, out Line line)) {
                        canvas.Children.Remove(line);
                        segmentElements.Remove(segment);
                    }
                }
            }
        }

        private void DrawPoint(NetworkPointUI point) {
            Ellipse ellipse = new Ellipse {
                Fill = Brushes.MediumTurquoise,
                Width = 8,
                Height = 8,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            
            Canvas.SetLeft(ellipse, point.X - (ellipse.Width / 2)); // de berekening die hier in plaats vind zorgt ervoor dat
            Canvas.SetTop(ellipse, point.Y - (ellipse.Width / 2));   // het midden van de ellipse overeenstemt met de exacte coordinaten
                                                                    // van het punt eerder zorgde dit voor problemen met de lijnen 
            canvas.Children.Add(ellipse);
            pointElements[point] = ellipse;
        }

        private void DrawLine(SegmentUI segment) {
            // Create a new Line
            Line line = new Line {
                Stroke = Brushes.OrangeRed,
                StrokeThickness = 1,
                X1 = segment.StartPoint.X,     
                Y1 = segment.StartPoint.Y,     
                X2 = segment.EndPoint.X,     
                Y2 = segment.EndPoint.Y
            };

            canvas.Children.Add(line);
            segmentElements.Add(segment, line);
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Point mousePos = e.GetPosition(canvas);
            NetworkPointUI clickLocation = new(mousePos.X, mousePos.Y);
            NetworkPointUI nearbyPoint = FindNearbyPoint(clickLocation);
            if (nearbyPoint != default && !addConnectionClicked) {
                ShowCoordinates(nearbyPoint);
            } else if(nearbyPoint != default && addConnectionClicked) {
                selectedPoint = nearbyPoint;
                HighlightPoint(selectedPoint);
                AddConnection(nearbyPoint);
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
                MenuItemAddConnection.Visibility = Visibility.Collapsed;
                MenuItemRemoveConnection.Visibility = Visibility.Collapsed;
            } else {
                clickedLocation = mousePos;
                HighlightPoint(clickedLocation);
                MenuItemRemoveNetworkPoint.Visibility = Visibility.Collapsed;
                MenuItemUpdateNetworkPoint.Visibility = Visibility.Collapsed;
                MenuItemAddNetworkPoint.Visibility = Visibility.Visible;
                MenuItemAddConnection.Visibility = Visibility.Visible;
                MenuItemRemoveConnection.Visibility = Visibility.Visible;
            }
        }

        private TextBlock coordinatesTextBlock; // Ik gebruik dit veld om achteraf de textblock makkelijk te verwijderen van het canvas

        private void ShowCoordinates(NetworkPointUI point) {
            RemovePreviousCoordinates();
            coordinatesTextBlock = new TextBlock();
            coordinatesTextBlock.Text = $"X:{point.X}    Y:{point.Y}";
            Canvas.SetLeft(coordinatesTextBlock, point.X);
            Canvas.SetTop(coordinatesTextBlock, point.Y);

            canvas.Children.Add(coordinatesTextBlock);
        }

        private void RemovePreviousCoordinates() {
            if(coordinatesTextBlock != null && canvas.Children.Contains(coordinatesTextBlock))
                canvas.Children.Remove(coordinatesTextBlock);
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

        private Ellipse highlightElement; // Ik gebruik dit veld om achteraf makkelijk te verwijderen van het canvas

        private void RemovePreviousHighlight() {
            if (highlightElement != null && canvas.Children.Contains(highlightElement))
                canvas.Children.Remove(highlightElement);
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
            RemovePreviousHighlight();
            newConnection = new();
            addConnectionClicked = true;
            MessageBox.Show("Please select a start and endpoint for the new connection");
        }

        private void AddConnection(NetworkPointUI point) {
            if(newConnection.StartPoint == default) {
                newConnection.StartPoint = selectedPoint;
            } else if(newConnection.EndPoint == default) { 
                newConnection.EndPoint = selectedPoint;
            }

            if(newConnection.StartPoint != default && newConnection.EndPoint != default) {
                addConnectionClicked = false;
                RemovePreviousHighlight();
                int id = nm.AddConnection(SegmentMapper.MapToDomain(newConnection));
                newConnection.Id = id;
                segments.Add(newConnection);
                newConnection = default;
            }
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