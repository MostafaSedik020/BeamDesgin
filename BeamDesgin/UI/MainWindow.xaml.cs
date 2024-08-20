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
using Autodesk.Revit.DB;
using BeamDesgin.Excel;
using Microsoft.Win32;
using BeamDesgin.Elements;
using BeamDesgin.Data;

namespace BeamDesgin.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Document doc;
        public MainWindow(Document doc)
        {
            InitializeComponent();
            this.doc = doc;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }


        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Browse_Button_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Set filter options to only show Excel files
            openFileDialog.Filter = "Excel Files|*.xls;*.xlsx|All Files|*.*";

            // Show the dialog and check if the user selected a file
            if (openFileDialog.ShowDialog() == true)
            {
                // Get the selected file path
                string selectedFilePath = openFileDialog.FileName;

                // You can now use the selected file path as needed
                //MessageBox.Show($"Selected file: {selectedFilePath}");

                Path_TxtBox.Text = selectedFilePath;
            }
        }

        private void desin_btn_click(object sender, RoutedEventArgs e)
        {
            
            if (string.IsNullOrWhiteSpace(Path_TxtBox.Text))
            {
                MessageBox.Show("Please select a valid file path.", "Invalid Path", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            UpdateDataGrid();

            ItemizeCheckBox.Visibility = System.Windows.Visibility.Visible;
        }
        private void ItemizeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateDataGrid();
        }

        private void ItemizeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateDataGrid();
        }





        /// <summary>
        /// return the selected rebar sizes 
        /// </summary>
        /// <returns> List of integers</returns>
        private List<int> GetSelectedRebarSizes()
        {
            var selectedRebars = new List<int>();

            // Iterate through each CheckBox in the Expander's Grid
            foreach (var child in (RebarExpander.Content as System.Windows.Controls.Grid).Children)
            {
                if (child is CheckBox checkBox && checkBox.IsChecked == true)
                {
                    // Convert the CheckBox content to an integer and add it to the list
                    if (int.TryParse(checkBox.Content.ToString(), out int rebarSize))
                    {
                        selectedRebars.Add(rebarSize);
                    }
                }
                
            }
            

            return selectedRebars;
        }

        private void UpdateDataGrid()
        {
            BeamDataGrid.Items.Clear();

            List<Beam> beamsData = ManageExcel.GetBeamsData(Path_TxtBox.Text);

            List<int> selectedRebars = GetSelectedRebarSizes();

            var sortedList = ManageData.SortData(beamsData, selectedRebars);

            if (ItemizeCheckBox.IsChecked == true)
            {
                foreach (Beam beam in sortedList)
                {
                    BeamDataGrid.Items.Add(beam);
                }
            }
            else
            {
                var uniqueList = ManageData.UniqueSortedData(sortedList);

                foreach (Beam beam in uniqueList)
                {
                    BeamDataGrid.Items.Add(beam);
                }
            }

        }
    }
    
}
