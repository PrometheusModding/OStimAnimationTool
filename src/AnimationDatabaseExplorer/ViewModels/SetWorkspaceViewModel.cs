using System.Collections.ObjectModel;
using AnimationDatabaseExplorer.Views;
using OStimAnimationTool.Core.Events;
using OStimAnimationTool.Core.Models;
using OStimAnimationTool.Core.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;

namespace AnimationDatabaseExplorer.ViewModels
{
    public class SetWorkspaceViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private AnimationSet? _animationSet;

        public SetWorkspaceViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            eventAggregator.GetEvent<AddDestinationEvent>().Subscribe(AddDestination);

            OpenAnimationDetailCommand = new DelegateCommand<Animation>(OpenAnimationDetail);
            AnimationClassMenuCommand = new DelegateCommand<string>(SetAnimationClass);
            RemoveDestinationCommand = new DelegateCommand<AnimationSet>(RemoveDestination);

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

        public ObservableCollection<AnimationClassMenuItemViewModel> AnimationClassMenuItems { get; }

        public DelegateCommand<Animation> OpenAnimationDetailCommand { get; }
        public DelegateCommand<string> AnimationClassMenuCommand { get; }
        public DelegateCommand<AnimationSet> RemoveDestinationCommand { get; }

        public AnimationSet? AnimationSet
        {
            get => _animationSet;
            private set => SetProperty(ref _animationSet, value);
        }

        private void RemoveDestination(AnimationSet animationSet)
        {
            if (AnimationSet is HubAnimationSet hubAnimationSet)
            {
                hubAnimationSet.Destinations.Remove(animationSet);
            }
        }

        private void AddDestination(AnimationSet animationSet)
        {
            if (!IsActive) return;
            if (AnimationSet is not HubAnimationSet hubAnimationSet) return;
            if (!hubAnimationSet.Destinations.Contains(animationSet))
                hubAnimationSet.Destinations.Add(animationSet);
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            foreach (var view in _regionManager.Regions["WorkspaceRegion"].Views)
            {
                if (view is not SetWorkspaceView { DataContext: SetWorkspaceViewModel setWorkspaceViewModel }) continue;
                var animationSet = navigationContext.Parameters.GetValue<AnimationSet>("animationSet");
                return setWorkspaceViewModel.AnimationSet != null &&
                       setWorkspaceViewModel.AnimationSet.Equals(animationSet);
            }

            return false;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            AnimationSet = navigationContext.Parameters.GetValue<AnimationSet>("animationSet");
        }

        private void OpenAnimationDetail(Animation animation)
        {
            var p = new NavigationParameters { { "animation", animation } };
            _regionManager.RequestNavigate("AnimationDetailRegion", "AnimationDetailView", p);
        }

        private void SetAnimationClass(string animationClass)
        {
            if (AnimationSet != null)
                AnimationSet.AnimationClass = animationClass;
            _eventAggregator.GetEvent<ChangeAnimationClassEvent>().Publish();
        }
    }
}
