using CvnetClient.Class;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System.Data;
using System.IO;
using System.Windows.Forms;

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
        public SysMstTb SysMst; /* システムﾏｽﾀDATA .*/
        public DataTable SysTax; /* システム消費税DATA .*/
        public DataTable SysCnt; /* システム件数DATA .*/
        public DataTable SysImp; /* 入力用初期値データ .*/
        public DataTable SysMeisho;	/* 名称CD用CSVデータ */
        public DataTable SysHhtMst; /* ハンディ用CSVデータ .*/
        public SysKintaiMstTb SysKintaiMst; /* 勤怠管理用CSVデータ .*/
        public HHTCsvTb HHT_Csv = new HHTCsvTb(); /* ハンディ用データ .*/

        public int ComboListFLg = 0; /* BtListのOnTouchでの一覧表示FLG 1:ダイアログ表示 */
        public int ComboListFlg2 = 0; /* ComboKey押下時はコンボボックス内で一覧表示 */
        public int ComboListFlg3 = 0; /* 伝票内での一覧表示 */
        public string BizUrl = string.Empty;

        public Dictionary<string, string> HelpDef;
        public Dictionary<string, MstItem> MstDialog; /* マスタ取得用専用ダイアログ保存 */
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
            MstDialog = new Dictionary<string, MstItem> {
                { "商品",
                    new MstItem {
                        v_mstname = typeof(SubDlgSelShoView).FullName ?? string.Empty,
                        v_para = null
                    }
                },
                { "得意先",
                    new MstItem {
                        v_mstname = typeof(SubDlgSelTokView).FullName ?? string.Empty,
                        v_para = new string[] { "店種区分 between 1 and 3" } 
                    }
                },
                { "Ac得意先", 
                    new MstItem { 
                        v_mstname = typeof(SubDlgSelTokView).FullName ?? string.Empty,
                        v_para = new string[] { "店種区分 between 1 and 3 and 出荷停止FLG=0" }
                    } 
                },
                { "全得意先", 
                    new MstItem { 
                        v_mstname = typeof(SubDlgSelTokView).FullName ?? string.Empty, 
                        v_para = new string[] { "出荷停止FLG=0" } 
                    } 
                },
                { "店舗", 
                    new MstItem { 
                        v_mstname = typeof(SubDlgSelTokView).FullName ?? string.Empty, 
                        v_para = new string[] { "((店種区分=1 AND 在庫管理FLG=1) OR 店種区分 between 3 and 8)" } 
                    } 
                },
                { "Ac店舗", 
                    new MstItem { 
                        v_mstname = typeof(SubDlgSelTokView).FullName ?? string.Empty, 
                        v_para = new string[] { "((店種区分=1 AND 在庫管理FLG=1) OR 店種区分 between 3 and 8) and 出荷停止FLG=0" } 
                    } 
                },
                { "勤怠店舗", 
                    new MstItem { 
                        v_mstname = typeof(SubDlgSelTokView).FullName ?? string.Empty, 
                        v_para = new string[] { "店種区分 IN (0,3,6,9)" } 
                    } 
                },
                { "倉庫", 
                    new MstItem { 
                        v_mstname = typeof(SubDlgSelTokView).FullName ?? string.Empty, 
                        v_para = new string[] { "(店種区分=0 OR 倉庫区分=9)" } 
                    } 
                },
                { "Ac倉庫", 
                    new MstItem { 
                        v_mstname = typeof(SubDlgSelTokView).FullName ?? string.Empty, 
                        v_para = new string[] { "(店種区分=0 OR 倉庫区分=9) and 出荷停止FLG=0" } 
                    } 
                },
                { "倉庫2", 
                    new MstItem { 
                        v_mstname = typeof(SubDlgSelTokView).FullName ?? string.Empty, 
                        v_para = new string[] { "店種区分=0 and 倉庫区分 in (2,3,4,6,7,8)", 
                                                "名称CD01=:1" } 
                    } 
                },
                { "Ac倉庫2", 
                    new MstItem { 
                        v_mstname = typeof(SubDlgSelTokView).FullName ?? string.Empty, 
                        v_para = new string[] { "店種区分=0 and 倉庫区分 in (2,3,4,6,7,8)",
                                                "名称CD01=:1 and 出荷停止FLG=0" } 
                    } 
                },
                { "仕入先", 
                    new MstItem { 
                        v_mstname = typeof(SubDlgSelSirView).FullName ?? string.Empty, 
                        v_para = null 
                    } 
                },
                { "請求", 
                    new MstItem { 
                        v_mstname = typeof(SubDlgSelTokView).FullName ?? string.Empty, 
                        v_para = new string[] { "(得意先CD=請求先CD or 請求先CD='.')",
                                                "出荷停止FLG=0 and ( (締日=:1 and 入金予定日!='0') or (締日2=:2 and 入金予定日2!='0') or (締日3=:3 and 入金予定日3!='0') )" } 
                    }
                },
                { "Ac請求",
                    new MstItem {
                        v_mstname = typeof(SubDlgSelTokView).FullName ?? string.Empty,
                        v_para = new string[] { "(得意先CD=請求先CD or 請求先CD='.')",
                                                "締日=:1 and 出荷停止FLG=0" }
                    }
                },
                { "請求先登録",
                    new MstItem {
                        v_mstname = typeof(SubDlgSelTokView).FullName ?? string.Empty,
                        v_para = new string[] { "(得意先CD=請求先CD or 請求先CD='.')",
                                                "得意先CD=:1" }
                    }
                },
                { "支払",
                    new MstItem {
                        v_mstname = typeof(SubDlgSelTokView).FullName ?? string.Empty,
                        v_para = new string[] { "(仕入先CD=支払先CD or 支払先CD='.')",
                                                "締日=:1" }
                    }
                },
                { "支払先登録",
                    new MstItem {
                        v_mstname = typeof(SubDlgSelSirView).FullName ?? string.Empty,
                        v_para = new string[] { "(仕入先CD=支払先CD or 支払先CD='.')",
                                                "仕入先CD=:1" }
                    }
                },
                { "Ac移動倉庫",
                    new MstItem {
                        v_mstname = typeof(SubDlgSelTokView).FullName ?? string.Empty,
                        v_para = new string[] { "店種区分<9 and 出荷停止FLG=0" }
                    }
                },
                { "移動倉庫",
                    new MstItem {
                        v_mstname = typeof(SubDlgSelTokView).FullName ?? string.Empty,
                        v_para = new string[] { "店種区分<9" }
                    }
                },
                { "Ac取置倉庫",
                    new MstItem {
                        v_mstname = typeof(SubDlgSelTokView).FullName ?? string.Empty,
                        v_para = new string[] { "在庫管理FLG=1 and 店種区分=0 and 倉庫区分=7 and 出荷停止FLG=0" }
                    }
                },
                { "取置倉庫",
                    new MstItem {
                        v_mstname = typeof(SubDlgSelTokView).FullName ?? string.Empty,
                        v_para = new string[] { "在庫管理FLG=1 and 店種区分=0 and 倉庫区分=7" }
                    }
                },
                { "担当",
                    new MstItem {
                        v_mstname = typeof(SubDlgSelUsrView).FullName ?? string.Empty,
                        v_para = new string[] { "就業FLG='0'" }
                    }
                },
                { "営業担当",
                    new MstItem {
                        v_mstname = typeof(SubDlgSelUsrView).FullName ?? string.Empty,
                        v_para = new string[] { "営業FLG=1 and 就業FLG='0'" }
                    }
                },
                { "顧客",
                    new MstItem {
                        v_mstname = typeof(SubDlgSelKokyakuView).FullName ?? string.Empty,
                        v_para = null
                    }
                },
                { "ポイント",
                    new MstItem {
                        v_mstname = typeof(SubDlgSelPointView).FullName ?? string.Empty,
                        v_para = null
                    }
                },
                { "請求単位得意先",
                    new MstItem {
                        v_mstname = typeof(SubDlgSelTokView).FullName ?? string.Empty,
                        v_para = new string[] { "店種区分 between 1 and 3 and 消費税計算方法=0" }
                    }
                },
                { "支払単位仕入先",
                    new MstItem {
                        v_mstname = typeof(SubDlgSelSirView).FullName ?? string.Empty,
                        v_para = new string[] { "消費税計算方法=0" }
                    }
                }
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
             
            MstDialog.Add("siire", new MstItem { v_mstname = typeof(SubDlgSelSirView).FullName ?? string.Empty, v_para = null });
            MstDialog.Add("tokui", new MstItem { v_mstname = typeof(SubDlgSelTokView).FullName ?? string.Empty, v_para = null });
            MstDialog.Add("社員", new MstItem { v_mstname = typeof(SubDlgSelUsrView).FullName ?? string.Empty, v_para = null });
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
        /// ■関数 GetSqlList = 最大取得件数を制限したSQL文を返す(印刷用)
        /// </summary>
        /// <param name="p_querystr">元のSQL文</param>
        /// <returns>件数制限されたSQL文</returns>
        public string GetSqlList(string p_querystr)
        {
            string ret_sqlstrwrk = "select * from (" + p_querystr + ") where rownum<=" + AppData.ClassCvnet.MaxCntPrint.ToString();
            return ret_sqlstrwrk;
        }

        /// <summary>
        /// ■関数 GetSysTax = 消費税率を求める
        /// </summary>
        /// <param name="v_no">消費税NO</param>
        /// <param name="v_date">日付</param>
        /// <returns>消費税率(%)</returns>
        public decimal GetSysTax(int v_no, DateTime v_date)
        {
            var sysTax = AppData.ClassCvnet.SysTax; // DataTable (消費税マスタ)

            // Invalid tax number
            if (v_no <= 0 || v_no > sysTax.Rows.Count)
            {
                return 0m;
            }

            DataRow row = sysTax.Rows[v_no - 1];

            // Parse effective date
            DateTime d_start;
            DateTime.TryParse(row[5].ToString(), out d_start);

            DateTime d_first = new DateTime(1901, 1, 1);
            DateTime d_now = v_date;

            decimal ret_val;

            if (d_start <= d_first || d_now < d_start)
            {
                // Use old tax rate (col 4)
                decimal.TryParse(row[4].ToString(), out ret_val);
            }
            else
            {
                // Use new tax rate (col 6)
                decimal.TryParse(row[6].ToString(), out ret_val);
            }

            return ret_val;
        }

        /// <summary>
        /// ■関数 GetSysWeek = 指定日が含まれる週先頭日付を求める
        /// </summary>
        /// <param name="v_date">日付</param>
        /// <returns>週先頭日</returns>
        public DateTime GetSysWeek(DateTime v_date)
        {
            // Get week start type from SysMst (0 = Sunday start, else = Monday start)
            string weekStartFlag = AppData.ClassCvnet.SysMst._data.Rows[0][18].ToString();

            // C# DayOfWeek: Sunday=0, Monday=1, ..., Saturday=6
            int dayOfWeek = (int)v_date.DayOfWeek;
            int v_day;

            if (weekStartFlag == "0") // Sunday start
            {
                v_day = dayOfWeek;
            }
            else // Monday start
            {
                v_day = (dayOfWeek == 6) ? 0 : dayOfWeek + 1;
            }

            return v_date.AddDays(-v_day);
        }

        /// <summary>
        /// 丸め処理
        /// </summary>
        /// <param name="v_kingaku">対象数値</param>
        /// <param name="v_keta">丸め桁 (-6 ～ 6)</param>
        /// <param name="v_hasu">端数処理方式 (0=四捨五入, 1=切り上げ, 2=切り捨て)</param>
        /// <returns>計算結果数値</returns>
        public static decimal GetRound(decimal v_kingaku, int v_keta, int v_hasu)
        {
            if (v_keta < -6 || v_keta > 6) return v_kingaku;

            // 10^(-keta)
            decimal factor = (decimal)Math.Pow(10, -v_keta);

            decimal result;
            switch (v_hasu)
            {
                case 1: // roundup
                    result = Math.Ceiling(v_kingaku * factor) / factor;
                    break;
                case 2: // rounddown
                    result = Math.Floor(v_kingaku * factor) / factor;
                    break;
                default: // normal round
                    result = Math.Round(v_kingaku, -v_keta, MidpointRounding.AwayFromZero);
                    break;
            }

            return result;
        }

        /// <summary>
        /// ■関数 CheckImpDate = 入力日が正当かチェックする(開始日、先付、修正)
        /// </summary>
        /// <param name="v_date">対象日付</param>
        /// <returns>0:正常, -1:エラー</returns> 
        public int CheckImpDate(DateTime v_date)
        {
            // 開始日
            var date = AppData.ClassCvnet.SysMst._data.Rows[0][17].ToString();
            var v_start = DateTime.Parse(date);
            if (v_date < v_start) return -1;

            // v_day = 入力日 - 今日
            int v_day = (v_date - DateTime.Today).Days;

            // 許容範囲
            int v_pre = int.Parse(AppData.ClassCvnet.SysMst._data.Rows[0][15].ToString());  // 過去許容
            int v_next = int.Parse(AppData.ClassCvnet.SysMst._data.Rows[0][16].ToString()); // 未来許容

            if (v_day > 0) // future date
            {
                if (v_day > v_pre) return -1;
            }
            else if (v_day < 0) // past date
            {
                if (Math.Abs(v_day) > v_next) return -1;
            }

            // フラグ
            int v_flg = int.Parse(AppData.ClassCvnet.SysMst._data.Rows[0][42].ToString());
             
            int _get_sime = 0;
            int.TryParse(AppData.ClassCvnet.SysMst.GetSime(), out _get_sime);

            string v_firstStr = AppData.ClassSatoo.GetDateVal4(DateTime.Today, _get_sime, 0);
            DateTime v_first;
            if (!DateTime.TryParseExact(v_firstStr, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out v_first))
            {
                throw new FormatException($"Invalid date format from GetDateVal4: {v_firstStr}");
            }

            if (v_flg == 1)
            {
                if (v_first > v_date) return -1;
            }

            return 0;
        }

        /// <summary>
        /// ■関数 AspxSqlQuerySysMst = 勤怠管理マスタの問い合わせを行う
        /// </summary>
        public void AspxSqlQuerySysKintaiMst()
        {
            string sqlstr = "select * from HC$Master_KINTAI_KANRI";
            var wrk_csv = AppData.Http?.AspxSqlQuery(sqlstr);
            if (wrk_csv != null)
            {
                AppData.ClassCvnet.SysKintaiMst._data.Clear(); 
                AppData.ClassCvnet.SysKintaiMst._data = wrk_csv; 
            }
        }

        /// <summary>
        /// ■関数 AspxSqlQuerySysHHTMst = ハンディ用マスタの問い合わせを行う 
        /// </summary>
        public void AspxSqlQuerySysHHTMst()
        {
            string sqlstr = "select * from HC$MASTER_HHT_KANRI";
            var wrk_csv = AppData.Http?.AspxSqlQuery(sqlstr);
            if (wrk_csv == null) return;
            AppData.ClassCvnet.SysHhtMst = wrk_csv;
            /* バルカン対応 */ 
            if (wrk_csv?.Rows.Count > 0)
            {
                if (wrk_csv.Rows[0][33].ToString() == "1")
                {
                    var HHT_Csv = AppData.ClassCvnet.HHT_Csv;
                    HHT_Csv._data.Clear();
                    HHT_Csv._data = new DataTable("HHT_Csv");

                    HHT_Csv._data.Columns.Add("Flag", typeof(int));
                    HHT_Csv._data.Columns.Add("Name", typeof(string));
                    HHT_Csv._data.Columns.Add("PathPattern", typeof(string));
                    HHT_Csv._data.Columns.Add("Directory", typeof(string));
                    HHT_Csv._data.Columns.Add("FileNamePattern", typeof(string));

                    HHT_Csv._data.Rows.Add(0, "マスタ", "hht/hksnds1", "hht/", "hksnds1");
                    HHT_Csv._data.Rows.Add(1, "ハンディデータ", "hht/HKALLS1", "hht/", "HKALLS1");
                }
            }
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

            /* 専門店  08.10.09 */
            if (AppData.ClassCvnet.config.smtflg == 1)
            {
                if (p_kubun == "移動倉庫")
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
                    sql_query += " where 得意先CD>=:1 and 在庫管理FLG=1 and 店種区分<9 and 出荷停止FLG=0 order by 得意先CD";
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
                    sql_query += " where 得意先CD>=:1 and 店種区分 between 3 and 8 and 出荷停止FLG=0 order by 得意先CD";
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
                    sql_query += " where 仕入先CD>=:1 and 発注停止FLG=0 order by 仕入先CD";
                    sql_query = GetSqlDisp(sql_query);
                    ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
                }
                else if (p_kubun == "大分類")
                {
                    if (v_flg == 1)
                    {
                        v_retu = "名称CD 大分類CD,名称 大分類名";
                    }
                    else
                    {
                        v_retu = "名称CD||' '||名称 一覧";
                    }
                    sql_query = "select " + v_retu + " from HC$MASTER_MEISHO ";
                    sql_query += " where 名称区分='BN0'";
                    sql_query += " order by 名称区分,名称CD";
                    ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
                }
                else if (p_kubun == "中分類")
                {
                    var bun_csv = AppData.Http?.AspxSqlQuery("select count(*) from HC$MASTER_CONVERT where 区分='BNR'", null);
                    if (v_flg == 1)
                    {
                        v_retu = "a.名称CD 中分類CD,a.名称 中分類名";
                    }
                    else
                    {
                        v_retu = "a.名称CD||' '||a.名称 一覧";
                    }
                    sql_query = "select " + v_retu + " from HC$MASTER_MEISHO a ";
                    sql_query += " where a.名称区分='BN1'";
                    int bun_count = 0;
                    int.TryParse(bun_csv?.Rows[0][0].ToString(), out bun_count);
                    if (v_para2 != null && bun_count != 0)
                    {
                        sql_query += " and exists (select b.一意CD01 from HC$MASTER_CONVERT b where b.一意CD01='" + v_para2[0] + "'";
                        sql_query += " and a.名称cd=b.一意CD02)";
                    }
                    sql_query += " order by 名称区分,名称CD";
                    ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
                }
                else if (p_kubun == "小分類")
                {
                    var bun_csv = AppData.Http?.AspxSqlQuery("select count(*) from HC$MASTER_CONVERT where 区分='BNR'", null);
                    if (v_flg == 1)
                    {
                        v_retu = "a.名称CD 小分類CD,a.名称 小分類名";
                    }
                    else
                    {
                        v_retu = "a.名称CD||' '||a.名称 一覧";
                    }
                    sql_query = "select " + v_retu + " from HC$MASTER_MEISHO a ";
                    sql_query += " where a.名称区分='BN2'";
                    int bun_count = 0;
                    int.TryParse(bun_csv?.Rows[0][0].ToString(), out bun_count);
                    if (v_para2 != null && bun_count != 0)
                    {
                        sql_query += " and exists (select b.一意CD01 from HC$MASTER_CONVERT b where b.一意CD01='" + v_para2[0] + "' and b.一意CD02='" + v_para2[1] + "'";
                        sql_query += " and a.名称cd=b.一意CD03)";
                    }
                    sql_query += " order by 名称区分,名称CD";
                    ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
                }
                else { v_next = 1; }

                if (v_next == 0)
                {
                    if (ret_csv?.Rows.Count > 0) {
                        string firstCell = ret_csv.Rows[0][0]?.ToString() ?? string.Empty;
                        if (firstCell.Contains("null", StringComparison.OrdinalIgnoreCase))
                        {
                            ret_csv.Rows[0].Delete();
                            ret_csv.AcceptChanges(); 
                        }

                    }
                    return ret_csv;
                }
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
        /// 登録された各初期フラグの呼出
        /// </summary>
        /// <returns>CSVデータ(0列目=seq, 1列目=v_cr, 3列目=v_up, 4列目=カテゴリ, 5列目=フラグ名, 6列目=値, 7列目=リストボックス使用文字列, 8列目=注釈)</returns>
        public DataTable AspxSqlQueryConfig()
        {
            string sql_str0 = "select * from HC$master_config order by カテゴリ,フラグ名";
            var ret_csv0 = AppData.Http?.AspxSqlQuery(sql_str0);
            if (ret_csv0?.Rows.Count > 0)
            {
                var jan_ar = new BizArray();
                var reg_ar = new BizArray();
                var bar_ar = new BizArray();
                var mon_ar = new BizArray();

                foreach (DataRow row in ret_csv0.Rows)
                {
                    string row4 = row[4].ToString() ?? string.Empty;
                    string row5 = row[5].ToString() ?? string.Empty;
                    var conf = AppData.ClassCvnet.config.FindChild(row4);
                    if (conf != null) AppData.ClassCvnet.SetChild(row4, row5);
                    else if (row4.Contains("janpattern", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!string.IsNullOrEmpty(row5)) jan_ar.Add(row5);
                    }
                    else if (row4.Contains("Regaxchk", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!string.IsNullOrEmpty(row5)) reg_ar.Add(row5);
                    }
                    else if (row4.Contains("barpattern", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!string.IsNullOrEmpty(row5)) bar_ar.Add(row5);
                    }
                    else if (row4.Contains("ManageMonthly", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!string.IsNullOrEmpty(row5)) mon_ar.Add(row5);
                    }
                    var cvcnf = AppData.ClassCvnet.FindChild(row4);
                    if (cvcnf != null) AppData.ClassCvnet.SetChild(row4, row5);
                    else if (row4.Contains("HelpDef", StringComparison.OrdinalIgnoreCase))
                    {
                        if (AppData.ClassCvnet.HelpDef == null) AppData.ClassCvnet.HelpDef = new Dictionary<string, string>();
                        string hlp = row4.Substring(row4.Length - 2);
                        AppData.ClassCvnet.HelpDef.Add(hlp, string.IsNullOrEmpty(row5) ? "" : row5);
                    }
                }
                var conf1 = AppData.ClassCvnet.config.FindChild("janpattern");
                if (conf1 != null)
                {
                    conf1 = string.Empty;
                    for (var i = 0; i < jan_ar.Count; i++)
                    {
                        conf1 += ((i == 0) ? "" : "\n") + jan_ar[i];
                    }
                    AppData.ClassCvnet.config.SetChild("janpattern", conf1);
                }
                var conf2 = AppData.ClassCvnet.config.FindChild("Regaxchk");
                if (conf2 != null)
                {
                    conf2 = string.Empty;
                    for (var i = 0; i < reg_ar.Count; i++)
                    {
                        conf2 += ((i == 0) ? "" : "\n") + reg_ar[i];
                    }
                    AppData.ClassCvnet.config.SetChild("janpattern", conf2);
                }
                var conf3 = AppData.ClassCvnet.config.FindChild("barpattern");
                if (conf3 != null)
                {
                    conf3 = string.Empty;
                    for (var i = 0; i < bar_ar.Count; i++)
                    {
                        conf3 += ((i == 0) ? "" : "\n") + bar_ar[i];
                    }
                    AppData.ClassCvnet.config.SetChild("barpattern", conf3);
                }
                var conf4 = AppData.ClassCvnet.config.FindChild("ManageMonthly");
                if (conf4 != null)
                {
                    conf4 = string.Empty;
                    for (var i = 0; i < mon_ar.Count; i++)
                    {
                        conf4 += ((i == 0) ? "" : "\n") + mon_ar[i];
                    }
                    AppData.ClassCvnet.config.SetChild("ManageMonthly", conf4);
                }
                /* 初期値を再セットする */
                AppData.ClassCvnet.ImpDateDiff = AppData.ClassCvnet.config.DefDateRange;
                /* PDF出力FLGを再セットする */
                AppData.ClassSatoo.PrintPDFFlg = AppData.ClassCvnet.config.PrintPDFFlg;
            }
            return ret_csv0;
        }

        /// <summary>
        /// ■関数 AspxSqlQueryImp = 入力業務前の初期値の取得
        /// </summary>
        /// <returns>CSVデータ(1列目=0,倉庫CD,倉庫名, 2列目=1,店舗CD,店舗名,在管FLG,店種)</returns>
        public DataTable AspxSqlQueryImp()
        {
            if (AppData.ClassCvnet.SysImp.Rows.Count > 0) return AppData.ClassCvnet.SysImp;
            string sql_str0 = "select '0' 区分,得意先CD,得意先名,在庫管理FLG,店種区分,名称CD01,NVL((SELECT 名称 FROM HC$MASTER_MEISHO WHERE 名称区分='C01' AND 名称CD=名称CD01),'') 名称01 from HC$master_tokui where 得意先CD=:1";
            sql_str0 += " union ";
            sql_str0 += "select '1' 区分,得意先CD,得意先名,在庫管理FLG,店種区分,名称CD01,NVL((SELECT 名称 FROM HC$MASTER_MEISHO WHERE 名称区分='C01' AND 名称CD=名称CD01),'') 名称01 from HC$master_tokui where 得意先CD=:2";
            sql_str0 += " union ";
            sql_str0 += "select '2' 区分,得意先CD,得意先名,在庫管理FLG,店種区分,名称CD01,NVL((SELECT 名称 FROM HC$MASTER_MEISHO WHERE 名称区分='C01' AND 名称CD=名称CD01),'') 名称01 from HC$master_tokui where 得意先CD='00000001'";
            var para0 = new BizArray();
            para0.Set(0, AppData.ClassCvnet.SysMst._data.Rows[0][21].ToString() ?? string.Empty);
            para0.Set(1, AppData.ClassSatoo.SHAIN_Tenpo);
            var ret_csv0 = AppData.Http?.AspxSqlQuery(sql_str0, para0.ToArray());
            if (ret_csv0?.Rows.Count > 0)
            {
                ret_csv0.Rows.Add(ret_csv0.NewRow());
                ret_csv0.Rows.Add(ret_csv0.NewRow());
                ret_csv0.Rows.Add(ret_csv0.NewRow());
            }
            else if (ret_csv0?.Rows.Count == 1)
            {
                if (ret_csv0.Rows[0][0].ToString() == "0")
                {
                    ret_csv0.Rows.Add(ret_csv0.NewRow());
                    ret_csv0.Rows.Add(ret_csv0.NewRow());
                }
                else if (ret_csv0.Rows[0][0].ToString() == "1")
                {
                    ret_csv0.Rows.InsertAt(ret_csv0.NewRow(), 0);
                    ret_csv0.Rows.Add(ret_csv0.NewRow());
                }
                else
                {
                    ret_csv0.Rows.InsertAt(ret_csv0.NewRow(), 0);
                    ret_csv0.Rows.InsertAt(ret_csv0.NewRow(), 0);
                }
            }
            else if (ret_csv0?.Rows.Count == 2)
            {
                string kubun0 = ret_csv0.Rows[0][0].ToString() ?? string.Empty;
                if (kubun0 == "0")
                {
                    string kubun1 = ret_csv0.Rows[1][0].ToString() ?? string.Empty;
                    if (kubun1 == "1")
                    {
                        ret_csv0.Rows.Add(ret_csv0.NewRow()); 
                    }
                    else
                    {
                        ret_csv0.Rows.InsertAt(ret_csv0.NewRow(), 1); 
                    }
                }
                else
                {
                    ret_csv0.Rows.InsertAt(ret_csv0.NewRow(), 0);
                }
            }
            AppData.ClassCvnet.SysImp = ret_csv0;
            return AppData.ClassCvnet.SysImp;
        }

        /// <summary>
        /// ■関数 AspxSqlQueryImp = 入力用名称マスタの一括取得
        /// 戻値		CSVデータ(名称区分,名称CD,名称,略称)
        /// </summary>
        public DataTable AspxSqlQuerySysMeisho()
        {
            if (AppData.ClassCvnet.SysMeisho.Rows.Count > 0) 
                return AppData.ClassCvnet.SysMeisho;
            string sql_query = "select 名称CD||' '||名称 一覧,名称区分,名称CD,名称,略称  from HC$MASTER_MEISHO ";
            sql_query += " where 名称区分 between 'B01' and 'B10' or 名称区分 in ('SZN','GEN') order by 名称区分,名称CD";
            AppData.ClassCvnet.SysMeisho = AppData.Http?.AspxSqlQuery(sql_query);
            return AppData.ClassCvnet.SysMeisho ?? new DataTable();
        }

        /// <summary>
        /// ■関数 AspxSqlQuerySysMst = システム管理マスタ、消費税マスタの問い合わせを行う
        /// 戻値		なし
        /// </summary>
        public void AspxSqlQuerySysMst()
        {
            string sqlstr = "select * from HC$Master_SYSKANRI";
            SysMst = new SysMstTb();
            SysMst._data = AppData.Http?.AspxSqlQuery(sqlstr);
            sqlstr = "select * from HC$Master_SYSTAX";
            SysTax = AppData.Http?.AspxSqlQuery(sqlstr);
            if (SysMst._data?.Rows.Count > 0 && SysMst._data?.Columns.Count > 15)
            {
                int _impDateDiff = 0;
                int.TryParse(SysMst._data.Rows[0][15].ToString(), out _impDateDiff);
                AppData.ClassCvnet.ImpDateDiff = _impDateDiff;
            }
            /* 初期値はDefDateとする */
            AppData.ClassCvnet.ImpDateDiff = AppData.ClassCvnet.config.DefDateRange;
        }

        /// <summary> 
        /// </summary>
        /// <param name="shainTenpo">入力業務前の初期値の取得</param>
        /// <returns>CSVデータ(1列目=0,倉庫CD,倉庫名, 2列目=1,店舗CD,店舗名,在管FLG,店種)</returns>
        public DataTable AspxSqlQueryImp(string shainTenpo)
        { 
            if (AppData.ClassCvnet.SysImp.Rows.Count > 0) return AppData.ClassCvnet.SysImp;
            string sql_str0 = "select '0' 区分,得意先CD,得意先名,在庫管理FLG,店種区分,名称CD01,NVL((SELECT 名称 FROM HC$MASTER_MEISHO WHERE 名称区分='C01' AND 名称CD=名称CD01),'') 名称01 from HC$master_tokui where 得意先CD=:1";
            sql_str0 += " union ";
            sql_str0 += "select '1' 区分,得意先CD,得意先名,在庫管理FLG,店種区分,名称CD01,NVL((SELECT 名称 FROM HC$MASTER_MEISHO WHERE 名称区分='C01' AND 名称CD=名称CD01),'') 名称01 from HC$master_tokui where 得意先CD=:2";
            sql_str0 += " union ";
            sql_str0 += "select '2' 区分,得意先CD,得意先名,在庫管理FLG,店種区分,名称CD01,NVL((SELECT 名称 FROM HC$MASTER_MEISHO WHERE 名称区分='C01' AND 名称CD=名称CD01),'') 名称01 from HC$master_tokui where 得意先CD='00000001'";
            var para0 = new BizArray();
            para0.Set(0, AppData.ClassCvnet.SysMst._data.Rows[0][21].ToString() ?? string.Empty);
            para0.Set(1, AppData.ClassSatoo.SHAIN_Tenpo);
            var ret_csv0 = AppData.Http?.AspxSqlQuery(sql_str0, para0.ToArray());
            if (ret_csv0?.Rows.Count > 0)
            {
                ret_csv0.Rows.Add(ret_csv0.NewRow());
                ret_csv0.Rows.Add(ret_csv0.NewRow());
                ret_csv0.Rows.Add(ret_csv0.NewRow()); 
            }
            else if (ret_csv0.Rows.Count == 1)
            {
                string kubun = ret_csv0.Rows[0][0].ToString();
                if (kubun == "0")
                {
                    ret_csv0.Rows.Add(ret_csv0.NewRow());
                    ret_csv0.Rows.Add(ret_csv0.NewRow());
                }
                else if (kubun == "1")
                {
                    ret_csv0.Rows.InsertAt(ret_csv0.NewRow(), 0);
                    ret_csv0.Rows.Add(ret_csv0.NewRow());
                }
                else
                {
                    ret_csv0.Rows.InsertAt(ret_csv0.NewRow(), 0);
                    ret_csv0.Rows.InsertAt(ret_csv0.NewRow(), 0);
                }
            }
            else if (ret_csv0.Rows.Count == 2)
            {
                string kubun0 = ret_csv0.Rows[0][0].ToString();
                string kubun1 = ret_csv0.Rows[1][0].ToString();

                if (kubun0 == "0")
                {
                    if (kubun1 == "1")
                        ret_csv0.Rows.Add(ret_csv0.NewRow());
                    else
                        ret_csv0.Rows.InsertAt(ret_csv0.NewRow(), 1);
                }
                else
                {
                    ret_csv0.Rows.InsertAt(ret_csv0.NewRow(), 0);
                }
            }
            AppData.ClassCvnet.SysImp = ret_csv0;
            return AppData.ClassCvnet.SysImp;
        } 

        /// <summary>
        /// ■関数 timeconv = 時刻文字列変換
        /// </summary>
        /// <param name="tmp">秒で取得</param>
        /// <returns>時刻文字列</returns>
        public string GetTimeVal(int tmp)
        {
            int hr = tmp / 3600;
            int mi = (tmp % 3600) / 60;
            int sd = tmp % 60;

            return $"{hr}:{mi:D2}:{sd:D2}";
        }

        /// <summary>
        /// ■関数 timeconv2 = 時刻文字列変換
        /// </summary>
        /// <param name="tmp">時刻文字列</param>
        /// <param name="flg">0:秒,1;分</param>
        /// <returns>時間</returns>
        public int GetTimeVal2(string tmp, int flg)
        {
            if (string.IsNullOrWhiteSpace(tmp))
                return 0;

            var parts = tmp.Split(':');
            int se = 0;

            for (int n = 0; n < parts.Length; n++)
            {
                if (int.TryParse(parts[n], out int val))
                {
                    // (2 - n) → 2 for hours, 1 for minutes, 0 for seconds
                    int power = 2 - n;
                    se += val * (int)Math.Pow(60, power);
                }
            }
            if (flg == 1)
            {
                se /= 60; // convert to minutes
            }
            return se;
        }

        /// <summary>
        /// ■関数 timestr = 時刻文字列変換
        /// </summary>
        /// <param name="tmp">時刻を:なしで入力</param>
        /// <returns>
        /// 時刻文字列
        /// エラー時は"err"を返す
        /// </returns>
        public string GetTimeStr(string tmp)
        {
            if (string.IsNullOrEmpty(tmp))
                return string.Empty;

            // pad right with zeros until length is 6
            while (tmp.Length < 6)
            {
                tmp += "0";
            }

            // regex for HHMMSS
            var pattern = new System.Text.RegularExpressions.Regex(@"^([0-2][0-9])([0-5][0-9])([0-5][0-9])$");
            var match = pattern.Match(tmp);

            if (match.Success)
            {
                int hour = int.Parse(match.Groups[1].Value);
                if (hour < 24)
                {
                    return $"{match.Groups[1].Value}:{match.Groups[2].Value}:{match.Groups[3].Value}";
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// ■関数 GetShohin = 商品取得（明細チェック用）
        /// </summary>
        /// <param name="para">商品CD, 在庫計上日</param>
        /// <returns>商品マスタ読込内容</returns>
        public string[] GetShohin(string[] para)
        {
            var v_para = new BizArray();
            v_para.Set(0, para[0]);
            string v_genka = "19010101";
            if (para.Length > 1 && para[1] != null) v_genka = para[1];
            /* 2019.05.17 GET_JODAIの引数用 */
            string v_jodai = "19010101";
            if (para[1] != null) v_jodai = new string(para[1]);
            string sql_str = "select A.商品CD,A.商品名,GET_JODAI(A.商品CD,'.','.','" + v_jodai + "','.') 上代,A.絵型名,A.展示会CD,A.ブランドCD,A.アイテムCD,A.デリバリー日";
            sql_str += ",B.名称 展示会名,C.名称 ブランド名,D.名称 アイテム名,GET_GENKA(A.商品CD,0,'" + v_genka + "') 原価,A.消費税計算方法,上代 マスタ上代 ";
            sql_str += ",A.営業原価, A.店頭投入日";
            /* 仕入値追加 2008.06.25 */
            if (AppData.ClassCvnet.UserFlg == 23 || AppData.ClassCvnet.UserFlg == 20)
            {
                sql_str += ",A.原価 仕入値";
            }
            else if (AppData.ClassCvnet.UserFlg == 75)
            {
                sql_str += ",GET_GENKA(A.商品CD,0,'" + para[1] + "') 仕入値";
            }
            else
            {
                sql_str += ",A.仕入価格 仕入値";
            }
            /* 納品日追加 2008.09.29 */
            sql_str += ",A.納品日";
            sql_str += ",A.メーカー品番";
            sql_str += ",A.仕入区分||' '||decode(A.仕入区分,1,'買取',2,'委託',3,'消化','') 仕入区分";
            sql_str += ((AppData.ClassCvnet.config.tanpin == 1) ? ",A.商品管理FLG" : ",0 商品管理FLG");
            sql_str += ((AppData.ClassCvnet.config.oroshi >= 1) ? ",A.単位" : ",'' 単位");
            sql_str += ",A.商品サイズ区分";    /* サイズ区分追加 2011.05.24 */
            sql_str += " from HC$MASTER_SHOHIN A";
            sql_str += " ,(select 名称CD,名称 from HC$Master_MEISHO where 名称区分='TNJ') B";
            sql_str += " ,(select 名称CD,名称 from HC$Master_MEISHO where 名称区分='BRD') C";
            sql_str += " ,(select 名称CD,名称 from HC$Master_MEISHO where 名称区分='ITM') D";
            sql_str += " where A.商品CD=:1 and (A.展示会CD=B.名称CD(+) and A.ブランドCD=C.名称CD(+) and A.アイテムCD=D.名称CD(+) )";

            /* 協和のみ */
            if (AppData.ClassCvnet.UserFlg == 8)
            {
                sql_str += " and 予備01!='1' ";
            }

            if (para.Length > 1)
            { /* FLG */
                if (para[1] == "0")
                { /* 仕入先限定 */
                    v_para[1] = new string(para[2]);
                    sql_str += " and A.メーカーCD=:2";
                }
            }

            /* ｴｽﾗｸﾞｼﾞｭｰﾙのみ */
            if (AppData.ClassCvnet.config.UserFlg == 56) sql_str += " and A.承認FLG=1";

            sql_str += "  order by 商品CD";
            var ret_para = new BizArray();
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, v_para.ToArray());
            if (ret_csv?.Rows.Count > 0)
            {
                ret_para.Set(0, ret_csv.Rows[0][0].ToString());   /* 商品CD */
                ret_para.Set(1, ret_csv.Rows[0][1].ToString());   /* 商品名 */
                ret_para.Set(2, ret_csv.Rows[0][2].ToString());   /* 上代 */
                ret_para.Set(3, ret_csv.Rows[0][3].ToString());   /* 絵型 */
                ret_para.Set(4, ret_csv.Rows[0][4].ToString());   /* 展示会CD */
                ret_para.Set(5, ret_csv.Rows[0][5].ToString());   /* ブランドCD */
                ret_para.Set(6, ret_csv.Rows[0][6].ToString());   /* アイテムCD */
                ret_para.Set(7, ret_csv.Rows[0][7].ToString());   /* デリバリー日 */
                ret_para.Set(8, ret_csv.Rows[0][11].ToString());  /* 原価 */
                ret_para.Set(9, ret_csv.Rows[0][12].ToString());  /* 消費税計算方法 */
                /* 項目追加 *//* 2007.10.03 営業原価・店頭投入日追加 */
                ret_para.Set(10, ret_csv.Rows[0][5].ToString());  /* ブランドCD */
                ret_para.Set(11, ret_csv.Rows[0][9].ToString());  /* ブランド名 */
                ret_para.Set(12, ret_csv.Rows[0][13].ToString()); /* 元上代 */
                ret_para.Set(13, ret_csv.Rows[0][15].ToString()); /* 店頭投入日 */
                ret_para.Set(14, ret_csv.Rows[0][14].ToString()); /* 営業原価 */
                /* 仕入値追加 2008.06.25 */
                ret_para.Set(15, ret_csv.Rows[0][16].ToString()); /* 仕入値 */
                /* 納品日追加 2008.09.29 */
                ret_para.Set(16, ret_csv.Rows[0][17].ToString()); /* 納品日 */
                /* さらに追加・メーカー品番 20081106 */
                ret_para.Set(17, ret_csv.Rows[0][18].ToString());
                /* さらに追加・仕入区分 20091006 */
                ret_para.Set(18, ret_csv.Rows[0][19].ToString());
                /* さらに追加・商品管理FLG 20100405 */
                ret_para.Set(19, ret_csv.Rows[0][20].ToString());
                /* さらに追加・単位 20100428 */
                ret_para.Set(20, ret_csv.Rows[0][21].ToString());
                /* さらに追加・売単価 20100706 */
                ret_para.Set(21, string.Empty);
                /* さらに追加 副名1・2・3 20100714 */
                ret_para.Set(22, string.Empty);
                ret_para.Set(23, string.Empty);
                ret_para.Set(24, string.Empty);
                ret_para.Set(25, string.Empty);
                ret_para.Set(26, string.Empty);
                ret_para.Set(27, string.Empty);
                /* さらに追加 サイズ区分 20110524 */
                ret_para.Set(28, ret_csv.Rows[0][22].ToString());
            }
            return ret_para.ToArray();
        }

        /// <summary>
        /// EC用仮追加↓
        /// ■関数 GetShohin_ec = EC商品取得（明細チェック用）
        /// </summary>
        /// <param name="para">商品CD,仕入先CD</param>
        /// <returns>EC商品マスタ読込内容</returns>
        public string[] GetShohin_ec(string[] para)
        {
            var v_para = new BizArray();
            v_para.Set(0, para[0]);
            string sql_str = "select A.商品CD,A.商品名,A.上代,A.絵型名,A.展示会CD,A.ブランドCD,A.アイテムCD,A.デリバリー日";
            sql_str += ",B.名称 展示会名,C.名称 ブランド名,D.名称 アイテム名,A.原価,A.消費税計算方法,元上代 ";
            sql_str += " from HC$MASTER_SHOHIN A";
            sql_str += " ,(select 名称CD,名称 from HC$Master_MEISHO where 名称区分='TNJ') B";
            sql_str += " ,(select 名称CD,名称 from HC$Master_MEISHO where 名称区分='BRD') C";
            sql_str += " ,(select 名称CD,名称 from HC$Master_MEISHO where 名称区分='ITM') D";
            sql_str += " ,HC$MASTER_SHOHIN_EC E";
            sql_str += " where A.商品CD=E.商品CD and A.商品CD=:1 and (A.展示会CD=B.名称CD(+) and A.ブランドCD=C.名称CD(+) and A.アイテムCD=D.名称CD(+) )";

            sql_str += "  order by 商品CD";
            var ret_para = new BizArray();
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, v_para.ToArray());
            if (ret_csv?.Rows.Count > 0)
            {
                ret_para.Set(0, ret_csv.Rows[0][0].ToString());   /* 商品CD */
                ret_para.Set(1, ret_csv.Rows[0][1].ToString());   /* 商品名 */
                ret_para.Set(2, ret_csv.Rows[0][2].ToString());   /* 上代 */
                ret_para.Set(3, ret_csv.Rows[0][3].ToString());   /* 絵型 */
                ret_para.Set(4, ret_csv.Rows[0][4].ToString());   /* 展示会CD */
                ret_para.Set(5, ret_csv.Rows[0][5].ToString());   /* ブランドCD */
                ret_para.Set(6, ret_csv.Rows[0][6].ToString());   /* アイテムCD */
                ret_para.Set(7, ret_csv.Rows[0][7].ToString());   /* デリバリー日 */
                ret_para.Set(8, ret_csv.Rows[0][11].ToString());  /* 原価 */
                ret_para.Set(9, ret_csv.Rows[0][12].ToString());  /* 消費税計算方法 */
                /* 項目追加 */
                ret_para.Set(10, ret_csv.Rows[0][5].ToString());  /* ブランドCD */
                ret_para.Set(11, ret_csv.Rows[0][9].ToString());  /* ブランド名 */
                ret_para.Set(12, ret_csv.Rows[0][13].ToString()); /* 元上代 */
            }
            return ret_para.ToArray();
        }

        /// <summary>
        /// ■関数 GetShohinOroshi = 商品取得（卸用）
        /// </summary>
        /// <param name="para">商品CD,FLG,仕入先CD,日付,得意先CD,色CD,サイズCD</param>
        /// <returns>商品マスタ読込内容</returns>
        public string[] GetShohinOroshi(string[] para)
        {
            var v_para = new BizArray();
            v_para.Set(0, para[0]);

            string sql_str = "select A.商品CD,A.商品名";
            sql_str += ((AppData.ClassCvnet.config.oroshi == 1) ? ",get_jodai(A.商品CD,'" + para[5] + "','" + para[6] + "','" + para[3] + "','" + para[4] + "',0) 上代" : ",A.上代");
            sql_str += ",A.絵型名,A.展示会CD,A.ブランドCD,A.アイテムCD,A.デリバリー日";
            sql_str += ",B.名称 展示会名,C.名称 ブランド名,D.名称 アイテム名";
            sql_str += ((AppData.ClassCvnet.config.oroshi == 2) ? ",get_genka(A.商品CD,'0','" + para[3] + "','" + para[5] + "','" + para[6] + "') 原価" : ",get_genka(A.商品CD,'0','" + para[3] + "') 原価");
            sql_str += ",A.消費税計算方法,元上代 ";
            sql_str += ",A.営業原価, A.店頭投入日";
            sql_str += ",A.仕入価格 仕入値";
            sql_str += ",A.納品日";
            sql_str += ",A.メーカー品番";
            sql_str += ",A.仕入区分||' '||decode(A.仕入区分,1,'買取',2,'委託',3,'消化','') 仕入区分";
            sql_str += ((AppData.ClassCvnet.config.tanpin == 1) ? ",A.商品管理FLG" : ",0 商品管理FLG");
            sql_str += ",A.単位";
            sql_str += ((AppData.ClassCvnet.config.oroshi == 1) ? ",get_jodai(A.商品CD,'" + para[5] + "','" + para[6] + "','" + para[3] + "','" + para[4] + "',1) 売単価" : ",0 売単価");
            sql_str += ",A.名称CD01 副名CD1";
            sql_str += ",E.名称 副名1";
            sql_str += ",A.名称CD02 副名CD2";
            sql_str += ",F.名称 副名2";
            sql_str += ",A.名称CD03 副名CD3";
            sql_str += ",G.名称 副名3";
            sql_str += " from HC$MASTER_SHOHIN A";
            sql_str += " ,(select 名称CD,名称 from HC$Master_MEISHO where 名称区分='TNJ') B";
            sql_str += " ,(select 名称CD,名称 from HC$Master_MEISHO where 名称区分='BRD') C";
            sql_str += " ,(select 名称CD,名称 from HC$Master_MEISHO where 名称区分='ITM') D";
            sql_str += " ,(select 名称CD,名称 from HC$Master_MEISHO where 名称区分='B01') E";
            sql_str += " ,(select 名称CD,名称 from HC$Master_MEISHO where 名称区分='B02') F";
            sql_str += " ,(select 名称CD,名称 from HC$Master_MEISHO where 名称区分='B03') G";
            sql_str += " where A.商品CD=:1 and (A.展示会CD=B.名称CD(+) and A.ブランドCD=C.名称CD(+) and A.アイテムCD=D.名称CD(+) and A.名称CD01=E.名称CD(+) and A.名称CD02=F.名称CD(+) and A.名称CD03=G.名称CD(+) )";
            if (para.Length > 1)
            { /* FLG */
                if (para[1].ToString().Trim() == "0")
                { /* 仕入先限定 */
                    v_para[1] = para[2];
                    sql_str += " and A.メーカーCD=:2";
                }
            }
            sql_str += "  order by 商品CD";

            var ret_para = new BizArray();
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, v_para.ToArray());
            if (ret_csv?.Rows.Count > 0)
            {
                ret_para.Set(0, ret_csv.Rows[0][0].ToString());   /* 商品CD */
                ret_para.Set(1, ret_csv.Rows[0][1].ToString());   /* 商品名 */
                ret_para.Set(2, ret_csv.Rows[0][2].ToString());   /* 上代 */
                ret_para.Set(3, ret_csv.Rows[0][3].ToString());   /* 絵型 */
                ret_para.Set(4, ret_csv.Rows[0][4].ToString());   /* 展示会CD */
                ret_para.Set(5, ret_csv.Rows[0][5].ToString());   /* ブランドCD */
                ret_para.Set(6, ret_csv.Rows[0][6].ToString());   /* アイテムCD */
                ret_para.Set(7, ret_csv.Rows[0][7].ToString());   /* デリバリー日 */
                ret_para.Set(8, ret_csv.Rows[0][11].ToString());  /* 原価 */
                ret_para.Set(9, ret_csv.Rows[0][12].ToString());  /* 消費税計算方法 */
                ret_para.Set(10, ret_csv.Rows[0][5].ToString());  /* ブランドCD */
                ret_para.Set(11, ret_csv.Rows[0][9].ToString());  /* ブランド名 */
                ret_para.Set(12, ret_csv.Rows[0][13].ToString()); /* 元上代 */
                ret_para.Set(13, ret_csv.Rows[0][15].ToString()); /* 店頭投入日 */
                ret_para.Set(14, ret_csv.Rows[0][14].ToString()); /* 営業原価 */
                ret_para.Set(15, ret_csv.Rows[0][16].ToString()); /* 仕入値 */
                ret_para.Set(16, ret_csv.Rows[0][17].ToString()); /* 納品日 */
                ret_para.Set(17, ret_csv.Rows[0][18].ToString()); /* A.メーカー品番 */
                ret_para.Set(18, ret_csv.Rows[0][19].ToString()); /* 仕入区分 */
                ret_para.Set(19, ret_csv.Rows[0][20].ToString()); /* 商品管理FLG */
                ret_para.Set(20, ret_csv.Rows[0][21].ToString()); /* 単位 */
                ret_para.Set(21, ret_csv.Rows[0][22].ToString()); /* 売単価 */
                /* さらに追加・副名1・2・3 20100714 */
                ret_para.Set(22, ret_csv.Rows[0][23].ToString());
                ret_para.Set(23, ret_csv.Rows[0][24].ToString());
                ret_para.Set(24, ret_csv.Rows[0][25].ToString());
                ret_para.Set(25, ret_csv.Rows[0][26].ToString());
                ret_para.Set(26, ret_csv.Rows[0][27].ToString());
                ret_para.Set(27, ret_csv.Rows[0][28].ToString());
            }
            return ret_para.ToArray();
        }

        /// <summary>
        /// ■関数 GetCol = 商品色取得（明細チェック用）
        /// </summary>
        /// <param name="para">商品CD,色CD</param>
        /// <returns>商品色サイズ読込内容</returns>
        public string[] GetCol(string[] para)
        {
            var ret_para = new BizArray();
            string sql_str = "Select A.色CD,'',DECODE(A.上代,0,B.上代,A.上代) 上代,B.上代 商品上代";
            sql_str += ",A.商品CD";
            sql_str += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='COL' and H.名称CD=A.色CD),'.') 色名";
            sql_str += ",'' サイズ名";
            sql_str += " From HC$MASTER_SHOHIN_JAN A";
            sql_str += " join HC$master_SHOHIN B on (B.商品CD=A.商品CD)";
            sql_str += " ,HC$MASTER_SHOHIN_EC E";
            sql_str += " where A.商品CD=:1";
            sql_str += " and A.商品CD=E.商品CD";
            if (para.Length > 1) { sql_str += " and A.色CD=:2 and A.色CD=E.色CD"; }
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, para);
            if (ret_csv?.Rows.Count > 0)
            {
                ret_para.Set(0, ret_csv.Rows[0][0].ToString());
                ret_para.Set(1, ret_csv.Rows[0][1].ToString());
                ret_para.Set(2, ret_csv.Rows[0][5].ToString());
                ret_para.Set(3, ret_csv.Rows[0][6].ToString());
                ret_para.Set(4, ret_csv.Rows[0][2].ToString());
            }
            return ret_para.ToArray();
        }

        /// <summary>
        /// ■関数 GetCol = 商品色取得（明細チェック用）
        /// </summary>
        /// <param name="para">商品CD,色CD</param>
        /// <returns>商品色サイズ読込内容</returns>
        public string[] GetCol2(string[] para)
        {
            var ret_para = new BizArray();
            string sql_sub = ",GET_COLORNAME(A.色CD) 色名";
            if (AppData.ClassCvnet.config.ColSizMei == 1) sql_sub = ",MAX(A.色名) 色名";

            string sql_str = ""
            + "SELECT "
                + "A.色CD" + sql_sub + ",MAX(A.商品CD)||' '||MAX(B.商品名) 商品"
            + " FROM "
                + "HC$MASTER_SHOHIN_JAN A,HC$master_SHOHIN B"
            + " WHERE "
                + "A.商品CD=B.商品CD AND A.商品CD=:1 AND A.色CD=:2"
            + " GROUP BY "
                + "A.色CD";

            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, para);
            if (ret_csv?.Rows.Count > 0)
            {
                ret_para.Set(0, ret_csv.Rows[0][0].ToString());
                ret_para.Set(1, ret_csv.Rows[0][1].ToString());
            }
            return ret_para.ToArray();
        }

        /// <summary>
        /// ■関数 GetColSiz = 商品色サイズ取得（明細チェック用）
        /// </summary>
        /// <param name="para">商品CD,色CD</param>
        /// <returns>商品色サイズ読込内容</returns>
        public string[] GetColSiz(string[] para)
        {
            var sql_str = "Select A.色CD,A.サイズCD,DECODE(A.上代,0,B.上代,A.上代) 上代,B.上代 商品上代";
            sql_str += ",A.商品CD";
            sql_str += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='COL' and H.名称CD=A.色CD),'.') 色名";
            sql_str += ",GET_SIZENAME(A.商品CD,A.サイズCD) サイズ名";
            sql_str += " From HC$MASTER_SHOHIN_JAN A";
            sql_str += " join HC$master_SHOHIN B on (B.商品CD=A.商品CD)";
            sql_str += " where A.商品CD=:1";
            if (para.Length > 1) { sql_str += " and A.色CD=:2"; }
            if (para.Length > 2) { sql_str += " and A.サイズCD=:3"; }

            var ret_para = new BizArray();
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, para);
            if (ret_csv?.Rows.Count > 0)
            {
                ret_para.Set(0, ret_csv.Rows[0][0].ToString());
                ret_para.Set(1, ret_csv.Rows[0][1].ToString());
                ret_para.Set(2, ret_csv.Rows[0][5].ToString());
                ret_para.Set(3, ret_csv.Rows[0][6].ToString());
                ret_para.Set(4, ret_csv.Rows[0][2].ToString());
                ret_para.Set(5, ret_csv.Rows[0][2].ToString());
            }
            return ret_para.ToArray();
        }

        /// <summary>
        /// ■関数 GetColSiz87 = 商品色サイズ取得（明細チェック用）	※.ｲﾝｺﾝﾄﾛ専用
        /// </summary>
        /// <param name="para">商品CD,色CD</param>
        /// <returns>商品色サイズ読込内容(海外名)</returns>
        public string[] GetColSiz87(string[] para)
        {
            string sql_str = "Select A.色CD,A.サイズCD,DECODE(A.上代,0,B.上代,A.上代) 上代,B.上代 商品上代";
            sql_str += ",A.商品CD";
            sql_str += ",GET_COLORNAME2(B.展示会CD,B.予備04,A.色CD) 色名";
            sql_str += ",GET_SIZENAME2(B.予備05,A.サイズCD) サイズ名";
            sql_str += " From HC$MASTER_SHOHIN_JAN A";
            sql_str += " join HC$master_SHOHIN B on (B.商品CD=A.商品CD)";
            sql_str += " where A.商品CD=:1";
            if (para.Length > 1) sql_str += " and A.色CD=:2";
            if (para.Length > 2) sql_str += " and A.サイズCD=:3";

            var ret_para = new BizArray();
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, para);
            if (ret_csv?.Rows.Count > 0)
            {
                ret_para.Set(0, ret_csv.Rows[0][0].ToString());
                ret_para.Set(1, ret_csv.Rows[0][1].ToString());
                ret_para.Set(2, ret_csv.Rows[0][5].ToString());
                ret_para.Set(3, ret_csv.Rows[0][6].ToString());
                ret_para.Set(4, ret_csv.Rows[0][2].ToString());
                ret_para.Set(5, ret_csv.Rows[0][2].ToString());
            }
            return ret_para.ToArray();
        }

        /// <summary>
        /// ■関数 GetTanpin = 単品NO取得（明細チェック用）	※.堀田丸正専用
        /// </summary>
        /// <param name="para">商品CD,単品NO</param>
        /// <returns>JANマスタ読込内容</returns>
        public string[] GetTanpin(string[] para)
        {
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

            var ret_para = new BizArray();
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, v_para);
            if (ret_csv?.Rows.Count > 0)
            {
                for (int i = 0; i < ret_csv.Columns.Count; i++)
                {
                    var cellValue = ret_csv.Rows[0][i];
                    ret_para.Set(i, cellValue != DBNull.Value ? cellValue.ToString() : string.Empty);
                }
            }
            return ret_para.ToArray();
        }

        /// <summary>
        /// ■関数 GetFukumei = 副名取得（明細チェック用）	※.堀田丸正専用
        /// </summary>
        /// <param name="para">副名CD</param>
        /// <param name="flg">FLG（1：副名1（B01）、2：副名2（B02）、3：副名3（B03））</param>
        /// <returns>名称マスタ（B01～B03）読込内容</returns>
        public string[] GetFukumei(string[] para, int flg)
        {
            string[] v_para = new string[1];
            v_para[0] = para[0].ToString();
            string kbn = "B0" + flg.ToString();
            string sql_str = "select 名称CD,名称 from HC$MASTER_MEISHO  where 名称区分='" + kbn + "' AND 名称CD=:1";

            var ret_para = new BizArray();
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, v_para);
            if (ret_csv?.Rows.Count > 0)
            {
                for (int i = 0; i < ret_csv.Columns.Count; i++)
                {
                    var cellValue = ret_csv.Rows[0][i];
                    ret_para.Set(i, cellValue != DBNull.Value ? cellValue.ToString() : string.Empty);
                }
            }
            return ret_para.ToArray();
        }

        /// <summary>
        /// ■関数 GetSouko = 倉庫名取得（明細チェック用）	※.堀田丸正専用
        /// </summary>
        /// <param name="para">引数１:I v_para = 倉庫CD</param>
        /// <returns>得意先マスタ読込内容</returns>
        public string[] GetSouko(string[] para)
        {
            string[] v_para = new string[1];
            v_para[0] = para[0].ToString();
            string sql_str = "select 得意先CD,得意先名 from HC$MASTER_TOKUI where 得意先CD=:1";

            var ret_para = new BizArray();
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, v_para);
            if (ret_csv?.Rows.Count > 0)
            {
                for (int i = 0; i < ret_csv.Columns.Count; i++)
                {
                    var cellValue = ret_csv.Rows[0][i];
                    ret_para.Set(i, cellValue != DBNull.Value ? cellValue.ToString() : string.Empty);
                }
            }
            return ret_para.ToArray();
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

            var ret_para = new BizArray();
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, v_para);
            if (ret_csv?.Rows.Count > 0)
            {
                for (int i = 0; i < ret_csv.Columns.Count; i++)
                {
                    var cellValue = ret_csv.Rows[0][i];
                    ret_para.Set(i, cellValue != DBNull.Value ? cellValue.ToString() : string.Empty);
                }
            }
            return ret_para.ToArray();
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
            string[] v_para = new string[1];
            v_para[0] = para[0].ToString();
            string sql_str = "select A.商品CD,A.商品名,A.単価,A.仕入先商品CD";
            sql_str += " from HC$MASTER_SHKIJI A";
            sql_str += " where A.商品CD=:1 order by 商品CD";

            var ret_para = new BizArray();
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, v_para);
            if (ret_csv?.Rows.Count > 0)
            {
                ret_para.Set(0, ret_csv.Rows[0][0].ToString());
                ret_para.Set(1, ret_csv.Rows[0][1].ToString());
                ret_para.Set(2, ret_csv.Rows[0][2].ToString());
            }
            return ret_para.ToArray();
        }

        /// <summary>
        /// ■関数 GetMaker = 仕入先名取得（明細チェック用）	※.アージュ専用
        /// </summary>
        /// <param name="para">引数１:I v_para = 仕入先CD</param>
        /// <returns>仕入先マスタ読込内容</returns>
        public string[] GetMaker(string[] para)
        {
            string[] v_para = new string[1];
            v_para[0] = para[0].ToString();
            string sql_str = "select 仕入先CD,仕入先名 from HC$MASTER_SIIRE where 仕入先CD=:1";

            var ret_para = new BizArray();
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, v_para);
            if (ret_csv?.Rows.Count > 0)
            {
                for (int i = 0; i < ret_csv.Columns.Count; i++)
                {
                    var cellValue = ret_csv.Rows[0][i];
                    ret_para.Set(i, cellValue != DBNull.Value ? cellValue.ToString() : string.Empty);
                }
            }
            return ret_para.ToArray();
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
            string[] v_para = new string[2];
            v_para[0] = para[0].ToString();
            v_para[1] = para[1].ToString();
            string sql_str = "select 名称CD,名称 from HC$MASTER_MEISHO  where 名称区分=:1 AND 名称CD=:2";

            var ret_para = new BizArray();
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, v_para);
            if (ret_csv?.Rows.Count > 0)
            {
                for (int i = 0; i < ret_csv.Columns.Count; i++)
                {
                    var cellValue = ret_csv.Rows[0][i];
                    ret_para.Set(i, cellValue != DBNull.Value ? cellValue.ToString() : string.Empty);
                }
            }
            return ret_para.ToArray();
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

        /// <summary>
        /// ■関数 GetMeiList_Shohin = 集計項目（括り）のリスト取得
        /// </summary>
        /// <returns>CSVリスト(商品マスタ名称リスト)</returns>
        public DataTable GetMeiList_Shohin()
        {
            var joken = " M.名称CD BETWEEN 'B01' AND 'B10'";

            /* ユーザー対応 11.12.09 */
            if (AppData.ClassCvnet.config.UserFlg == 84)
            {
                joken += " OR M.名称CD IN ('BRD','ITM','MKR','TNJ','SZI','SZN','GEN','DZN','SIK')";
            }
            else
            {
                joken += " OR M.名称CD IN ('BRD','ITM','MKR','TNJ','SZI','SZN','GEN','DZN','BN0','BN1','BN2','SIK')";
            }
            /* 20100726 卸対応 */
            if (AppData.ClassCvnet.config.oroshi >= 1) joken = " M.名称CD BETWEEN 'B01' AND 'B18'";
             
            string sql_str = "SELECT M.名称CD||' '||M.名称 名称";
            sql_str += " FROM HC$MASTER_MEISHO M";
            sql_str += " WHERE M.名称区分='IDX'";
            sql_str += " AND (";

            sql_str += joken;
            sql_str += " )";
            sql_str += " ORDER BY M.名称CD";

            return AppData.Http?.AspxSqlQuery(sql_str);
        }

        /// <summary>
        /// ■関数 GetMeiList_Tokui = 集計項目（括り）のリスト取得
        /// </summary>
        /// <returns>CSVリスト(得意先マスタ名称リスト)</returns>
        public DataTable GetMeiList_Tokui()
        { 
            string sql_str = "SELECT M.名称CD||' '||M.名称 名称";
            sql_str += " FROM HC$MASTER_MEISHO M";
            sql_str += " WHERE M.名称区分='IDX'";
            sql_str += " AND M.名称CD BETWEEN 'C01' AND 'C10'";
            sql_str += " ORDER BY M.名称CD";

            return AppData.Http?.AspxSqlQuery(sql_str);
        }

        /// <summary>
        /// ■関数 GetMeiList_Shiire = 集計項目（括り）のリスト取得
        /// </summary>
        /// <returns>CSVリスト(仕入先マスタ名称リスト)</returns>
        public DataTable GetMeiList_Shiire() 
        {
            string sql_str = "SELECT M.名称CD||' '||M.名称 名称";
            sql_str += " FROM HC$MASTER_MEISHO M";
            sql_str += " WHERE M.名称区分='IDX'";
            sql_str += " AND M.名称CD BETWEEN 'D01' AND 'D10'";
            sql_str += " ORDER BY M.名称CD";

            return AppData.Http?.AspxSqlQuery(sql_str);
        }

        /// <summary>
        /// ■関数 GetMeiList_Kokyaku = 集計項目（括り）のリスト取得
        /// </summary>
        /// <returns>CSVリスト(顧客マスタ名称リスト)</returns>
        public DataTable GetMeiList_Kokyaku()
        { 
            string sql_str = "SELECT M.名称CD||' '||M.名称 名称";
            sql_str += " FROM HC$MASTER_MEISHO M";
            sql_str += " WHERE M.名称区分='IDX'";
            sql_str += " AND M.名称CD BETWEEN 'K01' AND 'K10'";
            sql_str += " ORDER BY M.名称CD";

            return AppData.Http?.AspxSqlQuery(sql_str);
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
        /// 商品マスタ印刷処理 2009.12.09 共通化
        /// </summary>
        /// <param name="wrk_para">印刷条件</param>
        /// <param name="flg">（呼び元判別FLG） 0=商品マスタ、1=各種マスタ印刷、2=各伝票入力画面</param> 
        public DataTable OnQueryPrintShohin(string[] wrk_para, int flg)
        {
            var cvnet_config = AppData.ClassCvnet.config;
            /* JAN先頭桁取得処理 */
            int janlength3 = 0;
            if (cvnet_config.janlength1 - cvnet_config.janlength2 > 0)
            {
                janlength3 = cvnet_config.janlength2;
            }
            else if (cvnet_config.janlength1 - cvnet_config.janlength2 == 0)
            {
                janlength3 = cvnet_config.janlength1;
            }

            int max_col = 87; /* 2010.12.16 外貨単価対応 86->87 */
            /* 絵型画像格納先のPATHを取得 */
            var wrk_para2 = new BizArray();
            wrk_para2.Set(0, "Data/img");
            var ret_csv = AppData.Http?.AspxSqlQuery2("get_img_path", wrk_para2.ToArray(), "", -1);
            string image_path = string.Empty;
            if (string.IsNullOrEmpty(ret_csv)) {
                image_path = ret_csv?.Split('\n')[0] + "\\";
            }

            string joken_sql1 = "a.商品CD in (" + wrk_para[0] + ")";
            string joken_sql2 = "z.商品CD in (" + wrk_para[0] + ")";
            string sort = " order by A.商品CD,B.色CD,B.サイズCD";
            if (cvnet_config.oroshi >= 1) sort = " order by A.商品CD";	/* 卸対応 */
            if (flg == 1)
            {
                joken_sql1 = wrk_para[0];
                joken_sql2 = wrk_para[0];
                sort = wrk_para[1];
            }
            else if (flg == 2)
            {
                joken_sql1 = "a.商品CD='" + wrk_para[0] + "' ";
                joken_sql2 = "z.商品CD='" + wrk_para[0] + "' ";
            }

            string sql_query = "select A.SEQ_NO";
            sql_query += ",SUBSTR(GET_VDATE(a.VDATE_CREATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_CREATE),10,6) 作成日時";
            sql_query += ",SUBSTR(GET_VDATE(a.VDATE_UPDATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_UPDATE),10,6) 更新日時";

            sql_query += ",A.商品CD,A.商品名,A.略称,A.旧コード";

            /* 卸対応 */
            if (cvnet_config.oroshi >= 1) {
                sql_query += ",A.名称CD11,A.名称CD12,A.名称CD13,A.名称CD14,A.名称CD15,A.名称CD16,A.名称CD17,A.名称CD18";
            }
            else {
                sql_query += ",A.展示会CD,A.ブランドCD,A.アイテムCD,A.シーズンCD,A.素材CD,A.デザイナーCD,A.メーカーCD,A.原産国CD";
            }

            sql_query += ",A.元上代,A.上代,A.売変日,A.原価,A.営業原価,A.加工工賃,A.デリバリー日,A.納品日,A.店頭投入日";
            sql_query += ",A.JANコード1 年度,A.JANコード2 製品番号,A.JANコード3 商品CD連番,A.洗濯表示,A.絵型名,A.メモ,A.消費税計算方法,A.在庫管理FLG,A.消費税CD";
            sql_query += ",A.名称CD01,A.名称CD02,A.名称CD03,A.名称CD04,A.名称CD05,A.名称CD06,A.名称CD07,A.名称CD08,A.名称CD09,A.名称CD10";
            sql_query += ",A.自動配分FLG,A.販売期限,A.商品区分FLG,A.商品サイズ区分,A.基準倉庫CD,A.ゼロ単価区分,A.JAN先頭桁,A.POS区分";
            sql_query += ",A.予備01,A.予備02,A.予備03,A.予備04,A.予備05,A.予備06,A.予備07,A.予備08,A.予備09,A.予備10";
            sql_query += ",A.予備11,A.予備12,A.予備13,A.予備14,A.予備15,A.予備16,A.予備17,A.予備18,A.予備19,A.予備20";
            sql_query += ",A.仕入区分,A.消化桁切指定,A.消化端数区分,A.消化計算区分,A.消化掛率,A.男女区分,A.コラボ出力区分";
            sql_query += ",A.代表品番FLG";
            sql_query += ",A.セール区分,A.リピート日,A.メーカー品番";
            sql_query += ",A.仕入価格,A.納品区分";
            sql_query += ",A.絵型名2,A.販売開始日";
            /* 2010.12.16 外貨単価対応 S */
            sql_query += ((cvnet_config.dispGaitan >= 1) ? ",A.外貨単価" : ",'-999999999' 外貨単価");

            /* コードの空き確保処理 */
            for (var i = max_col; i < 150; i++)
            {
                sql_query += " ,'' DummyCD" + i.ToString("000");
            }

            /* 卸対応 */
            if (cvnet_config.oroshi >= 1)
            {
                sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B11' and H.名称CD=A.名称CD11),'.') 補足11名";
                sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B12' and H.名称CD=A.名称CD12),'.') 補足12名";
                sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B13' and H.名称CD=A.名称CD13),'.') 補足13名";
                sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B14' and H.名称CD=A.名称CD14),'.') 補足14名";
                sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B15' and H.名称CD=A.名称CD15),'.') 補足15名";
                sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B16' and H.名称CD=A.名称CD16),'.') 補足16名";
                sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B17' and H.名称CD=A.名称CD17),'.') 補足17名";
                sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B18' and H.名称CD=A.名称CD18),'.') 補足18名";
            }
            else
            {
                sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='TNJ' and H.名称CD=A.展示会CD),'.') 展示会名";
                sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='BRD' and H.名称CD=A.ブランドCD),'.') ブランド名";
                sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='ITM' and H.名称CD=A.アイテムCD),'.') アイテム名";
                sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='SZN' and H.名称CD=A.シーズンCD),'.') シーズン名";
                sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='SZI' and H.名称CD=A.素材CD),'.') 素材名";
                sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='DZN' and H.名称CD=A.デザイナーCD),'.') デザイナー名";
                sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='MKR' and H.名称CD=A.メーカーCD),'.') メーカー名";
                sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='GEN' and H.名称CD=A.原産国CD),'.') 原産国名";
            }
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B01' and H.名称CD=A.名称CD01),'.') 補足01名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B02' and H.名称CD=A.名称CD02),'.') 補足02名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B03' and H.名称CD=A.名称CD03),'.') 補足03名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B04' and H.名称CD=A.名称CD04),'.') 補足04名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B05' and H.名称CD=A.名称CD05),'.') 補足05名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B06' and H.名称CD=A.名称CD06),'.') 補足06名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B07' and H.名称CD=A.名称CD07),'.') 補足07名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B08' and H.名称CD=A.名称CD08),'.') 補足08名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B09' and H.名称CD=A.名称CD09),'.') 補足09名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B10' and H.名称CD=A.名称CD10),'.') 補足10名";
            sql_query += ",NVL((select H.得意先名 from HC$MASTER_TOKUI H where H.得意先CD=A.基準倉庫CD),'.') 倉庫名";
            sql_query += ",A.入力社員CD ||' '||(select S.名前 from HC$MASTER_SHAIN S where S.社員CD=A.入力社員CD) 最終修正者";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='MLK' and H.名称CD=A.男女区分),'.') 男女区分名";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='B01'),'.') title1";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='B02'),'.') title2";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='B03'),'.') title3";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='B04'),'.') title4";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='B05'),'.') title5";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='B06'),'.') title6";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='B07'),'.') title7";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='B08'),'.') title8";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='B09'),'.') title9";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='B10'),'.') title10";
            sql_query += ",substr(A.JAN先頭桁,0," + janlength3 + ") 加工JAN先頭桁";            /* JANコード桁数の表示桁数を設定 */
            sql_query += ((cvnet_config.oroshi >= 1) ? ",'' 色" : ",NVL(B.色CD ||' '|| (select H.名称 from HC$MASTER_MEISHO H where H.名称区分='COL' and H.名称CD=B.色CD),'.') 色");
            sql_query += ((cvnet_config.oroshi >= 1) ? ",'' サイズ" : ",NVL(B.サイズCD ||' '||get_sizename(A.商品CD,B.サイズCD),'.') サイズ");
            sql_query += ",NVL((select H.名称 from hc$master_meisho H where H.名称区分='IDX' and H.名称CD=A.商品サイズ区分),'.') サイズ区分";

            /* 画面上ComboBoxの文字列取得 */
            sql_query += "," + comboItem00.GetCaseStr("する", "a.在庫管理FLG") + " 在庫管理名";
            sql_query += "," + comboItem00.GetCaseStr("仕入区分", "a.仕入区分") + " 仕入区分名";
            sql_query += "," + comboItem00.GetCaseStr("桁切", "a.消化桁切指定") + " 消化桁切名";
            sql_query += "," + comboItem00.GetCaseStr("端数", "a.消化端数区分") + " 消化端数名";
            sql_query += ",CASE WHEN (A.POS区分='0') THEN '0 通常' WHEN (A.POS区分='1') THEN '9 POSﾏｽﾀ削除指示' WHEN (A.POS区分='2') THEN '10 出力しない' ELSE '.' END  POS区分名";
            sql_query += ",CASE WHEN (A.消化計算区分='0') THEN '0 原価代入' WHEN (A.消化計算区分='1') THEN '1 掛率計算' ELSE '.' END  消化計算名";
            sql_query += ",CASE WHEN (A.自動配分FLG='0') THEN '0 自動補充発注しない' WHEN (A.自動配分FLG='1') THEN '1 自動補充売上基準' ELSE '.' END  自動配分FLG名";
            sql_query += ",CASE WHEN (A.商品区分FLG='0') THEN '0 通常' WHEN (A.商品区分FLG='1') THEN '1 商品外' ELSE '.' END  商品区分FLG名";
            sql_query += ",CASE WHEN (A.ゼロ単価区分='0') THEN '0 禁止' WHEN (A.ゼロ単価区分='1') THEN '1 許可' WHEN (A.ゼロ単価区分='2') THEN '2 必須入力' ELSE '.' END  ゼロ単価区分名";
            sql_query += ",'' 予備02名";
            sql_query += ",'' 予備03名";
            sql_query += ",'" + image_path + "'||nvl(A.絵型名,'.') 絵型PATH";
            sql_query += ",CASE WHEN (A.代表品番FLG='0') THEN '0 通常商品' WHEN (A.代表品番FLG='1') THEN '1 代表品番商品' ELSE '.' END  代表品番FLG名";
            /* sql_query +=",'" + image_path + "'||nvl(A.洗濯表示,'.') 洗濯表示PATH"; */
            sql_query += ",'" + image_path + "'||nvl(A.絵型名2,'.') 絵型2PATH";
            sql_query += ",CASE WHEN (A.セール区分='0') THEN '0 プロパー商品' WHEN (A.セール区分='1') THEN '1 セール商品' ELSE '.' END セール区分名";
            sql_query += ",CASE WHEN (A.納品区分='0') THEN '0 倉庫入庫' WHEN (A.納品区分='1') THEN '1 店舗入庫' ELSE '.' END 納品区分名";
            sql_query += ((cvnet_config.tanpin == 1) ? ",CASE WHEN (A.商品管理FLG='0') THEN '0 通常' WHEN (A.商品管理FLG='1') THEN '1 ロット' WHEN (A.商品管理FLG='2') THEN '2 単品' ELSE '.' END 商品管理FLG名" : ",'' 商品管理FLG名");
            sql_query += "," + comboItem00.GetCaseStr("課税区分", "a.消費税計算方法") + " 消費税計算方法名";

            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='B11'),'.') title11";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='B12'),'.') title12";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='B13'),'.') title13";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='B14'),'.') title14";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='B15'),'.') title15";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='B16'),'.') title16";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='B17'),'.') title17";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='B18'),'.') title18";

            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='Y01'),'予備01') titleY01";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='Y02'),'予備02') titleY02";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='Y03'),'予備03') titleY03";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='Y04'),'予備04') titleY04";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='Y05'),'予備05') titleY05";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='Y06'),'予備06') titleY06";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='Y07'),'予備07') titleY07";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='Y08'),'予備08') titleY08";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='Y09'),'予備09') titleY09";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='Y10'),'予備10') titleY10";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='Y11'),'予備11') titleY11";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='Y12'),'予備12') titleY12";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='Y13'),'予備13') titleY13";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='Y14'),'予備14') titleY14";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='Y15'),'予備15') titleY15";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='Y16'),'予備16') titleY16";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='Y17'),'予備17') titleY17";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='Y18'),'予備18') titleY18";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='Y19'),'予備19') titleY19";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='Y20'),'予備20') titleY20";

            /* 2015.04.01 #19666対応（予備項目の名称を追加）/////////////////////////////////////////////////////////////////////////////////////// START */
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y01' and H.名称CD=A.予備01),'.') nameY01";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y02' and H.名称CD=A.予備02),'.') nameY02";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y03' and H.名称CD=A.予備03),'.') nameY03";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y04' and H.名称CD=A.予備04),'.') nameY04";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y05' and H.名称CD=A.予備05),'.') nameY05";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y06' and H.名称CD=A.予備06),'.') nameY06";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y07' and H.名称CD=A.予備07),'.') nameY07";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y08' and H.名称CD=A.予備08),'.') nameY08";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y09' and H.名称CD=A.予備09),'.') nameY09";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y10' and H.名称CD=A.予備10),'.') nameY10";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y11' and H.名称CD=A.予備11),'.') nameY11";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y12' and H.名称CD=A.予備12),'.') nameY12";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y13' and H.名称CD=A.予備13),'.') nameY13";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y14' and H.名称CD=A.予備14),'.') nameY14";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y15' and H.名称CD=A.予備15),'.') nameY15";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y16' and H.名称CD=A.予備16),'.') nameY16";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y17' and H.名称CD=A.予備17),'.') nameY17";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y18' and H.名称CD=A.予備18),'.') nameY18";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y19' and H.名称CD=A.予備19),'.') nameY19";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y20' and H.名称CD=A.予備20),'.') nameY20";
            /* 2015.04.01 #19666対応（予備項目の名称を追加）/////////////////////////////////////////////////////////////////////////////////////// END */
            sql_query += " ,CASE WHEN (A.消費税CD = '1') THEN '1 通常税率' WHEN (A.消費税CD = '2') THEN '2 軽減税率' ELSE TO_CHAR(A.消費税CD) END 税区分";/*#52188 軽減税率対応*/
            /* 2015.04.01 #62670対応（CVEC項目を追加）/////////////////////////////////////////////////////////////////////////////////////// START */
            sql_query += ",CASE WHEN (A.EC連携='0') THEN '0 EC連携しない' WHEN (A.EC連携='1') THEN '1 EC連携する' ELSE '.' END EC連携名";
            sql_query += ",CASE WHEN (A.EC取置='0') THEN '0 EC取置しない' WHEN (A.EC取置='1') THEN '1 EC取置する' ELSE '.' END EC取置名";
            /* 2015.04.01 #62670対応（CVEC項目を追加）/////////////////////////////////////////////////////////////////////////////////////// END */
            /* 名称を新規に追加する場合は、下の件数を増やすようにする */
            /* これ重要 */
            int MeiCount = 254;

            /* 名称ダミー代入処理 */
            for (var i = MeiCount; i < 300; i++)
            {
                sql_query += " ,'' DummyMei" + i.ToString("000");
            }

            sql_query += " from HC$Master_SHOHIN A";
            sql_query += ((cvnet_config.oroshi >= 1) ? "" : ", HC$Master_SHOHIN_JAN B");
            sql_query += " where ";
            sql_query += ((cvnet_config.oroshi >= 1) ? "" : "A.商品CD= B.商品CD(+) and ");
            sql_query += joken_sql1;
            sql_query += sort;

            sql_query = "select aa.*,h1.品質 品質1,h2.品質 品質2,h3.品質 品質3,h4.品質 品質4,h5.品質 品質5,h6.品質 品質6,h7.品質 品質7,h8.品質 品質8,h9.品質 品質9, h10.品質 品質10 from (" + sql_query + ") aa";
            sql_query += ",(SELECT a.商品CD,a.品質||' '||a.パーセント||'%'||decode(nvl(a.区分,'0'),'1',' (表)','2',' (裏)','3',' (その他)','') 品質 FROM (SELECT row_number() over (partition by z.商品CD order by z.生地付属CD,z.パーセント desc) ct,z.品質,z.商品CD,z.パーセント,z.生地付属CD 区分 FROM HC$MASTER_SHOHIN_GRADE z,HC$MASTER_SHOHIN a WHERE a.商品CD= z.商品CD(+) and " + joken_sql2 + " GROUP BY z.商品CD,z.品質,z.パーセント,z.生地付属CD) a WHERE ct=1) h1";
            sql_query += ",(SELECT a.商品CD,a.品質||' '||a.パーセント||'%'||decode(nvl(a.区分,'0'),'1',' (表)','2',' (裏)','3',' (その他)','') 品質 FROM (SELECT row_number() over (partition by z.商品CD order by z.生地付属CD,z.パーセント desc) ct,z.品質,z.商品CD,z.パーセント,z.生地付属CD 区分 FROM HC$MASTER_SHOHIN_GRADE z,HC$MASTER_SHOHIN a WHERE a.商品CD= z.商品CD(+) and " + joken_sql2 + " GROUP BY z.商品CD,z.品質,z.パーセント,z.生地付属CD) a WHERE ct=2) h2";
            sql_query += ",(SELECT a.商品CD,a.品質||' '||a.パーセント||'%'||decode(nvl(a.区分,'0'),'1',' (表)','2',' (裏)','3',' (その他)','') 品質 FROM (SELECT row_number() over (partition by z.商品CD order by z.生地付属CD,z.パーセント desc) ct,z.品質,z.商品CD,z.パーセント,z.生地付属CD 区分 FROM HC$MASTER_SHOHIN_GRADE z,HC$MASTER_SHOHIN a WHERE a.商品CD= z.商品CD(+) and " + joken_sql2 + " GROUP BY z.商品CD,z.品質,z.パーセント,z.生地付属CD) a WHERE ct=3) h3";
            sql_query += ",(SELECT a.商品CD,a.品質||' '||a.パーセント||'%'||decode(nvl(a.区分,'0'),'1',' (表)','2',' (裏)','3',' (その他)','') 品質 FROM (SELECT row_number() over (partition by z.商品CD order by z.生地付属CD,z.パーセント desc) ct,z.品質,z.商品CD,z.パーセント,z.生地付属CD 区分 FROM HC$MASTER_SHOHIN_GRADE z,HC$MASTER_SHOHIN a WHERE a.商品CD= z.商品CD(+) and " + joken_sql2 + " GROUP BY z.商品CD,z.品質,z.パーセント,z.生地付属CD) a WHERE ct=4) h4";
            sql_query += ",(SELECT a.商品CD,a.品質||' '||a.パーセント||'%'||decode(nvl(a.区分,'0'),'1',' (表)','2',' (裏)','3',' (その他)','') 品質 FROM (SELECT row_number() over (partition by z.商品CD order by z.生地付属CD,z.パーセント desc) ct,z.品質,z.商品CD,z.パーセント,z.生地付属CD 区分 FROM HC$MASTER_SHOHIN_GRADE z,HC$MASTER_SHOHIN a WHERE a.商品CD= z.商品CD(+) and " + joken_sql2 + " GROUP BY z.商品CD,z.品質,z.パーセント,z.生地付属CD) a WHERE ct=5) h5";
            sql_query += ",(SELECT a.商品CD,a.品質||' '||a.パーセント||'%'||decode(nvl(a.区分,'0'),'1',' (表)','2',' (裏)','3',' (その他)','') 品質 FROM (SELECT row_number() over (partition by z.商品CD order by z.生地付属CD,z.パーセント desc) ct,z.品質,z.商品CD,z.パーセント,z.生地付属CD 区分 FROM HC$MASTER_SHOHIN_GRADE z,HC$MASTER_SHOHIN a WHERE a.商品CD= z.商品CD(+) and " + joken_sql2 + " GROUP BY z.商品CD,z.品質,z.パーセント,z.生地付属CD) a WHERE ct=6) h6";
            sql_query += ",(SELECT a.商品CD,a.品質||' '||a.パーセント||'%'||decode(nvl(a.区分,'0'),'1',' (表)','2',' (裏)','3',' (その他)','') 品質 FROM (SELECT row_number() over (partition by z.商品CD order by z.生地付属CD,z.パーセント desc) ct,z.品質,z.商品CD,z.パーセント,z.生地付属CD 区分 FROM HC$MASTER_SHOHIN_GRADE z,HC$MASTER_SHOHIN a WHERE a.商品CD= z.商品CD(+) and " + joken_sql2 + " GROUP BY z.商品CD,z.品質,z.パーセント,z.生地付属CD) a WHERE ct=7) h7";
            sql_query += ",(SELECT a.商品CD,a.品質||' '||a.パーセント||'%'||decode(nvl(a.区分,'0'),'1',' (表)','2',' (裏)','3',' (その他)','') 品質 FROM (SELECT row_number() over (partition by z.商品CD order by z.生地付属CD,z.パーセント desc) ct,z.品質,z.商品CD,z.パーセント,z.生地付属CD 区分 FROM HC$MASTER_SHOHIN_GRADE z,HC$MASTER_SHOHIN a WHERE a.商品CD= z.商品CD(+) and " + joken_sql2 + " GROUP BY z.商品CD,z.品質,z.パーセント,z.生地付属CD) a WHERE ct=8) h8";
            sql_query += ",(SELECT a.商品CD,a.品質||' '||a.パーセント||'%'||decode(nvl(a.区分,'0'),'1',' (表)','2',' (裏)','3',' (その他)','') 品質 FROM (SELECT row_number() over (partition by z.商品CD order by z.生地付属CD,z.パーセント desc) ct,z.品質,z.商品CD,z.パーセント,z.生地付属CD 区分 FROM HC$MASTER_SHOHIN_GRADE z,HC$MASTER_SHOHIN a WHERE a.商品CD= z.商品CD(+) and " + joken_sql2 + " GROUP BY z.商品CD,z.品質,z.パーセント,z.生地付属CD) a WHERE ct=9) h9";
            sql_query += ",(SELECT a.商品CD,a.品質||' '||a.パーセント||'%'||decode(nvl(a.区分,'0'),'1',' (表)','2',' (裏)','3',' (その他)','') 品質 FROM (SELECT row_number() over (partition by z.商品CD order by z.生地付属CD,z.パーセント desc) ct,z.品質,z.商品CD,z.パーセント,z.生地付属CD 区分 FROM HC$MASTER_SHOHIN_GRADE z,HC$MASTER_SHOHIN a WHERE a.商品CD= z.商品CD(+) and " + joken_sql2 + " GROUP BY z.商品CD,z.品質,z.パーセント,z.生地付属CD) a WHERE ct=10) h10";
            sql_query += " WHERE ";
            sql_query += "aa.商品CD=h1.商品CD(+)";
            sql_query += " AND aa.商品CD=h2.商品CD(+)";
            sql_query += " AND aa.商品CD=h3.商品CD(+)";
            sql_query += " AND aa.商品CD=h4.商品CD(+)";
            sql_query += " AND aa.商品CD=h5.商品CD(+)";
            sql_query += " AND aa.商品CD=h6.商品CD(+)";
            sql_query += " AND aa.商品CD=h7.商品CD(+)";
            sql_query += " AND aa.商品CD=h8.商品CD(+)";
            sql_query += " AND aa.商品CD=h9.商品CD(+)";
            sql_query += " AND aa.商品CD=h10.商品CD(+)";

            var qfm_file = "cvnet_shouhin_v2.qfm";
            if (cvnet_config.oroshi >= 1) qfm_file = "cvnet_shouhin_w.qfm"; /* 卸対応 */
            if (cvnet_config.UserFlg == 43) qfm_file = "cvnet_shouhin_43.qfm"; 
            return AppData.Http?.AspxSqlQuery(sql_query, wrk_para, qfm_file);
        }

        /// <summary>
        /// 得意先マスタ印刷処理 2010.01.13 共通化
        /// </summary>
        /// <param name="wrk_para">印刷条件</param>
        /// <param name="flg">（呼び元判別FLG） 0=得意先マスタ、1=各種マスタ印刷、2=各伝票入力画面</param>
        /// <param name="wrk_para2">各種マスタ印刷専用パラメータ</param>
        /// <returns></returns>
        public string OnQueryPrintTokui(string[] wrk_para, int flg, string[] wrk_para2 = null)
        { 
            int max_col = 103;
            string joken_sql1 = " WHERE A.得意先CD in (" + wrk_para[0] + ") and A.店種区分>=0";
            string sort = " order by A.得意先CD";

            if (flg == 1)
            {
                joken_sql1 = " WHERE " + wrk_para[0];
                sort = wrk_para[1];
            }
            else if (flg == 2)
            {
                joken_sql1 = " WHERE a.得意先CD='" + wrk_para[0] + "' ";
            }

            string sql_query = "select A.SEQ_NO";
            sql_query += ",SUBSTR(GET_VDATE(a.VDATE_CREATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_CREATE),10,6) 作成日時";
            sql_query += ",SUBSTR(GET_VDATE(a.VDATE_UPDATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_UPDATE),10,6) 更新日時";
            sql_query += ",A.得意先CD,A.得意先名,A.カナ,A.旧コード,A.略称,A.郵便番号,A.住所1,A.住所2,A.住所3,A.TEL,A.FAX";
            sql_query += ",A.宛名FLG1,A.宛名FLG2,A.宛名FLG3,A.宛名名称1,A.宛名名称2,A.営業担当CD,A.店種区分,A.坪数,A.在庫管理FLG";
            sql_query += ",A.掛率,A.セール掛率,A.店頭セール掛率,A.請求先CD,A.請求印刷,A.締日,A.入金予定月,A.入金予定日,A.入金方法,A.下代桁切指定";
            sql_query += ",A.下代端数区分,A.下代計算FLG,A.消費税CD,A.消費税計算方法,A.消費税端数,A.与信限度額,A.入金率,A.出荷停止FLG";
            sql_query += ",A.伝票発行区分,A.備考,A.伝票印字1,A.伝票印字2,A.伝票印字3,A.伝票印字4";
            sql_query += ",A.自動配分FLG,A.開始日,A.終了日,A.棚卸日,A.名称CD01,A.名称CD02,A.名称CD03,A.名称CD04,A.名称CD05,A.名称CD06";
            sql_query += ",A.倉庫区分,A.営業時間1,A.営業時間2,A.営業時間3,A.施工業者情報,A.デベロッパ,A.開始時刻,A.終了時刻,A.端末ID,A.営業時間,A.棚卸日END";
            sql_query += ",A.部門,A.為替区分,A.為替桁切指定,A.為替端数区分,A.名称CD07,A.名称CD08,A.名称CD09,A.名称CD10,A.配分ランク01,A.配分ランク02";
            sql_query += ",A.基準倉庫FLG,A.基準倉庫CD,A.出荷FLG,A.POS区分,A.配分方法FLG,A.伝票印字5,A.伝票印字6,A.伝票印字7,A.伝票印字8";
            sql_query += ",A.店舗売場コード,A.ECFLG,A.締日2,A.締日3,入金予定月2,入金予定日2,入金予定月3,入金予定日3";
            sql_query += ",A.伝票社名,A.伝票店名";
            sql_query += ",移動区分";
            sql_query += ",A.振込先1,A.振込先2,A.振込先3"; /* 2011.03.10 (99,100,101) */
            /*2021.04.3 #51847 得意先MAIL追加*/
            sql_query += ",A.得意先MAIL";
            sql_query += ",A.登録番号"; /* 2023.12.22 #70586対応追加 */

            /* コードの空き確保処理 */
            for (var i = max_col; i < 150; i++)
            {
                sql_query += " ,'' DummyCD" + i.ToString("000");
            }
            /* 得意先マスタの名称追加は特に意識する必要はなし、後ろに追加すれば良い */

            sql_query += ",NVL((select H.名前 from HC$MASTER_SHAIN H where H.社員CD=A.営業担当CD),'.') 担当名";
            sql_query += ",NVL((select H.得意先名 from HC$MASTER_TOKUI H where  H.得意先CD=A.請求先CD),'.') 請求名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C01' and H.名称CD=A.名称CD01),'.') 補足01名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C02' and H.名称CD=A.名称CD02),'.') 補足02名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C03' and H.名称CD=A.名称CD03),'.') 補足03名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C04' and H.名称CD=A.名称CD04),'.') 補足04名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C05' and H.名称CD=A.名称CD05),'.') 補足05名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C06' and H.名称CD=A.名称CD06),'.') 補足06名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C07' and H.名称CD=A.名称CD07),'.') 補足07名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C08' and H.名称CD=A.名称CD08),'.') 補足08名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C09' and H.名称CD=A.名称CD09),'.') 補足09名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C10' and H.名称CD=A.名称CD10),'.') 補足10名";
            sql_query += ",(A.入力社員CD ||' '|| (select B.名前 from HC$MASTER_SHAIN B where B.社員CD=A.入力社員CD)) 最終修正者";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='BMN' and H.名称CD=A.部門),'.') 部門名";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C01'),'.') title1";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C02'),'.') title2";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C03'),'.') title3";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C04'),'.') title4";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C05'),'.') title5";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C06'),'.') title6";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C07'),'.') title7";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C08'),'.') title8";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C09'),'.') title9";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C10'),'.') title10";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='RAT' and H.名称CD=A.為替区分),'.') 為替区分名";
            sql_query += ",NVL((select H.得意先名 from HC$MASTER_TOKUI H where H.得意先CD=A.基準倉庫CD),'.') 倉庫名";
            sql_query += "," + comboItem00.GetCaseStr("する", "A.在庫管理FLG") + " 在庫管理名";
            sql_query += "," + comboItem00.GetCaseStr("する", "A.請求印刷") + " 請求印刷名";
            sql_query += "," + comboItem00.GetCaseStr("予定月", "A.入金予定月") + " 入金予定月名";
            sql_query += "," + comboItem00.GetCaseStr("入金区分2", "A.入金方法") + " 入金方法名";
            sql_query += "," + comboItem00.GetCaseStr("桁切", "A.下代桁切指定") + " 下代桁切名";
            sql_query += "," + comboItem00.GetCaseStr("端数", "A.下代端数区分") + " 下代端数名";
            sql_query += "," + comboItem00.GetCaseStr("下代計算", "A.下代計算FLG") + " 下代計算名";

            /* 卸対応 10.08.31 */
            if (AppData.ClassCvnet.config.oroshi == 0)
            {
                sql_query += ",CASE WHEN (A.消費税CD=0) THEN '0 非課税' WHEN (A.消費税CD=1) THEN '1 課税' ELSE '.' END  消費税CD名";
            }
            else
            {
                sql_query += ",CASE WHEN (A.消費税CD=0) THEN '0 非課税' WHEN (A.消費税CD=1) THEN '1 外税' WHEN (A.消費税CD=2) THEN '2 内税' ELSE '.' END  消費税CD名";
            }

            sql_query += "," + comboItem00.GetCaseStr("端数", "A.消費税端数") + " 消費税端数名";
            sql_query += "," + comboItem00.GetCaseStr("消費税計算", "A.消費税計算方法") + " 消費税計算名";
            sql_query += "," + comboItem00.GetCaseStr("する", "A.出荷停止FLG") + " 出荷停止名";
            sql_query += "," + comboItem00.GetCaseStr("為替桁切", "A.為替桁切指定") + " 為替桁切名";
            sql_query += "," + comboItem00.GetCaseStr("端数", "A.為替端数区分") + " 為替端数名";
            sql_query += "," + comboItem00.GetCaseStr("倉庫区分02", "A.倉庫区分") + " 倉庫区分名";
            /* #28853 店種区分の名前が不正 16.07.06 */
            sql_query += ",CASE WHEN (A.店種区分='0') THEN '0 倉庫' WHEN (A.店種区分='1') THEN '1 卸先' WHEN (A.店種区分='3') THEN '3 売仕店' WHEN (A.店種区分='6') THEN '6 直営店' ELSE '.' END  店種区分名";
            sql_query += ",CASE WHEN (A.POS区分='0') THEN '0 通常' WHEN (A.POS区分='1') THEN '9 POSﾏｽﾀ削除指示' WHEN (A.POS区分='2') THEN '10 出力しない' ELSE '.' END  POS区分名";
            sql_query += ",CASE WHEN (A.ECFLG='0') THEN '0 通常店舗' WHEN (A.ECFLG='1') THEN '1 EC店舗' ELSE '.' END  ECFLG名";
            /*2021.04.23 #51847 自動補充FLG変更*/
            /* sql_query += ",CASE WHEN (A.自動配分FLG='0') THEN '000 自動補充しない' WHEN (A.自動配分FLG='1') THEN '127 補充（毎日）' ELSE '.' END  自動配分名"; */
            sql_query += ",CASE WHEN (A.自動配分FLG='0') THEN '0 自動補充しない' WHEN (A.自動配分FLG='1') THEN '1 補充（毎日）' ELSE '.' END  自動配分名";
            sql_query += ",CASE WHEN (A.基準倉庫FLG='0') THEN '0 商品マスタの基準倉庫' WHEN (A.基準倉庫FLG='1') THEN '1 得意先マスタの基準倉庫' ELSE '.' END  基準倉庫名";
            sql_query += ",CASE WHEN (A.出荷FLG='0') THEN '0 出荷予定' WHEN (A.出荷FLG=1) THEN '1 出荷確定' ELSE '.' END  出荷FLG名";
            sql_query += ",CASE WHEN (A.配分方法FLG='0') THEN '0 売上順' WHEN (A.配分方法FLG='1') THEN '1 ランク順' WHEN (A.配分方法FLG='2') THEN '2 均等配分' ELSE '.' END  配分方法名";
            sql_query += ",CASE WHEN (A.伝票発行区分=0 OR A.伝票発行区分=1) THEN '' WHEN (A.伝票発行区分=2 OR A.伝票発行区分=5 OR A.伝票発行区分=8) THEN '店別' WHEN (A.伝票発行区分=3 OR A.伝票発行区分=4 OR A.伝票発行区分=6 OR A.伝票発行区分=7) THEN '社･店コード' ELSE '.' END  伝票印字ラベル1";
            sql_query += ",CASE WHEN (A.伝票発行区分=0 OR A.伝票発行区分=1) THEN '' WHEN (A.伝票発行区分=2 OR A.伝票発行区分=5 OR A.伝票発行区分=8) THEN '品別番号' WHEN (A.伝票発行区分=3 OR A.伝票発行区分=4 OR A.伝票発行区分=6 OR A.伝票発行区分=7) THEN '分類コード' ELSE '.' END  伝票印字ラベル2";
            sql_query += ",CASE WHEN (A.伝票発行区分=0 OR A.伝票発行区分=1) THEN '' WHEN (A.伝票発行区分=2 OR A.伝票発行区分=3 OR A.伝票発行区分=4 OR A.伝票発行区分=5 OR A.伝票発行区分=6 OR A.伝票発行区分=7 OR A.伝票発行区分=8) THEN '取引先コード' ELSE '.' END  伝票印字ラベル3";
            sql_query += ",CASE WHEN (A.伝票発行区分=0 OR A.伝票発行区分=1) THEN '' WHEN (A.伝票発行区分=2 OR A.伝票発行区分=5 OR A.伝票発行区分=8) THEN '納品場所' WHEN (A.伝票発行区分=3 OR A.伝票発行区分=4 OR A.伝票発行区分=6 OR A.伝票発行区分=7) THEN '伝票区分' ELSE '.' END  伝票印字ラベル4";
            sql_query += ",CASE WHEN (A.伝票発行区分=0 OR A.伝票発行区分=1 OR A.伝票発行区分=3 OR A.伝票発行区分=4 OR A.伝票発行区分=6 OR A.伝票発行区分=7) THEN '' WHEN (A.伝票発行区分=2 OR A.伝票発行区分=8) THEN '店出場所' WHEN (A.伝票発行区分=5) THEN '売場名' ELSE '.' END  伝票印字ラベル5";
            sql_query += ",CASE WHEN (A.伝票発行区分=0 OR A.伝票発行区分=1 OR A.伝票発行区分=3 OR A.伝票発行区分=4 OR A.伝票発行区分=5 OR A.伝票発行区分=6 OR A.伝票発行区分=7) THEN '' WHEN (A.伝票発行区分=2 OR A.伝票発行区分=8) THEN '売場名' ELSE '.' END  伝票印字ラベル6";
            sql_query += ",CASE WHEN (A.伝票発行区分=0 OR A.伝票発行区分=1 OR A.伝票発行区分=3 OR A.伝票発行区分=4 OR A.伝票発行区分=5 OR A.伝票発行区分=6 OR A.伝票発行区分=7) THEN '' WHEN (A.伝票発行区分=2 OR A.伝票発行区分=8) THEN '内線番号' ELSE '.' END  伝票印字ラベル7";
            sql_query += ",CASE WHEN (A.伝票発行区分=0 OR A.伝票発行区分=1 OR A.伝票発行区分=3 OR A.伝票発行区分=4 OR A.伝票発行区分=5 OR A.伝票発行区分=6 OR A.伝票発行区分=7) THEN '' WHEN (A.伝票発行区分=2 OR A.伝票発行区分=8) THEN '担当者' ELSE '.' END  伝票印字ラベル8";
            sql_query += "," + comboItem00.GetCaseStr("伝票", "A.伝票発行区分") + " 伝票発行区分名";
            sql_query += "," + comboItem00.GetCaseStr("予定月", "A.入金予定月2") + " 入金予定月名2";
            sql_query += "," + comboItem00.GetCaseStr("予定月", "A.入金予定月2") + " 入金予定月名3";
            sql_query += ",CASE WHEN (A.伝票発行区分=0 OR A.伝票発行区分=1) THEN '' WHEN (A.伝票発行区分=2 OR A.伝票発行区分=3 OR A.伝票発行区分=4 OR A.伝票発行区分=5 OR A.伝票発行区分=6 OR A.伝票発行区分=7 OR A.伝票発行区分=8) THEN '伝票社名' ELSE '.' END 伝票社名ラベル";
            sql_query += ",CASE WHEN (A.伝票発行区分=0 OR A.伝票発行区分=1) THEN '' WHEN (A.伝票発行区分=2 OR A.伝票発行区分=3 OR A.伝票発行区分=4 OR A.伝票発行区分=5 OR A.伝票発行区分=6 OR A.伝票発行区分=7 OR A.伝票発行区分=8) THEN '伝票店名' ELSE '.' END 伝票店名ラベル";
            sql_query += "," + comboItem00.GetCaseStr("得意先移動区分", "A.移動区分") + " 移動区分名";
            sql_query += ",A.名称CD11,A.名称CD12,A.名称CD13,A.名称CD14,A.名称CD15";
            sql_query += ",A.名称CD16,A.名称CD17,A.名称CD18,A.名称CD19,A.名称CD20";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C11'),'.') title11";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C12'),'.') title12";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C13'),'.') title13";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C14'),'.') title14";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C15'),'.') title15";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C16'),'.') title16";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C17'),'.') title17";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C18'),'.') title18";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C19'),'.') title19";
            sql_query += ",NVL((select 名称 from HC$master_meisho where 名称区分='IDX' and 名称CD='C20'),'.') title20";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C11' and H.名称CD=A.名称CD11),'.') 補足11名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C12' and H.名称CD=A.名称CD12),'.') 補足12名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C13' and H.名称CD=A.名称CD13),'.') 補足13名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C14' and H.名称CD=A.名称CD14),'.') 補足14名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C15' and H.名称CD=A.名称CD15),'.') 補足15名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C16' and H.名称CD=A.名称CD16),'.') 補足16名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C17' and H.名称CD=A.名称CD17),'.') 補足17名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C18' and H.名称CD=A.名称CD18),'.') 補足18名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C19' and H.名称CD=A.名称CD19),'.') 補足19名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='C20' and H.名称CD=A.名称CD20),'.') 補足20名";
            sql_query += " from HC$Master_TOKUI A ";
            sql_query += joken_sql1;
            sql_query += sort;

            sql_query = "select * from (" + sql_query + ")";

            var qfm_name = "cvnet_tokuisaki.qfm";

            if (AppData.ClassCvnet.config.smtflg == 1) qfm_name = "cvnet_tokuisaki_r.qfm";

            /* 卸対応　10.04.27 */
            if (AppData.ClassCvnet.config.oroshi != 0) qfm_name = "cvnet_tokuisaki_w.qfm";
             
            return AppData.Http?.AspxSqlQueryCsv(sql_query, wrk_para2, qfm_name);
        }

        /// <summary>
        /// 仕入先マスタ印刷処理 2010.01.13 共通化 
        /// </summary>
        /// <param name="wrk_para">印刷条件</param>
        /// <param name="flg">（呼び元判別FLG） 0=仕入先マスタ、1=各種マスタ印刷、2=各伝票入力画面</param>
        /// <param name="wrk_para2">各種マスタ印刷専用パラメータ</param>
        /// <returns></returns>
        public DataTable OnQueryPrintSiire(string[] wrk_para, int flg, string[] wrk_para2 = null)
        { 
            int max_col = 73;
            string joken_sql1 = " WHERE A.仕入先CD in (" + wrk_para[0] + ")";
            string sort = " order by A.仕入先CD";

            if (flg == 1)
            {
                joken_sql1 = " WHERE " + wrk_para[0];
                sort = wrk_para[1];
            }
            else if (flg == 2)
            {
                joken_sql1 = " WHERE a.仕入先CD='" + wrk_para[0] + "' ";
            }

            string sql_query = "select A.SEQ_NO";
            sql_query += ",SUBSTR(GET_VDATE(a.VDATE_CREATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_CREATE),10,6) 作成日時";
            sql_query += ",SUBSTR(GET_VDATE(a.VDATE_UPDATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_UPDATE),10,6) 更新日時";
            sql_query += ",A.仕入先CD,A.仕入先名,A.カナ,A.旧コード,A.略称,A.郵便番号,A.住所1,A.住所2,A.住所3,A.TEL,A.FAX";
            sql_query += ",A.宛名FLG1,A.宛名FLG2,A.宛名FLG3,A.宛名名称1,A.宛名名称2";
            sql_query += ",A.掛率,A.掛率2,A.支払先CD,A.支払印刷,A.締日,A.支払予定月,A.支払予定日,A.支払方法";
            sql_query += ",A.消費税CD,A.消費税計算方法,A.消費税端数,A.支払率";
            sql_query += ",A.名称CD01,A.備考,A.伝票印字1,A.伝票印字2,A.伝票印字3,A.伝票印字4,A.振込銀行,A.振込支店,A.振込種別,A.振込口座,A.備考2,A.発注FLG";
            sql_query += ",A.伝票発行区分,A.部門,A.名称CD02,A.名称CD03,A.名称CD04,A.名称CD05,A.名称CD06,A.名称CD07,A.名称CD08,A.名称CD09,A.名称CD10";
            sql_query += ",A.生産FLG,A.為替区分,A.為替桁切指定,A.為替端数区分,A.発注停止FLG,A.仕入区分";
            sql_query += ",法人CD,連携先法人CD,連携CD";  /* 2010.09.21 追加 */
            sql_query += ",期日,下限額"; /* 2012.02.20 追加 USER87対応 */
            sql_query += ",締日2,支払予定月2,支払予定日2,締日3,支払予定月3,支払予定日3";    /* 2020.10.05 #58551対応追加 */
            sql_query += ",仕入先MAIL";    /* #51907対応追加 */
            sql_query += ",A.登録番号"; /* 2023.12.22 #70586対応追加 */

            /* コードの空き確保処理 */
            for (var i = max_col; i < 150; i++)
            {
                sql_query += " ,'' DummyCD" + i.ToString("000");
            }

            /* 可変処理対応SQL */
            sql_query += ",NVL((select H.仕入先名 from HC$Master_SIIRE H where H.仕入先CD=A.支払先CD),'.') 支払先名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='D01' and H.名称CD=A.名称CD01),'.') 分類01名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='D02' and H.名称CD=A.名称CD02),'.') 分類02名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='D03' and H.名称CD=A.名称CD03),'.') 分類03名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='D04' and H.名称CD=A.名称CD04),'.') 分類04名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='D05' and H.名称CD=A.名称CD05),'.') 分類05名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='D06' and H.名称CD=A.名称CD06),'.') 分類06名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='D07' and H.名称CD=A.名称CD07),'.') 分類07名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='D08' and H.名称CD=A.名称CD08),'.') 分類08名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='D09' and H.名称CD=A.名称CD09),'.') 分類09名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='D10' and H.名称CD=A.名称CD10),'.') 分類10名";
            sql_query += ",(A.入力社員CD ||' '|| (select B.名前 from HC$MASTER_SHAIN B where B.社員CD=A.入力社員CD)) 最終修正者";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='BMN' and H.名称CD=A.部門),'.') 部門名";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='D01'),'.') title1";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='D02'),'.') title2";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='D03'),'.') title3";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='D04'),'.') title4";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='D05'),'.') title5";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='D06'),'.') title6";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='D07'),'.') title7";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='D08'),'.') title8";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='D09'),'.') title9";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='D10'),'.') title10";
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='RAT' and H.名称CD=A.為替区分),'.') 為替区分名";

            /* 画面上ComboBoxの文字列取得 */
            sql_query += "," + comboItem00.GetCaseStr("仕入先仕入区分", "A.仕入区分") + "仕入区分名";
            sql_query += "," + comboItem00.GetCaseStr("する", "A.発注FLG") + "発注停止";
            sql_query += "," + comboItem00.GetCaseStr("する", "A.支払印刷") + "支払印刷名";
            sql_query += "," + comboItem00.GetCaseStr("締日", "A.締日") + "締日名";
            sql_query += "," + comboItem00.GetCaseStr("予定月", "A.支払予定月") + "支払予定月名";
            sql_query += "," + comboItem00.GetCaseStr("入金区分2", "A.支払方法") + "支払方法名";

            /* 卸対応 10.08.31 */
            if (AppData.ClassCvnet.config.oroshi == 0)
            {
                sql_query += ",CASE WHEN (A.消費税CD=0) THEN '0 非課税' WHEN (A.消費税CD=1) THEN '1 課税' ELSE '.' END  消費税CD名";
            }
            else
            {
                sql_query += ",CASE WHEN (A.消費税CD=0) THEN '0 非課税' WHEN (A.消費税CD=1) THEN '1 外税' WHEN (A.消費税CD=2) THEN '2 内税' ELSE '.' END  消費税CD名";
            }

            sql_query += "," + comboItem00.GetCaseStr("端数", "A.消費税端数") + "消費税端数名";
            sql_query += "," + comboItem00.GetCaseStr("消費税計算2", "A.消費税計算方法") + "消費税計算名";
            sql_query += "," + comboItem00.GetCaseStr("為替桁切", "A.為替桁切指定") + "為替桁切指定名";
            sql_query += "," + comboItem00.GetCaseStr("端数", "A.為替端数区分") + "為替端数区分名";

            /* 2020.10.05 #58851対応追加 */
            sql_query += "," + comboItem00.GetCaseStr("締日2", "A.締日2") + "締日2名";
            sql_query += "," + comboItem00.GetCaseStr("予定月", "A.支払予定月2") + "支払予定月2名";
            sql_query += "," + comboItem00.GetCaseStr("締日2", "A.締日3") + "締日3名";
            sql_query += "," + comboItem00.GetCaseStr("予定月", "A.支払予定月3") + "支払予定月3名";

            sql_query += " from HC$Master_SIIRE A";
            sql_query += joken_sql1;
            sql_query += sort;

            sql_query = "select * from (" + sql_query + ")";

            var qfm_name = "cvnet_siire_v2.qfm";

            /* 卸対応　10.04.27 */
            if (AppData.ClassCvnet.config.oroshi != 0) qfm_name = "cvnet_siire_w.qfm";

            return AppData.Http?.AspxSqlQuery(sql_query, wrk_para2, qfm_name);
        }


        public string Get_KokyakuCsvQuery(string sql_str, int flg)
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

        public object FindChild(string propertyName)
        {
            var prop = this.GetType().GetProperty(propertyName,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.IgnoreCase);

            if (prop != null)
            {
                return prop.GetValue(this);
            }
            return null;
        }

        // FindChild but return strongly typed reference for update
        public bool SetChild(string propertyName, object value)
        {
            var prop = this.GetType().GetProperty(propertyName,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.IgnoreCase);

            if (prop != null && prop.CanWrite)
            {
                // Convert value type if needed
                var convertedValue = Convert.ChangeType(value, prop.PropertyType);
                prop.SetValue(this, convertedValue);
                return true;
            }
            return false;
        }
    }

    public class MstItem
    {
        public string v_mstname { get; set; } = string.Empty;
        public string[] v_para { get; set; } = null;
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

        // Generic FindChild (string property name)
        public object FindChild(string propertyName)
        {
            var prop = this.GetType().GetProperty(propertyName,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.IgnoreCase);

            if (prop != null)
            {
                return prop.GetValue(this);
            }
            return null;
        }

        // FindChild but return strongly typed reference for update
        public bool SetChild(string propertyName, object value)
        {
            var prop = this.GetType().GetProperty(propertyName,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.IgnoreCase);

            if (prop != null && prop.CanWrite)
            {
                // Convert value type if needed
                var convertedValue = Convert.ChangeType(value, prop.PropertyType);
                prop.SetValue(this, convertedValue);
                return true;
            }
            return false;
        }
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
                    _list.Add((TKey)(object)i.ToString("00"), i.ToString("00"));
                _list.Add((TKey)(object)"99", "99");
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

    public class SysMstTb
    {
        public DataTable _data { get; set; }

        public SysMstTb() 
        {
            _data = new DataTable();
        }

        public SysMstTb(DataTable _datatable)
        {
            _data = _datatable;
        }

        /// <summary>
        /// SysMst:自社締日取得
        /// </summary> 
        public string GetSime()
        {
            if (_data.Rows.Count == 0 || _data.Columns.Count < 34)
            {
                return "99"; // no data
            }
            return _data.Rows[0][14].ToString();
        }

        /// <summary>
        /// SysMst:期首年月日取得
        /// </summary> 
        public DateTime GetKishu()
        {
            if (_data.Rows.Count == 0 || _data.Columns.Count < 34)
            {
                return new DateTime(1901, 1, 1); // default
            } 
            if (DateTime.TryParse(_data.Rows[0][13]?.ToString(), out DateTime result))
            {
                return result;
            }
            return new DateTime(1901, 1, 1);
        }

        /// <summary>
        /// SysMst:セール掛率取得0-3
        /// </summary>
        public int GetSaleKake(int v_no)
        {
            if (_data.Rows.Count == 0 || _data.Columns.Count < 34)
            {
                return 100;
            }
            if (v_no < 0 || v_no > 3)
            {
                return 100;
            }
            return Convert.ToInt32(_data.Rows[0][24 + v_no]);
        }

        public void Clear()
        {
            _data.Clear();
        }
    }

    public class SysKintaiMstTb
    {
        public DataTable _data { get; set; }

        public SysKintaiMstTb()
        {
            _data = new DataTable();
        }

        public SysKintaiMstTb(DataTable _datatable)
        {
            _data = _datatable;
        }

        /// <summary>
        /// SysMst:自社締日取得
        /// </summary> 
        public string GetSime()
        {
            if (_data.Rows.Count == 0 || _data.Columns.Count < 34)
            {
                return "99"; // no data
            }
            return _data.Rows[0][12].ToString();
        }
    }

    public class HHTCsvTb
    {
        private readonly string hhtPath = "/hht/";
        public DataTable _data { get; set; }

        public HHTCsvTb()
        {
            _data = new DataTable("HHT_Csv");
             
            _data.Columns.Add("Flag", typeof(int));
            _data.Columns.Add("Name", typeof(string));
            _data.Columns.Add("PathPattern", typeof(string));
            _data.Columns.Add("Directory", typeof(string));
            _data.Columns.Add("FileNamePattern", typeof(string));
             
            _data.Rows.Add(0, "部門マスタ", "CSV/DOWNLOAD/bumon.dat", "CSV/DOWNLOAD/", "bumon.dat");
            _data.Rows.Add(1, "出庫実績", "CSV/Syuko/SY*.CSV", "CSV/Syuko/", "SY*.CSV");
            _data.Rows.Add(1, "入庫実績", "CSV/Nyuko/NY*.CSV", "CSV/Nyuko/", "NY*.CSV");
            _data.Rows.Add(1, "棚卸実績", "CSV/Tana/TA*.CSV", "CSV/Tana/", "TA*.CSV");
            _data.Rows.Add(1, "即時移動実績", "CSV/Move/MV*.CSV", "CSV/Move/", "MV*.CSV");
            _data.Rows.Add(1, "在庫登録実績", "CSV/Zaiko/ZA*.CSV", "CSV/Zaiko/", "ZA*.CSV");
        }

        public HHTCsvTb(DataTable _datatable)
        {
            _data = _datatable;
        }

        /// <summary>
        /// Get normalized path
        /// </summary>
        public string GetPath(string? dirStr)
        {
            if (string.IsNullOrEmpty(dirStr))
                dirStr = hhtPath;

            // Ensure directory exists
            if (dirStr == hhtPath)
            {
                Directory.CreateDirectory(hhtPath.TrimStart('/'));
            }

            string path = Path.Combine(dirStr.Trim('/'), "wrk.txt");

            // remove "wrk.txt"
            string v_ret = path.Substring(0, path.Length - "wrk.txt".Length);
            return v_ret;
        }

        /// <summary>
        /// Write environment path to /hht/hht_env.txt
        /// </summary>
        public void SetEnv(string v_outpath)
        {
            if (string.IsNullOrEmpty(v_outpath))
                return;

            // Ensure ends with backslash
            if (!v_outpath.EndsWith("\\"))
            {
                v_outpath += "\\";
            }

            Directory.CreateDirectory("hht");
            string filePath = Path.Combine("hht", "hht_env.txt");

            System.IO.File.WriteAllText(filePath, v_outpath);
        }

        /// <summary>
        /// Read environment path from /hht/hht_env.txt
        /// </summary>
        public string GetEnv()
        {
            Directory.CreateDirectory("hht");
            string filePath = Path.Combine("hht", "hht_env.txt");

            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    return System.IO.File.ReadAllText(filePath).Trim();
                }
            }
            catch (IOException ex)
            {
                // mimic Biz Designer exception logic
                throw new Exception("Failed to read hht_env.txt", ex);
            }

            return string.Empty;
        }

        /// <summary>
        /// Equivalent to OnMap (initialize private root)
        /// </summary>
        public void OnMap()
        {
            // In C#, you can define a private root path
            // Example: Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            string privateRoot = Path.Combine(Environment.CurrentDirectory, "hht");
            Directory.CreateDirectory(privateRoot);
        }
    }
}
