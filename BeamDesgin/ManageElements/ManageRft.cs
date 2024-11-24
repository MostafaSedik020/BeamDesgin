using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeamDesgin.Elements;
using BeamDesgin.Utils;
using OfficeOpenXml.Table.PivotTable;

namespace BeamDesgin.ManageElements
{
    public static class ManageRft
    {
        public static List<double> GetAreaRFT(List<int> bars)
        {
            List<double> areaBars = new List<double>();
            foreach (var bar in bars)
            {
                double area = (Math.PI * Math.Pow(bar, 2)) / 4;
                areaBars.Add(area);
            }
            return areaBars;
        }

        public static double GetAreaRFT (Reinforcement reinforcement)
        {
            var shearRft = reinforcement as ShearRFT;
            double singleArea = 0;
            double totalArea = 0;
            

            if (shearRft != null)
            {
                singleArea = (Math.PI * Math.Pow(shearRft.Diameter, 2)) / 4;
                 totalArea = singleArea * shearRft.NumberOfBars * (1000/shearRft.spacing);

            }
            else
            {
                singleArea = (Math.PI * Math.Pow(reinforcement.Diameter, 2)) / 4;
                 totalArea = singleArea * reinforcement.NumberOfBars;
            }
            
            

            return totalArea;
        }
        public static (int diameter, double noOfBars) GetChosenFlexRFT(double As, double breadth, List<int> bars, int? forcedBarDiameter = null)
        {
            List<double> areaBars = GetAreaRFT(bars);
            double chosenAs = 0;
            double chosenAsBars = 0;
            int chosenAsDia = 0;
            double barsNumber = 0;
            int diameter = 0;

            for (int i = 0; i < areaBars.Count; i++)
            {

                if (bars[i] < 12)
                {
                    continue; // any bar less than 12 shall not be included
                }
                // If a forced bar diameter is provided and the current bar is not the forced one, skip it
                if (forcedBarDiameter.HasValue && bars[i] != forcedBarDiameter.Value)
                {
                    continue;
                }


                double numberOfBarsDec = As / areaBars[i];
                double numberOfBars = Math.Ceiling(numberOfBarsDec);
                if(numberOfBars == 0)
                {
                    numberOfBars = 2;
                }

                //get max number of bars in a beam
                double maxNumberEqu = ((breadth - 40 - 20) / (bars[i] + 30));
                double maxNumberofBarsInRow = Math.Floor(maxNumberEqu);
                double maxNumberofBars = 3 * maxNumberofBarsInRow;

                //get the min number of bars in a beam
                double minNumberOfBars = Math.Floor(breadth / 100);

                if (numberOfBars < minNumberOfBars)
                {
                    barsNumber = minNumberOfBars; //check if the bars numbers are lower than min 
                    diameter = bars[i];
                }
                else if (numberOfBars < maxNumberofBars)
                {
                    barsNumber = numberOfBars;    //check if the bars numbers are higher than max
                    diameter = bars[i];
                }

                double totalAs = barsNumber * areaBars[i];

                if (chosenAs == 0 || totalAs < chosenAs)
                {
                    chosenAs = totalAs;            // get the chosen are steel
                    chosenAsBars = barsNumber;
                    chosenAsDia = diameter;

                }
            }
            return (chosenAsDia, chosenAsBars);
        }

        public static (int diameter, double noOfBrnaches, double spacing) GetShearRFT(double As, double breadth, List<int> bars, int? forcedBarDiameter = null)
        {
            List<int> barsNewSort = bars.OrderBy(b => b).ToList();
            List<double> areaBars = GetAreaRFT(barsNewSort);

            double chosenAs = 0;
            double chosenSpacing = 0;
            int chosenAsDia = 0;
            double spacing = 0;
            int diameter = 0;
            double totalAs = 0;

            //get the  number of branches in a beam
            double numberOfBranches = MathFun.RoundDownToEven(breadth / 100);

            if (numberOfBranches == 0)
            {
                numberOfBranches = 2;
            }

            for (int i = 0; i < areaBars.Count; i++)
            {
                if (forcedBarDiameter.HasValue && barsNewSort[i] != forcedBarDiameter.Value)
                {
                    continue;
                }


                for (double j = 150; j >= 100; j = j - 25)
                {
                    double newAs = numberOfBranches * areaBars[i] * (1000 / j);

                    if (newAs > As)
                    {
                        spacing = j;
                        diameter = barsNewSort[i];
                        totalAs = newAs;
                        break;
                    }

                }
                if (totalAs > 0)
                {
                    break;  // Exit the loop if we have found a suitable bar
                }

            }

            chosenAs = totalAs;
            chosenAsDia = diameter;
            chosenSpacing = spacing;

            return (chosenAsDia, numberOfBranches, chosenSpacing);
        }
        //public static (double noOfBars, int diameter) ChangeRftDiameter(Reinforcement rft , int newDia, double breadth)
        //{
        //    List<int> areaBars = new List<int>();
        //    areaBars.Add(newDia);

        //    double totalAs =  GetAreaRFT(rft );

        //    var newRft = GetChosenFlexRFT(totalAs, breadth, areaBars);

        //    return (newRft.noOfBars, newRft.diameter);

        //}
        public static (double noOfBars, double diameter) ChangeRftDiameter(Reinforcement rft, int newDia, double breadth)
        {
            List<int> areaBars = new List<int>();
            areaBars.Add(newDia);

            double totalAs = GetAreaRFT(rft);
            double singleArea = (Math.PI * Math.Pow(newDia, 2)) / 4;
            double newNoOfBars = Math.Ceiling(totalAs/singleArea);

            return (newNoOfBars, newDia);

        }


    }
}
