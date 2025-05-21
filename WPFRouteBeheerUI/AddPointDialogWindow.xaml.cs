using RouteBeheerBL.Model;
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
using WPFRouteBeheerUI.Model;

namespace WPFRouteBeheerUI {
    /// <summary>
    /// Interaction logic for AddPointDialogWindow.xaml
    /// </summary>
    public partial class AddPointDialogWindow : Window {
        public Segment segmentToAdd;
        public NetworkPoint selectedPoint;
        private NetworkPointStopsUI pointFromWhereToCheck;
        private List<Segment> _segments;
        private bool frontOrEnd; //front is true, end is false
        public bool orientation; //true is reversed, false is not reversed
        public AddPointDialogWindow( List<Segment> segments, NetworkPointStopsUI pointFromWhereToCheck, RouteUI route, bool frontOrEnd) {
            InitializeComponent();
            _segments = segments;
            this.pointFromWhereToCheck = pointFromWhereToCheck;
            this.frontOrEnd = frontOrEnd;
            var existingRouteSegments = route.Segments.Select(t => t.Item1).ToList();

            // Filter segments zodat het enkel de segmenten zijn die nog niet in de route zitten en verbonden zijn met het punt van waaruit we de connectie maken
            var filteredSegments = _segments
                .Where(s => !existingRouteSegments.Any(rs => rs.Id == s.Id))  // excludeer degene die al in de route zitten
                .Where(s => s.StartPoint.Id == pointFromWhereToCheck.Id || s.EndPoint.Id == pointFromWhereToCheck.Id) // filter op segmenten die verbonden zijn met het punt van waaruit we de connectie maken
                .ToList();

            // Selecteer het punt dat niet gelijk is aan het punt van waaruit we de connectie maken
            var pointsToDisplay = filteredSegments
                .Select(s => s.StartPoint.Id == pointFromWhereToCheck.Id ? s.EndPoint : s.StartPoint)
                .Distinct()
                .ToList();

            ListBoxPoints.ItemsSource = pointsToDisplay;
        }
        private void SelectButton_Click(object sender, RoutedEventArgs e) {
            if (ListBoxPoints.SelectedItem is NetworkPoint selectedPoint) {
                this.selectedPoint = selectedPoint;
                // zoek het segment dat overeenkomt met het geselecteerde punt
                var segment = _segments.FirstOrDefault(s =>
                    (s.StartPoint.Id == pointFromWhereToCheck.Id && s.EndPoint.Id == selectedPoint.Id) ||
                    (s.EndPoint.Id == pointFromWhereToCheck.Id && s.StartPoint.Id == selectedPoint.Id)
                );

                if (segment != null) {
                    segmentToAdd = segment;
                    DialogResult = true;
                }

                if (frontOrEnd) { // deze check dient om te kijken wanneer we een punt vooraan toevoegen dat de orientatie normaal is of nier
                                  // vb. A-b b-c c-d en ik voeg een punt toe voor A. Dan zou A het eindpunt moeten zijn van dat segment
                                  // dus de orientatie is normaal. Indien dit niet is dan is de orientatie reversed (true)
                    if (segment.EndPoint.Id == pointFromWhereToCheck.Id) {
                        orientation = false;
                    } else {
                        orientation = true;
                    }
                } else { // deze check dient om te kijken wanneer we een punt vooraan toevoegen dat de orientatie normaal is of nier
                         // vb. A-b b-c c-d en ik voeg een punt toe na d. Dan zou d het startpunt moeten zijn van dat segment
                         // dus de orientatie is normaal. Indien dit niet is dan is de orientatie reversed (true)
                    if (segment.StartPoint.Id == pointFromWhereToCheck.Id) {
                        orientation = false;
                    } else {
                        orientation = true;
                    }
                }
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
