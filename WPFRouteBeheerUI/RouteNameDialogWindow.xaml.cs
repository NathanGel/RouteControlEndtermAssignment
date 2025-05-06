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

namespace WPFRouteBeheerUI {
    /// <summary>
    /// Interaction logic for RouteNameDialogWindow.xaml
    /// </summary>
    public partial class RouteNameDialogWindow : Window {
        public string RouteName;
        public RouteNameDialogWindow() {
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
            this.Close();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e) {
            RouteName = TxtRouteName.Text;
            DialogResult = true;
            this.Close();
        }
    }
}
