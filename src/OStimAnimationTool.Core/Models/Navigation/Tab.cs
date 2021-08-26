using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace OStimAnimationTool.Core.Models.Navigation
{
    public class Tab : BindableBase
    {
        private Actors _actor;

        private TabIcons _icon;

        private Colors _textColor;

        private Colors _iconColor;

        private ObservableCollection<Page> _pages = new();

        private XMenu? _xMenu;

        public XMenu? XMenu
        {
            get => _xMenu;
            set => SetProperty(ref _xMenu, value);
        }

        public Actors Actor
        {
            get => _actor;
            set => SetProperty(ref _actor, value);
        }
        
        public TabIcons Icon
        {
            get => _icon;
            set => SetProperty(ref _icon, value);
        }
        
        public Colors TextColor
        {
            get => _textColor;
            set => SetProperty(ref _textColor, value);
        }
        
        public Colors IconColor
        {
            get => _iconColor;
            set => SetProperty(ref _iconColor, value);
        }

        public ObservableCollection<Page> Pages
        {
            get => _pages;
            set => SetProperty(ref _pages, value);
        }
    }
}
