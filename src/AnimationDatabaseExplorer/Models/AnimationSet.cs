using System.Collections.ObjectModel;

namespace AnimationDatabaseExplorer.Models
{
    public class AnimationSet : ObservableCollection<Animation>
    {
        public string SetName { get; set; }
    }
}
