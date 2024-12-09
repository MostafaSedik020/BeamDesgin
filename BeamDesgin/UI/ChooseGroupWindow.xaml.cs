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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeamDesgin.UI
{
    /// <summary>
    /// Interaction logic for ChooseGroupWindow.xaml
    /// </summary>
    public partial class ChooseGroupWindow : Window
    {
        public string SelectedOption { get; private set; }
        public string SelectedDesignCode { get; private set; }
        public ChooseGroupWindow(string[] options)
        {
            InitializeComponent();
            // Set the DataContext of the window to the list of strings
            this.DataContext = options;
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected option from the ComboBox
            SelectedOption = OptionsComboBox.SelectedItem as string;
            if (AciRadioButton.IsChecked == true)
            {
                SelectedDesignCode = "ACI";
            }
            else if (EcpRadioButton.IsChecked == true)
            {
                SelectedDesignCode = "ECP";
            }

            // Close the window and set DialogResult to true
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the window without setting DialogResult
            DialogResult = false;
            Close();
        }


    }
}
