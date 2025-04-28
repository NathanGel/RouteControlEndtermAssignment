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
using WPFFaciliteitBeheerUI.Model;

namespace WPFFaciliteitBeheerUI {
    /// <summary>
    /// Interaction logic for FacilityWindow.xaml
    /// </summary>
    public partial class FacilityWindow : Window {
        private bool isUpdate;
        public FacilityUI Facility;
        public FacilityWindow(FacilityUI facility ,bool isUpdate) {
            InitializeComponent();
            this.Facility = facility;
            this.isUpdate = isUpdate;
            TopBarTitle.Text = isUpdate ? "Update Facility" : "Add Facility"; // dit bepaald de inhoud van de TopBar volgens de property isUpdate
            if (isUpdate) {
                textBoxId.Text = Facility.Id.ToString();
                textBoxName.Text = Facility.Name;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e) {
            if (isUpdate) {
                Facility.Name = textBoxName.Text;
                DialogResult = true;
            } else {
                Facility = new FacilityUI(textBoxName.Text);
                DialogResult = true;
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}
