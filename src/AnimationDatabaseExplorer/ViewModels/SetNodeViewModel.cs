#region

using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using NodeNetwork.Toolkit.ValueNode;
using NodeNetwork.ViewModels;
using NodeNetwork.Views;
using OStimAnimationTool.Core.Models;
using ReactiveUI;
using Splat;

#endregion

namespace AnimationDatabaseExplorer.ViewModels
{
    public class SetNodeViewModel : NodeViewModel
    {
        private HubAnimationSet _animationSet;

        static SetNodeViewModel()
        {
            Locator.CurrentMutable.Register(() => new NodeView(), typeof(IViewFor<SetNodeViewModel>));
        }

        public SetNodeViewModel(HubAnimationSet animationSet)
        {
            _animationSet = animationSet;

            Input = new ValueListNodeInputViewModel<AnimationSet>();
            Inputs.Add(Input);

            Output = new ValueNodeOutputViewModel<AnimationSet>();
            Outputs.Add(Output);
            Output.Value = Observable.Return(AnimationSet);
        }

        public HubAnimationSet AnimationSet
        {
            get => _animationSet;
            set => this.RaiseAndSetIfChanged(ref _animationSet, value);
        }

        public ValueListNodeInputViewModel<AnimationSet>? Input { get; }
        public ValueNodeOutputViewModel<AnimationSet>? Output { get; }

        public void Initialize()
        {
            Input?.Values.CountChanged.Subscribe(_ =>
                AnimationSet.Destinations = new ObservableCollection<AnimationSet>(Input.Values.Items));
        }
    }
}
