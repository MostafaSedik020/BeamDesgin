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

        public static (StringBuilder totalWarnings, int count) CheckRevitModel(ObservableCollection<Beam> beamsData, Document doc)
        {
            int count = 0;

            // Retrieve all beams from Revit
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            var revitBeams = collector.OfCategory(BuiltInCategory.OST_StructuralFraming)
                                       .WhereElementIsNotElementType()
                                       .ToList();

            // Initialize warning builders
            StringBuilder notFoundBeam = new StringBuilder("Beams found in ETABS but missing in Revit:\n");
            StringBuilder wrongBeams = new StringBuilder("Beams with wrong dimensions:\n");
            StringBuilder totalWarnings = new StringBuilder();

            // Iterate through each ETABS beam
            foreach (var beam in beamsData)
            {
                // Find Revit beams with matching ID
                var selectedRevitBeams = revitBeams.Where(b => b.Id.ToString() == beam.UniqueName).ToList();

                if (!selectedRevitBeams.Any())
                {
                    // Beam missing in Revit
                    notFoundBeam.AppendLine($"{beam.UniqueName}");
                    count++;
                    continue;
                }

                // Check dimensions for each selected Revit beam
                bool isDimensionMismatch = false;

                foreach (var rBeam in selectedRevitBeams)
                {
                    ElementType beamType = doc.GetElement(rBeam.GetTypeId()) as ElementType;

                    // Retrieve depth (D) and breadth (B) parameters
                    Parameter depthParam = beamType?.LookupParameter("D");
                    Parameter breadthParam = beamType?.LookupParameter("B");

                    if (depthParam == null || breadthParam == null)
                    {
                        isDimensionMismatch = true;
                        break;
                    }

                    // Convert dimensions to millimeters
                    double revitDepth = MathFun.convertUnitsToMiliMeters(depthParam.AsDouble());
                    double revitBreadth = MathFun.convertUnitsToMiliMeters(breadthParam.AsDouble());

                    // Compare with ETABS beam dimensions
                    if (beam.Depth != revitDepth || beam.Breadth != revitBreadth)
                    {
                        isDimensionMismatch = true;
                        break;
                    }
                }

                if (isDimensionMismatch)
                {
                    wrongBeams.AppendLine($"{beam.UniqueName}");
                    count++;
                }
            }

            // Consolidate warnings
            totalWarnings.AppendLine(notFoundBeam.ToString());
            totalWarnings.AppendLine(wrongBeams.ToString());

            // Return warnings and count
            return (totalWarnings, count);
        }


    }
}
