using CvnetClient.Class;
using CvnetClient.Models;
using System.Data; 

namespace CvnetBaseCore
{
    public class cvnet
    {
        public int UserFlg = 0;  /* ユーザ別フラグ0-52(0=Package,1=Abl,2=Shirt,..) .*/
        public int UserFlg2 = 0; /* ユーザ別フラグ2 0=本番用, 1以上=仮 .*/
        public int MaxCntDisp = 40; /* オンライン系一覧件数 .*/
        public int MaxCntPrint = 120; /* 帳票系一覧件数 .*/
        public int Preview = 1; /* 1:ﾌﾟﾚﾋﾞｭｰあり,0:ﾌﾟﾚﾋﾞｭｰなし .*/
        public string FormTitleFontKind = "ＭＳ Ｐ明朝";
        public int FormTitleFontFace = 3;
        public string MenuXml = "menu_user01.xml"; /* メニュー用XMLファイル */
        public string SdMenuXml = "menu_sd_user00.xml";	/* メニュー用XMLファイル(mobile版) */
        public int ReadmeFlg = 1; /* Readme参照フラグ2 0=表示, 1=非表示 .*/
        public int MaxZaikoNissu = 100; /* 最大在庫日数 .*/

        public int PassLength = 8; /* パスワード文字数下限値 */
        public int MaxErrCnt = 4; /* ログイン制御回数 */

        /* 初期設定フラグレコード */
        public Config config = new Config();
        public int ImpDateDiff = 7; /* 範囲指定時に補正する日付の日数 .*/
        public int LoginDialogFlag = 1;
        public int VerClear = 0; /* 起動時にキャッシュクリアしたかどうか .*/

        public int ComboListFLg = 0; /* BtListのOnTouchでの一覧表示FLG 1:ダイアログ表示 */
        public int ComboListFlg2 = 0; /* ComboKey押下時はコンボボックス内で一覧表示 */
        public int ComboListFlg3 = 0; /* 伝票内での一覧表示 */
        public string BizUrl = string.Empty;

        public Array HelfDef;
        public Dictionary<string, string> MstDialog; /* マスタ取得用専用ダイアログ保存 */
        public int TanaFlg = 0; /* 棚卸の基準　0:月次、1:棚卸日指定 */
        public int PosFlg = 0; /* POSの実行　0:POS連携なし、1:TEC 、2:三谷*/
        public int YosanFlg = 0; /* 0 店別ブランド別の予算、1 日別の予算*/
        public int YosanFlg2 = 0; /* 0 予算表 ブランド項目なし 1 予算表ブランド項目あり*/
        public int JdaihenFlg = 0; /* 0 色サイズ展開無し、1 色サイズ展開あり */

        public string[] OrgMenuSub; 	/* オリジナルサブメニュー定義 .*/
        /* CRSを起動する場合には必ずこのエントリに登録し、かつユーザ毎の起動設定をしなければならない */
        public OrgMenuDef orgMenuDef = new OrgMenuDef();

        public ComboItem00 comboItem00 = new ComboItem00();

        /// <summary>
        /// Represent Biz menu_next inital setup
        /// </summary>
        public cvnet()
        {
            ComboListFLg = 1;
            ComboListFlg2 = 0;
            ComboListFlg3 = 1;
            MstDialog = new Dictionary<string, string> {
                { "商品", "SelShoView" },
                { "得意先", "SelTokView@店種区分 between 1 and 3" },
                { "Ac得意先", "SelTokView@店種区分 between 1 and 3 and 出荷停止FLG=0" },
                { "全得意先", "SelTokView@出荷停止FLG=0" },
                { "店舗", "SelTokView@((店種区分=1 AND 在庫管理FLG=1) OR 店種区分 between 3 and 8)" },
                { "Ac店舗", "SelTokView@((店種区分=1 AND 在庫管理FLG=1) OR 店種区分 between 3 and 8) and 出荷停止FLG=0" },
                { "勤怠店舗", "SelTokView@店種区分 IN (0,3,6,9)" },
                { "倉庫", "SelTokView@(店種区分=0 OR 倉庫区分=9)" },
                { "Ac倉庫", "SelTokView@(店種区分=0 OR 倉庫区分=9) and 出荷停止FLG=0" },
                { "倉庫2", "SelTokView@店種区分=0 and 倉庫区分 in (2,3,4,6,7,8)@名称CD01=:1" },
                { "Ac倉庫2", "SelTokView@店種区分=0 and 倉庫区分 in (2,3,4,6,7,8)@名称CD01=:1 and 出荷停止FLG=0" },
                { "仕入先", "SelSirView" },
                { "請求", "SelTokView@(得意先CD=請求先CD or 請求先CD='.')@出荷停止FLG=0 and ( (締日=:1 and 入金予定日!='0') or (締日2=:2 and 入金予定日2!='0') or (締日3=:3 and 入金予定日3!='0') )" },
                { "Ac請求", "SelTokView@(得意先CD=請求先CD or 請求先CD='.')@締日=:1 and 出荷停止FLG=0" },
                { "請求先登録", "SelTokView@(得意先CD=請求先CD or 請求先CD='.')@得意先CD=:1" },
                { "支払", "SelTokView@(仕入先CD=支払先CD or 支払先CD='.')@締日=:1" },
                { "支払先登録", "SelSirView@(仕入先CD=支払先CD or 支払先CD='.')@仕入先CD=:1" },
                { "Ac移動倉庫", "SelTokView@店種区分<9  and 出荷停止FLG=0" },
                { "移動倉庫", "SelTokView@店種区分<9" },
                { "Ac取置倉庫", "SelTokView@在庫管理FLG=1 and 店種区分=0 and 倉庫区分=7 and 出荷停止FLG=0" },
                { "取置倉庫", "SelTokView@在庫管理FLG=1 and 店種区分=0 and 倉庫区分=7" },
                { "担当", "SelUsrView@就業FLG='0'" },
                { "営業担当", "SelUsrView@営業FLG=1 and 就業FLG='0'" },
                { "顧客", "SelKokyakuView" },
                { "ポイント", "SelPointView" },
                { "請求単位得意先", "SelTokView@店種区分 between 1 and 3 and 消費税計算方法=0" },
                { "支払単位仕入先", "SelSirView@消費税計算方法=0" },
            };

            /***** 汎用検索設定 *****/

            /* 商品分類検索BOX追加項目 */
            AppData.ClassEtc.SearchShohinCols = new string[7];
            AppData.ClassEtc.SearchShohinCols[0] = "上代";
            AppData.ClassEtc.SearchShohinCols[1] = "原価";
            AppData.ClassEtc.SearchShohinCols[2] = "JANコード1";
            AppData.ClassEtc.SearchShohinCols[3] = "店頭投入日";
            AppData.ClassEtc.SearchShohinCols[4] = "リピート日";
            AppData.ClassEtc.SearchShohinCols[5] = "メーカー品番";
            AppData.ClassEtc.SearchShohinCols[6] = "商品CD";

            /* 得意先分類検索BOX追加項目 */
            AppData.ClassEtc.SearchTokuiCols = new string[7];
            AppData.ClassEtc.SearchTokuiCols[0] = "店種区分";
            AppData.ClassEtc.SearchTokuiCols[1] = "営業担当CD";
            AppData.ClassEtc.SearchTokuiCols[2] = "請求先CD";
            AppData.ClassEtc.SearchTokuiCols[3] = "締日";
            AppData.ClassEtc.SearchTokuiCols[4] = "入金予定月";
            AppData.ClassEtc.SearchTokuiCols[5] = "入金予定日";
            AppData.ClassEtc.SearchTokuiCols[6] = "出荷停止FLG";

            /* 仕入先分類検索BOX追加項目 */
            AppData.ClassEtc.SearchSiireCols = new string[6];
            AppData.ClassEtc.SearchSiireCols[0] = "発注停止FLG";
            AppData.ClassEtc.SearchSiireCols[1] = "仕入区分";
            AppData.ClassEtc.SearchSiireCols[2] = "支払先CD";
            AppData.ClassEtc.SearchSiireCols[3] = "締日";
            AppData.ClassEtc.SearchSiireCols[4] = "支払予定月";
            AppData.ClassEtc.SearchSiireCols[5] = "支払予定日";

            AppData.ClassEtc.ListFlexPara = new List<CsvItem>()
            {
                new CsvItem { col01 = "大分類" },
                new CsvItem { col01 = "中分類" },
                new CsvItem { col01 = "仕入先" },
                new CsvItem { col01 = "シーズン" },
                new CsvItem { col01 = "JANコード1" },
                new CsvItem { col01 = "ｱｲﾃﾑ" },
                new CsvItem { col01 = "店頭投入日" },
                new CsvItem { col01 = "リピート日" },
                new CsvItem { col01 = "商品CD" },
            };

            AppData.ClassEtc.Bunrui_List0 = new List<CsvItem>()
            {
                new CsvItem { col01 = "大分類" },
                new CsvItem { col01 = "中分類" },
                new CsvItem { col01 = "仕入先" },
                new CsvItem { col01 = "シーズン" },
                new CsvItem { col01 = "JANコード1" },
                new CsvItem { col01 = "ｱｲﾃﾑ" },
                new CsvItem { col01 = "店頭投入日" },
                new CsvItem { col01 = "リピート日" },
                new CsvItem { col01 = "商品CD" },
            };

            AppData.ClassEtc.SearchShohinStr = new string[1];
            AppData.ClassEtc.SearchShohinStr[0] = "商品CD";

            MstDialog.Add("siire", "SelSirView");
            MstDialog.Add("tokui", "SelTokView");
            MstDialog.Add("社員", "SelUsrView");
        }

        /// <summary>
        /// ■関数 GetSqlDisp = 最大取得件数を制限したSQL文を返す(表示用)
        /// </summary>
        /// <param name="p_querystr">引数1:I	String = 元のSQL文</param>
        /// <param name="p_line">引数2:I	Number = 最大行</param>
        /// <returns>戻値 String = 件数制限されたSQL文</returns>
        public string GetSqlDisp(string p_querystr, int p_line = 0)
        {
            string ret_sqlstrwrk = "select * from (" + p_querystr + ") where rownum<=" + ((p_line == 0) ? AppData.ClassCvnet.MaxCntDisp : p_line);
            return ret_sqlstrwrk;
        }  

