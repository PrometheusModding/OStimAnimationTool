﻿using Fluent;

namespace OStimAnimationTool.Core.Controls
{
    public class StartScreen2 : StartScreen
    {
        protected override bool Show()
        {
            if (Shown) return false;

            base.Show();

            var parentRibbon = GetParentRibbon(this);
            parentRibbon?.TitleBar?.SetCurrentValue(RibbonTitleBar.IsCollapsedProperty, false);

            return Shown;
        }
    }
}
