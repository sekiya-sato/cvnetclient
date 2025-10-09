using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using System.Collections.ObjectModel;
using System.Data; 

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01Sho2ViewModel : BaseViewModel
    {
        [RelayCommand]
        void Init()
        {
            EditProduct = new MasterShohin();

            var comboItem = AppData.ClassCvnet.comboItem00;
            #region Set ComboList
            // Set 在庫管理 ComboList
            InvMngmentList = comboItem.ComboItem_00<int>("する");
            EditProduct.InvMngmentFLG = InvMngmentList.FirstOrDefault().Key;
            // Set セール区分 ComboList
            SalesCateList = new Dictionary<int, string>
            {
                {  0, "0 プロパー商品" },
                {  1, "1 セール商品" }
            };
            EditProduct.SalesCate = SalesCateList.FirstOrDefault().Key;
            // Set 売上基準自動補充 ComboList
            AutoDistList = new Dictionary<int, string>
            {
                {  0, "0 自動補充しない" },
                {  1, "1 自動補充する" }
            };
            EditProduct.AutoDistFLG = AutoDistList.FirstOrDefault().Key;
            // Set 納品区分 ComboList
            DeliverCateList = new Dictionary<int, string>
            {
                {  0, "0 倉庫入庫" },
                {  1, "1 店舗入庫" }
            };
            EditProduct.DeliveryCate = DeliverCateList.FirstOrDefault().Key;
            // Set EC連携 ComboList 
            EcConnectList = new Dictionary<int, string>
            {
                {  0, "0 連携しない" },
                {  1, "1 連携する" }
            };
            EditProduct.EcConnect = EcConnectList.FirstOrDefault().Key;
            // Set EC取置 ComboList
            EcReserveList = new Dictionary<int, string>
            {
                {  0, "0 取置しない" },
                {  1, "1 取置する" }
            };
            EditProduct.EcReserve = EcReserveList.FirstOrDefault().Key;
            // Set 仕入区分 ComboList
            PurchCateList = comboItem.ComboItem_00<int>("仕入区分");
            EditProduct.PurchaseCate = PurchCateList.FirstOrDefault().Key;
            // Set 消化計算 ComboList
            DgCalcList = new Dictionary<int, string>
            {
                {  0, "0 仕入価格代入" },
                {  1, "1 掛率計算" }
            };
            EditProduct.DgCalcCate = DgCalcList.FirstOrDefault().Key;
            // Set 消化桁切 ComboList
            DgCutOffList = comboItem.ComboItem_00<int>("桁切");
            EditProduct.DgCutOffSpec = DgCutOffList.FirstOrDefault().Key;
            // Set 消化端数 ComboList
            ConPurcCateList = comboItem.ComboItem_00<int>("端数");
            EditProduct.ConsignPurcCate = ConPurcCateList.FirstOrDefault().Key;
            // Set POS区分 ComboList
            PosCateList = new Dictionary<int, string>
            {
                {  0, "0 通常" },
                {  9, "9 POSﾏｽﾀ削除指示" },
                { 10, "10 出力しない" },
            };
            EditProduct.PosCate = PosCateList.FirstOrDefault().Key;
            // Set 代表品番FLG ComboList
            RepresNoFlgList = new Dictionary<int, string>
            {
                {  0, "0 通常商品" },
                {  1, "1 代表品番商品" }
            };
            EditProduct.RepresentNoFLG = RepresNoFlgList.FirstOrDefault().Key;
            // Set 商品区分FLG ComboList
            ProdCateFlgList = new Dictionary<int, string>
            {
                {  0, "0 通常" },
                {  1, "1 商品外" }
            };
            EditProduct.ProdCateFLG = ProdCateFlgList.FirstOrDefault().Key;
            // Set 消費税CD (軽減税率) ComboList
            TaxCDList = new Dictionary<int, string>
            {
                {  1, "1 通常税率" },
                {  2, "2 軽減税率" }
            };
            EditProduct.TaxCD = TaxCDList.FirstOrDefault().Key;
            // Set 商品サイズ区分 ComboList  
            var prod_list = new Dictionary<string, string>();
            string sql_query = "select 名称CD, 名称 from hc$master_meisho where 名称区分='IDX' and (名称CD like 'US%' or 名称CD='SIZ' ) order by 名称CD";
            var get_prodSiz = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_prodSiz.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                prod_list.Add(key, string.Format("{0} {1}", key, value));
            }
            ProdSizCateList = prod_list;
            EditProduct.ProdSizeCate = ProdSizCateList.FirstOrDefault().Key;
            // Set ゼロ単価区分 ComboList
            ZeroPriCateList = new Dictionary<int, string>
            {
                {  0, "0 禁止" },
                {  1, "1 許可" },
                {  2, "2 必須入力" }
            };
            EditProduct.ZeroPriceCate = ZeroPriCateList.FirstOrDefault().Key;
            // Set 消費税計算方法 ComboList
            TaxCalcList = comboItem.ComboItem_00<int>("課税区分");
            EditProduct.TaxCalcMethod = TaxCalcList.FirstOrDefault().Key;
            // Set コラボ出力区分 (下札サイズ) ComboList
            ColOutCateList = new Dictionary<int, string>
            {
                {  0, "0 禁止" },
                {  1, "1 許可" },
                {  2, "2 必須入力" }
            };
            EditProduct.CollabOutCate = ColOutCateList.FirstOrDefault().Key;
            #endregion
        }

        [ObservableProperty]
        ObservableCollection<MasterShohin>? listProduct;

        /// <summary>
		/// 修正用の一時的なProductオブジェクト
		/// </summary>
		[ObservableProperty]
        MasterShohin? editProduct;

        [ObservableProperty]
        MasterShohin? selectedProduct;

        string sql_collist = """
               商品CD,商品名,略称,旧コード,展示会CD,ブランドCD,アイテムCD,シーズンCD,素材CD,デザイナーCD,
               メーカーCD,原産国CD,元上代,上代,売変日,原価,営業原価,加工工賃,デリバリー日,納品日,
               店頭投入日,JANコード1,JANコード2,JANコード3,洗濯表示,絵型名,メモ,消費税計算方法,在庫管理FLG,消費税CD, 
               名称CD01,名称CD02,名称CD03,名称CD04,名称CD05,名称CD06,名称CD07,名称CD08,名称CD09,名称CD10,
               自動配分FLG,販売期限,商品区分FLG,商品サイズ区分,基準倉庫CD,ゼロ単価区分,JAN先頭桁,POS区分,
               予備01,予備02,予備03,予備04,予備05,予備06,予備07,予備08,予備09,予備10,
               予備11,予備12,予備13,予備14,予備15,予備16,予備17,予備18,予備19,予備20,
               仕入区分,消化桁切指定,消化端数区分,消化計算区分,消化掛率,男女区分,コラボ出力区分,代表品番FLG, セール区分,リピート日,
               メーカー品番,仕入価格,納品区分,絵型名2,販売開始日,外貨単価,EC連携,EC取置
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
                            OriPrice = long.Parse(dr["元上代"].ToString()), 
                            Price = long.Parse(dr["上代"].ToString()),
                            PriceChgDate = dr["売変日"].ToString() ?? string.Empty,
                            Cost = long.Parse(dr["原価"].ToString()),
                            OpCostPrice = long.Parse(dr["営業原価"].ToString()),
                            ManufactFee = long.Parse(dr["加工工賃"].ToString()),
                            CustDeliDate = dr["デリバリー日"].ToString() ?? string.Empty,
                            DeliveryDate = dr["納品日"].ToString() ?? string.Empty,
                            InitLaunchDate = dr["店頭投入日"].ToString() ?? string.Empty,
                            JanCode1 = dr["JANコード1"].ToString() ?? string.Empty,
                            JanCode2 = dr["JANコード2"].ToString() ?? string.Empty,
                            JanCode3 = dr["JANコード3"].ToString() ?? string.Empty,
                            CareLabel = dr["洗濯表示"].ToString() ?? string.Empty,
                            ImgName = dr["絵型名"].ToString() ?? string.Empty,
                            Memo = dr["メモ"].ToString() ?? string.Empty,
                            TaxCalcMethod = int.Parse(dr["消費税計算方法"].ToString()),
                            InvMngmentFLG = Convert.ToInt32(dr["在庫管理FLG"]),
                            TaxCD = long.Parse(dr["消費税CD"].ToString()),
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
                            AutoDistFLG = Convert.ToInt32(dr["自動配分FLG"]),
                            SalesPeriod = dr["販売期限"].ToString() ?? string.Empty,
                            ProdCateFLG = Convert.ToInt32(dr["商品区分FLG"]),
                            ProdSizeCate = dr["商品サイズ区分"].ToString() ?? string.Empty,
                            StandWareCD = dr["基準倉庫CD"].ToString() ?? string.Empty,
                            ZeroPriceCate = Convert.ToInt32(dr["ゼロ単価区分"]),
                            Jan1stdigit = dr["JAN先頭桁"].ToString() ?? string.Empty,
                            PosCate = Convert.ToInt32(dr["POS区分"]),
                            Reserve01 = dr["予備01"].ToString() ?? string.Empty,
                            Reserve02 = dr["予備02"].ToString() ?? string.Empty,
                            Reserve03 = dr["予備03"].ToString() ?? string.Empty,
                            Reserve04 = dr["予備04"].ToString() ?? string.Empty,
                            Reserve05 = dr["予備05"].ToString() ?? string.Empty,
                            Reserve06 = dr["予備06"].ToString() ?? string.Empty,
                            Reserve07 = dr["予備07"].ToString() ?? string.Empty,
                            Reserve08 = dr["予備08"].ToString() ?? string.Empty,
                            Reserve09 = dr["予備09"].ToString() ?? string.Empty,
                            Reserve10 = dr["予備10"].ToString() ?? string.Empty,
                            Reserve11 = dr["予備11"].ToString() ?? string.Empty,
                            Reserve12 = dr["予備12"].ToString() ?? string.Empty,
                            Reserve13 = dr["予備13"].ToString() ?? string.Empty,
                            Reserve14 = dr["予備14"].ToString() ?? string.Empty,
                            Reserve15 = dr["予備15"].ToString() ?? string.Empty,
                            Reserve16 = dr["予備16"].ToString() ?? string.Empty,
                            Reserve17 = dr["予備17"].ToString() ?? string.Empty,
                            Reserve18 = dr["予備18"].ToString() ?? string.Empty,
                            Reserve19 = dr["予備19"].ToString() ?? string.Empty,
                            Reserve20 = dr["予備20"].ToString() ?? string.Empty,
                            PurchaseCate = Convert.ToInt32(dr["仕入区分"]),
                            DgCutOffSpec = Convert.ToInt32(dr["消化桁切指定"]),
                            ConsignPurcCate = Convert.ToInt32(dr["消化端数区分"]),
                            DgCalcCate = Convert.ToInt32(dr["消化計算区分"]),
                            ConsignPurcRate = Convert.ToInt32(dr["消化掛率"]),
                            GenderCate = dr["男女区分"].ToString() ?? string.Empty,
                            CollabOutCate = Convert.ToInt32(dr["コラボ出力区分"]),
                            RepresentNoFLG = Convert.ToInt32(dr["代表品番FLG"]),
                            SalesCate = Convert.ToInt32(dr["セール区分"]),
                            RepeatDate = dr["リピート日"].ToString() ?? string.Empty,
                            MakerNo = dr["メーカー品番"].ToString() ?? string.Empty,
                            PurchasePrice = long.Parse(dr["仕入価格"].ToString()),
                            DeliveryCate = Convert.ToInt32(dr["納品区分"]),
                            ImgName2 = dr["絵型名2"].ToString() ?? string.Empty,
                            SaleStDate = dr["販売開始日"].ToString() ?? string.Empty,
                            ForeignCurPrice = long.Parse(dr["外貨単価"].ToString()),
                            EcConnect = Convert.ToInt32(dr["EC連携"]),
                            EcReserve = Convert.ToInt32(dr["EC取置"]),

                            ExhibitName = dr["展示会名"].ToString() ?? string.Empty,
                            BrandName = dr["ブランド名"].ToString() ?? string.Empty,
                            ItemName = dr["アイテム名"].ToString() ?? string.Empty,
                            SeasonName = dr["シーズン名"].ToString() ?? string.Empty,
                            MaterialName = dr["素材名"].ToString() ?? string.Empty,
                            DesignName = dr["デザイナー名"].ToString() ?? string.Empty,
                            ManufactName = dr["メーカー名"].ToString() ?? string.Empty,
                            MadeInName = dr["原産国名"].ToString() ?? string.Empty,
                            SubName01 = dr["補足01名"].ToString() ?? string.Empty,
                            SubName02 = dr["補足02名"].ToString() ?? string.Empty,
                            SubName03 = dr["補足03名"].ToString() ?? string.Empty,
                            SubName04 = dr["補足04名"].ToString() ?? string.Empty,
                            SubName05 = dr["補足05名"].ToString() ?? string.Empty,
                            SubName06 = dr["補足06名"].ToString() ?? string.Empty,
                            SubName07 = dr["補足07名"].ToString() ?? string.Empty,
                            SubName08 = dr["補足08名"].ToString() ?? string.Empty,
                            SubName09 = dr["補足09名"].ToString() ?? string.Empty,
                            SubName10 = dr["補足10名"].ToString() ?? string.Empty,
                            StockName = dr["倉庫名"].ToString() ?? string.Empty,
                            ModifiedBy = dr["最終修正者"].ToString() ?? string.Empty,
                            GenderCateName = dr["男女区分名"].ToString() ?? string.Empty,
                            CareLabelName = dr["洗濯表示名"].ToString() ?? string.Empty,
                            Approval = dr["承認"].ToString() ?? string.Empty,
                            CostChgHistFLG = Convert.ToInt32(dr["商品原価変更履歴有無FLG"]),
                            ReserveName01 = dr["予備01名"].ToString() ?? string.Empty,
                            ReserveName02 = dr["予備02名"].ToString() ?? string.Empty,
                            ReserveName03 = dr["予備03名"].ToString() ?? string.Empty,
                            ReserveName04 = dr["予備04名"].ToString() ?? string.Empty,
                            ReserveName05 = dr["予備05名"].ToString() ?? string.Empty,
                            ReserveName06 = dr["予備06名"].ToString() ?? string.Empty,
                            ReserveName07 = dr["予備07名"].ToString() ?? string.Empty,
                            ReserveName08 = dr["予備08名"].ToString() ?? string.Empty,
                            ReserveName09 = dr["予備09名"].ToString() ?? string.Empty,
                            ReserveName10 = dr["予備10名"].ToString() ?? string.Empty,
                            ReserveName11 = dr["予備11名"].ToString() ?? string.Empty,
                            ReserveName12 = dr["予備12名"].ToString() ?? string.Empty,
                            ReserveName13 = dr["予備13名"].ToString() ?? string.Empty,
                            ReserveName14 = dr["予備14名"].ToString() ?? string.Empty,
                            ReserveName15 = dr["予備15名"].ToString() ?? string.Empty,
                            ReserveName16 = dr["予備16名"].ToString() ?? string.Empty,
                            ReserveName17 = dr["予備17名"].ToString() ?? string.Empty,
                            ReserveName18 = dr["予備18名"].ToString() ?? string.Empty,
                            ReserveName19 = dr["予備19名"].ToString() ?? string.Empty,
                            ReserveName20 = dr["予備20名"].ToString() ?? string.Empty
                        }).OrderBy(c => c.ProductCD).ToList();
            //Common.ConvertDotStringDel(list);
            ListProduct = new ObservableCollection<MasterShohin>(list);
            if (ListProduct.Count > 0)
            {
                SelectedProduct = ListProduct[0];
            }
        }
         
        partial void OnAutoDistListChanged(Dictionary<int, string> value)
        { 
            
        }

        #region Combobox List 
        // 在庫管理 
        [ObservableProperty]
        public Dictionary<int, string>? m_InvMngmentList;
        // セール区分 
        [ObservableProperty]
        public Dictionary<int, string>? m_salesCateList;
        // autoDistFLG 
        [ObservableProperty]
        public Dictionary<int, string>? m_autoDistList;
        // 納品区分 
        [ObservableProperty]
        public Dictionary<int, string>? m_deliverCateList;
        // EC連携 
        [ObservableProperty]
        public Dictionary<int, string>? m_ecConnectList;
        // EC取置 
        [ObservableProperty]
        public Dictionary<int, string>? m_ecReserveList;
        // 仕入区分
        [ObservableProperty]
        public Dictionary<int, string>? m_purchCateList;
        // 消化計算
        [ObservableProperty]
        public Dictionary<int, string>? m_dgCalcList;
        // 消化桁切
        [ObservableProperty]
        public Dictionary<int, string>? m_dgCutOffList;
        // 消化端数
        [ObservableProperty]
        public Dictionary<int, string>? m_conPurcCateList;
        // POS区分
        [ObservableProperty]
        public Dictionary<int, string>? m_posCateList;
        // 代表品番FLG
        [ObservableProperty]
        public Dictionary<int, string>? m_represNoFlgList;
        // 商品区分FLG
        [ObservableProperty]
        public Dictionary<int, string>? m_prodCateFlgList;
        // 消費税CD (軽減税率)
        [ObservableProperty]
        public Dictionary<int, string>? m_taxCDList;
        // 商品サイズ区分
        [ObservableProperty]
        public Dictionary<string, string>? m_prodSizCateList;
        // ゼロ単価区分
        [ObservableProperty]
        public Dictionary<int, string>? m_zeroPriCateList;
        // 消費税計算方法
        [ObservableProperty]
        public Dictionary<int, string>? m_taxCalcList;
        // コラボ出力区分 (下札サイズ)
        [ObservableProperty]
        public Dictionary<int, string>? m_colOutCateList;
        #endregion

        #region Dialog Search
        [RelayCommand]
        public void SelDspUpdate()
        {
            //商品
            //if (AppData.ClassCvnet.MstDialog.ContainsKey("商品"))
            //{
            //    var ar = new string[] { "0" };
            //    var vm = AppData.DlgService.GetSelSho(AppData.ClassCvnet.MstDialog["商品"].v_mstname, null, ar);
            //}

            //担当 
            //if (AppData.ClassCvnet.MstDialog.ContainsKey("担当"))
            //{
            //    var ar = new string[] { "0" };
            //    var vm = AppData.DlgService.GetSelUsr("担当", null, ar);
            //}

            //Ac得意先
            //if (AppData.ClassCvnet.MstDialog.ContainsKey("Ac得意先"))
            //{
            //    var ar = new string[] { "0" };
            //    var vm = AppData.DlgService.GetSelTok("Ac得意先", null, ar);
            //}

            //仕入先
            //if (AppData.ClassCvnet.MstDialog.ContainsKey("仕入先"))
            //{
            //    var ar = new string[] { "0" };
            //    var vm = AppData.DlgService.GetSelSir("仕入先", null, ar);
            //}

            //顧客
            //if (AppData.ClassCvnet.MstDialog.ContainsKey("顧客"))
            //{
            //    var ar = new string[] { "1", "2", "" };
            //    var vm = AppData.DlgService.GetSelKokyaku("顧客", ar);
            //}

            //ポイント
            if (AppData.ClassCvnet.MstDialog.ContainsKey("ポイント"))
            {
                var ar = new string[] { "2" };
                var vm = AppData.DlgService.GetSelPoint("ポイント", null, ar);
            }

            //仕入先
            //if (AppData.ClassCvnet.MstDialog.ContainsKey("仕入先"))
            //{
            //    var ar = new string[] { "0" };
            //    var vm = AppData.DlgService.GetSelSir("仕入先", null, ar);
            //}
        }

        [RelayCommand]
        public void SelBrand(object value)
        { 
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditProduct != null)
            {
                EditProduct.BrandCD = get_sel00.Code;
                EditProduct.BrandName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelItem(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditProduct != null)
            {
                EditProduct.ItemCD = get_sel00.Code;
                EditProduct.ItemName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelSeason(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditProduct != null)
            {
                EditProduct.SeasonCD = get_sel00.Code;
                EditProduct.SeasonName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelMaterial(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditProduct != null)
            {
                EditProduct.MaterialCD = get_sel00.Code;
                EditProduct.MaterialName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelDesign(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditProduct != null)
            {
                EditProduct.DesignCD = get_sel00.Code;
                EditProduct.DesignName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelMadeIn(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditProduct != null)
            {
                EditProduct.MadeInCD = get_sel00.Code;
                EditProduct.MadeInName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelExhibit(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditProduct != null)
            {
                EditProduct.ExhibitCD = get_sel00.Code;
                EditProduct.ExhibitName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelManufact(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditProduct != null)
            {
                EditProduct.ManufactCD = get_sel00.Code;
                EditProduct.ManufactName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelName((object Result1, object Result2) value)
        {
            var (result1, result2) = value;

            var get_sel00 = (SelValueModel)result1;
            string kubun = string.Empty;
            if (result2 is string res2) kubun = res2;

            if (get_sel00 != null && EditProduct != null)
            {
                if (kubun == "B01")
                {
                    EditProduct.NameCD01 = get_sel00.Code;
                    EditProduct.SubName01 = get_sel00.Name;
                }
                if (kubun == "B02")
                {
                    EditProduct.NameCD02 = get_sel00.Code;
                    EditProduct.SubName02 = get_sel00.Name;
                }
                if (kubun == "B03")
                {
                    EditProduct.NameCD03 = get_sel00.Code;
                    EditProduct.SubName03 = get_sel00.Name;
                }
                if (kubun == "B04")
                {
                    EditProduct.NameCD04 = get_sel00.Code;
                    EditProduct.SubName04 = get_sel00.Name;
                }
                if (kubun == "B05")
                {
                    EditProduct.NameCD05 = get_sel00.Code;
                    EditProduct.SubName05 = get_sel00.Name;
                }
                if (kubun == "B06")
                {
                    EditProduct.NameCD06 = get_sel00.Code;
                    EditProduct.SubName06 = get_sel00.Name;
                }
                if (kubun == "B07")
                {
                    EditProduct.NameCD07 = get_sel00.Code;
                    EditProduct.SubName07 = get_sel00.Name;
                }
                if (kubun == "B08")
                {
                    EditProduct.NameCD08 = get_sel00.Code;
                    EditProduct.SubName08 = get_sel00.Name;
                }
                if (kubun == "B09")
                {
                    EditProduct.NameCD09 = get_sel00.Code;
                    EditProduct.SubName09 = get_sel00.Name;
                }
                if (kubun == "B10")
                {
                    EditProduct.NameCD10 = get_sel00.Code;
                    EditProduct.SubName10 = get_sel00.Name;
                }
            } 
        }
        [RelayCommand]
        public void SelStandWare(object value)
        {
            //var get_sel00 = (SelValueModel)value;
            // Note: Require Call SelTok to get ID before Start GetSell00 
            /*
            var get_sel00 = AppData.DlgService.GetSel00("倉庫");
            if (get_sel00 != null && EditProduct != null)
            {
                EditProduct.StandWareCD = get_sel00.SelectedValue?.Code;
                EditProduct.StockName = get_sel00?.SelectedValue?.Name;
            }
            */
        }
        [RelayCommand]
        public void SelReserve((object Result1, object Result2) value)
        {
            var (result1, result2) = value;
            var get_sel00 = (SelValueModel)result1;
            string kubun = string.Empty;
            if (result2 is string res2) kubun = res2;

            if (get_sel00 != null && EditProduct != null)
            {
                if (kubun == "Y01")
                {
                    EditProduct.Reserve01 = get_sel00.Code;
                    EditProduct.ReserveName01 = get_sel00.Name;
                }
                if (kubun == "Y02")
                {
                    EditProduct.Reserve02 = get_sel00.Code;
                    EditProduct.ReserveName02 = get_sel00.Name;
                }
                if (kubun == "Y03")
                {
                    EditProduct.Reserve03 = get_sel00.Code;
                    EditProduct.ReserveName03 = get_sel00.Name;
                }
                if (kubun == "Y04")
                {
                    EditProduct.Reserve04 = get_sel00.Code;
                    EditProduct.ReserveName04 = get_sel00.Name;
                }
                if (kubun == "Y05")
                {
                    EditProduct.Reserve05 = get_sel00.Code;
                    EditProduct.ReserveName05 = get_sel00.Name;
                }
                if (kubun == "Y06")
                {
                    EditProduct.Reserve06 = get_sel00.Code;
                    EditProduct.ReserveName06 = get_sel00.Name;
                }
                if (kubun == "Y07")
                {
                    EditProduct.Reserve07 = get_sel00.Code;
                    EditProduct.ReserveName07 = get_sel00.Name;
                }
                if (kubun == "Y08")
                {
                    EditProduct.Reserve08 = get_sel00.Code;
                    EditProduct.ReserveName08 = get_sel00.Name;
                }
                if (kubun == "Y09")
                {
                    EditProduct.Reserve09 = get_sel00.Code;
                    EditProduct.ReserveName09 = get_sel00.Name;
                }
                if (kubun == "Y10")
                {
                    EditProduct.Reserve10 = get_sel00.Code;
                    EditProduct.ReserveName10 = get_sel00.Name;
                }
                if (kubun == "Y11")
                {
                    EditProduct.Reserve11 = get_sel00.Code;
                    EditProduct.ReserveName11 = get_sel00.Name;
                }
                if (kubun == "Y12")
                {
                    EditProduct.Reserve12 = get_sel00.Code;
                    EditProduct.ReserveName12 = get_sel00.Name;
                }
                if (kubun == "Y13")
                {
                    EditProduct.Reserve13 = get_sel00.Code;
                    EditProduct.ReserveName13 = get_sel00.Name;
                }
                if (kubun == "Y14")
                {
                    EditProduct.Reserve14 = get_sel00.Code;
                    EditProduct.ReserveName14 = get_sel00.Name;
                }
                if (kubun == "Y15")
                {
                    EditProduct.Reserve15 = get_sel00.Code;
                    EditProduct.ReserveName15 = get_sel00.Name;
                }
                if (kubun == "Y16")
                {
                    EditProduct.Reserve16 = get_sel00.Code;
                    EditProduct.ReserveName16 = get_sel00.Name;
                }
                if (kubun == "Y17")
                {
                    EditProduct.Reserve17 = get_sel00.Code;
                    EditProduct.ReserveName17 = get_sel00.Name;
                }
                if (kubun == "Y18")
                {
                    EditProduct.Reserve18 = get_sel00.Code;
                    EditProduct.ReserveName18 = get_sel00.Name;
                }
                if (kubun == "Y19")
                {
                    EditProduct.Reserve19 = get_sel00.Code;
                    EditProduct.ReserveName19 = get_sel00.Name;
                }
                if (kubun == "Y20")
                {
                    EditProduct.Reserve20 = get_sel00.Code;
                    EditProduct.ReserveName20 = get_sel00.Name;
                }
            }
        }
        [RelayCommand]
        public void SelGenderCate((object Result1, object Result2) value)
        {
            var (result1, result2) = value; 
            var get_sel00 = (SelValueModel)result1; 
            if (get_sel00 != null && EditProduct != null)
            { 
                EditProduct.GenderCate = get_sel00.Code;
                EditProduct.GenderCateName = get_sel00.Name;
            }
        }
        #endregion
    }
}
