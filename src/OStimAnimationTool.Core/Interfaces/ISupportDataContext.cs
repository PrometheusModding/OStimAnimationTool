namespace OStimAnimationTool.Core.Interfaces
{
    // Interface to support sharing of datacontext between dependent views
    public interface ISupportDataContext
    {
        object DataContext { get; set; }
    }
}
