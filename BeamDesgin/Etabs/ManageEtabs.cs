using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using BeamDesgin.Elements;
using BeamDesgin.Utils;
using ETABSv1;

namespace BeamDesgin.Etabs
{
    public static class ManageEtabs
    {
        private static cSapModel mySapModel = default;
        public static void LinkEtabsModel()
        {
            //set the following flag to true to attach to an existing instance of the program 
            //otherwise a new instance of the program will be started 
            bool AttachToInstance;
            AttachToInstance = true;

            //set the following flag to true to manually specify the path to ETABS.exe
            //this allows for a connection to a version of ETABS other than the latest installation
            //otherwise the latest installed version of ETABS will be launched
            bool SpecifyPath;
            SpecifyPath = false;

            //dimension the ETABS Object as cOAPI type
            cOAPI myETABSObject = null;

            //Use ret to check if functions return successfully (ret = 0) or fail (ret = nonzero) 
            int ret = 0;

            //create API helper object
            cHelper myHelper = new Helper();
             
            

            //attach to a running instance of ETABS 
            try
            {
                //get the active ETABS object
                myETABSObject = myHelper.GetObject("CSI.ETABS.API.ETABSObject");
            }
            catch (Exception ex)
            {
                MessageBox.Show("No running instance of the program found or failed to attach.");
                
            }
            //Get a reference to cSapModel to access all API classes and functions
            //cSapModel mySapModel = default;
            mySapModel = myETABSObject.SapModel;
            
            //Run analysis
            
            //ret = mySapModel.Analyze.RunAnalysis();
            //ret = mySapModel.SetModelIsLocked(false);



            //Check ret value 
            if (ret == 0)
            {
                MessageBox.Show("API script completed successfully.");
            }
            else
            {
                MessageBox.Show("API script FAILED to complete.");
            }

        }
        public static string[] GetGroupNames()
        {
            int ret = 0;
            int NumberNames = 0;
            string[] GroupNames = null;

            ret = mySapModel.GroupDef.GetNameList(ref NumberNames, ref GroupNames);

            return GroupNames;
        }
        public static List<Beam>  GetDataFromEtabs(string groupName,string desginCode)
        {
            List<Beam> beamList = new List<Beam>();
            List<Beam> columnList = new List<Beam>();
            int ret = 0;
            ret = mySapModel.SetModelIsLocked(false);

            // get all frames
            int numberNames = 0;
            string[] allFramesUniName = null;
            string[] allFramePropName = null;
            string[] storyName = null;
            string[] pointName1 = null;
            string[] pointName2 = null;
            double[] point1X = null;
            double[] point1Y = null;
            double[] point1Z = null;
            double[] point2X = null;
            double[] point2Y = null;
            double[] point2Z = null;
            double[] angle = null;
            double[] offset1X = null;
            double[] offset2X = null;
            double[] offset1Y = null;
            double[] offset2Y = null;
            double[] offset1Z = null;
            double[] offset2Z = null;
            int[] cardinalPoint = null;

            //get the section name
            int myType = 0;
            string PropName = null;
            string SAuto = null;

            //get section dimensions
            string FileName = null;
            string MatProp = null;
            double T3 = 0;
            double T2 = 0;
            int Color = 0;
            string Notes = null;
            string GUID = null;

            

            //get all frames properties
            ret = mySapModel.FrameObj.GetAllFrames(
            ref numberNames, ref allFramesUniName, ref allFramePropName, ref storyName, ref pointName1, ref pointName2,
            ref point1X, ref point1Y, ref point1Z, ref point2X, ref point2Y, ref point2Z,
            ref angle, ref offset1X, ref offset2X, ref offset1Y, ref offset2Y, ref offset1Z, ref offset2Z, ref cardinalPoint
            );

            for (int i = 0; i < numberNames; i++)
            {

                ret = mySapModel.PropFrame.GetTypeRebar(allFramePropName[i], ref myType);// check if the type beam or column
                ret = mySapModel.PropFrame.GetRectangle(allFramePropName[i], ref FileName, ref MatProp, ref T3, ref T2, ref Color, ref Notes, ref GUID);// get the beam concrete dimensions
                if (myType == 1) //used to filter beam elements only
                {

                    Beam column = new Beam
                    {
                        UniqueName = allFramesUniName[i],
                        Name = allFramePropName[i],
                        Breadth = T2 * 1000,
                        Depth = T3 * 1000,
                        EtabsStory = storyName[i],
                        point1X = point1X[i],
                        point1Y = point1Y[i],
                        point1Z = point1Z[i],
                        point2X = point2X[i],
                        point2Y = point2Y[i],
                        point2Z = point2Z[i],
                        Angle = angle[i],

                    };

                    columnList.Add(column);
                }

            }

            //get the frame story
            string label = null;
            string story = null;

            //get frame element in the group
            int NumberItems = 0;
            int[] ObjectType = null;
            string[] ObjectName = null; //unique name
            int index = 0;

            ret = mySapModel.GroupDef.GetAssignments(groupName,ref NumberItems,ref ObjectType,ref ObjectName);

            

            //assign the unique name of the beam
            for (int i = 0; i < NumberItems; i++)
            {
                ret = mySapModel.FrameObj.GetSection(ObjectName[i],ref PropName,ref SAuto); //return section name e.g:B300X700
                ret = mySapModel.PropFrame.GetTypeRebar(PropName, ref myType);// check if the type beam or column
                ret = mySapModel.PropFrame.GetRectangle(PropName,ref FileName, ref MatProp,ref T3,ref T2,ref Color,ref Notes,ref GUID);// get the beam concrete dimensions
                ret = mySapModel.FrameObj.GetLabelFromName(ObjectName[i],ref label,ref story);
                index = Array.IndexOf(allFramesUniName,ObjectName[i]);

                if (myType == 2) //used to filter beam elements only
                {

                    
                    Beam beam = new Beam
                    {
                        UniqueName = ObjectName[i],
                        Name = PropName,
                        Breadth = T2*1000,
                        Depth = T3*1000,
                        EtabsStory = story,
                        point1X = point1X[index],
                        point1Y = point1Y[index],
                        point1Z = point1Z[index],
                        point2X = point2X[index],
                        point2Y = point2Y[index],
                        point2Z = point2Z[index],
                        Angle = MathFun.NormalizeAngleTo90( angle[index]),

                    };
                    
                    beamList.Add(beam);
                }
                
            }

            bool AutoOffset = true;
            double Length1 = 0;
            double Length2 = 0;
            double RZ = 0;

            string beamPoint1 = null;
            string beamPoint2 = null;

            string columnPoint1 = null;
            string columnPoint2 = null;
            
            Beam selectedColumn1 = null;
            Beam selectedColumn2 = null;
            if (desginCode == "ACI")
            {
                int count = 0;
                foreach (var beam in beamList)
                {
                    selectedColumn1 = null;
                    selectedColumn2 = null;
                    //get the supports
                    var columnsAtSameStory = columnList.Where(x=>x.EtabsStory == beam.EtabsStory).ToList();

                    //get beam points
                    ret = mySapModel.FrameObj.GetPoints(beam.UniqueName, ref beamPoint1, ref beamPoint2);

                    //get column points to determine whicj is selected column
                    count = 0;
                    foreach (var column in columnsAtSameStory)
                    {
                        ret = mySapModel.FrameObj.GetPoints(column.UniqueName, ref columnPoint1, ref columnPoint2);
                        if((columnPoint1 == beamPoint1 || columnPoint1 == beamPoint2 ||
                            columnPoint2 == beamPoint1 || columnPoint2 == beamPoint2) && count <2)
                        {
                            if (count == 0)
                                selectedColumn1 = column;
                            else
                                selectedColumn2 = column;
                            count++;
                        }
                    }
                    double beamAngle = MathFun.GetSlopeAngle(beam.point1X,beam.point1Y, beam.point2X, beam.point2Y);
                    double addi = 0;
                    double addj = 0;

                    if(selectedColumn1 != null)
                    {
                        double anglei = Math.Abs(selectedColumn1.Angle - beamAngle);
                        addi = GetOffsetvalue(selectedColumn1.Breadth, selectedColumn1.Depth, anglei);
                    }
                    if(selectedColumn2 != null)
                    {
                        double anglej = Math.Abs(selectedColumn2.Angle - beamAngle);
                        addj = GetOffsetvalue(selectedColumn2.Breadth, selectedColumn2.Depth, anglej);
                    }
                    ret = mySapModel.FrameObj.SetEndLengthOffset(beam.UniqueName, false, 0, 0, 0);
                    double offsetValue1 = (beam.Depth-50 + addi) / 1000;
                    double offsetValue2 = (beam.Depth - 50 + addj) / 1000;
                    ret = mySapModel.FrameObj.SetEndLengthOffset(beam.UniqueName, false, offsetValue1, offsetValue2, 0);
                }
            }
            else
            {
                foreach (var beam in beamList)
                {
                    ret = mySapModel.FrameObj.GetEndLengthOffset(beam.UniqueName, ref AutoOffset, ref Length1, ref Length2, ref RZ);
                    string beamName = beam.UniqueName;
                    double offsetValue1 = ((beam.Depth/2 - 50) / 1000) + Length1;
                    double offsetValue2 = ((beam.Depth/2 - 50) / 1000) + Length2;
                    ret = mySapModel.FrameObj.SetEndLengthOffset(beamName, false, offsetValue1, offsetValue2, 0);
                }
            }

            //run the analysis and desgin
            //ret = mySapModel.Analyze.RunAnalysis();
            //ret = mySapModel.DesignConcrete.StartDesign();

            ////beams parameters
            //NumberItems = 0;
            //string[] FrameName = null;
            //double[] Location = null;
            ////flexural
            //string[] TopCombo = null;
            //double[] TopArea = null;
            //string[] BotCombo = null;
            //double[] BotArea = null;
            ////shear
            //string[] VMajorCombo = null;
            //double[] VMajorArea = null;
            ////torsion
            //string[] TLCombo = null;
            //double[] TLArea = null;
            //string[] TTCombo = null;
            //double[] TTArea = null;
            //string[] ErrorSummary = null;
            //string[] WarningSummary = null;

            ////get shear RFT
            //ret = mySapModel.DesignConcrete.GetSummaryResultsBeam(groupName,ref NumberItems,ref FrameName,ref Location,
            //    ref TopCombo,ref TopArea,ref BotCombo,ref BotArea,ref VMajorCombo,ref VMajorArea,ref TLCombo,ref TLArea,ref TTCombo, ref TTArea
            //    ,ref ErrorSummary,ref WarningSummary,eItemType.Group);

            //foreach (var beam in beamList)
            //{
            //    var indices = FrameName
            //                    .Select((value, index) => new { value, index })
            //                    .Where(x => x.value == beam.UniqueName)
            //                    .Select(x => x.index)
            //                    .ToArray();
            //}

            //unlock model

            //remove offset

            //runagain and desgin

            //get flexural RFT



            return beamList;
        }

        public static double GetOffsetvalue(double columnWidth, double columnDepth,double angle)
        {
            double offsetValue = 0;
            double hyp = Math.Sqrt(Math.Pow(columnDepth, 2) + Math.Pow(columnWidth, 2));
            double radianAngle = angle * (Math.PI / 180);

            if (angle > 45)
            {
                offsetValue = ((4*hyp-2*columnDepth)/Math.PI)*radianAngle+columnDepth/2;
            }
            else
            {
                offsetValue = ((2*columnWidth-4*hyp)/Math.PI)*radianAngle-((columnWidth-2*hyp)/2)+hyp;
            }

            return offsetValue;
        }
    }
}
