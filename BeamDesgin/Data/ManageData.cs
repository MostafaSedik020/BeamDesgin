using Autodesk.Revit.UI;
using BeamDesgin.Elements;
using BeamDesgin.ManageElements;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeamDesgin.Data
{
    public static class ManageData
    {
        public static List<Beam> transAreaData(List<Beam> beamList,List<int> bars, string prequal)
        {


            // Calculate bottom Reinforcment
            foreach (Beam beam in beamList)
            {
                int domainDia = ManageRft.GetChosenFlexRFT(beam.BotMiddleAs, beam.Breadth, bars).diameter;
                double breadth = beam.Breadth;

                #region Bottom Reinforcement

                var chosenMidBot = ManageRft.GetChosenFlexRFT(beam.BotMiddleAs, breadth, bars, domainDia);
                beam.ChosenAsMidBot.NumberOfBars = chosenMidBot.noOfBars;
                beam.ChosenAsMidBot.Diameter = chosenMidBot.diameter;

                var chosenCornerBot = ManageRft.GetChosenFlexRFT(beam.BotCornerAs, breadth, bars, domainDia);
                if(chosenCornerBot.noOfBars > chosenMidBot.noOfBars / 2 ||
                    chosenCornerBot.noOfBars < chosenMidBot.noOfBars)
                {
                    beam.ChosenCornerAsBot.NumberOfBars = chosenMidBot.noOfBars;
                    beam.ChosenCornerAsBot.Diameter = chosenMidBot.diameter;
                }
                else
                {
                    beam.ChosenCornerAsBot.NumberOfBars = chosenCornerBot.noOfBars;
                    beam.ChosenCornerAsBot.Diameter = chosenCornerBot.diameter;
                }
                

                
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
            var sortedBeamList = sortData(beamList ,prequal);

            //StringBuilder sb = new StringBuilder();

            //int num = 0;

            //DEMO SHOW THAT THE DATA AR LISTED PROPERLY
            //foreach (var beam in sortedBeamList)
            //{
            //    sb.AppendLine($"{num}+"+ beam.Depth.ToString());
            //    num++;
            //}
            //TaskDialog.Show("test",sb.ToString());

            

            return sortedBeamList;
        }

        public static List<Beam> UniqueSortedData(List<Beam> sortedBeamList)
        {
            // Group the beams by BeamMark and select the first beam from each group
            var uniqueSortedList = sortedBeamList
                .GroupBy(beam => beam.Mark.Number) // Group by BeamMark's Number
                .Select(group => group.First())    // Select the first beam from each group
                .ToList();

            return uniqueSortedList;
        }
        public static List<Beam> UniqueSortedData(ObservableCollection<Beam> sortedBeamList)
        {
            // Group the beams by BeamMark and select the first beam from each group
            var uniqueSortedList = sortedBeamList
                .GroupBy(beam => beam.Mark.Number) // Group by BeamMark's Number
                .Select(group => group.First())    // Select the first beam from each group
                .ToList();

            return uniqueSortedList;
        }


        public static List<Beam> sortData(List<Beam> beamList ,string prequal)
        {
            var sortedList =  beamList.OrderBy(b => b.Breadth)
                                         .ThenBy(b => b.Depth)
                                         .ThenBy(b => b.ChosenAsMidBot.Diameter)
                                         .ThenBy(b => b.ChosenAsMidBot.NumberOfBars)
                                         .ThenBy(b => b.ChosenCornerAsTop.Diameter)
                                         .ThenBy(b => b.ChosenCornerAsTop.NumberOfBars)
                                         .ThenBy(b => b.ChosenMidAsTop.Diameter)
                                         .ThenBy(b => b.ChosenMidAsTop.NumberOfBars)
                                         .ThenBy(b => b.ChosenShearAsCorner.Diameter)
                                         .ThenBy(b => b.ChosenShearAsCorner.spacing)
                                         .ToList();

            string currentMarkType = prequal;
            int markNumber = 1;

            if (sortedList.Any())
            {
                Beam previousBeam = sortedList.First();
                previousBeam.Mark.Type = currentMarkType;
                previousBeam.Mark.Number = markNumber;

                foreach (var beam in sortedList.Skip(1))
                {
                    if (ManageBeams.AreBeamsSimilar(beam, previousBeam))
                    {
                        // Assign the same mark as the previous beam
                        beam.Mark.Number = previousBeam.Mark.Number;
                        beam.Mark.Type = previousBeam.Mark.Type;
                    }
                    else
                    {
                        // Assign a new mark
                        markNumber++;
                        beam.Mark.Number = markNumber;
                        beam.Mark.Type = currentMarkType;
                    }
                    previousBeam = beam;
                }
            }

            //StringBuilder sb = new StringBuilder();

            //foreach ( var beam in sortedList)
            //{
            //    sb.AppendLine(beam.BeamMark);
            //}

            //TaskDialog.Show("data",sb.ToString());

            return sortedList;
        }
        public static List<Beam> sortData(ObservableCollection<Beam> beamList , string prequal)
        {
            var sortedList = beamList.OrderBy(b => b.Breadth)
                                         .ThenBy(b => b.Depth)
                                         .ThenBy(b => b.ChosenAsMidBot.Diameter)
                                         .ThenBy(b => b.ChosenAsMidBot.NumberOfBars)
                                         .ThenBy(b => b.ChosenCornerAsTop.Diameter)
                                         .ThenBy(b => b.ChosenCornerAsTop.NumberOfBars)
                                         .ThenBy(b => b.ChosenMidAsTop.Diameter)
                                         .ThenBy(b => b.ChosenMidAsTop.NumberOfBars)
                                         .ThenBy(b => b.ChosenShearAsCorner.Diameter)
                                         .ThenBy(b => b.ChosenShearAsCorner.spacing)
                                         .ToList();
            string currentMarkType = prequal;
            int markNumber = 1;

            if (sortedList.Any())
            {
                Beam previousBeam = sortedList.First();
                previousBeam.Mark.Type = currentMarkType;
                previousBeam.Mark.Number = markNumber;

                foreach (var beam in sortedList.Skip(1))
                {
                    if (ManageBeams.AreBeamsSimilar(beam, previousBeam))
                    {
                        // Assign the same mark as the previous beam
                        beam.Mark.Number = previousBeam.Mark.Number;
                        beam.Mark.Type = previousBeam.Mark.Type;
                    }
                    else
                    {
                        // Assign a new mark
                        markNumber++;
                        beam.Mark.Number = markNumber;
                        beam.Mark.Type = currentMarkType;
                    }
                    previousBeam = beam;
                }
            }

            //StringBuilder sb = new StringBuilder();

            //foreach (var beam in sortedList)
            //{
            //    sb.AppendLine(beam.BeamMark);
            //}

            //TaskDialog.Show("data", sb.ToString());
            return sortedList;
        }
    }
}
