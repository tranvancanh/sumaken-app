using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace technoleight_THandy.Event
{
    public class MoveTriggerAction : TriggerAction<VisualElement>
    {
        public bool IsActive { get; set; }

        protected override void Invoke(VisualElement sender)
        {
            if (IsActive)
            {
                sender.TranslationX = -sender.Width;
                sender.Opacity = 0;

                sender.TranslateTo(0, 0);
                sender.FadeTo(2);
            }
            else
            {
                sender.TranslateTo(sender.Width, 0);
                sender.FadeTo(0);
            }
        }
    }
}
