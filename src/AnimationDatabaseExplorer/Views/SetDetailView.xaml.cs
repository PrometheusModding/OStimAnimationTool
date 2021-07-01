using OStimAnimationTool.Core.Interfaces;
using Prism.Regions;

namespace AnimationDatabaseExplorer.Views
{
    public partial class SetDetailView : IRegionManagerAware, ISupportDataContext
    {
        public SetDetailView()
        {
            InitializeComponent();
        }

        public IRegionManager? RegionManager { get; set; }
    }
}
