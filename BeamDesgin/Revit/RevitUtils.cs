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

namespace BeamDesgin.Revit
{
    public static class RevitUtils
    {
        public static void SendDataToRevit(ObservableCollection<Beam> beamsData , Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);

            var beams = collector.OfCategory(BuiltInCategory.OST_StructuralFraming)
                                 .WhereElementIsNotElementType()
                                 .ToElements()
                                 .ToList();
            
            
            
            foreach ( var beam in beamsData )
            {
                string uName = beam.UniqueName;
                var chosenBeams = beams.Where(b => b.LookupParameter("ETABS Unique Name").HasValue &&
                                        b.LookupParameter("ETABS Unique Name").AsString() == uName)
                                        .ToList();
                                   

                foreach (var chosenBeam in chosenBeams)
                {
                    
                    if (chosenBeam == null) {  continue; }
                    using (Transaction trn = new Transaction(doc, "Apply Beam RFT"))
                    {
                        trn.Start();
                        
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

                        trn.Commit();
                    }

                    
                        
                }

                
            }
                


        }
    }
}
