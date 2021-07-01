#region

using NodeNetwork.ViewModels;
using NodeNetwork.Views;
using ReactiveUI;
using Splat;

#endregion

namespace AnimationDatabaseExplorer.ViewModels
{
    public class SetNodeNetwork : NetworkViewModel
    {
        static SetNodeNetwork()
        {
            Locator.CurrentMutable.Register(() => new NetworkView(), typeof(IViewFor<SetNodeNetwork>));
        }
    }
}
