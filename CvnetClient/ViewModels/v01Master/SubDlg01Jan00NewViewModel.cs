using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01Jan00NewViewModel : BaseViewModel
    {
        #region Variables
        [ObservableProperty]
        string? m_Title = "店舗 : JANコード確認";

        [ObservableProperty]
        string? m_Mess2;

        [ObservableProperty]
        Jan00NewOpt m_Jan00NewOpt = new Jan00NewOpt();
        #endregion

        #region Result Variable
        /// <summary>
        /// Current Dialog return value
        /// </summary>
        public BizArray ret_para;
        #endregion

        public void OnInit(string[] init_para)
        {
            /* 初期化処理 */
            OnInitBase(init_para);
        }

        #region Events
        [RelayCommand]
        public void SelProduct(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Jan00NewOpt != null)
            {
                Jan00NewOpt.ProdCD = new BtListHelper(get_sel00.Code, get_sel00.Name); 
            }
        }

        [RelayCommand]
        public void SelColorSize()
        {
            if (Jan00NewOpt == null) return;
            var wrk_para = new BizArray();
            wrk_para[0] = Jan00NewOpt.ProdCD.Code;
            var vm_result = AppData.DlgService.GetSelclsz0(wrk_para.ToArray());
            if (vm_result != null)
            {
                Jan00NewOpt.ColorCD = vm_result.ret_para[0];
                Jan00NewOpt.SizeCD = vm_result.ret_para[1];
            }
        }

        [RelayCommand]
        public void DoConfirmJan()
        {
            if (Jan00NewOpt == null || Jan00NewOpt.ProdCD == null) return;
            var v_para = new BizArray();
            v_para[0] = Jan00NewOpt.ProdCD.Code;
            v_para[1] = Jan00NewOpt.CostFlg.ToString();
            v_para[2] = Jan00NewOpt.ColorCD;
            v_para[3] = Jan00NewOpt.SizeCD;
            string sql_str = "select  GET_JANCODE(B.商品CD,NVL(C.行NO,0) ,B.色CD, B.サイズCD) JANCD,B.商品CD, B.色CD, B.サイズCD "
                            + " from HC$MASTER_SHOHIN_JAN B"
                            + " left outer join HC$master_shohin_genka C on (C.商品CD=B.商品CD)"
                            + " join HC$master_shohin A on (B.商品CD=A.商品CD)"
                            + " where B.商品CD=:1and NVL(C.行NO,0)=:2 and B.色CD=:3 and B.サイズCD=:4";

            if (AppData.ClassCvnet.config.usegenka == 1)
            {
                sql_str = "SELECT "
                            + "decode(d.原価FLG段数,1,CDJAN(substr(a.JAN1,0,d.原価FLG位置-1)||a.原価FLG||substr(a.JAN1,d.原価FLG位置+length(a.原価FLG),(12-length(a.JAN1)-1))),'') JAN1,"
                            + "JAN2,"
                            + "JAN3"
                        + " FROM "
                            + "(SELECT "
                                + "GET_JANCODE(B.商品CD,NVL(C.行NO,0) ,B.色CD, B.サイズCD) JAN1,"
                                + "nvl((SELECT j.JANコード2 FROM HC$MASTER_SHOHIN_JAN j WHERE j.商品CD=B.商品CD AND j.色CD=B.色CD AND j.サイズCD=B.サイズCD) ,'') JAN2,"
                                + "nvl((SELECT j.JANコード3 FROM HC$MASTER_SHOHIN_JAN j WHERE j.商品CD=B.商品CD AND j.色CD=B.色CD AND j.サイズCD=B.サイズCD) ,'') JAN3,"
                                + "NVL(C.行NO,0) 原価FLG"
                            + " FROM "
                                + "HC$MASTER_SHOHIN_JAN B"
                                + " left outer join HC$master_shohin_genka C on (C.商品CD=B.商品CD)"
                                + " join HC$master_shohin A on (B.商品CD=A.商品CD)"
                            + " WHERE "
                                + "B.商品CD=:1 AND "
                                + "NVL(C.行NO,0)=:2 AND "
                                + "B.色CD=:3 AND "
                                + "B.サイズCD=:4"
                            + ") a ,"
                            + "HC$master_hht_kanri d";
            }
            else
            {
                /* 2009.07.07 JAN2・JAN3追加 */
                sql_str = "SELECT "
                            + "GET_JANCODE(:1,:2,:3,:4) JAN1,"
                            + "nvl((SELECT JANコード2 FROM HC$MASTER_SHOHIN_JAN WHERE 商品CD=:1 AND 色CD=:3 AND サイズCD=:4) ,'.') JAN2,"
                            + "nvl((SELECT JANコード3 FROM HC$MASTER_SHOHIN_JAN WHERE 商品CD=:1 AND 色CD=:3 AND サイズCD=:4) ,'.') JAN3"
                        + " FROM "
                            + "dual";
            }
            /* 初期化 */
            Jan00NewOpt.Jancode1 = string.Empty;
            Jan00NewOpt.Jancode2 = string.Empty;
            Jan00NewOpt.Jancode3 = string.Empty;

            var wrk_csv = AppData.Http?.AspxSqlQuery(sql_str, v_para.ToArray());
            if (wrk_csv.Rows.Count > 0)
            {
                if (wrk_csv.Columns[0].ColumnName == "ERROR_CODE")
                {
                    Mess2 = "指定した品番が間違っているかマスターに登録されてません";
                }
                else
                {
                    Jan00NewOpt.Jancode1 = wrk_csv.Rows[0][0].ToString();
                    /* 2009.07.07 JAN2・JAN3追加 */
                    Jan00NewOpt.Jancode2 = wrk_csv.Rows[0][1].ToString();
                    Jan00NewOpt.Jancode3 = wrk_csv.Rows[0][2].ToString();
                    Mess2 = "JANCODEを取得しました";
                }
            }
        }

        /* 設定 */
        [RelayCommand]
        public void DoExecute()
        {
            ret_para = new BizArray();
            ret_para[0] = Jan00NewOpt.Jancode1;
            ret_para[1] = Jan00NewOpt.Jancode2;
            ret_para[2] = Jan00NewOpt.Jancode3;
            ClientLib.ExitDialogResult(this, true);
        }

        [RelayCommand]
        public void DoExit()
        {
            ClientLib.ExitDialogResult(this, true);
        }

        #endregion
    }

    public partial class Jan00NewOpt : ObservableObject
    {
        /// <summary>
        /// 商品CD
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_ProdCD;

        /// <summary>
        /// 原価FLG
        /// </summary>
        [ObservableProperty]
        int? m_CostFlg;

        [ObservableProperty]
        int? m_CostFlgVisible;

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
        /// JANコード
        /// </summary>
        [ObservableProperty]
        string? m_Jancode1;

        /// <summary>
        /// JANコード2
        /// </summary>
        [ObservableProperty]
        string? m_Jancode2;

        /// <summary>
        /// JANコード3
        /// </summary>
        [ObservableProperty]
        string? m_Jancode3;

        public Jan00NewOpt()
        {
            CostFlg = 0;
            CostFlgVisible = (AppData.ClassCvnet.config.usegenka == 1) ? 1 : 0;
        }
    }
}
