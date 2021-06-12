#region

using Prism.Regions;

#endregion

namespace OStimAnimationTool.Core.Prism
{
    public interface IRegionManagerAware
    {
        IRegionManager? RegionManager { get; set; }
    }
}
