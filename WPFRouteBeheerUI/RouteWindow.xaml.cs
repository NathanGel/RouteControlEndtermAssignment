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
        private ObservableCollection<(NetworkPoint, bool)> _stopsCollection;

        public RouteWindow(RouteManager rm, RouteUI route) {
            _routeManager = rm;
            InitializeComponent();
            LoadRoute(route);
        }

        private void LoadRoute(RouteUI route) {
            _currentRoute = route;
            TxtId.Text = route.Id.ToString();
            TxtName.Text = route.Name;

            _stopsCollection = route.Stops;
            DataGridStops.ItemsSource = NetworkPointStopsMapper.MapToUIModel(route.Stops.ToList());

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

        private void DataGridRow_Selected(object sender, RoutedEventArgs e) {
            // This is handled by DataGridStops_SelectionChanged
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e) {
            try {
                _currentRoute.Name = TxtName.Text;
                _currentRoute.Stops = _stopsCollection;

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

        }

        private void RemovePoint_Click(object sender, RoutedEventArgs e) {
            if (DataGridStops.SelectedItem is NetworkPointStopsUI selectedStop) {
                // Implement removal logic
            }
        }

        private void AddToFront_Click(object sender, RoutedEventArgs e) {
            if (DataGridStops.SelectedItem is NetworkPointStopsUI selectedStop) {
                // Implement logic to add to front
            }
        }

        private void AddToEnd_Click(object sender, RoutedEventArgs e) {
            if (DataGridStops.SelectedItem is NetworkPointStopsUI selectedStop) {
                // Implement logic to add to end
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