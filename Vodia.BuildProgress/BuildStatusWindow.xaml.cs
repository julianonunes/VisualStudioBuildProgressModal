using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Vodia.Vodia_BuildProgress
{
    /// <summary>
    /// Interaction logic for BuildStatusWindow.xaml
    /// </summary>
    public partial class BuildStatusWindow : DialogWindow
    {
        private const int MAX_WIDTH = 364;
        public BuildStatusWindow()
        {
            InitializeComponent();
        }

        public void SetProgress(int progressValue)
        {
            if (progressValue < 0 || progressValue > 100)
                throw new ArgumentOutOfRangeException();

            double newWidth = (MAX_WIDTH * progressValue) / 100f;

            progressRectangle.Width = newWidth;
            lblPercent.Content = progressValue + "%";
        }

        public void SetProject(string project)
        {
        }
    }
}
