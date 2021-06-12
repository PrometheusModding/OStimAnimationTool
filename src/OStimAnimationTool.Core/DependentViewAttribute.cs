#region

using System;

#endregion

namespace OStimAnimationTool.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DependentViewAttribute : Attribute
    {
        public DependentViewAttribute(Type viewType, string targetRegionName)
        {
            Type = viewType;
            TargetRegionName = targetRegionName;
        }

        public Type Type { get; }

        public string TargetRegionName { get; }
    }
}
