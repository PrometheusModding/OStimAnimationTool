#region

using DynamicData;
using NodeNetwork.Toolkit.ValueNode;
using NodeNetwork.ViewModels;
using NodeNetwork.Views;
using ReactiveUI;
using Splat;

#endregion

namespace AnimationDatabaseExplorer.ViewModels
{
    public class SetNodeGroupViewModel : NodeViewModel
    {
        static SetNodeGroupViewModel()
        {
            Locator.CurrentMutable.Register(() => new NodeView(), typeof(IViewFor<SetNodeGroupViewModel>));
        }

        public SetNodeGroupViewModel(NetworkViewModel subnet)
        {
            Name = "Group";
            Subnet = subnet;

            Input = new ValueListNodeInputViewModel<string>();
            Inputs.Add(Input);

            Output = new ValueNodeOutputViewModel<string>();
            Outputs.Add(Output);
        }

        public NetworkViewModel Subnet { get; }

        public ValueListNodeInputViewModel<string>? Input { get; }
        public ValueNodeOutputViewModel<string>? Output { get; }
    }
}
