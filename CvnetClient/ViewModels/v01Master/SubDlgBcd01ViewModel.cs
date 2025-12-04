using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgBcd01ViewModel : BaseViewModel
    {
        [ObservableProperty]
        private KetaClass keta;
        [ObservableProperty]
        private BrdConfigClass brdConfig;
        [ObservableProperty]
        private string? barcode;
        [ObservableProperty]
        private string? baika;
        private int denp = 0;
        private string souko;
        private string hiduke;
        private int kubun;
        private int JodaiBrdFlg = AppData.ClassCvnet.config.JodaiBrdFlg;
        private int JodaiConfig = AppData.ClassCvnet.config.JodaiConfig;
        public bool IsFeatureEnabled { get; set; }
        public SubDlgBcd01ViewModel(string[] para) 
        {
            denp = int.Parse(para[0]);
            souko = para[1];
            hiduke = para[2];
            int X = int.Parse(para[3]) + 290;
            int Y = int.Parse(para[4]) + 30;
            kubun = int.Parse(para[5]);
            
            if (denp == 5 || denp == 10 || denp == 11 || denp == 61 || denp == 17 || denp == 18)
            {
                //Form1.Spread1.FlexRecord1.Set01.Set02.Line23.width = 0;
                //Form1.Spread1.FlexRecord1.Set01.Set03.Line5.width = 0;
                //Form1.Dsp4.Visible = $false;
                kubun = 10;
            }

            /* 仕入 */
            if (denp == 3 || denp == 13)
            {
                //Form1.Spread1.FlexRecord1.Set01.Set02.Line21.title = "下代";
                //Form1.Spread1.FlexRecord1.Set01.Set03.Line6.title = "下代金額";
                //Form1.Label2.Visible =$false;
                //Form1.Text2.Visible =$false;
                if (AppData.ClassCvnet.config.UserFlg == 50)
                {
                    //Form1.Spread1.FlexRecord1.Set01.Set02.Line21.title = "金額";
                    //Form1.Spread1.FlexRecord1.Set01.Set03.Line6.title = "金額";
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
                            //Form1.CheckBox1.CheckItem1[0].Selected =$false;
                            break;
                        case 1:
                            //Form1.CheckBox1.CheckItem1[0].Selected =$true;
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
                //Form1.CheckBox1.Active = $false;

                //Form1.B_Code.Visible =$false;
                //Form1.NumberEdit1.Visible =$true;
                //Form1.Label3.Visible =$true;

                //Form1.Spread1.FlexRecord1.Set01.Set03.dummy1.Width = 380;
                //Form1.Spread1.FlexRecord1.Set01.Set03.dsp1.Width = 40;
                //Form1.Spread1.FlexRecord1.Set01.Set03.dsp1.Title = "OFF率";

            }
        }

        [RelayCommand]
        private void BarcodeEnter(KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            ProcessBarcode();
        }

        [RelayCommand]
        private void BarcodeLostFocus()
        {
            ProcessBarcode();
        }

        private void ProcessBarcode()
        {
            if (string.IsNullOrWhiteSpace(Barcode))
                return;

            var lngt = Barcode.Length;
            if (lngt != Keta.Dan1_keta)
            {
                ClientLib.MessageBoxError(this,"読込バーコードが違います " + lngt.ToString() + " " + Keta.Dan1_keta.ToString(), "注意");
                Barcode = string.Empty;
                return;
            }
            if (Barcode.Substring(0, Keta.Jan_check).ToUpper() != Keta.Jan_sento)
            {
                ClientLib.MessageBoxError(this, "読込バーコードが違います", "注意");
                Barcode = string.Empty;
                return;
            }
            if (IsFeatureEnabled)
            {
                OnSetFocus();
            }
            else
            {
                var v_wkpara = new BizArray();
                if (AppData.ClassCvnet.UserFlg == 13)
                {
                    v_wkpara[0] = (Barcode.Substring(1,lngt - 2)).ToString();
                }
                else
                {
                    v_wkpara[0] = new string(Barcode);
                }

                var ret_csv = OnQuery(v_wkpara);

                if (ret_csv.Rows.Count == 0)
                {
                    ClientLib.MessageBoxError(this, "商品色サイズマスタに存在しません", "注意");
                    Barcode = string.Empty;
                    return;
                }
                var v_sirne = ret_csv.Rows[0][13];
                var v_gflg = ret_csv.Rows[0][14];
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
                        brdjodai = int.Parse(Barcode.Substring(BrdConfig.Jodai_pos - 1, BrdConfig.Jodai_keta));
                        OnSetSpread(ret_csv, brdjodai, v_sirne, v_gflg);
                    }
                    else
                    {
                        brdjodai = int.Parse(Baika.Substring(BrdConfig.Jodai_pos - 1, BrdConfig.Jodai_keta));
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

        private DataTable OnQuery(BizArray v_para) 
        {
            var sql_query = "";

            sql_query = "select j.商品CD, NVL(s.商品名,'') 商品名, j.色CD, j.サイズCD, get_colorname(j.色CD) 色名, get_sizename(j.商品CD,j.サイズCD) サイズ名";
            if (denp == 1)
            {
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
                    sql_query += ",get_jodai(j.商品CD, j.色CD, j.サイズCD, '" + hiduke + "', nvl((select 標準倉庫CD from HC$MASTER_SYSKANRI),'')) 上代";
                }
                else
                {
                    sql_query += ",get_jodai(j.商品CD, j.色CD, j.サイズCD, '" + hiduke + "', '" + souko + "') 上代";
                }
            }

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
            if (AppData.ClassCvnet.config.UserFlg == 13)
            {
                sql_query += " ,0 掛率,0 社員掛率";
            }
            else sql_query += " ,get_kakeritu('" + souko + "','" + kubun + "',s.商品CD) 掛率,get_kakeritu('" + souko + "','" + kubun + "',s.商品CD,1) 社員掛率";
            sql_query += ",nvl((select 0 from hc$master_convert where 区分='HBN' and 一意CD01='" + souko + "' and 一意CD02=s.ブランドCD),-1) ブランド判定";

            sql_query += " , CASE WHEN (SELECT 仕入価格 FROM hc$master_shohin_jan WHERE 商品cd = j.商品cd AND 色cd = j.色cd AND サイズcd = j.サイズcd ) IS NULL OR ( (SELECT 仕入価格 FROM hc$master_shohin_jan WHERE 商品cd = j.商品cd AND 色cd = j.色cd AND サイズcd = j.サイズcd ) = 0 ) THEN (SELECT 仕入価格 FROM HC$MASTER_SHOHIN WHERE 商品CD=j.商品CD) ELSE (SELECT 仕入価格 FROM hc$master_shohin_jan WHERE 商品cd = j.商品cd AND 色cd = j.色cd AND サイズcd = j.サイズcd) END AS 原価";
            if (AppData.ClassCvnet.config.usegenka == 1)
            {
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

            sql_query += " ,s.納品日";

            sql_query += " from HC$master_shohin_jan j,HC$master_shohin s,HC$master_shohin_GENKA G"
                + " where j.商品CD=s.商品CD(+) AND J.商品CD=G.商品CD(+)";

            var wrk_para = new BizArray();

            if (AppData.ClassCvnet.config.UserFlg == 41)
            {
                sql_query += " and (j.JANコード1=:1";
                wrk_para[0] = v_para[BrdConfig.Sho_dan - 1].ToString();
                sql_query += " or j.JANコード3=:1)";
            }
            else if (AppData.ClassCvnet.config.usegenka == 1 && AppData.ClassCvnet.config.janConvertFlg1 != "")
            {
                var v_genka = "0";
                if (BrdConfig.Genka_keta == 2) v_genka = "00";
                sql_query += " and ( CDJAN(substr( j.JANコード1,1," + (BrdConfig.Genka_pos - 1).ToString() + ")||NVL(to_char(G.行NO,'FM" + v_genka.ToString() + "'),'00')||substr( j.JANコード1," + (BrdConfig.Genka_pos + BrdConfig.Genka_keta).ToString() + ",(12-" + (BrdConfig.Genka_pos + BrdConfig.Genka_keta - 1).ToString() + ")))=:1";
                wrk_para[0] = v_para[BrdConfig.Sho_dan - 1].ToString();
                sql_query += " or j.JANコード3=:1)";
            }
            else if (AppData.ClassCvnet.config.usegenka == 0 && AppData.ClassCvnet.config.janConvertFlg1 != "")
            {
                sql_query += " and (j.JANコード1=:1";
                wrk_para[0] = v_para[BrdConfig.Sho_dan - 1].ToString();
                sql_query += " or j.JANコード3=:1)";

            }
            else
            {
                sql_query += " and ((substr(j.JANコード" + BrdConfig.Sho_dan.ToString() + "," + (BrdConfig.Sho_pos).ToString() + "," + (BrdConfig.Sho_keta).ToString() + ")"
                    + " = " + "substr(:1" + "," + (BrdConfig.Sho_pos).ToString() + "," + (BrdConfig.Sho_keta).ToString() + ")";
                wrk_para[0] = v_para[BrdConfig.Sho_dan - 1].ToString();
                sql_query += " and substr(j.JANコード" + (BrdConfig.Iro_dan).ToString() + "," + (BrdConfig.Iro_pos).ToString() + "," + (BrdConfig.Iro_keta).ToString() + ")"
                    + " = " + "substr(:1" + "," + (BrdConfig.Iro_pos).ToString() + "," + (BrdConfig.Iro_keta).ToString() + ")";
                wrk_para[1] = v_para[BrdConfig.Iro_dan - 1].ToString();
                sql_query += " and substr(j.JANコード" + (BrdConfig.Siz_dan).ToString() + "," + (BrdConfig.Siz_pos).ToString() + "," + (BrdConfig.Siz_keta).ToString() + ")"
                    + " = " + "substr(:1" + "," + (BrdConfig.Siz_pos).ToString() + "," + (BrdConfig.Siz_keta).ToString() + ")";
                wrk_para[2] = v_para[BrdConfig.Siz_dan - 1].ToString();
                sql_query += ") or j.JANコード3=:1)";

            }
            return AppData.Http!.AspxSqlQuery(sql_query, wrk_para.ToArray());
        }
        void OnSetSpread(DataTable wrk_csv, int? price = 0,object? v_sirne = null,object? v_gflg = null) 
        {
            //wrk_csv.inscol(1);
            
            //if (price != null)
            //{
            //    wrk_csv.setcell(0, wrk_csv.Columns.Count - 5, price);
            //    if (cvnet.Config.UserFlg == 13) price = rounddown(price * (100 - Form1.NumberEdit1.Value) / 100);   /* 実売のみOFF率反映 */
            //    wrk_csv.setcell(0, wrk_csv.columns - 1, price);
            //}
            //else wrk_csv.setcell(0, wrk_csv.columns - 1, wrk_csv.getcell(0, 6));

            //var amount = 0;
            //long row_num = 999999;

            //if (Form1.Spread1.RowCount != 0)
            //{
            //    var row1 = Form1.Spread1.GetRow();
            //    while (!row1.end)
            //    {
            //        if (row1.Line7.value == wrk_csv.getcell(0, 0)
            //            && row1.Line18.value == wrk_csv.getcell(0, 2)
            //            && row1.Line19.value == wrk_csv.getcell(0, 3)
            //            && row1.Line21.value == wrk_csv.getcell(0, 13)
            //            && row1.Line29.value == v_gflg)
            //        {
            //            row_num = row1.position;
            //            amount = row1.Line4.value;
            //            break;
            //        }
            //        row1.movenext();
            //    }
            //}

            //if (row_num == 999999)
            //{
            //    Form1.Spread1.InsertRow();
            //    row_num = Form1.Spread1.RowCount - 1;
            //}
            //var row1 = Form1.Spread1.GetRow(row_num);
            //row1.Line7.value = wrk_csv.getcell(0, 0);
            //row1.Line1.value = wrk_csv.getcell(0, 1);
            //row1.Line2.value = wrk_csv.getcell(0, 4);
            //row1.Line3.value = wrk_csv.getcell(0, 5);
            //row1.Line4.value = amount + 1;
            //row1.Line14.value = wrk_csv.getcell(0, 8); 
            //row1.Line18.value = wrk_csv.getcell(0, 2);  
            //row1.Line19.value = wrk_csv.getcell(0, 3);  

            //row1.Line21.value = wrk_csv.getcell(0, 13); 
            //row1.Line22.value = wrk_csv.getcell(0, 7);
            //row1.Line23.value = wrk_csv.getcell(0, 9);

            //row1.Line27.value = wrk_csv.getcell(0, 10);
            //row1.Line28.value = wrk_csv.getcell(0, 11);

            //if (denp == 3 || denp == 13)
            //{
            //    if (kubun != 30)
            //    {
            //        row1.Line21.value = v_sirne;
            //        if (AppData.ClassCvnet.config.usegenka == 1)
            //        {
            //            row1.Line21.value = wrk_csv.Rows[0][7];
            //        }
            //    }
            //    else
            //    {
            //        row1.Line21.value = 0;
            //    }
            //}

            ///* 原価FLG */
            //row1.Line29.value = v_gflg;

            ///* 金額・上代金額 */
            //row1.Line5.value = (amount + 1) * row1.Line23.value;
            //row1.Line6.value = (amount + 1) * row1.Line21.value;

            ///* 2009.06.18 納品日追加 */
            //row1.Line30.value = wrk_csv.getcell(0, 12);

            //if (AppData.ClassCvnet.config.UserFlg == 13) row1.dsp1.value = Form1.NumberEdit1.Value;


            //Form1.Spread1.ChangeTotal();
            //Form1.Spread1.RowPosition = Form1.Spread1.RowCount - 1;
        }
        void OnSetFocus() 
        {
            if (Baika == "")
            {
                ProcessBarcode();
                Baika = string.Empty;
                return;
            }

            var lngt = Baika.Length;
            if (lngt != Keta.Dan2_keta)
            {
                ClientLib.MessageBoxError(this,"読込バーコードが違います", "注意");
                Baika = string.Empty;
                return;
            }

            var v_wkpara = new BizArray();
            if (AppData.ClassCvnet.UserFlg == 13)
            {
                v_wkpara[0] = Barcode.Substring( 1, Baika.Length - 2).ToString();
            }
            else
            {
                v_wkpara[0] = Barcode;
                v_wkpara[1] = Baika;
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
                        brdjodai = int.Parse(Baika.Substring( BrdConfig.Jodai_pos - 1, BrdConfig.Jodai_keta));
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
            Baika = string.Empty;
            ProcessBarcode();
        }
        [RelayCommand]
        public void CheckJan() 
        {
            var vm = new SubDlg01Jan00NewViewModel();
            var window = new SubDlg01Jan00NewView { DataContext = vm };
            window.ShowDialog();
        }
        [RelayCommand]
        public void Do() 
        { 
        
        }

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
}
