using CvnetClient.Models;
using System.Data; 

namespace CvnetBaseCore
{
    public class cvnet
    {
        public static int UserFlg = 0;  /* ユーザ別フラグ0-52(0=Package,1=Abl,2=Shirt,..) .*/
        public static int UserFlg2 = 0; /* ユーザ別フラグ2 0=本番用, 1以上=仮 .*/
        public static int MaxCntDisp = 40; /* オンライン系一覧件数 .*/
        public static int MaxCntPrint = 120; /* 帳票系一覧件数 .*/
        public static int Preview = 1; /* 1:ﾌﾟﾚﾋﾞｭｰあり,0:ﾌﾟﾚﾋﾞｭｰなし .*/
        public static string FormTitleFontKind = "ＭＳ Ｐ明朝";
        public static int FormTitleFontFace = 3;
        public static string MenuXml = "menu_user01.xml"; /* メニュー用XMLファイル */
        public static string SdMenuXml = "menu_sd_user00.xml";	/* メニュー用XMLファイル(mobile版) */
        public static int ReadmeFlg = 1; /* Readme参照フラグ2 0=表示, 1=非表示 .*/
        public static int MaxZaikoNissu = 100; /* 最大在庫日数 .*/

        public static int PassLength = 8; /* パスワード文字数下限値 */
        public static int MaxErrCnt = 4; /* ログイン制御回数 */

        /* 初期設定フラグレコード */
        public static Config config = new Config();
        public static int ImpDateDiff = 7; /* 範囲指定時に補正する日付の日数 .*/
        public static int LoginDialogFlag = 1;
        public static int VerClear = 0; /* 起動時にキャッシュクリアしたかどうか .*/

        public static int ComboListFLg = 0; /* BtListのOnTouchでの一覧表示FLG 1:ダイアログ表示 */
        public static int ComboListFlg2 = 0; /* ComboKey押下時はコンボボックス内で一覧表示 */
        public static int ComboListFlg3 = 0; /* 伝票内での一覧表示 */
        public static string BizUrl = string.Empty;

        public static Array HelfDef;
        public static Array MstDialog; /* マスタ取得用専用ダイアログ保存 */
        public static int TanaFlg = 0; /* 棚卸の基準　0:月次、1:棚卸日指定 */
        public static int PosFlg = 0; /* POSの実行　0:POS連携なし、1:TEC 、2:三谷*/
        public static int YosanFlg = 0; /* 0 店別ブランド別の予算、1 日別の予算*/
        public static int YosanFlg2 = 0; /* 0 予算表 ブランド項目なし 1 予算表ブランド項目あり*/
        public static int JdaihenFlg = 0; /* 0 色サイズ展開無し、1 色サイズ展開あり */

        public static string[] OrgMenuSub; 	/* オリジナルサブメニュー定義 .*/
        /* CRSを起動する場合には必ずこのエントリに登録し、かつユーザ毎の起動設定をしなければならない */
        public static OrgMenuDef orgMenuDef = new OrgMenuDef();

        /// <summary>
        /// ■関数 GetSqlDisp = 最大取得件数を制限したSQL文を返す(表示用)
        /// </summary>
        /// <param name="p_querystr">引数1:I	String = 元のSQL文</param>
        /// <param name="p_line">引数2:I	Number = 最大行</param>
        /// <returns>戻値 String = 件数制限されたSQL文</returns>
        public static string GetSqlDisp(string p_querystr, int p_line = 0)
        {
            string ret_sqlstrwrk = "select * from (" + p_querystr + ") where rownum<=" + ((p_line == 0) ? cvnet.MaxCntDisp : p_line);
            return ret_sqlstrwrk;
        }

        /// <summary>
        /// ■関数 AspxSqlQueryMst = マスター関係の問い合わせを行う
        /// </summary>
        /// <param name="p_kubun">引数1:I String = マスター区分名</param>
        /// <param name="v_para">引数2:I Array = ﾊﾟﾗﾒｰﾀ配列 (開始CDなど)</param>
        /// <param name="v_para2">引数4:I Number = ダイアログ表示用</param>
        /// <param name="v_flg">戻値 CSVデータ(タイトル名設定済)</param>
        public static DataTable AspxSqlQueryMst(string p_kubun, string[] v_para = null, string[] v_para2 = null, int v_flg = 0)
        {
            string sql_query = string.Empty;
            DataTable ret_csv = new DataTable();
            string v_retu = string.Empty;
            /* 協和バッグ */
            int v_next = 0;

            if (UserFlg == 56)
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
                sql_query += " where 社員CD>=:1 and 就業FLG='0' order by 社員CD";
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
                if (ClassSatoo.LoginKubun == 1)
                {
                    sql_query += " and メーカーCD='" + ClassSatoo.SHAIN_CD + "'";
                }
                /* ｴｽﾗｸﾞｼﾞｭｰﾙのみ */
                if (config.UserFlg == 56) sql_query += " and 承認FLG=1";

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
                if (ClassSatoo.LoginKubun == 1)
                {
                    sql_query += " and メーカーCD='" + ClassSatoo.SHAIN_CD + "'";
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
                sql_query += " where 名称区分='" + v_para2 + "' order by 名称区分,名称CD";
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
                sql_query += " where 名称区分='" + v_para2 + "' order by 名称区分,名称CD";
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
                sql_query += " where 名称区分='" + v_para2 + "' order by 名称区分,名称CD";
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
        /// <returns>戻値 検索用SQL文字列</returns>
        public static string GetQueryStrHoujin(string arias = "")
        {
            string col_str = string.Empty;
            if (config.MultiCoop != null && config.MultiCoop >= 0)
            {
                col_str += " and ( ";
                if (arias != "" || arias != null) col_str += arias + ".";
                col_str += "法人CD='" + config.MultiCoop + "' ";
                col_str += " or ";
                if (arias != "" || arias != null) col_str += arias + ".";
                col_str += "法人CD='.') ";
            }
            return col_str;
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
}
