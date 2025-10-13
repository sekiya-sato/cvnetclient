using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01TokViewModel : BaseViewModel
    {
        [ObservableProperty]
        ObservableCollection<MasterShop>? listShop;
        [ObservableProperty]
        MasterShop? selectedShop;
        [ObservableProperty]
        MasterShop? editShop;
        [ObservableProperty]
        public string day1;
        [ObservableProperty]
        public string day2;
        [ObservableProperty]
        public string day3;
        [ObservableProperty]
        public string day4;
        [ObservableProperty]
        public string day5;
        [ObservableProperty]
        public string day6;
        [ObservableProperty]
        public string hour1;
        [ObservableProperty]
        public string hour2;
        [ObservableProperty]
        public string min1;
        [ObservableProperty]
        public string min2;
        [ObservableProperty]
        public string sec1;
        [ObservableProperty]
        public string sec2;
        #region ComboBox
        [ObservableProperty]
        public Dictionary<string, string> comboListBumon;
        [ObservableProperty]
        public Dictionary<string, string> comboListTanto;
        [ObservableProperty]
        public Dictionary<string, string> comboListTenshu;
        [ObservableProperty]
        public Dictionary<string, string> comboListZaiko;
        [ObservableProperty]
        public Dictionary<string, string> comboListIdo;
        [ObservableProperty]
        public Dictionary<string, string> comboListShukka;
        [ObservableProperty]
        public Dictionary<string, string> comboListJido;
        [ObservableProperty]
        public Dictionary<string, string> comboListEC;
        [ObservableProperty]
        public Dictionary<string, string> comboListPOS;
        [ObservableProperty]
        public Dictionary<string, string> comboListSoko;
        [ObservableProperty]
        public Dictionary<string, string> comboListDay;
        [ObservableProperty]
        public Dictionary<string, string> comboListHour;
        [ObservableProperty]
        public Dictionary<string, string> comboListMinSec;
        [ObservableProperty]
        public Dictionary<string, string> comboListGeKeisan;
        [ObservableProperty]
        public Dictionary<string, string> comboListGeKiri;
        [ObservableProperty]
        public Dictionary<string, string> comboListGeHasu;
        [ObservableProperty]
        public Dictionary<string, string> comboListShoKeisan;
        [ObservableProperty]
        public Dictionary<string, string> comboListShoCD;
        [ObservableProperty]
        public Dictionary<string, string> comboListShoHasu;
        [ObservableProperty]
        public Dictionary<string, string> comboListNohin;
        [ObservableProperty]
        public Dictionary<string, string> comboListBunrui01;
        [ObservableProperty]
        public Dictionary<string, string> comboListBunrui02;
        [ObservableProperty]
        public Dictionary<string, string> comboListBunrui03;
        [ObservableProperty]
        public Dictionary<string, string> comboListBunrui04;
        [ObservableProperty]
        public Dictionary<string, string> comboListBunrui05;
        [ObservableProperty]
        public Dictionary<string, string> comboListBunrui06;
        [ObservableProperty]
        public Dictionary<string, string> comboListBunrui07;
        [ObservableProperty]
        public Dictionary<string, string> comboListBunrui08;
        [ObservableProperty]
        public Dictionary<string, string> comboListBunrui09;
        [ObservableProperty]
        public Dictionary<string, string> comboListBunrui10;
        [ObservableProperty]
        public Dictionary<string, string> comboListBunrui11;
        [ObservableProperty]
        public Dictionary<string, string> comboListBunrui12;
        [ObservableProperty]
        public Dictionary<string, string> comboListBunrui13;
        [ObservableProperty]
        public Dictionary<string, string> comboListBunrui14;
        [ObservableProperty]
        public Dictionary<string, string> comboListBunrui15;
        [ObservableProperty]
        public Dictionary<string, string> comboListBunrui16;
        [ObservableProperty]
        public Dictionary<string, string> comboListBunrui17;
        [ObservableProperty]
        public Dictionary<string, string> comboListBunrui18;
        [ObservableProperty]
        public Dictionary<string, string> comboListBunrui19;
        [ObservableProperty]
        public Dictionary<string, string> comboListBunrui20;
        #endregion 

        [RelayCommand]
        void Init() 
        { 
            EditShop = new MasterShop();

            ComboListDay = new Dictionary<string, string>
            {
                { "00","00" },
                { "01","01" },
                { "02","02" },
                { "03","03" },
                { "04","04" },
                { "05","05" },
                { "06","06" },
                { "07","07" },
                { "08","08" },
                { "09","09" },
                { "10","10" },
                { "11","11" },
                { "12","12" },
                { "13","13" },
                { "14","14" },
                { "15","15" },
                { "16","16" },
                { "17","17" },
                { "18","18" },
                { "19","19" },
                { "20","20" },
                { "21","21" },
                { "22","22" },
                { "23","23" },
                { "24","24" },
                { "25","25" },
                { "26","26" },
                { "27","27" },
                { "28","28" },
                { "99","99" }
            };
            Day1 = ComboListDay.FirstOrDefault(x => x.Key == "99").Key;
            Day2 = ComboListDay.FirstOrDefault().Key;
            Day3 = ComboListDay.FirstOrDefault().Key;
            Day4 = ComboListDay.FirstOrDefault(x => x.Key == "99").Key;
            Day5 = ComboListDay.FirstOrDefault().Key;
            Day6 = ComboListDay.FirstOrDefault().Key;

            ComboListHour = new Dictionary<string, string>
            {
                { "00","00" },
                { "01","01" },
                { "02","02" },
                { "03","03" },
                { "04","04" },
                { "05","05" },
                { "06","06" },
                { "07","07" },
                { "08","08" },
                { "09","09" },
                { "10","10" },
                { "11","11" },
                { "12","12" },
                { "13","13" },
                { "14","14" },
                { "15","15" },
                { "16","16" },
                { "17","17" },
                { "18","18" },
                { "19","19" },
                { "20","20" },
                { "21","21" },
                { "22","22" },
                { "23","23" }
            };
            Hour1 = ComboListHour.FirstOrDefault().Key;
            Hour2 = ComboListHour.FirstOrDefault().Key;

            ComboListMinSec = new Dictionary<string, string>
            {
                { "00","00" },
                { "01","01" },
                { "02","02" },
                { "03","03" },
                { "04","04" },
                { "05","05" },
                { "06","06" },
                { "07","07" },
                { "08","08" },
                { "09","09" },
                { "10","10" },
                { "11","11" },
                { "12","12" },
                { "13","13" },
                { "14","14" },
                { "15","15" },
                { "16","16" },
                { "17","17" },
                { "18","18" },
                { "19","19" },
                { "20","20" },
                { "21","21" },
                { "22","22" },
                { "23","23" },
                { "24","24" },
                { "25","25" },
                { "26","26" },
                { "27","27" },
                { "28","28" },
                { "29","29" },
                { "30","30" },
                { "31","31" },
                { "32","32" },
                { "33","33" },
                { "34","34" },
                { "35","35" },
                { "36","36" },
                { "37","37" },
                { "38","38" },
                { "39","39" },
                { "40","40" },
                { "41","41" },
                { "42","42" },
                { "43","43" },
                { "44","44" },
                { "45","45" },
                { "46","46" },
                { "47","47" },
                { "48","48" },
                { "49","49" },
                { "50","50" },
                { "51","51" },
                { "52","52" },
                { "53","53" },
                { "54","54" },
                { "55","55" },
                { "56","56" },
                { "57","57" },
                { "58","58" },
                { "59","59" }
            };
            Min1 = ComboListMinSec.FirstOrDefault().Key;
            Min2 = ComboListMinSec.FirstOrDefault().Key;
            Sec1 = ComboListMinSec.FirstOrDefault().Key;
            Sec2 = ComboListMinSec.FirstOrDefault().Key;

            ComboListTenshu = new Dictionary<string, string> 
            {
                { "0","0 倉庫" },
                { "1","1 卸先" },
                { "3","3 売仕店" },
                { "6","6 直営店" }
            };
            ComboListZaiko = new Dictionary<string, string> 
            {
                { "0","0 しない" },
                { "1","1 する" }
            };
            ComboListIdo = new Dictionary<string, string> 
            {
                { "0","0 指定無" },
                { "5","5 即時" },
                { "10","10 積送" }
            };
            ComboListShukka = new Dictionary<string, string> 
            {
                { "0","0 しない" },
                { "1","1 する" }
            };
            ComboListJido = new Dictionary<string, string> 
            {
                { "0","0 自動補充しない" },
                { "1","1 自動補充する" }
            };
            ComboListEC = new Dictionary<string, string> 
            {
                { "0","0 通常店舗" },
                { "1","1 EC店舗" }
            };
            ComboListPOS = new Dictionary<string, string> 
            {
                { "0","0 通常" },
                { "9","9 POSﾏｽﾀ削除指示" },
                { "10","10 出力しない" }
            };
            ComboListSoko = new Dictionary<string, string> 
            {
                { "0","0 自社倉庫" },
                { "1","1 委託倉庫01" },
                { "2","2 委託倉庫02" },
                { "3","3 委託倉庫03" },
                { "4","4 委託倉庫04" },
                { "5","5 委託倉庫05" },
                { "6","6 委託倉庫06" },
                { "7","7 委託倉庫07" },
                { "8","8 委託倉庫08" },
                { "9","9 委託倉庫09" }
            };
            ComboListGeKeisan = new Dictionary<string, string> 
            {
                { "0","0 売価*掛率" },
                { "1","1 上代*掛率" }
            };
            ComboListGeKiri = new Dictionary<string, string> 
            {
                { "0","0 一円単位" },
                { "1","1 十円単位" },
                { "2","2 百円単位" },
                { "3","3 千円単位" },
                { "4","4 万円単位" }
            };
            ComboListGeHasu = new Dictionary<string, string> 
            {
                { "0","0 四捨五入" },
                { "1","1 切り上げ" },
                { "2","2 切り捨て" }
            };
            ComboListShoKeisan = new Dictionary<string, string> 
            {
                { "0","0 請求単位" },
                { "1","1 伝票単位" }
            };
            ComboListShoCD = new Dictionary<string, string> 
            {
                { "0","0 非課税" },
                { "1","1 課税" }
            };
            ComboListShoHasu = new Dictionary<string, string> 
            {
                { "0","0 四捨五入" },
                { "1","1 切り上げ" },
                { "2","2 切り捨て" }
            };
            ComboListNohin = new Dictionary<string, string> 
            {
                { "0","0 印刷しない" },
                { "1","1 自社伝票" },
                { "2","2 百貨店伝票" },
                { "3","3 チェーンストア統一伝票1型" },
                { "4","4 チェーンストア統一伝票" },
                { "5","5 百貨店伝票（丸井用）" },
                { "6","6 チェーンストア統一伝票（ターンアラウンド用2型）" },
                { "7","7 チェーンストア統一伝票（ターンアラウンド用1型）" },
                { "8","8 百貨店伝票Ⅱ型" }
            };

            var comboList = new Dictionary<string, string>();
            string sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='BMN' order by a.名称CD";
            var get_combolist = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboListBumon = comboList;
            EditShop.Department = ComboListBumon.FirstOrDefault().Key;

            comboList = new Dictionary<string, string>();
            sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='C01' order by a.名称CD";
            get_combolist = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboListBunrui01 = comboList;
            EditShop.NameCD01 = ComboListBunrui01.FirstOrDefault().Key;

            comboList = new Dictionary<string, string>();
            sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='C02' order by a.名称CD";
            get_combolist = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboListBunrui02 = comboList;
            EditShop.NameCD02 = ComboListBunrui02.FirstOrDefault().Key;

            comboList = new Dictionary<string, string>();
            sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='C03' order by a.名称CD";
            get_combolist = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboListBunrui03 = comboList;
            EditShop.NameCD03 = ComboListBunrui03   .FirstOrDefault().Key;

            comboList = new Dictionary<string, string>();
            sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='C04' order by a.名称CD";
            get_combolist = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboListBunrui04 = comboList;
            EditShop.NameCD04 = ComboListBunrui04.FirstOrDefault().Key;

            comboList = new Dictionary<string, string>();
            sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='C05' order by a.名称CD";
            get_combolist = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboListBunrui05 = comboList;
            EditShop.NameCD05 = ComboListBunrui05.FirstOrDefault().Key;

            comboList = new Dictionary<string, string>();
            sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='C06' order by a.名称CD";
            get_combolist = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboListBunrui06 = comboList;
            EditShop.NameCD06 = ComboListBunrui06.FirstOrDefault().Key;

            comboList = new Dictionary<string, string>();
            sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='C07' order by a.名称CD";
            get_combolist = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboListBunrui07 = comboList;
            EditShop.NameCD07 = ComboListBunrui07.FirstOrDefault().Key;

            comboList = new Dictionary<string, string>();
            sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='C08' order by a.名称CD";
            get_combolist = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboListBunrui08 = comboList;
            EditShop.NameCD08 = ComboListBunrui08.FirstOrDefault().Key;

            comboList = new Dictionary<string, string>();
            sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='C09' order by a.名称CD";
            get_combolist = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboListBunrui09 = comboList;
            EditShop.NameCD09 = ComboListBunrui09.FirstOrDefault().Key;

            comboList = new Dictionary<string, string>();
            sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='C10' order by a.名称CD";
            get_combolist = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboListBunrui10 = comboList;
            EditShop.NameCD10 = ComboListBunrui10.FirstOrDefault().Key;

            comboList = new Dictionary<string, string>();
            sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='C11' order by a.名称CD";
            get_combolist = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboListBunrui11 = comboList;
            EditShop.NameCD11 = ComboListBunrui11.FirstOrDefault().Key;

            comboList = new Dictionary<string, string>();
            sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='C12' order by a.名称CD";
            get_combolist = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboListBunrui12 = comboList;
            EditShop.NameCD12 = ComboListBunrui12.FirstOrDefault().Key;

            comboList = new Dictionary<string, string>();
            sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='C13' order by a.名称CD";
            get_combolist = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboListBunrui13 = comboList;
            EditShop.NameCD13 = ComboListBunrui13.FirstOrDefault().Key;

            comboList = new Dictionary<string, string>();
            sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='C14' order by a.名称CD";
            get_combolist = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboListBunrui14 = comboList;
            EditShop.NameCD14 = ComboListBunrui14.FirstOrDefault().Key;

            comboList = new Dictionary<string, string>();
            sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='C15' order by a.名称CD";
            get_combolist = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboListBunrui15 = comboList;
            EditShop.NameCD15 = ComboListBunrui15.FirstOrDefault().Key;

            comboList = new Dictionary<string, string>();
            sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='C16' order by a.名称CD";
            get_combolist = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboListBunrui16 = comboList;
            EditShop.NameCD16 = ComboListBunrui16.FirstOrDefault().Key;

            comboList = new Dictionary<string, string>();
            sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='C17' order by a.名称CD";
            get_combolist = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboListBunrui17 = comboList;
            EditShop.NameCD17 = ComboListBunrui17.FirstOrDefault().Key;

            comboList = new Dictionary<string, string>();
            sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='C18' order by a.名称CD";
            get_combolist = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboListBunrui18 = comboList;
            EditShop.NameCD18 = ComboListBunrui18.FirstOrDefault().Key;

            comboList = new Dictionary<string, string>();
            sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='C19' order by a.名称CD";
            get_combolist = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboListBunrui19 = comboList;
            EditShop.NameCD19 = ComboListBunrui19.FirstOrDefault().Key;

            comboList = new Dictionary<string, string>();
            sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='C20' order by a.名称CD";
            get_combolist = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboListBunrui20 = comboList;
            EditShop.NameCD20 = ComboListBunrui20.FirstOrDefault().Key;


        }

        [RelayCommand]
        void DoList() 
        { 
        
        }
    }
}
