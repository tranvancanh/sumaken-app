using SQLite;
using System;
using System.Collections.Generic;
using technoleight_THandy.Models;
using System.Threading.Tasks;
using technoleight_THandy.Common;
using static technoleight_THandy.Models.ScanCommon;
using static technoleight_THandy.Models.Setting;
using System.Linq;

namespace technoleight_THandy.Driver
{
    public class technoleight_THandydatabase
    {
        readonly SQLiteAsyncConnection _database;

        public technoleight_THandydatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            //ログインデータテーブル
            _database.CreateTableAsync<Login.LoginUserSqlLite>().Wait();
            //設定データテーブル
            _database.CreateTableAsync<Setting.SettingSqlLite>().Wait();
            //メニューテーブル
            _database.CreateTableAsync<MenuX>().Wait();
            //スキャン入荷テーブル
            _database.CreateTableAsync<Qrcode.QrcodeItem>().Wait();
            //スキャン入荷送信テーブル
            _database.CreateTableAsync<ScanCommonApiPostRequestBody>().Wait();
            //酒倉デポAGF設定データテーブル
            _database.CreateTableAsync<SettingHandyApiAgfUrl>().Wait();
            //酒倉デポAGF出荷かんばんデータテーブル
            _database.CreateTableAsync<AGFShukaKanbanData>().Wait();

            DateTime dateTime = DateTime.Now;
       
        }

        #region
        public Task<List<Login.LoginUserSqlLite>> GetLognAsync()
        {
            return _database.Table<Login.LoginUserSqlLite>()
                            .ToListAsync();
        }

        async public Task<int> SavLoginAsync(Login.LoginUserSqlLite login)
        {
            await _database.DeleteAllAsync<Login.LoginUserSqlLite>();
            return await _database.InsertAsync(login);

        }
        #endregion

        #region Settingテーブル操作
        public Task<List<Setting.SettingSqlLite>> GetSettingAsync()
        {
            return _database.Table<Setting.SettingSqlLite>()
                            .ToListAsync();
        }

        public Task<int> GetSettingAsync2()
        {
            return _database.Table<Setting.SettingSqlLite>()
                           .CountAsync();
        }

        public async Task<int> SavSettingAsync(Setting.SettingSqlLite Setting)
        {
            await _database.DeleteAllAsync<Setting.SettingSqlLite>();
            return await _database.InsertAsync(Setting);
            
        }

        public Task<int> DeleteSettingAsync(Setting.SettingSqlLite Setting)
        {
            return _database.DeleteAsync(Setting);
        }

        public async Task<int> ALLDeleteSettingAsync()
        {
            return  await _database.DeleteAllAsync<Setting.SettingSqlLite>();
        }
        #endregion

        //2021/04/11 作成
        #region Menuテーブル操作
        public Task<List<MenuX>> GetMenuAsync()
        {
            return _database.Table<MenuX>().OrderBy(x => x.HandyPageNumber)
                            .ToListAsync();
        }

        async public Task<int> SavMenuAsync(MenuX menu0)
        {
            return await _database.InsertAsync(menu0);
        }

        public async Task<int> ALLDeleteMenuAsync()
        {
            return await _database.DeleteAllAsync<MenuX>();
        }
        #endregion

