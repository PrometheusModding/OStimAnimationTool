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
        private AnimationSet _animationSet = new("");

        public AnimationSetDetailViewModel()
        {
            AnimationClassMenuCommand = new DelegateCommand<string>(SetAnimationClass);
            OpenAnimationDetailCommand = new DelegateCommand<Animation>(OpenAnimationDetail);

            AnimationClassMenuItems = new ObservableCollection<MenuItemViewModel>
            {
                new("Vaginal"),
                new("Anal"),
                new("Foreplay")
                {
                    MenuItems = new ObservableCollection<MenuItemViewModel>
                    {
                        new("Blowjob")
                        {
                            MenuItems = new ObservableCollection<MenuItemViewModel>
                            {
                                new("Normal"),
                                new("Head Held Blowjob"),
                                new("Penisjob (Blowjob with Jerking)"),
                                new("Head Held Penisjob"),
                                new("Self")
                            }
                        },
                        new("Handjob")
                        {
                            MenuItems = new ObservableCollection<MenuItemViewModel>
                            {
                                new("Normal"),
                                new("Masturbate"),
                                new("Head Held Masturbate"),
                                new("Apart Handjob"),
                                new("Dual Handjob")
                            }
                        },
                        new("Cuddling")
                        {
                            MenuItems = new ObservableCollection<MenuItemViewModel>
                            {
                                new("Standing Apart"),
                                new("Standing Apart Undressing"),
                                new("Embracing"),
                                new("Holding"),
                                new("Rough Holding")
                            }
                        },
                        new("Fingering")
                        {
                            Header = "Fingering", MenuItems = new ObservableCollection<MenuItemViewModel>
                            {
                                new("Rubbing Clit"),
                                new("1 Finger"),
                                new("2 Finger")
                            }
                        },
                        new("69")
                        {
                            Header = "69", MenuItems = new ObservableCollection<MenuItemViewModel>
                            {
                                new("69 with Blowjob"),
                                new("69 with Handjob")
                            }
                        },
                        new("Cunnilingus"),
                        new("Footjob"),
                        new("Boobjob"),
                        new("Breastfeeding")
                    }
                }
            };
        }

        public DelegateCommand<string> AnimationClassMenuCommand { get; }
        public DelegateCommand<Animation> OpenAnimationDetailCommand { get; }

        public ObservableCollection<MenuItemViewModel> AnimationClassMenuItems { get; }

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
