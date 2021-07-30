using System;

namespace OStimAnimationTool.Core.Attributes
{
    // Attribute to mark views which are navigated simultaneously.
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DependentViewAttribute : Attribute
    {
        public DependentViewAttribute(Type viewType, string targetRegionName)
        {
            Type = viewType;
            TargetRegionName = targetRegionName;
        }

        // Type of the connected view
        public Type Type { get; }

        // Name of the target region of connected view
        public string TargetRegionName { get; }
    }
}
