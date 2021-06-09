using System.Collections.ObjectModel;

namespace AnimationDatabaseExplorer.ViewModels
{
    public class MenuItemViewModel
    {
        public MenuItemViewModel(string header)
        {
            Header = header;
        }

        public string Header { get; init; }

        public ObservableCollection<MenuItemViewModel>? MenuItems { get; set; }
    }
}
