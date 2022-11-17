using System;
using System.Collections.Generic;
using technoleight_THandy.ViewModels;
using technoleight_THandy.Views;
using Xamarin.Forms;

namespace technoleight_THandy
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            //Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            //Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        }

    }
}
