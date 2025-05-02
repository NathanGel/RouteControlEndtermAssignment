using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using RouteBeheerBL.Managers;
using RouteBeheerBL.Model;
using RouteBeheerDL;

namespace WPFNetwerkBeheerUI {
    /// <summary>
    /// Interaction logic for FacilitiesWindow.xaml
    /// </summary>
    public partial class FacilitiesWindow : Window {
        public ObservableCollection<Facility> selectedFacilities;
        private ObservableCollection<Facility> allFacilities;
        private NetworkManager nm;
        private readonly string connectionString = @"Data Source=NATHAN\SQLExpress;Initial Catalog=NetworkControlTesting;Integrated Security=True;Trust Server Certificate=True";
        public FacilitiesWindow(ObservableCollection<Facility> facilities) {
            InitializeComponent();
            nm = new(new NetworkRepository(connectionString));
            allFacilities = new ObservableCollection<Facility>(nm.GetAllFacilities());
            selectedFacilities = facilities;
            ListBoxSelectedFacilities.ItemsSource = selectedFacilities;
            ListBoxAllFacilities.ItemsSource = allFacilities;
        }

        private void AddFacility_Click(object sender, RoutedEventArgs e) {
            List<Facility> facilities = new List<Facility>();
            foreach (var item in ListBoxAllFacilities.SelectedItems) facilities.Add((Facility)item);
            foreach (var item in facilities) {
                selectedFacilities.Add(item);
                allFacilities.Remove(item);
            }
        }

        private void RemoveFacility_Click(object sender, RoutedEventArgs e) {
            List<Facility> facilities = new List<Facility>();
            foreach (var item in ListBoxSelectedFacilities.SelectedItems) facilities.Add((Facility)item);
            foreach (var item in facilities) {
                selectedFacilities.Remove(item);
                allFacilities.Add(item);
            }
        }

        private void AddAllFacilities_Click(object sender, RoutedEventArgs e) {
            List<Facility> facilities = new List<Facility>();
            foreach (var item in ListBoxAllFacilities.Items) facilities.Add((Facility)item);
            foreach (var item in facilities) {
                selectedFacilities.Add(item);
                allFacilities.Remove(item);
            }
        }

        private void RemoveAllFacilities_Click(object sender, RoutedEventArgs e) {
            List<Facility> facilities = new List<Facility>();
            foreach (var item in ListBoxSelectedFacilities.Items) facilities.Add((Facility)item);
            foreach (var item in facilities) {
                selectedFacilities.Remove(item);
                allFacilities.Add(item);
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e) {

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) {
            Close();
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
    }
}
