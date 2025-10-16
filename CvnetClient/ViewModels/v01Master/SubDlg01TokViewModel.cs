using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Windows.Controls;

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
        [ObservableProperty]
        string? selShopCD;
        [ObservableProperty]
        int? pageNow;
        [ObservableProperty]
        int? pageTotal;
        private BizArray col_list;
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

        string sql_collist = """
               得意先CD,得意先名,カナ,旧コード,略称,郵便番号,住所1,住所2,住所3,TEL,FAX,
        宛名FLG1,宛名FLG2,宛名FLG3,宛名名称1,宛名名称2,営業担当CD,店種区分,坪数,在庫管理FLG,
        掛率,セール掛率,店頭セール掛率,請求先CD,請求印刷,締日,入金予定月,入金予定日,入金方法,下代桁切指定,
        下代端数区分,下代計算FLG,消費税CD,消費税計算方法,消費税端数,与信限度額,入金率,出荷停止FLG,
        伝票発行区分,備考,伝票印字1,伝票印字2,伝票印字3,伝票印字4
        ,倉庫区分,営業時間1,営業時間2,営業時間3,施工業者情報,デベロッパ,開始時刻,終了時刻,端末ID,営業時間,棚卸日END
        ,部門,為替区分,為替桁切指定,為替端数区分,名称CD07,名称CD08,名称CD09,名称CD10,配分ランク01,配分ランク02
        ,基準倉庫FLG,基準倉庫CD,出荷FLG,POS区分,配分方法FLG,伝票印字5,伝票印字6,伝票印字7,伝票印字8
        ,店舗売場コード,ECFLG,締日2,締日3,入金予定月2,入金予定日2,入金予定月3,入金予定日3
        ,伝票社名,伝票店名,移動区分,法人CD,連携先法人CD,連携CD,振込先1,振込先2,振込先3,期日,下限額
        ,名称CD11,名称CD12,名称CD13,名称CD14,名称CD15,名称CD16,名称CD17,名称CD18,名称CD19,名称CD20
        ,得意先MAIL,登録番号
        """;

        
        public void OnInit() 
        { 
            EditShop = new MasterShop();
            #region ComboList Init
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

            #endregion

            sql_collist = sql_collist.Replace("\r", "").Replace("\n", "").Trim();
            string[] wrk_list = sql_collist.Trim().Split(',');
            for (int i = 0; i < wrk_list.Length; i++)
            {
                wrk_list[i] = wrk_list[i].Trim();
            }
            col_list = (wrk_list.Length > 0) ? new BizArray(wrk_list) : new BizArray();
        }

        partial void OnSelectedShopChanged(MasterShop? value)
        {
            if (value != null)
            {
                EditShop = CvnetBaseCore.Common.CloneObject(value);
                if (EditShop.StartTime != null && EditShop.StartTime != ".") { 
                    Hour1 = EditShop.StartTime.Substring(0,2);
                    Min1 = EditShop.StartTime.Substring(2, 2);
                    Sec1 = EditShop.StartTime.Substring(4, 2);
                }
                if (EditShop.EndTime != null && EditShop.EndTime != ".")
                {
                    Hour2 = EditShop.EndTime.Substring(0, 2);
                    Min2 = EditShop.EndTime.Substring(2, 2);
                    Sec2 = EditShop.EndTime.Substring(4, 2);
                }
            }                            
            else
                EditShop = null;
        }

        [RelayCommand]
        void DoList() 
        {
            if (AppData.ClassCvnet.MstDialog.ContainsKey("得意先") && AppData.ClassCvnet.ComboListFLg == 1)
            {
                var ar = new string[] { "A" };
                var vm = AppData.DlgService.GetSelTok(AppData.ClassCvnet.MstDialog["得意先"].v_mstname, null, ar);
                if (vm != null)
                {
                    //vm.SelShoResult0 
                    PageView(vm.SelTokResult1.Item2, 0, vm.SelTokResult1.Item1);
                }
                return;
            }
            var v_para = new string[] { SelShopCD };
        }

        /// <summary>
        /// 20210104 ページ表示機能
        /// </summary>
        /// <param name="v_para"></param>
        /// <param name="action">[0 Refresh, 1 Next, 2 Prev, 3 Page]</param>
        /// <param name="param"></param>
        public void PageView(BizArray v_para, int action, string param = "")
        {
            int colNum = 0;

            string sql_query = " select A.得意先CD ";
            sql_query += " from HC$Master_TOKUI A ";
            if (string.IsNullOrEmpty(param))
                sql_query += " where A.得意先CD >= :1";
            else
                sql_query += " where A.得意先CD in (" + param + ")";
            sql_query += " order by A.得意先CD asc ";

            var wrk_csv2 = AppData.Http?.AspxSqlQuery(sql_query, v_para.ToArray());

            if (wrk_csv2 == null || wrk_csv2.Rows.Count == 0) return;

            /* get total page count */
            string para_seq;
            BizArray arrPageSEQ = new BizArray();
            int totalRow = wrk_csv2.Rows.Count;
            int maxDsp = AppData.ClassCvnet.MaxCntDisp;
            int pageCnt = (totalRow / maxDsp);
            int modpageCnt = totalRow % maxDsp;
            if (modpageCnt > 0) pageCnt = pageCnt + 1;

            /* create array of first seq NO of each page*/
            for (int i = 0; i < pageCnt; i++)
                arrPageSEQ[i] = wrk_csv2.Rows[i * maxDsp][colNum].ToString();

            // Refresh
            if (action == 0)
            {
                /* update page number indicator */
                if (arrPageSEQ.Count > 0)
                {
                    PageNow = 1;
                    PageTotal = arrPageSEQ.Count;
                }
                /* get csv first page */
                para_seq = arrPageSEQ[0];
                OnQuery(v_para, param, para_seq);
            }
            // Next
            else if (action == 1)
            {
                if (PageNow > 0 && PageNow < arrPageSEQ.Count)
                {
                    /* update page indicator */
                    PageNow = (int)PageNow + 1;
                    /* get csv next page */
                    para_seq = arrPageSEQ[(int)PageNow - 1];
                }
                else return;
                OnQuery(v_para, param, para_seq);
            }
            // Prev
            else if (action == 2)
            {
                if (PageNow > 1 && PageNow <= arrPageSEQ.Count)
                {
                    /* update page indicator */
                    PageNow = (PageNow) - 1;
                    /* get csv prev page */
                    para_seq = arrPageSEQ[(int)PageNow - 1];
                }
                else return;
                OnQuery(v_para, param, para_seq);
            }
            // Page
            else if (action == 3)
            {
                if (PageNow >= 1 && PageNow <= arrPageSEQ.Count)
                {
                    /* get csv number page */
                    para_seq = arrPageSEQ[(int)PageNow - 1];
                }
                else return;
                OnQuery(v_para, param, para_seq);
            }
        }

        /// <summary>
        /// 画面表示更新用検索処理 
        /// </summary>
        /// <param name="v_para"></param>
        /// <param name="qs"></param>
        /// <param name="para_seq"></param>
        private void OnQuery(BizArray v_para, string qs, string para_seq)
        {
            var sql_query = "select A.SEQ_NO,A.VDATE_CREATE,A.VDATE_UPDATE";
            for (int i = 0; i < col_list.Count; i++)
                sql_query += ",A." + col_list[i];

            sql_query += ",NVL((select H.名前 from HC$MASTER_SHAIN H 	where H.社員CD=A.営業担当CD),'.') 担当名";
            sql_query += ",NVL((select H.得意先名 from HC$MASTER_TOKUI H where  H.得意先CD=A.請求先CD),'.') 請求名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C01' and H.名称CD=A.名称CD01),'.') 補足01名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C02' and H.名称CD=A.名称CD02),'.') 補足02名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C03' and H.名称CD=A.名称CD03),'.') 補足03名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C04' and H.名称CD=A.名称CD04),'.') 補足04名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C05' and H.名称CD=A.名称CD05),'.') 補足05名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C06' and H.名称CD=A.名称CD06),'.') 補足06名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C07' and H.名称CD=A.名称CD07),'.') 補足07名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C08' and H.名称CD=A.名称CD08),'.') 補足08名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C09' and H.名称CD=A.名称CD09),'.') 補足09名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C10' and H.名称CD=A.名称CD10),'.') 補足10名";
            /****** 20070111 最終修正者追加対応 START ******/
            sql_query += ",(A.入力社員CD ||' '|| (select B.名前 from HC$MASTER_SHAIN B where B.社員CD=A.入力社員CD)) 最終修正者";
            /****** 20070111 最終修正者追加対応 END    ******/
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='BMN' and H.名称CD=A.部門),'.') 部門名";
            sql_query += ",NVL((select T.得意先名 from HC$MASTER_TOKUI T where T.得意先CD=A.基準倉庫CD),'.') 基準倉庫名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='HJN' and H.名称CD=A.法人CD),'.') 法人名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='HJN' and H.名称CD=A.連携先法人CD),'.') 連携先法人名";

            /* 2015.08.28 #22020対応（名称CD11～20追加）//////////////////////////////////////////////////////////////////////////////////////// start */
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C11' and H.名称CD=A.名称CD11),'.') 補足11名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C12' and H.名称CD=A.名称CD12),'.') 補足12名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C13' and H.名称CD=A.名称CD13),'.') 補足13名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C14' and H.名称CD=A.名称CD14),'.') 補足14名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C15' and H.名称CD=A.名称CD15),'.') 補足15名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C16' and H.名称CD=A.名称CD16),'.') 補足16名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C17' and H.名称CD=A.名称CD17),'.') 補足17名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C18' and H.名称CD=A.名称CD18),'.') 補足18名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C19' and H.名称CD=A.名称CD19),'.') 補足19名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C20' and H.名称CD=A.名称CD20),'.') 補足20名";
            /* 2015.08.28 #22020対応（名称CD11～20追加）//////////////////////////////////////////////////////////////////////////////////////// end */

            /* 2017.10.05 ide 予備名 追加対応 E */
            sql_query += " from HC$Master_TOKUI A";

            if (qs == null) sql_query += " where A.得意先CD >= :1";
            else sql_query += " where A.得意先CD in (" + qs + ")";

            /* 20210104 ページ表示機能 */
            if (para_seq != null)
                sql_query += " and A.得意先CD >= '" + para_seq + "' ";
            sql_query += " order by A.得意先CD asc ";
            /* 20210104 ページ表示機能 */
            if (para_seq != null) sql_query = AppData.ClassCvnet.GetSqlDisp(sql_query);

            var retData = AppData.Http?.AspxSqlQuery(sql_query, v_para.ToArray());
            if (retData == null || retData.Rows.Count == 0) return;
            try
            {
                var list = (from DataRow dr in retData.Rows
                            select new MasterShop
                            {
                                SeqNo = long.TryParse(dr["SEQ_NO"].ToString(), out var _seqNo) ? _seqNo : 0,
                                VdateCreate = decimal.TryParse(dr["VDATE_CREATE"].ToString(),out var _dateC) ? _dateC : 0,
                                VdateUpdate = decimal.TryParse(dr["VDATE_UPDATE"].ToString(), out var _dateU) ? _dateU : 0,
                                TradingCD = dr["得意先CD"].ToString() ?? string.Empty,
                                TradingName = dr["得意先名"].ToString() ?? string.Empty,
                                Postal = dr["郵便番号"].ToString() ?? string.Empty,
                                Address1 = dr["住所1"].ToString() ?? string.Empty,
                                Address2 = dr["住所2"].ToString() ?? string.Empty,
                                Address3 = dr["住所3"].ToString() ?? string.Empty,
                                OldCD = dr["旧コード"].ToString() ?? string.Empty,
                                Abbr = dr["略称"].ToString() ?? string.Empty,
                                Katakana = dr["カナ"].ToString() ?? string.Empty,
                                TelNo = dr["TEL"].ToString() ?? string.Empty,
                                FaxNo = dr["FAX"].ToString() ?? string.Empty,
                                SalesRepCD = dr["担当名"].ToString() ?? string.Empty,
                                StoreCate = int.TryParse(dr["店種区分"].ToString(), out var _storeCate) ? _storeCate : 0,
                                Area = long.TryParse(dr["坪数"].ToString(), out var _area) ? _area : 0,
                                CommissionRate = double.TryParse(dr["掛率"].ToString(), out var _comm) ? _comm : 0.0,
                                SaleCommRate = double.TryParse(dr["セール掛率"].ToString(), out var _scomm) ? _scomm : 0.0,
                                InStoreSaleRate = double.TryParse(dr["店頭セール掛率"].ToString(), out var _icomm) ? _icomm : 0.0,
                                InvoiceAddrCD = dr["請求先CD"].ToString() ?? string.Empty,
                                InvoicePrint = int.TryParse(dr["請求印刷"].ToString(), out var _invoicePrint) ? _invoicePrint: 0,
                                ClosingDate = int.TryParse(dr["締日"].ToString(), out var _closeDate) ? _closeDate : 0,
                                ExpectedPayMonth = int.TryParse(dr["入金予定月"].ToString(), out var _expectedPayMonth) ? _expectedPayMonth : 0,                               
                                ExpectedPayDate = int.TryParse(dr["入金予定日"].ToString(), out var _expectedPayDate) ? _expectedPayDate : 0,                               
                                PayMethod = dr["入金方法"].ToString() ?? string.Empty,
                                LowerPriceCutoffSpec = int.TryParse(dr["下代桁切指定"].ToString(), out var _lowerPriceCutoffSpec) ? _lowerPriceCutoffSpec : 0,
                                LowerPriceFracCate = int.TryParse(dr["下代端数区分"].ToString(), out var _lowerPriceFracCate) ? _lowerPriceFracCate : 0,
                                LowerPriceCalcFLG = int.TryParse(dr["下代計算FLG"].ToString(), out var _lowerPriceCalcFLG) ? _lowerPriceCalcFLG : 0,
                                ConsumpTaxCD = int.TryParse(dr["消費税CD"].ToString(), out var _consumpTaxCD) ? _consumpTaxCD : 0,
                                ConsumpTaxCalc = int.TryParse(dr["消費税計算方法"].ToString(), out var _consumpTaxCalc) ? _consumpTaxCalc : 0,
                                ConsumpTaxFrac = int.TryParse(dr["消費税端数"].ToString(), out var _consumpTaxFrac) ? _consumpTaxFrac : 0,
                                CreditLimit = int.TryParse(dr["与信限度額"].ToString(), out var _creditLimit) ? _creditLimit : 0,
                                PaymentRate = int.TryParse(dr["入金率"].ToString(), out var _paymentRate) ? _paymentRate : 0,
                                ShipmentStopFlg = int.TryParse(dr["出荷停止FLG"].ToString(), out var _shipmentStopFlg) ? _shipmentStopFlg : 0,
                                AddressName1 = dr["宛名名称1"].ToString() ?? string.Empty,
                                AddressName2 = dr["宛名名称2"].ToString() ?? string.Empty,
                                InvoiceIssueCate = int.TryParse(dr["伝票発行区分"].ToString(), out var _invoiceIssueCate) ? _invoiceIssueCate : 0,
                                Department = dr["部門"].ToString() ?? string.Empty,
                                Notes = dr["備考"].ToString() ?? string.Empty,
                                InvManageFLG = int.TryParse(dr["在庫管理FLG"].ToString(), out var _invManageFLG) ? _invManageFLG : 0,
                                AutoAllocFLG = int.TryParse(dr["自動配分FLG"].ToString(), out var _autoAllocFLG) ? _autoAllocFLG : 0,
                                StartDate = dr["開始日"].ToString() ?? string.Empty,
                                EndDate = dr["終了日"].ToString() ?? string.Empty,
                                NameCD01 = dr["名称CD01"].ToString() ?? string.Empty,
                                NameCD02 = dr["名称CD02"].ToString() ?? string.Empty,
                                NameCD03 = dr["名称CD03"].ToString() ?? string.Empty,
                                NameCD04 = dr["名称CD04"].ToString() ?? string.Empty,
                                NameCD05 = dr["名称CD05"].ToString() ?? string.Empty,
                                NameCD06 = dr["名称CD06"].ToString() ?? string.Empty,
                                NameCD07 = dr["名称CD07"].ToString() ?? string.Empty,
                                NameCD08 = dr["名称CD08"].ToString() ?? string.Empty,
                                NameCD09 = dr["名称CD09"].ToString() ?? string.Empty,
                                NameCD10 = dr["名称CD10"].ToString() ?? string.Empty,
                                InvDate = dr["棚卸日"].ToString() ?? string.Empty,
                                StartTime = dr["開始時刻"].ToString() ?? string.Empty,
                                EndTime = dr["終了時刻"].ToString() ?? string.Empty,
                                TerminalID = dr["端末ID"].ToString() ?? string.Empty,
                                WarehouseCate = int.TryParse(dr["倉庫区分"].ToString(), out var _warehouseCate) ? _warehouseCate : 0,
                                BusinHours1 = dr["営業時間1"].ToString() ?? string.Empty,
                                BusinHours2 = dr["営業時間2"].ToString() ?? string.Empty,
                                BusinHours3 = dr["営業時間3"].ToString() ?? string.Empty,
                                ContractInfo = dr["施工業者情報"].ToString() ?? string.Empty,
                                Developer = dr["デベロッパ"].ToString() ?? string.Empty,
                                BusinessHours = double.TryParse(dr["営業時間"].ToString(), out var _businessHours) ? _businessHours : 0.0,
                                InvDateEND = dr["棚卸日END"].ToString() ?? string.Empty,
                                ExchangeCate = dr["為替区分"].ToString() ?? string.Empty,
                                ExDeciCutoff = int.TryParse(dr["為替桁切指定"].ToString(), out var _exDeciCutoff) ? _exDeciCutoff : 0,
                                ExFracCate = int.TryParse(dr["為替端数区分"].ToString(), out var _exFracCate) ? _exFracCate : 0,
                                AllocRank01 = dr["配分ランク01"].ToString() ?? string.Empty,
                                AllocRank02 = dr["配分ランク02"].ToString() ?? string.Empty,
                                ShippingFLG = int.TryParse(dr["出荷FLG"].ToString(), out var _shippingFLG) ? _shippingFLG : 0,
                                BaseWarehouseFLG = int.TryParse(dr["基準倉庫FLG"].ToString(), out var _baseWarehouseFLG) ? _baseWarehouseFLG : 0,
                                BaseWarehouseCD = dr["基準倉庫CD"].ToString() ?? string.Empty,
                                AllocMethodFLG = int.TryParse(dr["配分方法FLG"].ToString(), out var _allocMethodFLG) ? _allocMethodFLG : 0,
                                PosCate = int.TryParse(dr["POS区分"].ToString(), out var _posCate) ? _posCate : 0,
                                SlipPrint5 = dr["伝票印字5"].ToString() ?? string.Empty,
                                SlipPrint6 = dr["伝票印字6"].ToString() ?? string.Empty,
                                SlipPrint7 = dr["伝票印字7"].ToString() ?? string.Empty,
                                SlipPrint8 = dr["伝票印字8"].ToString() ?? string.Empty,
                                EnterEmployeeCD = dr["最終修正者"].ToString() ?? string.Empty,
                                StoreSalesFloorCode = dr["店舗売場コード"].ToString() ?? string.Empty,
                                EcFLG = int.TryParse(dr["ECFLG"].ToString(), out var _ecFLG) ? _ecFLG : 0,
                                //BaseSales01 = long.TryParse(dr["基準売上01"].ToString(), out var _baseSales01) ? _baseSales01 : 0,
                                //BaseSales02 = long.TryParse(dr["基準売上02"].ToString(), out var _baseSales02) ? _baseSales02 : 0,
                                //BaseSales03 = long.TryParse(dr["基準売上03"].ToString(), out var _baseSales03) ? _baseSales03 : 0,
                                //CommissionRate01 = double.TryParse(dr["歩率01"].ToString(), out var _commissionRate01) ? _commissionRate01 : 0.0,
                                //CommissionRate02 = double.TryParse(dr["歩率02"].ToString(), out var _commissionRate02) ? _commissionRate02 : 0.0,
                                //CommissionRate03 = double.TryParse(dr["歩率03"].ToString(), out var _commissionRate03) ? _commissionRate03 : 0.0,
                                //OtherRatio01 = double.TryParse(dr["他比率01"].ToString(), out var _otherRatio01) ? _otherRatio01 : 0.0,
                                //OtherRatio02 = double.TryParse(dr["他比率02"].ToString(), out var _otherRatio02) ? _otherRatio02 : 0.0,
                                //OtherRatio03 = double.TryParse(dr["他比率03"].ToString(), out var _otherRatio03) ? _otherRatio03 : 0.0,
                                //OtherExpenses01 = double.TryParse(dr["他費用01"].ToString(), out var _otherExpenses01) ? _otherExpenses01 : 0.0,
                                //OtherExpenses02 = double.TryParse(dr["他費用02"].ToString(), out var _otherExpenses02) ? _otherExpenses02 : 0.0,
                                //OtherExpenses03 = double.TryParse(dr["他費用03"].ToString(), out var _otherExpenses03) ? _otherExpenses03 : 0.0,
                                NameCD11 = dr["名称CD11"].ToString() ?? string.Empty,
                                NameCD12 = dr["名称CD12"].ToString() ?? string.Empty,
                                NameCD13 = dr["名称CD13"].ToString() ?? string.Empty,
                                NameCD14 = dr["名称CD14"].ToString() ?? string.Empty,
                                NameCD15 = dr["名称CD15"].ToString() ?? string.Empty,
                                NameCD16 = dr["名称CD16"].ToString() ?? string.Empty,
                                NameCD17 = dr["名称CD17"].ToString() ?? string.Empty,
                                NameCD18 = dr["名称CD18"].ToString() ?? string.Empty,
                                NameCD19 = dr["名称CD19"].ToString() ?? string.Empty,
                                NameCD20 = dr["名称CD20"].ToString() ?? string.Empty,
                                //RentCalcFLG = int.TryParse(dr["賃料計算FLG"].ToString(), out var _rentCalcFLG) ? _rentCalcFLG : 0,
                                //MinRent = long.TryParse(dr["最低保障家賃"].ToString(), out var _minRent) ? _minRent : 0,
                                CloseDate2 = int.TryParse(dr["締日2"].ToString(), out var _closeDate2) ? _closeDate2 : 0,
                                CloseDate3 = int.TryParse(dr["締日3"].ToString(), out var _closeDate3) ? _closeDate3 : 0,
                                XpPayMon2 = int.TryParse(dr["入金予定月2"].ToString(), out var _xpPayMon2) ? _xpPayMon2 : 0,
                                XpPayDate2 = int.TryParse(dr["入金予定日2"].ToString(), out var _xpPayDate2) ? _xpPayDate2 : 0,
                                XpPayMon3 = int.TryParse(dr["入金予定月3"].ToString(), out var _xpPayMon3) ? _xpPayMon3 : 0,
                                XpPayDate3 = int.TryParse(dr["入金予定日3"].ToString(), out var _xpPayDate3) ? _xpPayDate3 : 0,
                                InvoCompName = dr["伝票社名"].ToString() ?? string.Empty,
                                InvoStoreName = dr["伝票店名"].ToString() ?? string.Empty,
                                TransCate = int.TryParse(dr["移動区分"].ToString(), out var _transCate) ? _transCate : 0,
                                CorpCD = dr["法人CD"].ToString() ?? string.Empty,
                                AffiCD = dr["連携CD"].ToString() ?? string.Empty,
                                AffiCorpCD = dr["連携先法人CD"].ToString() ?? string.Empty,
                                PayDesti1 = dr["振込先1"].ToString() ?? string.Empty,
                                PayDesti2 = dr["振込先2"].ToString() ?? string.Empty,
                                PayDesti3 = dr["振込先3"].ToString() ?? string.Empty,
                                //CustBrdCD = dr["得意先ブランドCD"].ToString() ?? string.Empty,
                                DueDate = int.TryParse(dr["期日"].ToString(), out var _dueDate) ? _dueDate : 0,
                                MinAm = int.TryParse(dr["下限額"].ToString(), out var _minAm) ? _minAm : 0,
                                //TransDays = int.TryParse(dr["移動日数"].ToString(), out var _transDays) ? _transDays : 0,
                                CustEmail = dr["得意先MAIL"].ToString() ?? string.Empty,
                                //StoreImgName = dr["店舗画像名"].ToString() ?? string.Empty,
                                //ClosedDays = dr["定休日"].ToString() ?? string.Empty,
                                //Appeal = dr["アピール"].ToString() ?? string.Empty,
                                //StatName = dr["駅名"].ToString() ?? string.Empty,
                                //Longitude = long.TryParse(dr["経度"].ToString(), out var _longitude) ? _longitude : 0,
                                //Latitude = long.TryParse(dr["緯度"].ToString(), out var _latitude) ? _latitude : 0,
                                //ECDisp = int.TryParse(dr["EC表示"].ToString(), out var _ecDisp) ? _ecDisp : 0,
                                //ECDispName = dr["EC表示名"].ToString() ?? string.Empty,
                                //Area2 = int.TryParse(dr["地域"].ToString(), out var _area2) ? _area2 : 0,
                                //Brand = dr["ブランド"].ToString() ?? string.Empty,
                                //Reservation = int.TryParse(dr["取置"].ToString(), out var _reservation) ? _reservation : 0,
                                //Order = int.TryParse(dr["取寄"].ToString(), out var _order) ? _order : 0,
                                //OrderSeq = int.TryParse(dr["取寄順"].ToString(), out var _orderseq) ? _orderseq : 0,
                                //ECBusHours = dr["EC営業時間"].ToString() ?? string.Empty,
                                //INSTA = dr["INSTA"].ToString() ?? string.Empty,
                                //WEAR = dr["WEAR"].ToString() ?? string.Empty,
                                //BLOG = dr["BLOG"].ToString() ?? string.Empty,
                                //MAPURL = dr["MAPURL"].ToString() ?? string.Empty,
                                //ECAppeal = dr["ECアピール"].ToString() ?? string.Empty,
                                //Site = dr["サイト"].ToString() ?? string.Empty,
                                //AdjustedInv = int.TryParse(dr["調整在庫"].ToString(), out var _adjustedInv) ? _adjustedInv : 0,
                                RegistNum = dr["登録番号"].ToString() ?? string.Empty,
                                //APPBusHours = dr["APP営業時間"].ToString() ?? string.Empty,
                                //OriStoreImgName = dr["元店舗画像名"].ToString() ?? string.Empty,
                                //DispOrder = int.TryParse(dr["表示順"].ToString(), out var _dispOrder) ? _dispOrder : 0,

                            }).OrderBy(c => c.TradingCD).ToList();
                ListShop = new ObservableCollection<MasterShop>(list);
                if (ListShop.Count > 0)
                {
                    SelectedShop = ListShop[0];
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        [RelayCommand]
        void DoInsert()
        {
            if (!ClientLib.MessageBox(this, "新規登録しますか？")) return;
            var item = Common.CloneObject(EditShop);
            Common.ConvertDotStringAdd(item);
            if (item == null) return;
            var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.INSERT, "MASTER_TOKUI", 0, "0",
                new string[] { "得意先CD", "得意先名", "部門", "郵便番号", "住所1","住所2","住所3","旧コード","略称","カナ",
                    "TEL","FAX","営業担当CD","店種区分","坪数","掛率","セール掛率","店頭セール掛率","請求先CD","請求印刷","締日","入金予定月","入金予定日","入金方法",
                    "下代桁切指定","下代端数区分","下代計算FLG","消費税CD","消費税計算方法","消費税端数","与信限度額","入金率","出荷停止FLG",
                "宛名名称1","宛名名称2","伝票発行区分","備考","在庫管理FLG","自動配分FLG","開始日","終了日","","",""},
                new string[] { item.TradingCD!, item.TradingName!, item.Department!, item.Postal,item.Address1!,item.Address2,item.Address3,item.OldCD,item.Abbr,item.Katakana,
                    item.TelNo,item.FaxNo,item.SalesRepCD,item.StoreCate.ToString(),item.Area.ToString(),item.CommissionRate.ToString(),item.SaleCommRate.ToString(),item.InStoreSaleRate.ToString(),
                    item.InvoiceAddrCD.ToString(),item.InvoicePrint.ToString(),item.ClosingDate.ToString(),item.ExpectedPayMonth.ToString(),item.ExpectedPayDate.ToString(),item.PayMethod,
                    item.LowerPriceCutoffSpec.ToString(),item.LowerPriceFracCate.ToString(),item.LowerPriceCalcFLG.ToString(),item.ConsumpTaxCD.ToString(),
                    item.ConsumpTaxCalc.ToString(),item.ConsumpTaxFrac.ToString(),item.CreditLimit.ToString(),item.PaymentRate.ToString(),item.ShipmentStopFlg.ToString(),
                    item.AddressName1,item.AddressName2,item.InvoiceIssueCate.ToString(),item.Notes,item.InvManageFLG.ToString(),item.AutoAllocFLG.ToString(),
                    item.StartDate,item.EndDate,item.NameCD01,item.NameCD02,item.NameCD03,item.NameCD04,item.NameCD05,item.NameCD06,item.NameCD07,item.NameCD08,item.NameCD09,item.NameCD10,
                    item.InvDate,item.StartTime,item.EndTime,item.TerminalID,item.WarehouseCate.ToString(),item.BusinHours1,item.BusinHours2,item.BusinHours3,item.ContractInfo,item.Developer,
                    item.BusinessHours.ToString(),item.InvDateEND,item.ExchangeCate,item.ExDeciCutoff.ToString(),item.ExFracCate.ToString(),item.AllocRank01,item.AllocRank02,item.ShippingFLG.ToString(),
                    item.BaseWarehouseFLG.ToString(),item.BaseWarehouseCD,item.AllocMethodFLG.ToString(),item.PosCate.ToString(),item.SlipPrint5,item.SlipPrint6,item.SlipPrint7,item.SlipPrint8,
                    item.EnterEmployeeCD,item.StoreSalesFloorCode,item.EcFLG.ToString(),item.NameCD11,item.NameCD12,item.NameCD13,item.NameCD14,item.NameCD15,item.NameCD16,item.NameCD17,
                    item.NameCD18,item.NameCD19,item.NameCD20,item.CloseDate2.ToString(),item.CloseDate3.ToString(),item.XpPayMon2.ToString(),item.XpPayDate2.ToString(),item.XpPayMon3.ToString(),
                    item.XpPayDate3.ToString(),item.InvoCompName,item.InvoStoreName,item.TransCate.ToString(),item.CorpCD,item.AffiCD,item.AffiCorpCD,item.PayDesti1,item.PayDesti2,
                    item.PayDesti3,item.DueDate.ToString(),item.MinAm.ToString(),item.CustEmail,item.RegistNum});
            if (ret.Code == 0)
            {
                item.SeqNo = ret.NewSeq;
                item.VdateUpdate = decimal.Parse(ret.VDate);
                item.VdateCreate = item.VdateUpdate;
                Common.ConvertDotStringDel(item);
                ListShop!.Add(item);
                SelectedShop = item;
            }
            else
            {
                ClientLib.MessageBoxError(this, ret.Code.ToString());
            }
        }

        [RelayCommand]
        void DoUpdate()
        {
            if (!ClientLib.MessageBox(this, "修正しますか？")) return;
            var item = Common.CloneObject(EditShop);
            Common.ConvertDotStringAdd(item);
            if (item == null) return;
            var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.UPDATE, "HASTER_TOKUI", item.SeqNo, item.VdateUpdate.ToString(),
                new string[] { "得意先CD","得意先名" },
                new string[] { item.TradingCD!, item.TradingName!, item.Department!, item.Postal,item.Address1!,item.Address2,item.Address3,item.OldCD,item.Abbr,item.Katakana,
                    item.TelNo,item.FaxNo,item.SalesRepCD,item.StoreCate.ToString(),item.Area.ToString(),item.CommissionRate.ToString(),item.SaleCommRate.ToString(),item.InStoreSaleRate.ToString(),
                    item.InvoiceAddrCD.ToString(),item.InvoicePrint.ToString(),item.ClosingDate.ToString(),item.ExpectedPayMonth.ToString(),item.ExpectedPayDate.ToString(),item.PayMethod,
                    item.LowerPriceCutoffSpec.ToString(),item.LowerPriceFracCate.ToString(),item.LowerPriceCalcFLG.ToString(),item.ConsumpTaxCD.ToString(),
                    item.ConsumpTaxCalc.ToString(),item.ConsumpTaxFrac.ToString(),item.CreditLimit.ToString(),item.PaymentRate.ToString(),item.ShipmentStopFlg.ToString(),
                    item.AddressName1,item.AddressName2,item.InvoiceIssueCate.ToString(),item.Department,item.Notes,item.InvManageFLG.ToString(),item.AutoAllocFLG.ToString(),
                    item.StartDate,item.EndDate,item.NameCD01,item.NameCD02,item.NameCD03,item.NameCD04,item.NameCD05,item.NameCD06,item.NameCD07,item.NameCD08,item.NameCD09,item.NameCD10,
                    item.InvDate,item.StartTime,item.EndTime,item.TerminalID,item.WarehouseCate.ToString(),item.BusinHours1,item.BusinHours2,item.BusinHours3,item.ContractInfo,item.Developer,
                    item.BusinessHours.ToString(),item.InvDateEND,item.ExchangeCate,item.ExDeciCutoff.ToString(),item.ExFracCate.ToString(),item.AllocRank01,item.AllocRank02,item.ShippingFLG.ToString(),
                    item.BaseWarehouseFLG.ToString(),item.BaseWarehouseCD,item.AllocMethodFLG.ToString(),item.PosCate.ToString(),item.SlipPrint5,item.SlipPrint6,item.SlipPrint7,item.SlipPrint8,
                    item.EnterEmployeeCD,item.StoreSalesFloorCode,item.EcFLG.ToString(),item.NameCD11,item.NameCD12,item.NameCD13,item.NameCD14,item.NameCD15,item.NameCD16,item.NameCD17,
                    item.NameCD18,item.NameCD19,item.NameCD20,item.CloseDate2.ToString(),item.CloseDate3.ToString(),item.XpPayMon2.ToString(),item.XpPayDate2.ToString(),item.XpPayMon3.ToString(),
                    item.XpPayDate3.ToString(),item.InvoCompName,item.InvoStoreName,item.TransCate.ToString(),item.CorpCD,item.AffiCD,item.AffiCorpCD,item.PayDesti1,item.PayDesti2,
                    item.PayDesti3,item.DueDate.ToString(),item.MinAm.ToString(),item.CustEmail,item.RegistNum });
            if (ret.Code == 0)
            {
                Common.ConvertDotStringDel(item);
                if (SelectedShop != null)
                {
                    SelectedShop.VdateUpdate = decimal.Parse(ret.VDate);
                    SelectedShop.TradingCD = item.TradingCD;
                    SelectedShop.TradingName = item.TradingName;
                    SelectedShop.Abbr = item.Abbr;
                    SelectedShop.Postal = item.Postal;
                    SelectedShop.Address1 = item.Address1;
                    SelectedShop.Address2 = item.Address2;
                    SelectedShop.Address3 = item.Address3;
                    SelectedShop.OldCD = item.OldCD;
                    SelectedShop.Katakana = item.Katakana;
                    SelectedShop.TelNo = item.TelNo;
                    SelectedShop.FaxNo = item.FaxNo;
                    SelectedShop.SalesRepCD = item.SalesRepCD;
                    SelectedShop.StoreCate = item.StoreCate;
                    SelectedShop.Area = item.Area;
                    SelectedShop.CommissionRate = item.CommissionRate;
                    SelectedShop.SaleCommRate = item.SaleCommRate;
                    SelectedShop.InStoreSaleRate = item.InStoreSaleRate;
                    SelectedShop.InvoiceAddrCD = item.InvoiceAddrCD;
                    SelectedShop.InvoicePrint = item.InvoicePrint;
                    SelectedShop.ClosingDate = item.ClosingDate;
                    SelectedShop.ExpectedPayMonth = item.ExpectedPayMonth;
                    SelectedShop.ExpectedPayDate = item.ExpectedPayDate;
                    SelectedShop.PayMethod = item.PayMethod;
                    SelectedShop.LowerPriceCutoffSpec = item.LowerPriceCutoffSpec;
                    SelectedShop.LowerPriceFracCate = item.LowerPriceFracCate;
                    SelectedShop.LowerPriceCalcFLG = item.LowerPriceCalcFLG;
                    SelectedShop.ConsumpTaxCD = item.ConsumpTaxCD;
                    SelectedShop.ConsumpTaxCalc = item.ConsumpTaxCalc;
                    SelectedShop.ConsumpTaxFrac = item.ConsumpTaxFrac;
                    SelectedShop.CreditLimit = item.CreditLimit;
                    SelectedShop.PaymentRate = item.PaymentRate;
                    SelectedShop.ShipmentStopFlg = item.ShipmentStopFlg;
                    SelectedShop.AddressName1 = item.AddressName1;
                    SelectedShop.AddressName2 = item.AddressName2;
                    SelectedShop.InvoiceIssueCate = item.InvoiceIssueCate;
                    SelectedShop.Department = item.Department;
                    SelectedShop.Notes = item.Notes;
                    SelectedShop.InvManageFLG = item.InvManageFLG;
                    SelectedShop.AutoAllocFLG = item.AutoAllocFLG;
                    SelectedShop.StartDate = item.StartDate;
                    SelectedShop.EndDate = item.EndDate;
                    SelectedShop.NameCD01 = item.NameCD01;
                    SelectedShop.NameCD02 = item.NameCD02;
                    SelectedShop.NameCD03 = item.NameCD03;
                    SelectedShop.NameCD04 = item.NameCD04;
                    SelectedShop.NameCD05 = item.NameCD05;
                    SelectedShop.NameCD06 = item.NameCD06;
                    SelectedShop.NameCD07 = item.NameCD07;
                    SelectedShop.NameCD08 = item.NameCD08;
                    SelectedShop.NameCD09 = item.NameCD09;
                    SelectedShop.NameCD10 = item.NameCD10;
                    SelectedShop.InvDate = item.InvDate;
                    SelectedShop.StartTime = item.StartTime;
                    SelectedShop.EndTime = item.EndTime;
                    SelectedShop.TerminalID = item.TerminalID;
                    SelectedShop.WarehouseCate = item.WarehouseCate;
                    SelectedShop.BusinHours1 = item.BusinHours1;
                    SelectedShop.BusinHours2 = item.BusinHours2;
                    SelectedShop.BusinHours3 = item.BusinHours3;
                    SelectedShop.ContractInfo = item.ContractInfo;
                    SelectedShop.Developer = item.Developer;
                    SelectedShop.BusinessHours = item.BusinessHours;
                    SelectedShop.InvDateEND = item.InvDateEND;
                    SelectedShop.ExchangeCate = item.ExchangeCate;
                    SelectedShop.ExDeciCutoff = item.ExDeciCutoff;
                    SelectedShop.ExFracCate = item.ExFracCate;
                    SelectedShop.AllocRank01 = item.AllocRank01;
                    SelectedShop.AllocRank02 = item.AllocRank02;
                    SelectedShop.ShippingFLG = item.ShippingFLG;
                    SelectedShop.BaseWarehouseFLG = item.BaseWarehouseFLG;
                    SelectedShop.BaseWarehouseCD = item.BaseWarehouseCD;
                    SelectedShop.AllocMethodFLG = item.AllocMethodFLG;
                    SelectedShop.PosCate = item.PosCate;
                    SelectedShop.SlipPrint5 = item.SlipPrint5;
                    SelectedShop.SlipPrint6 = item.SlipPrint6;
                    SelectedShop.SlipPrint7 = item.SlipPrint7;
                    SelectedShop.SlipPrint8 = item.SlipPrint8;
                    SelectedShop.EnterEmployeeCD = item.EnterEmployeeCD;
                    SelectedShop.StoreSalesFloorCode = item.StoreSalesFloorCode;
                    SelectedShop.EcFLG = item.EcFLG;
                    SelectedShop.NameCD11 = item.NameCD11;
                    SelectedShop.NameCD12 = item.NameCD12;
                    SelectedShop.NameCD13 = item.NameCD13;
                    SelectedShop.NameCD14 = item.NameCD14;
                    SelectedShop.NameCD15 = item.NameCD15;
                    SelectedShop.NameCD16 = item.NameCD16;
                    SelectedShop.NameCD17 = item.NameCD17;
                    SelectedShop.NameCD18 = item.NameCD18;
                    SelectedShop.NameCD19 = item.NameCD19;
                    SelectedShop.NameCD20 = item.NameCD20;
                    SelectedShop.CloseDate2 = item.CloseDate2;
                    SelectedShop.CloseDate3 = item.CloseDate3;
                    SelectedShop.XpPayMon2 = item.XpPayMon2;
                    SelectedShop.XpPayDate2 = item.XpPayDate2;
                    SelectedShop.XpPayMon3 = item.XpPayMon3;
                    SelectedShop.XpPayDate3 = item.XpPayDate3;
                    SelectedShop.InvoCompName = item.InvoCompName;
                    SelectedShop.InvoStoreName = item.InvoStoreName;
                    SelectedShop.TransCate = item.TransCate;
                    SelectedShop.CorpCD = item.CorpCD;
                    SelectedShop.AffiCD = item.AffiCD;
                    SelectedShop.AffiCorpCD = item.AffiCorpCD;
                    SelectedShop.PayDesti1 = item.PayDesti1;
                    SelectedShop.PayDesti2 = item.PayDesti2;
                    SelectedShop.PayDesti3 = item.PayDesti3;
                    SelectedShop.DueDate = item.DueDate;
                    SelectedShop.MinAm = item.MinAm;
                    SelectedShop.CustEmail = item.CustEmail;
                    SelectedShop.RegistNum = item.RegistNum;
                    EditShop = Common.CloneObject(SelectedShop);
                }

            }
            else
            {
                ClientLib.MessageBoxError(this, ret.Code.ToString());
            }
        }

        [RelayCommand]
        void DoDelete()
        {
            if (!ClientLib.MessageBox(this, "削除しますか？")) return;
            if (EditShop == null) return;
            var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.DELETE, "Master_TOKUI", EditShop.SeqNo, EditShop.VdateUpdate.ToString(),
                new string[0], new string[0]);
            if (ret.Code == 0)
            {
                if (SelectedShop != null)
                {
                    ListShop!.Remove(SelectedShop);
                    var item = ListShop.Where(c => c.TradingCD == ListShop.Min(c => c.TradingCD)).FirstOrDefault();
                    SelectedShop = item;
                }
            }
            else
            {
                ClientLib.MessageBoxError(this, ret.Code.ToString());
            }
        }

        [RelayCommand]
        async Task DoPrintAsync()
        {
            if (!ClientLib.MessageBox(this, "印刷しますか？")) return;
            ClientLib.CursorToWait();
            if (ListShop == null || ListShop.Count == 0) return;
            var paramNames = ListShop.Select((c, i) => $":p{i}||''").ToList();
            // Build parameter values
            var parameters = ListShop.Select(c => c.TradingCD).ToArray();
            var joined = string.Join(",", parameters.Select(p => $"'{p}'"));
            string[] param = new string[1];
            param[0] = joined;
            var ret = AppData.ClassCvnet.OnQueryPrintTokui(param, 0);

            if (ret.Split('\n').Length < 2)
            {
                ClientLib.MessageBoxError(this, "PDFデータがありません");
                return;
            }
            var ret1 = ret.Split('\n');
            var url = AppData.Http.URLroot + ret1[0] + "/data.pdf";
            await Task.Delay(1500); // PDF生成待ち
            var win = new WebpdfView();
            var vm = win.DataContext as WebpdfViewModel;
            if (vm == null) return;
            vm.Pdfdata = url;
            ClientLib.CursorToNormal();
            ClientLib.ShowDialogView(win, this);
        }
    }
}
