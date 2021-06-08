using Prism.Regions;

namespace OStimAnimationTool.Core.Prism
{
    public interface IRegionManagerAware
    {
        IRegionManager RegionManager { get; set; }
    }
}
