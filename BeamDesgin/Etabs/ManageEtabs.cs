using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                MessageBox.Show("ETABS Linked successfully.");
            }
            else
            {
                MessageBox.Show("ETABS Linked Failed.");
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
                if (myType == 1) //used to filter column elements only
                {
                    //if (ObjectName[i] == "115")
                    //{
                    //    MessageBox.Show("hey");
                    //}
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
                        Angle = MathFun.NormalizeAngleTo90( angle[i]),

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
                        
                        if(columnPoint1 == beamPoint1 || columnPoint2 == beamPoint1)
                        {
                            selectedColumn1 = column;
                        }
                        if (columnPoint1 == beamPoint2 || columnPoint2 == beamPoint2) 
                        {
                            selectedColumn2 = column;
                        }
                    }
                    //if (beam.UniqueName == "1868")
                    //{
                    //    MessageBox.Show("hey");
                    //}
                    double beamAngle = MathFun.GetSlopeAngle(beam.point1X,beam.point1Y, beam.point2X, beam.point2Y);
                    double beamLength = MathFun.GetLineLength(beam.point1X, beam.point1Y, beam.point2X, beam.point2Y);
                    double addi = 0;
                    double addj = 0;
                    double offsetValue1 = 0;
                    double offsetValue2 = 0;
                double beamPart = beam.Depth / 2;

                if (desginCode == "ACI")
                {
                     beamPart = beam.Depth;
                }
                


                    //check of there is no columns support beam then its auto selected by ETABS and skip this beam
                    if (selectedColumn1 == null && selectedColumn2 == null)
                    {
                        ret = mySapModel.FrameObj.SetEndLengthOffset(beam.UniqueName, true, 0, 0, 0);
                        continue;
                    }

                    if(selectedColumn1 != null)
                    {
                        double anglei = Math.Abs(Math.Abs(selectedColumn1.Angle) - Math.Abs(beamAngle));
                        addi = GetOffsetvalue(selectedColumn1.Breadth, selectedColumn1.Depth, anglei);
                        offsetValue1 = (beamPart - 50 + addi) / 1000;
                        if (offsetValue1 > beamLength/2)
                        {
                            offsetValue1 = beamLength/2;
                        }
                    }
                    
                    if(selectedColumn2 != null)
                    {
                        double anglej = Math.Abs(Math.Abs(selectedColumn2.Angle) - Math.Abs(beamAngle));
                        addj = GetOffsetvalue(selectedColumn2.Breadth, selectedColumn2.Depth, anglej);
                        offsetValue2 = (beamPart - 50 + addj) / 1000;
                        if (offsetValue2 > beamLength / 2)
                        {
                            offsetValue2 = beamLength / 2;
                        }
                    }

                    
                    ret = mySapModel.FrameObj.SetEndLengthOffset(beam.UniqueName, false, offsetValue1, offsetValue2, 0);
                }





            //run the analysis and desgin
            ret = mySapModel.Analyze.RunAnalysis();
            ret = mySapModel.DesignConcrete.StartDesign();

            //beams parameters
            NumberItems = 0;
            string[] FrameName = null;
            double[] Location = null;
            //flexural
            string[] TopCombo = null;
            double[] TopArea = null;
            string[] BotCombo = null;
            double[] BotArea = null;
            //shear
            string[] VMajorCombo = null;
            double[] VMajorArea = null;
            //torsion
            string[] TLCombo = null;
            double[] TLArea = null;
            string[] TTCombo = null;
            double[] TTArea = null;
            string[] ErrorSummary = null;
            string[] WarningSummary = null;

            //get shear RFT
            ret = mySapModel.DesignConcrete.GetSummaryResultsBeam(groupName, ref NumberItems, ref FrameName, ref Location,
                ref TopCombo, ref TopArea, ref BotCombo, ref BotArea, ref VMajorCombo, ref VMajorArea, ref TLCombo, ref TLArea, ref TTCombo, ref TTArea
                , ref ErrorSummary, ref WarningSummary, eItemType.Group);

            foreach (var beam in beamList)
            {
                // Get indices matching the beam's unique name
                var indices = FrameName
                    .Select((value, idx) => new { value, idx })
                    .Where(x => x.value == beam.UniqueName)
                    .Select(x => x.idx)
                    .ToArray();
                // Find the maximum VMajorArea value for these indices
                if (indices.Any()) // Ensure there are matching indices
                {
                    beam.CornerShearAs = indices.Select(idx => VMajorArea[idx]).Max();

                    if (beam.CornerShearAs < 0)
                    {
                        beam.CornerShearAs = 0;
                    }
                }
                else
                {
                    beam.CornerShearAs = 0;
                }


            }

            //unlock model
            ret = mySapModel.SetModelIsLocked(false);
            //remove offset
            foreach (var beam in beamList)
            {
                ret = mySapModel.FrameObj.SetEndLengthOffset(beam.UniqueName, true, 0, 0, 0);
            }
            //runagain and desgin
            ret = mySapModel.Analyze.RunAnalysis();
            ret = mySapModel.DesignConcrete.StartDesign();

            //get flexural RFT
            ret = mySapModel.DesignConcrete.GetSummaryResultsBeam(groupName, ref NumberItems, ref FrameName, ref Location,
                ref TopCombo, ref TopArea, ref BotCombo, ref BotArea, ref VMajorCombo, ref VMajorArea, ref TLCombo, ref TLArea, ref TTCombo, ref TTArea
                , ref ErrorSummary, ref WarningSummary, eItemType.Group);

            double factor = 1000000;

            foreach (var beam in beamList)
            {
                // Get indices matching the beam's unique name
                var indices = FrameName
                    .Select((value, idx) => new { value, idx })
                    .Where(x => x.value == beam.UniqueName)
                    .Select(x => x.idx)
                    .ToArray();
                // Find the maximum Top Area value for these indices
                if (indices.Any()) // Ensure there are matching indices
                {

                    beam.TopCornerAs = Math.Max( indices.Select(idx => TopArea[idx]).First(), indices.Select(idx => TopArea[idx]).Last())*factor;

                    if (beam.TopCornerAs < 0)
                    {
                        beam.TopCornerAs = 0;
                    }
                    //get bot corner
                    beam.BotCornerAs = Math.Max(indices.Select(idx => BotArea[idx]).First(), indices.Select(idx => BotArea[idx]).Last())*factor;
                    if (beam.BotCornerAs < 0)
                    {
                        beam.BotCornerAs = 0;
                    }

                    //get mid location
                    var chosenLocation = indices.Select(idx => Location[idx]).ToArray();
                    
                    double mean = chosenLocation.Average();
                    double closestNumber = chosenLocation.OrderBy(num => Math.Abs(num - mean)).First();

                    int midIndex = Array.IndexOf(chosenLocation, closestNumber);

                    var chosenAsBot = indices.Select(idx => BotArea[idx]).ToArray(); // get as bot for mid of the beam
                    var chosenAsTop = indices.Select(idx => TopArea[idx]).ToArray(); // get as top for mid of the beam

                    //get bot middle

                    beam.BotMiddleAs = chosenAsBot[midIndex]*factor;

                    //get top middle

                    beam.TopMiddleAs = chosenAsTop[midIndex] * factor;

                    if (beam.TopMiddleAs < beam.BotMiddleAs * 0.2)
                    {
                        beam.TopMiddleAs = beam.BotMiddleAs * 0.2;
                    }
                }
                else
                {
                    beam.TopCornerAs = 0;
                    beam.BotCornerAs = 0;
                    beam.BotMiddleAs = 0;
                    beam.TopMiddleAs = 0;
                }


            }


            return beamList;
        }

        public static double GetOffsetvalue(double columnWidth, double columnDepth,double angle)
        {
            double offsetValue = 0;
            double hyp = Math.Sqrt(Math.Pow(columnDepth/2, 2) + Math.Pow(columnWidth/2, 2));
            double radianAngle = angle * (Math.PI / 180);
            double midAngle = Math.Atan((columnWidth/2)/(columnDepth/2));
            double midAngleInDegree = (midAngle*180)/Math.PI;
            double a = 0; double b = 0;
            double c = 0;

            if (angle == 90)
            {
                offsetValue = columnWidth/2;
            }
            else if (angle < 90 && angle > midAngleInDegree)
            {
                c = Math.Tan(radianAngle);
                a = columnWidth / (2 * Math.Tan(radianAngle));
                b = columnWidth /2;
                offsetValue = Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
            }
            else if (angle < midAngleInDegree && angle > 0)
            {
                c = Math.Tan(radianAngle);
                a = (columnDepth * Math.Tan(radianAngle)/2);
                b = columnDepth/2;
                offsetValue = Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
            }
            else if(angle == midAngleInDegree)
            {
                offsetValue = hyp;
            }
            else if ( angle == 0)
            {
                offsetValue = columnDepth/2;
            }
            else
            {
                offsetValue = columnDepth / 2;
            }
            return offsetValue;
        }
    }
}
