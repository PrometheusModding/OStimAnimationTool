using System.Collections.ObjectModel;

namespace AnimationDatabaseExplorer.ViewModels
{
    public class MenuItemViewModel
    {
        public MenuItemViewModel(string header)
        {
            Header = header;
        }

        public string Header { get; set; }

        public ObservableCollection<MenuItemViewModel> MenuItems { get; set; }
    }
}
