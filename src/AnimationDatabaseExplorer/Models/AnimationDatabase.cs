using System.Collections.ObjectModel;

namespace AnimationDatabaseExplorer.Models
{
    public class AnimationDatabase : ObservableCollection<AnimationSet>
    {
        public AnimationDatabase(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
