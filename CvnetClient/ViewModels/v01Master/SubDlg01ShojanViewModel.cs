using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
//using static CvnetClient.ViewModels.SubDlg09Upkeihi2ViewModel;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01ShojanViewModel : BaseViewModel
    {
        [ObservableProperty]
        ObservableCollection<MasterShohin>? listProduct;
        [ObservableProperty]
        MasterShohinJan? editProduct;
        [ObservableProperty]
        MasterShohinJan? findProduct;
        [ObservableProperty]
        public Dictionary<int, string>? chushi;
        [ObservableProperty]
        public Dictionary<int, string>? jidohaibun;
        [ObservableProperty]
        ObservableCollection<MasterShohinJan>? listShohinJan;
        [ObservableProperty]
        MasterShohinJan? selectedProduct;
        [ObservableProperty]
        BtListHelper editColorCd = new();
        [ObservableProperty]
        BtListHelper editSizeCd = new();
        [ObservableProperty]
        BtListHelper editProductCd = new();
        [ObservableProperty]
        string? selProductCd;
        [ObservableProperty]
        int? pageNow;
        [ObservableProperty]
        int? pageTotal;
        [ObservableProperty]
        decimal? timex = 0;

        public int pageCnt = 1;

        private BizArray col_list;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(NextListCommand))]
        private bool canNext;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(BackListCommand))]
        private bool canBack;




        public static BizArray v_para2 = new BizArray();
        private int page = 1;

        public void OnInit()
        {
            SelectedProduct = new MasterShohinJan();
            FindProduct = new MasterShohinJan();
            EditProduct = new MasterShohinJan();
            EditColorCd = new BtListHelper();
            EditSizeCd = new BtListHelper();
            EditProductCd = new BtListHelper();
            ListShohinJan = new ObservableCollection<MasterShohinJan>();

            EditProduct.ScheduledProductionQuantity = 0;
            //            EditProduct.CuttingQuantity = 0;
            EditProduct.TagNumber = 0;

 //           CanBack = true;
 //           CanNext = true;

            Chushi = new Dictionary<int, string>
            {
                { 0, "0 正規" },
                { 1, "1 中止" },
            };
            EditProduct.UseFlag = Chushi.FirstOrDefault().Key;

            Jidohaibun = new Dictionary<int, string>
            {
                { 0, "0 しない" },
                { 1, "1 売上基準" },
                { 9, "9 商品マスタ依存" }
            };
            EditProduct.AutoAllocationFlag = Jidohaibun.FirstOrDefault().Key;

            string sql_query = "select nvl((select 値 from hc$master_config where フラグ名 = 'dispColSizKakaku'),0) flg from dual";

            //string sql_query_init = "SELECT A.SEQ_NO,A.VDATE_CREATE,A.VDATE_UPDATE, A.商品CD, A.色CD, A.サイズCD, A.JANコード1, A.JANコード2, A.JANコード3, A.メモ, A.使用FLG, A.生産予定数, A.裁断数, A.下札枚数, A.自動配分FLG, A.上代, A.仕入価格, A.外貨仕入価格, A.原価,B.商品名,GET_COLORNAME(A.色CD) 色名,GET_SIZENAME(A.商品CD,A.サイズCD) サイズ名,GET_GENKA(B.商品CD,0,'20991231',A.色CD,A.サイズCD) 原価1 FROM HC$MASTER_SHOHIN_JAN A, HC$MASTER_SHOHIN B";
            //sql_query_init = AppData.ClassCvnet.GetSqlDisp(sql_query_init);
            //var ret_csv_init = AppData.Http?.AspxSqlQuery(sql_query_init);
            v_para2[0] = "0";

        }

        string[] sql_col_list =
        {
            "商品CD", "色CD", "サイズCD", "JANコード1", "JANコード2", "JANコード3",
            "メモ", "使用FLG", "生産予定数", "裁断数", "下札枚数",
            "自動配分FLG", "上代", "仕入価格", "外貨仕入価格", "原価"
        };



        //[RelayCommand]
        //void DoList()
        //{
        //    if (!ClientLib.MessageBox(this, "本当にしますか？")) return;

        //}

        [RelayCommand]
        public void SelDspUpdate(string? para)
        {
            int.TryParse(para, out var p);
            ExecuteShohinJanSearch(p);
        }

        // On.......Changed 
        partial void OnSelectedProductChanged(MasterShohinJan? value)
        {
            if (value != null)
                EditProduct = Common.CloneObject(value);
            else
                EditProduct = null;
        }

        [RelayCommand]
        public void SelProduct(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditProductCd != null)
            {
                EditProductCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
                EditProduct.ProductCD = EditProductCd.Code;
                EditProduct.ProductName = EditProductCd.Name;
            }
        }

        [RelayCommand]
        public void SelColor(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditColorCd != null)
            {
                EditColorCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
                // Put the selected color string into the bindable property
                EditProduct.ColorCD = EditColorCd.Code;
                EditProduct.ColorName = EditColorCd.Name;
            }
        }

        [RelayCommand]
        public void SelSize(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditSizeCd != null)
            {
                EditSizeCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
                // Put the selected color string into the bindable property
                EditProduct.SizeCD = EditSizeCd.Code;
                EditProduct.SizeName = EditSizeCd.Name;
            }
        }

        [RelayCommand(CanExecute = nameof(CanBack))]
        void BackList()
        {
            v_para2[0] = "1";
            pageCnt = pageCnt - 39;
            if (pageCnt < 1) pageCnt = 1;
            SelDspUpdate(pageCnt.ToString());

            //if (ListWorker == null || ListWorker.Count == 0)
            //    ClientLib.MessageBoxOk(this, "データがありません");
            UpdatePagingFlags();
        }

        [RelayCommand(CanExecute = nameof(CanNext))]
        private void NextList()
        {
            v_para2[0] = "2";
            //SelDspUpdate(pageCnt.ToString());
            //pageCnt = pageCnt + 39;
            //if (ListShohinJan == null || ListShohinJan.Count < 40 )
            //    ClientLib.MessageBoxOk(this, "データがありません");
            //else
            //{
            pageCnt = pageCnt + 39;
            SelDspUpdate(pageCnt.ToString());
            //}
            UpdatePagingFlags();
        }

        private void UpdatePagingFlags()
        {
            // 「前へ」は 1 ページ目より後なら有効
            CanBack = pageCnt > 1;

            // 「次へ」は、検索結果作成時に設定するのがベスト（list.Count > Max で CanNext = true/false）
            // もしそこでもう設定しているならここでは何もしなくてOK
        }

        // for 更新 button
        [RelayCommand]
        void DoUpdate()
        {
            if (!ClientLib.MessageBox(this, "修正しますか？")) return;
            var item = Common.CloneObject(EditProduct);
            Common.ConvertDotStringDel(item);
            if (item == null) return;
            var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.UPDATE, "Master_SHOHIN_JAN",
                item.SeqNo, item.VdateUpdate.ToString(),
                new string[] { "商品CD", "色CD", "サイズCD", "JANコード1", "JANコード2", "JANコード3",
                "メモ", "使用FLG", "生産予定数", "裁断数", "下札枚数",
                "自動配分FLG", "上代", "仕入価格", "外貨仕入価格", "原価"},
                new string[] { item.ProductCD!, item.ColorCD!, item.SizeCD!, item.JanCode1!, item.JanCode2!,
                item.JanCode3!, item.Memo!, item.UseFlag.ToString()!, item.ScheduledProductionQuantity.ToString()!, item.CuttingQuantity.ToString()!, item.TagNumber.ToString()!, item.AutoAllocationFlag.ToString()!, item.RetailPrice.ToString()!, item.SupplierPrice.ToString()!, item.ForeignCurrencyPrice.ToString()!, item.CostPrice.ToString()!});
            if (ret.Code == 0)
            {
                Common.ConvertDotStringDel(item);
                if (SelectedProduct != null)
                {
                    SelectedProduct.VdateUpdate = decimal.Parse(ret.VDate);
                    SelectedProduct.ProductCD = item.ProductCD;
                    SelectedProduct.ColorCD = item.ColorCD;
                    SelectedProduct.SizeCD = item.SizeCD;
                    SelectedProduct.JanCode1 = item.JanCode1;
                    SelectedProduct.JanCode2 = item.JanCode2;
                    SelectedProduct.JanCode3 = item.JanCode3;
                    SelectedProduct.Memo = item.Memo;
                    SelectedProduct.UseFlag = item.UseFlag;
                    SelectedProduct.ScheduledProductionQuantity = item.ScheduledProductionQuantity;
                    SelectedProduct.CuttingQuantity = item.CuttingQuantity;
                    SelectedProduct.TagNumber = item.TagNumber;
                    SelectedProduct.AutoAllocationFlag = item.AutoAllocationFlag;
                    SelectedProduct.RetailPrice = item.RetailPrice;
                    SelectedProduct.SupplierCD = item.SupplierCD;
                    SelectedProduct.ForeignCurrencyPrice = item.ForeignCurrencyPrice;
                    SelectedProduct.CostPrice = item.CostPrice;
                    EditProduct = Common.CloneObject(SelectedProduct);
                    ClientLib.MessageBoxOk(this, "修正しました");
                }
            }
            else
            {
                ClientLib.MessageBoxError(this, ret.Code.ToString());
            }
        }

        // for 削除 button
        [RelayCommand]
        void DoDelete()
        {
            if (!ClientLib.MessageBox(this, "削除しますか？")) return;
            if (EditProduct == null) return;
            var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.DELETE, "Master_SHOHIN_JAN", EditProduct.SeqNo, EditProduct.VdateUpdate.ToString(),
                new string[0], new string[0]);
            if (ret.Code == 0)
            {
                if (SelectedProduct != null)
                {
                    ListShohinJan!.Remove(SelectedProduct);
                    var item = ListShohinJan.Where(c => c.ProductCD == ListShohinJan.Min(c => c.ProductCD)).FirstOrDefault();
                    SelectedProduct = item;
                    ClientLib.MessageBoxOk(this, "削除しました");
                }
            }
            else
            {
                ClientLib.MessageBoxError(this, ret.Code.ToString());
            }

        }

        // for 追加 button
        [RelayCommand]
        void DoInsert()
        {
            if (!ClientLib.MessageBox(this, "新規登録しますか？")) return;
            var item = Common.CloneObject(EditProduct);
            Common.ConvertDotStringAdd1(item);

            if (item == null) return;

            var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.INSERT, "Master_SHOHIN_JAN", 0, "0",
                   new string[] { "商品CD", "色CD", "サイズCD", "JANコード1", "JANコード2", "JANコード3",
                                "メモ", "使用FLG", "生産予定数", "裁断数", "下札枚数",
                                "自動配分FLG", "上代", "仕入価格", "外貨仕入価格", "原価"},
                   new string[] { item.ProductCD!, item.ColorCD!, item.SizeCD!, item.JanCode1!, item.JanCode2!,
                                item.JanCode3!, item.Memo!, item.UseFlag.ToString()!, item.ScheduledProductionQuantity.ToString()!, item.CuttingQuantity.ToString()!, item.TagNumber.ToString()!, item.AutoAllocationFlag.ToString()!, item.RetailPrice.ToString()!, item.SupplierPrice.ToString()!, item.ForeignCurrencyPrice.ToString()!, item.CostPrice.ToString()!});

            if (ret.Code == 0)
            {
                item.SeqNo = ret.NewSeq;
                item.VdateUpdate = decimal.Parse(ret.VDate);
                item.VdateCreate = item.VdateUpdate;
                Common.ConvertDotStringDel(item);
                ListShohinJan!.Add(item);
                SelectedProduct = item;
                ClientLib.MessageBoxOk(this, "登録しました");
                timex = EditProduct.VdateUpdate;
            }
            else
            {
                ClientLib.MessageBoxError(this, ret.Code.ToString());
            }



        }

        // for 印刷 button
        [RelayCommand]
        async Task DoPrintAsync()
        {
            if (!ClientLib.MessageBox(this, "印刷しますか？")) return;
            ClientLib.CursorToWait();
            if (ListShohinJan == null || ListShohinJan.Count == 0) return;

            //var getCateShhojan = AppData.ClassCvnet.comboItem00.GetCaseStr()
        }

        private DataTable OnQuery(string[] v_para, string[] v_para2, int para1)
        {
            string sql_query = "SELECT A.SEQ_NO,A.VDATE_CREATE,A.VDATE_UPDATE";

            if (v_para2[0] == "1" || v_para2[0] == "2")
            {
                // Append every column from the list with prefix A.
                for (int i = 0; i < sql_col_list.Length - 1; i++)
                {
                    sql_query += ", A." + sql_col_list[i];
                }
                sql_query += ",B.商品名,GET_COLORNAME(A.色CD) 色名,GET_SIZENAME(A.商品CD,A.サイズCD) サイズ名,GET_GENKA(B.商品CD,0,'20991231',A.色CD,A.サイズCD) 原価1";
                sql_query += ",ROW_NUMBER() OVER (ORDER BY A.商品CD,A.色CD,A.サイズCD) 行NO";
                sql_query += " FROM HC$MASTER_SHOHIN_JAN A, HC$MASTER_SHOHIN B";
                sql_query += " WHERE ";
                if (!string.IsNullOrEmpty(v_para[0]) && v_para[0] != ".") sql_query += $"A.商品CD = '{v_para[0]}' AND ";
                if (!string.IsNullOrEmpty(v_para[1]) && v_para[1] != ".") sql_query += $"A.色CD='{v_para[1]}' AND ";
                if (!string.IsNullOrEmpty(v_para[2]) && v_para[2] != ".") sql_query += $"A.サイズCD='{v_para[2]}' AND ";
                if (!string.IsNullOrEmpty(v_para[3]) && v_para[3] != ".") sql_query += $"A.JANコード1='{v_para[3]}' AND ";
                if (!string.IsNullOrEmpty(v_para[4]) && v_para[4] != ".") sql_query += $"A.JANコード2='{v_para[4]}' AND ";
                if (!string.IsNullOrEmpty(v_para[5]) && v_para[5] != ".") sql_query += $"A.JANコード3='{v_para[5]}' AND ";
                sql_query += "A.商品CD=B.商品CD";
                sql_query = "SELECT * FROM (" + sql_query + ") ";
                if (v_para2[0] == "2")
                    sql_query += " where 行NO between " + Convert.ToString(para1) + " AND " + Convert.ToString(para1 + 40);
                else if (v_para2[0] == "1")
                    sql_query += " where 行NO between " + Convert.ToString(para1) + " AND " + Convert.ToString(para1 + 40);
                sql_query += " order by 商品CD,色CD,サイズCD";

                v_para2[0] = null;      // Reset 
            }
            else
            {
                for (int i = 0; i < sql_col_list.Length - 1; i++)
                {
                    sql_query += ", A." + sql_col_list[i];
                }
                sql_query += ",B.商品名,GET_COLORNAME(A.色CD) 色名,GET_SIZENAME(A.商品CD,A.サイズCD) サイズ名,GET_GENKA(B.商品CD,0,'20991231',A.色CD,A.サイズCD) 原価1";
                sql_query += " FROM HC$MASTER_SHOHIN_JAN A, HC$MASTER_SHOHIN B";
                sql_query += " WHERE ";
                if (!string.IsNullOrEmpty(v_para[0]) && v_para[0] != ".") sql_query += $"A.商品CD = '{v_para[0]}' AND ";
                if (!string.IsNullOrEmpty(v_para[1]) && v_para[1] != ".") sql_query += $"A.色CD='{v_para[1]}' AND ";
                if (!string.IsNullOrEmpty(v_para[2]) && v_para[2] != ".") sql_query += $"A.サイズCD='{v_para[2]}' AND ";
                if (!string.IsNullOrEmpty(v_para[3]) && v_para[3] != ".") sql_query += $"A.JANコード1='{v_para[3]}' AND ";
                if (!string.IsNullOrEmpty(v_para[4]) && v_para[4] != ".") sql_query += $"A.JANコード2='{v_para[4]}' AND ";
                if (!string.IsNullOrEmpty(v_para[5]) && v_para[5] != ".") sql_query += $"A.JANコード3='{v_para[5]}' AND ";
                sql_query += "A.商品CD=B.商品CD";
                sql_query += " ORDER BY A.商品CD,A.色CD,A.サイズCD";
                sql_query = AppData.ClassCvnet.GetSqlDisp(sql_query);
            }

            var ret_csv = AppData.Http?.AspxSqlQuery(sql_query);

            return ret_csv;
        }

        /// <summary>
        /// 共通処理: 商品JAN一覧の検索＋ListShohinJan 更新＋CanNext/SelectedProduct 設定
        /// 他のメソッドからも呼び出せるようにする。
        /// </summary>
        private void ExecuteShohinJanSearch(int para)
        {
            if (FindProduct == null) return;

            // v_para: 商品CD/色CD/サイズCD/JAN1/2/3
            var v_para = BuildVpara(FindProduct);   // BizArray

            // ★ ここで v_para2 は既にどこかでセットされている前提
            var wrk_csv2 = OnQuery(v_para.ToArray(), v_para2.ToArray(), para);

            var list = ConvertToMasterShohinJanList(wrk_csv2);

            if (list.Count >= AppData.maxQueryCnt)
            {
                // There is more data → enable Next
                list = list.Take(AppData.maxQueryCnt).ToList();
                CanNext = true;
            }
            else
            {
                // End reached
                CanNext = false;
            }

            UpdatePagingFlags();
            // 小数点などのフォーマット調整
            Common.ConvertDotStringDel(list);

            ListShohinJan = new ObservableCollection<MasterShohinJan>(list);

            if (ListShohinJan.Count > 0)
            {
                SelectedProduct = ListShohinJan[0];
            }
        }



        /// Converts a DataTable to a list of MasterShohinJan objects.
        private static List<MasterShohinJan> ConvertToMasterShohinJanList(DataTable wrk_csv2)
        {
            var list = new List<MasterShohinJan>();

            if (wrk_csv2 == null || wrk_csv2.Rows.Count == 0)
                return list;

            foreach (DataRow dr in wrk_csv2.Rows)
            {
                try
                {
                    var item = new MasterShohinJan
                    {
                        SeqNo = Convert.ToInt64(dr["SEQ_NO"]),
                        VdateCreate = Convert.ToDecimal(dr["VDATE_CREATE"]),
                        VdateUpdate = Convert.ToDecimal(dr["VDATE_UPDATE"]),
                        ProductCD = dr["商品CD"].ToString() ?? string.Empty,
                        ColorCD = dr["色CD"].ToString() ?? string.Empty,
                        SizeCD = dr["サイズCD"].ToString() ?? string.Empty,
                        JanCode1 = dr["JANコード1"].ToString() ?? string.Empty,
                        JanCode2 = dr["JANコード2"].ToString() ?? string.Empty,
                        JanCode3 = dr["JANコード3"].ToString() ?? string.Empty,
                        Memo = dr["メモ"].ToString() ?? string.Empty,
                        UseFlag = Convert.ToInt32(dr["使用FLG"]),
                        ScheduledProductionQuantity = Convert.ToDecimal(dr["生産予定数"]),
                        CuttingQuantity = Convert.ToDecimal(dr["裁断数"]),
                        TagNumber = Convert.ToDecimal(dr["下札枚数"]),
                        AutoAllocationFlag = Convert.ToInt32(dr["自動配分FLG"]),
                        RetailPrice = Convert.ToDecimal(dr["上代"]),
                        SupplierPrice = Convert.ToInt32(dr["仕入価格"]),
                        ForeignCurrencyPrice = Convert.ToInt32(dr["外貨仕入価格"]),
                        ProductName = dr["商品名"].ToString() ?? string.Empty,
                        ColorName = dr["色名"].ToString() ?? string.Empty,
                        SizeName = dr["サイズ名"].ToString() ?? string.Empty,
                        CostPrice = Convert.ToDecimal(dr["原価1"]),
                    };

                    list.Add(item);
                }
                catch (Exception ex)
                {
                    // Log or handle conversion issues gracefully
                    System.Diagnostics.Debug.WriteLine($"Row conversion error: {ex.Message}");
                }
            }

            return list;
        }


        private static BizArray BuildVpara(MasterShohinJan? findProduct)
        {
            var v_para = new BizArray();

            // Even if findProduct is null, fill with empty strings for all 6 expected elements
            if (findProduct == null)
            {
                for (int i = 0; i < 6; i++)
                    v_para.Add("");
                return v_para;
            }

            v_para.Add(findProduct.ProductCD?.Trim() ?? "");
            v_para.Add(findProduct.ColorCD?.Trim() ?? "");
            v_para.Add(findProduct.SizeCD?.Trim() ?? "");
            v_para.Add(findProduct.JanCode1?.Trim() ?? "");
            v_para.Add(findProduct.JanCode2?.Trim() ?? "");
            v_para.Add(findProduct.JanCode3?.Trim() ?? "");

            return v_para;
        }
    }
}