using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace technoleight_THandy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomDialog : ContentPage
    {
        public event EventHandler<bool> PopupResult; // Event to pass back the result
        public CustomDialog()
        {
            InitializeComponent();
        }

        private async void OnOKClicked(object sender, EventArgs e)
        {
            PopupResult?.Invoke(this, true); // Invoke the event with true for OK
            await Navigation.PopModalAsync();
            //// Return result as true for OK button
            //await Navigation.PopModalAsync(true);
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            PopupResult?.Invoke(this, false); // Invoke the event with false for Cancel
            await Navigation.PopModalAsync();
            //// Return result as false for Cancel button
            //await Navigation.PopModalAsync(false);
        }
    }
}