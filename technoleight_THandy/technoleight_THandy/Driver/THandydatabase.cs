using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using technoleight_THandy.Models;
using System.Threading.Tasks;
using System.ComponentModel.Design;
using System.Collections.ObjectModel;

namespace technoleight_THandy.Driver
{
    public class technoleight_THandydatabase
    {
        readonly SQLiteAsyncConnection _database;

        public technoleight_THandydatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);

            //設定データテーブル
            _database.CreateTableAsync<Setei>().Wait();
            //読取データテーブル
            _database.CreateTableAsync<ScanReadData>().Wait();
            //メニューテーブル
            _database.CreateTableAsync<MenuX>().Wait();
            //バーコードテーブル
            _database.CreateTableAsync<BarCodeM>().Wait();
            //時区テーブル
            _database.CreateTableAsync<JikuDB>().Wait();
            //車両マスタテーブル
            _database.CreateTableAsync<Car>().Wait();
            _database.CreateTableAsync<CarDB>().Wait();
            //納品書テーブル
            _database.CreateTableAsync<Nouhin>().Wait();
            //現品票テーブル
            _database.CreateTableAsync<NouhinJL>().Wait();
            //出庫テーブル
            _database.CreateTableAsync<Shuko>().Wait();
            //在庫入庫テーブル
            _database.CreateTableAsync<ScanReceipt>().Wait();
            //在庫入庫テーブル
            _database.CreateTableAsync<PageItem>().Wait();

