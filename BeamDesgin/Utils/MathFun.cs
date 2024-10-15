using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeamDesgin.Utils
{
    public static class MathFun
    {
        public static double RoundUpToEven(double number)
        {
            int rounded = (int)Math.Ceiling(number);
            return (rounded % 2 == 0) ? rounded : rounded + 1;
        }
        public static double RoundDownToEven(double number)
        {
            int rounded = (int)Math.Floor(number);
            // If the number is already even, return it; otherwise, subtract 1 to make it even.
            return (rounded % 2 == 0) ? rounded : rounded - 1;
        }
        public static double convertUnitsToMeters(double value)
        {
            double convertedNum = UnitUtils.ConvertFromInternalUnits((double)value, UnitTypeId.Meters);//REVIT2021++
            //double convertedNum = UnitUtils.ConvertFromInternalUnits((double)value, DisplayUnitType.DUT_METERS);
            return convertedNum;
        }
    }
}
