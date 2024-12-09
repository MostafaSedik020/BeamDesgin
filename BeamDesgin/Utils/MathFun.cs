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
        public static double convertUnitsToMiliMeters(double value)
        {
            double convertedNum = UnitUtils.ConvertFromInternalUnits((double)value, UnitTypeId.Millimeters);//REVIT2021++
            //double convertedNum = UnitUtils.ConvertFromInternalUnits((double)value, DisplayUnitType.DUT_METERS);
            return convertedNum;
        }
        public static double GetSlopeAngle(double point1_x, double point1_y,double point2_x, double point2_y)
        {
            // Calculate the difference in coordinates
            double deltaX = point2_x - point1_x;
            double deltaY = point2_y - point1_y;

            // Use Math.Atan2 to calculate the angle in radians
            double slopeAngle = Math.Atan2(Math.Abs(deltaY), Math.Abs(deltaX)) * (180 / Math.PI);

            return slopeAngle; // Guaranteed to be between 0 and 90
        }
        public static double NormalizeAngleTo90(double angle)
        {
            // Reduce the angle modulo 360 to get its equivalent within [0, 360]
            double modAngle = angle % 360;
            if (modAngle < 0)
            {
                modAngle += 360; // Ensure positive angles
            }

            // Map the angle to [0, 90] using symmetry
            return modAngle % 90;
        }
    }
}
