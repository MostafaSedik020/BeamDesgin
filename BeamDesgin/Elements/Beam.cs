using BeamDesgin.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System.Windows;
using System.Data;

namespace BeamDesgin.Elements
{
    public class Beam : INotifyPropertyChanged
    {
        private double _Breadth;
        private double _Depth;
        private FlexuralRft _ChosenCornerAsTop;
        private FlexuralRft _ChosenMidAsTop;
        private FlexuralRft _ChosenCornerAsBot;
        private FlexuralRft _ChosenAsMidBot;
        private ShearRFT _ChosenShearAsCorner;
        private ShearRFT _ChosenShearAsMid;
        private bool _isSelected;
        private BeamMark _Mark;

        // Public properties with getters and setters
        public double Breadth
        {
            get { return _Breadth; }
            set
            {
                if (_Breadth != value)
                {
                    _Breadth = value;
                    OnPropertyChanged(nameof(Breadth));
                }
            }
        }

        public double Depth
        {
            get { return _Depth; }
            set
            {
                if (_Depth != value)
                {
                    _Depth = value;
                    OnPropertyChanged(nameof(Depth));
                }
            }
        }

        public FlexuralRft ChosenCornerAsTop
        {
            get { return _ChosenCornerAsTop; }
            set
            {
                if (_ChosenCornerAsTop != value)
                {
                    _ChosenCornerAsTop = value;
                    OnPropertyChanged(nameof(ChosenCornerAsTop));
                }
            }
        }

        public FlexuralRft ChosenMidAsTop
        {
            get { return _ChosenMidAsTop; }
            set
            {
                if (_ChosenMidAsTop != value)
                {
                    _ChosenMidAsTop = value;
                    OnPropertyChanged(nameof(ChosenMidAsTop));
                }
            }
        }

        public FlexuralRft ChosenCornerAsBot
        {
            get { return _ChosenCornerAsBot; }
            set
            {
                if (_ChosenCornerAsBot != value)
                {
                    _ChosenCornerAsBot = value;
                    OnPropertyChanged(nameof(ChosenCornerAsBot));
                }
            }
        }

        public FlexuralRft ChosenAsMidBot
        {
            get { return _ChosenAsMidBot; }
            set
            {
                if (_ChosenAsMidBot != value)
                {
                    _ChosenAsMidBot = value;
                    OnPropertyChanged(nameof(ChosenAsMidBot));
                }
            }
        }

        public ShearRFT ChosenShearAsCorner
        {
            get { return _ChosenShearAsCorner; }
            set
            {
                if (_ChosenShearAsCorner != value)
                {
                    _ChosenShearAsCorner = value;
                    OnPropertyChanged(nameof(ChosenShearAsCorner));
                }
            }
        }

