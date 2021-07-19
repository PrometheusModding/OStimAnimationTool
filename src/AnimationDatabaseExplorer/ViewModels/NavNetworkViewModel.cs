#region

using System.Linq;
using DynamicData;
using NodeNetwork.Toolkit.BreadcrumbBar;
using NodeNetwork.Toolkit.Layout.ForceDirected;
using NodeNetwork.ViewModels;
using OStimAnimationTool.Core.Models;
using OStimAnimationTool.Core.ViewModels;
using Prism.Commands;
using Prism.Regions;
using ReactiveUI;

#endregion

namespace AnimationDatabaseExplorer.ViewModels
{
    // Extension for the BreadcrumbVM to hold Networks
    public class NetworkBreadcrumb : BreadcrumbViewModel
    {
        private NetworkViewModel _network = new();

        public NetworkViewModel Network
        {
            get => _network;
            set => this.RaiseAndSetIfChanged(ref _network, value);
        }
    }

    // VM for the NodeView in the WorkspaceRegion
    public class NavNetworkViewModel : ViewModelBase
    {
        private AnimationDatabase _animationDatabase = AnimationDatabase.Instance;
        private NetworkViewModel _network = new();

        public NavNetworkViewModel()
        {
            NetworkBreadcrumbBar.ActivePath.Add(new NetworkBreadcrumb
            {
                Name = "Main",
                Network = Network
            });

            LayoutCommand = new DelegateCommand(Layouter);
            OpenGroupCommand = new DelegateCommand(OpenGroup);
        }

        // NetworkBreadcrumbBar that is used for Databinding of the NetworkView
        public BreadcrumbBarViewModel NetworkBreadcrumbBar { get; } = new();
        public DelegateCommand OpenGroupCommand { get; }
        public DelegateCommand LayoutCommand { get; }


        // Network that Holds the GroupNodes
        private NetworkViewModel Network
        {
            get => _network;
            set => SetProperty(ref _network, value);
        }

        private AnimationDatabase AnimationDatabase
        {
            get => _animationDatabase;
            set => SetProperty(ref _animationDatabase, value);
        }

        // Method to see Networks inside GroupNodes
        private void OpenGroup()
        {
            var selectedNodeGroup = (SetNodeGroupViewModel) Network.SelectedNodes.Items.First();
            NetworkBreadcrumbBar.ActivePath.Add(new NetworkBreadcrumb
            {
                Network = selectedNodeGroup.Subnet,
                Name = selectedNodeGroup.Name
            });
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            AnimationDatabase = navigationContext.Parameters.GetValue<AnimationDatabase>("animationDatabase");
            NavFinder();
        }

        private void NavFinder()
        {
            foreach (var animationSet in AnimationDatabase.AnimationSets)
            {
                var groupNode = SetNodeGroupFinder(animationSet.PositionKey);
                switch (animationSet)
                {
                    case TransitionAnimationSet transitionAnimationSet:
                    {
                        var transitionNode = TransitionNodeFinder(transitionAnimationSet, groupNode.Subnet);
                        AnimationSet destination = transitionAnimationSet.Destination;
                        if (animationSet.PositionKey != destination.PositionKey)
                        {
                            var destinationGroupNode = SetNodeGroupFinder(destination.PositionKey);
                            switch (destination)
                            {
                                case TransitionAnimationSet transitionDestination:
                                    TransitionNodeFinder(transitionDestination, destinationGroupNode.Subnet);
                                    break;
                                case HubAnimationSet hubDestination:
                                    SetNodeFinder(hubDestination, destinationGroupNode.Subnet);
                                    break;
                            }
                        }
                        else
                        {
                            switch (destination)
                            {
                                case TransitionAnimationSet transitionDestination:
                                    groupNode.Subnet.Connections.Add(
                                        groupNode.Subnet.ConnectionFactory(transitionNode.Input,
                                            TransitionNodeFinder(transitionDestination, groupNode.Subnet).Output));
                                    break;
                                case HubAnimationSet hubDestination:
                                    groupNode.Subnet.Connections.Add(
                                        groupNode.Subnet.ConnectionFactory(transitionNode.Input,
                                            SetNodeFinder(hubDestination, groupNode.Subnet).Output));
                                    break;
                            }
                        }

                        break;
                    }
                    case HubAnimationSet hubAnimationSet:
                    {
                        var setNode = SetNodeFinder(hubAnimationSet, groupNode.Subnet);
                        foreach (AnimationSet destination in hubAnimationSet.Destinations)
                            if (animationSet.PositionKey != destination.PositionKey)
                            {
                                var destinationGroupNode = SetNodeGroupFinder(destination.PositionKey);
                                switch (destination)
                                {
                                    case TransitionAnimationSet transitionDestination:
                                        TransitionNodeFinder(transitionDestination, destinationGroupNode.Subnet);
                                        break;
                                    case HubAnimationSet hubDestination:
                                        SetNodeFinder(hubDestination, destinationGroupNode.Subnet);
                                        break;
                                }
                            }
                            else
                            {
                                switch (destination)
                                {
                                    case TransitionAnimationSet transitionDestination:
                                        groupNode.Subnet.Connections.Add(
                                            groupNode.Subnet.ConnectionFactory(setNode.Input,
                                                TransitionNodeFinder(transitionDestination, groupNode.Subnet).Output));
                                        break;
                                    case HubAnimationSet hubDestination:
                                        groupNode.Subnet.Connections.Add(
                                            groupNode.Subnet.ConnectionFactory(setNode.Input,
                                                SetNodeFinder(hubDestination, groupNode.Subnet).Output));
                                        break;
                                }
                            }

                        break;
                    }
                }
            }

            foreach (var nodeViewModel in Network.Nodes.Items)
            {
                var groupNode = (SetNodeGroupViewModel) nodeViewModel;
                foreach (var node in groupNode.Subnet.Nodes.Items)
                {
                    var setNode = (SetNodeViewModel) node;
                    setNode.Initialize();
                }
            }
        }

        private static SetNodeViewModel SetNodeFinder(HubAnimationSet animationSet, NetworkViewModel subnet)
        {
            foreach (var node in subnet.Nodes.Items)
            {
                if (node is not SetNodeViewModel destNode) continue;
                if (destNode.AnimationSet.Equals(animationSet)) return destNode;
            }

            var destinationNode = new SetNodeViewModel(animationSet) {Name = animationSet.SetName};
            subnet.Nodes.Add(destinationNode);

            return destinationNode;
        }

        private static TransitionNodeViewModel TransitionNodeFinder(TransitionAnimationSet animationSet,
            NetworkViewModel subnet)
        {
            foreach (var node in subnet.Nodes.Items)
            {
                if (node is not TransitionNodeViewModel destNode) continue;
                if (destNode.AnimationSet.Equals(animationSet)) return destNode;
            }

            var destinationNode = new TransitionNodeViewModel(animationSet) {Name = animationSet.SetName};
            subnet.Nodes.Add(destinationNode);

            return destinationNode;
        }

        private SetNodeGroupViewModel SetNodeGroupFinder(string positionKey)
        {
            SetNodeGroupViewModel groupNode;

            foreach (var node in Network.Nodes.Items)
            {
                groupNode = (SetNodeGroupViewModel) node;
                if (groupNode.Name.Equals(positionKey)) return groupNode;
            }

            groupNode = new SetNodeGroupViewModel(new NetworkViewModel()) {Name = positionKey};
            Network.Nodes.Add(groupNode);

            return groupNode;
        }

        private void Layouter()
        {
            ForceDirectedLayouter layouter = new();
            layouter.Layout(new Configuration {Network = Network}, 1000);
        }
    }
}
