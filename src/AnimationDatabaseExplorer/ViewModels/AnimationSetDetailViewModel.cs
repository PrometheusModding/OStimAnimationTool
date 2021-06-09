using System.Collections.ObjectModel;
using AnimationDatabaseExplorer.Models;
using OStimAnimationTool.Core;
using OStimAnimationTool.Core.Prism;
using Prism.Commands;
using Prism.Regions;

namespace AnimationDatabaseExplorer.ViewModels
{
    public class AnimationSetDetailViewModel : TabViewModelBase, IRegionManagerAware
    {
        private AnimationSet _animationSet = new("New Animation Set");

        public AnimationSetDetailViewModel()
        {
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

        public IRegionManager? RegionManager { get; set; }

        private void OpenAnimationDetail(Animation animation)
        {
            if (RegionManager is null)
                return;

            var p = new NavigationParameters {{"animation", animation}};
            RegionManager.RequestNavigate("AnimationDetailRegion", "AnimationDetailView", p);
        }

        private void SetAnimationClass(string animationClass)
        {
            AnimationSet.AnimationClass = animationClass;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey("animationSet"))
                AnimationSet = navigationContext.Parameters.GetValue<AnimationSet>("animationSet");
            Title = AnimationSet.SetName;
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }
    }
}
