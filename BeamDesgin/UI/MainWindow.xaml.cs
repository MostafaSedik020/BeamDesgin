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
using System.Diagnostics;
using BeamDesgin.ManageElements;

namespace BeamDesgin.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Document doc;
        private ObservableCollection<Beam> _beamsData;

        private List<Beam> ParentList;
        private List<int> selectedRebars;
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
            selectedRebars = GetSelectedRebarSizes();
            ParentList = ManageData.transAreaData(beamsData, selectedRebars);

            var uniqueList = ManageData.UniqueSortedData(ParentList);

            foreach (Beam beam in uniqueList)
            {
                BeamsData.Add(beam); // Add the new data to the ObservableCollection

            }

            desgin_btn.IsEnabled = false;

        }

        private void Update_btn_Click(object sender, RoutedEventArgs e)
        {

            var selectedBeams = BeamDataGrid.Items
                                .OfType<Beam>() // Filter out non-Beam items
                                .Where(b => b.IsSelected)
                                .ToList();

            if (selectedBeams.Any())
            {
                foreach (Beam selectedBeam in selectedBeams)
                {
                    Beam incrementedBeam = FindNextStrongerBeam(selectedBeam);

                    if (incrementedBeam != null)
                    {
                        UpdateBeamsMark(selectedBeam.Mark.Number, incrementedBeam);
                    }
                }

                UpdateDataGrid(); // Update grid once after processing all beams
            }
            else
            {
                MessageBox.Show("No beams selected.");
            }
        }
        private void refresh_btn_Click(object sender, RoutedEventArgs e)
        {
            BeamDataGrid.Items.Refresh();
        }
        private void BeamDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(BeamDataGrid.SelectedItems.Count > 0)
            {
                Update_btn.IsEnabled=true;
            }
            else { Update_btn.IsEnabled=false; }
            
            
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateButtonState();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateButtonState();
        }

        private void Clear_btn_Click(object sender, RoutedEventArgs e)
        {
            desgin_btn.IsEnabled= true;
            BeamsData.Clear();
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

        private void UpdateButtonState()
        {
            var anyChecked = BeamDataGrid.Items
                            .OfType<Beam>() // Filter out non-Beam items
                            .Any(b => b.IsSelected);

            Update_btn.IsEnabled = anyChecked;
        }

        

        /// <summary>
        /// update the data grid in the UI
        /// needs List of Beams
        /// </summary>
        /// <param name="beamList"> the beam list that will be represented to the user (aka  a unique list)</param>
        private void UpdateDataGrid()
        {
            BeamsData.Clear();

            ParentList = ManageData.sortData(ParentList);

            int newMarkNumber = 1;

            int oldMarkNumber = 0;

            foreach (var beam in ParentList)
            {
                if (beam.Mark.Number > oldMarkNumber)
                {
                    oldMarkNumber = beam.Mark.Number;

                    beam.Mark.Number = newMarkNumber;

                    newMarkNumber++;
                }
                else if( beam.Mark.Number == oldMarkNumber)
                {
                    beam.Mark.Number = newMarkNumber;
                }
            }

            var uniqueList = ManageData.UniqueSortedData(ParentList);

            foreach (Beam beam in uniqueList)
            {
                beam.IsSelected = false;
                BeamsData.Add(beam); // Add the new data to the ObservableCollection
                
            }
        }

        private Beam FindNextStrongerBeam(Beam selectedBeam)
        {
            return ParentList
                .Where(pBeam =>
                    pBeam.Depth == selectedBeam.Depth &&
                    pBeam.Breadth == selectedBeam.Breadth &&
                    pBeam.Mark.Number > selectedBeam.Mark.Number &&
                    ManageRft.GetAreaRFT(pBeam.ChosenAsMidBot) + ManageRft.GetAreaRFT(pBeam.ChosenCornerAsBot) >=
                    ManageRft.GetAreaRFT(selectedBeam.ChosenAsMidBot) + ManageRft.GetAreaRFT(selectedBeam.ChosenCornerAsBot) &&
                    ManageRft.GetAreaRFT(pBeam.ChosenCornerAsTop) >= ManageRft.GetAreaRFT(selectedBeam.ChosenCornerAsTop) &&
                    ManageRft.GetAreaRFT(pBeam.ChosenShearAsCorner) >= ManageRft.GetAreaRFT(selectedBeam.ChosenShearAsCorner))
                .FirstOrDefault();
        }

        private void UpdateBeamsMark(int selectedMarkNumber, Beam incrementedBeam)
        {
            
            foreach (Beam beam in ParentList.Where(b => b.Mark.Number == selectedMarkNumber))
            {
                MessageBox.Show($"before {beam.BeamMark}");
                beam.Mark = incrementedBeam.Mark;
                beam.ChosenCornerAsTop = incrementedBeam.ChosenCornerAsTop;
                beam.ChosenMidAsTop = incrementedBeam.ChosenMidAsTop;
                beam.ChosenCornerAsBot = incrementedBeam.ChosenCornerAsBot;
                beam.ChosenAsMidBot = incrementedBeam.ChosenAsMidBot;
                beam.ChosenShearAsCorner = incrementedBeam.ChosenShearAsCorner;
                beam.ChosenShearAsMid = incrementedBeam.ChosenShearAsMid;
                MessageBox.Show($"after {beam.BeamMark}");
            }
        }


    }
}
