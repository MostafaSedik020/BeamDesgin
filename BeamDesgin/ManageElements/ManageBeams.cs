using BeamDesgin.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeamDesgin.Elements;


namespace BeamDesgin.ManageElements
{
    public static class ManageBeams
    {
        public static bool AreBeamsSimilar(Beam beam1, Beam beam2)
        {
            bool check = false;
            if (beam1.Breadth == beam2.Breadth &&
                   beam1.Depth == beam2.Depth &&
                   beam1.ChosenAsMidBot.Diameter == beam2.ChosenAsMidBot.Diameter &&
                   beam1.ChosenAsMidBot.NumberOfBars == beam2.ChosenAsMidBot.NumberOfBars &&
                   beam1.ChosenCornerAsBot.Diameter == beam2.ChosenCornerAsBot.Diameter &&
                   beam1.ChosenCornerAsBot.NumberOfBars == beam2.ChosenCornerAsBot.NumberOfBars &&
                   beam1.ChosenCornerAsTop.Diameter == beam2.ChosenCornerAsTop.Diameter &&
                   beam1.ChosenCornerAsTop.NumberOfBars == beam2.ChosenCornerAsTop.NumberOfBars &&
                   beam1.ChosenMidAsTop.Diameter == beam2.ChosenMidAsTop.Diameter &&
                   beam1.ChosenMidAsTop.NumberOfBars == beam2.ChosenMidAsTop.NumberOfBars &&
                   beam1.ChosenShearAsCorner.Diameter == beam2.ChosenShearAsCorner.Diameter &&
                   beam1.ChosenShearAsCorner.NumberOfBars == beam2.ChosenShearAsCorner.NumberOfBars &&
                   beam1.ChosenShearAsCorner.spacing == beam2.ChosenShearAsCorner.spacing)
            {
                check = true;
            }

            return check;
        }
    }
}
