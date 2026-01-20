using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using System.Collections.ObjectModel;
using System.Data;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg70Tag3ViewModel : BaseViewModel {
        #region Variables
        [ObservableProperty]
        string? title = "外部連携 : 下札発行用CSVデータ作成";

        [ObservableProperty]
        Tag3SearchOpt? tagSearchOpt;

        [ObservableProperty]
        string? lblTitle15 = "得意先";

        [ObservableProperty]
        string? mess2 = string.Empty;

        [ObservableProperty]
        BizCsvDocument? flexDataTable = null;

        /// <summary>
        /// 変更タイトル
        /// </summary>
        [ObservableProperty]
        string? m_ChgTitle;

        /// <summary>
        /// 変更内容(文字列用)
        /// </summary>
        [ObservableProperty]
        string? m_ChgWord;

        /// <summary>
        /// 変更内容(数値用)
        /// </summary>
        [ObservableProperty]
        int m_ChgNum;

        private int col_max = 90;
        private string txt_no = string.Empty;
        private string pos_no = string.Empty;
        private int num_su = 0;
        private int total_cnt = 0;

        private string init_den = "";
        private string init_kbn = "";
        #endregion

        #region Combobox List 
        /// <summary>
        /// 札種 List
        /// </summary>
        [ObservableProperty]
        public ObservableCollection<BillItem>? m_BillList;
        /// <summary>
        /// 札種
        /// </summary>
        [ObservableProperty]
        BillItem? m_SelBill;

        [ObservableProperty]
        public Dictionary<string, string>? m_SlipCatList;
        /// <summary>
        /// 伝票処理区分
        /// </summary>
        [ObservableProperty]
        string? m_SelSlipCat;
        #endregion 

        public void OnInit(object? init_para = null, string? init_flg = null)
        {
            OnInitBase(init_para);
            TagSearchOpt = new Tag3SearchOpt();
            FlexDataTable = new BizCsvDocument();

            /* xmlパターンファイル取得/読込追加 */
            GetServerPaturn();
            SelBill = BillList?.FirstOrDefault();

            /* (08.12.17追加) 区分・伝票区分の初期値設定 */
            /* init_para設定内容：3桁　(頭1桁)タグCSV.xmlのkbn⇒札種、(残2桁)⇒伝票処理区分 */
            if (para.Count > 0)
            {
                init_kbn = (para[0].Length > 0) ? para[0].Substring(0, 1) : string.Empty;
                init_den = (para[0].Length >= 4) ? para[0].Substring(1, 3) : string.Empty;

                int kbn_val = int.TryParse(init_kbn, out kbn_val) ? kbn_val : 0;
                BillItem? selected_bill = BillList?.Where(x => x.kbn == kbn_val).FirstOrDefault();
                if (selected_bill != null) SelBill = selected_bill;
            }
            AppData.ClassCvnet.AspxSqlQueryImp();
        }

        #region Dialog Search
        [RelayCommand]
        public void SelProd01()
        {
            if (TagSearchOpt == null) return;
            /* 自社社員 全表示 */
            if (AppData.ClassSatoo.LoginKubun == 0)
            {
                if (AppData.ClassCvnet.MstDialog.ContainsKey("商品"))
                {
                    SelValueModel sel_value = new SelValueModel();
                    sel_value.Code = TagSearchOpt.SelProdStart.Code;
                    sel_value.Name = TagSearchOpt.SelProdStart.Name;
                    var vm = AppData.DlgService.GetSelSho(AppData.ClassCvnet.MstDialog["商品"].v_mstname, null,
                                                          new string[] { "0" }, null, null, 0, sel_value);
                    if (vm != null)
                    {
                        TagSearchOpt.SelProdStart = new BtListHelper(vm.SelectedValue.Code, vm.SelectedValue.Name);
                        TagSearchOpt.SelProdEnd = new BtListHelper(vm.SelectedValue.Code, vm.SelectedValue.Name);
                    }
                }
            }
            else
            {
                /* 自社社員以外(仕入先) 仕入先縛り有 */
                var v_para = new BizArray();
                v_para[0] = AppData.ClassSatoo.SHAIN_CD;
                v_para[1] = TagSearchOpt.SelProdStart.Code;
                var ret_csv = OnGetShohin(v_para);

                TagSearchOpt.SelProdStart = new BtListHelper("", "");

            }
        }
        [RelayCommand]
        public void SelProd02()
        {
            if (TagSearchOpt == null) return;
            /* 自社社員 全表示 */
            if (AppData.ClassSatoo.LoginKubun == 0)
            {
                if (AppData.ClassCvnet.MstDialog.ContainsKey("商品"))
                {
                    SelValueModel sel_value = new SelValueModel();
                    sel_value.Code = TagSearchOpt.SelProdEnd.Code;
                    sel_value.Name = TagSearchOpt.SelProdEnd.Name;
                    var vm = AppData.DlgService.GetSelSho(AppData.ClassCvnet.MstDialog["商品"].v_mstname, null,
                                                          new string[] { "0" }, null, null, 0, sel_value);
                    if (vm != null)
                    {
                        TagSearchOpt.SelProdEnd = new BtListHelper(vm.SelectedValue.Code, vm.SelectedValue.Name);
                    }
                }
            }
            else
            {
                /* 自社社員以外(仕入先) 仕入先縛り有 */
                var v_para = new BizArray();
                v_para[0] = AppData.ClassSatoo.SHAIN_CD;
                v_para[1] = TagSearchOpt.SelProdEnd.Code;
                var ret_csv = OnGetShohin(v_para);
            }
        }
        [RelayCommand]
        public void SelCust01(object value)
        {
            if (TagSearchOpt == null) return;
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                TagSearchOpt.SelCustStart = new BtListHelper(get_sel00.Code, get_sel00.Name);
                TagSearchOpt.SelCustEnd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelCust02(object value)
        {
            if (TagSearchOpt == null) return;
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                TagSearchOpt.SelCustEnd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelStore01(object value)
        {
            if (TagSearchOpt == null) return;
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                TagSearchOpt.SelStoreStart = new BtListHelper(get_sel00.Code, get_sel00.Name);
                TagSearchOpt.SelStoreEnd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelStore02(object value)
        {
            if (TagSearchOpt == null) return;
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                TagSearchOpt.SelStoreEnd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// 札種 Event Changed
        /// </summary>
        partial void OnSelBillChanged(BillItem? value)
        {
            if (TagSearchOpt == null) return;
            switch (value?.kbn)
            {
                /* 伝票基準 */
                case 0:
                    TagSearchOpt.SelInvStartDate = DateTime.Now;
                    TagSearchOpt.SelInvEndDate = DateTime.Now;
                    TagSearchOpt.SelStoreStart = new BtListHelper("", "");
                    TagSearchOpt.SelStoreEnd = new BtListHelper("zzzzzzzzzzzzzzzzzzz", "");
                    TagSearchOpt.SelCustEnd = new BtListHelper("zzzzzzzzzzzzzzzzzzz", "");
                    TagSearchOpt.SelProdEnd = new BtListHelper("zzzzzzzzzzzzzzzzzzz", "");
                    /* 伝票処理区分表示制御 */
                    if (SlipCatList == null) SlipCatList = new Dictionary<string, string>();
                    else SlipCatList.Clear();
                    var cat_list = AppData.ClassCvnet.comboItem00.ComboItem_00<string>("システム区分");
                    cat_list["00"] = "00 出荷売上";
                    cat_list["02"] = "02 生地付属仕入";
                    cat_list["05"] = "05 即時移動";
                    cat_list["06"] = "10 積送移動";
                    cat_list.Remove("04");
                    cat_list.Remove("07");
                    cat_list["14"] = "14 契約発注";
                    SlipCatList = cat_list;
                    SelSlipCat = SlipCatList.FirstOrDefault().Key;
                    break;
                case 1:
                    TagSearchOpt.SelInvStartDate = null;
                    TagSearchOpt.SelInvEndDate = null;
                    TagSearchOpt.SelStoreStart = new BtListHelper("", "");
                    TagSearchOpt.SelStoreEnd = new BtListHelper("", "");
                    TagSearchOpt.SelCustEnd = new BtListHelper("zzzzzzzzzzzzzzzzzzz", "");
                    TagSearchOpt.SelProdEnd = new BtListHelper("zzzzzzzzzzzzzzzzzzz", "");
                    /* 伝票処理区分表示制御 */
                    if (SlipCatList == null) SlipCatList = new Dictionary<string, string>();
                    else SlipCatList.Clear();
                    SlipCatList.Add("88", "88 セール基準");
                    SlipCatList.Add("99", "99 商品基準");
                    SelSlipCat = SlipCatList.FirstOrDefault().Key;
                    break;
                /* マスタ基準 */
                case 2:
                    TagSearchOpt.SelInvStartDate = null;
                    TagSearchOpt.SelInvEndDate = null;
                    TagSearchOpt.SelStoreStart = new BtListHelper("", "");
                    TagSearchOpt.SelStoreEnd = new BtListHelper("", "");
                    TagSearchOpt.SelCustEnd = new BtListHelper("", "");
                    TagSearchOpt.SelProdEnd = new BtListHelper("", "");
                    /* 伝票処理区分表示制御 */
                    if (SlipCatList == null) SlipCatList = new Dictionary<string, string>();
                    else SlipCatList.Clear();
                    SlipCatList.Add("99", "99 商品基準");
                    SelSlipCat = SlipCatList.FirstOrDefault().Key;
                    break;
                default:
                    break;
            }
            /* menu_xml初期設定があれば、通るルート */
            if (!string.IsNullOrEmpty(init_den)) {
                int kbn_value = (int.TryParse(init_kbn, out kbn_value)) ? kbn_value : 0;
                if (SelBill?.kbn == kbn_value) SelSlipCat = init_kbn;
            }
            OnSelSlipCatChanged(SelSlipCat);
        }

        /// <summary>
        /// 伝票処理区分 Event Changed
        /// </summary>
        partial void OnSelSlipCatChanged(string? value)
        {
            if (TagSearchOpt == null) return;
            /* 得意先情報と倉庫情報をクリア */
            TagSearchOpt.SelCustStart = new BtListHelper("", "");
            if (SelBill?.kbn > 0)
                TagSearchOpt.SelStoreStart = new BtListHelper("", "");
            /* ラベル名変更 */
            switch (value)
            {
                case "00":
                    LblTitle15 = "得意先"; break;
                case "01":
                    LblTitle15 = "店舗"; break;
                case "02":
                    LblTitle15 = "仕入先"; break;
                case "03":
                    LblTitle15 = "仕入先"; break;
                case "05":
                    LblTitle15 = "出庫先"; break;
                case "11":
                    LblTitle15 = "出庫先"; break;
                case "12":
                    LblTitle15 = "得意先"; break;
                case "13":
                    LblTitle15 = "仕入先"; break;
                case "14":
                    LblTitle15 = "仕入先";
                    TagSearchOpt.SelStoreStart = new BtListHelper("", "");
                    break;
                default:
                    LblTitle15 = "得意先"; break;
            }
        }

        [RelayCommand]
        void DoUpdateCols(string headerName)
        { 
            if (string.IsNullOrEmpty(headerName) || TagSearchOpt == null || 
                FlexDataTable == null || FlexDataTable.csv_table.Rows.Count == 0) return;

            /* 「変更タイトル」、「変更内容」に入力されている値をタイトルがダブルクリックされたカラムへ反映 */

            DataTable tempDb = new DataTable();
            tempDb = FlexDataTable.csv_table;

            int colIndex = tempDb.Columns.IndexOf(headerName);
            if (colIndex >= 0)
            {
                DataRow firstRow = tempDb.Rows[0];

                bool IsNumValue = true; 
                int count = 0;
                foreach (DataRow row in tempDb.Rows)
                {
                    IsNumValue = int.TryParse(row[colIndex].ToString(), out int result);
                    count++;
                    if (IsNumValue == false || count > 3) break;           
                }

                if (IsNumValue)
                {
                    /* 数値項目 */
                    var messbox = System.Windows.MessageBox.Show("「" + headerName + " 」の内容を以下のように変更します。\nよろしいですか？\n\n変更後タイトル　：　" + 
                                                                 ChgTitle + "\n変更後内容　：　" + ChgNum +
                                                                 "\n\n「はい」選択で、タイトルと内容を変更\n「いいえ」選択で、内容のみを変更", "確認",
                                  System.Windows.MessageBoxButton.YesNoCancel, System.Windows.MessageBoxImage.Error);

                    if (messbox == System.Windows.MessageBoxResult.Yes)
                    {
                        FlexDataTable = null;
                        tempDb.Columns[colIndex].ColumnName = ChgTitle;
                        foreach (DataRow row in tempDb.Rows)
                        {
                            row[colIndex] = ChgNum;
                        } 
                        FlexDataTable = new BizCsvDocument(tempDb);
                    }
                    else if (messbox == System.Windows.MessageBoxResult.No)
                    {
                        FlexDataTable = null;
                        foreach (DataRow row in tempDb.Rows)
                        {
                            row[colIndex] = ChgNum;
                        }
                        FlexDataTable = new BizCsvDocument(tempDb);
                    }
                }
                else
                {
                    /* 文字項目 */
                    var messbox = System.Windows.MessageBox.Show("「" + headerName + " 」の内容を以下のように変更します。\nよろしいですか？\n\n変更後タイトル　：　" +
                                                                 ChgTitle + "\n変更後内容　：　" + ChgWord +
                                                                 "\n\n「はい」選択で、タイトルと内容を変更\n「いいえ」選択で、内容のみを変更", "確認",
                                  System.Windows.MessageBoxButton.YesNoCancel, System.Windows.MessageBoxImage.Error);

                    if (messbox == System.Windows.MessageBoxResult.Yes)
                    {
                        FlexDataTable = null;
                        tempDb.Columns[colIndex].ColumnName = ChgTitle;
                        foreach (DataRow row in tempDb.Rows)
                        {
                            row[colIndex] = ChgWord;
                        }
                        FlexDataTable = new BizCsvDocument(tempDb);
                    }
                    else if (messbox == System.Windows.MessageBoxResult.No)
                    {
                        FlexDataTable = null;
                        foreach (DataRow row in tempDb.Rows)
                        {
                            row[colIndex] = ChgWord;
                        }
                        FlexDataTable = new BizCsvDocument(tempDb);
                    }
                }
            }
        }

        /// <summary>
        /// 品番検索実行 Button
        /// </summary>
        [RelayCommand]
        void DoSearch()
        {
            try
            {
                if (TagSearchOpt == null || SelBill == null) return;
                OnPaturnSet(SelBill.id);
                TagSearchOpt.OutputMess = string.Empty;
                TagSearchOpt.OutputDest = string.Empty;
            }
            catch (Exception ex) {
                System.Windows.MessageBox.Show("パターンファイル正しいレイアウトではない可能性があります", "Error",
                            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 割合加算 Button
        /// </summary>
        [RelayCommand]
        void DoPercentAdd()
        {
            if (TagSearchOpt == null || FlexDataTable == null || FlexDataTable.csv_table.Rows.Count == 0) return;
            /* 枚数項目の位置をチェック */
            int colIndex = FlexDataTable.csv_table.Columns.IndexOf("枚数");
            if (colIndex >= 0)
            {
                foreach (DataRow row in FlexDataTable.csv_table.Rows)
                {
                    decimal num = decimal.TryParse(row[colIndex].ToString(), out num) ? num : 0;
                    row[colIndex] = (int)Math.Round(num * (100 + TagSearchOpt.PercentAdd) / 100);
                }
            }
        }
        /// <summary>
        /// 枚数加算 Button
        /// </summary>
        [RelayCommand]
        void DoSheetNum()
        {
            if (TagSearchOpt == null || FlexDataTable == null || FlexDataTable.csv_table.Rows.Count == 0) return; 
            /* 枚数項目の位置をチェック */
            int colIndex = FlexDataTable.csv_table.Columns.IndexOf("枚数");
            if (colIndex >= 0)
            {
                foreach (DataRow row in FlexDataTable.csv_table.Rows)
                {
                    int num = int.TryParse(row[colIndex].ToString(), out num) ? num : 0;
                    row[colIndex] = num + TagSearchOpt.SheetAdd;
                }
            }
        }
        /// <summary>
        /// データ作成 Button
        /// </summary>
        [RelayCommand]
        void DoExecute()
        {
            if (FlexDataTable == null || FlexDataTable.csv_table.Rows.Count == 0)
            {
                Mess2 = "対象データがありません！";
                return;
            }
            if (TagSearchOpt == null) return;
            TagSearchOpt.OutputDest = FlexDataTable.SaveCsv("バーコードラベル");
        }
        /// <summary>
        /// 戻る Button
        /// </summary>
        [RelayCommand]
        void DoCancel()
        {
            ClientLib.ExitDialogResult(this, true);
        }
        #endregion

        #region Function
        void GetServerPaturn()
        {

            var paturn = AppData.Http?.GetDataXmlFile("TagCsv_Paturn.xml");
            #region ComboList Init
            var bill_list = new ObservableCollection<BillItem>();
            foreach (var item in paturn.Paturn)
            {
                BillItem bill_item = new BillItem();
                bill_item.kbn = item.kbn;
                bill_item.id = item.id;
                bill_item.name = item.name;

                bill_list.Add(bill_item); 
            }
            BillList = bill_list; 
            #endregion
        }
        DataTable OnGetShohin(BizArray v_para)
        {
            var sql_str = "select 商品CD||' '||商品名 from HC$master_shohin";
            sql_str += " where メーカーCD = :1 and 商品CD >= :2";
            sql_str += " order by 商品CD";
            sql_str = AppData.ClassCvnet.GetSqlDisp(sql_str);
            return AppData.Http?.AspxSqlQuery(sql_str, v_para.ToArray());
        }

        /// <summary>
        ///  xmlパターンファイル取得/読込追加↓
        /// </summary> 
        async void OnPaturnSet(string id)
        {
            if (TagSearchOpt == null) return;

            var paturn = AppData.Http?.GetDataXmlFile("TagCsv_Paturn.xml");
            if (paturn == null) return;
            List<dynamic> paturn_list = ((IEnumerable<dynamic>)paturn.Paturn).ToList();
            dynamic node = paturn_list.FirstOrDefault(x => x.id == id);
            if (node == null) return;

            /* 初期化処理 */
            txt_no = string.Empty;
            /*#63643 最終行に追加表示の時は消さない*/
            if (TagSearchOpt.SelLineCond == 1)
            {
                FlexDataTable?.Clear();
            }

            List<dynamic> listnodes = ((IEnumerable<dynamic>)node.List).ToList();
            string select_cols = "";
            string group_cols = "";
            string hinshitsu = "";
            string col_name = "";
            string col_title = "";
            var roop_cnt = 0;
            var o_num_cnt = 0;
            var i_num_cnt = 0;
            var o_txt_cnt = 0;
            int slip_cat = int.TryParse(SelSlipCat, out slip_cat) ? slip_cat : 0;

            /* selectするカラム名を連結する */
            for (int i = 0; i < listnodes.Count; i++) {
                for (int j = 0; j < listnodes.Count; j++) {
                    if (listnodes[j].col == i + 1) {
                        if (listnodes[j].type.Substring(0, 5) == "o_txt")
                        {  
                            if (listnodes[j].name == "品質表示")
                            {
                                /* 文字列出力領域時 */
                                //var child = Form1.CvnetFlexView1.FlexRecord1.FindChild("Text" + str(o_txt_cnt + 1));
                                //child.width = listnodes[j].getAttribute("width");
                                //child.title = listnodes[j].getAttribute("name");
                                col_title += listnodes[j].name + ",";

                                /* 品質表示の加工 */
                                roop_cnt = listnodes[j].rp; 
                                for (int k = 0; k < roop_cnt; k++)
                                {
                                    if (slip_cat < 70 && k == 0) { hinshitsu += "max("; }
                                    hinshitsu += listnodes[j].value.Replace("sel_no", (k + 1).ToString());
                                    hinshitsu += "||' '||";
                                }
                                hinshitsu = hinshitsu.Substring(0, hinshitsu.Length - 7);
                                if (slip_cat < 70) { hinshitsu += ")"; }
                                hinshitsu += " " + listnodes[j].sqlnm;
                                select_cols += hinshitsu.ToString();
                                select_cols += ",";
                                col_name += "Text" + (o_txt_cnt + 1).ToString();
                                col_name += ",";
                                txt_no += (o_txt_cnt + 1).ToString();
                                txt_no += ",";
                                pos_no += listnodes[j].col.ToString();
                                pos_no += ",";
                            }
                            else
                            {
                                //var child = Form1.CvnetFlexView1.FlexRecord1.FindChild("Text" + str(o_txt_cnt + 1));
                                //child.width = listnodes[j].getAttribute("width");
                                //child.title = listnodes[j].getAttribute("name");
                                col_title += listnodes[j].name + ",";

                                if (slip_cat < 70) { select_cols += "max("; }
                                select_cols += listnodes[j].value.ToString();
                                if (slip_cat < 70) { select_cols += ")"; }
                                select_cols += " " + listnodes[j].sqlnm;
                                select_cols += ",";
                                col_name += "Text" + (o_txt_cnt + 1).ToString();
                                col_name += ",";
                                txt_no += (o_txt_cnt + 1).ToString();
                                txt_no += ",";
                                pos_no += listnodes[j].col.ToString();
                                pos_no += ",";
                                if (slip_cat < 70 && (listnodes[j].type.Contains("_gp")))
                                {
                                    group_cols += listnodes[j].value.ToString();
                                    group_cols += ",";
                                }
                            }
                            o_txt_cnt++;
                        }
                        else if (listnodes[j].type.Substring(0, 5) == "o_num")
                        {
                            /* 数値出力領域時 */
                            //var child = Form1.CvnetFlexView1.FlexRecord1.FindChild("Text" + str(51 + o_num_cnt));
                            //child.width = listnodes[j].getAttribute("width");
                            //child.title = listnodes[j].getAttribute("name");
                            //child.TitleBgColor = Form1.num_color;
                            col_title += listnodes[j].name + ",";

                            if (slip_cat < 70) { select_cols += "max("; }
                            select_cols += listnodes[j].value.ToString();
                            if (slip_cat < 70) { select_cols += ")"; }
                            select_cols += " " + listnodes[j].sqlnm;
                            select_cols += ",";
                            col_name += "Text" + (51 + o_num_cnt).ToString();
                            col_name += ",";
                            txt_no += (51 + o_num_cnt).ToString();
                            txt_no += ",";
                            pos_no += listnodes[j].col.ToString();
                            pos_no += ",";
                            if (slip_cat < 70 && (listnodes[j].type.Contains("_gp")))
                            {
                                group_cols += listnodes[j].value.ToString();
                                group_cols += ",";
                            }
                            o_num_cnt++;
                        }
                        else if (listnodes[j].type.Substring(0, 5) == "i_num")
                        {
                            /* 数値入力領域時 */
                            //var child = Form1.CvnetFlexView1.FlexRecord1.FindChild("Text" + str(81 + i_num_cnt));
                            //child.width = listnodes[j].getAttribute("width");
                            //child.title = listnodes[j].getAttribute("name");
                            //child.TitleBgColor = Form1.num_color;
                            col_title += listnodes[j].name + ",";

                            if (listnodes[j].name == "枚数" && slip_cat < 70 && listnodes[j].value == "0")
                            {
                                select_cols += listnodes[j].value.Replace("0", "sum(t1.数量)");
                                /* 枚数位置セーブ 10.03.04 */
                                num_su = 81 + i_num_cnt;
                            }
                            else
                            {
                                if (slip_cat < 70) { select_cols += "max("; }
                                select_cols += listnodes[j].value.ToString();
                                if (slip_cat < 70) { select_cols += ")"; }
                            }
                            select_cols += " " + listnodes[j].sqlnm;
                            select_cols += ",";
                            col_name += "Text" + (81 + i_num_cnt).ToString();
                            col_name += ",";
                            txt_no += (81 + i_num_cnt).ToString();
                            txt_no += ",";
                            pos_no += listnodes[j].col.ToString();
                            pos_no += ",";

                            if ((slip_cat < 70) && (listnodes[j].type.Contains("_gp")))
                            {
                                group_cols += listnodes[j].value.ToString();
                                group_cols += ",";
                            }

                            /* 枚数位置セーブ 10.03.04 */
                            if (listnodes[j].name == "枚数")
                            {
                                num_su = 81 + i_num_cnt;
                            }

                            i_num_cnt++;
                        }
                    }
                }
            }

            if (select_cols.Length > 0 && select_cols.Substring(select_cols.Length - 1) == ",") 
                select_cols = select_cols.Substring(0, select_cols.Length - 1); 

            if (col_name.Length > 0 && col_name.Substring(col_name.Length - 1) == ",")
                col_name = col_name.Substring(0, col_name.Length - 1);

            if (col_title.Length > 0 && col_title.Substring(col_title.Length - 1) == ",")
                col_title = col_title.Substring(0, col_title.Length - 1);

            if (txt_no.Length > 0 && txt_no.Substring(txt_no.Length - 1) == ",")
                txt_no = txt_no.Substring(0, txt_no.Length - 1);

            if (pos_no.Length > 0 && pos_no.Substring(pos_no.Length - 1) == ",")
                pos_no = pos_no.Substring(0, pos_no.Length - 1);

            if (group_cols.Length > 0 && group_cols.Substring(group_cols.Length - 1) == ",")
                group_cols = group_cols.Substring(0, group_cols.Length - 1);

            if (AppData.ClassCvnet.config.usegenka == 1) group_cols += ",NVL(C.行NO,0)";

            var wrk_para2 = new BizArray();
            if (slip_cat < 70)
            {
                wrk_para2[0] = SelSlipCat;
                if (SelSlipCat == "14") wrk_para2[0] = "3";
                wrk_para2[1] = TagSearchOpt.SelCustStart.Code;
                wrk_para2[2] = TagSearchOpt.SelStoreStart.Code;
                wrk_para2[3] = TagSearchOpt.SelSlipStartNo;
                wrk_para2[4] = TagSearchOpt.SelSlipEndNo;
                wrk_para2[5] = TagSearchOpt.SelInvStartDate?.ToString("yyyyMMdd");
                wrk_para2[6] = TagSearchOpt.SelCustEnd.Code;
                wrk_para2[7] = TagSearchOpt.SelStoreEnd.Code;
                wrk_para2[8] = TagSearchOpt.SelInvEndDate?.ToString("yyyyMMdd");
            }

            /* (08.10.28) WHERE文を追加 */
            /* <SQL>～</SQL>内に条件文を追加する仕様です！ */
            string where_str01 = string.Empty;
            try { where_str01 = node.SQL.Trim(); } catch { } 

            string wrk_sql = "select ";
            if (slip_cat >= 70) wrk_sql += "distinct ";
            wrk_sql += select_cols;
            if (AppData.ClassCvnet.config.usegenka == 1) wrk_sql += ",NVL(C.行NO,0)";
            wrk_sql += " from HC$MASTER_SHOHIN_JAN B";
            wrk_sql += " join HC$MASTER_SHOHIN A on (A.商品CD=B.商品CD)";

            /* 原価FLG使用? */
            if (AppData.ClassCvnet.config.usegenka == 1) { wrk_sql += " left outer join HC$MASTER_SHOHIN_GENKA C on (C.商品CD=B.商品CD)"; }

            if (slip_cat < 70) { 
                if (slip_cat == 14)
                    wrk_sql += ",HC$TRAN_VTORI0 t0,HC$TRAN_VTORI1 t1";
                else
                    wrk_sql += ",HC$TRAN_TORI0 t0,HC$TRAN_TORI1 t1";
            }
            wrk_sql += " where A.商品CD between :1 and :2 and A.ブランドCD between :3 and :4 and A.アイテムCD between :5 and :6";
            wrk_sql += " " + where_str01;
            if (slip_cat < 70) {
                wrk_sql += " and A.商品CD = t1.商品CD and t1.ヘッダNO=t0.SEQ_NO";
                wrk_sql += " and B.色CD = t1.色CD and B.サイズCD = t1.サイズCD";

                if (AppData.ClassCvnet.config.usegenka == 1) { wrk_sql += " and t1.原価FLG=decode(t1.原価FLG,0,0,C.行NO)"; }

                /* #21908 返品は除く 16.07.14 */
                wrk_sql += " and t0.取引区分<20";

                wrk_sql += " and t1.伝票処理区分 = " + wrk_para2[0].ToString();

                if (TagSearchOpt.SelCustStart.Code != "")
                    wrk_sql += " and t0.取引先CD1 between '" + wrk_para2[1].ToString() + "' and '" + wrk_para2[6].ToString() + "'";

                if (TagSearchOpt.SelStoreStart.Code != "")
                    wrk_sql += " and t0.倉庫CD between '" + wrk_para2[2] + "' and '" + wrk_para2[7].ToString() + "'";

                wrk_sql += " and t1.ヘッダNO between " + wrk_para2[3].ToString() + " and " + wrk_para2[4].ToString();

                if (TagSearchOpt.SelInvStartDate?.ToString("yyyyMMdd")  != "")
                    wrk_sql += " and t0.在庫計上日 between '" + wrk_para2[5].ToString() + "' and '" + wrk_para2[8].ToString() + "'";

                wrk_sql += " group by " + group_cols;
            }
            if (slip_cat == 88)
            {
                if (AppData.ClassCvnet.config.SellMFlg == 1)
                {
                    /* 新セールマスタ */
                    wrk_sql += " and exists ("
                    + "SELECT 'X' FROM HC$MASTER_PRICE0 p0,HC$MASTER_PRICE1 p1,HC$MASTER_PRICE2 p2"
                    + " WHERE p1.商品CD=A.商品CD AND p0.SEQ_NO=p1.ヘッダNO AND p0.SEQ_NO=p2.ヘッダNO AND "
                    + "(p2.店舗CD='そうここーど' or p2.店舗CD='.') AND	p2.期間S between 'ひづけ1' AND 'ひづけ2'"
                    + ")";
                }
                else
                {
                    /* 旧セールマスタ */
                    wrk_sql += " and exists (select 'X' from HC$MASTER_SHO_PRICE C where c.商品CD=A.商品CD"
                    + " and (C.店舗CD='そうここーど' or C.店舗CD='.') and C.期間S between 'ひづけ1' and 'ひづけ2')";
                }
            }
            if (AppData.ClassCvnet.config.usegenka == 1) wrk_sql += " order by A.商品CD,NVL(C.行NO,0),B.色CD,B.サイズCD";
            else
            { 
                if (slip_cat >= 70)
                    wrk_sql += " order by A.商品CD,B.色CD,B.サイズCD";
                else
                    wrk_sql += " order by A.商品CD,max(B.色CD),max(B.サイズCD)";
            }
            wrk_sql += "";

            var wrk_para = new BizArray();
            if (string.IsNullOrEmpty(TagSearchOpt.SelProdStart.Code)) { wrk_para[0] = "."; } else { wrk_para[0] = TagSearchOpt.SelProdStart.Code; }
            if (string.IsNullOrEmpty(TagSearchOpt.SelProdEnd.Code)) { wrk_para[1] = "."; } else { wrk_para[1] = TagSearchOpt.SelProdEnd.Code; }
            wrk_para[2] = "."; // CodeBrd1
            wrk_para[3] = "99999999"; // CodeBrd2
            wrk_para[4] = "."; // CodeItem1
            wrk_para[5] = "ZZZZZZ"; // CodeItem2

            if (slip_cat >= 70) {
                /* ダミーデータで変換を掛けることとする */
                /* 倉庫CDはそうここーど
                日付はひづけとひらがなを変換する */

                wrk_sql = wrk_sql.Replace("そうここーど", TagSearchOpt.SelStoreStart.Code);
                wrk_sql = wrk_sql.Replace("ひづけ1", TagSearchOpt.SelInvStartDate?.ToString("yyyyMMdd"));
                wrk_sql = wrk_sql.Replace("ひづけ2", TagSearchOpt.SelInvEndDate?.ToString("yyyyMMdd"));

                if (SelBill?.kbn < 2) 
                    wrk_sql = "select * from (" + wrk_sql + ") where 枚数>0"; 
            }

            var ret_cntcsv = AppData.Http?.AspxSqlQuery("select count(*) from (" + wrk_sql + ")", wrk_para.ToArray());
            int ret_value = int.TryParse(ret_cntcsv.Rows[0][0].ToString(), out ret_value) ? ret_value : 0;
            if (ret_value > 800)
            {
                var result = System.Windows.MessageBox.Show("800レコード以上あります\nこのまま実行しますか？\n件数に比例した時間がかかります(" + ret_value.ToString() + ")", "確認",
                                System.Windows.MessageBoxButton.OKCancel, System.Windows.MessageBoxImage.Information);
                if (result == System.Windows.MessageBoxResult.Cancel)
                {
                    Mess2 = "中止しました";
                    return;
                }
            }
            else if (ret_cntcsv.Rows[0][0] == "0" || ret_cntcsv.Rows[0][0] == "") {
                Mess2 = "対象データがありませんでした";
            }

            var ret_csv = AppData.Http?.AspxSqlQuery(wrk_sql, wrk_para.ToArray(), "dummy");
            string[] resp_csv;
            if (ret_csv != null)
            {
                if (string.IsNullOrEmpty(ret_csv.Rows[0][0].ToString())) return;
                resp_csv = ret_csv.Rows[0][0].ToString().Split('\n');
            } else return;
             
            total_cnt = int.TryParse(resp_csv[1], out total_cnt) ? total_cnt : 0;
            if (total_cnt <= 0)
            {
                Mess2 = "対象データがありませんでした";
                return;
            }

            string dataUrl = AppData.Url + resp_csv[0].ToString() + "/data.txt";
            var ret_csv2 = new BizCsvDocument();
            await ret_csv2.LoadDataFromUrlAsync(dataUrl);
            //ret_csv2.SetColHeader(col_name);
            ret_csv2.SetColHeader(col_title);

            if (FlexDataTable == null) FlexDataTable = new BizCsvDocument();
            if (FlexDataTable.csv_table.Rows.Count == 0)
                FlexDataTable.SetColHeader(col_title);
             
            if (TagSearchOpt.SelLineCond == 1)
            {
                /*#63643 通常表示*/
                FlexDataTable = ret_csv2;
            }
            else
            {
                DataTable dt = ret_csv2.GetTable();
                /*#63643 最終行に追加表示*/
                var rows = dt.Rows.Cast<DataRow>().ToList(); 
                foreach (DataRow row in rows)
                {
                    FlexDataTable.csv_table.ImportRow(row);
                }
            }
        }
        #endregion
    }

    public partial class Tag3SearchOpt : ObservableObject
    { 
        /// <summary>
        /// 商品CD - Start
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_SelProdStart;

        /// <summary>
        /// 商品CD - End
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_SelProdEnd;

        /// <summary>
        /// 得意先 - Start
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_SelCustStart;

        /// <summary>
        /// 得意先 - End
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_SelCustEnd;

        /// <summary>
        /// 得意先 Mstname
        /// </summary>
        [ObservableProperty]
        string? m_SelMstCust;

        /// <summary>
        /// 倉庫 - Start
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_SelStoreStart;

        /// <summary>
        /// 倉庫 - End
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_SelStoreEnd;

        /// <summary>
        /// 倉庫 Mstname
        /// </summary>
        [ObservableProperty]
        string? m_SelMstStore;

        /// <summary>
        /// 伝票NO - Start
        /// </summary>
        [ObservableProperty]
        string? m_SelSlipStartNo;

        /// <summary>
        /// 伝票NO - End
        /// </summary>
        [ObservableProperty]
        string? m_SelSlipEndNo;

        /// <summary>
        /// 在庫計上日 - Start
        /// </summary>
        [ObservableProperty]
        DateTime? m_SelInvStartDate;

        /// <summary>
        /// 在庫計上日 - End
        /// </summary>
        [ObservableProperty]
        DateTime? m_SelInvEndDate;

        /// <summary>
        /// 行 Option (0: 追加, 1: 新規)
        /// </summary>
        [ObservableProperty]
        int m_SelLineCond;

        /// <summary>
        /// 割合加算
        /// </summary>
        [ObservableProperty]
        int m_PercentAdd;

        /// <summary>
        /// 枚数加算
        /// </summary>
        [ObservableProperty]
        int m_SheetAdd;

        /// <summary>
        /// 出力先
        /// </summary>
        [ObservableProperty]
        string? m_OutputDest;

        /// <summary>
        /// 出力先 Message
        /// </summary>
        [ObservableProperty]
        string? m_OutputMess;

        public Tag3SearchOpt()
        {
            SelProdStart = new();
            SelProdEnd = new();
            SelCustStart = new();    
            SelCustEnd = new();
            SelMstCust = string.Empty;
            SelStoreStart = new();
            SelStoreEnd = new();
            SelMstStore = string.Empty;
            SelSlipStartNo = "0";
            SelSlipEndNo = "99999999999999";
            SelInvStartDate = DateTime.Now;
            SelInvEndDate =  DateTime.Now;
            SelLineCond = 1;
            //ChgTitle = string.Empty;
            //ChgWordCnt = string.Empty;
            //ChgNumCnt = 0;
            PercentAdd = 0;
            SheetAdd = 0;
            OutputDest = string.Empty;
            OutputMess = string.Empty;
        }
    }

    public class BillItem
    {
        public string id { get; set; }
        public string name { get; set; }
        public int kbn { get; set; }
    }
}