        /// <summary>
        /// ■関数 AspxSqlQueryMst = マスター関係の問い合わせを行う
        /// </summary>
        /// <param name="p_kubun">引数1:I String = マスター区分名</param>
        /// <param name="v_para">引数2:I Array = ﾊﾟﾗﾒｰﾀ配列 (開始CDなど)</param>
        /// <param name="v_para2">引数4:I Number = ダイアログ表示用</param>
        /// <param name="v_flg">戻値 CSVデータ(タイトル名設定済)</param>
        public DataTable AspxSqlQueryMst(string p_kubun, string[] v_para = null, string[] v_para2 = null, int v_flg = 0)
        {
            string sql_query = string.Empty;
            DataTable ret_csv = new DataTable();
            string v_retu = string.Empty;
            /* 協和バッグ */
            int v_next = 0;

            if (AppData.ClassCvnet.UserFlg == 56)
            {
                if (p_kubun == "ブランド")
                {
                    if (v_flg == 1)
                        v_retu = "名称CD ブランドCD,名称 ブランド名";
                    else
                        v_retu = "名称CD||' '||名称 一覧";
                    sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                    sql_query += " where 名称区分='BRD' ";
                    if (v_para2 != null)
                    {
                        sql_query += " and カナ ='" + v_para2[0].ToString() + "'";
                    }
                    sql_query += " order by 名称区分,名称CD";
                    ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
                }
                else v_next = 1;
                if (v_next == 0)
                {
                    if (ret_csv?.Rows.Count > 0)
                    {
                    }
                }
                return ret_csv;
            }

            string houjin_str = GetQueryStrHoujin();
            /* 既存 */
            if (p_kubun == "倉庫")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名 倉庫DATA";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 得意先CD>=:1 and (店種区分=0 OR 倉庫区分=9) " + houjin_str + " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "Ac倉庫")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名 倉庫DATA";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 出荷停止FLG = 0 and 得意先CD>=:1 and (店種区分=0 OR 倉庫区分=9) " + houjin_str + " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "倉庫2")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名 倉庫DATA";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 得意先CD>=:1 and 店種区分=0 and 倉庫区分 in (2,3,4,6,7,8)";
                if (v_para2 != null)
                {
                    sql_query += " and 名称CD01='" + v_para2[0] + "'";
                }
                sql_query += houjin_str;
                sql_query += " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "Ac倉庫2")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名 倉庫DATA";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 出荷停止FLG = 0 and 得意先CD>=:1 and 店種区分=0 and 倉庫区分 in (2,3,4,6,7,8)";
                if (v_para2 != null)
                {
                    sql_query += " and 名称CD01='" + v_para2[0] + "'";
                }
                sql_query += houjin_str;
                sql_query += " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "移動倉庫")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名 倉庫DATA";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 得意先CD>=:1 and (店種区分 = 0 or (店種区分 between 3 and 8) or (店種区分=1 and 在庫管理FLG=1)) " + houjin_str + " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "Ac移動倉庫")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名 倉庫DATA";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 出荷停止FLG= 0 and 得意先CD>=:1 and (店種区分 = 0 or (店種区分 between 3 and 8) or (店種区分=1 and 在庫管理FLG=1)) " + houjin_str + " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "移動倉庫S")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名 倉庫DATA";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 得意先CD>=:1 and (店種区分 = 0 or (店種区分 between 3 and 8) or (店種区分=1 and 在庫管理FLG=1)) order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "取置倉庫")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名 倉庫DATA";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where  得意先CD>=:1 and 在庫管理FLG=1 and 店種区分=0 and 倉庫区分=7 " + houjin_str + " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "Ac取置倉庫")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名 倉庫DATA";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where  出荷停止FLG = 0 and 得意先CD>=:1 and 在庫管理FLG=1 and 店種区分=0 and 倉庫区分=7 " + houjin_str + " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "配分店舗")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 得意先CD>=:1 and 店種区分 in (0,3,6) and 出荷停止FLG=0 order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "受注配分店舗")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 得意先CD>=:1 and 店種区分 in (0,1,3,6) and 出荷停止FLG=0 order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "担当")
            {
                if (v_flg == 1)
                {
                    v_retu = "社員CD,名前";
                }
                else
                {
                    v_retu = "社員CD||' '||名前 社員DATA";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_SHAIN ";
                if (v_para != null)
                {
                    sql_query += " where 社員CD>=:1 and 就業FLG='0' order by 社員CD";
                }
                else { 
                    sql_query += " where 就業FLG='0' order by 社員CD";
                }

                    
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "営業担当")
            {
                if (v_flg == 1)
                {
                    v_retu = "社員CD,名前";
                }
                else
                {
                    v_retu = "社員CD||' '||名前 社員DATA";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_SHAIN ";
                sql_query += " where 社員CD>=:1 and 営業FLG=1 and 就業FLG='0' order by 社員CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "請求")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 得意先CD>=:1  and (得意先CD=請求先CD or 請求先CD='.')";
                if (v_para2 != null)
                {
                    /* 2017.04.14 締日＋入金予定日が0ではない時に変更 */
                    sql_query += " and ( (締日=" + v_para2[0] + " and 入金予定日!='0')"
                    /* 2015.06.12 #20551対応修正 */
                    + " or (締日2=" + v_para2[0] + " and 入金予定日2!='0')" + " or (締日3=" + v_para2[0] + " and 入金予定日3!='0') )";
                }
                sql_query += houjin_str;
                sql_query += " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "Ac請求")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 出荷停止FLG = 0 and 得意先CD>=:1  and (得意先CD=請求先CD or 請求先CD='.')";
                if (v_para2 != null)
                {
                    sql_query += " and (締日=" + v_para2[0]
                    + " or 締日2=" + v_para2[0] + " or 締日3=" + v_para2[0] + ")";
                }
                sql_query += houjin_str;
                sql_query += " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "支払")
            {
                if (v_flg == 1)
                {
                    v_retu = "仕入先CD,仕入先名";
                }
                else
                {
                    v_retu = "仕入先CD||' '||仕入先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_SIIRE ";
                sql_query += " where 仕入先CD>=:1  and (仕入先CD=支払先CD or 支払先CD='.')";
                if (v_para2 != null)
                {
                    sql_query += " and ( (締日=" + v_para2[0] + " and 支払予定日!='0')"
                    + " or (締日2=" + v_para2[0] + " and 支払予定日2!='0')" + " or (締日3=" + v_para2[0] + " and 支払予定日3!='0') )";
                }
                sql_query += houjin_str;
                sql_query += " order by 仕入先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "請求先登録")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 得意先CD>=:1  and (得意先CD=請求先CD or 請求先CD='.'";
                if (v_para2 != null)
                {
                    sql_query += " or 得意先CD='" + v_para2[0] + "')";
                }
                else sql_query += ")";
                sql_query += houjin_str;
                sql_query += " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "子得意先")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名  一覧";
                }
                sql_query = "select " + v_retu + " from (select 得意先CD,得意先名,decode(請求先CD,'.',得意先CD,請求先CD) 請求先CD from HC$MASTER_TOKUI";
                sql_query += " where 得意先CD>=:1)";
                if (v_para2 != null)
                {
                    sql_query += " where 請求先CD='" + v_para2[0] + "'";
                }
                sql_query += houjin_str;
                sql_query += " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "支払先登録")
            {
                if (v_flg == 1)
                {
                    v_retu = "仕入先CD,仕入先名";
                }
                else
                {
                    v_retu = "仕入先CD||' '||仕入先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_SIIRE ";
                sql_query += " where 仕入先CD>=:1  and (仕入先CD=支払先CD or 支払先CD='.'";
                if (v_para2 != null)
                {
                    sql_query += " or 仕入先CD='" + v_para2[0] + "')";
                }
                else sql_query += ")";
                sql_query += houjin_str;
                sql_query += " order by 仕入先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "子仕入先")
            {
                if (v_flg == 1)
                {
                    v_retu = "仕入先CD,仕入先名";
                }
                else
                {
                    v_retu = "仕入先CD||' '||仕入先名  一覧";
                }
                sql_query = "select " + v_retu + " from (select 仕入先CD,仕入先名,decode(支払先CD,'.',仕入先CD,支払先CD) 支払先CD from HC$MASTER_SIIRE";
                sql_query += " where 仕入先CD>=:1)";
                if (v_para2 != null)
                {
                    sql_query += " where 支払先CD='" + v_para2[0] + "'";
                }
                sql_query += houjin_str;
                sql_query += " order by 仕入先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "請求締")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 得意先CD>=:1  and (得意先CD=請求先CD or 請求先CD='.')"
                    + " and 締日=:2"
                    + houjin_str
                    + " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "支払締")
            {
                if (v_flg == 1)
                {
                    v_retu = "仕入先CD,仕入先名";
                }
                else
                {
                    v_retu = "仕入先CD||' '||仕入先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_SIIRE ";
                sql_query += " where 仕入先CD>=:1  and (仕入先CD=支払先CD and 締日=:2 " + houjin_str + " order by 仕入先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "店舗")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 得意先CD>=:1 and ((店種区分=1 AND 在庫管理FLG=1) OR 店種区分 between 3 and 8) " + houjin_str + " order by 得意先CD";

                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "Ac店舗")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 出荷停止FLG = 0 and 得意先CD>=:1 and ((店種区分=1 AND 在庫管理FLG=1) OR 店種区分 between 3 and 8) " + houjin_str + " order by 得意先CD";

                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "店舗3")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 得意先CD>=:1 and 店種区分=3 " + houjin_str + " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "丸井店舗")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 得意先CD>=:1 and 名称CD02 = '11' " + houjin_str + " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "仕入先")
            {
                if (v_flg == 1)
                {
                    v_retu = "仕入先CD,仕入先名";
                }
                else
                {
                    v_retu = "仕入先CD||' '||仕入先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_SIIRE ";
                sql_query += " where 仕入先CD>=:1 " + houjin_str + " order by 仕入先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "得意先")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 得意先CD>=:1 and 店種区分 between 1 and 3 " + houjin_str + " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "Ac得意先")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 出荷停止FLG = 0 and 得意先CD>=:1 and 店種区分 between 1 and 3 " + houjin_str + " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "siire")
            {
                if (v_flg == 1)
                {
                    v_retu = "仕入先CD,仕入先名";
                }
                else
                {
                    v_retu = "仕入先CD||' '||仕入先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_SIIRE ";
                sql_query += " where 仕入先CD>=:1 " + houjin_str + " order by 仕入先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "tokui")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 得意先CD>=:1 " + houjin_str + " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "全得意先")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 得意先CD>=:1 and 店種区分 >0 " + houjin_str + " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "Ac全得意先")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 出荷停止FLG = 0　and 得意先CD>=:1 and 店種区分 >0 " + houjin_str + " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "部門")
            {   /* 全部取得 */
                if (v_flg == 1)
                {
                    v_retu = "名称CD 部門CD,名称 部門名";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='BMN' order by 名称区分,名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, null);
            }
            else if (p_kubun == "仕入品目")
            {   /* 全部取得 */
                if (v_flg == 1)
                {
                    v_retu = "名称CD 仕入品目CD,名称 仕入品目名";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='S04' order by 名称区分,名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, null);
            }
            else if (p_kubun == "品質")
            {   /* 全部取得 */
                if (v_flg == 1)
                {
                    v_retu = "名称CD 品質CD,名称 品質名";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='HIN' order by 名称区分,名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, null);
            }
            else if (p_kubun == "展示会")
            {   /* 全部取得 */
                if (v_flg == 1)
                {
                    v_retu = "名称CD 展示会CD,名称 展示会名";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='TNJ' order by 名称区分,名称CD"; /*  and 名称CD>=:1  */
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "ブランド")
            {  /* 全部取得 */
                if (v_flg == 1)
                {
                    v_retu = "名称CD ブランドCD,名称 ブランド名";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='BRD' order by 名称区分,名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "アイテム")
            {  /* 全部取得 */
                if (v_flg == 1)
                {
                    v_retu = "名称CD アイテムCD,名称 アイテム名";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='ITM' order by 名称区分,名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "シーズン")
            {   /* 全部取得 */
                if (v_flg == 1)
                {
                    v_retu = "名称CD \"シーズンCD\",名称 \"シーズン名\"";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='SZN' order by 名称区分,名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "シーズン2")
            {   /* 全部取得 ランクがはいっていないもののみ*/
                if (v_flg == 1)
                {
                    v_retu = "名称CD \"シーズンCD\",名称 \"シーズン名\"";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='SZN' and ランク='.' order by 名称区分,名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "素材")
            {   /* 全部取得 */
                if (v_flg == 1)
                {
                    v_retu = "名称CD 素材CD,名称 素材名";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='SZI' order by 名称区分,名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "デザイナー")
            {   /* 全部取得 */
                if (v_flg == 1)
                {
                    v_retu = "名称CD \"デザイナーCD\",名称 \"デザイナー名\"";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='DZN' order by 名称区分,名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "メーカー")
            {   /* 全部取得 */
                if (v_flg == 1)
                {
                    v_retu = "名称CD \"メーカーCD\",名称 \"メーカー名\"";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='MKR' order by 名称区分,名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "原産国")
            {   /* 全部取得 */
                if (v_flg == 1)
                {
                    v_retu = "名称CD 原産国CD,名称 原産国名";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='GEN' order by 名称区分,名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "商品")
            {
                if (v_flg == 1)
                {
                    v_retu = "商品CD,商品名";
                }
                else
                {
                    v_retu = "商品CD||' '||商品名 一覧";
                }
                sql_query = "select " + v_retu + " from HC$Master_SHOHIN ";
                sql_query += " where 商品CD>=:1";
                /* if (v_para2!=null){ */
                if (AppData.ClassSatoo.LoginKubun == 1)
                {
                    sql_query += " and メーカーCD='" + AppData.ClassSatoo.SHAIN_CD + "'";
                }
                /* ｴｽﾗｸﾞｼﾞｭｰﾙのみ */
                if (AppData.ClassCvnet.UserFlg == 56) sql_query += " and 承認FLG=1";

                sql_query += " order by 商品CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "ｾｯﾄ商品")
            {
                if (v_flg == 1)
                {
                    v_retu = "商品CD,商品名";
                }
                else
                {
                    v_retu = "商品CD||' '||商品名 一覧";
                }
                sql_query = "select " + v_retu + " from HC$Master_SHOHIN ";
                sql_query += " where 商品CD>=:1";
                if (AppData.ClassSatoo.LoginKubun == 1)
                {
                    sql_query += " and メーカーCD='" + AppData.ClassSatoo.SHAIN_CD + "'";
                }
                sql_query += " and 名称CD10='1' ";

                sql_query += " order by 商品CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "EC商品")
            {
                if (v_flg == 1)
                {
                    v_retu = "商品CD,EC商品名";
                }
                else
                {
                    v_retu = "商品CD||' '||EC商品名 一覧";
                }
                sql_query = "select " + v_retu + " from HC$Master_SHOHIN_ECCUBE";
                sql_query += " where 商品CD>=:1";
                if (v_para2 != null)
                {
                    sql_query += " and 商品CD<>'" + v_para2[0] + "'";
                }
                sql_query += " order by 商品CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "生地")
            {
                if (v_flg == 1)
                {
                    v_retu = "商品CD,商品名";
                }
                else
                {
                    v_retu = "商品CD||' '||商品名 一覧";
                }
                sql_query = "select " + v_retu + " from HC$Master_SHKIJI ";
                sql_query += " where 商品CD>=:1 order by 商品CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "カラー")
            {   /* 全部取得(ユーザ依存) */
                if (v_flg == 1)
                {
                    v_retu = "名称CD 色CD,名称 色名";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='COL'  order by 名称区分,名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "サイズ")
            {   /* 全部取得(ユーザ依存) */
                if (v_flg == 1)
                {
                    v_retu = "名称CD サイズCD,名称 サイズ名";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='SIZ' order by 名称区分,名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "サイズ3")
            {   /* 全部取得(ユーザ依存) */
                if (v_flg == 1)
                {
                    v_retu = "名称CD サイズCD,名称 サイズ名";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='" + v_para2[0] + "' order by 名称区分,名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "名称")
            {   /* 全部取得(ユーザ依存) */
                /* v_para2は文字列とする */
                if (v_flg == 1)
                {
                    v_retu = "名称CD,名称,名称CD 予備1,名称 予備2,略称";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 一覧,名称CD,名称,略称";
                }
                sql_query = "select " + v_retu + "  from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='" + v_para2[0] + "' order by 名称区分,名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query);
            }
            else if (p_kubun == "シフト1")
            {
                if (v_flg == 1)
                {
                    v_retu = "シフトCD,rpad(名称,20)||' '||to_char(to_date(開始時刻,'HH24MISS'),'HH24:MI:SS')||' - '||to_char(to_date(終了時刻,'HH24MISS'),'HH24:MI:SS') 名称";
                }
                else
                {
                    v_retu = "シフトCD||' '||rpad(名称,20)||' '||to_char(to_date(開始時刻,'HH24MISS'),'HH24:MI:SS')||' - '||to_char(to_date(終了時刻,'HH24MISS'),'HH24:MI:SS') 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_SHIFT ";
                sql_query += " where シフトCD>=:1 ";
                if (v_para2.Length > 0)
                {
                    sql_query += " and (店舗CD = :2 or 店舗CD = '.') ";
                    v_para[v_para.Length] = v_para2[0];
                }
                sql_query += "order by シフトCD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "シフト2")
            {
                if (v_flg == 1)
                {
                    v_retu = "シフトCD,名称";
                }
                else
                {
                    v_retu = "シフトCD||' '||名称 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_SHIFT ";
                sql_query += " where シフトCD>=:1 ";
                if (v_para2.Length > 0)
                {
                    sql_query += " and (店舗CD = :2 or 店舗CD = '.') ";
                    v_para[v_para.Length] = v_para2[0];
                }
                sql_query += "order by シフトCD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "シフト3")
            {
                if (v_flg == 1)
                {
                    v_retu = "substr(シフトCD,-2) シフトCD,rpad(名称,20)||' '||to_char(to_date(開始時刻,'HH24MISS'),'HH24:MI:SS')||' - '||to_char(to_date(終了時刻,'HH24MISS'),'HH24:MI:SS') 名称";
                }
                else
                {
                    v_retu = "substr(シフトCD,-2)||' '||rpad(名称,20)||' '||to_char(to_date(開始時刻,'HH24MISS'),'HH24:MI:SS')||' - '||to_char(to_date(終了時刻,'HH24MISS'),'HH24:MI:SS') 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_SHIFT ";
                if (v_para2.Length > 0)
                {
                    sql_query += " where (店舗CD = '" + v_para2[0] + "' or 店舗CD = '.') ";
                    v_para[v_para.Length] = v_para2[0];
                }
                sql_query += "order by シフトCD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, null);
            }
            else if (p_kubun == "役職")
            {   /* 全部取得(ユーザ依存) */
                if (v_flg == 1)
                {
                    v_retu = "名称CD 役職CD,名称 役職名";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分 = 'YAK' order by 名称区分,名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "部課")
            {    /* 全部取得 */
                if (v_flg == 1)
                {
                    v_retu = "名称CD 部課CD,名称 部課名";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='BKA' order by 名称区分,名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, null);
            }
            else if (p_kubun == "勤怠店舗")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名 倉庫DATA";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 得意先CD>=:1 and 店種区分 IN (0,3,6,9) order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "為替")
            {   /* 全部取得(ユーザ依存) */
                if (v_flg == 1)
                {
                    v_retu = "名称CD 為替区分,名称 為替";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='RAT' order by 名称区分,名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "カラー2")
            {
                if (v_flg == 1)
                {
                    v_retu = "色CD,GET_COLORNAME(色CD) 色名";
                }
                else
                {
                    v_retu = "色CD||' '||GET_COLORNAME(色CD) 一覧";
                }
                if (v_para2 != null)
                {
                    v_para[0] = string.Join(",", v_para2);
                }
                sql_query = "select " + v_retu + " from (select distinct 商品CD,色CD from HC$MASTER_SHOHIN_JAN ";
                sql_query += " where 商品CD=:1) order by 色CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "サイズ2")
            {
                if (v_flg == 1)
                {
                    v_retu = "サイズCD,GET_SIZENAME(商品CD,サイズCD) サイズ名";
                }
                else
                {
                    v_retu = "サイズCD||' '||GET_SIZENAME(商品CD,サイズCD) 一覧";
                }
                if (v_para2 != null)
                {
                    v_para[0] = string.Join(",", v_para2);
                }
                sql_query = "select " + v_retu + " from (select distinct 商品CD,サイズCD from HC$MASTER_SHOHIN_JAN ";
                sql_query += " where 商品CD=:1) order by サイズCD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "天気")
            {
                sql_query = "select 名称CD||' '||名称 一覧 from HC$MASTER_MEISHO ";
                sql_query += " where 名称CD>=:1 and 名称区分 = 'TNK' order by 名称CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "アイテム2")
            {
                sql_query = "select 名称CD||' '||名称 from hc$master_meisho";
                sql_query += " where 名称区分 = 'ITM' and 名称CD in";
                sql_query += "(select distinct ランク from hc$master_meisho where 名称区分 = 'ITM' and ランク >= :1 ) order by ランク";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "グループ")
            {
                if (v_flg == 1)
                {
                    v_retu = "社員CD,名前";
                }
                else
                {
                    v_retu = "社員CD||' '||名前 社員DATA";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_SHAIN ";
                sql_query += " where 社員CD>=:1 and 出力FLG>=90 order by 社員CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "顧客")
            {
                if (v_flg == 1)
                {
                    v_retu = "顧客CD,顧客名";
                }
                else
                {
                    v_retu = "顧客CD||' '||顧客名 顧客DATA";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_KOKYAKU ";
                sql_query += " where 顧客CD>=:1 and 顧客区分 < 9 order by 顧客CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "支払区分")
            {   /* 全部取得 */
                if (v_flg == 1)
                {
                    v_retu = "名称CD 支払区分,名称 支払区分名";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='SHK' order by 名称区分,名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "配送区分")
            {   /* 全部取得 */
                if (v_flg == 1)
                {
                    v_retu = "名称CD 配送区分,名称 配送区分名";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='HSK' order by 名称区分,名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "得意先MST")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 得意先CD>=:1 " + houjin_str + " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "仕入先MST")
            {
                if (v_flg == 1)
                {
                    v_retu = "仕入先CD,仕入先名";
                }
                else
                {
                    v_retu = "仕入先CD||' '||仕入先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_SIIRE ";
                sql_query += " where 仕入先CD>=:1 " + houjin_str + " order by 仕入先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "店舗ブランド")
            {   /* 全部取得 */
                if (v_flg == 1)
                {
                    v_retu = "名称CD ブランドCD,名称 ブランド名";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 店舗ブランド";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='C01' order by 名称区分,名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "仕入先ブランド")
            {   /* 全部取得 */
                if (v_flg == 1)
                {
                    v_retu = "名称CD ブランドCD,名称 ブランド名";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 仕入先ブランド";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='D03' order by 名称区分,名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "取扱ブランド")
            {
                if (v_flg == 1)
                {
                    v_retu = "一意CD02 ブランドCD,nvl((select 名称 from hc$master_meisho where 名称区分='BRD' and 名称CD=一意CD02),'') ブランド名";
                }
                else
                {
                    v_retu = "一意CD02||' '||nvl((select 名称 from hc$master_meisho where 名称区分='BRD' and 名称CD=一意CD02),'') 取扱ブランド";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_CONVERT ";
                sql_query += " where 区分='HBN' and 一意CD01 = :1 order by 一意CD02";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "買取仕入先")
            {
                if (v_flg == 1)
                {
                    v_retu = "仕入先CD,仕入先名";
                }
                else
                {
                    v_retu = "仕入先CD||' '||仕入先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_SIIRE ";
                sql_query += " where 仕入先CD>=:1 and 仕入区分=1 " + houjin_str + " order by 仕入先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "委託仕入先")
            {
                if (v_flg == 1)
                {
                    v_retu = "仕入先CD,仕入先名";
                }
                else
                {
                    v_retu = "仕入先CD||' '||仕入先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_SIIRE ";
                sql_query += " where 仕入先CD>=:1 and 仕入区分=2 " + houjin_str + " order by 仕入先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "消化仕入先")
            {
                if (v_flg == 1)
                {
                    v_retu = "仕入先CD,仕入先名";
                }
                else
                {
                    v_retu = "仕入先CD||' '||仕入先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_SIIRE ";
                sql_query += " where 仕入先CD>=:1 and 仕入区分=3 " + houjin_str + " order by 仕入先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "ブランド店舗")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 得意先CD>=:1 and ((店種区分=1 AND 在庫管理FLG=1) OR 店種区分 between 3 and 8)";
                if (v_para2 != null)
                {
                    sql_query += " and 名称CD01=" + v_para2[0];
                }
                sql_query += houjin_str;
                sql_query += " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "予備")
            {   /* 全部取得(ユーザ依存) */
                /* v_para2は文字列とする */
                if (v_flg == 1)
                {
                    v_retu = "名称CD,名称,名称CD 予備1,名称 予備2,略称";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 一覧,名称CD,名称,略称";
                }
                sql_query = "select " + v_retu + "  from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='" + v_para2[0] + "' order by 名称区分,名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query);
            }
            else if (p_kubun == "店・卸営業")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名 倉庫DATA";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 得意先CD>=:1 and 店種区分 in (3,6,9) " + houjin_str + " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "年代")
            {
                if (v_flg == 1)
                {
                    v_retu = "名称CD 年代CD,名称 年代名";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 年代";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='F01' order by 名称区分,名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "卸先")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名 倉庫DATA";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 得意先CD>=:1 and 店種区分=1 and 出荷停止FLG<>1 " + houjin_str + " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "セール01")
            {
                if (v_flg == 1)
                {
                    v_retu = "名称CD 年代CD,名称 SELL名";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 SELL";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='S01' order by 名称区分,名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "銀行")
            {
                if (v_flg == 1)
                {
                    v_retu = "名称CD 銀行CD,名称 銀行名";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='GN1'";
                sql_query += " order by 名称区分,名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "銀行支店")
            {
                if (v_flg == 1)
                {
                    v_retu = "名称CD 銀行支店CD,名称 銀行支店名";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='GN2'";
                if (v_para2 != null)
                {
                    sql_query += " and ランク LIKE '" + v_para2[0] + "%'";
                }
                sql_query += " order by 名称区分,名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "全サイズ")
            {
                if (v_flg == 1)
                {
                    v_retu = "名称CD サイズCD,名称 サイズ名";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分 like 'US%'";
                sql_query += " order by 名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "セールG")
            {
                if (v_flg == 1)
                {
                    v_retu = "a.名称CD SELLCD,a.名称 SELL名";
                }
                else
                {
                    v_retu = "a.名称CD||' '||a.名称 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO a ";
                sql_query += " where a.名称区分='SLG'";
                sql_query += " order by A.名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "店舗担当")
            {
                if (v_flg == 1)
                {
                    v_retu = "社員CD,名前";
                }
                else
                {
                    v_retu = "社員CD||' '||名前 一覧";
                }
                sql_query = "select " + v_retu + " from HC$Master_SHAIN ";
                sql_query += " where 就業FLG='0'";
                sql_query += " and 社員CD>=:1";
                sql_query += " order by 社員CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "納品先")
            {
                if (v_flg == 1)
                {
                    v_retu = "納品先CD,納品先名";
                }
                else
                {
                    v_retu = "納品先CD||' '||納品先名 納品先DATA";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_NOHIN ";
                sql_query += " where 納品先CD>=:1";
                if (v_para2 != null)
                {
                    sql_query += " and 得意先CD='" + v_para2[0] + "'";
                }
                sql_query += " order by 納品先CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "ブランドグループ")
            {
                if (v_flg == 1)
                {
                    v_retu = "名称CD, 名称";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 一覧";
                }
                sql_query = "SELECT " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分 in ( 'B03' , 'B09' ) order by 名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, null);
            }
            else if (p_kubun == "帳票名")
            {
                if (v_flg == 1)
                {
                    v_retu = "SEQ_NO, 帳票名";
                }
                else
                {
                    v_retu = "SEQ_NO||' '||帳票名 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_PRTM00 ";
                sql_query += " where 帳票名>=:1 order by 帳票名";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "チャネル得意先")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI z";
                sql_query += " where 得意先CD>=:1 and 店種区分 between 1 and 3";
                if (v_para2 != null)
                {
                    sql_query += " and exists (select 'X' from HC$MASTER_TOKUI t2 where t2.得意先CD = '" + v_para2[0] + "' and t2.名称CD03 in ( '.', z.名称CD03 ) ) ";
                }
                sql_query += " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "チャネル移動倉庫")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI z";
                sql_query += " where 得意先CD>=:1 and 在庫管理FLG=1 and 店種区分<9 and 出荷停止FLG=0";
                if (v_para2 != null)
                {
                    sql_query += " and exists (select 'X' from HC$MASTER_TOKUI t2 where t2.得意先CD = '" + v_para2[0] + "' and t2.名称CD03 in ( '.', z.名称CD03 ) ) ";
                }
                sql_query += " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "チャネル全得意先")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI z";
                sql_query += " where 得意先CD>=:1 and 店種区分 >0 " + houjin_str;
                if (v_para2 != null)
                {
                    sql_query += " and exists (select 'X' from HC$MASTER_TOKUI t2 where t2.得意先CD = '" + v_para2[0] + "' and t2.名称CD03 in ( '.', z.名称CD03 ) ) ";
                }
                sql_query += " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "チャネル倉庫")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名 倉庫DATA";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI z";
                sql_query += " where 得意先CD>=:1 and (店種区分=0 OR 倉庫区分=9) " + houjin_str;
                if (v_para2 != null)
                {
                    sql_query += " and exists (select 'X' from HC$MASTER_TOKUI t2 where t2.得意先CD = '" + v_para2[0] + "' and t2.名称CD03 in ( '.', z.名称CD03 ) ) ";
                }
                sql_query += " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "Acチャネル倉庫")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名 倉庫DATA";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI z";
                sql_query += " where 出荷停止FLG = 0 and 得意先CD>=:1 and (店種区分=0 OR 倉庫区分=9) " + houjin_str;
                if (v_para2 != null)
                {
                    sql_query += " and exists (select 'X' from HC$MASTER_TOKUI t2 where t2.得意先CD = '" + v_para2[0] + "' and t2.名称CD03 in ( '.', z.名称CD03 ) ) ";
                }
                sql_query += " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "下げ札メーカー")
            {
                if (v_flg == 1)
                {
                    v_retu = "仕入先CD,仕入先名";
                }
                else
                {
                    v_retu = "仕入先CD||' '||仕入先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_SIIRE ";
                sql_query += " where 名称CD01 = '6' ";
                sql_query += " order by 仕入先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, null);
            }
            else if (p_kubun == "店舗出荷倉庫")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI z";
                sql_query += " where 店種区分 = 0";
                if (v_para2 != null)
                {
                    sql_query += " and exists (select 'X' from HC$MASTER_CONVERT c where c.区分 = 'TSY' and c.一意CD01 = '" + v_para2[0] + "' and c.一意CD02 = z.得意先CD ) ";
                }
                sql_query += " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, null);
            }
            else if (p_kubun == "Ac店舗出荷倉庫")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI z";
                sql_query += " where 出荷停止FLG = 0 and 店種区分 = 0";
                if (v_para2 != null)
                {
                    sql_query += " and exists (select 'X' from HC$MASTER_CONVERT c where c.区分 = 'TSY' and c.一意CD01 = '" + v_para2[0] + "' and c.一意CD02 = z.得意先CD ) ";
                }
                sql_query += " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, null);
            }
            else if (p_kubun == "移動倉庫53")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名 倉庫DATA";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 得意先CD>=:1 and 在庫管理FLG=1 and 店種区分<9 and 出荷停止FLG=0 and 倉庫区分=0 order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "商品53")
            {
                if (v_flg == 1)
                {
                    v_retu = "商品CD,商品名";
                }
                else
                {
                    v_retu = "商品CD||' '||商品名 一覧";
                }
                sql_query = "select " + v_retu + " from HC$Master_SHOHIN ";
                sql_query += " where 商品CD>=:1";

                sql_query += " order by 商品CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "名称USER53")
            {   /* ユーザー固有 */
                /* v_para2は文字列とする */
                if (v_flg == 1)
                {
                    v_retu = "substrb(名称CD,5) 名称CD,名称,substrb(名称CD,5) 予備1,名称 予備2,略称";
                }
                else
                {
                    v_retu = "substrb(名称CD,5)||' '||名称 一覧,substrb(名称CD,5) 名称CD,名称,略称";
                }
                sql_query = "select " + v_retu + "  from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='" + v_para2[0] + "' and substr(名称CD,1,4)='" + v_para2[1] + "'";
                sql_query += " and 名称CD>=:1";
                sql_query += " order by 名称区分,名称CD";
                v_para[0] = v_para2[1] + v_para[0];
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "EC店舗")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 得意先CD>=:1 and 店種区分 between 3 and 8 and 出荷停止FLG=0 and ECFLG=1 order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            /* 12.01.26 勤怠グループ(レイカズン用追加) */
            else if (p_kubun == "勤怠グループ")
            {
                if (v_flg == 1)
                {
                    v_retu = "名称CD コード,名称 名称";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='KGP' order by 名称区分,名称CD";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "店舗パターン")
            {
                if (v_flg == 1)
                {
                    v_retu = "パターンNO コード,パターン名 名称";
                }
                else
                {
                    v_retu = "パターンNO||' '||パターン名 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_84TENPO_PTN ";
                sql_query += " group by パターンNO,パターン名 order by パターンNO";
                sql_query = "select * from (" + sql_query + ")";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "商品パターン")
            {
                if (v_flg == 1)
                {
                    v_retu = "パターンNO コード,パターン名 名称";
                }
                else
                {
                    v_retu = "パターンNO||' '||パターン名 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_84SHOHIN_PTN ";
                sql_query += " group by パターンNO,パターン名 order by パターンNO";
                sql_query = "select * from (" + sql_query + ")";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "サイズパターン")
            {
                if (v_flg == 1)
                {
                    v_retu = "名称CD コード,名称 名称";
                }
                else
                {
                    v_retu = "名称CD||' '||名称 一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='IDX' and 名称CD like 'SZ%' and not 名称CD in ('SZI','SZN')";
                sql_query += " order by 名称CD";
                sql_query = "select * from (" + sql_query + ")";
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "ポイント")
            {   /* 2016.10.25 #30941対応追加 */
                if (v_flg == 1)
                {
                    v_retu = "ポイントCD コード,名称 名称";
                }
                else
                {
                    v_retu = "ポイントCD||' '||名称 一覧";
                }
                sql_query = "SELECT " + v_retu + " FROM HC$MASTER_POINT ";
                if (v_para2[0] == "5")
                {
                    sql_query += " where ポイント区分=1";
                }
                else sql_query += " where 送信FLG=0";
                if (v_para2[0] == "3")
                {
                    sql_query += " and 優先区分='1'";
                }
                else if (v_para2[0] == "4")
                {
                    sql_query += " and 優先区分 in ('2', '3')";
                }
                sql_query += " order by ポイントCD";
                sql_query = "select * from (" + sql_query + ")";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, null);
            }
            else if (p_kubun == "User")
            {
                if (v_flg == 1)
                {
                    v_retu = "ユーザーCD,NVL((select 得意先名 from HC$MASTER_TOKUI T where T.得意先CD = ユーザーCD),'.') 得意先名";
                }
                else
                {
                    v_retu = "ユーザーCD||' '||NVL((select 得意先名 from HC$MASTER_TOKUI T where T.得意先CD = ユーザーCD),'.')  一覧";
                }
                sql_query = "select " + v_retu + " from (select ユーザーCD from HC$Master_REVENUE";
                sql_query += " where ユーザーCD>=:1)";
                sql_query += " order by ユーザーCD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            /* 2021.05.19 #62205_消費税計算方法参照版 */
            else if (p_kubun == "支払単位仕入先")
            {
                if (v_flg == 1)
                {
                    v_retu = "仕入先CD,仕入先名";
                }
                else
                {
                    v_retu = "仕入先CD||' '||仕入先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_SIIRE ";
                sql_query += " where 仕入先CD>=:1 and 消費税計算方法=0 " + houjin_str + " order by 仕入先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            else if (p_kubun == "請求単位得意先")
            {
                if (v_flg == 1)
                {
                    v_retu = "得意先CD,得意先名";
                }
                else
                {
                    v_retu = "得意先CD||' '||得意先名  一覧";
                }
                sql_query = "select " + v_retu + " from HC$MASTER_TOKUI ";
                sql_query += " where 得意先CD>=:1 and 店種区分 between 1 and 3 and 消費税計算方法=0 " + houjin_str + " order by 得意先CD";
                sql_query = GetSqlDisp(sql_query);
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
            return ret_csv;
        }
         
        /// <summary>
        /// ■関数 GetQueryStrHoujin = 法人CD検索用SQL文字列取得
        /// </summary>
        /// <param name="arias">引数 接続文字列</param>
        /// <returns>検索用SQL文字列</returns>
        public string GetQueryStrHoujin(string arias = "")
        {
            string col_str = string.Empty;
            if (AppData.ClassCvnet.config.MultiCoop != null && AppData.ClassCvnet.config.MultiCoop >= 0)
            {
                col_str += " and ( ";
                if (arias != "" || arias != null) col_str += arias + ".";
                col_str += "法人CD='" + AppData.ClassCvnet.config.MultiCoop + "' ";
                col_str += " or ";
                if (arias != "" || arias != null) col_str += arias + ".";
                col_str += "法人CD='.') ";
            }
            return col_str;
        }
          
        /// <summary>
        /// ■関数 GetTanpin = 単品NO取得（明細チェック用）	※.堀田丸正専用
        /// </summary>
        /// <param name="para">商品CD,単品NO</param>
        /// <returns>JANマスタ読込内容</returns>
        public string[] GetTanpin(string[] para)
        {
            string[] ret_para = null;
            string[] v_para = new string[2];
            v_para[0] = para[0].ToString();
            v_para[1] = para[1].ToString();

            /* 単品NOに頭0詰めして検索する */
            string zero_str = string.Empty;
            for (var k = 0; k < 13 - v_para[1].Length; k++) { zero_str += "0"; }
            v_para[1] = zero_str + v_para[1];

            string sql_str = ""
            + "SELECT "
                + "j.色CD 単品NO,j.商品CD,j.商品名,j.相手商品NO,j.備考CD01,j.備考01,j.備考CD02,j.備考02,j.備考CD03,j.備考03,j.元上代,j.原価,j.JANコード1"
                + ",s.商品管理FLG,j.明細仕入区分,s.単位,j.仕入価格,j.上代"
            + " FROM "
                + "hc$master_shohin_jan j,hc$master_shohin s"
            + " WHERE "
                + "s.商品CD=j.商品CD";

            if (v_para[0] != "" || v_para[1] != "")
            {
                string hin = string.Empty;
                string iro = string.Empty;
                sql_str += " AND ";
                if (v_para[0] != "") hin += "j.商品CD='" + v_para[0] + "'";
                if (v_para[1] != "")
                {
                    if (hin != "") iro += " AND ";
                    iro += "j.色CD='" + v_para[1] + "'";
                }
                sql_str += hin + iro;
            }
            sql_str += " ORDER BY j.色CD,j.商品CD,j.サイズCD";
            sql_str = "SELECT * FROM (" + sql_str + ")";
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, v_para);
            if (ret_csv?.Rows.Count > 0)
            {
                ret_para = new string[ret_csv.Columns.Count];
                for (int i = 0; i < ret_csv.Columns.Count; i++)
                {
                    var cellValue = ret_csv.Rows[0][i];
                    ret_para[i] = cellValue != DBNull.Value ? cellValue.ToString() : string.Empty;
                }
            }
            return ret_para;
        }

        /// <summary>
        /// ■関数 GetFukumei = 副名取得（明細チェック用）	※.堀田丸正専用
        /// </summary>
        /// <param name="para">副名CD</param>
        /// <param name="flg">FLG（1：副名1（B01）、2：副名2（B02）、3：副名3（B03））</param>
        /// <returns>名称マスタ（B01～B03）読込内容</returns>
        public string[] GetFukumei(string[] para, int flg)
        {
            string[] ret_para = null;
            string[] v_para = new string[1];
            v_para[0] = para[0].ToString();
            string kbn = "B0" + flg.ToString();
            string sql_str = "select 名称CD,名称 from HC$MASTER_MEISHO  where 名称区分='" + kbn + "' AND 名称CD=:1";
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, v_para);
            if (ret_csv?.Rows.Count > 0)
            {
                ret_para = new string[ret_csv.Columns.Count];
                for (int i = 0; i < ret_csv.Columns.Count; i++)
                {
                    var cellValue = ret_csv.Rows[0][i];
                    ret_para[i] = cellValue != DBNull.Value ? cellValue.ToString() : string.Empty;
                }
            }
            return ret_para;
        }

        /// <summary>
        /// ■関数 GetSouko = 倉庫名取得（明細チェック用）	※.堀田丸正専用
        /// </summary>
        /// <param name="para">引数１:I v_para = 倉庫CD</param>
        /// <returns>得意先マスタ読込内容</returns>
        public string[] GetSouko(string[] para)
        {
            string[] ret_para = null;
            string[] v_para = new string[1];
            v_para[0] = para[0].ToString();
            string sql_str = "select 得意先CD,得意先名 from HC$MASTER_TOKUI where 得意先CD=:1";
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, v_para);
            if (ret_csv?.Rows.Count > 0)
            {
                ret_para = new string[ret_csv.Columns.Count];
                for (int i = 0; i < ret_csv.Columns.Count; i++)
                {
                    var cellValue = ret_csv.Rows[0][i];
                    ret_para[i] = cellValue != DBNull.Value ? cellValue.ToString() : string.Empty;
                }
            }
            return ret_para;
        }

        /// <summary>
        /// ■関数 GetTagCode = タグコード情報取得	※.ロン都専用
        /// </summary>
        /// <param name="para">
        /// 引数１:v_para = タグコード（JANコード1）
        /// 引数２:v_para = 日付
        /// 引数３:v_para = 店舗CD
        /// </param>
        /// <returns>商品色サイズ読込内容</returns>
        public string[] GetTagCode(string[] para)
        {
            string[] ret_para = null;
            string[] v_para = new string[1];
            v_para[0] = para[0].ToString();
            string sql_str = ""
            + "SELECT "
                + "A.商品CD,"
                + "A.色CD,"
                + "A.サイズCD,"
                + "GET_JODAI(A.商品CD,A.色CD,A.サイズCD,'" + para[1] + "','" + para[2] + "',A.JANコード1) 上代単価,"
                + "GET_GENKA(A.商品CD,'" + para[0] + "','" + para[1] + "') 下代単価,"
                + "B.商品名,"
                + "NVL((SELECT H.名称 FROM HC$MASTER_MEISHO H WHERE H.名称区分='COL' AND H.名称CD=A.色CD),'.') 色名,"
                + "GET_SIZENAME(A.商品CD,A.サイズCD) サイズ名,"
                + "B.メーカー品番,"
                + "B.元上代,"
                + "B.原価,"
                + "A.仕入価格 仕入値,"
                + "A.明細仕入区分||' '||decode(A.明細仕入区分,1,'買取',2,'委託',3,'消化','') 仕入区分"
            + " FROM "
                + "HC$MASTER_SHOHIN_JAN A JOIN "
                + "HC$MASTER_SHOHIN B ON (B.商品CD=A.商品CD)"
            + " WHERE "
                + "A.JANコード1=:1";
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, v_para);
            if (ret_csv?.Rows.Count > 0)
            {
                ret_para = new string[ret_csv.Columns.Count];
                for (int i = 0; i < ret_csv.Columns.Count; i++)
                {
                    var cellValue = ret_csv.Rows[0][i];
                    ret_para[i] = cellValue != DBNull.Value ? cellValue.ToString() : string.Empty;
                }
            }
            return ret_para;
        }

        /// <summary>
        /// ■関数 GetSime = 締日取得
        /// </summary>
        /// <param name="para1">
        /// 引数1:I para1 = 名称CD
        /// 引数2:I para2 = 日付(chr)
        /// </param>
        /// <returns>ステータス(0:正常,1:エラー)</returns>
        public string GetSime(string para1 = null)
        { 
            string[] v_para = new string[1];
            if (para1 != null) v_para[0] = para1;
            else v_para[0] = "1";
            string sql_str = "select nvl((select 名称 from hc$master_meisho where 名称区分='SIM' and 名称cd=:1),'19010101') 値 from dual";
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, v_para);
            if (ret_csv?.Rows.Count > 0)
            {
                var cellValue = ret_csv.Rows[0][0];
                return cellValue != DBNull.Value ? cellValue.ToString() : string.Empty;

            }
            return string.Empty;
        }

        /// <summary>
        /// ■関数 GetSime2 = 締日取得
        /// </summary>
        /// <param name="para">
        /// 引数1:I para1 = 名称CD
        /// 引数2:I para2 = 日付(chr)	
        /// </param>
        /// <returns>ステータス(0:正常,1:エラー)</returns>
        public DataTable GetSime2(string[] para1 = null)
        {
            string sql_str = "";
            sql_str += " select 1 順, nvl((select 名称 from hc$master_meisho where 名称区分='SIM' and 名称cd='2'),'19010101') 値 from dual";
            sql_str += " union all";
            sql_str += " select 2 順, nvl((select 名称 from hc$master_meisho where 名称区分='SIM' and 名称cd='3'),'19010101') 値 from dual";
            sql_str += " union all";
            sql_str += " select 3 順, nvl((select 名称 from hc$master_meisho where 名称区分='SIM' and 名称cd='4'),'19010101') 値 from dual";

            sql_str = "select * from ( " + sql_str + " ) a order by a.順";
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str);
            return ret_csv;
        }

        /// <summary>
        /// ■関数 GetKij = 生地付属取得（明細チェック用）
        /// </summary>
        /// <param name="para">引数１:I v_para = 商品CD</param>
        /// <returns>生地付属マスタ読込内容</returns>
        public string[] GetKij(string[] para)
        {
            string[] ret_para = null;
            string[] v_para = new string[1];
            v_para[0] = para[0].ToString();
            string sql_str = "select A.商品CD,A.商品名,A.単価,A.仕入先商品CD";
            sql_str += " from HC$MASTER_SHKIJI A";
            sql_str += " where A.商品CD=:1 order by 商品CD";
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, v_para);
            if (ret_csv?.Rows.Count > 0)
            {
                ret_para = new string[3];
                ret_para[0] = ret_csv.Rows[0][0].ToString();
                ret_para[1] = ret_csv.Rows[0][1].ToString();
                ret_para[2] = ret_csv.Rows[0][2].ToString();
            }
            return ret_para;
        }

        /// <summary>
        /// ■関数 GetMaker = 仕入先名取得（明細チェック用）	※.アージュ専用
        /// </summary>
        /// <param name="para">引数１:I v_para = 仕入先CD</param>
        /// <returns>仕入先マスタ読込内容</returns>
        public string[] GetMaker(string[] para)
        {
            string[] ret_para = null;
            string[] v_para = new string[1];
            v_para[0] = para[0].ToString();
            string sql_str = "select 仕入先CD,仕入先名 from HC$MASTER_SIIRE where 仕入先CD=:1";
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, v_para);
            if (ret_csv?.Rows.Count > 0)
            {
                ret_para = new string[ret_csv.Columns.Count];
                for (int i = 0; i < ret_csv.Columns.Count; i++)
                {
                    var cellValue = ret_csv.Rows[0][i];
                    ret_para[i] = cellValue != DBNull.Value ? cellValue.ToString() : string.Empty;
                }
            }
            return ret_para;
        }

        /// <summary>
        /// ■関数 GetBunrui = 分類取得（明細チェック用）	※.アージュ専用
        /// </summary>
        /// <param name="para">
        /// 引数１:I v_para = 分類区分（名称区分）
        /// 引数２:I v_para = 分類CD（名称CD）
        /// </param>
        /// <returns>名称マスタ読込内容</returns>
        public string[] GetBunrui(string[] para)
        { 
            string[] ret_para = null;
            string[] v_para = new string[2];
            v_para[0] = para[0].ToString();
            v_para[1] = para[1].ToString();
            string sql_str = "select 名称CD,名称 from HC$MASTER_MEISHO  where 名称区分=:1 AND 名称CD=:2";
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, v_para); 
            if (ret_csv?.Rows.Count > 0)
            {
                ret_para = new string[ret_csv.Columns.Count]; 
                for (int i = 0; i < ret_csv.Columns.Count; i++)
                {
                    var cellValue = ret_csv.Rows[0][i];
                    ret_para[i] = cellValue != DBNull.Value ? cellValue.ToString() : string.Empty;
                }
            }
            return ret_para;
        }

        /// <summary>
        /// ■関数 GetStringValue = Split文字にて分けた添え字1以上のものを取得
        /// </summary>
        /// <param name="obj_str">str = 元String</param>
        /// <param name="sp">sp = Split文字 nullのときは" "</param>
        /// <returns>Split文字にて分けた添え字1以上のもの</returns>
        public string GetStringValue(string obj_str, string sp = null)
        {
            if (string.IsNullOrEmpty(obj_str)) return string.Empty; 
            string sep_str = sp ?? " ";
            string[] sep_ar = obj_str.Split(new string[] { sep_str }, StringSplitOptions.None);

            if (sep_ar.Length <= 1)
                return string.Empty;

            // join from index 1 onwards
            return string.Join(sep_str, sep_ar.Skip(1));
        }

        /// <summary>
        /// ■関数 GetCrs = ユーザー７６神戸ﾚｻﾞｰ用商品変換 12.0627
        /// </summary>
        /// <param name="para"></param>
        /// <param name="flg"></param>
        /// <returns></returns>
        public string GetShohin76(string para, int? flg = 1)
        {
            if (string.IsNullOrEmpty(para)) return null;

            // Default to 1 if null
            int mode = flg ?? 1;

            if (mode == 1)
            {
                // Insert hyphens
                if (para.Length != 12) return null;
                return $"{para.Substring(0, 3)}-{para.Substring(3, 4)}-{para.Substring(7, 4)}-{para.Substring(11, 1)}";
            }
            else if (mode == 2)
            {
                // Remove hyphens
                if (para.Length != 15) return null;
                return para.Substring(0, 3) + para.Substring(4, 4) + para.Substring(9, 4) + para.Substring(14, 1);
            }

            return para;
        }


        public string Get_KokyakuCsvQuery(string sql_str, int flg )
        { 
            var sql_query = ""
            + "SELECT "
                + "A.顧客CD,A.顧客名,A.カナ,"
                + comboItem00.GetCaseStr("顧客区分LCV", "A.顧客区分") + " 顧客区分,A.誕生日,A.性別||' '||DECODE(A.性別, 1, '女性', 2, '男性', '未設定') 性別,"
                + "A.ポイントランク,"
                + "A.TEL1,A.TEL2,A.EMAIL,"
                + "SUBSTRB(A.顧客ランク, 1, 1) 顧客ランクR,SUBSTRB(A.顧客ランク, 2, 1) 顧客ランクF,SUBSTRB(A.顧客ランク, 3, 1) 顧客ランクM,"
                + "A.郵便番号,"
                + "TRIM(A.住所1) 住所1,TRIM(A.住所2) 住所2,TRIM(A.住所3) 住所3,"
                + "A.最終来店日,"
                + "NVL((SELECT P.REALポイント FROM HC$POINT_REAL P WHERE P.顧客CD=A.顧客CD),0) ポイント,"
                + "A.累計来店回数,A.累計購入数量,A.累計購入金額,"
                + "A.店舗CD||' '||NVL((SELECT T.得意先名 FROM HC$MASTER_TOKUI T WHERE T.得意先CD=A.店舗CD),'') 店舗名,"
                + "A.販売員CD||' '||NVL((SELECT E.名前 FROM HC$MASTER_SHAIN E WHERE E.社員CD=A.販売員CD),'') 販売員名,"
                + "A.名称CD01||' '||NVL((SELECT M.名称 FROM HC$MASTER_MEISHO M WHERE M.名称区分='K01' AND M.名称CD=A.名称CD01),'') 属性01,"
                + "A.名称CD02||' '||NVL((SELECT M.名称 FROM HC$MASTER_MEISHO M WHERE M.名称区分='K02' AND M.名称CD=A.名称CD02),'') 属性02,"
                + "A.名称CD03||' '||NVL((SELECT M.名称 FROM HC$MASTER_MEISHO M WHERE M.名称区分='K03' AND M.名称CD=A.名称CD03),'') 属性03,"
                + "A.名称CD04||' '||NVL((SELECT M.名称 FROM HC$MASTER_MEISHO M WHERE M.名称区分='K04' AND M.名称CD=A.名称CD04),'') 属性04,"
                + "A.名称CD05||' '||NVL((SELECT M.名称 FROM HC$MASTER_MEISHO M WHERE M.名称区分='K05' AND M.名称CD=A.名称CD05),'') 属性05,"
                + "A.名称CD06||' '||NVL((SELECT M.名称 FROM HC$MASTER_MEISHO M WHERE M.名称区分='K06' AND M.名称CD=A.名称CD06),'') 属性06,"
                + "A.名称CD07||' '||NVL((SELECT M.名称 FROM HC$MASTER_MEISHO M WHERE M.名称区分='K07' AND M.名称CD=A.名称CD07),'') 属性07,"
                + "A.名称CD08||' '||NVL((SELECT M.名称 FROM HC$MASTER_MEISHO M WHERE M.名称区分='K08' AND M.名称CD=A.名称CD08),'') 属性08,"
                + "A.名称CD09||' '||NVL((SELECT M.名称 FROM HC$MASTER_MEISHO M WHERE M.名称区分='K09' AND M.名称CD=A.名称CD09),'') 属性09,"
                + "A.名称CD10||' '||NVL((SELECT M.名称 FROM HC$MASTER_MEISHO M WHERE M.名称区分='K10' AND M.名称CD=A.名称CD10),'') 属性10,"
                + "replace(TRANSLATE (A.拡張メモ, '#' || CHR(13) || CHR(10), '#'), ',', '') 拡張メモ"
            + " FROM (" + sql_str + ") A"
            + " ORDER BY A.顧客CD ASC";

            return sql_query;
        }
    }

    public class Config
    {
        public int UserFlg { get; set; }
        public int UserFlg2 { get; set; }
        public int ComboListFLg { get; set; }
        public int ComboListFLg2 { get; set; }
        public int ComboListFLg3 { get; set; }
        public int TanaFlg { get; set; }
        public int PosFlg { get; set; }
        public int YosanFlg { get; set; }
        public int YosanFlg2 { get; set; }
        public int equalFlg { get; set; }
        public int janlength1 { get; set; }
        public int janlength2 { get; set; }
        public string janpattern { get; set; } = string.Empty;
        public string pttrnStyle { get; set; } = string.Empty;
        public string[] MstDialog { get; set; }
        public int UseUUID { get; set; }

        public int? MultiCoop { get; set; } = null;
        public int ykzaiko { get; set; }
        public int denpattern_no { get; set; }
        public int denpattern_sei { get; set; }
        public int ido_kubun { get; set; } = 5;
        public int makejan { get; set; }
        public string Regaxchk { get; set; } = string.Empty;
        public int renbanflg { get; set; }
        public int ido_disp { get; set; }
        public int zaikokin { get; set; }
        public int ECUniqueKey { get; set; }
        public int MakerOnly { get; set; }
        public int danflg { get; set; }
        public string barpattern { get; set; } = string.Empty;
        public int usegenka { get; set; }
        public int smtflg { get; set; }
        public int check_shime { get; set; }
        public int JodaiConfig { get; set; }
        public int JodaiBrdFlg { get; set; }
        public string ManageMonthly { get; set; } = string.Empty;
        public string CollabHachuCode { get; set; } = string.Empty;
        public int UseCollab { get; set; }
        public int LastSiire { get; set; }
        public int Real_Zaiko { get; set; }
        public int TenTonyu { get; set; }
        public int CountNoZaiKanri { get; set; }
        public int DirectJuchu { get; set; }
        public int den_grouphin { get; set; }
        public int den_grouphin3 { get; set; }
        public int den_grouphin4 { get; set; }
        public int den_grouphin6 { get; set; }
        public int den_grouphin7 { get; set; }
        public int GetMaruiHenpin { get; set; }
        public int DeleteWithJan { get; set; }
        public int ColSizFlg { get; set; }
        public int DefDateRange { get; set; } = 7;
        public int UseKintai { get; set; }
        public int sokohyjflg { get; set; }
        public int UseBrdRank { get; set; }
        public int PrintPDFFlg { get; set; }
        public int NendoKeta { get; set; }
        public int InpShoFlg { get; set; }
        public int OutMasterMei { get; set; }
        public int Use_Idoshiji { get; set; }
        public string Category { get; set; } = "BN0@2;BN1@2;BN2@2";
        public int ColSizMei { get; set; }
        public int UseSaleKubun { get; set; }
        public int Huke_Month { get; set; } = 1;
        public string ECazukariCD { get; set; } = string.Empty;
        public int torikin { get; set; }
        public string HHTcaseCD { get; set; } = string.Empty;
        public int TenpoFix { get; set; } = 1;
        public string AttachFiles { get; set; } = string.Empty;
        public string janConvertFlg { get; set; } = string.Empty;
        public string ztksouko { get; set; } = string.Empty;
        public string bkgkey { get; set; } = string.Empty;
        public int jodaihyjflg { get; set; }
        public string Postyubun { get; set; } = string.Empty;
        public int BaikaKangen { get; set; }
        public int Tenjiken { get; set; }
        public int hhtchk { get; set; }
        public int hhtchkmg { get; set; }
        public int TenjiShoki { get; set; }
        public string YBSchema { get; set; } = string.Empty;
        public int VARShime { get; set; }
        public string bindtenpo { get; set; } = "ブランド";
        public int Rk_Zaiko { get; set; }
        public int rfvflg { get; set; }
        public int SellMFlg { get; set; }
        public int SKUFlg { get; set; }
        public int zaikock { get; set; }
        public int kakehyjflg { get; set; }
        public int DateSort { get; set; }
        public int hhtmach { get; set; }
        public string janConvertFlg1 { get; set; } = string.Empty;
        public string janConvertFlg2 { get; set; } = string.Empty;
        public string janConvertFlg3 { get; set; } = string.Empty;
        public string janConvertFlg4 { get; set; } = string.Empty;
        public string janConvertFlg5 { get; set; } = string.Empty;
        public int souheikin { get; set; }
        public int desktop { get; set; }
        public int tanaido { get; set; }
        public int urikakeflg { get; set; }
        public int tanpin { get; set; }
        public int oroshi { get; set; }
        public int syousu { get; set; }
        public int Set49JAN { get; set; }
        public int janproFlg { get; set; }
        public int cpacsvflg { get; set; }
        public int idoukeflg { get; set; }
        /* 2010.12.16 外貨単価対応 S */
        public int dispGaitan { get; set; }  /* 外貨単価表示有無 */
        public int computeGaikaToGedai { get; set; } = 1; /* 自動計算(外貨単価→下代金額) */
        public int computeGedaiToGaika { get; set; } = 0; /* 自動計算(下代金額→外貨単価) */
        public int computeGedaiGaikaDef { get; set; } = 1; /* 自動計算ﾃﾞﾌｫﾙﾄ(1:外貨単価→下代金額 2:下代金額→外貨単価) */
        /* 2010.12.16 外貨単価対応 E */
        /* 2011.02.04 汎用SQL自動実行登録 S */
        public int GenericSQLRegAutoExec { get; set; } = 0;  /* 汎用SQLを自動実行に登録する(0:登録しない,1:登録する) */
        /* 2011.02.04 汎用SQL自動実行登録 E */
        public int sijicshyj { get; set; }
        public int shokbnflg { get; set; }
        public int hhtkisyu { get; set; }
        /* 2011.12.09 原価(商品色サイズマスタ)ごとに持つ */
        public int GenkaSKU { get; set; }
        public int syukastop { get; set; }
        public int hatyustop { get; set; }
        /* 11.12.12 大興Posﾏｽﾀ連携バージョン */
        public int PosDaiMstflg { get; set; }
        /* 2012.06.18 バッチ在庫 */
        public int Batch_Zaiko { get; set; }
        /* 2014.05.7 価格セーブ */
        public int pricesvflg { get; set; }
        /* 2014.04.25 取置入力での売上伝票の登録 */
        public int ToriokiUriDen { get; set; }
        /* 2014.06.12 #13570対応追加 */
        public int CpaGphFlg { get; set; }
        /* 2016.06.28 #27360追加 */
        public int LcvmailFlg { get; set; }
        /* 2017.12.18 #38986追加 */
        public int idoShiji_Kanryo { get; set; }
        public int kakedate { get; set; }
        /* 20201228 sspread 対応 */
        public int ExcelOutFlg { get; set; }
        /*#60683 追加*/
        public int Disp_Hinban { get; set; }
        /* 2021.07.15 ECフラグ 追加 */
        public int ECFLG { get; set; }
        /* 20211203 伝票メール送信対応 */
        public int MailSendFlg { get; set; }
        /* 2023.12.20 #71837対応追加 CPA表示件数制限 */
        public int CpaCount { get; set; }
    }

    public class OrgMenuDef
    {
        public int No1 { get; set; } /* 初期サブメニュー番号(0-) */
        public int No2 { get; set; } /* 初期メニュー番号(0-) */
        public string MenuName { get; set; } = string.Empty;
        public int MenuEnable { get; set; }
        public string CrsName { get; set; } = string.Empty;
        public string CrsPara { get; set; } = string.Empty;
        public int UserFlg { get; set; }
    }

    public class ComboItem00 
    {
        public Dictionary<TKey, string> ComboItem_00<TKey>(string _name)
        {
            var _list = new Dictionary<TKey, string>();

            if (_name == "締日")
            {
                for (int i = 1; i < 29; i++)
                    _list.Add((TKey)(object)i, i.ToString("00"));
                _list.Add((TKey)(object)99, "99");
            }
            if (_name == "週区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 0, "0 月-日" },
                     { (TKey)(object) 1, "1 日-土" }
                };
            }
            if (_name == "端数")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 0, "0 四捨五入" },
                     { (TKey)(object) 1, "1 切り上げ" },
                     { (TKey)(object) 2, "2 切り捨て" }
                };
            }
            if (_name == "店種")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 1, "1 卸先" },
                     { (TKey)(object) 3, "3 売仕店" },
                     { (TKey)(object) 6, "6 直営店" }
                };
            }
            if (_name == "店種2")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 0, "0 倉庫" },
                     { (TKey)(object) 1, "1 卸先" },
                     { (TKey)(object) 3, "3 売仕店" },
                     { (TKey)(object) 6, "6 直営店" }
                };
            }
            if (_name == "予定月")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 0, "0 当月" },
                     { (TKey)(object) 1, "1 翌月" },
                     { (TKey)(object) 2, "2 翌々月" },
                     { (TKey)(object) 3, "3 翌々々月" },
                     { (TKey)(object) 4, "4 4ヶ月後" },
                     { (TKey)(object) 5, "5 5ヶ月後" },
                     { (TKey)(object) 6, "6 6ヶ月後" }
                };
            }
            if (_name == "する")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 0, "0 しない" },
                     { (TKey)(object) 1, "1 する" }
                };
            }
            if (_name == "有")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 0, "0 無" },
                     { (TKey)(object) 1, "1 有" }
                };
            }
            if (_name == "入金区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 80, "80 現金" },
                     { (TKey)(object) 81, "81 小切手" },
                     { (TKey)(object) 82, "82 振込" },
                     { (TKey)(object) 83, "83 振込手数料" },
                     { (TKey)(object) 85, "85 手形" },
                     { (TKey)(object) 88, "88 相殺" },
                     { (TKey)(object) 89, "89 その他" },
                     { (TKey)(object) 99, "99 関連伝票" }
                };
            }
            if (_name == "入金区分2")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 80, "80 現金" },
                     { (TKey)(object) 81, "81 小切手" },
                     { (TKey)(object) 82, "82 振込" },
                     { (TKey)(object) 85, "85 手形" }
                };
            }
            if (_name == "商品仕入区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 10, "10 仕入" },
                     { (TKey)(object) 15, "15 消化仕入" },
                     { (TKey)(object) 20, "20 仕入返品" },
                     { (TKey)(object) 25, "25 消化仕入返品" },
                     { (TKey)(object) 30, "30 値引" }
                };
            }
            if (_name == "生地仕入区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 10, "10 仕入" },
                     { (TKey)(object) 20, "20 仕入返品" },
                     { (TKey)(object) 30, "30 値引" },
                     { (TKey)(object) 99, "99 消費税" }
                };
            }
            if (_name == "共通仕入区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 10, "10 仕入" },
                     { (TKey)(object) 15, "15 消化仕入" },
                     { (TKey)(object) 20, "20 仕入返品" },
                     { (TKey)(object) 25, "25 消化仕入返品" },
                     { (TKey)(object) 30, "30 値引" },
                     { (TKey)(object) 99, "99 消費税" }
                };
            }
            if (_name == "本部売上区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 10, "10 売上" },
                     { (TKey)(object) 11, "11 ｾｰﾙ売上" },
                     { (TKey)(object) 20, "20 売上返品" },
                     { (TKey)(object) 21, "21 ｾｰﾙ売上返品" },
                     { (TKey)(object) 30, "30 合計値引" },
                     { (TKey)(object) 31, "31 単品値引" },
                     { (TKey)(object) 99, "99 消費税" }
                };
            }
            if (_name == "店舗売上区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 10, "10 売上" },
                     { (TKey)(object) 11, "11 ｾｰﾙ売上" },
                     { (TKey)(object) 14, "14 社販売上" },
                     { (TKey)(object) 20, "20 売上返品" },
                     { (TKey)(object) 21, "21 ｾｰﾙ売上返品" },
                     { (TKey)(object) 24, "24 社販売上返品" },
                     { (TKey)(object) 30, "30 合計値引" },
                     { (TKey)(object) 31, "31 単品値引" },
                     { (TKey)(object) 99, "99 消費税" }
                };
            }
            if (_name == "共通売上区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 10, "10 売上" },
                     { (TKey)(object) 11, "11 ｾｰﾙ売上" },
                     { (TKey)(object) 14, "14 社販売上" },
                     { (TKey)(object) 20, "20 売上返品" },
                     { (TKey)(object) 21, "21 ｾｰﾙ売上返品" },
                     { (TKey)(object) 24, "24 社販売上返品" },
                     { (TKey)(object) 30, "30 合計値引" },
                     { (TKey)(object) 31, "31 単品値引" },
                     { (TKey)(object) 99, "99 消費税" }
                };
            }
            if (_name == "棚卸区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 10, "10 実棚" },
                     { (TKey)(object) 11, "11 ロス" }
                };
            }
            if (_name == "移動区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 10, "10 移動" },
                     { (TKey)(object) 11, "11 受注移動" }
                };
            }
            if (_name == "移動受区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 10, "10 移動受" }
                };
            }
            if (_name == "桁切")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 0, "0 一円単位" },
                     { (TKey)(object) 1, "1 十円単位" },
                     { (TKey)(object) 2, "2 百円単位" },
                     { (TKey)(object) 3, "3 千円単位" },
                     { (TKey)(object) 4, "4 万円単位" }
                };
            }
            if (_name == "為替桁切")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 0, "0 整数" },
                     { (TKey)(object) 1, "1 小数点第一位" },
                     { (TKey)(object) 2, "2 小数点第二位" }
                };
            }
            if (_name == "下代計算")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 0, "0 売価*掛率" },
                     { (TKey)(object) 1, "1 上代*掛率" }
                };
            }
            if (_name == "消費税計算")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 0, "0 請求単位" },
                     { (TKey)(object) 1, "1 伝票単位" }
                };
            }
            if (_name == "消費税計算2")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 0, "0 支払単位" },
                     { (TKey)(object) 1, "1 伝票単位" }
                };
            }
            if (_name == "伝票")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 0, "0 印刷しない" },
                     { (TKey)(object) 1, "1 自社伝票" },
                     { (TKey)(object) 2, "2 百貨店伝票" },
                     { (TKey)(object) 3, "3 チェーンストア統一伝票1型" },
                     { (TKey)(object) 4, "4 チェーンストア統一伝票" },
                     { (TKey)(object) 5, "5 特殊伝票" },
                     { (TKey)(object) 6, "6 チェーンストア統一伝票（ターンアラウンド用2型）" },
                     { (TKey)(object) 7, "7 チェーンストア統一伝票（ターンアラウンド用1型）" },
                     { (TKey)(object) 8, "8 百貨店伝票Ⅱ型" }
                };
            }
            if (_name == "課税区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 0, "0 非課税" },
                     { (TKey)(object) 1, "1 外税" },
                     { (TKey)(object) 2, "2 内税" }
                };
            }
            if (_name == "生地付属")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 0, "0 製品" },
                     { (TKey)(object) 1, "1 生地" },
                     { (TKey)(object) 2, "2 付属品" },
                     { (TKey)(object) 3, "3 工賃" },
                     { (TKey)(object) 4, "4 ネーム" },
                     { (TKey)(object) 5, "5 プレス" },
                     { (TKey)(object) 6, "6 その他" }
                };
            }
            if (_name == "自動配分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) "000", "000 なし" },
                     { (TKey)(object) "127", "127 毎日" },
                     { (TKey)(object) "001", "001 週1回:日" },
                     { (TKey)(object) "009", "009 週2回:日水" },
                     { (TKey)(object) "041", "041 週3回:日水金" },
                     { (TKey)(object) "128", "128 一時中止" }
                };
            }
            if (_name == "システム区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) "00", "00 売上" },
                     { (TKey)(object) "01", "01 店舗売上" },
                     { (TKey)(object) "02", "02 仕入" },
                     { (TKey)(object) "03", "03 製品仕入" },
                     { (TKey)(object) "04", "04 棚卸" },
                     { (TKey)(object) "05", "05 移動" },
                     { (TKey)(object) "06", "06 入金" },
                     { (TKey)(object) "07", "07 支払" },
                     { (TKey)(object) "11", "11 移動受" },
                     { (TKey)(object) "12", "12 受注" },
                     { (TKey)(object) "13", "13 発注" },
                     { (TKey)(object) "14", "14 社販売上" }
                };
            }
            if (_name == "商品発注区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 10, "10 発注" },
                     { (TKey)(object) 11, "11 追加発注" },
                     { (TKey)(object) 15, "15 生産発注" }
                };
            }
            if (_name == "商品発注区分53")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 10, "10 直営" },
                     { (TKey)(object) 11, "11 Web" },
                     { (TKey)(object) 15, "12 卸" }
                };
            }
            if (_name == "商品受注区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 10, "10 受注" },
                     { (TKey)(object) 11, "11 追加1" },
                     { (TKey)(object) 12, "12 追加2" },
                     { (TKey)(object) 13, "13 追加3" },
                     { (TKey)(object) 14, "14 追加4" },
                     { (TKey)(object) 15, "15 追加5" },
                     { (TKey)(object) 16, "16 追加6" },
                     { (TKey)(object) 17, "17 追加7" },
                     { (TKey)(object) 18, "18 追加8" },
                     { (TKey)(object) 19, "19 追加9" }
                };
            }
            if (_name == "倉庫区分02")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 0, "0 自社倉庫" },
                     { (TKey)(object) 1, "1 委託倉庫01" },
                     { (TKey)(object) 2, "2 委託倉庫02" },
                     { (TKey)(object) 3, "3 委託倉庫03" },
                     { (TKey)(object) 4, "4 委託倉庫04" },
                     { (TKey)(object) 5, "5 委託倉庫05" },
                     { (TKey)(object) 6, "6 委託倉庫06" },
                     { (TKey)(object) 7, "7 委託倉庫07" },
                     { (TKey)(object) 8, "8 委託倉庫08" },
                     { (TKey)(object) 9, "9 委託倉庫09" }
                };
            }
            if (_name == "休暇")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 0, "0 通常" },
                     { (TKey)(object) 1, "1 通常休暇" },
                     { (TKey)(object) 2, "2 有給休暇" },
                     { (TKey)(object) 3, "3 特別休暇" }
                };
            }
            if (_name == "来勘")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 0, "0 当月勘定" },
                     { (TKey)(object) 1, "1 来月勘定" }
                };
            }
            if (_name == "HHT08")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) "03", "03 仕入" },
                     { (TKey)(object) "04", "04 仕入返品" },
                     { (TKey)(object) "05", "05 移動" },
                     { (TKey)(object) "08", "08 発注" },
                     { (TKey)(object) "21", "21 売上" },
                     { (TKey)(object) "22", "22 売上返品" },
                     { (TKey)(object) "60", "60 棚卸" },
                     { (TKey)(object) "61", "61 棚卸売消" }
                };
            }
            if (_name == "棚卸区分08")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 10, "10 実棚" },
                     { (TKey)(object) 61, "61 売消" }
                };
            }
            if (_name == "仕入区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 1, "1 買取" },
                     { (TKey)(object) 2, "2 委託" },
                     { (TKey)(object) 3, "3 消化" }
                };
            }
            if (_name == "セール区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 0, "0 プロパー" },
                     { (TKey)(object) 1, "1 セール" }
                };
            }
            if (_name == "価格区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) "MAC", "MAC 月次平均原価" },
                     { (TKey)(object) "DAC", "DAC 日次平均原価" }
                };
            }
            if (_name == "配分出荷区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 10, "10 出荷" },
                     { (TKey)(object) 11, "11 追加出荷" }
                };
            }
            if (_name == "配分出荷区分2")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 10, "10 在庫配分（自）" },
                     { (TKey)(object) 11, "11 在庫配分（手）" },
                     { (TKey)(object) 15, "15 在庫配分（倉）" },
                     { (TKey)(object) 18, "18 受注配分" },
                     { (TKey)(object) 20, "20 卸出荷" },
                     { (TKey)(object) 40, "40 EC出荷" },
                     { (TKey)(object) 60, "60 客注取置" },
                     { (TKey)(object) 70, "70 移動指示" },
                     { (TKey)(object) 80, "80 店舗出荷依頼" },
                     { (TKey)(object) 90, "90 初回配分（店）" },
                     { (TKey)(object) 95, "95 初回配分（倉）" },
                     { (TKey)(object) 98, "98 受注配分" }
                };
            }
            if (_name == "顧客区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 0, "0 通常会員" },
                     { (TKey)(object) 1, "1 ｺﾞｰﾙﾄﾞ会員" },
                     { (TKey)(object) 9, "9 退会済み" }
                };
            }
            if (_name == "仕入先仕入区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 0, "0 無" },
                     { (TKey)(object) 1, "1 買取" },
                     { (TKey)(object) 2, "2 委託" },
                     { (TKey)(object) 3, "3 消化" }
                };
            }
            if (_name == "POS区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object)  0, "0 通常" },
                     { (TKey)(object)  9, "9 POSﾏｽﾀ削除指示" },
                     { (TKey)(object) 10, "10 出力しない" }
                };
            }
            if (_name == "調整移動区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 10, "10 上代調整" },
                     { (TKey)(object) 11, "11 原価調整" },
                     { (TKey)(object) 19, "19 その他調整" }
                };
            }
            if (_name == "調整区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 10, "10 入庫" },
                     { (TKey)(object) 20, "20 出庫" }
                };
            }
            if (_name == "性別")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) ".", "." },
                     { (TKey)(object) "0", "0 女性" },
                     { (TKey)(object) "1", "1 男性" }
                };
            }
            if (_name == "セール展開")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 1, "1 商品" },
                     { (TKey)(object) 2, "2 商品色" },
                     { (TKey)(object) 3, "3 商品ｻｲｽ" },
                     { (TKey)(object) 4, "4 商品色ｻｲｽﾞ" }
                };
            }
            if (_name == "得意先移動区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object)  0, "0 指定無" },
                     { (TKey)(object)  5, "5 即時" },
                     { (TKey)(object) 10, "10 積送" }
                };
            }
            if (_name == "配送区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 0, "0 宅配便" },
                     { (TKey)(object) 1, "1 ヤマト運輸" },
                     { (TKey)(object) 2, "2 佐川急便" }
                };
            }
            if (_name == "支払区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 0, "0 銀行振込" },
                     { (TKey)(object) 1, "1 クレジットカード" },
                     { (TKey)(object) 2, "2 代金引換" }
                };
            }
            if (_name == "入力区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 0, "0 手入力" },
                     { (TKey)(object) 1, "1 Yahoo" },
                     { (TKey)(object) 2, "2 楽天" },
                     { (TKey)(object) 9, "9 ECCUBE" }
                };
            }
            if (_name == "EC商品受注区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 10, "10 BtoC注文" },
                     { (TKey)(object) 20, "20 キャンセル" }
                };
            }
            if (_name == "EC店舗売上区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 15, "15 BtoC売上" },
                     { (TKey)(object) 25, "25 BtoC返品" }
                };
            }
            if (_name == "ECステータス")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) -2, "-2 売上返品" },
                     { (TKey)(object) -1, "-1 キャンセル" },
                     { (TKey)(object)  0, "0 処理中" },
                     { (TKey)(object)  3, "3 出荷中" },
                     { (TKey)(object)  9, "9 完了" }
                };
            }
            if (_name == "移動送信FLG")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 0, "0 未" },
                     { (TKey)(object) 2, "2 入荷予定" },
                     { (TKey)(object) 4, "4 出荷確定" },
                     { (TKey)(object) 6, "6 入荷予定+出荷確定" }
                };
            }
            if (_name == "発注送信FLG")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 0, "0 未" },
                     { (TKey)(object) 2, "2 入荷予定" }
                };
            }
            if (_name == "発送区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 10, "10 発送" },
                     { (TKey)(object) 20, "20 発送取消" }
                };
            }
            if (_name == "在庫登録区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 10, "10 在庫登録" },
                     { (TKey)(object) 20, "20 在庫登録取消" }
                };
            }
            if (_name == "仕入返品指示区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 10, "10 指示" }
                };
            }
            if (_name == "調整区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 10, "10 入庫" },
                     { (TKey)(object) 20, "20 紛失" },
                     { (TKey)(object) 21, "21 盗難" },
                     { (TKey)(object) 22, "22 破損" },
                     { (TKey)(object) 23, "23 検品ミス" },
                     { (TKey)(object) 29, "29 その他" }
                };
            }
            if (_name == "商品発注区分76")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 10, "10 新規発注" },
                     { (TKey)(object) 11, "11 追加発注" },
                     { (TKey)(object) 12, "12 ｾｯﾄ新規発注" },
                     { (TKey)(object) 13, "13 ｾｯﾄ追加発注" },
                     { (TKey)(object) 14, "14 重点品番" },
                     { (TKey)(object) 15, "15 補充発注" },
                     { (TKey)(object) 16, "16 FC発注" }
                };
            }
            if (_name == "発送区分76")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 10, "10 発送" }
                };
            }
            if (_name == "補充発注区分76")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 10, "10 補充発注" }
                };
            }
            if (_name == "FC補充発注区分76")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 11, "11 FC補充発注" }
                };
            }
            if (_name == "予約発注区分76")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 10, "10 予約" }
                };
            }
            if (_name == "承認区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 0, "0 未" },
                     { (TKey)(object) 1, "1 済" }
                };
            }
            if (_name == "顧客区分LCV")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object) 0, "0 カード未使用" },
                     { (TKey)(object) 1, "1 スマホ" },
                     { (TKey)(object) 4, "4 スマホログインのみ" },
                     { (TKey)(object) 9, "9 会員情報未登録" }
                };
            }
            if (_name == "取引詳細区分")
            {
                _list = new Dictionary<TKey, string>
                {
                     { (TKey)(object)  0, "0 通常" },
                     { (TKey)(object)  1, "1 調整" },
                     { (TKey)(object) 22, "22 GMO返金" }
                };
            }

            return _list;
        }

        public string GetStr(string p_str)
        {
            var wrkitem = ComboItem_00<object>(p_str);
            if (wrkitem == null || wrkitem.Count == 0) return null;

            // Join all values (like Biz Designer V did with array entries)
            return string.Join(",", wrkitem.Values);
        }

        public string FindStr(string p_str, string p_code)
        {
            /* 該当するｺｰﾄﾞをもつ名称付き文字列を返す .*/
            var wrkitem = ComboItem_00<object>(p_str);
            if (wrkitem.Count == 0) return null;
            foreach (var item in wrkitem)
            {
                if (item.Key.ToString() == p_code)
                {
                    return item.Value; // e.g. "001 週1回:日"
                }
            }
            return null;
        }

        public string GetDecodeStr(string p_str, string p_codename)
        {
            /* 該当するComboBoxのDECODE文を求める(名称のみ) .*/
            var wrkitem = ComboItem_00<object>(p_str);
            if (wrkitem.Count == 0) return (new string(""));
            var ret_str = "DECODE(" + p_codename;
            foreach (var item in wrkitem)
            {
                ret_str += "," + item.Key.ToString() + ",'" + item.Value + "'";
            }
            if (p_str.Contains("区分"))
            {
                ret_str += ",99,'消費税'";
            }
            ret_str += ",'.')";
            return ret_str;
        }

        public string GetDecodeStr2(string p_str, string p_codename)
        {
            /* 該当するComboBoxのDECODE文を求める(名称のみ) .*/
            var wrkitem = ComboItem_00<object>(p_str);
            if (wrkitem.Count == 0) return (new string(""));
            var ret_str = "DECODE(" + p_codename;
            foreach (var item in wrkitem)
            {
                ret_str += "," + item.Key.ToString() + ",'" + item.Value.Split(" ")[1] + "'";
            }
            if (p_str.Contains("区分"))
            {
                ret_str += ",99,'消費税'";
            }
            ret_str += ",'.')";
            return ret_str;
        }

        public string GetCaseStr(string p_str, string p_codename)
        {
            var wrkitem = ComboItem_00<object>(p_str);
            if (wrkitem.Count == 0) return string.Empty;
            string ret_str = "CASE";
            foreach (var item in wrkitem)
            {
                ret_str += $" WHEN ({p_codename}='{item.Key.ToString()}') THEN '{item.Value}'";
            }
            if (p_str.Contains("区分"))
            {
                ret_str += $" WHEN ({p_codename}='99') THEN '99 消費税'";
            }
            ret_str += " ELSE '.' END";
            return ret_str;
        }

        public string GetCaseStr2(string p_str, string p_codename)
        {
            var wrkitem = ComboItem_00<object>(p_str);
            if (wrkitem.Count == 0) return string.Empty;
            string ret_str = "CASE";
            foreach (var item in wrkitem)
            {
                ret_str += $" WHEN ({p_codename}='{item.Key.ToString()}') THEN '{item.Value.Split(" ")[1]}'";
            }
            if (p_str.Contains("区分"))
            {
                ret_str += $" WHEN ({p_codename}='99') THEN '99 消費税'";
            }
            ret_str += " ELSE '.' END";
            return ret_str;
        }
    }
}
