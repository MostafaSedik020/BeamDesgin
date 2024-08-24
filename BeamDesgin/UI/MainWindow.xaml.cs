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
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            // Only allow moving the window when the left mouse button is pressed
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Get the mouse position relative to the window
                System.Windows.Point position = e.GetPosition(this);

                // Check if the mouse is within the top 30 pixels of the window
                if (position.Y <= 30)
                {
                    // If the window is maximized and dragging from the top 30 pixels
                    if (this.WindowState == WindowState.Maximized)
                    {
                        // Calculate the mouse position relative to the screen
                        var mousePosition = PointToScreen(e.MouseDevice.GetPosition(this));

                        // Restore the window size
                        this.WindowState = WindowState.Normal;

                        // Set the window position so the top of the window aligns with the mouse pointer
                        // Offset the window's Left and Top based on the mouse position
                        this.Left = mousePosition.X - (this.RestoreBounds.Width / 2);
                        this.Top = 0; // Align the window's top with the screen's top

                        // Move the window
                        this.DragMove();
                    }
                    else if (this.WindowState == WindowState.Normal)
                    {
                        // If the window is already normal, just move it
                        this.DragMove();
                    }
                }
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

        private void desgin_btn_click(object sender, RoutedEventArgs e)
        {
            
            if (string.IsNullOrWhiteSpace(Path_TxtBox.Text))
            {
                MessageBox.Show("Please select a valid file path.", "Invalid Path", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            UpdateDataGrid();

        }






        /// <summary>
        /// return the selected rebar sizes 
        /// </summary>
        /// <returns> List of integers</returns>
        private List<int> GetSelectedRebarSizes()
        {
            var selectedRebars = new List<int>();

            // Ensure the Expander's content is a Grid and not null
            if (RebarExpander.Content is System.Windows.Controls.Grid rebarGrid)
            {
                // Iterate through each StackPanel in the Grid
                foreach (var panel in rebarGrid.Children.OfType<StackPanel>())
                {
                    // Iterate through each CheckBox in the StackPanel
                    foreach (var child in panel.Children.OfType<CheckBox>())
                    {
                        if (child.IsChecked == true)
                        {
                            // Convert the CheckBox content to an integer and add it to the list
                            if (int.TryParse(child.Content.ToString(), out int rebarSize))
                            {
                                selectedRebars.Add(rebarSize);
                            }
                        }
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

               foreach (Beam beam in sortedList)
               {
                    BeamDataGrid.Items.Add(beam);
               }
        }
    }
    
}
