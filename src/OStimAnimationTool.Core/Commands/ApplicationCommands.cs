#region

using Prism.Commands;

#endregion

namespace OStimAnimationTool.Core.Commands
{
    public interface IApplicationCommands
    {
        CompositeCommand SaveDatabaseCommand { get; }
    }

    public class ApplicationCommands : IApplicationCommands
    {
        public CompositeCommand SaveDatabaseCommand { get; } = new();
    }
}
