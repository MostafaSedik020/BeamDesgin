using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using BeamDesgin.Excel;
using BeamDesgin.Elements;
using BeamDesgin.Data;
using System.Text;

namespace BeamDesgin.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Document doc;
        private ObservableCollection<Beam> _beamsData;
        public ObservableCollection<Beam> BeamsData
        {
            get { return _beamsData; }
            set { _beamsData = value; }
        }

        public MainWindow(Document doc)
        {
            InitializeComponent();
            this.doc = doc;
            _beamsData = new ObservableCollection<Beam>(); // Initialize as an empty collection
            this.DataContext = this; // Set DataContext to the window itself
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Any actions needed when the window is loaded
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                System.Windows.Point position = e.GetPosition(this);

                if (position.Y <= 30)
                {
                    if (this.WindowState == WindowState.Maximized)
                    {
                        var mousePosition = PointToScreen(e.MouseDevice.GetPosition(this));
                        this.WindowState = WindowState.Normal;
                        this.Left = mousePosition.X - (this.RestoreBounds.Width / 2);
                        this.Top = 0;
                        this.DragMove();
                    }
                    else if (this.WindowState == WindowState.Normal)
                    {
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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files|*.xls;*.xlsx|All Files|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
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

            BeamsData.Clear(); // Clear any existing data in the ObservableCollection

            List<Beam> beamsData = ManageExcel.GetBeamsData(Path_TxtBox.Text);
            List<int> selectedRebars = GetSelectedRebarSizes();
            var sortedList = ManageData.SortData(beamsData, selectedRebars);

            foreach (Beam beam in sortedList)
            {
                BeamsData.Add(beam); // Add the new data to the ObservableCollection
            }

            StringBuilder sb = new StringBuilder();
            foreach (Beam beam in BeamsData)
            {
                sb.Append(beam.BotAsMidData.ToString());
            }
            //MessageBox.Show($"BeamsData count: {sb}", "Debug", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Return the selected rebar sizes.
        /// </summary>
        /// <returns>List of integers</returns>
        private List<int> GetSelectedRebarSizes()
        {
            var selectedRebars = new List<int>();

            if (RebarExpander.Content is System.Windows.Controls.Grid rebarGrid)
            {
                foreach (var panel in rebarGrid.Children.OfType<StackPanel>())
                {
                    foreach (var child in panel.Children.OfType<CheckBox>())
                    {
                        if (child.IsChecked == true)
                        {
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
    }
}
