using BeamDesgin.Elements;
using BeamDesgin.ManageElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeamDesgin.Data
{
    public static class ManageData
    {
        public static List<Beam> SortData(List<Beam> beamList,List<int> bars)
        {


            // Calculate bottom Reinforcment
            foreach (Beam beam in beamList)
            {

                #region Bottom Reinforcement
                int domainDia = ManageRft.GetChosenFlexRFT(beam.BotMiddleAs, beam.Breadth, bars).diameter;
                double breadth = beam.Breadth;

                var chosenCornerBot = ManageRft.GetChosenFlexRFT(beam.BotCornerAs, breadth, bars, domainDia);
                beam.ChosenCornerAsBot.NumberOfBars = chosenCornerBot.noOfBars;
                beam.ChosenCornerAsBot.Diameter = chosenCornerBot.diameter;

                double steelDiff = Math.Abs(beam.BotMiddleAs - beam.BotCornerAs);
                var chosenMidBot = ManageRft.GetChosenFlexRFT(steelDiff, breadth, bars, domainDia);
                beam.ChosenAsMidBot.NumberOfBars = chosenMidBot.noOfBars;
                beam.ChosenAsMidBot.Diameter = chosenMidBot.diameter;
                #endregion

                #region Top Reinforcement
                var chosenCornerTop = ManageRft.GetChosenFlexRFT(beam.TopCornerAs, breadth, bars);
                beam.ChosenCornerAsTop.NumberOfBars = chosenCornerTop.noOfBars;
                beam.ChosenCornerAsTop.Diameter = chosenCornerTop.diameter;

                var chosenMidTop = ManageRft.GetChosenFlexRFT(beam.TopMiddleAs, breadth, bars);
                beam.ChosenMidAsTop.NumberOfBars = chosenMidTop.noOfBars;
                beam.ChosenMidAsTop.Diameter = chosenMidTop.diameter;
                #endregion

                #region Shear Reinforcment
                int domainDiaShear = ManageRft.GetShearRFT(beam.CornerShearAs, breadth, bars).diameter;

                var chosenCornerShear = ManageRft.GetShearRFT(beam.CornerShearAs, breadth, bars, domainDiaShear);
                beam.ChosenShearAsCorner.Diameter = chosenCornerShear.diameter;
                beam.ChosenShearAsCorner.NumberOfBars = chosenCornerShear.noOfBrnaches;
                beam.ChosenShearAsCorner.spacing = chosenCornerShear.spacing;

                beam.ChosenShearAsMid.Diameter = chosenCornerShear.diameter;
                beam.ChosenShearAsMid.NumberOfBars = chosenCornerShear.noOfBrnaches;
                beam.ChosenShearAsMid.spacing = 200;
                #endregion

            }
            //sorting
            var sortedBeamList = beamList.OrderBy(b => b.Breadth)
                                         .ThenBy(b => b.Depth)
                                         .ThenBy(b => b.ChosenAsMidBot.Diameter)
                                         .ThenBy(b => b.ChosenAsMidBot.NumberOfBars + b.ChosenCornerAsBot.NumberOfBars)
                                         .ThenBy(b => b.ChosenCornerAsTop.Diameter)
                                         .ThenBy(b => b.ChosenCornerAsTop.NumberOfBars)
                                         .ThenBy(b => b.ChosenMidAsTop.Diameter)
                                         .ThenBy(b => b.ChosenMidAsTop.NumberOfBars)
                                         .ThenBy(b => b.ChosenShearAsCorner.Diameter)
                                         .ThenBy(b => b.ChosenShearAsCorner.spacing)
                                         .ToList();


            string currentMark = "B1";
            int markNumber = 1;

            if (sortedBeamList.Any())
            {
                Beam previousBeam = sortedBeamList.First();
                previousBeam.BeamMark = currentMark;

                foreach (var beam in sortedBeamList.Skip(1))
                {
                    if (ManageBeams.AreBeamsSimilar(beam, previousBeam))
                    {
                        beam.BeamMark = previousBeam.BeamMark;
                    }
                    else
                    {
                        // Assign a new mark
                        currentMark = $"B{markNumber + 1}";
                        beam.BeamMark = currentMark;
                        markNumber++;
                    }
                    previousBeam = beam;
                }
            }

            return sortedBeamList;
        }
    }
}
