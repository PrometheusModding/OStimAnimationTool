#region

using OStimAnimationTool.Core.Interfaces;
using OStimAnimationTool.Core.Prism;

#endregion

namespace AnimationDatabaseExplorer.Views
{
    public partial class SetDetailView : ICreateRegionManagerScope, ISupportDataContext
    {
        public SetDetailView()
        {
            InitializeComponent();
        }

        public bool CreateRegionManagerScope => true;
    }
}
