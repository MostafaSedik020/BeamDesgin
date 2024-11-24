using BeamDesgin.Elements;
using BeamDesgin.Revit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BeamDesgin.UI
{
    /// <summary>
    /// Interaction logic for WarningWindow.xaml
    /// </summary>
    public partial class WarningWindow : Window
    {
        
        public WarningWindow(ObservableCollection<Beam> beamsData ,Document doc)
        {
            InitializeComponent();
            var numOfWarnings = RevitUtils.CheckRevitModel(beamsData, doc);
            Warning_TxtBox.Text = numOfWarnings.totalWarnings.ToString();
            
        }
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                System.Windows.Point position = e.GetPosition(this);

                if (position.Y <= 30)
                {
                    if (this.WindowState == WindowState.Maximized)
                    {
                        var mousePosition = PointToScreen(e.MouseDevice.GetPosition(this));
                        this.WindowState = WindowState.Normal;
                        this.Left = mousePosition.X - (this.RestoreBounds.Width / 2);
                        this.Top = 0;
                        this.DragMove();
                    }
                    else if (this.WindowState == WindowState.Normal)
                    {
                        this.DragMove();
                    }
                }
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
    }
}
