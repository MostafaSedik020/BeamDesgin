using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeamDesgin.Elements;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows;
using BeamDesgin.Utils;

namespace BeamDesgin.Revit
{
    public static class RevitUtils
    {
        public static void SendDataToRevit(ObservableCollection<Beam> beamsData, Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);

            var beams = collector.OfCategory(BuiltInCategory.OST_StructuralFraming)
                                 .WhereElementIsNotElementType()
                                 .ToElements()
                                 .ToList();

            using (Transaction trn = new Transaction(doc, "Apply Beam RFT"))
            {
                trn.Start();

                foreach (var beam in beamsData)
                {
                    string uName = beam.UniqueName;
                    var chosenBeams = beams.Where(b => b.LookupParameter("ETABS Unique Name").HasValue &&
                                            b.LookupParameter("ETABS Unique Name").AsString() == uName)
                                            .ToList();


                    foreach (var chosenBeam in chosenBeams)
                    {

                        if (chosenBeam == null) { continue; }

                        chosenBeam.LookupParameter("BEAM MARK").Set(beam.BeamMark);
                        chosenBeam.LookupParameter("TOP RFT_LEFT").Set(beam.TOP_RFT_CORNER);
                        chosenBeam.LookupParameter("TOP RFT_RIGHT").Set(beam.TOP_RFT_CORNER);
                        chosenBeam.LookupParameter("TOP RFT_MIDSPAN").Set(beam.TOP_RFT_MID);
                        chosenBeam.LookupParameter("BOTTOM RFT_LEFT").Set(beam.BOTTOM_RFT_CORNER);
                        chosenBeam.LookupParameter("BOTTOM RFT_RIGHT").Set(beam.BOTTOM_RFT_CORNER);
                        chosenBeam.LookupParameter("BOTTOM RFT_MIDSAPN").Set(beam.BOTTOM_RFT_MID);
                        chosenBeam.LookupParameter("LINKS_LEFT").Set(beam.LINKS_CORNER);
                        chosenBeam.LookupParameter("LINKS_RIGHT").Set(beam.LINKS_CORNER);
                        chosenBeam.LookupParameter("LINKS_SPAN").Set(beam.LINKS_MID);

                    }
                }

                trn.Commit();

            }
        }

        public static (StringBuilder notFoundBeam, StringBuilder wrongBeams) CheckRevitModel(ObservableCollection<Beam> beamsData , Document doc)
        {
           
            double revitDepth = 0;
            double revitBreadth = 0;


            FilteredElementCollector collector = new FilteredElementCollector(doc);

            var beams = collector.OfCategory(BuiltInCategory.OST_StructuralFraming)
                                 .WhereElementIsNotElementType()
                                 .ToElements()
                                 .ToList();
            
            StringBuilder notFoundBeam = new StringBuilder("Beams were not found in Revit:\n");
            StringBuilder wrongBeams = new StringBuilder("Beams with wrong dimensions:\n");
            
            foreach ( var beam in beamsData )
            {
                var revitBeam = beams.Where(b => b.Id.ToString() == beam.UniqueName).FirstOrDefault();

                Parameter depthParam = null;
                Parameter breadthParam = null;

                if ( revitBeam != null)
                {
                    ElementId typeId = revitBeam.GetTypeId();
                    ElementType beamType = doc.GetElement(typeId) as ElementType;

                     depthParam = beamType.LookupParameter("D");
                     breadthParam = beamType.LookupParameter("B");
                }
                else
                {
                    notFoundBeam.Append(beam.UniqueName);
                    notFoundBeam.Append(";");
                    continue;
                }

                if (depthParam == null || breadthParam == null)
                {
                    wrongBeams.Append(beam.UniqueName);
                    wrongBeams.Append(";");
                    continue;
                }
                else
                {
                    revitDepth = MathFun.convertUnitsToMiliMeters(depthParam.AsDouble());
                    revitBreadth = MathFun.convertUnitsToMiliMeters(breadthParam.AsDouble());
                }
                if (beam.Depth != revitDepth || beam.Breadth != revitBreadth)
                {
                    wrongBeams.Append(beam.UniqueName);
                    wrongBeams.Append(";");
                    
                }

            }

            // Return both StringBuilders as a tuple
            return (notFoundBeam, wrongBeams);
        }
    
    }
}