        public ShearRFT ChosenShearAsMid
        {
            get { return _ChosenShearAsMid; }
            set
            {
                if (_ChosenShearAsMid != value)
                {
                    _ChosenShearAsMid = value;
                    OnPropertyChanged(nameof(ChosenShearAsMid));
                }
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public BeamMark Mark
        {
            get { return _Mark; }
            set
            {
                if (_Mark != value)
                {
                    _Mark = value;
                    OnPropertyChanged(nameof(Mark));
                    
                }
            }
        }
        public List<int> RebarSizes { get; } = new List<int> { 12, 14, 16, 18, 20, 22, 25 };  // Constant list of rebar sizes

        private int _selectedRebarSize1;
        public int SelectedRebarSize1
        {
            get { return _selectedRebarSize1; }
            set
            {
                if (_selectedRebarSize1 != value)
                {
                    _selectedRebarSize1 = value;
                    OnPropertyChanged(nameof(SelectedRebarSize1));
                    
                }
            }
        }

        private int _selectedRebarSize2;
        public int SelectedRebarSize2
        {
            get { return _selectedRebarSize2; }
            set
            {
                if (_selectedRebarSize2 != value)
                {
                    _selectedRebarSize2 = value;
                    OnPropertyChanged(nameof(SelectedRebarSize2));
                }
            }
        }

        private int _selectedRebarSize3;
        public int SelectedRebarSize3
        {
            get { return _selectedRebarSize3; }
            set
            {
                if (_selectedRebarSize3 != value)
                {
                    _selectedRebarSize3 = value;
                    OnPropertyChanged(nameof(SelectedRebarSize3));
                }
            }
        }

        private int _selectedRebarSize4;
        public int SelectedRebarSize4
        {
            get { return _selectedRebarSize4; }
            set
            {
                if (_selectedRebarSize4 != value)
                {
                    _selectedRebarSize4 = value;
                    OnPropertyChanged(nameof(SelectedRebarSize4));
                }
            }
        }

        private int _selectedRebarSize5;
        public int SelectedRebarSize5
        {
            get { return _selectedRebarSize5; }
            set
            {
                if (_selectedRebarSize5 != value)
                {
                    _selectedRebarSize5 = value;
                    OnPropertyChanged(nameof(SelectedRebarSize5));
                }
            }
        }

        private int _selectedRebarSize6;
        public int SelectedRebarSize6
        {
            get { return _selectedRebarSize6; }
            set
            {
                if (_selectedRebarSize6 != value)
                {
                    _selectedRebarSize6 = value;
                    OnPropertyChanged(nameof(SelectedRebarSize6));
                }
            }
        }



        // Remaining public properties
        public string UniqueName { get; set; }
        public string Name { get; set; }
        public double TopCornerAs { get; set; }
        public double TopMiddleAs { get; set; }
        public double BotCornerAs { get; set; }
        public double BotMiddleAs { get; set; }
        public double CornerShearAs { get; set; }
        public int Count { get; set; }

        //properties for etabs only
        public string EtabsStory { get; set; }
        public double point1X { get; set; }
        public double point1Y { get; set; }
        public double point1Z { get; set; }
        public double point2X { get; set; }
        public double point2Y { get; set; }
        public double point2Z { get; set; }
        public double Angle { get; set; }


        // Data exported to Revit
        // paramerter names a all uppercase because it mimic
        // the their apperance in revit

        public string BeamMark => $"{Mark.Type}{Mark.Number}";
        public string BOTTOM_RFT_CORNER => $"{ChosenCornerAsBot.NumberOfBars}T{ChosenCornerAsBot.Diameter}";
        public string BOTTOM_RFT_MID => $"{ChosenAsMidBot.NumberOfBars}T{ChosenAsMidBot.Diameter}";
        public string TOP_RFT_CORNER => $"{ChosenCornerAsTop.NumberOfBars}T{ChosenCornerAsTop.Diameter}";
        public string TOP_RFT_MID => $"{ChosenMidAsTop.NumberOfBars}T{ChosenMidAsTop.Diameter}";
        public string LINKS_CORNER => ChosenShearAsMid.Diameter == 8
            ? $"{ChosenShearAsCorner.NumberOfBars}LR{ChosenShearAsCorner.Diameter}@{ChosenShearAsCorner.spacing}"
            : $"{ChosenShearAsCorner.NumberOfBars}LT{ChosenShearAsCorner.Diameter}@{ChosenShearAsCorner.spacing}";
        public string LINKS_MID => ChosenShearAsMid.Diameter == 8
            ? $"{ChosenShearAsMid.NumberOfBars}LR{ChosenShearAsMid.Diameter}@{ChosenShearAsMid.spacing}"
            : $"{ChosenShearAsMid.NumberOfBars}LT{ChosenShearAsMid.Diameter}@{ChosenShearAsMid.spacing}";


        public Beam()
        {
            ChosenCornerAsTop = new FlexuralRft();
            ChosenMidAsTop = new FlexuralRft();
            ChosenCornerAsBot = new FlexuralRft();
            ChosenAsMidBot = new FlexuralRft();
            ChosenShearAsCorner = new ShearRFT();
            ChosenShearAsMid = new ShearRFT();
            Mark = new BeamMark();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            
        }
    }
}
