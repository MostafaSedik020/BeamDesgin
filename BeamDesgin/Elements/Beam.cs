using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeamDesgin.Elements
{
    public class Beam
    {
        public string UniqueName { get; set; }

        public string Name { get; set; }

        public double TopCornerAs { get; set; }

        public double TopMiddleAs { get; set; }

        public double BotCornerAs { get; set; }

        public double BotMiddleAs { get; set; }

        public double CornerShearAs { get; set; }

        public string BeamMark { get; set; }

        public double Breadth { get; set; }

        public double Depth { get; set; }

        public FlexuralRft ChosenCornerAsTop { get; set; }

        public FlexuralRft ChosenMidAsTop { get; set; }

        public FlexuralRft ChosenCornerAsBot { get; set; }

        public FlexuralRft ChosenAsMidBot { get; set; }

        public ShearRFT ChosenShearAsCorner { get; set; }

        public ShearRFT ChosenShearAsMid { get; set; }

        public Beam()
        {
            ChosenCornerAsTop = new FlexuralRft();
            ChosenMidAsTop = new FlexuralRft();
            ChosenCornerAsBot = new FlexuralRft();
            ChosenAsMidBot = new FlexuralRft();
            ChosenShearAsCorner = new ShearRFT();
            ChosenShearAsMid = new ShearRFT();
        }
    }
}
