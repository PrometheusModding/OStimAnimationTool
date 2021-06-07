using AnimationDatabaseExplorer.Models;
using OStimAnimationTool.Core;
using Prism.Regions;

namespace AnimationDatabaseExplorer.ViewModels
{
    public class AnimationSetDatagridViewModel : TabViewModelBase
    {
        private AnimationDatabase _animationDatabase;
        private string _name;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public AnimationDatabase AnimationDatabase
        {
            get => _animationDatabase;
            set => SetProperty(ref _animationDatabase, value);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            _name = navigationContext.Parameters.GetValue<string>("name");
            AnimationDatabase = new AnimationDatabase(_name);
            Title = _name;
        }
    }
}
