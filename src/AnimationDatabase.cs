using System;
using System.Collections.Generic;

namespace OStimConversionTool
{
    public class Animation : IEquatable<Animation>
    {
        private readonly string animationSet;
        private readonly string animationName;

        public Animation(string animSet, string animName)
        {
            animationSet = animSet;
            animationName = animName;
        }

        public bool Equals(Animation? other)
        {
            if (other is null)
                throw new NullReferenceException();

            if (animationSet.Equals(other.animationSet) && animationName.Equals(other.animationName))
                return true;

            return false;
        }
    }

    public class AnimationDatabase
    {
        private List<Animation> animationDatabase = new();

        public AnimationDatabase()
        {
        }

        public bool Add(string animSet, string animName)
        {
            if (animSet is null || animName is null)
                return false;

            animationDatabase.Add(new Animation(animSet, animName));
            return true;
        }
    }
}
