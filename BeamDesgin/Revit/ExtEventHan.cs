using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BeamDesgin.Entry;

namespace BeamDesgin.Revit
{
    public class ExtEventHan : IExternalEventHandler
    {
        public int Request { get; set; }
        public void Execute(UIApplication app)
        {
            RevitUtils.SendDataToRevit(RvtData.BeamsData, ExtCmd.Doc);
        }

        public string GetName()
        {
            return "ProMark";
        }
    }
}
