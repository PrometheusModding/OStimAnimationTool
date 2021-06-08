using System.Collections.ObjectModel;

namespace AnimationDatabaseExplorer.Models
{
    public class AnimationSet : ObservableCollection<Animation>
    {
        private string _setName;

        public AnimationSet(string setName)
        {
            _setName = setName;
        }

        public string SetName
        {
            get => _setName;
            set
            {
                if (value != null && value == _setName) return;
                _setName = value;
            }
        }
    }
}
