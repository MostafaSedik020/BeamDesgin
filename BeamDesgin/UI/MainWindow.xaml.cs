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
using BeamDesgin.Revit;
using System.ComponentModel;
using BeamDesgin.Etabs;
using System.Windows.Controls.Primitives;
using BeamDesgin.Entry;


namespace BeamDesgin.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // Declare the event from INotifyPropertyChanged interface
        public event PropertyChangedEventHandler PropertyChanged;

        // Method to raise the PropertyChanged event
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        Document doc;
        private ObservableCollection<Beam> _BeamsData;
        private ObservableCollection<Beam> _UserList;



        private List<int> selectedRebars;
        private int numOfWarnings;
        public ObservableCollection<Beam> BeamsData
        {
            get { return _BeamsData; }
            set { _BeamsData = value; }
        }
        public ObservableCollection<Beam> UserList
        {
            get { return _UserList; }
            set { _UserList = value; OnPropertyChanged("UserList"); }
        }

        public MainWindow(Document doc)
        {
            InitializeComponent();
            this.doc = doc;
            _BeamsData = new ObservableCollection<Beam>(); // Initialize as an empty collection
            _UserList = new ObservableCollection<Beam>(); // Now ObservableCollection
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

        //private void Browse_Button_Click(object sender, RoutedEventArgs e)
        //{
        //    OpenFileDialog openFileDialog = new OpenFileDialog();
        //    openFileDialog.Filter = "Excel Files|*.xls;*.xlsx|All Files|*.*";

        //    if (openFileDialog.ShowDialog() == true)
        //    {
        //        string selectedFilePath = openFileDialog.FileName;
        //        Path_TxtBox.Text = selectedFilePath;
        //    }
        //}

        private void desgin_btn_click(object sender, RoutedEventArgs e)
        {
            //if (string.IsNullOrWhiteSpace(Path_TxtBox.Text))
            //{
            //    MessageBox.Show("Please select a valid file path.", "Invalid Path", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    return;
            //}

            //old version to get data using excel
            //List<Beam> beamsData = ManageExcel.GetBeamsData(Path_TxtBox.Text);

            //new version using etabs
            //find etabs model
            ManageEtabs.LinkEtabsModel();

            //get all groups in etabs
            string[] etabsGroupNames = ManageEtabs.GetGroupNames();
            string selectedGroup = "All";
            string selectedDesignCode = "ACI";

            ChooseGroupWindow chooseGroupWindow = new ChooseGroupWindow(etabsGroupNames);
            if (chooseGroupWindow.ShowDialog() == true)
            {
                selectedGroup = chooseGroupWindow.SelectedOption;
                selectedDesignCode = chooseGroupWindow.SelectedDesignCode;
            }
            else
            {
                return;
            }

            //get data from etabs
            List<Beam> beamsData = ManageEtabs.GetDataFromEtabs(selectedGroup, selectedDesignCode);

            //get the selected rebars from the user
            selectedRebars = GetSelectedRebarSizes();


            string prequal = "B";
            int startNum = 1;
            int result;
            bool isInt = int.TryParse(startNum_txtbox.Text, out result);
            if (!string.IsNullOrWhiteSpace(prequal_txtbox.Text))
            {
                prequal = prequal_txtbox.Text;
            }
            if (!string.IsNullOrWhiteSpace(startNum_txtbox.Text) || isInt)
            {
                startNum = result;
            }
            var newList = ManageData.transAreaData(beamsData, selectedRebars, prequal, startNum);

            foreach (Beam beam in newList)
            {
                BeamsData.Add(beam); // Add the new data to the ObservableCollection

            }

            //Check the Revit model once to avoid redundant calls
            numOfWarnings = RevitUtils.CheckRevitModel(BeamsData, doc).count;
            if (numOfWarnings > 0)
            {
                warningcount_label.Content = $"Warning Found : {numOfWarnings}";
                warningcount_label.Visibility = System.Windows.Visibility.Visible;
                warning_btn.Visibility = System.Windows.Visibility.Visible;
            }

            foreach (Beam beam in BeamsData)
            {
                Beam incrementedBeam = FindNextStrongerBeam(beam);

                if (incrementedBeam != null)
                {
                    UpdateBeamsMark(beam.Mark.Number, incrementedBeam);
                }
            }

            UpdateDataGrid(prequal, startNum); // Update grid once after processing all beams

            // Disable the design button after processing
            desgin_btn.IsEnabled = false;

        }
        private void warning_btn_Click(object sender, RoutedEventArgs e)
        {
            WarningWindow warningWindow = new WarningWindow(BeamsData, doc);

            warningWindow.ShowDialog();
        }

        private void Update_btn_Click(object sender, RoutedEventArgs e)
        {
            //var aselect = BeamDataGrid.Items.OfType<Beam>().Where(b =>b.SelectedRebarSize1 != 0).FirstOrDefault();
            var selectedBeams = BeamDataGrid.Items
                                .OfType<Beam>() // Filter out non-Beam items
                                .Where(b => b.IsSelected)
                                .ToList();
            string prequal = "B";
            int startNum = 1;
            int result;
            bool isInt = int.TryParse(startNum_txtbox.Text, out result);
            if (!string.IsNullOrWhiteSpace(prequal_txtbox.Text))
            {
                prequal = prequal_txtbox.Text;
            }
            if (!string.IsNullOrWhiteSpace(startNum_txtbox.Text) || isInt)
            {
                startNum = result;
            }
            bool areAllConcDimEqual = selectedBeams.All(b => b.Breadth == selectedBeams.First().Breadth) &&
                selectedBeams.All(b => b.Depth == selectedBeams.First().Depth);
            if (selectedBeams.Any() && areAllConcDimEqual == true)
            {
                MergeBeams(selectedBeams);

                UpdateDataGrid(prequal,startNum); // Update grid once after processing all beams
            }
            else if (selectedBeams.Any() && areAllConcDimEqual == false)
            {
                MessageBox.Show("Selected beams are not the same concrete dimensions", "Process Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                MessageBox.Show("No beams selected.");
            }
        }

        private void Update_rebar_only_Click(object sender, RoutedEventArgs e)
        {
            //var aselect = BeamDataGrid.Items.OfType<Beam>().Where(b => b.SelectedRebarSize1 != 0).FirstOrDefault();
            var selectedBeams = BeamDataGrid.Items
                                .OfType<Beam>() // Filter out non-Beam items
                                .Where(b => b.IsSelected)
                                .ToList();
            string prequal = "B";
            int startNum = 1;
            int result;
            bool isInt = int.TryParse(startNum_txtbox.Text, out result);
            if (!string.IsNullOrWhiteSpace(prequal_txtbox.Text))
            {
                prequal = prequal_txtbox.Text;
            }
            if (!string.IsNullOrWhiteSpace(startNum_txtbox.Text) || isInt )
            {
                startNum = result;
            }
            if (selectedBeams.Any())
            {
                bool IsChangeable = selectedBeams.Any(b => b.SelectedRebarSize1 != 0 || b.SelectedRebarSize2 != 0 ||
                                                         b.SelectedRebarSize3 != 0 || b.SelectedRebarSize4 != 0 ||
                                                         b.SelectedRebarSize5 != 0 || b.SelectedRebarSize6 != 0);


                if (IsChangeable)
                {
                    List<Beam> changeableBeams = selectedBeams.Where(b => b.SelectedRebarSize1 != 0 || b.SelectedRebarSize2 != 0 ||
                                                         b.SelectedRebarSize3 != 0 || b.SelectedRebarSize4 != 0 ||
                                                         b.SelectedRebarSize5 != 0 || b.SelectedRebarSize6 != 0).ToList();
                    foreach (Beam beam in changeableBeams)
                    {
                        ChangeRebarOnly(beam);
                        //MessageBox.Show("This button is not working now Please try again later XD");
                    }

                    UpdateDataGrid(prequal, startNum);
                }
                else
                {
                    MessageBox.Show("No Rebar changes found selected.");
                }
 
            }
            else
            {
                
                MessageBox.Show("No beams selected.");
            }
        }

        private void BeamDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(BeamDataGrid.SelectedItems.Count > 0)
            {
                Update_btn.IsEnabled=true;
                rebar_only_btn.IsEnabled=true;
            }
            else 
            { 
                Update_btn.IsEnabled=false; 
                rebar_only_btn.IsEnabled=false;
            }
            
            
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
            warning_btn.Visibility = System.Windows.Visibility.Hidden;
            warningcount_label.Visibility = System.Windows.Visibility.Hidden;
            UserList.Clear();
            BeamsData.Clear();
        }

        private void Import_btn_Click(object sender, RoutedEventArgs e)
        {

            if (numOfWarnings > 0)
            {
               MessageBoxResult result = MessageBox.Show($"There are {numOfWarnings} warnings left, proceed to revit?", "Warning",MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    //model from
                    RevitUtils.SendDataToRevit(BeamsData, doc);

                    //modless form
                    //RvtData.BeamsData = BeamsData;
                    //ExtCmd.ExtEvent.Raise();
                }

            }
            else
            {
                //model from
                RevitUtils.SendDataToRevit(BeamsData, doc);

                //modless form
                //RvtData.BeamsData = BeamsData;
                //ExtCmd.ExtEvent.Raise();
            }



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
        private void UpdateDataGrid(string prequal, int startNum)
        {
            UserList.Clear();

            var newSortedData = ManageData.sortData(BeamsData, prequal, startNum);
            BeamsData.Clear();

            foreach (var beam in newSortedData)
            {
                BeamsData.Add(beam); // Add sorted beams back to the UserList
            }

           

            var uniqueList = ManageData.UniqueSortedData(newSortedData);

            foreach (Beam beam in uniqueList)
            {
                int count = BeamsData.Where(b=> b.Mark.Number == beam.Mark.Number).Count();
                beam.Count = count;
                beam.IsSelected = false;
                
                UserList.Add(beam); // Add the new data to the user

            }

           
        }

        private void ChangeRebarOnly(Beam changingBeam )
        {
            foreach (var beam in BeamsData)
            {
                if (beam.Mark.Number == changingBeam.Mark.Number)
                {
                    if (beam.SelectedRebarSize1 != 0)
                    {
                        var newRft = ManageRft.ChangeRftDiameter(beam.ChosenAsMidBot,beam.SelectedRebarSize1,beam.Breadth);
                        beam.ChosenAsMidBot.Diameter = newRft.diameter;
                        beam.ChosenAsMidBot.NumberOfBars = newRft.noOfBars;
                    }

                    if (beam.SelectedRebarSize2 != 0)
                    {
                        var newRft = ManageRft.ChangeRftDiameter(beam.ChosenCornerAsBot,beam.SelectedRebarSize2,beam.Breadth);
                        beam.ChosenCornerAsBot.Diameter = newRft.diameter;
                        beam.ChosenCornerAsBot.NumberOfBars = newRft.noOfBars;
                    }

                    if (beam.SelectedRebarSize3 != 0)
                    {
                        var newRft = ManageRft.ChangeRftDiameter(beam.ChosenMidAsTop,beam.SelectedRebarSize3,beam.Breadth);
                        beam.ChosenMidAsTop.Diameter = newRft.diameter;
                        beam.ChosenMidAsTop.NumberOfBars = newRft.noOfBars;
                    }

                    if (beam.SelectedRebarSize4 != 0)
                    {
                        var newRft = ManageRft.ChangeRftDiameter(beam.ChosenCornerAsTop,beam.SelectedRebarSize4,beam.Breadth);
                        beam.ChosenCornerAsTop.Diameter = newRft.diameter;
                        beam.ChosenCornerAsTop.NumberOfBars = newRft.noOfBars;
                    }

                }
            }
        }

        private Beam FindNextStrongerBeam(Beam selectedBeam)
        {
            Beam newbeam = BeamsData.Where(b => b.Depth == selectedBeam.Depth &&
                                            b.Breadth == selectedBeam.Breadth &&
                                            b.Mark.Number > selectedBeam.Mark.Number &&
                                            //b.ChosenAsMidBot.Diameter == selectedBeam.ChosenAsMidBot.Diameter &&
                                            ManageRft.GetAreaRFT(b.ChosenAsMidBot) >= ManageRft.GetAreaRFT(selectedBeam.ChosenAsMidBot) &&
                                            ManageRft.GetAreaRFT(b.ChosenCornerAsTop) == ManageRft.GetAreaRFT(selectedBeam.ChosenCornerAsTop) &&
                                            ManageRft.GetAreaRFT(b.ChosenShearAsCorner) == ManageRft.GetAreaRFT(selectedBeam.ChosenShearAsCorner))
                                      .FirstOrDefault();


            return newbeam;
                
        }
        private void MergeBeams(List<Beam> beamList)
        {
            //get highest values
            //bot mid rft
            Beam beamWithMaxAreaBot = beamList.OrderByDescending(b => ManageRft.GetAreaRFT(b.ChosenAsMidBot))
                                          .FirstOrDefault();
            //top corner
            Beam beamWithMaxAreaTop = beamList.OrderByDescending(b => ManageRft.GetAreaRFT(b.ChosenCornerAsTop))
                                          .FirstOrDefault();

            //shear rft
            Beam beamWithMaxShearRft = beamList.OrderByDescending(b => ManageRft.GetAreaRFT(b.ChosenShearAsCorner))
                                          .FirstOrDefault();



            foreach (Beam beam in BeamsData)
            {
                if (beamList.Any(b => b.Mark.Number == beam.Mark.Number))
                {
                    //change bot rft
                    beam.ChosenAsMidBot.Diameter = beamWithMaxAreaBot.ChosenAsMidBot.Diameter;
                    beam.ChosenAsMidBot.NumberOfBars = beamWithMaxAreaBot.ChosenAsMidBot.NumberOfBars;

                    beam.ChosenCornerAsBot.Diameter = beamWithMaxAreaBot.ChosenCornerAsBot.Diameter;
                    beam.ChosenCornerAsBot.NumberOfBars = beamWithMaxAreaBot.ChosenCornerAsBot.NumberOfBars;

                    //change top rft
                    beam.ChosenMidAsTop.Diameter = beamWithMaxAreaTop.ChosenMidAsTop.Diameter;
                    beam.ChosenMidAsTop.NumberOfBars = beamWithMaxAreaTop.ChosenMidAsTop.NumberOfBars;

                    beam.ChosenCornerAsTop.Diameter = beamWithMaxAreaTop.ChosenCornerAsTop.Diameter;
                    beam.ChosenCornerAsTop.NumberOfBars = beamWithMaxAreaTop.ChosenCornerAsTop.NumberOfBars;

                    //change shear rft
                    beam.ChosenShearAsMid.Diameter = beamWithMaxShearRft.ChosenShearAsMid.Diameter;
                    beam.ChosenShearAsMid.NumberOfBars = beamWithMaxShearRft.ChosenShearAsMid.NumberOfBars;
                    beam.ChosenShearAsMid.spacing = beamWithMaxShearRft.ChosenShearAsMid.spacing;

                    beam.ChosenShearAsCorner.NumberOfBars = beamWithMaxShearRft.ChosenShearAsCorner.NumberOfBars;
                    beam.ChosenShearAsCorner.Diameter = beamWithMaxShearRft.ChosenShearAsCorner.Diameter;
                    beam.ChosenShearAsCorner.spacing = beamWithMaxShearRft.ChosenShearAsCorner.spacing;
                }
            }
            
        }

        private void UpdateBeamsMark(int selectedMarkNumber, Beam incrementedBeam)
        {
            //var parentBeams = BeamsData.Where(b => b.Mark.Number == selectedMarkNumber).ToList();


            foreach ( var beam in BeamsData )
            {
                if (beam.Mark.Number == selectedMarkNumber)
                {
                    /*MessageBox.Show($"before {beam.BeamMark}")*/;
                    beam.Mark = incrementedBeam.Mark;
                    beam.ChosenCornerAsBot = incrementedBeam.ChosenCornerAsBot;
                    beam.ChosenAsMidBot = incrementedBeam.ChosenAsMidBot;
                    beam.ChosenCornerAsTop = incrementedBeam.ChosenCornerAsTop;
                    beam.ChosenMidAsTop = incrementedBeam.ChosenMidAsTop;
                    beam.ChosenShearAsCorner = incrementedBeam.ChosenShearAsCorner;
                    beam.ChosenShearAsMid = incrementedBeam.ChosenShearAsMid;
                    //MessageBox.Show($"after {beam.BeamMark}");
                }
            }

           
        }

        
    }
}
