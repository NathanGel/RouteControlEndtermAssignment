using RouteBeheerBL.Exceptions;
using RouteBeheerBL.Managers;
using RouteBeheerDL;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WPFFaciliteitBeheerUI.Mappers;
using WPFFaciliteitBeheerUI.Model;

namespace WPFFaciliteitBeheerUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private NetworkManager networkManager;
        private readonly string connectionString = @"Data Source=nathan\SQLExpress;Initial Catalog=NetworkControlTesting;Integrated Security=True;Trust Server Certificate=True";
        public ObservableCollection<FacilityUI> Facilities;
        public MainWindow() {
            InitializeComponent();
            networkManager = new NetworkManager(new NetworkRepository(connectionString));
            Facilities = new(networkManager.GetAllFacilities().Select( s => FacilityMapper.MapFromDomain(s)));
            DataGridFacilities.ItemsSource = Facilities;
        }

        private void UpdateFacility_Click(object sender, RoutedEventArgs e) {
            FacilityWindow window = new((FacilityUI)DataGridFacilities.SelectedItem, true);
            bool? result = window.ShowDialog();
            try {
                if (result == true) {
                    networkManager.UpdateFacility(FacilityMapper.MapToDomain(window.Facility));
                }
            } catch (NetworkException ex) { // Dit catch-blok vangt de exceptions op die gegooid worden in de manager UpdateFacility
                MessageBox.Show("An error occured because the facility does not meet the specified requirements", ex.Message, MessageBoxButton.OK, MessageBoxImage.Warning);
            } catch (ApplicationException) { // Dit catch-blok vangt de sql exceptions op die gegooid werden in de repo. De SqlException werd vertaald naar een ApplicationException
                MessageBox.Show("An error occured while updating the facility", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
            } catch (Exception ex) { // dit catch-blok dient als fallback indien er ergens een exception gegooid word die onverwacht is
                MessageBox.Show("An unexpected error occured: " + ex.Message, "System Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddFacility_Click(object sender, RoutedEventArgs e) {
            FacilityWindow window = new(null, false);
            bool? result = window.ShowDialog();
            try {
                if (result == true) {
                    FacilityUI newFacility = window.Facility;
                    int id = networkManager.AddFacility(FacilityMapper.MapToDomain(newFacility));
                    newFacility.Id = id;
                    Facilities.Add(newFacility);
                }
            } catch (NetworkException ex) {// Dit catch-blok vangt de exceptions op die gegooid worden in de manager AddFacility
                MessageBox.Show("An error occured because the facility does not meet the specified requirements", ex.Message, MessageBoxButton.OK, MessageBoxImage.Warning);
            } catch (ApplicationException) {// Dit catch-blok vangt de sql exceptions op die gegooid werden in de repo. De SqlException werd vertaald naar een ApplicationException
                MessageBox.Show("An error occured while adding the networkpoint", "Adding Error", MessageBoxButton.OK, MessageBoxImage.Error);
            } catch (Exception ex) { // dit catch-blok dient als fallback indien er ergens een exception gegooid word die onverwacht is
                MessageBox.Show("An unexpected error occured: " + ex.Message, "System Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemoveFacility_Click(object sender, RoutedEventArgs e) {
            try {
                networkManager.RemoveFacility(FacilityMapper.MapToDomain((FacilityUI)DataGridFacilities.SelectedItem));
                Facilities.Remove((FacilityUI)DataGridFacilities.SelectedItem);
            } catch (NetworkException ex) {// Dit catch-blok vangt de exceptions op die gegooid worden in de manager RemoveFacility
                MessageBox.Show("An error occured because the facility does not meet the specified requirements", ex.Message, MessageBoxButton.OK, MessageBoxImage.Warning);
            } catch (InvalidOperationException ex) { //deze exception is gelinkt aan het feit dat er connections zijn
                MessageBox.Show(ex.Message, "Deletion Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            } catch (ApplicationException) { // Dit catch-blok vangt de sql exceptions op die gegooid werden in de repo. De SqlException werd vertaald naar een ApplicationException
                MessageBox.Show("An error occured while deleting the Facility");
            } catch (Exception ex) { // dit catch-blok dient als fallback indien er ergens een exception gegooid word die onverwacht is
                MessageBox.Show("Unexpected error: " + ex.Message);
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (e.ButtonState == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e) {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e) {
            if (this.WindowState == WindowState.Normal) {
                this.WindowState = WindowState.Maximized;
            } else {
                this.WindowState = WindowState.Normal;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}