using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeamDesgin.Elements;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Controls;

namespace BeamDesgin.Revit
{
    public static class RevitUtils
    {
        public static void SendDataToRevit(List<Beam> beamsData , Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);

            var beams = collector.OfCategory(BuiltInCategory.OST_StructuralFraming)
                                 .WhereElementIsNotElementType()
                                 .ToElements()
                                 .ToList();

            foreach ( var beam in beamsData )
            {
                var chosenBeams  = beams.Where(b => b.LookupParameter("ETABS Unique Name").AsValueString() == beam.UniqueName )
                                        .ToList();
                
                foreach (var chosenBeam in chosenBeams)
                {
                    if (chosenBeam == null) {  continue; }
                    using (Transaction trn = new Transaction(doc, "Apply Beam RFT"))
                    {
                        trn.Start();

                        chosenBeam.LookupParameter("BEAM MARK").SetValueString(beam.BeamMark);
                        chosenBeam.LookupParameter("TOP RFT_LEFT").SetValueString(beam.TOP_RFT_CORNER);
                        chosenBeam.LookupParameter("TOP RFT_RIGHT").SetValueString(beam.TOP_RFT_CORNER);
                        chosenBeam.LookupParameter("TOP RFT_MIDSPAN").SetValueString(beam.TOP_RFT_MID);
                        chosenBeam.LookupParameter("BOTTOM RFT_LEFT").SetValueString(beam.BOTTOM_RFT_CORNER);
                        chosenBeam.LookupParameter("BOTTOM RFT_RIGHT").SetValueString(beam.BOTTOM_RFT_CORNER);
                        chosenBeam.LookupParameter("BOTTOM RFT_MIDSAPN").SetValueString(beam.BOTTOM_RFT_MID);

                        trn.Commit();
                    }
                        
                }
            }
                


        }
    }
}
