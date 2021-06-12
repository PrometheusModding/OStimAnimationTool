#region

using System.Collections.Generic;
using System.Windows;
using Microsoft.Xaml.Behaviors;
using TriggerBase = Microsoft.Xaml.Behaviors.TriggerBase;

#endregion

namespace OStimAnimationTool.Core
{
    public class InteractivityTemplate : DataTemplate
    {
    }

    public class InteractivityItems : FrameworkElement
    {
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.RegisterAttached("Template",
                typeof(InteractivityTemplate),
                typeof(InteractivityItems),
                new PropertyMetadata(default(InteractivityTemplate), OnTemplateChanged));

        private List<Behavior>? _behaviors;
        private List<TriggerBase>? _triggers;

        public new List<TriggerBase> Triggers => _triggers ??= new List<TriggerBase>();

        public List<Behavior> Behaviors => _behaviors ??= new List<Behavior>();


        public static InteractivityTemplate GetTemplate(DependencyObject obj)
        {
            return (InteractivityTemplate) obj.GetValue(TemplateProperty);
        }

        public static void SetTemplate(DependencyObject obj, InteractivityTemplate value)
        {
            obj.SetValue(TemplateProperty, value);
        }

        private static void OnTemplateChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var template = (InteractivityTemplate) e.NewValue;
            var items = (InteractivityItems) template.LoadContent();
            var behaviorCollection = Interaction.GetBehaviors(d);
            var triggerCollection = Interaction.GetTriggers(d);

            foreach (var behavior in items.Behaviors)
                behaviorCollection.Add(behavior);

            foreach (var trigger in items.Triggers)
                triggerCollection.Add(trigger);
        }
    }
}
