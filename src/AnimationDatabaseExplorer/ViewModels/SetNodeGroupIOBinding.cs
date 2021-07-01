using DynamicData;
using ExampleCodeGenApp.ViewModels;
using NodeNetwork.Toolkit.Group;
using NodeNetwork.Toolkit.ValueNode;
using NodeNetwork.ViewModels;

namespace AnimationDatabaseExplorer.ViewModels
{
    public class SetNodeGroupIOBinding : ValueNodeGroupIOBinding
    {
        public SetNodeGroupIOBinding(NodeViewModel groupNode, NodeViewModel entranceNode, NodeViewModel exitNode) : base(groupNode, entranceNode, exitNode)
        {
        }

        #region Endpoint Create
        public override ValueNodeOutputViewModel<T> CreateCompatibleOutput<T>(ValueNodeInputViewModel<T> input)
        {
            return new SetNodeOutputViewModel<T>(((SetNodePortViewModel)input.Port).PortType)
            {
                Name = input.Name,
                Editor = new GroupEndpointEditorViewModel<T>(this)
            };
        }

        public override ValueNodeOutputViewModel<IObservableList<T>> CreateCompatibleOutput<T>(ValueListNodeInputViewModel<T> input)
        {
            return new SetNodeOutputViewModel<IObservableList<T>>(((SetNodePortViewModel)input.Port).PortType)
            {
                Editor = new GroupEndpointEditorViewModel<IObservableList<T>>(this)
            };
        }

        public override ValueNodeInputViewModel<T> CreateCompatibleInput<T>(ValueNodeOutputViewModel<T> output)
        {
            return new CodeGenInputViewModel<T>(((SetNodePortViewModel)output.Port).PortType)
            {
                Name = output.Name,
                Editor = new GroupEndpointEditorViewModel<T>(this),
                HideEditorIfConnected = false
            };
        }

        public override ValueListNodeInputViewModel<T> CreateCompatibleInput<T>(ValueNodeOutputViewModel<IObservableList<T>> output)
        {
            return new SetNodeListInputViewModel<T>(((SetNodePortViewModel)output.Port).PortType)
            {
                Name = output.Name,
                Editor = new GroupEndpointEditorViewModel<T>(this),
                HideEditorIfConnected = false
            };
        }
        #endregion
    }
}
