using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using RouteBeheerBL.Model;
using RouteBeheerBL.Exceptions;
using WPFRouteBeheerUI.Model;
using RouteBeheerBL.Managers;
using WPFRouteBeheerUI.Mappers;
using System.Text.Json;

namespace WPFRouteBeheerUI {
    public partial class RouteWindow : Window {
        private RouteUI _currentRoute;
        private RouteManager _routeManager;
        private ObservableCollection<NetworkPointStopsUI> _stopsCollection;
        private RouteUI routeReference;
        private List<Segment> segments;

        public RouteWindow(RouteManager rm, RouteUI route, List<Segment> segments) {
            _routeManager = rm;
            this.segments = segments;
            InitializeComponent();
            LoadRoute(route);
        }

        private void LoadRoute(RouteUI route) {
            _currentRoute = new(route.Id, route.Name, route.Segments, route.Stops); // ik maak hier een nieuw aan ipv op reference te werken zodat ik wanneer
                                                                                    // de route aanpast maar op een foutieve manier controle heb over wat er
                                                                                    // in de applicatie getoont wordt
            routeReference = route;
            TxtId.Text = _currentRoute.Id.ToString();
            TxtName.Text = _currentRoute.Name;

            _stopsCollection = new (NetworkPointStopsMapper.MapToUIModel(route.Stops.ToList()));
            DataGridStops.ItemsSource = _stopsCollection;

            CalculateAndDisplayTotalDistance();
        }

        private void CalculateAndDisplayTotalDistance() {
            double totalDistance = 0;

            foreach(var segment in _currentRoute.Segments) {
                totalDistance += Route.GetDistance(segment.Item1.StartPoint, segment.Item1.EndPoint);
            }

            TxtDistance.Text = $"{totalDistance.ToString("F2")} km";
        }

