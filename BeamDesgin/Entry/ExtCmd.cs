using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BeamDesgin.Revit;
using BeamDesgin.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeamDesgin.Entry
{
    [Transaction(TransactionMode.Manual)]
    public class ExtCmd : IExternalCommand
    {
        public static UIDocument UIDoc {  get; set; }
        public static Document Doc { get; set; }
        //public static ExtEventHan ExtEventHan { get; set; }

        //public static ExternalEvent ExtEvent { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIDoc = commandData.Application.ActiveUIDocument;
            Doc = UIDoc.Document;

            //ExtEventHan = new ExtEventHan();
            //ExtEvent = ExternalEvent.Create(ExtEventHan);

            MainWindow mainWindow = new MainWindow(Doc);
            mainWindow.ShowDialog();
            //mainWindow.Show();

            return Result.Succeeded;
        }
    }
}
