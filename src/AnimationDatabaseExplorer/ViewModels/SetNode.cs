#region

using System;
using NodeNetwork.ViewModels;
using NodeNetwork.Views;
using ReactiveUI;
using Splat;

#endregion

namespace AnimationDatabaseExplorer.ViewModels
{
    public class SetNode : NodeViewModel, IEquatable<SetNode>
    {
        static SetNode()
        {
            Locator.CurrentMutable.Register(() => new NodeView(), typeof(IViewFor<SetNode>));
        }

        public bool Equals(SetNode? other)
        {
            return other is not null && Name.Equals(other.Name);
        }
    }
}