        private void DataGridStops_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (DataGridStops.SelectedItem is NetworkPointStopsUI) {
                NetworkPointStopsUI uiStops = (NetworkPointStopsUI)DataGridStops.SelectedItem;
                ListBoxFacilities.ItemsSource = _currentRoute.Stops.FirstOrDefault(s => s.Item1.Equals(uiStops.point)).Item1.Facilities;
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e) {
            try {
                _currentRoute.Name = TxtName.Text; // de naam van de route aanpassen
                _currentRoute.Stops = new (NetworkPointStopsMapper.MapFromUIModel(_stopsCollection)); // de stops van de route aanpassen

                _routeManager.UpdateRoute(RouteMapper.MapToDomain(_currentRoute)); // de route in de database aanpassen


                // hireonder volgt de code waarbij het UImodel overal in memery aangepast is na validatie van de waarden
                routeReference.Name = TxtName.Text; // de naam van de route aanpassen
                routeReference.Segments = _currentRoute.Segments; // de segmenten van de route aanpassen
                routeReference.Stops = _currentRoute.Stops; // de stops van de route aanpassen
                MessageBox.Show("Changes saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            } catch (RouteException ex) {
                MessageBox.Show(ex.Message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            } catch (Exception ex) {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveToFile_Click(object sender, RoutedEventArgs e) {
            var dialog = new SaveFileDialog { // de savefiledialog aanmaken met een filter op json files en een vooraf vastgelegde naam van het bestand
                Filter = "JSON files (*.json)|*.json",
                FileName = $"{_currentRoute.Name}_Route.json"
            };

            if (dialog.ShowDialog() == true) {
                var stopsSet = new HashSet<int>( //ik kijk hier eerst of het punt een stop is in de route
                    _currentRoute.Stops
                        .Where(s => s.Item2 == true)
                        .Select(s => s.Item1.Id)
                );

                var mergedJson = new { // hier leg ik de vorm van de json vast 
                    _currentRoute.Id,
                    _currentRoute.Name,
                    FullDistance = TxtDistance.Text, //de full distance uit het textblock in de ui halen
                    Segments = _currentRoute.Segments.Select(s => new {
                        s.Item1.Id,
                        Distance = $"{Route.GetDistance(s.Item1.StartPoint, s.Item1.EndPoint)} km", // afstand per segment berekenen
                        StartPoint = new {
                            s.Item1.StartPoint.Id,
                            s.Item1.StartPoint.X,
                            s.Item1.StartPoint.Y,
                            Facilities = s.Item1.StartPoint.Facilities.Select(f => new { f.Id, f.Name }), // alle facilities van het punt
                            IsStop = stopsSet.Contains(s.Item1.StartPoint.Id) // indien de hashset het punt bevat is het een stop
                        },
                        EndPoint = new {
                            s.Item1.EndPoint.Id,
                            s.Item1.EndPoint.X,
                            s.Item1.EndPoint.Y,
                            Facilities = s.Item1.EndPoint.Facilities.Select(f => new { f.Id, f.Name }), // alle facilities van het punt
                            IsStop = stopsSet.Contains(s.Item1.EndPoint.Id) // indien de hashset het punt bevat is het een stop
                        }
                    })
                };

                var options = new JsonSerializerOptions {
                    WriteIndented = true //de property true toevoegen als optie ivm indentation. Anders is de output in de file 1 lange lijn zonder structuur
                };

                string json = JsonSerializer.Serialize(mergedJson, options); //de jsonstring aanmaken met de vooraf vastgelegde serialization options
                File.WriteAllText(dialog.FileName, json); // wegschrijven naar de file
            }
        }

        private void StopIndicator_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (sender is Border border && border.DataContext is NetworkPointStopsUI stop) {
                // Toggle the IsStop property
                stop.IsStop = !stop.IsStop;
            }
        }

        private void RemovePoint_Click(object sender, RoutedEventArgs e) {
            if (DataGridStops.SelectedItem is NetworkPointStopsUI selectedStop) {

                if (_stopsCollection.Count > 0) {
                    int index = _stopsCollection.IndexOf(selectedStop);
                    if (index == 0 || index == _stopsCollection.Count - 1) {
                        _stopsCollection.Remove(selectedStop);

                        var segmentsList = _currentRoute.Segments.ToList(); // or access as List

                        // Find segments connected to the selected stop
                        var connectedSegments = segmentsList
                            .Where(s => s.Item1.StartPoint.Id == selectedStop.Id || s.Item1.EndPoint.Id == selectedStop.Id)
                            .ToList();

                        if (connectedSegments.Count == 0) {
                            // No connected segments found, nothing to remove
                            return;
                        }

                        // Now check if the segment is the first or last segment in the route's segments list
                        var firstSegment = segmentsList.First();
                        var lastSegment = segmentsList.Last();

                        // Remove segment only if it is the first or last
                        foreach (var seg in connectedSegments) {
                            if (seg.Item1.Id == firstSegment.Item1.Id || seg.Item1.Id == lastSegment.Item1.Id) {
                                _currentRoute.Segments.Remove(seg);
                                break;
                            }
                        }
                    } else {
                        MessageBox.Show("Only the first or last point can be removed.", "Remove Stop", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        private void AddToFront_Click(object sender, RoutedEventArgs e) {
            AddPointDialogWindow window = new AddPointDialogWindow(segments, _stopsCollection.First(), _currentRoute);
            bool? result = window.ShowDialog();
            if (result == true) {
                var newStop = (window.selectedPoint, true);
                _stopsCollection.Insert(0, NetworkPointStopsMapper.MapToUIModel(newStop));
                _currentRoute.Segments.Insert(0, (window.segmentToAdd, false));
            }
        }

        private void AddToEnd_Click(object sender, RoutedEventArgs e) {
            AddPointDialogWindow window = new AddPointDialogWindow(segments, _stopsCollection.Last(), _currentRoute);
            bool? result = window.ShowDialog();
            if (result == true) {
                var newStop = (window.selectedPoint, true);
                _stopsCollection.Add(NetworkPointStopsMapper.MapToUIModel(newStop));
                _currentRoute.Segments.Add((window.segmentToAdd, false));
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}