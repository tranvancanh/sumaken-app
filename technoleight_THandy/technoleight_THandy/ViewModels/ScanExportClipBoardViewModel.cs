using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace technoleight_THandy.ViewModels
{
    public class ScanExportClipBoardViewModel : ScanExportViewModel
    {
        public ScanExportClipBoardViewModel() { }

        public ScanExportClipBoardViewModel(string title, int pageID, INavigation navigation)
        {
            this.Title = title;
            this.PageID = pageID;
            this.Navigation = navigation;
        }

        public async Task OnAppearing()
        {
            await InitPage(Title, PageID, Navigation);
        }

    }
}
