using System.Windows.Controls;
using OStimAnimationTool.Core.Prism;

namespace AnimationDatabaseExplorer.Views
{
    /// <summary>
    ///     Interaction logic for AnimationSetDetailView.xaml
    /// </summary>
    public partial class AnimationSetDetailView : TabItem, ICreateRegionManagerScope
    {
        public AnimationSetDetailView()
        {
            InitializeComponent();
        }

        public bool CreateRegionManagerScope => true;
    }
}
