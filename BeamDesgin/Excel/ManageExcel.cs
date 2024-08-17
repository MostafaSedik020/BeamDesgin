using BeamDesgin.Elements;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeamDesgin.Excel
{
    public static class ManageExcel
    {
        public static List<Beam> GetBeamsData(string filePath)
        {
            // Load the Excel file
            FileInfo fileInfo = new FileInfo(filePath);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                // Get the  worksheets in the workbook
                ExcelWorksheet workSheetFlex = package.Workbook.Worksheets[0];
                ExcelWorksheet workSheetShear = package.Workbook.Worksheets[1];
                ExcelWorksheet workSheetSecProp = package.Workbook.Worksheets[2];
                ExcelWorksheet workSheetSecDef = package.Workbook.Worksheets[3];

                // Define the starting cell (C4)
                int startRow = 4;
                int column = 3; // Column C is the 3rd column
                List<Beam> beamList = new List<Beam>();

                // Loop through the cells in column C from row 4 to the last used row in the worksheet
                for (int row = startRow; row <= workSheetSecProp.Dimension.End.Row; row++)
                {
                    string cellUniqueName = workSheetSecProp.Cells[row, column].Text;
                    string cellName = workSheetSecProp.Cells[row, column + 3].Text;
                    Beam beam = new Beam();
                    beam.UniqueName = cellUniqueName; //get unique name
                    beam.Name = cellName; // get section name
                    beamList.Add(beam);
                }

                foreach (Beam beam in beamList)
                {
                    for (int row = startRow; row <= workSheetSecDef.Dimension.End.Row; row++)
                    {
                        //get beam Concrete Dimensions
                        string name = workSheetSecDef.Cells[row, 1].Text;
                        if (name == beam.Name)
                        {
                            beam.Breadth = double.Parse(workSheetSecDef.Cells[row, 5].Value.ToString());
                            beam.Depth = double.Parse(workSheetSecDef.Cells[row, 4].Value.ToString());
                            break;
                        }
                    }
                    //get the Flexural RFT values
                    for (int row = startRow; row <= workSheetFlex.Dimension.End.Row; row++)
                    {
                        string name = workSheetFlex.Cells[row, 3].Text;
                        if (name == beam.UniqueName)
                        {
                            if (workSheetFlex.Cells[row, 5].Text.Contains("End"))
                            {
                                //get top As Corner
                                object topCornerAsValue = workSheetFlex.Cells[row, 8].Value;
                                double topCornerAs = topCornerAsValue != null ? double.Parse(topCornerAsValue.ToString()) : 0;
                                beam.TopCornerAs = topCornerAs > beam.TopCornerAs ? topCornerAs : beam.TopCornerAs;

                                //get bot corner
                                object botCornerAsValue = workSheetFlex.Cells[row, 11].Value;
                                double botCornerAs = botCornerAsValue != null ? double.Parse(botCornerAsValue.ToString()) : 0;
                                beam.BotCornerAs = botCornerAs > beam.BotCornerAs ? botCornerAs : beam.BotCornerAs;
                            }
                            else
                            {
                                //get top middle
                                object topMidAsValue = workSheetFlex.Cells[row, 8].Value;
                                beam.TopMiddleAs = topMidAsValue != null ? double.Parse(topMidAsValue.ToString()) : 0;

                                //get bot middle
                                object botMidAsValue = workSheetFlex.Cells[row, 11].Value;
                                beam.BotMiddleAs = botMidAsValue != null ? double.Parse(botMidAsValue.ToString()) : 0;
                            }


                        }
                    }

                    //get the Shear RFT values
                    for (int row = startRow; row <= workSheetShear.Dimension.End.Row; row++)
                    {
                        string name = workSheetFlex.Cells[row, 3].Text;
                        if (name == beam.UniqueName)
                        {
                            if (workSheetShear.Cells[row, 5].Text.Contains("End"))
                            {
                                //get sheat As Corner
                                object shearAsValue = workSheetFlex.Cells[row, 8].Value;
                                double ShearAs = shearAsValue != null ? double.Parse(shearAsValue.ToString()) : 0;
                                beam.CornerShearAs = ShearAs > beam.CornerShearAs ? ShearAs : beam.CornerShearAs;

                            }
                        }
                    }



                        

                }
                return beamList;
            }
            
        }
    }
}
