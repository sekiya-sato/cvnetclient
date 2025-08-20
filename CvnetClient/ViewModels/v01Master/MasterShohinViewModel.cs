using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using System.Collections.ObjectModel;
using System.Data; 

namespace CvnetClient.ViewModels
{
    public partial class MasterShohinViewModel : BaseViewModel
    {
        [RelayCommand]
        void Init()
        { 
        
        }

        [ObservableProperty]
        ObservableCollection<MasterShohin>? listShohin;

        [ObservableProperty]
        MasterShohin? selectedShohin;

        string sql_collist = """
               商品CD,商品名,略称,旧コード,展示会CD,ブランドCD,アイテムCD,シーズンCD,素材CD,
               デザイナーCD,メーカーCD,原産国CD,
               元上代,上代,売変日,原価,営業原価,加工工賃,デリバリー日,納品日,店頭投入日,
               JANコード1,JANコード2,JANコード3,洗濯表示,絵型名,メモ,消費税計算方法,在庫管理FLG,消費税CD,
               名称CD01,名称CD02,名称CD03,名称CD04,名称CD05,名称CD06,名称CD07,名称CD08,名称CD09,名称CD10,
               自動配分FLG,販売期限,商品区分FLG,商品サイズ区分,基準倉庫CD,ゼロ単価区分,JAN先頭桁,POS区分,
               予備01,予備02,予備03,予備04,予備05,予備06,予備07,予備08,予備09,予備10,
               予備11,予備12,予備13,予備14,予備15,予備16,予備17,予備18,予備19,予備20,
               仕入区分,消化桁切指定,消化端数区分,消化計算区分,消化掛率,男女区分,コラボ出力区分,
               代表品番FLG, セール区分,リピート日,メーカー品番,
               仕入価格,納品区分,絵型名2,販売開始日,外貨単価,EC連携,EC取置
        """;

        string sql_query = """
            SELECT * FROM (
            SELECT A.SEQ_NO,A.VDATE_CREATE,A.VDATE_UPDATE,
                   [COLLIST]
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='TNJ' and H.名称CD=A.展示会CD),'.') 展示会名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='BRD' and H.名称CD=A.ブランドCD),'.') ブランド名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='ITM' and H.名称CD=A.アイテムCD),'.') アイテム名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='SZN' and H.名称CD=A.シーズンCD),'.') シーズン名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='SZI' and H.名称CD=A.素材CD),'.') 素材名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='DZN' and H.名称CD=A.デザイナーCD),'.') デザイナー名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='MKR' and H.名称CD=A.メーカーCD),'.') メーカー名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='GEN' and H.名称CD=A.原産国CD),'.') 原産国名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B01' and H.名称CD=A.名称CD01),'.') 補足01名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B02' and H.名称CD=A.名称CD02),'.') 補足02名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B03' and H.名称CD=A.名称CD03),'.') 補足03名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B04' and H.名称CD=A.名称CD04),'.') 補足04名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B05' and H.名称CD=A.名称CD05),'.') 補足05名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B06' and H.名称CD=A.名称CD06),'.') 補足06名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B07' and H.名称CD=A.名称CD07),'.') 補足07名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B08' and H.名称CD=A.名称CD08),'.') 補足08名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B09' and H.名称CD=A.名称CD09),'.') 補足09名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B10' and H.名称CD=A.名称CD10),'.') 補足10名
                   ,NVL((select H.得意先名 from HC$MASTER_TOKUI H where H.得意先CD=A.基準倉庫CD),'.') 倉庫名
                   ,A.入力社員CD ||' '||(select S.名前 from HC$MASTER_SHAIN S where S.社員CD=A.入力社員CD) 最終修正者
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='MLK' and H.名称CD=A.男女区分),'.') 男女区分名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='STH' and H.名称CD=A.洗濯表示),'.') 洗濯表示名
                   ,DECODE(A.承認FLG,1,'済','未済') 承認
                   ,DECODE((select count(*)  d	from hc$master_shohin_genka_henkou sgh where A.商品CD = sgh.商品CD), 0, 0, 1) 商品原価変更履歴有無FLG
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y01' and H.名称CD=A.予備01),'.') 予備01名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y02' and H.名称CD=A.予備02),'.') 予備02名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y03' and H.名称CD=A.予備03),'.') 予備03名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y04' and H.名称CD=A.予備04),'.') 予備04名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y05' and H.名称CD=A.予備05),'.') 予備05名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y06' and H.名称CD=A.予備06),'.') 予備06名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y07' and H.名称CD=A.予備07),'.') 予備07名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y08' and H.名称CD=A.予備08),'.') 予備08名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y09' and H.名称CD=A.予備09),'.') 予備09名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y10' and H.名称CD=A.予備10),'.') 予備10名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y11' and H.名称CD=A.予備11),'.') 予備11名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y12' and H.名称CD=A.予備12),'.') 予備12名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y13' and H.名称CD=A.予備13),'.') 予備13名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y14' and H.名称CD=A.予備14),'.') 予備14名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y15' and H.名称CD=A.予備15),'.') 予備15名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y16' and H.名称CD=A.予備16),'.') 予備16名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y17' and H.名称CD=A.予備17),'.') 予備17名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y18' and H.名称CD=A.予備18),'.') 予備18名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y19' and H.名称CD=A.予備19),'.') 予備19名
                   ,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y20' and H.名称CD=A.予備20),'.') 予備20名
            FROM HC$Master_SHOHIN A
            WHERE A.商品CD{0}:1
            ORDER BY A.商品CD {1}
            ) WHERE ROWNUM<={2}
            """;

        /// <summary>
		/// 一覧表示
		/// </summary>
        void subList(string startCd, string sql_P1, string sql_P2, int sql_P3)
        { 
            var sql = string.Format(sql_query, sql_P1, sql_P2, sql_P3);
            var retData = AppData.Http?.AspxSqlQuery(sql, new string[] { startCd });
            if (retData == null || retData.Rows.Count == 0) return;
            var list = (from DataRow dr in retData.Rows
                        select new MasterShohin
                        {
                            SeqNo = Convert.ToInt64(dr["SEQ_NO"]),
                            VdateCreate = Convert.ToDecimal(dr["VDATE_CREATE"]),
                            VdateUpdate = Convert.ToDecimal(dr["VDATE_UPDATE"]),
                            ProductCD = dr["商品CD"].ToString() ?? string.Empty,
                            ProductName = dr["商品名"].ToString() ?? string.Empty,
                            Abbreviation = dr["略称"].ToString() ?? string.Empty,
                            OldCode = dr["旧コード"].ToString() ?? string.Empty,
                            ExhibitCD = dr["展示会CD"].ToString() ?? string.Empty,
                            BrandCD = dr["ブランドCD"].ToString() ?? string.Empty,
                            ItemCD = dr["アイテムCD"].ToString() ?? string.Empty,
                            SeasonCD = dr["シーズンCD"].ToString() ?? string.Empty,
                            MaterialCD = dr["素材CD"].ToString() ?? string.Empty,
                            DesignCD = dr["デザイナーCD"].ToString() ?? string.Empty,
                            ManufactCD = dr["メーカーCD"].ToString() ?? string.Empty,
                            MadeInCD = dr["原産国CD"].ToString() ?? string.Empty,

                        }).OrderBy(c => c.ProductCD).ToList();
            Common.ConvertDotStringDel(list);
            ListShohin = new ObservableCollection<MasterShohin>(list);
            if (ListShohin.Count > 0)
            {
                SelectedShohin = ListShohin[0];
            }
        }
    }
}
