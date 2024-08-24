using BeamDesgin.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeamDesgin.Elements
{
    public class Beam : INotifyPropertyChanged
    {
        public string UniqueName { get; set; }

        public string Name { get; set; }

        public double TopCornerAs { get; set; }

        public double TopMiddleAs { get; set; }

        public double BotCornerAs { get; set; }

        public double BotMiddleAs { get; set; }

        public double CornerShearAs { get; set; }

        public BeamMark Mark { get; set; }

        public double Breadth { get; set; }

        public double Depth { get; set; }

        public FlexuralRft ChosenCornerAsTop { get; set; }

        public FlexuralRft ChosenMidAsTop { get; set; }

        public FlexuralRft ChosenCornerAsBot { get; set; }

        public FlexuralRft ChosenAsMidBot { get; set; }

        public ShearRFT ChosenShearAsCorner { get; set; }

        public ShearRFT ChosenShearAsMid { get; set; }

        private bool _isSelected;
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

        //Data exported to Revit

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
