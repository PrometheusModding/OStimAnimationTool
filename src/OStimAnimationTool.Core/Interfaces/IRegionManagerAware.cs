#region

using Prism.Regions;

#endregion

namespace OStimAnimationTool.Core.Interfaces
{
    public interface IRegionManagerAware
    {
        IRegionManager? RegionManager { get; set; }
    }
}
