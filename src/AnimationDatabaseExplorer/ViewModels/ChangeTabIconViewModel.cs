using OStimAnimationTool.Core.Models.Navigation;
using Prism.Mvvm;

namespace AnimationDatabaseExplorer.ViewModels
{
    public class ChangeTabIconViewModel : BindableBase
    {
        private TabIcons _iconSource;
        
        public TabIcons IconSource
        {
            get => _iconSource;
            set => SetProperty(ref _iconSource, value);
        }
    }
}
