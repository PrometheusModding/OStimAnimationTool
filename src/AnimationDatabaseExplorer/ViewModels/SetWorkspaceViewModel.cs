using System.Collections.ObjectModel;
using System.Linq;
using AnimationDatabaseExplorer.Views;
using MaterialDesignThemes.Wpf;
using OStimAnimationTool.Core.Events;
using OStimAnimationTool.Core.Models;
using OStimAnimationTool.Core.Models.Navigation;
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
        private Page? _activePage;
        private Tab? _activeTab;
        private AnimationSet? _animationSet;
        private ObservableCollection<Tab>? _tabs;

        public SetWorkspaceViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            eventAggregator.GetEvent<AddDestinationEvent>().Subscribe(AddOption);

            OpenAnimationDetailCommand = new DelegateCommand<Animation>(OpenAnimationDetail);
            AnimationClassMenuCommand = new DelegateCommand<string>(SetAnimationClass);
            AddTabCommand = new DelegateCommand(AddTab);
            AddPageCommand = new DelegateCommand(AddPage);
            DeleteTabCommand = new DelegateCommand<Tab>(DeleteTab);
            DeletePageCommand = new DelegateCommand<Page>(DeletePage);
            DeleteOptionCommand = new DelegateCommand<Option>(DeleteOption);
            ChangeIconCommand = new DelegateCommand<object>(ChangeIcon);

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
        public DelegateCommand AddTabCommand { get; }
        public DelegateCommand AddPageCommand { get; }
        public DelegateCommand<Tab> DeleteTabCommand { get; }
        public DelegateCommand<Page> DeletePageCommand { get; }
        public DelegateCommand<Option> DeleteOptionCommand { get; }
        public DelegateCommand<object> ChangeIconCommand { get; }


        public AnimationSet? AnimationSet
        {
            get => _animationSet;
            private set => SetProperty(ref _animationSet, value, () => RaisePropertyChanged(nameof(Tabs)));
        }

        public Tab? ActiveTab
        {
            get => _activeTab;
            set
            {
                SetProperty(ref _activeTab, value);
                if (_activeTab?.Pages.FirstOrDefault() is not null)
                    ActivePage = _activeTab?.Pages[0];
            }
        }

        public Page? ActivePage
        {
            get => _activePage;
            set => SetProperty(ref _activePage, value);
        }

        public ObservableCollection<Tab>? Tabs
        {
            get => _tabs;
            set => SetProperty(ref _tabs, value);
        }

        private void AddTab()
        {
            Tabs?.Add(new Tab());
        }

        private void AddPage()
        {
            ActiveTab?.Pages.Add(new Page());
        }

        private void AddOption(AnimationSet animationSet)
        {
            if (!IsActive) return;
            if (AnimationSet is not HubAnimationSet) return;
            ActivePage?.Options.Add(new Option(animationSet));
        }

        private void DeleteTab(Tab tab)
        {
            Tabs?.Remove(tab);
        }

        private void DeletePage(Page page)
        {
            ActiveTab?.Pages.Remove(page);
        }

        private void DeleteOption(Option option)
        {
            ActivePage?.Options.Remove(option);
        }

        private void ChangeIcon(object icons)
        {
            DialogHost.Show(new ChangeTabIconView());

            _eventAggregator.GetEvent<ChangeIconEvent>().Publish(icons);
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
            if (AnimationSet is not HubAnimationSet hubAnimationSet) return;

            Tabs = hubAnimationSet.NavTabs;
            if (Tabs.FirstOrDefault() is null) return;
            
            ActiveTab = Tabs[0];
            if (_activeTab?.Pages.FirstOrDefault() is not null)
                ActivePage = _activeTab?.Pages[0];
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
