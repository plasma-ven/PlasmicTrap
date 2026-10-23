using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlasmicTrap.UI.ViewModels.Settings;

namespace PlasmicTrap.UI.Elements.Settings.Pages
{
    /// <summary>
    /// Interaction logic for PlasmicTrapPage.xaml
    /// </summary>
    public partial class PlasmicTrapPage
    {
        public PlasmicTrapPage()
        {
            DataContext = new PlasmicTrapViewModel();
            InitializeComponent();
        }
    }
}
