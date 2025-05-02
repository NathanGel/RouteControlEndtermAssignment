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
using RouteBeheerBL.Exceptions;

namespace WPFNetwerkBeheerUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private ObservableCollection<NetworkPointUI> points = new();
        // de lijst van punten die ik gebruik om op het canvas te tekenen.
        // Observable collection omdat ik de UI wil updaten wanneer er iets
        // veranderd in de lijst

        private Dictionary<Ellipse, NetworkPointUI> pointElements = new Dictionary<Ellipse, NetworkPointUI>();
        // dictionary om de punten te koppelen aan hun UI elementen
        // om ze makkelijk te kunnen verwijderen zonder dat ik telkens
        // na elke verandering het hele canvas opnieuw moet tekenen

        private ObservableCollection<SegmentUI> segments = new();
        // de lijst van segmenten die ik gebruik om op het canvas te tekenen
        // Observable collection idem met points

        private Dictionary<SegmentUI, Line> segmentElements = new Dictionary<SegmentUI, Line>();

        private NetworkPointUI selectedPoint; 
        //ik sla dit punt op om in de RemoveLocation/UpdateLocation en RemoveConnection/AddConnection
        // dit punt te gebruiken en vast te stellen of er wel een punt geselecteerd is na de click events op de knoppen

        private NetworkPointUI clickedLocation; 
        // dit punt sla ik op om te gebruiken in de AddLocation  en om te
        // kijken of er wel een locatie geselecteerd is na het click event op de knop

        private bool addConnectionClicked = false;

        private bool removeConnectionClicked = false;

        private SegmentUI connection;

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
            try {
                List<SegmentUI> segmentsUI = new(nm.GetSegments().Select(sm => SegmentMapper.MapFromDomain(sm)));
                foreach (var segment in segmentsUI) {
                    segments.Add(segment);
                }
                List<NetworkPointUI> pointsUI = new(nm.GetNetworkPoints().Select(np => NetworkPointMapper.MapFromDomain(np)));
                foreach (var point in pointsUI) {
                    points.Add(new NetworkPointUI(point.Id, point.X, point.Y, point.Facilities));
                }
            } catch (ApplicationException ex) { // dit catch-blok vangt elke mogelijke sqlexception die gegooid werden in de repo. De SqlException werd vertaald naar een ApplicationException
                MessageBox.Show("An error occured while retrieving the network for initialization", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            } catch (Exception ex) { // dit catch-blok dient als fallback indien er ergens een exception gegooid word die onverwacht is  
                MessageBox.Show("An unexpected error occured:  " + ex.Message, "System Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Points_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.Action == NotifyCollectionChangedAction.Add) {
                foreach (NetworkPointUI point in e.NewItems) {
                    DrawPoint(point);
                }
            } else if (e.Action == NotifyCollectionChangedAction.Remove) {
                foreach (NetworkPointUI point in e.OldItems) {
                    // Zoeken in de Dictionary naar de overeenstemmende ellipse
                    Ellipse targetEllipse = null;
                    foreach (var kvp in pointElements) {
                        if (kvp.Value == point) {
                            targetEllipse = kvp.Key;
                            break;
                        }
                    }

                    // indien er effectief een overeenstemmende ellipse gevonden
                    // is, verwijder ik de ellipse van het canvas en van de dictionary 
                    if (targetEllipse != null) {
                        canvas.Children.Remove(targetEllipse);
                        pointElements.Remove(targetEllipse);
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
                StrokeThickness = 2, 
            };
            
            Canvas.SetLeft(ellipse, point.X - (ellipse.Width / 2)); // de berekening die hier in plaats vind zorgt ervoor dat
            Canvas.SetTop(ellipse, point.Y - (ellipse.Width / 2));   // het midden van de ellipse overeenstemt met de exacte coordinaten
                                                                     // van het punt eerder zorgde dit voor problemen met de lijnen 
            ellipse.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;
            ellipse.MouseRightButtonDown += Ellipse_MouseRightButtonDown;
            //ellipse.MouseRightButtonDown += Ellipse_Clicked();
            canvas.Children.Add(ellipse);
            pointElements[ellipse] = point;
        }

        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (sender is Ellipse clickedEllipse && pointElements.TryGetValue(clickedEllipse, out NetworkPointUI point))
                selectedPoint = point;
        }

        private void Ellipse_MouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            if(sender is Ellipse clickedEllipse && pointElements.TryGetValue(clickedEllipse, out NetworkPointUI point))
                selectedPoint = point;
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
            Point mousePosPoint = e.GetPosition(canvas);

            // deze statement maakt gebruik van de hittest om te bepalen op wat voor UIElement 
            // er geklikt is. Indien dit geen ellipse is of mijn dictionary dit niet bevat
            // worden de voorafgaande highlights verwijdert. Dit om telkens te vermijden dat
            // er coordinaten of een highlight zichtbaar is wanneer dit niet langer nodig is
            HitTestResult result = VisualTreeHelper.HitTest(canvas, mousePosPoint);
            //if (result == null || result.VisualHit is not Ellipse || !pointElements.ContainsKey((Ellipse)result.VisualHit))
            if (result == null || result.VisualHit is not Ellipse) {
                selectedPoint = default;
                RemovePreviousHighlight();
                RemovePreviousCoordinates();

                addConnectionClicked = false;
                removeConnectionClicked = false;

                TextBlockConnection.Text = null;
                RemoveDisplayedConnections();
            }

            if (selectedPoint != default && !addConnectionClicked && !removeConnectionClicked) {
                RemovePreviousHighlight();
                DisplayCoordinates(selectedPoint);
            } else if(selectedPoint != default) {
                HighlightPoint(selectedPoint);
                if (addConnectionClicked)
                    AddConnection(selectedPoint);
                else if (removeConnectionClicked) {
                    DisplayExistingConnectionsFromPoint(selectedPoint);
                    RemoveConnection(selectedPoint);
                }
            }
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            TextBlockConnection.Text = null;
            RemoveDisplayedConnections();
            addConnectionClicked = false;   // wanneer er een right click event plaatsvindt op het canvas
            removeConnectionClicked = false;// impliceert dit dat de add of remove connection functionaliteit
                                            // word afgebroken

            Point mousePosPoint = e.GetPosition(canvas);

            // deze statement maakt gebruik van de hittest om te bepalen op wat voor UIElement 
            // er geklikt is. Indien dit geen ellipse is of mijn dictionary dit niet bevat
            // worden de voorafgaande highlights verwijdert. Dit om telkens te vermijden dat
            // er coordinaten of een highlight zichtbaar is wanneer dit niet langer nodig is
            HitTestResult result = VisualTreeHelper.HitTest(canvas, mousePosPoint);
            if (result == null || result.VisualHit is not Ellipse || !pointElements.ContainsKey((Ellipse)result.VisualHit)) {
                selectedPoint = default;
                RemovePreviousHighlight();
                RemovePreviousCoordinates();
            }

            // deze checks bepalen of er een bestaand punt geselescteerd is.
            // Dit bepaald dan weer welke opties er zichtbaar zijn het contextmenu
            if (selectedPoint != default) {
                RemovePreviousCoordinates();
                HighlightPoint(selectedPoint);
                MenuItemAddNetworkPoint.Visibility = Visibility.Collapsed;
                MenuItemRemoveNetworkPoint.Visibility = Visibility.Visible;
                MenuItemUpdateNetworkPoint.Visibility = Visibility.Visible;
            } else {
                TextBlockConnection.Text = null;
                NetworkPointUI mousePos = new(mousePosPoint.X, mousePosPoint.Y);
                clickedLocation = mousePos;
                HighlightPoint(clickedLocation);
                MenuItemRemoveNetworkPoint.Visibility = Visibility.Collapsed;
                MenuItemUpdateNetworkPoint.Visibility = Visibility.Collapsed;
                MenuItemAddNetworkPoint.Visibility = Visibility.Visible;
            }
        }

        // dit textblock element gebruik ik om de coordinaten op het canvas te tonen 
        // dit is achteraf ook makkelijk van het canvas te verwijderen
        private TextBlock coordinatesTextBlock;

        private void DisplayCoordinates(NetworkPointUI point) {
            RemovePreviousCoordinates();

            coordinatesTextBlock = new TextBlock {
                Text = $"X:{point.X}    Y:{point.Y}",
                Background = new SolidColorBrush(Color.FromRgb(37, 40, 54)), 
                Foreground = new SolidColorBrush(Color.FromRgb(245, 247, 251)), 
                Padding = new Thickness(4),
                FontSize = 12
            };

            
            double maxX = canvas.ActualWidth - 250; // het aantal px dat word opgeschoven naar rechts om te vermijden dat het coordinaten blok buiten het canvas valt
            double maxY = canvas.ActualHeight - 20; //het aantal px dat word opgeschoven naar boven om te vermijden dat het coordinaten blok buiten het canvas valt

            double x = Math.Min(point.X, maxX);
            double y = Math.Min(point.Y, maxY);

            Canvas.SetLeft(coordinatesTextBlock, x);
            Canvas.SetTop(coordinatesTextBlock, y);

            canvas.Children.Add(coordinatesTextBlock);
        }

        private void RemovePreviousCoordinates() {
            if(coordinatesTextBlock != null && canvas.Children.Contains(coordinatesTextBlock))
                canvas.Children.Remove(coordinatesTextBlock);
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
            highlightEllipse = highlightCircle;
        }

        // dit Ellipse element gebruik ik om te tekenen waar/op welk punt er geklikt is op 
        // het canvas dit is achteraf ook makkelijk van het canvas te verwijderen
        private Ellipse highlightEllipse;

        private void RemovePreviousHighlight() {
            if (highlightEllipse != null && canvas.Children.Contains(highlightEllipse))
                canvas.Children.Remove(highlightEllipse);
        }

        private void AddNetworkPoint_Click(object sender, RoutedEventArgs e) {
            try {
                if (clickedLocation != default) {
                    NetworkPointUI newPoint = new(clickedLocation.X, clickedLocation.Y);
                    newPoint.Id = nm.AddNetworkPoint(NetworkPointMapper.MapToDomain(newPoint));
                    points.Add(newPoint);
                    RemovePreviousHighlight();
                    clickedLocation = default;
                } else {
                    MessageBox.Show("No location selected for new network point");
                }
            } catch (NetworkException ex) { // Dit catch-blok vangt de exceptions op die gegooid worden in de manager AddNetworkPoint
                MessageBox.Show("An error occured because the networkpoint does not meet the specified requirements", ex.Message, MessageBoxButton.OK, MessageBoxImage.Warning);
            } catch (ApplicationException ex) { // Dit catch-blok vangt de sql exceptions op die gegooid werden in de repo. De SqlException werd vertaald naar een ApplicationException
                MessageBox.Show("An error occured while adding a networkpoint", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            } catch (Exception ex) { // dit catch-blok dient als fallback indien er ergens een exception gegooid word die onverwacht is
                MessageBox.Show("An unexpected error occured: " + ex.Message, "System Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemoveNetworkPoint_Click(object sender, RoutedEventArgs e) {
            if (selectedPoint != default) {
                try {
                    nm.RemoveNetworkPoint(NetworkPointMapper.MapToDomain(new NetworkPointUI(selectedPoint.Id, selectedPoint.X, selectedPoint.Y)));
                    points.Remove(selectedPoint);
                    RemovePreviousHighlight();
                    selectedPoint = default;
                    throw new Exception();
                }catch (NetworkException ex) { // Dit catch-blok vangt de exceptions op die gegooid worden in de manager RemoveNetworkPoint
                    MessageBox.Show("An error occured because the networkpoint did not meet the specified requirements", ex.Message, MessageBoxButton.OK, MessageBoxImage.Warning);
                } catch (InvalidOperationException) { // Dit catch-blok vangt specifiek de sqlexceptions op die gegooid werden wanneer er een conflict is met de foreign key constraints in de databank. Deze exception werd vertaald naar een InvalidOperationException
                    MessageBox.Show("An error occured because there are existing connections between points and segments", "Deletion Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                } catch (ApplicationException ex) { // Dit catch-blok vangt al de rest van de sql exceptions op die gegooid werden in de repo. De SqlException werd vertaald naar een ApplicationException
                    MessageBox.Show("An error occured while deleting the networkpoint", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                } catch (Exception ex) { // dit catch-blok dient als fallback indien er ergens een exception gegooid word die onverwacht is
                    MessageBox.Show("An Unexpected error occured: " + ex.Message, "System exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            } else {
                MessageBox.Show("No network point selected");
            }
        }

        private void UpdateNetworkPoint_Click(object sender, RoutedEventArgs e) {
            if (selectedPoint != default) {
                try {
                    double originalX = selectedPoint.X; //ik sla hier de originele x en y coordinaat op om fouten te vermijden
                    double originalY = selectedPoint.Y;

                    NetworkPointWindow npWindow = new(selectedPoint);
                    bool? result = npWindow.ShowDialog();
                    if (result == true) {
                        nm.UpdateNetworkPoint(NetworkPointMapper.MapToDomain(npWindow.point));

                        // dit codeblok zoekt in de points collectie naar het geselecteerde punt om het daar aan te passen
                        int index = points.IndexOf(selectedPoint);
                        if (index != -1) {
                            points[index] = npWindow.point;
                        }

                        // dit codeblok past het networkpoint aan in de Ellipse/networkpoint dictionary
                        var kvp = pointElements.FirstOrDefault(p => p.Value == selectedPoint);
                        if (kvp.Key != null) {
                            pointElements[kvp.Key] = npWindow.point; // vervang het punt in de dictionary

                            // Het punt opnieuw tekenen indien de coordinaten gewijzigd zijn
                            if (npWindow.point.X != originalX || npWindow.point.Y != originalY) {
                                canvas.Children.Remove(kvp.Key); // verwijder de oude ellipse van het canvas
                                pointElements.Remove(kvp.Key);   // verwijder de oude ellipse/point uit de dictionary

                                DrawPoint(npWindow.point);       // het punt opnieuw tekenen
                            }
                        }

                        RemovePreviousHighlight(); //zorgen dat de vorige highlight zeker weg is na het verloop van de dialogen
                        selectedPoint = default; //op default zetten zodat dit niet voor problemen kan zorgen bij de volgende method
                    }
                } catch (NetworkException ex) { // Dit catch-blok vangt de exceptions op die gegooid worden in de manager UpdateNetworkPoint
                    MessageBox.Show("An error occured because the networkpoint does not meet the specified requirements", ex.Message, MessageBoxButton.OK, MessageBoxImage.Warning);
                } catch (ApplicationException ex) { // Dit catch-blok vangt de sql exceptions op die gegooid werden in de repo. De SqlException werd vertaald naar een ApplicationException
                    MessageBox.Show("An error occured while updating a networkpoint", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                } catch (Exception ex) { // dit catch-blok dient als fallback indien er ergens een exception gegooid word die onverwacht is
                    MessageBox.Show("An unexpected error occured: " + ex.Message, "System Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            } else {
                MessageBox.Show("No network point selected");
            }
        }

        private void AddConnection_Click(object sender, RoutedEventArgs e) {
            RemovePreviousHighlight();
            RemovePreviousCoordinates();
            RemoveDisplayedConnections();
            removeConnectionClicked = false;
            connection = new();
            addConnectionClicked = true;
            TextBlockConnection.Text = "Please select a start and endpoint";
        }

        private void RemoveConnection_Click(object sender, RoutedEventArgs e) {
            RemovePreviousHighlight();
            RemovePreviousCoordinates();
            addConnectionClicked = false;
            connection = new();
            removeConnectionClicked = true;
            TextBlockConnection.Text = "Please select the connection you want to remove";
        }

        private void AddConnection(NetworkPointUI point) {
            if(connection.StartPoint == default)
                connection.StartPoint = selectedPoint;
            else if(connection.EndPoint == default)
                connection.EndPoint = selectedPoint;

            try {
                if (connection.StartPoint != default && connection.EndPoint != default) {
                    addConnectionClicked = false;
                    RemovePreviousHighlight();
                    int id = nm.AddConnection(SegmentMapper.MapToDomain(connection));
                    connection.Id = id;
                    segments.Add(connection);
                    connection = default;
                    TextBlockConnection.Text = null;
                    //zou nog graag beide punten achteraf hertekenen om te vermijden dat de lijn overlapt TODO
                }
            } catch (NetworkException ex) { // Dit catch-blok vangt de exceptions op die gegooid worden in de manager AddConnection
                MessageBox.Show("An error occured because the connection did not meet the specified requirements", ex.Message, MessageBoxButton.OK, MessageBoxImage.Warning);
            } catch (ApplicationException ex) { // Dit catch-blok vangt de sql exceptions op die gegooid werden in de repo. De SqlException werd vertaald naar een ApplicationException
                MessageBox.Show("An error occured while adding a connection", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            } catch (Exception ex) { // dit catch-blok dient als fallback indien er ergens een exception gegooid word die onverwacht is
                MessageBox.Show("An unexpected error occured: " + ex.Message, "System Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemoveConnection(NetworkPointUI point) {
            if (connection.StartPoint == default)
                connection.StartPoint = selectedPoint;
            else if (connection.EndPoint == default)
                connection.EndPoint = selectedPoint;

            //logica toevoegen die een segment zoekt op basis van start en eindpunt
            //logica ivm manager
            // wss nog een pop-up of je zeker bent??
            if (connection.StartPoint != default && connection.EndPoint != default) {
                TextBlockConnection.Text = null;

                RemovePreviousHighlight();
                RemoveDisplayedConnections();

                var segmentToRemove = segments.FirstOrDefault(s =>
                    (s.StartPoint.Id == connection.StartPoint.Id && s.EndPoint.Id == connection.EndPoint.Id) ||
                    (s.StartPoint.Id == connection.EndPoint.Id && s.EndPoint.Id == connection.StartPoint.Id));
                if (segmentToRemove != null) {
                    try {
                        nm.RemoveConnection(SegmentMapper.MapToDomain(segmentToRemove));
                        segments.Remove(segmentToRemove);
                    } catch (NetworkException ex) { // Dit catch-blok vangt de exceptions op die gegooid worden in de manager RemoveConnection
                        MessageBox.Show("An error occured because the connection does not meet the specified requirements", ex.Message, MessageBoxButton.OK, MessageBoxImage.Warning);
                    } catch (InvalidOperationException) { // Dit catch-blok vangt specifiek de sqlexceptions op die gegooid werden wanneer er een conflict is met de foreign key constraints in de databank. Deze exception werd vertaald naar een InvalidOperationException
                        MessageBox.Show("Än error occured because there are existing connections within routes", "Deletion Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    } catch (ApplicationException ex) { // Dit catch-blok vangt de sql exceptions op die gegooid werden in de repo. De SqlException werd vertaald naar een ApplicationException
                        MessageBox.Show("An error occured while deleting the networkpoint", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                    } catch (Exception ex) { // dit catch-blok dient als fallback indien er ergens een exception gegooid word die onverwacht is
                        MessageBox.Show("An unexpected error occured: " + ex.Message, "System Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

        }

        private void RemoveDisplayedConnections() {
            foreach (Ellipse ellipse in existingConnections.Keys)
                canvas.Children.Remove(ellipse);
        }

        // de originele ellipse opslaan in een dictionary samen
        // met het networkpoint om zo makkelijk op te zoeken
        Dictionary<Ellipse, NetworkPointUI> existingConnections = new Dictionary<Ellipse, NetworkPointUI>();

        private void DisplayExistingConnectionsFromPoint(NetworkPointUI point) {
            List<NetworkPointUI> connections = new();
            foreach (SegmentUI segment in segments) {
                if (segment.StartPoint.Id == point.Id)
                    connections.Add(segment.EndPoint);
                else if (segment.EndPoint.Id == point.Id)
                    connections.Add(segment.StartPoint);
            }

            foreach (NetworkPointUI np in connections) {
                Ellipse ellipse = new Ellipse() {
                    Width = 10,
                    Height = 10,
                    Fill = Brushes.Red,
                    Stroke = Brushes.Red,
                    StrokeThickness = 2
                };

                ellipse.MouseLeftButtonDown += Ellipse_MouseLeftButtonDownConnection;
                Canvas.SetLeft(ellipse, np.X - ellipse.Width / 2);
                Canvas.SetTop(ellipse, np.Y - ellipse.Height / 2);

                canvas.Children.Add(ellipse);
                existingConnections.Add(ellipse,np);
            }
        }

        private void Ellipse_MouseLeftButtonDownConnection(object sender, MouseButtonEventArgs e) {// een nieuwe ellipse left klik event om specifiek af te handelen
                                                                                                   // dat wanneer er op de opgelichte ellipses geklikt wordt het correcte
                                                                                                   // punt gekozen word
            if (sender is Ellipse clickedEllipse && existingConnections.TryGetValue(clickedEllipse, out NetworkPointUI point))
                selectedPoint = point;
        }

        // Onderstaande methods dienen enkel voor de window actions ivm
        // slepen, maximize, minimize, close en dubbel klik op te topbar om te maximaliseren
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