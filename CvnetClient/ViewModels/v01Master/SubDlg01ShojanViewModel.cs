using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using static CvnetClient.ViewModels.SubDlg09Upkeihi2ViewModel;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01ShojanViewModel : BaseViewModel
    {
        [ObservableProperty]
        ObservableCollection<MasterShohin>? listProduct;

        //[ObservableProperty]
        //      MasterShohin? editProduct;

        [ObservableProperty]
        MasterShohinJan? editProduct;

        //[ObservableProperty]
        //ObservableCollection<DeptItem>? deptList;

        //[ObservableProperty]
        //FlagOpt? selectedItem;

        [ObservableProperty]
        public Dictionary<int, string>? chushi;

        [ObservableProperty]
        public Dictionary<int, string>? jidohaibun;

        [ObservableProperty]
        ObservableCollection<MasterShohinJan>? listShohinJan;

        //[ObservableProperty]
        //string? selectedColorText;

        //[ObservableProperty]
        //string? selectedSizeText;

        [ObservableProperty]
        MasterShohinJan? selectedProduct;

        [ObservableProperty]
        string? selProductCd;

        [ObservableProperty]
        int? pageNow;

        [ObservableProperty]
        int? pageTotal;

        private BizArray col_list;

        private bool _canNext;
        public bool CanNext
        {
            get => _canNext;
            set => SetProperty(ref _canNext, value);
        }

        private bool _canBack;
        public bool CanBack
        {
            get => _canBack;
            set => SetProperty(ref _canBack, value);
        }

        public void OnInit()
        {
            SelectedProduct = new MasterShohinJan();
            EditProduct = new MasterShohinJan();

            string sql_query_init = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='KFK' order by a.名称CD";

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

        }

        string[] sql_col_list =
        {
            "商品CD", "色CD", "サイズCD", "JANコード1", "JANコード2", "JANコード3",
            "メモ", "使用FLG", "生産予定数", "裁断数", "下札枚数",
            "自動配分FLG", "上代", "仕入価格", "外貨仕入価格", "原価"
        };

        [RelayCommand]
        void DoList()
        {
            if (!ClientLib.MessageBox(this, "本当にしますか？")) return;

        }

        [RelayCommand]
        public void SelDspUpdate()
        {
            if (SelectedProduct == null) return;
            //SearchOpt.YearTbCor = 0;
            //SearchOpt.WeekNoTbCor = 0;

            var v_para = new BizArray();
            v_para[0] = SelectedProduct.ProductCD != null ? SelectedProduct.ProductCD.Trim() : "";
            v_para[1] = SelectedProduct.ColorCD != null ? SelectedProduct.ColorCD.Trim() : "";
            v_para[2] = SelectedProduct.SizeCD != null ? SelectedProduct.SizeCD.Trim() : "";
            v_para[3] = SelectedProduct.JanCode1 != null ? SelectedProduct.JanCode1.Trim() : "";
            v_para[4] = SelectedProduct.JanCode2 != null ? SelectedProduct.JanCode2.Trim() : "";
            v_para[5] = SelectedProduct.JanCode2 != null ? SelectedProduct.JanCode2.Trim() : "";

            var v_para2 = new BizArray();

            var wrk_csv2 = OnQuery(v_para.ToArray(), v_para2.ToArray());

            var list = (from DataRow dr in wrk_csv2.Rows
                        select new MasterShohinJan
                        {
                            SeqNo = Convert.ToInt64(dr["SEQ_NO"]),
                            VdateCreate = Convert.ToDecimal(dr["VDATE_CREATE"]),
                            VdateUpdate = Convert.ToDecimal(dr["VDATE_UPDATE"]),
                            ProductCD = dr["商品CD"].ToString() ?? string.Empty,
                            ColorCD = dr["色CD"].ToString() ?? string.Empty,
                            SizeCD = dr["サイズCD"].ToString() ?? string.Empty,
                            ProductName = dr["商品名"].ToString() ?? string.Empty,
                        });

            if (list.Count() > AppData.maxQueryCnt)
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

        }

        [RelayCommand]
        public void SelProduct(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditProduct != null)
            {
                EditProduct.ProductCD = get_sel00.Code;
                EditProduct.ProductName = get_sel00.Name;
            }
        }

        [RelayCommand]
        public void SelColor(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                // Put the selected color string into the bindable property
                EditProduct.ColorCD = get_sel00.Name;
            }
        }

        [RelayCommand]
        public void SelSize(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                // Put the selected color string into the bindable property
                EditProduct.SizeCD = get_sel00.Name;
            }
        }

        [RelayCommand]
        public void SelJan(object value)
        {
            
        }

        // for 更新 button
        [RelayCommand]
        void DoUpdate()
        {
            if (!ClientLib.MessageBox(this, "修正しますか？")) return;
        }

        // for 削除 button
        [RelayCommand]
        void DoDelete()
        {
            if (!ClientLib.MessageBox(this, "削除しますか？")) return;


        }

        // for 追加 button
        [RelayCommand]
        void DoInsert()
        {
            if (!ClientLib.MessageBox(this, "新規登録しますか？")) return;
        }

        // for 印刷 button
        [RelayCommand]
        async Task DoPrintAsync()
        {
            if (!ClientLib.MessageBox(this, "印刷しますか？")) return;
        }

        private DataTable OnQuery(string[] v_para, string[] v_para2)
        {
            string sql_query = "SELECT A.SEQ_NO,A.VDATE_CREATE,A.VDATE_UPDATE";
            // Append every column from the list with prefix A.
            for (int i = 0; i < sql_col_list.Length; i++)
            {
                sql_query += ", A." + sql_col_list[i];
            }
            sql_query += ",B.商品名,GET_COLORNAME(A.色CD) 色名,GET_SIZENAME(A.商品CD,A.サイズCD) サイズ名,GET_GENKA(B.商品CD,0,'20991231',A.色CD,A.サイズCD) 原価1";
            //sql_query += ",ROW_NUMBER() OVER (ORDER BY A.商品CD,A.色CD,A.サイズCD) 行NO";
            sql_query += " FROM HC$MASTER_SHOHIN_JAN A, HC$MASTER_SHOHIN B";
            sql_query += " WHERE ";
            //if (str(v_para[0]) != "" && str(v_para[0]) != ".") sql_query += "A.商品CD='" + str(v_para[0]) + "' and ";
            if (!string.IsNullOrEmpty(v_para[0]) && v_para[0] != ".") sql_query += $"A.商品CD = '{v_para[0]}' AND ";
            if (!string.IsNullOrEmpty(v_para[1]) && v_para[1] != ".") sql_query += $"A.色CD='{v_para[1]}' AND ";
            if (!string.IsNullOrEmpty(v_para[2]) && v_para[2] != ".") sql_query += $"A.サイズCD='{v_para[2]}' AND ";
            if (!string.IsNullOrEmpty(v_para[3]) && v_para[3] != ".") sql_query += $"A.JANコード1='{v_para[3]}' AND ";
            if (!string.IsNullOrEmpty(v_para[4]) && v_para[4] != ".") sql_query += $"A.JANコード2='{v_para[4]}' AND ";
            if (!string.IsNullOrEmpty(v_para[5]) && v_para[5] != ".") sql_query += $"A.JANコード3='{v_para[5]}' AND ";
            sql_query += "A.商品CD=B.商品CD";

            sql_query += " ORDER BY A.商品CD,A.色CD,A.サイズCD";
            sql_query = AppData.ClassCvnet.GetSqlDisp(sql_query);
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_query);

        //    //if (v_para2[0] == "1") // 次へ (Next Page)
        //    //{
        //    //    sql_query += $" WHERE 行NO BETWEEN {Form1.sv_end_cnt} " +
        //    //                 $"AND {Form1.sv_end_cnt + cvnet.MaxCntDisp - 1}";
        //    //}
        //    //else // 前へ (Previous Page)
        //    //{
        //    //    sql_query += $" WHERE 行NO BETWEEN {Form1.sv_end_cnt - cvnet.MaxCntDisp + 1} " +
        //    //                 $"AND {Form1.sv_end_cnt}";
        //    //}

            //    //sql_query += " ORDER BY 商品CD, 色CD, サイズCD";


            //    //var ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            return ret_csv;
        }
    }
}