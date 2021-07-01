using OStimAnimationTool.Core;
using OStimAnimationTool.Core.Interfaces;
using OStimAnimationTool.Core.Prism;

namespace AnimationDatabaseExplorer.Views
{
    [DependentView(typeof(SetDetailView), "DetailRegion")]
    public partial class SetWorkspaceView : ICreateRegionManagerScope, ISupportDataContext
    {
        public SetWorkspaceView()
        {
            InitializeComponent();
        }

        public bool CreateRegionManagerScope => true;
    }
}
