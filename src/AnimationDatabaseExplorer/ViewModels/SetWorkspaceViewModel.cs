#region

using System.Collections.ObjectModel;
using AnimationDatabaseExplorer.Models;
using OStimAnimationTool.Core;
using OStimAnimationTool.Core.Events;
using OStimAnimationTool.Core.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;

#endregion

namespace AnimationDatabaseExplorer.ViewModels
{
    public class SetWorkspaceViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private AnimationSet _animationSet = new("New Animation Set");

        public SetWorkspaceViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            AnimationClassMenuCommand = new DelegateCommand<string>(SetAnimationClass);
            OpenAnimationDetailCommand = new DelegateCommand<Animation>(OpenAnimationDetail);

            AnimationClassMenuItems = new ObservableCollection<AnimationClassMenuItemViewModel>
            {
                new("Vaginal", "Sx"),
                new("Anal", "An"),
                new("Foreplay", "BJ")
                {
                    MenuItems = new ObservableCollection<AnimationClassMenuItemViewModel>
                    {
                        new("Blowjob", "BJ")
                        {
                            MenuItems = new ObservableCollection<AnimationClassMenuItemViewModel>
                            {
                                new("Normal", "BJ"),
                                new("Head Held Blowjob", "HhBJ"),
                                new("Penisjob (Blowjob with Jerking)", "ApPJ"),
                                new("Head Held Penisjob", "HhPJ"),
                                new("Self", "SJ")
                            }
                        },
                        new("Handjob", "HJ")
                        {
                            MenuItems = new ObservableCollection<AnimationClassMenuItemViewModel>
                            {
                                new("Normal", "HJ"),
                                new("Masturbate", "Po"),
                                new("Head Held Masturbate", "HhPo"),
                                new("Apart Handjob", "ApHJ"),
                                new("Dual Handjob", "DHJ")
                            }
                        },
                        new("Cuddling", "Em")
                        {
                            MenuItems = new ObservableCollection<AnimationClassMenuItemViewModel>
                            {
                                new("Standing Apart", "Ap"),
                                new("Standing Apart Undressing", "ApU"),
                                new("Embracing", "Em"),
                                new("Holding", "Ho"),
                                new("Rough Holding", "Ro")
                            }
                        },
                        new("Fingering", "Cr")
                        {
                            Header = "Fingering", MenuItems = new ObservableCollection<AnimationClassMenuItemViewModel>
                            {
                                new("Rubbing Clit", "Cr"),
                                new("1 Finger", "Pf1"),
                                new("2 Finger", "Pf2")
                            }
                        },
                        new("69", "VBJ")
                        {
                            Header = "69", MenuItems = new ObservableCollection<AnimationClassMenuItemViewModel>
                            {
                                new("69 with Blowjob", "VBJ"),
                                new("69 with Handjob", "VHJ")
                            }
                        },
                        new("Cunnilingus", "VJ"),
                        new("Footjob", "FJ"),
                        new("Boobjob", "BoJ"),
                        new("Breastfeeding", "BoF")
                    }
                }
            };
        }

        public DelegateCommand<string> AnimationClassMenuCommand { get; }
        public DelegateCommand<Animation> OpenAnimationDetailCommand { get; }

        public ObservableCollection<AnimationClassMenuItemViewModel> AnimationClassMenuItems { get; }

        public AnimationSet AnimationSet
        {
            get => _animationSet;
            private set => SetProperty(ref _animationSet, value);
        }

        private void OpenAnimationDetail(Animation animation)
        {
            var p = new NavigationParameters {{"animation", animation}};
            _regionManager.RequestNavigate("AnimationDetailRegion", "AnimationDetailView", p);
        }

        private void SetAnimationClass(string animationClass)
        {
            AnimationSet.AnimationClass = animationClass;
            _eventAggregator.GetEvent<ChangeAnimationClassEvent>().Publish();
        }


        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            AnimationSet = navigationContext.Parameters.GetValue<AnimationSet>("animationSet");
        }
    }
}
