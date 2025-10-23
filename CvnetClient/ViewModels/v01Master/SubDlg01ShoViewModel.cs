using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using System.Collections.ObjectModel;
using System.Data;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01ShoViewModel : BaseViewModel
    {
        #region Variables
        [ObservableProperty]
        ObservableCollection<MasterShohin>? listProduct;

        [ObservableProperty]
        MasterShohin? editProduct;

        [ObservableProperty]
        MasterShohin? selectedProduct;

        [ObservableProperty]
        string? selProductCd;

        private BizArray col_list;

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
        #endregion

        #region Init Events 
        [RelayCommand]
        void Init(object? init_para)
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

            //Get Column Name List 
            sql_collist = sql_collist.Replace("\r", "").Replace("\n", "").Trim();
            string[] wrk_list = sql_collist.Trim().Split(',');
            for (int i = 0; i < wrk_list.Length; i++)
            {
                wrk_list[i] = wrk_list[i].Trim();
            }
            col_list = (wrk_list.Length > 0) ? new BizArray(wrk_list) : new BizArray();
        }
        #endregion

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

        #endregion
    }
}
