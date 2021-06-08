using System.Collections.Generic;
using System.Windows;
using Microsoft.Xaml.Behaviors;
using TriggerBase = Microsoft.Xaml.Behaviors.TriggerBase;

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

        private List<Behavior> _behaviors;
        private List<TriggerBase> _triggers;

        public new List<TriggerBase> Triggers
        {
            get
            {
                if (_triggers == null)
                    _triggers = new List<TriggerBase>();
                return _triggers;
            }
        }

        public List<Behavior> Behaviors
        {
            get
            {
                if (_behaviors == null)
                    _behaviors = new List<Behavior>();
                return _behaviors;
            }
        }


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
            var dt = (InteractivityTemplate) e.NewValue;
            var ih = (InteractivityItems) dt.LoadContent();
            var bc = Interaction.GetBehaviors(d);
            var tc = Interaction.GetTriggers(d);

            foreach (var behavior in ih.Behaviors)
                bc.Add(behavior);

            foreach (var trigger in ih.Triggers)
                tc.Add(trigger);
        }
    }
}