            DateTime dateTime = DateTime.Now;
       
        }

        //2021/04/11 作成
        #region Nouhinテーブル操作
        public Task<List<Nouhin>> GetNouhinAsync()
        {
            return _database.Table<Nouhin>()
                            .ToListAsync();
        }

        public Task<List<Nouhin>> GetNouhinAsync(string VHNOKU, string VHNOSE, string VHNONO)
        {
            return _database.Table<Nouhin>()
                            .Where(i => i.VHNOKU == VHNOKU && i.VHNOSE == VHNOSE && i.VHNOSE == VHNONO)
                            .ToListAsync();
        }

        public Task<int> GetNouhinAsync2()
        {
            return _database.Table<Nouhin>()
                           .CountAsync();
        }

        public Task<Nouhin> GetNouhinAsync(string VHNOKU, string VHNOSE, string VHNONO, string VILINE)
        {
            return _database.Table<Nouhin>()
                            .Where(i => i.VHNOKU == VHNOKU && i.VHNOSE == VHNOSE && i.VHNONO == VHNONO && i.VILINE == VILINE)
                            .FirstOrDefaultAsync();
        }

        public Task<List<Nouhin>> GetNouhinAsync2(string hinban, string suryo)
        {
            return _database.Table<Nouhin>()
                            .Where(i => i.VIBUNO == hinban && i.VISRYO == suryo)
                            .ToListAsync();
        }

        async public Task<int> SaveNouhinAsync(Nouhin nouhin)
        {
            Nouhin nouhinx = await GetNouhinAsync(nouhin.VHNOKU, nouhin.VHNOSE, nouhin.VHNONO, nouhin.VILINE);

            if (nouhinx == null )
            {
                return await _database.InsertAsync(nouhin);
            }
            else
            {
                nouhin.Id = nouhinx.Id;
                return await _database.UpdateAsync(nouhin);
            }
        }

        public Task<int> DeleteNouhinAsync(Nouhin nouhin)
        {
            return _database.DeleteAsync(nouhin);
        }

        public async Task<int> ALLDeleteNouhinAsync()
        {
            return await _database.DeleteAllAsync<Nouhin>();
        }
        #endregion

        #region NouhinJLテーブル操作
        public Task<List<NouhinJL>> GetNouhinJLAsync()
        {
            return _database.Table<NouhinJL>()
                            .ToListAsync();
        }
        public Task<NouhinJL> GetNouhinJLAsync(string JLNOKU, string JLNOSE, string JLNONO, string JLLINE, string JLRNNO, string JLBUNO, string JLNKSU)
        {
            return _database.Table<NouhinJL>()
                            .Where(i => i.JLNOKU == JLNOKU && i.JLNOSE == JLNOSE && i.JLNONO == JLNONO && i.JLLINE == JLLINE && i.JLRNNO == JLRNNO && i.JLBUNO == JLBUNO && i.JLNKSU == JLNKSU)
                            .FirstOrDefaultAsync();
        }
        async public Task<int> SaveNouhinJLAsync(NouhinJL nouhinJL)
        {
            //NouhinJL nouhinjlx = await GetNouhinJLAsync(nouhinJL.JLNOKU, nouhinJL.JLNOSE, nouhinJL.JLNONO, nouhinJL.JLLINE, nouhinJL.JLRNNO, nouhinJL.JLBUNO, nouhinJL.JLNKSU);

            //if (nouhinjlx == null)
            //{
            //    return await _database.InsertAsync(nouhinJL);
            //}
            //else
            //{
            //    nouhinJL.Id = nouhinjlx.Id;
            //    return await _database.UpdateAsync(nouhinJL);
            //}

            return await _database.InsertAsync(nouhinJL);
        }
        public Task<int> DeleteNouhinJLAsync(NouhinJL nouhinJL)
        {
            return _database.DeleteAsync(nouhinJL);
        }
        public async Task<int> ALLDeleteNouhinJLAsync()
        {
            return await _database.DeleteAllAsync<NouhinJL>();
        }
        #endregion

        #region Shukoテーブル操作
        public Task<List<Shuko>> GetShukoAsync()
        {
            return _database.Table<Shuko>()
                            .ToListAsync();
        }
        async public Task<int> SaveShukoAsync(Shuko shuko)
        {
                return await _database.InsertAsync(shuko);
        }
        public async Task<int> ALLDeleteShukoAsync()
        {
            return await _database.DeleteAllAsync<Shuko>();
        }
        #endregion

        #region Shukoテーブル操作
        public Task<List<Car>> GetCarAsync()
        {
            return _database.Table<Car>()
                            .ToListAsync();
        }
        async public Task<int> SaveCarAsync(Car car)
        {
            return await _database.InsertAsync(car);
        }
        public async Task<int> ALLDeleteCarAsync()
        {
            return await _database.DeleteAllAsync<Car>();
        }
        public Task<List<CarDB>> GetCarDBAsync()
        {
            return _database.Table<CarDB>()
                            .ToListAsync();
        }
        async public Task<int> SaveCarDBAsync(CarDB cardb)
        {
            return await _database.InsertAsync(cardb);
        }
        public async Task<int> ALLDeleteCarDBAsync()
        {
            return await _database.DeleteAllAsync<CarDB>();
        }
        #endregion

        #region Seteiテーブル操作
        public Task<List<Setei>> GetSeteiAsync()
        {
            return _database.Table<Setei>()
                            .ToListAsync();
        }

        public Task<int> GetSeteiAsync2()
        {
            return _database.Table<Setei>()
                           .CountAsync();
        }

        public Task<Setei> GetSeteiAsync(string WID1)
        {
            return _database.Table<Setei>()
                            .Where(i => i.WID == WID1)
                            .FirstOrDefaultAsync();
        }

        async public Task<int> SavSeteiAsync0(Setei Setei)
        {
            Setei setei = await GetSeteiAsync(Setei.WID);

            if (setei == null || Setei.WID != setei.WID)
            {
                return await _database.InsertAsync(Setei);
            }
            else
            {
                return await _database.UpdateAsync(Setei);
            }
        }
        async public Task<int> SavSeteiAsync(Setei Setei)
        {
            await _database.DeleteAllAsync<Setei>();
            return await _database.InsertAsync(Setei);
            
        }

        public Task<int> DeleteSeteiAsync(Setei Setei)
        {
            return _database.DeleteAsync(Setei);
        }

        public async Task<int> ALLDeleteSeteiAsync()
        {
            return  await _database.DeleteAllAsync<Setei>();
        }
        #endregion

        //2021/04/11 作成
        #region Menuテーブル操作
        public Task<List<MenuX>> GetMenuAsync(string WID1, string gamen_id1)
        {
            return _database.Table<MenuX>()
                            .Where(i => i.WID == WID1 && i.gamen_id == gamen_id1)
                            .ToListAsync();
        }

        public Task<int> GetMenuAsync2()
        {
            return _database.Table<MenuX>()
                           .CountAsync();
        }

        public Task<MenuX> GetMenuAsync(string WID1, string gamen_id1, string gamen_edaban1)
        {
            return _database.Table<MenuX>()
                            .Where(i => i.WID == WID1 && i.gamen_id == gamen_id1 && i.gamen_edaban == gamen_edaban1)
                            .FirstOrDefaultAsync();
        }

        async public Task<int> SavMenuAsync(MenuX menu0)
        {
            MenuX menux = await GetMenuAsync(menu0.WID, menu0.gamen_id, menu0.gamen_edaban);

            if (menux == null || ((menu0.WID != menux.WID ) || (menu0.gamen_id != menux.gamen_id) || (menu0.gamen_edaban != menux.gamen_edaban)))
            {
                return await _database.InsertAsync(menu0);
            }
            else
            {
                return await _database.UpdateAsync(menu0);
            }
        }

        public Task<int> DeleteMenuAsync(MenuX menu0)
        {
            return _database.DeleteAsync(menu0);
        }

        public async Task<int> ALLDeleteMenuAsync()
        {
            return await _database.DeleteAllAsync<MenuX>();
        }
        #endregion

        #region BarCodeMテーブル操作
        public Task<List<BarCodeM>> GetBarCodeMAsync()
        {
            return _database.Table<BarCodeM>()
                            .ToListAsync();
        }

        public Task<int> GetBarCodeMAsync2()
        {
            return _database.Table<BarCodeM>()
                           .CountAsync();
        }

        public Task<List<BarCodeM>> GetBarCodeMAsync(string WID1, string gamen_id1)
        {
            return _database.Table<BarCodeM>()
                            .Where(i => i.WID == WID1 && i.gamen_id == gamen_id1)
                            .ToListAsync();
        }

        public Task<BarCodeM> GetBarCodeMAsync(string WID1, string gamen_id1, string gamen_edaban1, string edaban1)
        {
            return _database.Table<BarCodeM>()
                            .Where(i => i.WID == WID1 && i.gamen_id == gamen_id1 && i.gamen_edaban == gamen_edaban1 && i.edaban == edaban1)
                            .FirstOrDefaultAsync();
        }

        async public Task<int> SaveBarCodeMAsync(BarCodeM BarCode_Master0)
        {
            BarCodeM barCode_Master = await GetBarCodeMAsync(BarCode_Master0.WID, BarCode_Master0.gamen_id, BarCode_Master0.gamen_edaban, BarCode_Master0.edaban);

            if (barCode_Master == null || ((BarCode_Master0.WID != barCode_Master.WID) || (BarCode_Master0.gamen_id != barCode_Master.gamen_id) || (BarCode_Master0.gamen_edaban != barCode_Master.gamen_edaban) || (BarCode_Master0.edaban != barCode_Master.edaban)))
            {
                return await _database.InsertAsync(BarCode_Master0);
            }
            else
            {
                return await _database.UpdateAsync(BarCode_Master0);
            }
        }

        public Task<int> DeleteBarCodeMAsync(BarCodeM BarCode_Master0)
        {
            return _database.DeleteAsync(BarCode_Master0);
        }

        public async Task<int> ALLDeleteBarCodeMAsync()
        {
            return await _database.DeleteAllAsync<BarCodeM>();
        }
        #endregion

        #region JikuDBテーブル項目
        public Task<List<JikuDB>> GetJikuDBAsync()
        {
            return _database.Table<JikuDB>()
                            .ToListAsync();
        }

        public Task<int> GetJikuDBAsync2()
        {
            return _database.Table<JikuDB>()
                           .CountAsync();
        }

        public Task<JikuDB> GetJikuDBAsync(string Id)
        {
            return _database.Table<JikuDB>()
                            .Where(i => i.Id == Id)
                            .FirstOrDefaultAsync();
        }

        async public Task<int> SaveJikuDBAsync(JikuDB JikuDB0)
        {
            JikuDB JikuDB1 = await GetJikuDBAsync(JikuDB0.Id);

            if (JikuDB1 == null || ((JikuDB1.Id != JikuDB0.Id)))
            {
                return await _database.InsertAsync(JikuDB0);
            }
            else
            {
                return await _database.UpdateAsync(JikuDB0);
            }
        }

        public Task<int> DeleteJikuDBAsync(JikuDB JikuDB0)
        {
            return _database.DeleteAsync(JikuDB0);
        }

        public async Task<int> ALLDeleteJikuDBAsync()
        {
            return await _database.DeleteAllAsync<JikuDB>();
        }
        #endregion


        //2021/01/19 作成
        #region ScanReadDataﾃｰﾌﾞﾙ操作
        public Task<int> DeleteAllScanReadData()
        {
            return _database.DeleteAllAsync<ScanReadData>();
        }

        async public Task<int> DeleteAllkubunScanReadData(string readkubun)
        {
            List<ScanReadData> ScanData2 = await App.DataBase.GetScanReadDataAsync(readkubun);
            int m = 0;
            if (ScanData2.Count > 0)
            {
                int x;
                //メニュー名を抽出
                for (x = 0; x <= ScanData2.Count - 1; x++)
                {
                    m = await _database.DeleteAsync(ScanData2[x]);
                }
            }
            return m ;
        }

        async public Task<int> DeleteScanReadData(ScanReadData ScanData)
        {
            return await _database.DeleteAsync(ScanData);
        }

        /// <summary>
        /// 全取得
        /// </summary>
        /// <returns></returns>
        public Task<List<ScanReadData>> GetScanReadDataAsync(string readkubun)
        {
            //return _database.Table<ScanReadData>().ToListAsync();
            return _database.Table<ScanReadData>()
                            .Where(i => i.Kubn == readkubun)
                            .ToListAsync();
        }

        //スキャンコードからデータをチェック
        public Task<ScanReadData> GetScanReadDataAsync(string readkubun, string scandata)
        {
            return _database.Table<ScanReadData>()
                            .Where(i => i.Scanstring == scandata && i.Kubn == readkubun)
                            .FirstOrDefaultAsync();
        }

        public Task<List<ScanReadData>> GetGenpinScanReadDataAsync(string readkubun, string scandata)
        {
            return _database.Table<ScanReadData>()
                            .Where(i => i.Scanstring.StartsWith(scandata) == true && i.Kubn == readkubun)
                           .ToListAsync(); ;
        }

        async public Task<int> DeleteGenpinScanReadData(string readkubun, string scandat)
        {
            //現品票の削除処理
            List<ScanReadData> ScanData2 = await App.DataBase.GetGenpinScanReadDataAsync(readkubun, scandat);
            int m = 0;
            if (ScanData2.Count > 0)
            {
                int x;
                //現品票を抽出
                for (x = 0; x <= ScanData2.Count - 1; x++)
                {
                    //現品票実績削除
                    m = await _database.DeleteAsync(ScanData2[x]);
                    //削除した現品票の数量を減らす
                    string VHNOKU = ScanData2[x].Scanstring.Substring(1, 1);
                    string VHNOSE = ScanData2[x].Scanstring.Substring(2, 1);
                    string VHNONO = ScanData2[x].Scanstring.Substring(3, 5);
                    string VILINE = ScanData2[x].Scanstring.Substring(8, 1);
                    string SRYO = ScanData2[x].Scanstring.Substring(12, 5);

                    int sr=0;
                    if (int.TryParse(SRYO, out sr)) { }
                    int NKSU = 0;
                    Nouhin nouhin = await App.DataBase.GetNouhinAsync(VHNOKU, VHNOSE, VHNONO, VILINE);
                    if (int.TryParse(nouhin.JJNKSU, out NKSU)) { }
                    nouhin.JJNKSU = (NKSU - sr).ToString();
                    //入庫数を減らす
                    await App.DataBase.SaveNouhinAsync(nouhin);
                }
            }
            return m;
        }

        //スキャンコード数のカウント
        async public Task<int> GetScanReadDataAsync4(string readkubun)
        {
            return await _database.Table<ScanReadData>()
                           .Where(i => i.Kubn == readkubun)
                           .CountAsync();
        }

        //日付で抽出
        async public Task<ScanReadData> GetScanReadDataAsync3(string readkubun, DateTime scandata)
        {
            DateTime t1 = new DateTime(scandata.Year, scandata.Month, scandata.Day, 0, 0, 0);
            DateTime t2 = new DateTime(scandata.Year, scandata.Month, scandata.Day, 23, 59, 59, 9999);

            return await _database.Table<ScanReadData>()
                            .Where(i => (i.cdate >= t1 && i.cdate <= t2) && i.Kubn == readkubun)
                            .FirstOrDefaultAsync();
        }

        // 1件追加
        async public Task<int> SaveScanReadDataAsync(string readkubun, ScanReadData ScanData)
        {
            ScanReadData viewQRC = await App.DataBase.GetScanReadDataAsync(readkubun, ScanData.Scanstring);
            if (viewQRC == null)
            {
                return await _database.InsertAsync(ScanData);
            }
            else
            {
                return 0;
            }
        }

        #endregion

        #region 在庫入庫データ
        /// <summary>
        /// 在庫入庫データを全取得
        /// </summary>
        /// <returns></returns>
        public Task<List<ScanReceipt>> GetScanReceiptAsync()
        {
            return _database.Table<ScanReceipt>()
                            .ToListAsync();
        }
        /// <summary>
        /// 在庫入庫データを1件保存
        /// </summary>
        /// <returns></returns>
        async public Task<int> SaveScanReceiptAsync(ScanReceipt scanReceipt)
        {
            var scanReceipts = await App.DataBase.GetScanReceiptAsync();
            return await _database.InsertAsync(scanReceipt);
        }
        /// <summary>
        /// 在庫入庫データを全件保存
        /// </summary>
        /// <returns></returns>
        async public Task<int> SaveScanReceiptListAsync(List<ScanReceipt> scanReceiptList)
        {
            var scanReceipts = await App.DataBase.GetScanReceiptAsync();
            return await _database.InsertAllAsync(scanReceiptList);
        }
        /// <summary>
        /// 在庫入庫データを全件削除
        /// </summary>
        /// <returns></returns>
        async public Task<int> DeleteAllScanReceipt()
        {
            return await _database.DeleteAllAsync<ScanReceipt>();
        }
        #endregion

        #region ページデータ
        /// <summary>
        /// 全取得
        /// </summary>
        /// <returns></returns>
        public Task<PageItem> GetPageItemAsync()
        {
            return _database.Table<PageItem>()
                .FirstOrDefaultAsync();
        }
        /// <summary>
        /// データを1件保存
        /// </summary>
        /// <returns></returns>
        async public Task<int> SavePageItemAsync(PageItem pageItem)
        {
            var pageItems = await GetPageItemAsync();
            if (pageItems == null)
            {
                return await _database.InsertAsync(pageItem);
            }
            else
            {
                return await _database.UpdateAsync(pageItem);
            }

        }
        /// <summary>
        /// 全件削除
        /// </summary>
        /// <returns></returns>
        async public Task<int> DeleteAllPageItem()
        {
            return await _database.DeleteAllAsync<PageItem>();
        }
        #endregion


    }
}
