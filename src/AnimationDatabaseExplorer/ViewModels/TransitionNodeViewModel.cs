using System;
using System.Reactive.Linq;
using DynamicData;
using NodeNetwork.Toolkit.ValueNode;
using NodeNetwork.ViewModels;
using NodeNetwork.Views;
using OStimAnimationTool.Core.Models;
using ReactiveUI;
using Splat;

namespace AnimationDatabaseExplorer.ViewModels
{
    public class TransitionNodeViewModel : NodeViewModel
    {
        private TransitionAnimationSet _animationSet;

        static TransitionNodeViewModel()
        {
            Locator.CurrentMutable.Register(() => new NodeView(), typeof(IViewFor<SetNodeViewModel>));
        }

        public TransitionNodeViewModel(TransitionAnimationSet animationSet)
        {
            _animationSet = animationSet;

            Input = new ValueNodeInputViewModel<AnimationSet>();
            Inputs.Add(Input);

            Output = new ValueNodeOutputViewModel<AnimationSet>();
            Outputs.Add(Output);
            Output.Value = Observable.Return(AnimationSet);
        }

        public TransitionAnimationSet AnimationSet
        {
            get => _animationSet;
            set => this.RaiseAndSetIfChanged(ref _animationSet, value);
        }

        public ValueNodeInputViewModel<AnimationSet>? Input { get; }
        public ValueNodeOutputViewModel<AnimationSet>? Output { get; }

        public void Initialize()
        {
            Input?.ValueChanged.Subscribe(_ => AnimationSet.Destination = Input.Value);
        }
    }
}
