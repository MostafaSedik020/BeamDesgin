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
                    MessageBox.Show("done");
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

        // Remaining public properties
        public string UniqueName { get; set; }
        public string Name { get; set; }
        public double TopCornerAs { get; set; }
        public double TopMiddleAs { get; set; }
        public double BotCornerAs { get; set; }
        public double BotMiddleAs { get; set; }
        public double CornerShearAs { get; set; }

        // Data exported to Revit
        
        public string BeamMark => $"{Mark.Type}{Mark.Number}";
        public string BotAsCornerData => $"{ChosenCornerAsBot.NumberOfBars}T{ChosenCornerAsBot.Diameter}";
        public string BotAsMidData => $"{ChosenAsMidBot.NumberOfBars}T{ChosenAsMidBot.Diameter}";
        public string TopAsCornerData => $"{ChosenCornerAsTop.NumberOfBars}T{ChosenCornerAsTop.Diameter}";
        public string TopAsMidData => $"{ChosenMidAsTop.NumberOfBars}T{ChosenMidAsTop.Diameter}";
        public string Corner_Astr_Data => $"{ChosenShearAsCorner.NumberOfBars}LT{ChosenShearAsCorner.Diameter}@{ChosenShearAsCorner.spacing}";
        public string Mid_Astr_Data => $"{ChosenShearAsMid.NumberOfBars}LT{ChosenShearAsMid.Diameter}@{ChosenShearAsMid.spacing}";

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
