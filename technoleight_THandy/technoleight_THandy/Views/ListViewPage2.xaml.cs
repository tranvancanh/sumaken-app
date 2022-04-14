using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using THandy.Data;
using THandy.Models;
using THandy.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace THandy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListViewPage2 : ContentPage
    {
        public ListViewPage2ViewModel vm;
        public ObservableCollection<string> Items { get; set; }

        public ListViewPage2(ListViewPage2ViewModel vm)
        {
            InitializeComponent();

            string manufacturer = "";
            try
            {
                manufacturer = DependencyService.Get<IDeviceService>().GetManufacturerName();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            string Model = "";
            try
            {
                Model = DependencyService.Get<IDeviceService>().GetModelName();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            string osVersion = "";
            try
            {
                osVersion = DependencyService.Get<IDeviceService>().GetDeviceVersion();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            string cpu = "";
            try
            {
                cpu = DependencyService.Get<IDeviceService>().GetCpuType();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            string id1 = "";
            try
            {
                id1 = DependencyService.Get<IDeviceService>().GetID();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            //アプリ名称を取得する場合
            string pkgName = "";
            try
            {
                pkgName = DependencyService.Get<IAssemblyService>().GetPackageName();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            //アプリバージョン文字列を取得する場合
            string verName = "";
            try
            {
                verName = DependencyService.Get<IAssemblyService>().GetVersionName();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            //アプリバージョンコードを取得する場合
            string verCode = "";
            try
            {
                verCode = DependencyService.Get<IAssemblyService>().GetVersionCode();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            ObservableCollection<PageTypeGroup> Items1 = new ObservableCollection<PageTypeGroup>();
            PageTypeGroup gr = new PageTypeGroup();
            gr.Title = manufacturer;
            gr.ShortName = "メーカー名";
            Items1.Add(gr);

            gr = new PageTypeGroup();
            gr.Title = Model;
            gr.ShortName = "モデル";
            Items1.Add(gr);

            gr = new PageTypeGroup();
            gr.Title = osVersion;
            gr.ShortName = "OS";
            Items1.Add(gr);

            gr = new PageTypeGroup();
            gr.Title = cpu;
            gr.ShortName = "CPU";
            Items1.Add(gr);

            gr = new PageTypeGroup();
            gr.Title = id1;
            gr.ShortName = "ID";
            Items1.Add(gr);

            gr = new PageTypeGroup();
            gr.Title = pkgName;
            gr.ShortName = "アプリ名称";
            Items1.Add(gr);

            gr = new PageTypeGroup();
            gr.Title = verName;
            gr.ShortName = "アプリバージョン文字";
            Items1.Add(gr);

            gr = new PageTypeGroup();
            gr.Title = verCode;
            gr.ShortName = "アプリバージョンコード";
            Items1.Add(gr);

            MyListView.ItemsSource = Items1;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
