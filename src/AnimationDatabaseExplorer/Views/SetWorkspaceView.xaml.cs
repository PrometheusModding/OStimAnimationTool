#region

using OStimAnimationTool.Core;
using OStimAnimationTool.Core.Interfaces;

#endregion

namespace AnimationDatabaseExplorer.Views
{
    [DependentView(typeof(SetDetailView), "DetailRegion")]
    public partial class SetWorkspaceView : ISupportDataContext
    {
        public SetWorkspaceView()
        {
            InitializeComponent();
        }
    }
}
