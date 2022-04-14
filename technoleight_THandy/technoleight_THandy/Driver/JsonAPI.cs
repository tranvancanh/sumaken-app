using Newtonsoft.Json;
using System.Collections.Generic;

namespace Web_PRS.API
{
    [JsonObject]
    public class JsonData
    {
        [JsonProperty("tablename")]
        public string tablename { get; set; }
        [JsonProperty("keyColumn")]
        public string keyColumn { get; set; }
        [JsonProperty("jsonSQLs")]
        public List<JsonSQL> jsonSQLs { get; set; }

        public JsonData()
        {
            tablename = "";
            keyColumn = "";
            jsonSQLs = new List<JsonSQL>();
        }
        public void setJsonSQLs(JsonSQL jsonSQL)
        {
            jsonSQLs.Add(jsonSQL);
        }
    }
    public class JsonSQL
    {
        [JsonProperty("code")]
        public string code { get; set; }
        [JsonProperty("item")]
        public List<JsonItem> item { get; set; }
        [JsonProperty("from")]
        public List<JsonTable> from { get; set; }
        [JsonProperty("where")]
        public List<JsonItem> where { get; set; }
        public JsonSQL()
        {
            code = "";
            item = new List<JsonItem>();
            from = new List<JsonTable>();
            where = new List<JsonItem>();
        }

        public void setSelectItems(string col, string data = "", string table = "")
        {
            string[] cols = { col};
            string[] datas = { data };
            setSelectItems(cols, datas, table);
        }
        public void setSelectItems(string[] cols, string[] datas = null, string table = "")
        {
            if (datas == null) datas = cols;
            for (int i = 0; i < cols.Length; i++)
            {
                JsonItem jsonItem = new JsonItem();
                jsonItem.column = cols[i];
                if (datas[i] == "") jsonItem.data = cols[i];
                else jsonItem.data = datas[i];
                char index = 'A';
                if (table == "") jsonItem.tindex = ((char)(index + from.Count)).ToString();
                else jsonItem.tindex = table;
                jsonItem.type = "";
                item.Add(jsonItem);
            }
        }
        public void setFromTables(string name, string tindex = "")
        {
            JsonTable jsonTable = new JsonTable();
            jsonTable.name = name;
            char index = 'A';
            if (tindex == "") jsonTable.tindex = ((char)(index + from.Count)).ToString();
            else jsonTable.tindex = tindex;
            jsonTable.on = null;
            from.Add(jsonTable);
        }
        /// <summary>
        /// where句のセット
        /// </summary>
        /// <param name="col">絞りたい項目</param>
        /// <param name="data">絞る値</param>
        /// <param name="table">どのテーブルか（省略した場合、セットされてるテーブル）</param>
        /// <param name="type"></param>
        public void setWhereItems(string col, string data, string table = "", string type = "")
        {
            JsonItem jsonItem = new JsonItem();
            jsonItem.column = col;
            jsonItem.data = data;
            char index = 'A';
            if (table == "") jsonItem.tindex = ((char)(index + from.Count)).ToString();
            else jsonItem.tindex = table;
            jsonItem.type = type;
            where.Add(jsonItem);
        }
        public void setInsertSQL(Dictionary<string,string> items,string tablename)
        {
            code = "INSERT";
            foreach(var item in items)
            {
                JsonItem jsonItem = new JsonItem();
                jsonItem.column = item.Key;
                jsonItem.data = item.Value;
                this.item.Add(jsonItem);
            }
            JsonTable jsonTable = new JsonTable();
            jsonTable.name = tablename;
            from.Add(jsonTable);
        }
        public void setUpdateSQL(Dictionary<string, string> items, string tablename,List<JsonItem> Where)
        {
            code = "UPDATE";
            foreach (var item in items)
            {
                JsonItem jsonItem = new JsonItem();
                jsonItem.column = item.Key;
                jsonItem.data = item.Value;
                this.item.Add(jsonItem);
            }
            JsonTable jsonTable = new JsonTable();
            jsonTable.name = tablename;
            from.Add(jsonTable);
            where = Where;
        }
    }
    public class JsonItem
    {
        [JsonProperty("tindex")]
        public string tindex { get; set; }
        [JsonProperty("column")]
        public string column { get; set; }
        [JsonProperty("data")]
        public string data { get; set; }
        [JsonProperty("type")]
        public string type { get; set; }
        public JsonItem()
        {
            tindex = "";
            column = "";
            data = "";
            type = "";
        }
    }
    public class JsonTable
    {
        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("tindex")]
        public string tindex { get; set; }
        [JsonProperty("column")]
        public List<JsonOn> on { get; set; }
        public JsonTable()
        {
            name = "";
            tindex = "";
            on = new List<JsonOn>();
        }
    }

    public class JsonOn
    {
        [JsonProperty("column_left")]
        public string column_left { get; set; }
        [JsonProperty("tindex")]
        public string tindex { get; set; }
        [JsonProperty("column_join")]
        public string column_join { get; set; }
        public JsonOn()
        {
            column_left = "";
            tindex = "";
            column_join = "";
        }
    }
}