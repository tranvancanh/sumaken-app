using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using technoleight_THandy.Models;
using technoleight_THandy.Views;
using System.Collections.Generic;
using static Android.Icu.Text.CaseMap;

namespace technoleight_THandy.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        public ObservableCollection<Item> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        public ItemsViewModel()
        {
            //メイン画面を起動
            Title = "メニュー";
            Items = new ObservableCollection<Item>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
                  
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;
            //不具合がでるので　IsBusy停止
            //IsBusy = true;

            try
            {
                Items.Clear();
                string WID1 = "0";
                List<Setei> Set2 = await App.DataBase.GetSeteiAsync();
                if (Set2.Count > 0)
                {
                    WID1 = Set2[0].WID;
                }

                //SQLiteより設定ファイルを抽出
                List<MenuX> menux = await App.DataBase.GetMenuAsync(WID1, "0");
                if (menux.Count > 0)
                {
                    int x;
                    //メニュー名を抽出
                    for (x = 0; x <= menux.Count - 1; x++)
                    {
                        Items.Add(new Item { Id = Guid.NewGuid().ToString(), Text = menux[x].gamen_name, Description = menux[x].gamen_edaban });
                    }
                }
                else
                {

                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}