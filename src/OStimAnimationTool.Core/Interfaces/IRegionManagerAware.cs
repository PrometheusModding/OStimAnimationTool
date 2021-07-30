using Prism.Regions;

namespace OStimAnimationTool.Core.Interfaces
{
    public interface IRegionManagerAware
    {
        IRegionManager? RegionManager { get; set; }
    }
}
