#region

using System.Collections.ObjectModel;

#endregion

namespace AnimationDatabaseExplorer.ViewModels
{
    public class AnimationClassMenuItemViewModel
    {
        public AnimationClassMenuItemViewModel(string header, string classToken)
        {
            Header = header;
            ClassToken = classToken;
        }

        public string ClassToken { get; }
        public string Header { get; init; }

        public ObservableCollection<AnimationClassMenuItemViewModel>? MenuItems { get; set; }
    }
}
