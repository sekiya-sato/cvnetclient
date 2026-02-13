using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Data;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgBcd01ViewModel : BaseViewModel
    {
        #region Variables
        /* バーコード桁数 */
        [ObservableProperty]
        private KetaClass keta = new KetaClass();
        [ObservableProperty]
        private BrdConfigClass brdConfig = new BrdConfigClass();
        private int JodaiBrdFlg = AppData.ClassCvnet.config.JodaiBrdFlg;
        private int JodaiConfig = AppData.ClassCvnet.config.JodaiConfig;
          
        private int denp = 0; /* 伝票処理区分 */
        private string souko; /* 上からの店舗コード */
        private string hiduke; /* 売上日 */
        private int kubun;  /* 取引区分 */

        [ObservableProperty]
        public string? m_Title = "ﾊﾞｰｺｰﾄﾞ入力";

        /// <summary>
        /// ﾊﾞｰｺｰﾄﾞ
        /// </summary>
        [ObservableProperty]
        string? m_Barcode;

        [ObservableProperty]
        public Bcd01ItemDsp m_Bcd01ItemDsp = new Bcd01ItemDsp();
         
        [ObservableProperty]
        public ObservableCollection<Bcd01Item> m_ListBrd01 = new ObservableCollection<Bcd01Item>();

        /// <summary>
        /// Current Dialog return value
        /// </summary>
        public List<BizArray> ret_para;
        #endregion
         
        public void OnInit(string[] para)
        {
            denp = int.Parse(para[0]);
            souko = para[1];
            hiduke = para[2];
            //int X = int.Parse(para[3]) + 290;
            //int Y = int.Parse(para[4]) + 30;
            kubun = int.Parse(para[5]);

            if (denp == 5 || denp == 10 || denp == 11 || denp == 61 || denp == 17 || denp == 18)
            {
                // Disable 上代 & 上代金額 Columns
                Bcd01ItemDsp.RetailVisible = Visibility.Collapsed;
                Bcd01ItemDsp.Dsp4Visible = Visibility.Collapsed;
                kubun = 10;
            }

            /* 仕入 */
            if (denp == 3 || denp == 13)
            {
                Bcd01ItemDsp.Amount1Title = "下代";
                Bcd01ItemDsp.Amount2Title = "下代金額";
                Bcd01ItemDsp.LblPriceVisible = Visibility.Collapsed;
                Bcd01ItemDsp.TxtPriceVisible = Visibility.Collapsed;
                if (AppData.ClassCvnet.config.UserFlg == 50)
                {
                    Bcd01ItemDsp.Amount1Title = "金額";
                    Bcd01ItemDsp.Amount2Title = "金額";
                }
            }

            for (var i = 0; i < AppData.ClassCvnet.SysHhtMst.Columns.Count; i++)
            {
                if (AppData.ClassCvnet.SysHhtMst.Columns[i].ColumnName == "B1段目桁")
                {
                    Keta.Dan1_keta = int.Parse(AppData.ClassCvnet.SysHhtMst.Rows[0][i].ToString());
                }
                else if (AppData.ClassCvnet.SysHhtMst.Columns[i].ColumnName == "B2段目桁")
                {
                    Keta.Dan2_keta = int.Parse(AppData.ClassCvnet.SysHhtMst.Rows[0][i].ToString());
                }
                else if (AppData.ClassCvnet.SysHhtMst.Columns[i].ColumnName == "B3段目桁")
                {
                    Keta.Dan3_keta = int.Parse(AppData.ClassCvnet.SysHhtMst.Rows[0][i].ToString());
                }
                else if (AppData.ClassCvnet.SysHhtMst.Columns[i].ColumnName == "B段数")
                {
                    BrdConfig.Dan = int.Parse(AppData.ClassCvnet.SysHhtMst.Rows[0][i].ToString());
                    switch (int.Parse(AppData.ClassCvnet.SysHhtMst.Rows[0][i].ToString()))
                    {
                        case 0:
                            Bcd01ItemDsp.IsRead2ndRow = false;
                            break;
                        case 1:
                            Bcd01ItemDsp.IsRead2ndRow = true;
                            break;
                        default:
                            break;
                    }
                }
                else if (AppData.ClassCvnet.SysHhtMst.Columns[i].ColumnName == "JAN先頭")
                {
                    Keta.Jan_sento = AppData.ClassCvnet.SysHhtMst.Rows[0][i].ToString();
                    Keta.Jan_check = AppData.ClassCvnet.SysHhtMst.Rows[0][i].ToString().Length;
                }
                else if (AppData.ClassCvnet.SysHhtMst.Columns[i].ColumnName == "B上代段数")
                {
                    BrdConfig.Jodai_dan = int.Parse(AppData.ClassCvnet.SysHhtMst.Rows[0][i].ToString());
                }
                else if (AppData.ClassCvnet.SysHhtMst.Columns[i].ColumnName == "B上代位置")
                {
                    BrdConfig.Jodai_pos = int.Parse(AppData.ClassCvnet.SysHhtMst.Rows[0][i].ToString());
                }
                else if (AppData.ClassCvnet.SysHhtMst.Columns[i].ColumnName == "B上代桁")
                {
                    BrdConfig.Jodai_keta = int.Parse(AppData.ClassCvnet.SysHhtMst.Rows[0][i].ToString());
                }
                else if (AppData.ClassCvnet.SysHhtMst.Columns[i].ColumnName == "B品番段数")
                {
                    BrdConfig.Sho_dan = int.Parse(AppData.ClassCvnet.SysHhtMst.Rows[0][i].ToString());
                }
                else if (AppData.ClassCvnet.SysHhtMst.Columns[i].ColumnName == "B品番位置")
                {
                    BrdConfig.Sho_pos = int.Parse(AppData.ClassCvnet.SysHhtMst.Rows[0][i].ToString());
                }
                else if (AppData.ClassCvnet.SysHhtMst.Columns[i].ColumnName == "B品番桁")
                {
                    BrdConfig.Sho_keta = int.Parse(AppData.ClassCvnet.SysHhtMst.Rows[0][i].ToString());
                }
                else if (AppData.ClassCvnet.SysHhtMst.Columns[i].ColumnName == "B色段数")
                {
                    BrdConfig.Iro_dan = int.Parse(AppData.ClassCvnet.SysHhtMst.Rows[0][i].ToString());
                }
                else if (AppData.ClassCvnet.SysHhtMst.Columns[i].ColumnName == "B色位置")
                {
                    BrdConfig.Iro_pos = int.Parse(AppData.ClassCvnet.SysHhtMst.Rows[0][i].ToString());
                }
                else if (AppData.ClassCvnet.SysHhtMst.Columns[i].ColumnName == "B色桁")
                {
                    BrdConfig.Iro_keta = int.Parse(AppData.ClassCvnet.SysHhtMst.Rows[0][i].ToString());
                }
                else if (AppData.ClassCvnet.SysHhtMst.Columns[i].ColumnName == "Bサイズ段数")
                {
                    BrdConfig.Siz_dan = int.Parse(AppData.ClassCvnet.SysHhtMst.Rows[0][i].ToString());
                }
                else if (AppData.ClassCvnet.SysHhtMst.Columns[i].ColumnName == "Bサイズ位置")
                {
                    BrdConfig.Siz_pos = int.Parse(AppData.ClassCvnet.SysHhtMst.Rows[0][i].ToString());
                }
                else if (AppData.ClassCvnet.SysHhtMst.Columns[i].ColumnName == "Bサイズ桁")
                {
                    BrdConfig.Siz_keta = int.Parse(AppData.ClassCvnet.SysHhtMst.Rows[0][i].ToString());
                }
                else if (AppData.ClassCvnet.SysHhtMst.Columns[i].ColumnName == "原価FLG段数")
                {
                    BrdConfig.Genka_dan = int.Parse(AppData.ClassCvnet.SysHhtMst.Rows[0][i].ToString());
                }
                else if (AppData.ClassCvnet.SysHhtMst.Columns[i].ColumnName == "原価FLG位置")
                {
                    BrdConfig.Genka_pos = int.Parse(AppData.ClassCvnet.SysHhtMst.Rows[0][i].ToString());
                }
                else if (AppData.ClassCvnet.SysHhtMst.Columns[i].ColumnName == "原価FLG桁")
                {
                    BrdConfig.Genka_keta = int.Parse(AppData.ClassCvnet.SysHhtMst.Rows[0][i].ToString());
                }
            }
            if (AppData.ClassCvnet.config.UserFlg == 13)
            {

                Keta.Jan_sento = "A" + Keta.Jan_sento;
                Keta.Jan_check = 1 + Keta.Jan_check;
                Keta.Dan1_keta = 2 + Keta.Dan1_keta;
                Keta.Dan2_keta = 2 + Keta.Dan2_keta;
                Bcd01ItemDsp.IsRead2ndRow = false;

                //Form1.B_Code.Visible =$false;
                //Form1.NumberEdit1.Visible =$true;
                //Form1.Label3.Visible =$true;
            }
        }

        #region Events 

        partial void OnBarcodeChanged(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            var lngt = value.Length;
            if (lngt != Keta.Dan1_keta)
            {
                ClientLib.MessageBoxError(this, "読込バーコードが違います " + lngt.ToString() + " " + Keta.Dan1_keta.ToString(), "注意");
                value = string.Empty;
                return;
            }
            if (value.Substring(0, Keta.Jan_check).ToUpper() != Keta.Jan_sento)
            {
                ClientLib.MessageBoxError(this, "読込バーコードが違います", "注意");
                value = string.Empty;
                return;
            }
            if (Bcd01ItemDsp.IsRead2ndRow)
            {
                OnSetFocus();
            }
            else
            {
                var v_wkpara = new BizArray();
                if (AppData.ClassCvnet.UserFlg == 13)
                {
                    v_wkpara[0] = (value.Substring(1, lngt - 2)).ToString();
                }
                else
                {
                    v_wkpara[0] = new string(value);
                }

                var ret_csv = OnQuery(v_wkpara);

                GlobalFunc.ChkDataTableColType(ret_csv);

                if (ret_csv.Rows.Count == 0)
                {
                    ClientLib.MessageBoxError(this, "商品色サイズマスタに存在しません", "注意");
                    Barcode = string.Empty;
                    return;
                }
                int v_sirne = int.TryParse(ret_csv.Rows[0][13].ToString(), out int sirne) ? sirne : 0;
                int v_gflg = int.TryParse(ret_csv.Rows[0][14].ToString(), out int gflg) ? gflg : 0;
                if (denp == 1 && AppData.ClassCvnet.config.UserFlg == 20)
                {
                    if (int.Parse(ret_csv.Rows[0][12].ToString()) < 0)
                    {
                        ClientLib.MessageBoxError(this, "取り扱いブランドではありません", "注意");
                        Barcode = string.Empty;
                        return;
                    }
                    else
                    {
                        ret_csv.Columns.RemoveAt(14);
                        ret_csv.Columns.RemoveAt(13);
                        ret_csv.Columns.RemoveAt(12);
                    }
                }
                else
                {
                    ret_csv.Columns.RemoveAt(14);
                    ret_csv.Columns.RemoveAt(13);
                    ret_csv.Columns.RemoveAt(12);
                }

                var brdjodai = 0;
                if (JodaiBrdFlg == 1)
                {
                    if (BrdConfig.Jodai_dan == 1)
                    {
                        brdjodai = int.Parse(value.Substring(BrdConfig.Jodai_pos - 1, BrdConfig.Jodai_keta));
                        OnSetSpread(ret_csv, brdjodai, v_sirne, v_gflg);
                    }
                    else
                    {
                        brdjodai = int.Parse(Bcd01ItemDsp.SellPrice.Substring(BrdConfig.Jodai_pos - 1, BrdConfig.Jodai_keta));
                        OnSetSpread(ret_csv, brdjodai, v_sirne, v_gflg);
                    }
                }
                else
                {
                    OnSetSpread(ret_csv, null, v_sirne, v_gflg);
                }
                Barcode = string.Empty;
            }
            Barcode = string.Empty;
        }

        [RelayCommand]
        private void Cost1Changed(Bcd01Item item)
        { 
            item.WholesalesAmt = item.Quantity * item.Cost1;
        }

        [RelayCommand]
        public void DoConfirmJAN()
        {
            var vm_result = AppData.DlgService.GetJan00New();
            if (vm_result != null)
            {
                Barcode = vm_result.ret_para[0].ToString();
            }
        }

        [RelayCommand]
        public void DoExecute()
        {
            if (ListBrd01.Count == 0) return;
            ret_para = new List<BizArray>();
            foreach (var item in ListBrd01)
            {
                var para = new BizArray();
                para[0] = item.ProdCD; /* 品番 */
                para[1] = item.ProdName; /* 商品名 */
                para[2] = (item.Cost1 * item.Quantity).ToString(); /* 実売価*数量 */
                para[3] = string.Empty; /* 絵型 */
                para[4] = string.Empty; /* 大分類 */
                para[5] = string.Empty; /* 中分類 */
                para[6] = string.Empty; /* 小分類 */
                para[7] = string.Empty; /* 発売日 */
                para[8] = (item.Cost * item.Quantity).ToString(); /* 仕入単価*数量 */  /* ここでは原価*数量 */
                para[9] = item.TaxCalcMethod.ToString(); /* 消費税計算方法 */
                para[10] = string.Empty; /* カテゴリー */
                para[11] = string.Empty; /* セットFLG */
                para[12] = string.Empty; /* 延長保証FLG 05.08.26*/
                para[13] = item.ColorCD; /* 状態CD */ /* 色CD */
                para[14] = item.SizeCD; /* 保証書CD */ /* サイズCD */
                para[15] = item.ColorName; /* 色名 */
                para[16] = item.SizeName; /* サイズ名 */
                para[17] = string.Empty; /* 単品管理FLG */
                para[18] = item.Quantity.ToString(); /* 数量 */

                para[19] = (item.Cost1 * item.Quantity).ToString(); /* 実売価*数量 */
                para[20] = "0";
                para[21] = (item.Cost * item.Quantity).ToString(); /* 仕入単価*数量 */ /* ここでは原価*数量 */
                para[22] = "0";
                /* 仕入対応 */
                if (denp == 3 || denp == 13)
                    para[23] = item.MasterRetail.ToString(); /* 上代 */
                else
                    para[23] = item.Cost1.ToString(); /* 販売単価 */ /* 売上では実売価 */
                para[24] = item.Cost.ToString();  /* 仕入単価 */ /* ここでは原価 */
                para[25] = item.MasterRetail.ToString(); /* マスタ上代 */ /* ここではマスタ上代 */
                /* 在庫数、引当数追加 */
                para[26] = string.Empty;
                para[27] = string.Empty;
                /* 掛率追加、下代掛率 */
                para[28] = item.Rate.ToString();
                /* 仕入対応 */
                if (denp == 3 || denp == 13)
                    para[29] = item.Cost1.ToString();
                else
                    para[29] = item.StaffRate;
                para[30] = string.Empty;
                para[31] = string.Empty;
                para[32] = item.DeliverDate; /* 納品日 */
                para[33] = string.Empty;
                /* 原価FLG */
                para[34] = item.CostFlg.ToString();

                ret_para.Add(para);
            }
            ClientLib.ExitDialogResult(this, true);
        }

        [RelayCommand]
        public void DoExit()
        {
            ClientLib.ExitDialogResult(this, true);
        }
        #endregion

        #region Functions
        /* 商品ＭＳ検索 */
        private DataTable OnQuery(BizArray v_para) 
        {
            var sql_query = "";
            /* ＪＡＮ読込 */
            sql_query = "select j.商品CD, NVL(s.商品名,'') 商品名, j.色CD, j.サイズCD, get_colorname(j.色CD) 色名, get_sizename(j.商品CD,j.サイズCD) サイズ名";
            if (denp == 1)
            {
                /* さらにジャコモ専用処理　社員販売は掛率を掛ける */
                if (kubun == 14 || kubun == 24 || kubun == 17 || kubun == 27 || kubun == 18 || kubun == 28 || kubun == 19 || kubun == 29)
                {
                    sql_query += ",trunc(get_jodai(j.商品CD, j.色CD, j.サイズCD, '" + hiduke + "', '" + souko + "')*get_kakeritu('" + souko + "','" + kubun + "',s.商品CD,1)/100,0) 上代";
                }
                else
                {
                    sql_query += ",get_jodai(j.商品CD, j.色CD, j.サイズCD, '" + hiduke + "', '" + souko + "') 上代";
                }
            }
            else
            {
                if (AppData.ClassCvnet.config.jodaihyjflg == 0) hiduke = new string("19000101");
                if (denp == 0 || denp == 12)
                {
                    /* 出荷売上・受注時は、標準倉庫CDをセット */
                    sql_query += ",get_jodai(j.商品CD, j.色CD, j.サイズCD, '" + hiduke + "', nvl((select 標準倉庫CD from HC$MASTER_SYSKANRI),'')) 上代";
                }
                else
                {
                    sql_query += ",get_jodai(j.商品CD, j.色CD, j.サイズCD, '" + hiduke + "', '" + souko + "') 上代";
                }
            }
            /* 原価FLG */
            if (AppData.ClassCvnet.config.usegenka == 1)
            {
                sql_query += ", get_genka(j.商品CD,substr(:1" + "," + BrdConfig.Genka_pos.ToString() + "," + BrdConfig.Genka_keta.ToString() + "),'" + hiduke + "',j.色CD,j.サイズCD) 原価";
            }
            else
            {
                sql_query += ", get_genka(j.商品CD,0,'" + hiduke + "',j.色CD,j.サイズCD) 原価";
            }
            sql_query += ", NVL(s.消費税計算方法,0) 消費税計算方法";
            if (JodaiConfig == 1)
            {
                sql_query += ",get_jodai(j.商品CD, j.色CD, j.サイズCD) マスタ上代 ";
            }
            else
            {
                sql_query += ",get_jodai(j.商品CD, j.色CD, j.サイズCD, '" + hiduke + "', '" + souko + "') マスタ上代";
            }
            /* ジャコモ専用掛率 */
            if (AppData.ClassCvnet.config.UserFlg == 13)
            {
                sql_query += " ,0 掛率,0 社員掛率";
            }
            else sql_query += " ,get_kakeritu('" + souko + "','" + kubun + "',s.商品CD) 掛率,get_kakeritu('" + souko + "','" + kubun + "',s.商品CD,1) 社員掛率";
            sql_query += ",nvl((select 0 from hc$master_convert where 区分='HBN' and 一意CD01='" + souko + "' and 一意CD02=s.ブランドCD),-1) ブランド判定";

            /* 原価FLG */
            sql_query += " , CASE WHEN (SELECT 仕入価格 FROM hc$master_shohin_jan WHERE 商品cd = j.商品cd AND 色cd = j.色cd AND サイズcd = j.サイズcd ) IS NULL OR ( (SELECT 仕入価格 FROM hc$master_shohin_jan WHERE 商品cd = j.商品cd AND 色cd = j.色cd AND サイズcd = j.サイズcd ) = 0 ) THEN (SELECT 仕入価格 FROM HC$MASTER_SHOHIN WHERE 商品CD=j.商品CD) ELSE (SELECT 仕入価格 FROM hc$master_shohin_jan WHERE 商品cd = j.商品cd AND 色cd = j.色cd AND サイズcd = j.サイズcd) END AS 原価";
            if (AppData.ClassCvnet.config.usegenka == 1)
            {
                /* 直参照変更 09.11.20 */
                if (AppData.ClassCvnet.config.janConvertFlg1 != "")
                {
                    sql_query += ",decode(j.janコード3,'" + v_para[BrdConfig.Sho_dan - 1].ToString() + "',0,substr(:1" + "," + BrdConfig.Genka_pos.ToString() + "," + BrdConfig.Genka_keta.ToString() + ")) 原価flg";
                }
                else
                {
                    sql_query += ",substr(:1" + "," + BrdConfig.Genka_pos.ToString() + "," + BrdConfig.Genka_keta.ToString() + ") 原価flg";
                }
            }
            else
            {
                sql_query += " ,0 原価flg";
            }
            /* 2009.06.18 納品日追加 */
            sql_query += " ,s.納品日";

            sql_query += " from HC$master_shohin_jan j,HC$master_shohin s,HC$master_shohin_GENKA G"
                + " where j.商品CD=s.商品CD(+) AND J.商品CD=G.商品CD(+)";

            /* ココからJANコードの取得についての設定 */
            /* 商品固定コードの紐付け */
            var wrk_para = new BizArray();

            if (AppData.ClassCvnet.config.UserFlg == 41)
            {
                /* 商品 */
                sql_query += " and (j.JANコード1=:1";
                wrk_para[0] = v_para[BrdConfig.Sho_dan - 1].ToString();

                /* 2009.07.07 JANコード3対応 */
                sql_query += " or j.JANコード3=:1)";
            }
            /* 原価有で直参照ルート 09.11.19		 */
            else if (AppData.ClassCvnet.config.usegenka == 1 && AppData.ClassCvnet.config.janConvertFlg1 != "")
            {
                var v_genka = "0";
                if (BrdConfig.Genka_keta == 2) v_genka = "00";
                /* 商品 */
                /* 2012.08.31 okamoto #696 取置メニューにてJAN検索からの入力ができない  MOD Start  */
                sql_query += " and ( CDJAN(substr( j.JANコード1,1," + (BrdConfig.Genka_pos - 1).ToString() + ")||NVL(to_char(G.行NO,'FM" + v_genka.ToString() + "'),'00')||substr( j.JANコード1," + (BrdConfig.Genka_pos + BrdConfig.Genka_keta).ToString() + ",(12-" + (BrdConfig.Genka_pos + BrdConfig.Genka_keta - 1).ToString() + ")))=:1";
                /* 2012.08.31 okamoto #696 取置メニューにてJAN検索からの入力ができない  MOD End */
                wrk_para[0] = v_para[BrdConfig.Sho_dan - 1].ToString();

                /* 2009.07.07 JANコード3対応 */
                sql_query += " or j.JANコード3=:1)";
            }
            /* 原価無で直参照ルート 09.12.15 */
            else if (AppData.ClassCvnet.config.usegenka == 0 && AppData.ClassCvnet.config.janConvertFlg1 != "")
            {
                /* 商品 */
                sql_query += " and (j.JANコード1=:1";
                wrk_para[0] = v_para[BrdConfig.Sho_dan - 1].ToString();
                sql_query += " or j.JANコード3=:1)";

            }
            else
            {
                /* 商品 */
                sql_query += " and ((substr(j.JANコード" + BrdConfig.Sho_dan.ToString() + "," + (BrdConfig.Sho_pos).ToString() + "," + (BrdConfig.Sho_keta).ToString() + ")"
                    + " = " + "substr(:1" + "," + (BrdConfig.Sho_pos).ToString() + "," + (BrdConfig.Sho_keta).ToString() + ")";
                wrk_para[0] = v_para[BrdConfig.Sho_dan - 1].ToString();

                /* 色 */
                sql_query += " and substr(j.JANコード" + (BrdConfig.Iro_dan).ToString() + "," + (BrdConfig.Iro_pos).ToString() + "," + (BrdConfig.Iro_keta).ToString() + ")"
                    + " = " + "substr(:1" + "," + (BrdConfig.Iro_pos).ToString() + "," + (BrdConfig.Iro_keta).ToString() + ")";
                wrk_para[1] = v_para[BrdConfig.Iro_dan - 1].ToString();

                /* サイズ */
                sql_query += " and substr(j.JANコード" + (BrdConfig.Siz_dan).ToString() + "," + (BrdConfig.Siz_pos).ToString() + "," + (BrdConfig.Siz_keta).ToString() + ")"
                    + " = " + "substr(:1" + "," + (BrdConfig.Siz_pos).ToString() + "," + (BrdConfig.Siz_keta).ToString() + ")";
                wrk_para[2] = v_para[BrdConfig.Siz_dan - 1].ToString();

                /* 2009.07.07 JANコード3対応 */
                sql_query += ") or j.JANコード3=:1)";
            }
            return AppData.Http!.AspxSqlQuery(sql_query, wrk_para.ToArray());
        }
        void OnSetSpread(DataTable wrk_csv, int? price = 0, int? v_sirne = null, int? v_gflg = null)
        {
            wrk_csv.Columns.Add(string.Format("Col{0}", wrk_csv.Columns.Count.ToString("00")), typeof(string));
            if (price != null)
            {
                /* 2009.06.18 納品日追加の為、ズレ調整 */
                wrk_csv.Rows[0][wrk_csv.Columns.Count - 5] = price;
                wrk_csv.Rows[0][wrk_csv.Columns.Count - 1] = price;
            }
            else wrk_csv.Rows[0][wrk_csv.Columns.Count - 1] = wrk_csv.Rows[0][6];

            int? amount = 0;
            int row_num = 999999;

            /*if (ListBrd01.Count != 0)
            {
                foreach (var item in ListBrd01)
                {
                    if (item.ProdCD == wrk_csv.Rows[0][0].ToString() &&
                        item.ColorCD == wrk_csv.Rows[0][2].ToString() &&
                        item.SizeCD == wrk_csv.Rows[0][3].ToString() &&
                        item.Cost1.ToString() == wrk_csv.Rows[0][13].ToString() &&
                        item.CostFlg == v_gflg)
                    {
                        amount = item.Quantity;
                        break;
                    }
                }
            }*/
              
            Bcd01Item brd_item = new Bcd01Item();
            brd_item.ProdCD = wrk_csv.Rows[0][0].ToString();
            brd_item.ProdName = wrk_csv.Rows[0][1].ToString();
            brd_item.ColorName = wrk_csv.Rows[0][4].ToString();
            brd_item.SizeName = wrk_csv.Rows[0][5].ToString();
            brd_item.Quantity = amount + 1;
            brd_item.TaxCalcMethod = int.TryParse(wrk_csv.Rows[0][8].ToString(), out int _tax_cal) ? _tax_cal : 0; /* 消費税計算方法 */
            brd_item.ColorCD = wrk_csv.Rows[0][2].ToString(); /* 色CD */
            brd_item.SizeCD = wrk_csv.Rows[0][3].ToString(); /* サイズCD */

            /* 単価追加 */
            /* 2009.06.18 納品日追加の為、ズレ調整 */
            /* ↓下1行修正 09.10.01 取得位置修正 */
            brd_item.Cost1 = int.TryParse(wrk_csv.Rows[0][13].ToString(), out int _cost1) ? _cost1 : 0; /* 販売単価 */
            brd_item.Cost = int.TryParse(wrk_csv.Rows[0][7].ToString(), out int _cost) ? _cost : 0; /* 原価 */
            brd_item.MasterRetail = int.TryParse(wrk_csv.Rows[0][9].ToString(), out int _master_retail) ? _master_retail : 0; /* マスタ上代単価 */

            /* 掛率の設定 */
            brd_item.Rate = wrk_csv.Rows[0][10].ToString(); /* 掛率 */
            brd_item.StaffRate = wrk_csv.Rows[0][11].ToString(); /* 掛率 */

            /* 仕入対応 */
            if (denp == 3 || denp == 13)
            {
                if (kubun != 30)
                {
                    brd_item.Cost1 = v_sirne;
                    if (AppData.ClassCvnet.config.usegenka == 1) {
                        brd_item.Cost1 = int.TryParse(wrk_csv.Rows[0][7].ToString(), out _cost1) ? _cost1 : 0;
                    }
                }
                else
                {
                    brd_item.Cost1 = 0;
                }
            }

            /* 原価FLG */
            brd_item.CostFlg = v_gflg;

            /* 金額・上代金額 */
            brd_item.Amount = (amount + 1) * brd_item.MasterRetail;
            brd_item.WholesalesAmt = (amount + 1) * brd_item.Cost1;

            /* 2009.06.18 納品日追加 */
            brd_item.DeliverDate = wrk_csv.Rows[0][12].ToString();
             
            var selected_row = ListBrd01.FirstOrDefault(x => x.ProdCD == wrk_csv.Rows[0][0].ToString() && 
                                                                x.ColorCD == wrk_csv.Rows[0][2].ToString() && 
                                                                x.SizeCD == wrk_csv.Rows[0][3].ToString() &&
                                                                x.Cost1.ToString() == wrk_csv.Rows[0][13].ToString());
            if (selected_row != null)
            {
                selected_row.Quantity = selected_row.Quantity + 1;
                /* 金額・上代金額 */
                selected_row.Amount = selected_row.Quantity * brd_item.MasterRetail;
                selected_row.WholesalesAmt = selected_row.Quantity * brd_item.Cost1; 

                var _temp = new ObservableCollection<Bcd01Item>(ListBrd01);
                ListBrd01 = null;
                ListBrd01 = new ObservableCollection<Bcd01Item>(_temp);
            }
            else ListBrd01.Add(brd_item); 

            OnChangeTotal();
        }
        void OnSetFocus() 
        {
            if (Bcd01ItemDsp.SellPrice == "")
            {
                //ProcessBarcode();
                Bcd01ItemDsp.SellPrice = string.Empty;
                return;
            }

            var lngt = Bcd01ItemDsp.SellPrice.Length;
            if (lngt != Keta.Dan2_keta)
            {
                ClientLib.MessageBoxError(this,"読込バーコードが違います", "注意");
                Bcd01ItemDsp.SellPrice = string.Empty;
                return;
            }

            var v_wkpara = new BizArray();
            if (AppData.ClassCvnet.UserFlg == 13)
            {
                v_wkpara[0] = Barcode.Substring( 1, Bcd01ItemDsp.SellPrice.Length - 2).ToString();
            }
            else
            {
                v_wkpara[0] = Barcode;
                v_wkpara[1] = Bcd01ItemDsp.SellPrice;
            }

            var ret_csv = OnQuery(v_wkpara);

            if (ret_csv.Rows.Count > 0)
            {
                if (int.Parse(ret_csv.Rows[0][12].ToString()) < 0 && AppData.ClassCvnet.UserFlg != 13)
                {
                    ClientLib.MessageBoxError(this, "取り扱いブランドではありません", "注意");
                    return;
                }
                else
                {
                    ret_csv.Columns.RemoveAt(14);
                    ret_csv.Columns.RemoveAt(13);
                    ret_csv.Columns.RemoveAt(12);
                }
                var brdjodai = 0;
                if (JodaiBrdFlg == 1)
                {
                    if (BrdConfig.Jodai_dan == 1)
                    {
                        brdjodai = int.Parse(Barcode.Substring(BrdConfig.Jodai_pos - 1, BrdConfig.Jodai_keta));
                        OnSetSpread(ret_csv, brdjodai);
                    }
                    else
                    {
                        brdjodai = int.Parse(Bcd01ItemDsp.SellPrice.Substring( BrdConfig.Jodai_pos - 1, BrdConfig.Jodai_keta));
                        OnSetSpread(ret_csv, brdjodai);
                    }
                }
                else
                {
                    if (AppData.ClassCvnet.config.UserFlg == 13)
                    {
                        OnSetSpread(ret_csv, int.Parse(ret_csv.Rows[0][ret_csv.Columns.Count - 3].ToString()));
                    }
                    else OnSetSpread(ret_csv);
                }
                /* } */
            }
            else
            {
                ClientLib.MessageBoxError(this, "商品色サイズマスタに存在しません", "注意");
            }

            Barcode = string.Empty;
            Bcd01ItemDsp.SellPrice = string.Empty;
            //ProcessBarcode();
        }

        private void OnChangeTotal()
        {
            Bcd01ItemDsp.Dsp3 = ListBrd01.Sum(x => x.Quantity);
            Bcd01ItemDsp.Dsp4 = ListBrd01.Sum(x => x.Amount);
            Bcd01ItemDsp.Dsp5 = ListBrd01.Sum(x => x.WholesalesAmt);
        }

        #endregion

        public partial class KetaClass : ObservableObject 
        {
            [ObservableProperty]
            private int dan1_keta = 0;
            [ObservableProperty]
            private int dan2_keta = 0;
            [ObservableProperty]
            private int dan3_keta = 0;
            [ObservableProperty]
            private string jan_sento = "";
            [ObservableProperty]
            private int jan_check = 0;
        }
        public partial class BrdConfigClass : ObservableObject 
        {
            [ObservableProperty]
            private int sho_dan = 0;
            [ObservableProperty]
            private int sho_pos = 0;
            [ObservableProperty]
            private int sho_keta = 0;
            [ObservableProperty]
            private int iro_dan = 0;
            [ObservableProperty]
            private int iro_pos = 0;
            [ObservableProperty]
            private int iro_keta = 0;
            [ObservableProperty]
            private int siz_dan = 0;
            [ObservableProperty]
            private int siz_pos = 0;
            [ObservableProperty]
            private int siz_keta = 0;
            [ObservableProperty]
            private int jodai_dan = 0;
            [ObservableProperty]
            private int jodai_pos = 0;
            [ObservableProperty]
            private int jodai_keta = 0;
            [ObservableProperty]
            private int genka_dan = 0;
            [ObservableProperty]
            private int genka_pos = 0;
            [ObservableProperty]
            private int genka_keta = 0;
            [ObservableProperty]
            private int dan = 0;

        }
    }

    public partial class Bcd01ItemDsp : ObservableObject
    {
        /// <summary>
        /// 実売価 (Line23) & 金額 (Line5) Visibility
        /// </summary>
        [ObservableProperty]
        Visibility m_RetailVisible;
         
        /// <summary>
        /// 売価
        /// </summary>
        [ObservableProperty]
        string? m_SellPrice;

        /// <summary>
        /// 売価 Label Visible
        /// </summary>
        [ObservableProperty]
        Visibility m_LblPriceVisible;

        /// <summary>
        /// 売価 Textbox Visible
        /// </summary>
        [ObservableProperty]
        Visibility m_TxtPriceVisible;

        /// <summary>
        /// 2段目まで読む
        /// </summary>
        [ObservableProperty]
        bool m_IsRead2ndRow;

        /// <summary>
        /// 上代 / 下代 (Line21 / Line6) Title
        /// </summary>
        [ObservableProperty]
        string m_Amount1Title;

        /// <summary>
        /// 上代金額 / 下代金額 (Line21 / Line6) Title
        /// </summary>
        [ObservableProperty]
        string m_Amount2Title;

        /// <summary>
        /// Dsp3 - Total Num
        /// </summary>
        [ObservableProperty]
        int? m_Dsp3;

        /// <summary>
        /// Dsp4 - Total Amount1
        /// </summary>
        [ObservableProperty]
        int? m_Dsp4;

        [ObservableProperty]
        Visibility m_Dsp4Visible;
          
        /// <summary>
        /// Dsp5 - Total Amount2
        /// </summary>
        [ObservableProperty]
        int? m_Dsp5;

        public Bcd01ItemDsp()
        { 
            RetailVisible = Visibility.Visible; 
            LblPriceVisible = Visibility.Visible;
            TxtPriceVisible = Visibility.Visible;
            Dsp4Visible = Visibility.Visible;
            Amount1Title = "上代";
            Amount2Title = "上代金額";
        }
    }

    public partial class Bcd01Item : ObservableObject
    {
        /// <summary>
        /// 行
        /// </summary>
        [ObservableProperty]
        int? m_Num;

        /// <summary>
        /// 商品CD
        /// </summary>
        [ObservableProperty]
        string? m_ProdCD;

        /// <summary>
        /// 商品名
        /// </summary>
        [ObservableProperty]
        string? m_ProdName;

        /// <summary>
        /// 色CD
        /// </summary>
        [ObservableProperty]
        string? m_ColorCD;

        /// <summary>
        /// サイズCD
        /// </summary>
        [ObservableProperty]
        string? m_SizeCD;

        /// <summary>
        /// 色名
        /// </summary>
        [ObservableProperty]
        string? m_ColorName;

        /// <summary>
        /// サイズ名
        /// </summary>
        [ObservableProperty]
        string? m_SizeName;

        /// <summary>
        /// 数量
        /// </summary>
        [ObservableProperty]
        int? m_Quantity; 

        /// <summary>
        /// 上代
        /// </summary>
        [ObservableProperty]
        int? m_Retail;

        /// <summary>
        /// 原価
        /// </summary>
        [ObservableProperty]
        int? m_Cost;

        /// <summary>
        /// 消費税計算方法
        /// </summary>
        [ObservableProperty]
        int? m_TaxCalcMethod;

        /// <summary>
        /// マスタ上代
        /// </summary>
        [ObservableProperty]
        int? m_MasterRetail;

        /// <summary>
        /// 掛率
        /// </summary>
        [ObservableProperty]
        string? m_Rate;

        /// <summary>
        /// 社員掛率
        /// </summary>
        [ObservableProperty]
        string? m_StaffRate;

        /// <summary>
        /// ブランド判定
        /// </summary>
        [ObservableProperty]
        int? m_BrandDecide;

        /// <summary>
        /// 原価1
        /// </summary>
        [ObservableProperty]
        int? m_Cost1;

        /// <summary>
        /// 原価FLG
        /// </summary>
        [ObservableProperty]
        int? m_CostFlg;

        /// <summary>
        /// 納品日
        /// </summary>
        [ObservableProperty]
        string? m_DeliverDate;

        /// <summary>
        /// 金額 (Line5)
        /// </summary>
        [ObservableProperty]
        int? m_Amount;

        /// <summary>
        /// 上代金額 (Line6)
        /// </summary>
        [ObservableProperty]
        int? m_WholesalesAmt;

        public Bcd01Item() { }

        public Bcd01Item(Bcd01Item item)
        {
            this.Num = item.Num;
            this.ProdCD = item.ProdCD;
            this.ProdName = item.ProdName;
            this.ColorCD = item.ColorCD;
            this.SizeCD = item.SizeCD;
            this.ColorName = item.ColorName;
            this.SizeName = item.SizeName;
            this.Quantity = item.Quantity;
            this.Retail = item.Retail;
            this.Cost = item.Cost;
            this.TaxCalcMethod = item.TaxCalcMethod;
            this.MasterRetail = item.MasterRetail;
            this.Rate = item.Rate;
            this.StaffRate = item.StaffRate;
            this.BrandDecide = item.BrandDecide;
            this.Cost1 = item.Cost1;
            this.CostFlg = item.CostFlg;
            this.DeliverDate = item.DeliverDate;
            this.Amount = item.Amount;
            this.WholesalesAmt = item.WholesalesAmt;
        }
    }
}
