using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BeamDesgin.Revit
{
    internal class ExtEventHan : IExternalEventHandler
    {
        public int Request { get; set; }
        public void Execute(UIApplication app)
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            return "ProMark";
        }
    }
}
