#region

using System.Collections.ObjectModel;

#endregion

namespace AnimationDatabaseExplorer.ViewModels
{
    // VM for MenuItems used in the AnimationDetail-View AnimationClass Selection Menu
    public class AnimationClassMenuItemViewModel
    {
        public AnimationClassMenuItemViewModel(string header, string classToken)
        {
            ClassToken = classToken;
            Header = header;
        }

        public string ClassToken { get; }
        public string Header { get; init; }

        public ObservableCollection<AnimationClassMenuItemViewModel>? MenuItems { get; set; }
    }
}
