using CommunityToolkit.Mvvm.ComponentModel;
using CvnetBaseCore;
using CvnetClient.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgSKU01ViewModel : BaseViewModel
    {
        [ObservableProperty]
        private Visibility visible;
        [ObservableProperty]
        private string label4;
        [ObservableProperty]
        private string label5;
        public SubDlgSKU01ViewModel(string[] init_para) 
        {

            Visible = (AppData.ClassCvnet.config.usegenka == 1) ? Visibility.Visible : Visibility.Collapsed;
            if (AppData.ClassCvnet.config.ColSizFlg == 1)
            {
                //Form1.Spread1.FlexRecord1.Flex0.Title = "色/ｻｲｽﾞ";
                Label4 = "ｻｲｽﾞ";
                Label5 = "色";
            }
            else
            {
                Label4 = "色";
                Label5 = "ｻｲｽﾞ";
            }

            var v_para = new string[1];
            v_para[0] = init_para[0];
            if (init_para.Length > 1)
            {
                v_para[0] = init_para[1];
                v_para[1] = init_para[2];
                v_para[2] = init_para[0];
                v_para[3] = init_para[0];
            }

            var v_jodai = "";
            //if (init_para2.length > 0)
            //{
            //    v_jodai = ",'" + init_para2[0] + "','" + init_para2[1] + "'";
            //    if (init_para2.length > 2)
            //    {
            //        if (init_para2[2] == "0" || init_para2[2] == "12")
            //        {
            //            Form1.kakeritu.Active = $TRUE;
            //            Form1.TextLabel6.Active = $TRUE;
            //        }
            //    }
            //}

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
            {
                sql_str += ",Get_Kakeritu(:1,:2,:3) 下代掛率";
            }
            else
            {
                sql_str += ",0 下代掛率";
            }
            sql_str += ", CASE WHEN (SELECT 仕入価格 FROM hc$master_shohin_jan WHERE 商品cd = j.商品cd AND 色cd = j.色cd AND サイズcd = j.サイズcd ) IS NULL OR ( (SELECT 仕入価格 FROM hc$master_shohin_jan WHERE 商品cd = j.商品cd AND 色cd = j.色cd AND サイズcd = j.サイズcd ) = 0 ) THEN (SELECT 仕入価格 FROM HC$MASTER_SHOHIN WHERE 商品CD=j.商品CD) ELSE (SELECT 仕入価格 FROM hc$master_shohin_jan WHERE 商品cd = j.商品cd AND 色cd = j.色cd AND サイズcd = j.サイズcd) END AS 原価";
            sql_str += ",NVL(s.納品日,'19010101') 納品日";
            sql_str += ",NVL(s.絵型名,'.') 絵型名";
            sql_str += ",NVL(s.メーカー品番,'.') MKR品番";
            sql_str += ",NVL(s.仕入区分||' '||decode(s.仕入区分,1,'買取',2,'委託',3,'消化',''),'') 仕入区分";

            sql_str += " from HC$master_shohin_jan j,HC$master_shohin s";
            sql_str += " where j.商品CD=s.商品CD(+) and j.商品CD=:1";
            if (AppData.ClassCvnet.config.UserFlg == 56) sql_str += " and s.承認FLG=1";
            sql_str += " order by j.色CD,j.サイズCD";
            var ret_csv = AppData.Http!.AspxSqlQuery(sql_str, v_para);

            //Form1.kake = ret_csv.Rows[0][6];
            //Form1.kakeritu = ret_csv.Rows[0][6];

            if (ret_csv.Rows.Count == 0)
            {
                ClientLib.MessageBoxError(this,"商品マスタ、もしくは商品色サイズマスタがありませんでした", "注意");
                
                return;
            }
            //Form1.TextLabel1.Value = init_para[0];
            //Form1.gedaihasu = init_para[4];
            //Form1.gedaiketa = init_para[3];

            v_para = new string[1];
            v_para[0] = init_para[0];   /* 商品CD */
            /* 画面表示処理 */
            CreateDsp(ret_csv, v_para);

            /* 専門店 */
            if (AppData.ClassCvnet.config.smtflg == 1)
            {
                //Form1.TextLabel7.Visible =$true;
            }
        }
        void CreateDsp(DataTable ret_csv, string[] v_para) 
        {
            //Form1.genka = ret_csv.Rows[0][3];
            //Form1.TextLabel2.Value = ret_csv.Rows[0][4];
            //Form1.shohi = ret_csv.Rows[0][5];

            //Form1.siirene << ret_csv;

            //Form1.TextLabel5.Value = ret_csv.Rows[0][2];
            //Form1.nouhinbi = ret_csv.Rows[0][8];
            //Form1.TextLabel7.Value = ret_csv.Rows[0][10];
            //Form1.TextLabel8.Value = ret_csv.Rows[0][11];
            //var egata = ret_csv.Rows[0][9];

            //var inscsv = new SatooCSVDocument;
            //inscsv.inscol(1);
            //inscsv.setcolumnname(0, "Flex0");

            //var siz_mei = "get_sizename(商品CD,サイズCD)";
            //var col_mei = "get_colorname(色CD)";

            //if (AppData.ClassCvnet.UserFlg == 87)
            //{
            //    siz_mei = "GET_SIZENAME2(B.予備05,A.サイズCD)";
            //    col_mei = "get_colorname2(B.展示会CD,B.予備04,A.色CD)";
            //}
            //if (AppData.ClassCvnet.config.ColSizMei == 1) { siz_mei = "サイズ名"; col_mei = "色名"; }

            //var sql_str = string.Empty;
            //if (AppData.ClassCvnet.config.ColSizFlg == 1)
            //{
            //    sql_str = "select distinct サイズCD," + siz_mei + " サイズ from HC$master_shohin_jan where 商品CD=:1 order by サイズCD";
            //}
            //else
            //{
            //    sql_str = "select distinct 色CD," + col_mei + " 色 from HC$master_shohin_jan where 商品CD=:1 order by 色CD";
            //}

            //if (AppData.ClassCvnet.config.UserFlg == 87)
            //{
            //    if (AppData.ClassCvnet.config.ColSizFlg == 1)
            //    {
            //        sql_str = "select distinct j.サイズCD,GET_SIZENAME2(s.予備05,j.サイズCD) サイズ from HC$master_shohin_jan j,HC$master_shohin s where j.商品CD=s.商品CD and j.商品CD=:1 order by j.サイズCD";
            //    }
            //    else
            //    {
            //        sql_str = "select distinct j.色CD,GET_COLORNAME2(s.展示会CD,s.予備04,j.色CD) 色 from HC$master_shohin_jan j,HC$master_shohin s where j.商品CD=s.商品CD and  j.商品CD=:1 order by j.色CD";
            //    }
            //}

            //var col_csv = AppData.Http!.AspxSqlQuery(sql_str, v_para);
            //Form1.csv_col << col_csv;

            //var cnt = col_csv.Rows.Count;
            //if (col_csv.Rows.Count > 30) cnt = 30;
            //if (cnt > 15) OnResize(cnt - 15);

            //for (var i = 0; i < col_csv.Rows.Count; i++)
            //{
            //    var flexcol = Form1.Spread1.FlexRecord1.FindChild("Flex" + (i + 1).ToString());
            //    if (flexcol != null)
            //    {
            //        inscsv.inscol(1);
            //        inscsv.setcolumnname(i + 1, "Flex" + str(i + 1) + ".data");
            //        flexcol.title = col_csv.getcell(i, 0);
            //        flexcol.width = 30;
            //    }
            //}
            //if (AppData.ClassCvnet.config.ColSizFlg == 0)
            //{
            //    sql_str = "select distinct サイズCD," + siz_mei + " サイズ from HC$master_shohin_jan where 商品CD=:1 order by サイズCD";
            //}
            //else
            //{
            //    sql_str = "select distinct 色CD," + col_mei + " 色 from HC$master_shohin_jan where 商品CD=:1 order by 色CD";
            //}

            //if (AppData.ClassCvnet.config.UserFlg == 87)
            //{
            //    if (AppData.ClassCvnet.config.ColSizFlg == 0)
            //    {
            //        sql_str = "select distinct j.サイズCD,GET_SIZENAME2(s.予備05,j.サイズCD) サイズ from HC$master_shohin_jan j,HC$master_shohin s where j.商品CD=s.商品CD and j.商品CD=:1 order by j.サイズCD";
            //    }
            //    else
            //    {
            //        sql_str = "select distinct j.色CD,GET_COLORNAME2(s.展示会CD,s.予備04,j.色CD) 色 from HC$master_shohin_jan j,HC$master_shohin s where j.商品CD=s.商品CD and  j.商品CD=:1 order by j.色CD";
            //    }
            //}

            //var siz_csv = AppData.Http!.AspxSqlQuery(sql_str, v_para);
            //Form1.csv_siz << siz_csv;

            //var cnt = siz_csv.Rows.Count;
            //if (siz_csv.Rows.Count > 20) cnt = 20; 
            //if (col_csv.Rows.Count > 10) cnt++;
            //if (cnt > 4) OnResize2(cnt - 4);

            //for (var i = 0; i < siz_csv.Rows.Count; i++)
            //{
            //    inscsv.insrow(1);
            //    inscsv.setcell(i, 0, siz_csv.Rows[i][0] + " " + siz_csv.Rows[i][1]);
            //}
            //var max_row = ret_csv.Rows.Count;
            //for (var i = 0; i < max_row; i++)
            //{
            //    var colnum = col_csv.find(0, "0==" + ret_csv.getcell(i, Form1.ColSizFlg));
            //    if (colnum < 0 || colnum >= 30) continue;
            //    var siznum = siz_csv.find(0, "0==" + ret_csv.getcell(i, abs(Form1.ColSizFlg - 1)));
            //    if (siznum < 0) continue;
                
            //    inscsv.setcell(siznum, colnum + 1, ret_csv.getcell(i, 2));
            //}
            //Form1.Spread1 << inscsv;

            //var row = Form1.Spread1.GetRow();
            //var colnum = Form1.csv_col.rows;
            //if (colnum > 30) colnum = 30;
            //while (!row.end)
            //{
            //    for (var j = 0; j < colnum; j++)
            //    {
            //        var childtxt = row.getcolumn(j);
            //        var ColorCode = "";
            //        var SizCode = "";
            //        if (AppData.ClassCvnet.config.ColSizFlg == 1)
            //        {
            //            SizCode = new String(Form1.csv_col.getCell(j, 0));
            //            ColorCode = new String(Form1.csv_siz.getCell(row.position, 0));
            //        }
            //        else
            //        {
            //            ColorCode = new String(Form1.csv_col.getCell(j, 0));
            //            SizCode = new String(Form1.csv_siz.getCell(row.position, 0));
            //        }
            //        var find_row = ret_csv.find(0, "0==" + str(ColorCode), "1==" + str(SizCode));
            //        if (find_row < 0)
            //        {
            //            childtxt.bgcolor = $LGRAY;
            //            childtxt.Editable = $FALSE;
            //        }
            //    }
            //    row.movenext();
            //}
            //Form1.kakeritu.OnTouch();
            //if (AppData.ClassCvnet.config.UserFlg == 18)
            //{
            //    Form1.Label1.Visible = $FALSE; Form1.Label2.Visible = $FALSE;
            //    Form1.kakeritu.Visible = $FALSE; Form1.TextLabel6.Visible = $FALSE;
            //}
            
            //Image img00;
            //var image_name = egata;
            //if (img00.Height == 0 || img00.Width == 0)
            //{
            //    try
            //    {
            //        img00.LoadImage(AppData.AspxPath + "Data/" + AppData.DataAddPath + image_name);
            //    }
            //    catch (Exception e)
            //    {
            //    }
            //}
            //Form1.ImageLabel1.SetImage(img00);

            //if (AppData.ClassCvnet.config.usegenka == 1)
            //{
            //    sql_str = "select 原価FLG桁 from HC$MASTER_HHT_KANRI";
            //    var ret_csv1 = AppData.Http!.AspxSqlQuery(sql_str, v_para);
            //    if (int.Parse(ret_csv1.Rows[0][0].ToString()) >= 0) Form1.genka_keta = ret_csv1.Rows[0][0];
            //}
        }
    }
}