        //2022/01/06 作成
        #region ScanReceiveSendDataﾃｰﾌﾞﾙ操作
        //async public Task<int> DeleteScanReceiveSendData(ScanCommonApiPostRequestBody ScanData)
        //{
        //    return await _database.DeleteAsync(ScanData);
        //}
        /// <summary>
        /// １件追加
        /// </summary>
        /// <returns></returns>
        public async Task<int> SaveScanReceiveSendDataAsync(ScanCommonApiPostRequestBody ScanData)
        {
            return await _database.InsertAsync(ScanData);
        }
        /// <summary>
        /// 全削除
        /// </summary>
        /// <returns></returns>
        public Task<int> DeleteAllScanReceiveSendData()
        {
            return _database.DeleteAllAsync<ScanCommonApiPostRequestBody>();
        }
        /// <summary>
        /// 削除
        /// PageIDを指定
        /// </summary>
        /// <returns></returns>
        public async Task DeleteScanReceiveSendData(int pageID)
        {
            var receiveApiPosts = await App.DataBase.GetScanReceiveSendDataAsync(pageID);
            foreach (var item in receiveApiPosts)
            {
                await _database.DeleteAsync<ScanCommonApiPostRequestBody>(item.Id);
            }
        }
        /// <summary>
        /// 削除
        /// PageIDとReceiveDateを指定
        /// </summary>
        /// <returns></returns>
        public async Task DeleteScanReceiveSendData(int pageID, string receiveDate)
        {
            var receiveApiPosts = await App.DataBase.GetScanReceiveSendDataAsync(pageID, receiveDate);
            foreach (var item in receiveApiPosts)
            {
                await _database.DeleteAsync<ScanCommonApiPostRequestBody>(item.Id);
            }
        }
        /// <summary>
        /// 全取得
        /// </summary>
        /// <returns></returns>
        public Task<List<ScanCommonApiPostRequestBody>> GetScanReceiveSendDataAsync()
        {
            //return _database.Table<ScanReadData>().ToListAsync();
            return _database.Table<ScanCommonApiPostRequestBody>()
                            .ToListAsync();
        }
        /// <summary>
        /// 取得
        /// </summary>
        /// <returns></returns>
        public Task<List<ScanCommonApiPostRequestBody>> GetScanReceiveSendDataAsync(int pageID)
        {
            //return _database.Table<ScanReadData>().ToListAsync();
            return _database.Table<ScanCommonApiPostRequestBody>()
                            .Where(i => i.HandyPageID == pageID)
                            .ToListAsync();
        }
        /// <summary>
        /// 取得 Okeyデータのみ
        /// </summary>
        /// <returns></returns>
        public Task<List<ScanCommonApiPostRequestBody>> GetScanReceiveSendOkeyDataAsync(int pageID)
        {
            //return _database.Table<ScanReadData>().ToListAsync();
            return _database.Table<ScanCommonApiPostRequestBody>()
                            .Where(i => i.HandyPageID == pageID && i.HandyOperationClass == (int)Enums.HandyOperationClass.Okey)
                            .ToListAsync();
        }
        /// <summary>
        /// 取得 Errorデータのみ
        /// </summary>
        /// <returns></returns>
        public Task<List<ScanCommonApiPostRequestBody>> GetScanReceiveSendErrorDataAsync(int pageID)
        {
            //return _database.Table<ScanReadData>().ToListAsync();
            return _database.Table<ScanCommonApiPostRequestBody>()
                            .Where(i => i.HandyPageID == pageID && i.HandyOperationClass != (int)Enums.HandyOperationClass.Okey)
                            .ToListAsync();
        }
        /// <summary>
        /// 取得
        /// </summary>
        /// <returns></returns>
        public Task<List<ScanCommonApiPostRequestBody>> GetScanReceiveSendDataAsync(int pageID, string receiveDate)
        {
            //return _database.Table<ScanReadData>().ToListAsync();
            return _database.Table<ScanCommonApiPostRequestBody>()
                            .Where(i => i.HandyPageID == pageID && i.ProcessDate == receiveDate)
                            .ToListAsync();
        }
        /// <summary>
        /// 取得 Okeyデータのみ
        /// </summary>
        /// <returns></returns>
        public Task<List<ScanCommonApiPostRequestBody>> GetScanReceiveSendOkeyDataAsync(int pageID, string receiveDate)
        {
            //return _database.Table<ScanReadData>().ToListAsync();
            return _database.Table<ScanCommonApiPostRequestBody>()
                            .Where(i => i.HandyPageID == pageID && i.ProcessDate == receiveDate && i.HandyOperationClass == (int)Enums.HandyOperationClass.Okey)
                            .ToListAsync();
        }
        /// <summary>
        /// 取得 Errorデータのみ
        /// </summary>
        /// <returns></returns>
        public Task<List<ScanCommonApiPostRequestBody>> GetScanReceiveSendErrorDataAsync(int pageID, string receiveDate)
        {
            //return _database.Table<ScanReadData>().ToListAsync();
            return _database.Table<ScanCommonApiPostRequestBody>()
                            .Where(i => i.HandyPageID == pageID && i.ProcessDate == receiveDate && i.HandyOperationClass != (int)Enums.HandyOperationClass.Okey)
                            .ToListAsync();
        }
        /// <summary>
        /// 取得
        /// </summary>
        /// <returns></returns>
        public Task<List<ScanCommonApiPostRequestBody>> GetReceiveSendOkeyDataAsync(int pageID, string receiveDate, string scanString)
        {
            return _database.Table<ScanCommonApiPostRequestBody>()
                            .Where(i => i.ScanString1.StartsWith(scanString) == true && i.ProcessDate == receiveDate && i.HandyPageID == pageID && i.HandyOperationClass == (int)Enums.HandyOperationClass.Okey)
                           .ToListAsync(); ;
        }
        ///// <summary>
        ///// 取得（ページ問わず）
        ///// </summary>
        ///// <returns></returns>
        //public Task<List<ScanCommonApiPostRequestBody>> GetReceiveSendOkeyDataAsync(string receiveDate, string scanString)
        //{
        //    return _database.Table<ScanCommonApiPostRequestBody>()
        //                    .Where(i => i.ScanString1.StartsWith(scanString) == true && i.ProcessDate == receiveDate && i.HandyOperationClass == (int)Enums.HandyOperationClass.Okey)
        //                   .ToListAsync(); ;
        //}
        /// <summary>
        /// 取得（ページ問わず）
        /// </summary>
        /// <returns></returns>
        public Task<List<ScanCommonApiPostRequestBody>> GetReceiveSendOkeyDataAsync(string scanString)
        {
            return _database.Table<ScanCommonApiPostRequestBody>()
                            .Where(i => i.ScanString1.StartsWith(scanString) == true && i.HandyOperationClass == (int)Enums.HandyOperationClass.Okey)
                           .ToListAsync(); ;
        }

