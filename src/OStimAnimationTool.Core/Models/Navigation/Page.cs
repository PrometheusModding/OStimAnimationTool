using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace OStimAnimationTool.Core.Models.Navigation
{
    public class Page : BindableBase
    {
        private PageIcons _icon;
        private Colors _iconIconColor;
        private ObservableCollection<Option> _options = new();

        public PageIcons Icon
        {
            get => _icon;
            set => SetProperty(ref _icon, value);
        }

        public ObservableCollection<Option> Options
        {
            get => _options;
            set => SetProperty(ref _options, value);
        }

        public Colors IconColor
        {
            get => _iconIconColor;
            set => SetProperty(ref _iconIconColor, value);
        }
    }
}
