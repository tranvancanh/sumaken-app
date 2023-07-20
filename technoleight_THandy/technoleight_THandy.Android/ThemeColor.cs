using Android.Content;
using technoleight_THandy.Droid;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using technoleight_THandy.Interface;
using Android.Views;
using System.Linq;
using System;
using Android.App;
using Xamarin.Forms.Platform.Android;
using Android.OS;

[assembly: Dependency(typeof(ThemeColor))]

namespace technoleight_THandy.Droid
{
	public class ThemeColor : IThemeColor
	{
        public bool SetStatusBarColor()
        {
            try
            {
                var activity = (Activity)Forms.Context;

                // テーマに合わせたステータスバー背景色をセット
                var themeColors = Xamarin.Forms.Application.Current.Resources.MergedDictionaries.ElementAt(0);
                var themeBackgroundColor = ((Color)themeColors["BackgroundColor"]).ToAndroid();
                activity.Window.SetStatusBarColor(themeBackgroundColor);

                bool darkStatusBarTint = themeColors.GetType().Equals(typeof(technoleight_THandy.DarkTheme));

                if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
                {
                    var flag = (Android.Views.StatusBarVisibility)Android.Views.SystemUiFlags.LightStatusBar;
                    activity.Window.DecorView.SystemUiVisibility = darkStatusBarTint ? 0 : flag;
                }

            }
            catch (Exception ex)
            {

            }

            return true;

        }
    }
}