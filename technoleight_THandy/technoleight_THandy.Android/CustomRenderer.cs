using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using technoleight_THandy.Droid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using technoleight_THandy.Controls;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
[assembly: ExportRenderer(typeof(CustomPicker), typeof(CustomPickerRenderer))]
[assembly: ExportRenderer(typeof(CustomDatePicker), typeof(CustomDatePickerRenderer))]
namespace technoleight_THandy.Droid
{
    public class CustomEntryRenderer : EntryRenderer
    {
        public CustomEntryRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                Control.Background = null;
                Control.SetPadding(5, 5, 5, 5);

            }
        }
    }

    public class CustomPickerRenderer : Xamarin.Forms.Platform.Android.AppCompat.PickerRenderer
    {
        public CustomPickerRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                Control.Background = null;
                Control.SetPadding(5, 5, 5, 5);
                //Control.Typeface = Android.Graphics.Typeface.CreateFromAsset(Forms.Context.Assets, "MPLUSRounded1c-Bold.ttf");
            }
        }
    }

    public class CustomDatePickerRenderer : DatePickerRenderer
    {
        public CustomDatePickerRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.DatePicker> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                Control.Background = null;
                Control.SetPadding(5, 5, 5, 5);
            }
        }
    }
}