        /// <summary>
        /// カウント取得
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetScanReceiveSendDataCountAsync(int pageID, string receiveDate)
        {
            return await _database.Table<ScanCommonApiPostRequestBody>()
                           .Where(i => i.HandyPageID == pageID && i.ProcessDate == receiveDate)
                           .CountAsync();
        }
        #endregion

        #region スキャン入荷データ
        /// <summary>
        /// 全取得
        /// </summary>
        /// <returns></returns>
        public Task<List<Qrcode.QrcodeItem>> GetScanReceiveAsync()
        {
            return _database.Table<Qrcode.QrcodeItem>()
                            .ToListAsync();
        }
        /// <summary>
        /// 取得
        /// </summary>
        /// <returns></returns>
        public Task<List<Qrcode.QrcodeItem>> GetScanReceiveAsync(int pageID)
        {
            return _database.Table<Qrcode.QrcodeItem>().Where(i => i.PageID == pageID).ToListAsync();
        }
        /// <summary>
        /// 取得
        /// </summary>
        /// <returns></returns>
        public Task<List<Qrcode.QrcodeItem>> GetScanReceiveAsync(int pageID, string receiveDate)
        {
            return _database.Table<Qrcode.QrcodeItem>().Where(i => i.PageID == pageID && i.ProcessceDate == receiveDate).ToListAsync();
        }
        /// <summary>
        /// 1件保存
        /// </summary>
        /// <returns></returns>
        public async Task<int> SaveScanReceiveAsync(Qrcode.QrcodeItem scanReceive)
        {
            var scanReceives = await App.DataBase.GetScanReceiveAsync();
            return await _database.InsertAsync(scanReceive);
        }
        /// <summary>
        /// 全件保存
        /// </summary>
        /// <returns></returns>
        public async Task<int> SaveScanReceiveListAsync(List<Qrcode.QrcodeItem> scanReceiveList)
        {
            var scanReceives = await App.DataBase.GetScanReceiveAsync();
            return await _database.InsertAllAsync(scanReceiveList);
        }
        /// <summary>
        /// 全件削除
        /// </summary>
        /// <returns></returns>
        public async Task<int> DeleteAllScanReceive()
        {
            return await _database.DeleteAllAsync<Qrcode.QrcodeItem>();
        }
        /// <summary>
        /// 削除
        /// PageIDを指定
        /// </summary>
        /// <returns></returns>
        public async Task DeleteScanReceive(int pageID)
        {
            var qrcodeItems = await App.DataBase.GetScanReceiveAsync(pageID);
            foreach (var item in qrcodeItems)
            {
                await _database.DeleteAsync<Qrcode.QrcodeItem>(item.Id);
            }
        }
        /// <summary>
        /// 削除
        /// PageIDとReceiveDateを指定
        /// </summary>
        /// <returns></returns>
        public async Task DeleteScanReceive(int pageID, string receiveDate)
        {
            var qrcodeItems = await App.DataBase.GetScanReceiveAsync(pageID, receiveDate);
            foreach (var item in qrcodeItems)
            {
                await _database.DeleteAsync<Qrcode.QrcodeItem>(item.Id);
            }
        }
        #endregion


