using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01TokViewModel : BaseViewModel
    {
        #region Declaration
        [ObservableProperty]
        ObservableCollection<MasterShop>? listShop;
        [ObservableProperty]
        MasterShop? selectedShop;
        [ObservableProperty]
        MasterShop? editShop;
        [ObservableProperty]
        private string day1;
        [ObservableProperty]
        private string day2;
        [ObservableProperty]
        private string day3;
        [ObservableProperty]
        private string day4;
        [ObservableProperty]
        private string day5;
        [ObservableProperty]
        private string day6;
        [ObservableProperty]
        private string hour1;
        [ObservableProperty]
        private string hour2;
        [ObservableProperty]
        private string min1;
        [ObservableProperty]
        private string min2;
        [ObservableProperty]
        private string sec1;
        [ObservableProperty]
        private string sec2;
        [ObservableProperty]
        string? selShopCD;
        [ObservableProperty]
        int? pageNow;
        [ObservableProperty]
        int? pageTotal;
        private BizArray col_list;
        [ObservableProperty]
        string? startCode;
        [ObservableProperty]
        Visibility showJido;
        [ObservableProperty]
        int? selectedJido;
        [ObservableProperty]
        Visibility show1;
        [ObservableProperty]
        Visibility show2;
        [ObservableProperty]
        Visibility show3;
        [ObservableProperty]
        int? selectedInvoice;
        #region ComboBox
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
        public Dictionary<int, string> comboListNohin;
        [ObservableProperty]
        public Dictionary<int, string> comboListStdWareFLG;
        [ObservableProperty]
        public Dictionary<int, string> comboListAllocMethodFLG;
        [ObservableProperty]
        public Dictionary<int, string> comboListShippingFLG;
        #endregion 
        private string sql_collist = """
               得意先CD,得意先名,カナ,旧コード,略称,郵便番号,住所1,住所2,住所3,TEL,FAX,
        宛名FLG1,宛名FLG2,宛名FLG3,宛名名称1,宛名名称2,営業担当CD,店種区分,坪数,在庫管理FLG,
        掛率,セール掛率,店頭セール掛率,請求先CD,請求印刷,締日,入金予定月,入金予定日,入金方法,下代桁切指定,
        下代端数区分,下代計算FLG,消費税CD,消費税計算方法,消費税端数,与信限度額,入金率,出荷停止FLG,
        伝票発行区分,備考,伝票印字1,伝票印字2,伝票印字3,伝票印字4
        ,倉庫区分,営業時間1,営業時間2,営業時間3,施工業者情報,デベロッパ,開始時刻,終了時刻,端末ID,営業時間,棚卸日END
        ,部門,為替区分,為替桁切指定,為替端数区分,名称CD01,名称CD02,名称CD03,名称CD04,名称CD05,名称CD06
        ,名称CD07,名称CD08,名称CD09,名称CD10,配分ランク01,配分ランク02
        ,基準倉庫FLG,基準倉庫CD,出荷FLG,POS区分,配分方法FLG,伝票印字5,伝票印字6,伝票印字7,伝票印字8
        ,店舗売場コード,ECFLG,締日2,締日3,入金予定月2,入金予定日2,入金予定月3,入金予定日3
        ,伝票社名,伝票店名,移動区分,法人CD,連携先法人CD,連携CD,振込先1,振込先2,振込先3,期日,下限額
        ,名称CD11,名称CD12,名称CD13,名称CD14,名称CD15,名称CD16,名称CD17,名称CD18,名称CD19,名称CD20
        ,得意先MAIL,登録番号,開始日,終了日,棚卸日,自動配分FLG,APP営業時間,元店舗画像名,表示順
        """;
        private string sql_list = "";
        #endregion
        #region Initialization
        public void OnInit(object? init_para = null, string? init_flg = null) 
        {
            OnInitBase(init_para, init_flg);
            SelectedShop = new MasterShop();
            EditShop = new MasterShop();
            SelectedJido = 0;
            SelectedInvoice = 0;
            ShowJido = Visibility.Hidden;
            Show1 = Visibility.Hidden;
            Show2 = Visibility.Hidden;
            Show3 = Visibility.Hidden;
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
            ComboListNohin = new Dictionary<int, string> 
            {
                { 0,"0 印刷しない" },
                { 1,"1 自社伝票" },
                { 2,"2 百貨店伝票" },
                { 3,"3 チェーンストア統一伝票1型" },
                { 4,"4 チェーンストア統一伝票" },
                { 5,"5 百貨店伝票（丸井用）" },
                { 6,"6 チェーンストア統一伝票（ターンアラウンド用2型）" },
                { 7,"7 チェーンストア統一伝票（ターンアラウンド用1型）" },
                { 8,"8 百貨店伝票Ⅱ型" }
            };
            ComboListStdWareFLG = new Dictionary<int, string>
            {
                { 0,"0 商品マスタの基準倉庫" },
                { 1,"1 得意先マスタの基準倉庫" },                
            };
            ComboListAllocMethodFLG = new Dictionary<int, string>
            {
                { 0,"0 売上順" },
                { 1,"1 ランク順" },
                { 2,"2 均等配分" },
                { 3,"3 在庫無視" },
            };                                 
            ComboListShippingFLG = new Dictionary<int, string>
            {
                { 0,"0 出荷予定" },
                { 1,"1 出荷確定" },
            };            
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
                SelectedJido = EditShop.AllocMethodFLG;
                SelectedInvoice = EditShop.InvoiceIssueCate;
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
        partial void OnSelectedJidoChanged(int? value)
        {
            if (value != null)
            {
                if (value == 0)
                {
                    ShowJido = Visibility.Hidden;
                }
                else if (value == 1)
                {
                    ShowJido = Visibility.Visible;
                }
            }
            else
            {
                ShowJido = Visibility.Hidden;
            }
            EditShop.AllocMethodFLG = value;
        }
        partial void OnSelectedInvoiceChanged(int? value) 
        { 
            if(value != null)
            {
                if (value == 0 || value == 1)
                {
                    Show1 = Visibility.Hidden;
                    Show2 = Visibility.Hidden;
                    Show3 = Visibility.Hidden;
                }
                else if (value == 2 || value == 8) 
                {
                    Show1 = Visibility.Visible;
                    Show2 = Visibility.Visible;
                    Show3 = Visibility.Visible;
                }
                else if (value == 3 || value == 4 || value == 6 || value == 7)
                {
                    Show1 = Visibility.Visible;
                    Show2 = Visibility.Hidden;
                    Show3 = Visibility.Hidden;
                }
                else if (value == 5)
                {
                    Show1 = Visibility.Visible;
                    Show2 = Visibility.Visible;
                    Show3 = Visibility.Hidden;
                }
            }
            else
            {
                Show1 = Visibility.Hidden;
                Show2 = Visibility.Hidden;
                Show3 = Visibility.Hidden;
            }
            EditShop.InvoiceIssueCate = value;
        }
        #endregion
        #region ComboBox Function
        [RelayCommand]
        public void SelDept(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditShop != null)
            {
                EditShop.Department = get_sel00.Code;
                EditShop.DepartmentName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelSalesRep(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditShop != null)
            {
                EditShop.SalesRepCD = get_sel00.Code;
                EditShop.SalesRepName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelBaseWare(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditShop != null)
            {
                EditShop.BaseWarehouseCD = get_sel00.Code;
                EditShop.BaseWarehouseName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelInvoice(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditShop != null)
            {
                EditShop.InvoiceAddrCD = get_sel00.Code;
                EditShop.InvoiceAddrName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelName((object Result1, object Result2) value)
        {
            var (result1, result2) = value;
            var get_sel00 = (SelValueModel)result1;
            string kubun = string.Empty;
            if (result2 is string res2) kubun = res2;

            if (get_sel00 != null && EditShop != null)
            {
                if (kubun == "C01")
                {
                    EditShop.NameCD01 = get_sel00.Code;
                    EditShop.NameCD01Name = get_sel00.Name;
                }
                if (kubun == "C02")
                {
                    EditShop.NameCD02 = get_sel00.Code;
                    EditShop.NameCD02Name = get_sel00.Name;
                }
                if (kubun == "C03")
                {
                    EditShop.NameCD03 = get_sel00.Code;
                    EditShop.NameCD03Name = get_sel00.Name;
                }
                if (kubun == "C04")
                {
                    EditShop.NameCD04 = get_sel00.Code;
                    EditShop.NameCD04Name = get_sel00.Name;
                }
                if (kubun == "C05")
                {
                    EditShop.NameCD05 = get_sel00.Code;
                    EditShop.NameCD05Name = get_sel00.Name;
                }
                if (kubun == "C06")
                {
                    EditShop.NameCD06 = get_sel00.Code;
                    EditShop.NameCD06Name = get_sel00.Name;
                }
                if (kubun == "C07")
                {
                    EditShop.NameCD07 = get_sel00.Code;
                    EditShop.NameCD07Name = get_sel00.Name;
                }
                if (kubun == "C08")
                {
                    EditShop.NameCD08 = get_sel00.Code;
                    EditShop.NameCD08Name = get_sel00.Name;
                }
                if (kubun == "C09")
                {
                    EditShop.NameCD09 = get_sel00.Code;
                    EditShop.NameCD09Name = get_sel00.Name;
                }
                if (kubun == "C10")
                {
                    EditShop.NameCD10 = get_sel00.Code;
                    EditShop.NameCD10Name = get_sel00.Name;
                }
                if (kubun == "C11")
                {
                    EditShop.NameCD11 = get_sel00.Code;
                    EditShop.NameCD11Name = get_sel00.Name;
                }
                if (kubun == "C12")
                {
                    EditShop.NameCD12 = get_sel00.Code;
                    EditShop.NameCD12Name = get_sel00.Name;
                }
                if (kubun == "C13")
                {
                    EditShop.NameCD13 = get_sel00.Code;
                    EditShop.NameCD13Name = get_sel00.Name;
                }
                if (kubun == "C14")
                {
                    EditShop.NameCD14 = get_sel00.Code;
                    EditShop.NameCD14Name = get_sel00.Name;
                }
                if (kubun == "C15")
                {
                    EditShop.NameCD15 = get_sel00.Code;
                    EditShop.NameCD15Name = get_sel00.Name;
                }
                if (kubun == "C16")
                {
                    EditShop.NameCD16 = get_sel00.Code;
                    EditShop.NameCD16Name = get_sel00.Name;
                }
                if (kubun == "C17")
                {
                    EditShop.NameCD17 = get_sel00.Code;
                    EditShop.NameCD17Name = get_sel00.Name;
                }
                if (kubun == "C18")
                {
                    EditShop.NameCD18 = get_sel00.Code;
                    EditShop.NameCD18Name = get_sel00.Name;
                }
                if (kubun == "C19")
                {
                    EditShop.NameCD19 = get_sel00.Code;
                    EditShop.NameCD19Name = get_sel00.Name;
                }
                if (kubun == "C20")
                {
                    EditShop.NameCD20 = get_sel00.Code;
                    EditShop.NameCD20Name = get_sel00.Name;
                }
            }
        }
        #endregion
        #region Data Acquisition
        [RelayCommand]
        void DoList() 
        {
            if (AppData.ClassCvnet.MstDialog.ContainsKey("得意先") && AppData.ClassCvnet.ComboListFLg == 1)
            {
                var ar = new string[] { "A" };
                var vm = AppData.DlgService.GetSelTok(AppData.ClassCvnet.MstDialog["得意先"].v_mstname, null, ar);
                if (vm != null)
                {
                    OnQuery(vm.SelTokResult1.Item2.ToArray(), vm.SelTokResult1.Item1, null);
                }
                return;
            }
            var v_para = new string[] { StartCode };
        }                          
        [RelayCommand]
        void BackList()
        {
            if (ListShop != null && ListShop.Count > 0)
            {
                StartCode = ListShop.Min(c => c.TradingCD);
            }
            else
            {
                DoList();
            }
            OnQuery(null, null, "<=");
            if (ListShop == null || ListShop.Count == 0)
                ClientLib.MessageBoxOk(this, "データがありません");
        }
        [RelayCommand]
        void NextList()
        {
            if (ListShop != null && ListShop.Count > 0)
            {
                StartCode = ListShop.Max(c => c.TradingCD);
            }
            else
            {
                DoList();
            }
            OnQuery(null, null, null);
            if (ListShop == null || ListShop.Count == 0)
                ClientLib.MessageBoxOk(this, "データがありません");
        }
        private void OnQuery(string[]? v_para = null, string? qs = null, string? p_sort = null)
        {
            if (qs != null)
            {
                qs += " order by 得意先CD";
                var ret_Data = AppData.Http!.AspxSqlQuery(qs, v_para);

                qs = string.Join(",",
                    ret_Data.AsEnumerable()
                           .Take(40)
                           .Select(r => $"'{r[0]}'"));
            }

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

            sql_query += ",(A.入力社員CD ||' '|| (select B.名前 from HC$MASTER_SHAIN B where B.社員CD=A.入力社員CD)) 最終修正者";

            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='BMN' and H.名称CD=A.部門),'.') 部門名";
            sql_query += ",NVL((select T.得意先名 from HC$MASTER_TOKUI T where T.得意先CD=A.基準倉庫CD),'.') 基準倉庫名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='HJN' and H.名称CD=A.法人CD),'.') 法人名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='HJN' and H.名称CD=A.連携先法人CD),'.') 連携先法人名";

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

            sql_query += " from HC$Master_TOKUI A";

            if (qs == null || qs == "") sql_query += " where A.得意先CD >= :1";
            else sql_query += " where A.得意先CD in (" + qs + ")";

            if (StartCode != null)
                sql_query += " and A.得意先CD >= '" + StartCode + "' ";
            sql_query += " order by A.得意先CD asc ";

            sql_query = "select * from (" + sql_query + ") where rownum <= " + AppData.maxQueryCnt;

            var retData = AppData.Http?.AspxSqlQuery(sql_query, v_para.ToArray());
            if (retData == null || retData.Rows.Count == 0) return;
            try
            {
                var list = (from DataRow dr in retData.Rows
                            select new MasterShop
                            {
                                SeqNo = long.TryParse(dr["SEQ_NO"].ToString(), out var _seqNo) ? _seqNo : 0,
                                VdateCreate = decimal.TryParse(dr["VDATE_CREATE"].ToString(), out var _dateC) ? _dateC : 0,
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
                                InvoiceAddrName = dr["請求名"].ToString() ?? string.Empty,
                                InvoicePrint = int.TryParse(dr["請求印刷"].ToString(), out var _invoicePrint) ? _invoicePrint : 0,
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
                                DepartmentName = dr["部門名"].ToString() ?? string.Empty,
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
                                NameCD01Name = dr["補足01名"].ToString() ?? string.Empty,
                                NameCD02Name = dr["補足02名"].ToString() ?? string.Empty,
                                NameCD03Name = dr["補足03名"].ToString() ?? string.Empty,
                                NameCD04Name = dr["補足04名"].ToString() ?? string.Empty,
                                NameCD05Name = dr["補足05名"].ToString() ?? string.Empty,
                                NameCD06Name = dr["補足06名"].ToString() ?? string.Empty,
                                NameCD07Name = dr["補足07名"].ToString() ?? string.Empty,
                                NameCD08Name = dr["補足08名"].ToString() ?? string.Empty,
                                NameCD09Name = dr["補足09名"].ToString() ?? string.Empty,
                                NameCD10Name = dr["補足10名"].ToString() ?? string.Empty,
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
                                BaseWarehouseName = dr["基準倉庫名"].ToString() ?? string.Empty,
                                AllocMethodFLG = int.TryParse(dr["配分方法FLG"].ToString(), out var _allocMethodFLG) ? _allocMethodFLG : 0,
                                PosCate = int.TryParse(dr["POS区分"].ToString(), out var _posCate) ? _posCate : 0,
                                SlipPrint5 = dr["伝票印字5"].ToString() ?? string.Empty,
                                SlipPrint6 = dr["伝票印字6"].ToString() ?? string.Empty,
                                SlipPrint7 = dr["伝票印字7"].ToString() ?? string.Empty,
                                SlipPrint8 = dr["伝票印字8"].ToString() ?? string.Empty,
                                EnterEmployeeCD = dr["最終修正者"].ToString() ?? string.Empty,
                                StoreSalesFloorCode = dr["店舗売場コード"].ToString() ?? string.Empty,
                                EcFLG = int.TryParse(dr["ECFLG"].ToString(), out var _ecFLG) ? _ecFLG : 0,
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
                                NameCD11Name = dr["補足11名"].ToString() ?? string.Empty,
                                NameCD12Name = dr["補足12名"].ToString() ?? string.Empty,
                                NameCD13Name = dr["補足13名"].ToString() ?? string.Empty,
                                NameCD14Name = dr["補足14名"].ToString() ?? string.Empty,
                                NameCD15Name = dr["補足15名"].ToString() ?? string.Empty,
                                NameCD16Name = dr["補足16名"].ToString() ?? string.Empty,
                                NameCD17Name = dr["補足17名"].ToString() ?? string.Empty,
                                NameCD18Name = dr["補足18名"].ToString() ?? string.Empty,
                                NameCD19Name = dr["補足19名"].ToString() ?? string.Empty,
                                NameCD20Name = dr["補足20名"].ToString() ?? string.Empty,
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
                                DueDate = int.TryParse(dr["期日"].ToString(), out var _dueDate) ? _dueDate : 0,
                                MinAm = int.TryParse(dr["下限額"].ToString(), out var _minAm) ? _minAm : 0,
                                CustEmail = dr["得意先MAIL"].ToString() ?? string.Empty,
                                RegistNum = dr["登録番号"].ToString() ?? string.Empty,
                                APPBusHours = dr["APP営業時間"].ToString() ?? string.Empty,
                                OriStoreImgName = dr["元店舗画像名"].ToString() ?? string.Empty,
                                DispOrder = int.TryParse(dr["表示順"].ToString(), out var _dispOrder) ? _dispOrder : 0,

                            }).OrderBy(c => c.TradingCD).ToList();
                ListShop = new ObservableCollection<MasterShop>(list);
                if (ListShop.Count > 0)
                {
                    SelectedShop = ListShop[0];
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
        #endregion
        #region CRUD
        [RelayCommand]
        void DoInsert()
        {
            if (!ClientLib.MessageBox(this, "新規登録しますか？")) return;
            var item = Common.CloneObject(EditShop);
            Common.ConvertDotStringAdd(item);
            if (item == null) return;
            var columns = new List<string> {"得意先CD", "得意先名", "部門", "郵便番号", "住所1","住所2","住所3","旧コード","略称","カナ",
                    "TEL","FAX","営業担当CD","店種区分","坪数","掛率","セール掛率","店頭セール掛率","請求先CD","請求印刷","締日","入金予定月","入金予定日","入金方法",
                    "下代桁切指定","下代端数区分","下代計算FLG","消費税CD","消費税計算方法","消費税端数","与信限度額","入金率","出荷停止FLG",
                    "宛名名称1","宛名名称2","伝票発行区分","備考","在庫管理FLG","自動配分FLG","開始日","終了日","名称CD01","名称CD02","名称CD03","名称CD04","名称CD05","名称CD06","名称CD07","名称CD08","名称CD09","名称CD10",
                    "棚卸日","開始時刻","終了時刻","端末ID","倉庫区分","営業時間1","営業時間2","営業時間3","施工業者情報","デベロッパ","営業時間","棚卸日END","為替区分","為替桁切指定","為替端数区分","配分ランク01","配分ランク02","出荷FLG",
                    "基準倉庫FLG","基準倉庫CD","配分方法FLG","POS区分","伝票印字5","伝票印字6","伝票印字7","伝票印字8","入力社員CD","店舗売場コード","ECFLG","名称CD11","名称CD12","名称CD13","名称CD14","名称CD15","名称CD16","名称CD17",
                    "名称CD18","名称CD19","名称CD20","締日2","締日3","入金予定月2","入金予定日2","入金予定月3","入金予定日3","伝票社名","伝票店名","移動区分","法人CD","連携CD","連携先法人CD","振込先1","振込先2",
                    "振込先3","期日","下限額","得意先MAIL","登録番号","振込先1","振込先2","振込先1" };
            var values = new List<string> { item.TradingCD!, item.TradingName!, item.Department!, item.Postal,item.Address1!,item.Address2,item.Address3,item.OldCD,item.Abbr,item.Katakana,
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
                    item.PayDesti3,item.DueDate.ToString(),item.MinAm.ToString(),item.CustEmail,item.RegistNum,item.PayDesti1,item.PayDesti2,item.PayDesti3};

            if (ShowJido == Visibility.Visible) 
            {
                columns.Add("基準倉庫FLG");
                values.Add(item.BaseWarehouseFLG.ToString());

                columns.Add("基準倉庫CD");
                values.Add(item.BaseWarehouseCD.ToString());

                columns.Add("配分方法FLG");
                values.Add(item.AllocMethodFLG.ToString());

                columns.Add("出荷FLG");
                values.Add(item.ShippingFLG.ToString());
            }

            if (Show1 == Visibility.Visible)
            {
                columns.Add("伝票印字1");
                values.Add(item.InvoicePrint1.ToString());
                columns.Add("伝票印字2");
                values.Add(item.InvoicePrint2.ToString());
                columns.Add("伝票印字3");
                values.Add(item.InvoicePrint3.ToString());
                columns.Add("伝票印字4");
                values.Add(item.InvoicePrint4.ToString());               

                columns.Add("伝票社名");
                values.Add(item.InvoCompName.ToString());
                columns.Add("伝票店名");
                values.Add(item.InvoStoreName.ToString());
            }
            if (Show2 == Visibility.Visible)
            {
                columns.Add("伝票印字5");
                values.Add(item.SlipPrint5.ToString());
            }
            if (Show3 == Visibility.Visible)
            {
                columns.Add("伝票印字6");
                values.Add(item.SlipPrint6.ToString());
                columns.Add("伝票印字7");
                values.Add(item.SlipPrint7.ToString());
                columns.Add("伝票印字8");
                values.Add(item.SlipPrint8.ToString());
            }

            var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.INSERT, "MASTER_TOKUI", 0, "0",columns.ToArray(),values.ToArray());
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
            var columns = new List<string> { "得意先CD", "得意先名", "部門", "郵便番号", "住所1","住所2","住所3","旧コード","略称","カナ",
                    "TEL","FAX","営業担当CD","店種区分","坪数","掛率","セール掛率","店頭セール掛率","請求先CD","請求印刷","締日","入金予定月","入金予定日","入金方法",
                    "下代桁切指定","下代端数区分","下代計算FLG","消費税CD","消費税計算方法","消費税端数","与信限度額","入金率","出荷停止FLG",
                    "宛名名称1","宛名名称2","伝票発行区分","備考","在庫管理FLG","自動配分FLG","開始日","終了日","名称CD01","名称CD02","名称CD03","名称CD04","名称CD05","名称CD06","名称CD07","名称CD08","名称CD09","名称CD10",
                    "棚卸日","開始時刻","終了時刻","端末ID","倉庫区分","営業時間1","営業時間2","営業時間3","施工業者情報","デベロッパ","営業時間","棚卸日END","為替区分","為替桁切指定","為替端数区分","配分ランク01","配分ランク02","出荷FLG",
                    "基準倉庫FLG","基準倉庫CD","配分方法FLG","POS区分","伝票印字5","伝票印字6","伝票印字7","伝票印字8","入力社員CD","店舗売場コード","ECFLG","名称CD11","名称CD12","名称CD13","名称CD14","名称CD15","名称CD16","名称CD17",
                    "名称CD18","名称CD19","名称CD20","締日2","締日3","入金予定月2","入金予定日2","入金予定月3","入金予定日3","伝票社名","伝票店名","移動区分","法人CD","連携CD","連携先法人CD","振込先1","振込先2",
                    "振込先3","期日","下限額","得意先MAIL","登録番号","振込先1","振込先2","振込先3"};
            var values = new List<string> {item.TradingCD!, item.TradingName!, item.Department!, item.Postal,item.Address1!,item.Address2,item.Address3,item.OldCD,item.Abbr,item.Katakana,
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
                    item.PayDesti3,item.DueDate.ToString(),item.MinAm.ToString(),item.CustEmail,item.RegistNum,item.PayDesti1,item.PayDesti2,item.PayDesti3};
            
            if (ShowJido == Visibility.Visible)
            {
                columns.Add("基準倉庫FLG");
                values.Add(item.BaseWarehouseFLG.ToString());

                columns.Add("基準倉庫CD");
                values.Add(item.BaseWarehouseCD.ToString());

                columns.Add("配分方法FLG");
                values.Add(item.AllocMethodFLG.ToString());

                columns.Add("出荷FLG");
                values.Add(item.ShippingFLG.ToString());
            }

            if (Show1 == Visibility.Visible)
            {
                columns.Add("伝票印字1");
                values.Add(item.InvoicePrint1.ToString());
                columns.Add("伝票印字2");
                values.Add(item.InvoicePrint2.ToString());
                columns.Add("伝票印字3");
                values.Add(item.InvoicePrint3.ToString());
                columns.Add("伝票印字4");
                values.Add(item.InvoicePrint4.ToString());

                columns.Add("伝票社名");
                values.Add(item.InvoCompName.ToString());
                columns.Add("伝票店名");
                values.Add(item.InvoStoreName.ToString());
            }
            if (Show2 == Visibility.Visible)
            {
                columns.Add("伝票印字5");
                values.Add(item.SlipPrint5.ToString());
            }
            if (Show3 == Visibility.Visible)
            {
                columns.Add("伝票印字6");
                values.Add(item.SlipPrint6.ToString());
                columns.Add("伝票印字7");
                values.Add(item.SlipPrint7.ToString());
                columns.Add("伝票印字8");
                values.Add(item.SlipPrint8.ToString());
            }
            var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.UPDATE, "MASTER_TOKUI", item.SeqNo, item.VdateUpdate.ToString(),columns.ToArray(),values.ToArray());
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
            var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.DELETE, "MASTER_TOKUI", EditShop.SeqNo, EditShop.VdateUpdate.ToString(),
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
        #endregion
        #region Function
        [RelayCommand]
        async Task DoPrintAsync()
        {
            if (!ClientLib.MessageBox(this, "印刷しますか？")) return;
            ClientLib.CursorToWait();
            if (ListShop == null || ListShop.Count == 0) { ClientLib.CursorToNormal(); return; }
            var paramNames = ListShop.Select((c, i) => $":p{i}||''").ToList();

            int max_col = 103;

            var sql_query = "select A.SEQ_NO";
            sql_query += ",SUBSTR(GET_VDATE(a.VDATE_CREATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_CREATE),10,6) 作成日時";
            sql_query += ",SUBSTR(GET_VDATE(a.VDATE_UPDATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_UPDATE),10,6) 更新日時";
            sql_query += ",A.得意先CD,A.得意先名,A.カナ,A.旧コード,A.略称,A.郵便番号,A.住所1,A.住所2,A.住所3,A.TEL,A.FAX";
            sql_query += ",A.宛名FLG1,A.宛名FLG2,A.宛名FLG3,A.宛名名称1,A.宛名名称2,A.営業担当CD,A.店種区分,A.坪数,A.在庫管理FLG";
            sql_query += ",A.掛率,A.セール掛率,A.店頭セール掛率,A.請求先CD,A.請求印刷,A.締日,A.入金予定月,A.入金予定日,A.入金方法,A.下代桁切指定";
            sql_query += ",A.下代端数区分,A.下代計算FLG,A.消費税CD,A.消費税計算方法,A.消費税端数,A.与信限度額,A.入金率,A.出荷停止FLG";
            sql_query += ",A.伝票発行区分,A.備考,A.伝票印字1,A.伝票印字2,A.伝票印字3,A.伝票印字4";
            sql_query += ",A.自動配分FLG,A.開始日,A.終了日,A.棚卸日,A.名称CD01,A.名称CD02,A.名称CD03,A.名称CD04,A.名称CD05,A.名称CD06";
            sql_query += ",A.倉庫区分,A.営業時間1,A.営業時間2,A.営業時間3,A.施工業者情報,A.デベロッパ,A.開始時刻,A.終了時刻,A.端末ID,A.営業時間,A.棚卸日END";
            sql_query += ",A.部門,A.為替区分,A.為替桁切指定,A.為替端数区分,A.名称CD07,A.名称CD08,A.名称CD09,A.名称CD10,A.配分ランク01,A.配分ランク02";
            sql_query += ",A.基準倉庫FLG,A.基準倉庫CD,A.出荷FLG,A.POS区分,A.配分方法FLG,A.伝票印字5,A.伝票印字6,A.伝票印字7,A.伝票印字8";
            sql_query += ",A.店舗売場コード,A.ECFLG,A.締日2,A.締日3,入金予定月2,入金予定日2,入金予定月3,入金予定日3";
            sql_query += ",A.伝票社名,A.伝票店名";
            sql_query += ",移動区分";
            sql_query += ",A.振込先1,A.振込先2,A.振込先3";
            sql_query += ",A.得意先MAIL";
            sql_query += ",A.登録番号";

            for (var i = max_col; i < 150; i++)
            {
                sql_query += " ,'' DummyCD" + i.ToString("000");
            }

            sql_query += ",NVL((select H.名前 from HC$MASTER_SHAIN H where H.社員CD=A.営業担当CD),'.') 担当名";
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
            sql_query += ",(A.入力社員CD ||' '|| (select B.名前 from HC$MASTER_SHAIN B where B.社員CD=A.入力社員CD)) 最終修正者";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='BMN' and H.名称CD=A.部門),'.') 部門名";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C01'),'.') title1";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C02'),'.') title2";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C03'),'.') title3";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C04'),'.') title4";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C05'),'.') title5";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C06'),'.') title6";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C07'),'.') title7";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C08'),'.') title8";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C09'),'.') title9";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C10'),'.') title10";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='RAT' and H.名称CD=A.為替区分),'.') 為替区分名";
            sql_query += ",NVL((select H.得意先名 from HC$MASTER_TOKUI H where H.得意先CD=A.基準倉庫CD),'.') 倉庫名";
            sql_query += "," + AppData.ClassCvnet.comboItem00.GetCaseStr("する", "A.在庫管理FLG") + " 在庫管理名";
            sql_query += "," + AppData.ClassCvnet.comboItem00.GetCaseStr("する", "A.請求印刷") + " 請求印刷名";
            sql_query += "," + AppData.ClassCvnet.comboItem00.GetCaseStr("予定月", "A.入金予定月") + " 入金予定月名";
            sql_query += "," + AppData.ClassCvnet.comboItem00.GetCaseStr("入金区分2", "A.入金方法") + " 入金方法名";
            sql_query += "," + AppData.ClassCvnet.comboItem00.GetCaseStr("桁切", "A.下代桁切指定") + " 下代桁切名";
            sql_query += "," + AppData.ClassCvnet.comboItem00.GetCaseStr("端数", "A.下代端数区分") + " 下代端数名";
            sql_query += "," + AppData.ClassCvnet.comboItem00.GetCaseStr("下代計算", "A.下代計算FLG") + " 下代計算名";

            if (AppData.ClassCvnet.config.oroshi == 0)
            {
                sql_query += ",CASE WHEN (A.消費税CD=0) THEN '0 非課税' WHEN (A.消費税CD=1) THEN '1 課税' ELSE '.' END  消費税CD名";
            }
            else
            {
                sql_query += ",CASE WHEN (A.消費税CD=0) THEN '0 非課税' WHEN (A.消費税CD=1) THEN '1 外税' WHEN (A.消費税CD=2) THEN '2 内税' ELSE '.' END  消費税CD名";
            }

            sql_query += "," + AppData.ClassCvnet.comboItem00.GetCaseStr("端数", "A.消費税端数") + " 消費税端数名";
            sql_query += "," + AppData.ClassCvnet.comboItem00.GetCaseStr("消費税計算", "A.消費税計算方法") + " 消費税計算名";
            sql_query += "," + AppData.ClassCvnet.comboItem00.GetCaseStr("する", "A.出荷停止FLG") + " 出荷停止名";
            sql_query += "," + AppData.ClassCvnet.comboItem00.GetCaseStr("為替桁切", "A.為替桁切指定") + " 為替桁切名";
            sql_query += "," + AppData.ClassCvnet.comboItem00.GetCaseStr("端数", "A.為替端数区分") + " 為替端数名";
            sql_query += "," + AppData.ClassCvnet.comboItem00.GetCaseStr("倉庫区分02", "A.倉庫区分") + " 倉庫区分名";

            sql_query += ",CASE WHEN (A.店種区分='0') THEN '0 倉庫' WHEN (A.店種区分='1') THEN '1 卸先' WHEN (A.店種区分='3') THEN '3 売仕店' WHEN (A.店種区分='6') THEN '6 直営店' ELSE '.' END  店種区分名";
            sql_query += ",CASE WHEN (A.POS区分='0') THEN '0 通常' WHEN (A.POS区分='1') THEN '9 POSﾏｽﾀ削除指示' WHEN (A.POS区分='2') THEN '10 出力しない' ELSE '.' END  POS区分名";
            sql_query += ",CASE WHEN (A.ECFLG='0') THEN '0 通常店舗' WHEN (A.ECFLG='1') THEN '1 EC店舗' ELSE '.' END  ECFLG名";

            sql_query += ",CASE WHEN (A.自動配分FLG='0') THEN '0 自動補充しない' WHEN (A.自動配分FLG='1') THEN '1 補充（毎日）' ELSE '.' END  自動配分名";
            sql_query += ",CASE WHEN (A.基準倉庫FLG='0') THEN '0 商品マスタの基準倉庫' WHEN (A.基準倉庫FLG='1') THEN '1 得意先マスタの基準倉庫' ELSE '.' END  基準倉庫名";
            sql_query += ",CASE WHEN (A.出荷FLG='0') THEN '0 出荷予定' WHEN (A.出荷FLG=1) THEN '1 出荷確定' ELSE '.' END  出荷FLG名";
            sql_query += ",CASE WHEN (A.配分方法FLG='0') THEN '0 売上順' WHEN (A.配分方法FLG='1') THEN '1 ランク順' WHEN (A.配分方法FLG='2') THEN '2 均等配分' ELSE '.' END  配分方法名";
            sql_query += ",CASE WHEN (A.伝票発行区分=0 OR A.伝票発行区分=1) THEN '' WHEN (A.伝票発行区分=2 OR A.伝票発行区分=5 OR A.伝票発行区分=8) THEN '店別' WHEN (A.伝票発行区分=3 OR A.伝票発行区分=4 OR A.伝票発行区分=6 OR A.伝票発行区分=7) THEN '社･店コード' ELSE '.' END  伝票印字ラベル1";
            sql_query += ",CASE WHEN (A.伝票発行区分=0 OR A.伝票発行区分=1) THEN '' WHEN (A.伝票発行区分=2 OR A.伝票発行区分=5 OR A.伝票発行区分=8) THEN '品別番号' WHEN (A.伝票発行区分=3 OR A.伝票発行区分=4 OR A.伝票発行区分=6 OR A.伝票発行区分=7) THEN '分類コード' ELSE '.' END  伝票印字ラベル2";
            sql_query += ",CASE WHEN (A.伝票発行区分=0 OR A.伝票発行区分=1) THEN '' WHEN (A.伝票発行区分=2 OR A.伝票発行区分=3 OR A.伝票発行区分=4 OR A.伝票発行区分=5 OR A.伝票発行区分=6 OR A.伝票発行区分=7 OR A.伝票発行区分=8) THEN '取引先コード' ELSE '.' END  伝票印字ラベル3";
            sql_query += ",CASE WHEN (A.伝票発行区分=0 OR A.伝票発行区分=1) THEN '' WHEN (A.伝票発行区分=2 OR A.伝票発行区分=5 OR A.伝票発行区分=8) THEN '納品場所' WHEN (A.伝票発行区分=3 OR A.伝票発行区分=4 OR A.伝票発行区分=6 OR A.伝票発行区分=7) THEN '伝票区分' ELSE '.' END  伝票印字ラベル4";
            sql_query += ",CASE WHEN (A.伝票発行区分=0 OR A.伝票発行区分=1 OR A.伝票発行区分=3 OR A.伝票発行区分=4 OR A.伝票発行区分=6 OR A.伝票発行区分=7) THEN '' WHEN (A.伝票発行区分=2 OR A.伝票発行区分=8) THEN '店出場所' WHEN (A.伝票発行区分=5) THEN '売場名' ELSE '.' END  伝票印字ラベル5";
            sql_query += ",CASE WHEN (A.伝票発行区分=0 OR A.伝票発行区分=1 OR A.伝票発行区分=3 OR A.伝票発行区分=4 OR A.伝票発行区分=5 OR A.伝票発行区分=6 OR A.伝票発行区分=7) THEN '' WHEN (A.伝票発行区分=2 OR A.伝票発行区分=8) THEN '売場名' ELSE '.' END  伝票印字ラベル6";
            sql_query += ",CASE WHEN (A.伝票発行区分=0 OR A.伝票発行区分=1 OR A.伝票発行区分=3 OR A.伝票発行区分=4 OR A.伝票発行区分=5 OR A.伝票発行区分=6 OR A.伝票発行区分=7) THEN '' WHEN (A.伝票発行区分=2 OR A.伝票発行区分=8) THEN '内線番号' ELSE '.' END  伝票印字ラベル7";
            sql_query += ",CASE WHEN (A.伝票発行区分=0 OR A.伝票発行区分=1 OR A.伝票発行区分=3 OR A.伝票発行区分=4 OR A.伝票発行区分=5 OR A.伝票発行区分=6 OR A.伝票発行区分=7) THEN '' WHEN (A.伝票発行区分=2 OR A.伝票発行区分=8) THEN '担当者' ELSE '.' END  伝票印字ラベル8";
            sql_query += "," + AppData.ClassCvnet.comboItem00.GetCaseStr("伝票", "A.伝票発行区分") + " 伝票発行区分名";
            sql_query += "," + AppData.ClassCvnet.comboItem00.GetCaseStr("予定月", "A.入金予定月2") + " 入金予定月名2";
            sql_query += "," + AppData.ClassCvnet.comboItem00.GetCaseStr("予定月", "A.入金予定月2") + " 入金予定月名3";
            sql_query += ",CASE WHEN (A.伝票発行区分=0 OR A.伝票発行区分=1) THEN '' WHEN (A.伝票発行区分=2 OR A.伝票発行区分=3 OR A.伝票発行区分=4 OR A.伝票発行区分=5 OR A.伝票発行区分=6 OR A.伝票発行区分=7 OR A.伝票発行区分=8) THEN '伝票社名' ELSE '.' END 伝票社名ラベル";
            sql_query += ",CASE WHEN (A.伝票発行区分=0 OR A.伝票発行区分=1) THEN '' WHEN (A.伝票発行区分=2 OR A.伝票発行区分=3 OR A.伝票発行区分=4 OR A.伝票発行区分=5 OR A.伝票発行区分=6 OR A.伝票発行区分=7 OR A.伝票発行区分=8) THEN '伝票店名' ELSE '.' END 伝票店名ラベル";
            sql_query += "," + AppData.ClassCvnet.comboItem00.GetCaseStr("得意先移動区分", "A.移動区分") + " 移動区分名";
            sql_query += ",A.名称CD11,A.名称CD12,A.名称CD13,A.名称CD14,A.名称CD15";
            sql_query += ",A.名称CD16,A.名称CD17,A.名称CD18,A.名称CD19,A.名称CD20";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C11'),'.') title11";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C12'),'.') title12";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C13'),'.') title13";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C14'),'.') title14";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C15'),'.') title15";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C16'),'.') title16";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C17'),'.') title17";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C18'),'.') title18";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C19'),'.') title19";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C20'),'.') title20";
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
            sql_query += " from HC$Master_TOKUI A ";

            var sql = sql_query +
                      $" where TO_CHAR(A.得意先CD) in ({string.Join(",", paramNames)}) order by A.得意先CD";

            var parameters = ListShop.Select(c => c.TradingCD).ToArray();

            var qfm_name = "cvnet_tokuisaki.qfm";
            if (AppData.ClassCvnet.config.smtflg == 1) qfm_name = "cvnet_tokuisaki_r.qfm";
            if (AppData.ClassCvnet.config.oroshi != 0) qfm_name = "cvnet_tokuisaki_w.qfm";

            var ret = AppData.Http!.AspxSqlQueryCsv(sql, parameters, qfm_name);
            var lines = ret.Split('\n');

            if (lines.Length < 2 || lines[1] == "0")
            {
                ClientLib.MessageBoxError(this, "PDFデータがありません");
                return;
            }

            string pdfPath = lines[0];
            string url = AppData.Http.URLroot + pdfPath + "/data.pdf";

            bool ready = await Utils.GlobalFunc.WaitForPdfAsync(url, TimeSpan.FromSeconds(30));
            if (!ready)
            {
                ClientLib.MessageBoxError(this, "PDF生成に時間がかかりすぎています。\n 条件を絞ってください。");
                return;
            }

            var win = new WebpdfView();
            if (win.DataContext is WebpdfViewModel vm)
            {
                vm.Pdfdata = url;
            }
            ClientLib.CursorToNormal();
            ClientLib.ShowDialogView(win, this);
        }
        [RelayCommand]
        void GetUUID() 
        {
            if (string.IsNullOrEmpty(SelectedShop.TerminalID)) 
            { 
                EditShop.TerminalID = Guid.NewGuid().ToString().ToUpper();
            }
        }
        #endregion
    }
}