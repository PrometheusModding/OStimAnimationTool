using System.Collections.ObjectModel;
using OStimAnimationTool.Core.Models;

namespace OStimAnimationTool.Core.Interfaces
{
    public interface IDatabaseFilter
    {
        public string FilterParameter { get; set; }
        ObservableCollection<Module> Apply(ObservableCollection<Module> modules);
    }
}
