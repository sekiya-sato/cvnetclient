using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgSKU01ViewModel : BaseViewModel
    {
        #region Binding Variable
        [ObservableProperty]
        public string? m_Title = "SKU入力";

        [ObservableProperty]
        public Sku01Dsp m_Sku01Dsp = new Sku01Dsp();

        [ObservableProperty]
        public DataTable m_Spread1;

        [ObservableProperty]
        public int? m_SpreadRowIdx = 0;
        #endregion

        #region Private Variable
        private DataTable csv_col;
        private DataTable csv_siz;
        private int kake = 0;
        /* 仕入値追加 2008.06.17 */
        private DataTable siirene;
        /* 納品日追加 2008.09.26 */
        private DateTime nouhinbi;
        private int ColSizFlg = 0;
        private int gedaihasu = 0;
        private int gedaiketa = 0;
        private int genka_keta = 0;
        /* 原価 */
        private int genka = 0;
        /* 消費税計算方法 */
        private int shohi = 1;
        /* 仕入区分 */
        private string TextLabel8;

        /* 色サイズのチェック */
        List<dynamic> ColSizList;

        /// <summary>
        /// Current Dialog return value
        /// </summary>
        public List<BizArray> ret_para;
        #endregion

        /************/
        /* 初期処理 */
        /************/
        public void OnInit(string[] init_para, string[] init_para2 = null)
        {
            try
            {
                ColSizFlg = AppData.ClassCvnet.config.ColSizFlg;
                if (ColSizFlg == 1)
                {
                    //Form1.Spread1.FlexRecord1.Flex0.Title = "色/ｻｲｽﾞ";
                    Sku01Dsp.Lbl4Title = "ｻｲｽﾞ";
                    Sku01Dsp.Lbl5Title = "色";
                }
                else
                {
                    Sku01Dsp.Lbl4Title = "色";
                    Sku01Dsp.Lbl5Title = "ｻｲｽﾞ";
                }

                var v_para = new BizArray();
                v_para[0] = init_para[0];
                if (init_para.Length > 1)
                {
                    v_para[0] = init_para[1];
                    v_para[1] = init_para[2];
                    v_para[2] = init_para[0];
                    v_para[3] = init_para[0];
                }

                var v_jodai = "";
                if (init_para2 != null && init_para2.Length > 0)
                {
                    v_jodai = ",'" + init_para2[0] + "','" + init_para2[1] + "'";
                    if (init_para2.Length > 2)
                    {
                        /* 受注・出荷売以外は(掛率・上代)修正不可 */
                        if (init_para2[2] == "0" || init_para2[2] == "12")
                        {
                            Sku01Dsp.RetailPercentActive = true;
                            Sku01Dsp.RetailTotalActive = true;
                        }
                    }
                }

                var sql_str = "";
                if (AppData.ClassCvnet.UserFlg == 87)
                {
                    sql_str = "select j.色CD,j.サイズCD,get_jodai(j.商品CD,j.色CD,j.サイズCD" + v_jodai + ") 上代"
                            + " , CASE WHEN (SELECT 仕入価格 FROM hc$master_shohin_jan WHERE 商品cd = j.商品cd AND 色cd = j.色cd AND サイズcd = j.サイズcd ) IS NULL OR ( (SELECT 仕入価格 FROM hc$master_shohin_jan WHERE 商品cd = j.商品cd AND 色cd = j.色cd AND サイズcd = j.サイズcd ) = 0 ) THEN (SELECT 仕入価格 FROM HC$MASTER_SHOHIN WHERE 商品CD=j.商品CD) ELSE (SELECT 仕入価格 FROM hc$master_shohin_jan WHERE 商品cd = j.商品cd AND 色cd = j.色cd AND サイズcd = j.サイズcd) END AS 原価"
                            + ",NVL(s.商品名,'') 商品名"
                            + ",NVL(s.消費税計算方法,'') 消費税計算方法";
                }
                else
                {
                    sql_str = "select j.色CD,j.サイズCD,get_jodai(j.商品CD,j.色CD,j.サイズCD" + v_jodai + ") 上代"
                            + " , CASE WHEN (SELECT 仕入価格 FROM hc$master_shohin_jan WHERE 商品cd = j.商品cd AND 色cd = j.色cd AND サイズcd = j.サイズcd ) IS NULL OR ( (SELECT 仕入価格 FROM hc$master_shohin_jan WHERE 商品cd = j.商品cd AND 色cd = j.色cd AND サイズcd = j.サイズcd ) = 0 ) THEN (SELECT 仕入価格 FROM HC$MASTER_SHOHIN WHERE 商品CD=j.商品CD) ELSE (SELECT 仕入価格 FROM hc$master_shohin_jan WHERE 商品cd = j.商品cd AND 色cd = j.色cd AND サイズcd = j.サイズcd) END AS 原価"
                            + ",NVL(s.商品名,'') 商品名"
                            + ",NVL(s.消費税計算方法,'') 消費税計算方法";
                }

                if (init_para.Length > 1)
                    sql_str += ",Get_Kakeritu(:1,:2,:3) 下代掛率";
                else
                    sql_str += ",0 下代掛率";

                /* 仕入値追加 2008.06.17 */
                sql_str += ", CASE WHEN (SELECT 仕入価格 FROM hc$master_shohin_jan WHERE 商品cd = j.商品cd AND 色cd = j.色cd AND サイズcd = j.サイズcd ) IS NULL OR ( (SELECT 仕入価格 FROM hc$master_shohin_jan WHERE 商品cd = j.商品cd AND 色cd = j.色cd AND サイズcd = j.サイズcd ) = 0 ) THEN (SELECT 仕入価格 FROM HC$MASTER_SHOHIN WHERE 商品CD=j.商品CD) ELSE (SELECT 仕入価格 FROM hc$master_shohin_jan WHERE 商品cd = j.商品cd AND 色cd = j.色cd AND サイズcd = j.サイズcd) END AS 原価";

                /* 納品日・絵型追加 2008.09.26 */
                sql_str += ",NVL(s.納品日,'19010101') 納品日";
                sql_str += ",NVL(s.絵型名,'.') 絵型名";
                sql_str += ",NVL(s.メーカー品番,'.') MKR品番";
                sql_str += ",NVL(s.仕入区分||' '||decode(s.仕入区分,1,'買取',2,'委託',3,'消化',''),'') 仕入区分";

                sql_str += " from HC$master_shohin_jan j,HC$master_shohin s";
                sql_str += " where j.商品CD=s.商品CD(+) and j.商品CD=:1";

                /* ｴｽﾗｸﾞｼﾞｭｰﾙ対応 */
                if (AppData.ClassCvnet.config.UserFlg == 56) sql_str += " and s.承認FLG=1";
                sql_str += " order by j.色CD,j.サイズCD";
                var ret_csv = AppData.Http!.AspxSqlQuery(sql_str, v_para.ToArray());

                kake = int.TryParse(ret_csv.Rows[0][6].ToString(), out int _kake) ? _kake : 0;
                Sku01Dsp.RetailPercent = kake;

                if (ret_csv.Rows.Count == 0)
                {
                    ClientLib.MessageBoxError(this, "商品マスタ、もしくは商品色サイズマスタがありませんでした", "注意");
                    return;
                }

                Sku01Dsp.ProdCD01 = init_para[0];
                gedaihasu = int.TryParse(init_para[4], out int _gedaihasu) ? _gedaihasu : 0;
                gedaiketa = int.TryParse(init_para[3], out int _gedaiketa) ? _gedaiketa : 0;

                v_para = new BizArray();
                v_para[0] = init_para[0];   /* 商品CD */
                /* 画面表示処理 */
                CreateDsp(ret_csv, v_para);

                /* 専門店 */
                if (AppData.ClassCvnet.config.smtflg == 1)
                {
                    Sku01Dsp.ProdCD02Visible = Visibility.Visible;
                }
            }
            catch { }
        }

        /*
	        展示会受注の入力補助画面から呼び出される
	        init_para[0] : 商品コード
	        init_para[1] : 在庫計上日
	        init_para[2] : 倉庫CD
	        init_para[3] : 掛率
	        init_para[4] : 色サイズフラグ
	        init_para[5] : 下代桁切
	        init_para[6] : 下代端数
	        init_para[7] : 下代単価（強制的にこの値にしたい場合のみセット）
	
	        既に入力されているデータ
	        init_data[i][0] : 色CD
	        init_data[i][1] : サイズCD
	        init_data[i][2] : 数量
	    */
        public void OnInit2(string[] init_para, DataTable init_data = null)
        {
            Sku01Dsp.ProdCD01 = init_para[0];
            gedaiketa = int.TryParse(init_para[5], out int _gedaiketa) ? _gedaiketa : 0;
            gedaihasu = int.TryParse(init_para[5], out int _gedaihasu) ? _gedaihasu : 0;

            /* 掛率 */
            kake = int.TryParse(init_para[3].ToString(), out int _kake) ? _kake : 0;
            Sku01Dsp.RetailPercent = kake;

            /* 掛率の編集は可能に */
            Sku01Dsp.RetailPercentActive = true;
            Sku01Dsp.RetailTotalActive = true;

            /* 色サイズフラグ */
            ColSizFlg = int.TryParse(init_para[4], out int _colsiz) ? _colsiz : 0;
            if (ColSizFlg == 1)
            {
                Sku01Dsp.Lbl4Title = "ｻｲｽﾞ";
                Sku01Dsp.Lbl5Title = "色";
            }
            else
            {
                Sku01Dsp.Lbl4Title = "色";
                Sku01Dsp.Lbl5Title = "ｻｲｽﾞ";
            }

            var v_para = new BizArray();
            v_para[0] = init_para[1];
            v_para[1] = init_para[2];
            v_para[2] = init_para[1];
            v_para[3] = init_para[3];
            v_para[4] = init_para[0];

            var sql_str = "";
            sql_str += "select j.色CD,j.サイズCD,get_jodai(j.商品CD,j.色CD,j.サイズCD, :0, :1) 上代";
            /* 2012.11.06 okamoto upd start */
            if (AppData.ClassCvnet.config.UserFlg == 87)
            {
                sql_str += ",NVL(s.仕入価格,0) 原価";
            }
            else
            {
                sql_str += " , CASE WHEN (SELECT 仕入価格 FROM hc$master_shohin_jan WHERE 商品cd = j.商品cd AND 色cd = j.色cd AND サイズcd = j.サイズcd ) IS NULL OR ( (SELECT 仕入価格 FROM hc$master_shohin_jan WHERE 商品cd = j.商品cd AND 色cd = j.色cd AND サイズcd = j.サイズcd ) = 0 ) THEN (SELECT 仕入価格 FROM HC$MASTER_SHOHIN WHERE 商品CD=j.商品CD) ELSE (SELECT 仕入価格 FROM hc$master_shohin_jan WHERE 商品cd = j.商品cd AND 色cd = j.色cd AND サイズcd = j.サイズcd) END AS 原価";
            }
            /* 2012.11.06 okamoto upd end */

            /* 2013.01.31 sato upd start */
            if (AppData.ClassCvnet.config.UserFlg == 87)
            {
                v_para = new BizArray();
                v_para[0] = init_para[1];
                v_para[1] = init_para[2];
                v_para[2] = init_para[3];
                v_para[3] = init_para[0];
            }
            /* 2013.01.31 sato upd end */

            sql_str += ",NVL(s.商品名,'') 商品名";
            sql_str += ",NVL(s.消費税計算方法,'') 消費税計算方法";
            sql_str += ",:3 下代掛率";

            sql_str += " , CASE WHEN (SELECT 仕入価格 FROM hc$master_shohin_jan WHERE 商品cd = j.商品cd AND 色cd = j.色cd AND サイズcd = j.サイズcd ) IS NULL OR ( (SELECT 仕入価格 FROM hc$master_shohin_jan WHERE 商品cd = j.商品cd AND 色cd = j.色cd AND サイズcd = j.サイズcd ) = 0 ) THEN (SELECT 仕入価格 FROM HC$MASTER_SHOHIN WHERE 商品CD=j.商品CD) ELSE (SELECT 仕入価格 FROM hc$master_shohin_jan WHERE 商品cd = j.商品cd AND 色cd = j.色cd AND サイズcd = j.サイズcd) END AS 原価";
            sql_str += ",NVL(s.納品日,'19010101') 納品日";
            sql_str += ",NVL(s.絵型名,'.') 絵型名";
            sql_str += ",NVL(s.メーカー品番,'.') MKR品番";

            sql_str += " from HC$master_shohin_jan j,HC$master_shohin s";
            sql_str += " where j.商品CD=s.商品CD(+) and j.商品CD=:4";

            /* ｴｽﾗｸﾞｼﾞｭｰﾙ対応 */
            if (AppData.ClassCvnet.config.UserFlg == 56) sql_str += " and s.承認FLG=1";
            sql_str += " order by j.色CD,j.サイズCD";

            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, v_para.ToArray());
            if (ret_csv.Rows.Count == 0)
            {
                ClientLib.MessageBoxError(this, "商品マスタ、もしくは商品色サイズマスタがありませんでした", "注意");
                return;
            }

            kake = int.TryParse(ret_csv.Rows[0][6].ToString(), out int _kake2) ? _kake2 : 0;
            Sku01Dsp.RetailPercent = kake;

            v_para = new BizArray();
            v_para[0] = init_para[0];   /* 商品CD */
            /* 画面表示処理 */
            CreateDsp(ret_csv, v_para);
            SetData(init_data);


            int para7 = int.TryParse(init_para[7], out int _para7) ? _para7 : 0;
            if (!string.IsNullOrEmpty(init_para[7]) && para7 > 0)
            {
                /* 値が入っている場合は、強制的に下代単価をセット */
                Sku01Dsp.RetailTotal = para7;
            }

            Sku01Dsp.LblCostBranchVisible = Visibility.Collapsed;
            Sku01Dsp.TxtCostBranchVisible = Visibility.Collapsed;
        }

        #region Events
        [RelayCommand]
        public void DoExecute()
        {
            if (Spread1.Rows.Count == 0) return;
            ret_para = new List<BizArray>();
            int colnum = csv_col.Rows.Count;
            for (int i = 0; i < Spread1.Rows.Count; i++)
            {
                /* var posi */
                for (int j = 0; j < colnum; j++)
                {
                    var v_para = new BizArray();
                    int childtxt = int.TryParse(Spread1.Rows[i][j+1].ToString(), out int _num) ? _num : 0; 
                    if (childtxt > 0)
                    {
                        v_para = new BizArray();
                        v_para[0] = Sku01Dsp.ProdCD01; /* 品番 */
                        v_para[1] = Sku01Dsp.ProdName; /* 商品名 */ 
                        v_para[2] = (Sku01Dsp.RetailPrice * childtxt).ToString(); /* 販売単価*数量 */ /* ここでは上代*数量 */
                        v_para[3] = string.Empty; /* 絵型 */
                        v_para[4] = string.Empty; /* 大分類 */
                        v_para[5] = string.Empty; /* 中分類 */
                        v_para[6] = string.Empty; /* 小分類 */
                        v_para[7] = string.Empty; /* 発売日 */
                        v_para[8] = (genka * childtxt).ToString(); /* 仕入単価*数量 */ /* ここでは原価*数量 */
                        v_para[9] = shohi.ToString(); /* 消費税計算方法 */
                        v_para[10] = string.Empty; /* カテゴリー */
                        v_para[11] = string.Empty; /* セットFLG */
                        v_para[12] = string.Empty; /* 延長保証FLG 05.08.26*/
                        if (ColSizFlg == 1)
                        {
                            v_para[13] = csv_siz.Rows[SpreadRowIdx ?? 0][0].ToString(); /* サイズCD */
                            v_para[14] = csv_col.Rows[j][0].ToString(); /* 色CD */
                            v_para[15] = csv_siz.Rows[SpreadRowIdx ?? 0][1].ToString(); /* サイズ名 */
                            v_para[16] = csv_col.Rows[j][1].ToString(); /* 色名 */
                        }
                        else
                        {
                            v_para[13] = csv_col.Rows[j][0].ToString(); /* 色CD */
                            v_para[14] = csv_siz.Rows[SpreadRowIdx ?? 0][0].ToString(); /* サイズCD */
                            v_para[15] = csv_col.Rows[j][1].ToString(); /* 色名 */
                            v_para[16] = csv_siz.Rows[SpreadRowIdx ?? 0][1].ToString(); /* サイズ名 */
                        }
                        v_para[17] = string.Empty; /* 単品管理FLG */
                        v_para[18] = childtxt.ToString(); /* 数量 */

                        v_para[19] = (Sku01Dsp.RetailPrice * childtxt).ToString(); /* 販売単価*数量 */ /* ここでは上代*数量 */
                        v_para[20] = "0";
                        v_para[21] = (genka * childtxt).ToString(); /* 仕入単価*数量 */ /* ここでは原価*数量 */
                        v_para[22] = "0";
                        v_para[23] = Sku01Dsp.RetailPrice.ToString(); /* 販売単価 */ /* ここではマスタ上代 */
                        v_para[24] = genka.ToString(); /* 仕入単価 */ /* ここでは原価 */
                        v_para[25] = Sku01Dsp.RetailPrice.ToString();  /* マスタ上代 */
                        
                        /* 在庫数、引当数追加 */
                        v_para[26] = "0";
                        v_para[27] = "0";

                        /* 下代掛率を追加 */
                        v_para[28] = kake.ToString();

                        /* 仕入値追加 2008.06.17 */
                        v_para[29] = siirene.Rows[j][7].ToString();

                        /* 下代単価・掛率の追加 */
                        v_para[30] = Sku01Dsp.RetailTotal.ToString();
                        v_para[31] = Sku01Dsp.RetailPercent.ToString();

                        /* 納品日追加 2008.09.26 */
                        v_para[32] = nouhinbi.ToString("yyyyMMdd");
                        /* メーカー品番追加 2008.11.06 */
                        v_para[33] = Sku01Dsp.ProdCD02;
                        /* 仕入区分追加 2009.10.06 */
                        v_para[34] = TextLabel8;
                        /* 原価枝番追加 2010.02.10 */
                        v_para[35] = Sku01Dsp.CostBranch.ToString();

                        ret_para.Add(v_para);
                    }
                }
            } 
            ClientLib.ExitDialogResult(this, true);
        }

        [RelayCommand]
        private void RetailPercentChanged()
        {
            // kakeritu change 
            Sku01Dsp.RetailTotal = OnGetGedai(Sku01Dsp.RetailPrice, Sku01Dsp.RetailPercent);
        }

        [RelayCommand]
        private void DataGridBeginEdit(DataGridBeginningEditEventArgs e)
        {
            if (e.Row.Item is not DataRowView rowView)
                return;

            try
            {
                int rowIndex = rowView.Row.Table.Rows.IndexOf(rowView.Row);

                int columnIndex = rowView.Row.Table.Columns
                                       .IndexOf(e.Column.SortMemberPath);

                Sku01Dsp.ProdColor = new BtListHelper(csv_col.Rows[columnIndex - 1][0].ToString(), csv_col.Rows[columnIndex - 1][1].ToString());
                Sku01Dsp.ProdSize = new BtListHelper(csv_siz.Rows[rowIndex][0].ToString(), csv_siz.Rows[rowIndex][1].ToString());
                 
                int result = ColSizList.Where(x => x.SizCode == Sku01Dsp.ProdSize.Code && x.ColorCode == Sku01Dsp.ProdColor.Code).Count();
                if (result <= 0)
                {
                    e.Cancel = true;
                    System.Windows.MessageBox.Show("現在の商品のサイズおよびカラーは、数量を設定できません。", "確認", System.Windows.MessageBoxButton.OK);
                }
            }
            catch { }
        }

        [RelayCommand]
        public void DoExit()
        {
            ClientLib.ExitDialogResult(this, true);
        }
        #endregion

        #region Function
        /****************/
        /* 画面表示処理 */
        /****************/
        private void CreateDsp(DataTable ret_csv, BizArray v_para)
        {
            genka = int.TryParse(ret_csv.Rows[0][3].ToString(), out int _genka) ? _genka : 0;
            Sku01Dsp.ProdName = ret_csv.Rows[0][4].ToString();
            shohi = int.TryParse(ret_csv.Rows[0][5].ToString(), out int _shohi) ? _shohi : 0;

            /* 仕入値追加 2008.06.17 */
            siirene = ret_csv;

            /* 上代を追加 */
            Sku01Dsp.RetailPrice = int.TryParse(ret_csv.Rows[0][2].ToString(), out int _retail) ? _retail : 0;
            /* 納品日・絵型追加 2008.09.26 */
            nouhinbi = DateTime.ParseExact(ret_csv.Rows[0][8].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);
            Sku01Dsp.ProdCD02 = ret_csv.Rows[0][10].ToString();
            /* 仕入区分追加 2009.10.06 */
            TextLabel8 = ret_csv.Rows[0][11].ToString();
            string egata = ret_csv.Rows[0][9].ToString();

            var inscsv = new DataTable();
            inscsv.Columns.Add(new DataColumn("Flex0", typeof(string)));

            string siz_mei = "get_sizename(商品CD,サイズCD)";
            string col_mei = "get_colorname(色CD)";

            if (AppData.ClassCvnet.UserFlg == 87)
            {
                siz_mei = "GET_SIZENAME2(B.予備05,A.サイズCD)";
                col_mei = "get_colorname2(B.展示会CD,B.予備04,A.色CD)";
            }
            if (AppData.ClassCvnet.config.ColSizMei == 1) { siz_mei = "サイズ名"; col_mei = "色名"; }

            string sql_str;
            if (ColSizFlg == 1)
                sql_str = "select distinct サイズCD," + siz_mei + " サイズ from HC$master_shohin_jan where 商品CD=:1 order by サイズCD";
            else
                sql_str = "select distinct 色CD," + col_mei + " 色 from HC$master_shohin_jan where 商品CD=:1 order by 色CD";

            /* 2012.05.28 USER87は海外名で */
            if (AppData.ClassCvnet.config.UserFlg == 87) {
                if (ColSizFlg == 1)
                    sql_str = "select distinct j.サイズCD,GET_SIZENAME2(s.予備05,j.サイズCD) サイズ from HC$master_shohin_jan j,HC$master_shohin s where j.商品CD=s.商品CD and j.商品CD=:1 order by j.サイズCD";
                else
                    sql_str = "select distinct j.色CD,GET_COLORNAME2(s.展示会CD,s.予備04,j.色CD) 色 from HC$master_shohin_jan j,HC$master_shohin s where j.商品CD=s.商品CD and  j.商品CD=:1 order by j.色CD";
            }

            var col_csv = AppData.Http?.AspxSqlQuery(sql_str, v_para.ToArray());
            csv_col = col_csv;

            /* 画面リサイズ処理（幅） */
            int cnt = col_csv.Rows.Count;
            if (col_csv.Rows.Count > 30) cnt = 30;

            for (int i = 0; i < col_csv.Rows.Count; i++)
            { 
                string colName = col_csv.Rows[i][0].ToString(); 
                inscsv.Columns.Add(new DataColumn(colName, typeof(string)));
            }

            if (ColSizFlg == 0) 
                sql_str = "select distinct サイズCD," + siz_mei + " サイズ from HC$master_shohin_jan where 商品CD=:1 order by サイズCD";
            else
                sql_str = "select distinct 色CD," + col_mei + " 色 from HC$master_shohin_jan where 商品CD=:1 order by 色CD";

            /* 2012.05.28 USER87は海外名で */
            if (AppData.ClassCvnet.config.UserFlg == 87)
            { 
                if (ColSizFlg == 0)
                    sql_str = "select distinct j.サイズCD,GET_SIZENAME2(s.予備05,j.サイズCD) サイズ from HC$master_shohin_jan j,HC$master_shohin s where j.商品CD=s.商品CD and j.商品CD=:1 order by j.サイズCD";
                else
                    sql_str = "select distinct j.色CD,GET_COLORNAME2(s.展示会CD,s.予備04,j.色CD) 色 from HC$master_shohin_jan j,HC$master_shohin s where j.商品CD=s.商品CD and  j.商品CD=:1 order by j.色CD";
            }

            var siz_csv = AppData.Http?.AspxSqlQuery(sql_str, v_para.ToArray());
            csv_siz = siz_csv;

            for (int i =0; i < siz_csv.Rows.Count; i++)
            {
                DataRow row = inscsv.NewRow(); 
                row[0] = siz_csv.Rows[i][0].ToString() + " " + siz_csv.Rows[i][1].ToString(); 
                inscsv.Rows.Add(row);
            }

            var merge_csv = new DataTable();
            merge_csv.Columns.Add("色 ｻｲｽﾞ", typeof(string));
            for (int i = 0; i < col_csv.Rows.Count; i++)
            {
                merge_csv.Columns.Add(col_csv.Rows[i][0].ToString(), typeof(string));
            }
            for (int i = 0; i < siz_csv.Rows.Count; i++)
            {
                DataRow row = merge_csv.NewRow();
                row[0] = siz_csv.Rows[i][0].ToString() + " " + siz_csv.Rows[i][1].ToString();
                merge_csv.Rows.Add(row);
            } 
            //int max_row = ret_csv.Rows.Count;
            //for (int i = 0; i < max_row; i++)
            //{
            //    string test = ret_csv.Rows[i][ColSizFlg].ToString();
            //    var colnum = col_csv.Columns[test].Ordinal;
            //    if (colnum < 0 || colnum >= 30) continue; /* 横軸絞り */
            //    var siznum = siz_csv.Columns[ret_csv.Rows[i][Math.Abs(ColSizFlg - 1)].ToString()].Ordinal;
            //    if (siznum < 0) continue; 
            //    inscsv.Rows[siznum][colnum + 1] = ret_csv.Rows[i][2];
            //} 
            Spread1 = merge_csv;

            /* 色サイズのチェックを追加 */
            int col_num = csv_col.Rows.Count;
            if (col_num > 30) col_num = 30;

            /* 色サイズのチェックを追加 */
            // Check & verify current cell is that editable
            //int colnum = csv_col.Rows.Count;
            //if (colnum > 30) colnum = 30;
            //ColSizList = new List<dynamic>();
            //for (int i = 0; i < Spread1.Rows.Count ; i++)
            //{
            //    for (int j = 0; j < colnum; j++)
            //    {
            //        dynamic col_siz = new ExpandoObject();
            //        if (ColSizFlg == 1)
            //        {
            //            col_siz.SizCode = csv_col.Rows[j][0].ToString(); /* 色CD */
            //            col_siz.ColorCode = csv_siz.Rows[i][0].ToString(); /* サイズCD */
            //        }
            //        else
            //        {
            //            col_siz.ColorCode = csv_col.Rows[j][0].ToString(); /* 色CD */
            //            col_siz.SizCode = csv_siz.Rows[i][0].ToString(); /* サイズCD */
            //        }
            //        ColSizList.Add(col_siz);
            //    }
            //} 
            ColSizList = new List<dynamic>();
            for (int i = 0; i < ret_csv.Rows.Count; i++)
            {
                dynamic col_siz = new ExpandoObject();
                if (ColSizFlg == 1)
                {
                    col_siz.SizCode = ret_csv.Rows[i][0].ToString();
                    col_siz.ColorCode = ret_csv.Rows[i][1].ToString();
                }
                else
                {
                    col_siz.SizCode = ret_csv.Rows[i][1].ToString();
                    col_siz.ColorCode = ret_csv.Rows[i][0].ToString();
                }
                ColSizList.Add(col_siz);
            } 
            RetailPercentChanged();

            if (!string.IsNullOrEmpty(egata)) 
                Sku01Dsp.ProdImg = AppData.Url + "Data/" + AppData.DataAddPath + egata; 
        }

        private void SetData(DataTable v_data)
        { 
            if (v_data == null) return;

            int idxcol;
            int idxsiz;

            for (int i = 0; i < v_data.Rows.Count; i++)
            {
                if (ColSizFlg == 0)
                {
                    idxcol = csv_col.Columns[v_data.Rows[i][0].ToString()].Ordinal;
                    idxsiz = csv_siz.Columns[v_data.Rows[i][1].ToString()].Ordinal;
                }
                else
                {
                    idxcol = csv_col.Columns[v_data.Rows[i][1].ToString()].Ordinal;
                    idxsiz = csv_siz.Columns[v_data.Rows[i][0].ToString()].Ordinal;
                }

                if (idxcol == -1 || idxsiz == -1) 
                    return;

                /* 数量セット */
                Spread1.Rows[idxsiz][idxcol] = v_data.Rows[i][2];
            }
        }

        private int OnGetGedai(int? jod, int? kake)
        {
            int get_gedai = 0;
            int hasu = gedaihasu * -1;
            int? sritu = kake;
            if (gedaihasu == 1)
            {
                if (gedaihasu == 0)
                    get_gedai = int.TryParse(GlobalFunc.RoundUp( (double)(jod * sritu) / 100).ToString(), out int _gedai) ? _gedai : 0;
                else
                    get_gedai = int.TryParse(GlobalFunc.RoundUp((double)(jod * sritu) / 100, hasu).ToString(), out int _gedai) ? _gedai : 0;
            }
            else if (gedaihasu == 2)
            {
                if (gedaihasu == 0)
                    get_gedai = int.TryParse(GlobalFunc.RoundDown((double)(jod * sritu) / 100).ToString(), out int _gedai) ? _gedai : 0;
                else
                    get_gedai = int.TryParse(GlobalFunc.RoundDown((double)(jod * sritu) / 100, hasu).ToString(), out int _gedai) ? _gedai : 0;
            }
            else
            {
                if (gedaihasu == 0)
                    get_gedai = int.TryParse(Math.Round((decimal)(jod * sritu) / 100).ToString(), out int _gedai) ? _gedai : 0;
                else
                    get_gedai = int.TryParse(Math.Round((decimal)(jod * sritu) / 100, hasu).ToString(), out int _gedai) ? _gedai: 0;
            }
            return get_gedai;
        }
        #endregion
    }

    public partial class Sku01Dsp : ObservableObject
    {
        [ObservableProperty]
        string? m_ProdImg;

        /// <summary>
        /// 商品CD (TextLabel1)
        /// </summary>
        [ObservableProperty]
        string? m_ProdCD01;

        /// <summary>
        /// 商品CD (TextLabel7)
        /// </summary>
        [ObservableProperty]
        string? m_ProdCD02;

        /// <summary>
        /// 専門店 Visible
        /// </summary>
        [ObservableProperty]
        Visibility m_ProdCD02Visible;

        /// <summary>
        /// 商品名
        /// </summary>
        [ObservableProperty]
        string? m_ProdName;

        [ObservableProperty]
        string? m_Lbl4Title;

        /// <summary>
        /// 色
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_ProdColor;

        [ObservableProperty]
        string? m_Lbl5Title;

        /// <summary>
        /// ｻｲｽﾞ
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_ProdSize;

        /// <summary>
        /// 上代単価
        /// </summary>
        [ObservableProperty]
        int? m_RetailPrice;

        /// <summary>
        /// 上代 (%)
        /// </summary>
        [ObservableProperty]
        int? m_RetailPercent;

        /// <summary>
        /// 上代 % (kakeritu) Active
        /// </summary>
        [ObservableProperty]
        bool? m_RetailPercentActive;

        /// <summary>
        /// 上代合計
        /// </summary>
        [ObservableProperty]
        int? m_RetailTotal;

        /// <summary>
        /// 上代合計 (TextLabel6) Active
        /// </summary>
        [ObservableProperty]
        bool? m_RetailTotalActive;

        /// <summary>
        /// 原価枝番
        /// </summary>
        [ObservableProperty]
        int? m_CostBranch;

        /// <summary>
        /// 原価枝番 (CvnetLabel6) Visible
        /// </summary>
        [ObservableProperty]
        Visibility m_LblCostBranchVisible;

        /// <summary>
        /// 原価枝番 (TextLabel9) Visible
        /// </summary>
        [ObservableProperty]
        Visibility m_TxtCostBranchVisible;

        public Sku01Dsp()
        { 
            Lbl4Title = "色";
            Lbl5Title = "ｻｲｽﾞ";
            RetailPercentActive = false;
            RetailTotalActive = false;
            ProdCD02Visible = Visibility.Collapsed;
            LblCostBranchVisible = Visibility.Visible;
            TxtCostBranchVisible = Visibility.Visible;
        }
    }
}
