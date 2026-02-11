using CommunityToolkit.Mvvm.ComponentModel;
using CvnetClient.Models;
using System.Data;
using System.Windows;

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

        /************/
        /* 初期処理 */
        /************/
        public void OnInit(string[] init_para)
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
                ClientLib.MessageBoxError(this, "商品マスタ、もしくは商品色サイズマスタがありませんでした", "注意");

                return;
            }
            //Form1.TextLabel1.Value = init_para[0];
            //Form1.gedaihasu = init_para[4];
            //Form1.gedaiketa = init_para[3];

            v_para = new string[1];
            v_para[0] = init_para[0];   /* 商品CD */
            /* 画面表示処理 */ 

            /* 専門店 */
            if (AppData.ClassCvnet.config.smtflg == 1)
            {
                //Form1.TextLabel7.Visible =$true;
            }
        }

        #region Events
        #endregion

        #region Function
        #endregion
    }

    public partial class Sku01Dsp : ObservableObject
    { 
        
    }
}