        #region 酒倉デポAGF設定データテーブル
        public async Task<int> DeleteALLSettingHandyApiAgfUrlAsync()
        {
            return await _database.DeleteAllAsync<Setting.SettingHandyApiAgfUrl>();
        }

        public async Task<int> SaveSettingHandyApiAgfUrlAsync(Setting.SettingHandyApiAgfUrl settingHandyApiAgfUrl)
        {
            return await _database.InsertAsync(settingHandyApiAgfUrl);
        }

        public async Task<List<Setting.SettingHandyApiAgfUrl>> GetSettingHandyApiAgfUrlAsync()
        {
            return await _database.Table<Setting.SettingHandyApiAgfUrl>()
                            .ToListAsync();
        }
        #endregion

        #region 酒倉デポAGF出荷かんばんデータテーブル
        public async Task<int> DeleteALLAGFShukaKanbanDataAsync()
        {
            return await _database.DeleteAllAsync<AGFShukaKanbanData>();
        }

        public async Task<int> SaveAGFShukaKanbanDataAsync(AGFShukaKanbanData agfShukaKanbanData)
        {
            return await _database.InsertAsync(agfShukaKanbanData);
        }

        public async Task<List<AGFShukaKanbanData>> GetAllAGFShukaKanbanDataAsync()
        {
            return await _database.Table<AGFShukaKanbanData>()
                            .ToListAsync();
        }

        public async Task<List<AGFShukaKanbanData>> GetAGFShukaKanbanDataAsync(int handyPageID, DateTime processDate)
        {
            var agfShukaKanbanDatas = await GetAllAGFShukaKanbanDataAsync();
            var datas = agfShukaKanbanDatas.Where(x => x.HandyPageID == handyPageID && x.ProcessceDate == processDate).ToList();
            return datas;
        }

        public async Task<bool> FindAGFShukaKanbanDataAsync(AGFShukaKanbanData agfShukaKanbanData)
        {
            var agfShukaKanbanDatas = await GetAllAGFShukaKanbanDataAsync();
            var find = agfShukaKanbanDatas.Exists(x => 
            x.DepoCode == agfShukaKanbanData.DepoCode 
            && x.TokuiSakiCode == agfShukaKanbanData.TokuiSakiCode
            && x.KoKu == agfShukaKanbanData.KoKu
            && x.Ukeire == agfShukaKanbanData.Ukeire);
            return find;
        }

        public async Task<bool> FindAGFShukaKanbanDataAsync(int depo, string tokuiSakiCode, string koku, string ukeire, string hinban)
        {
            var agfShukaKanbanDatas = await GetAllAGFShukaKanbanDataAsync();
            var find = agfShukaKanbanDatas.Exists(x =>
            x.DepoCode == depo
            && x.TokuiSakiCode == tokuiSakiCode
            && x.KoKu == koku
            && x.Ukeire == ukeire
            && x.Hinban == hinban);
            return find;
        }

        #endregion

    }
}
