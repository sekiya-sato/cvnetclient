using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvnetClient.Models
{
    public static class PageConfig
    {
        private static Dictionary<string, PageData> pages = new()
    { 
        { "マスタ", new PageData {
            Buttons = new List<ButtonConfig> {
                new ButtonConfig { Text = "名称マスタ", WindowId= typeof(Views.SubDlg01MeiView).FullName,Parameter = "0" },
                new ButtonConfig { Text = "得意先マスタ", WindowId=typeof(Views.SubDlg01TokView).FullName},
                new ButtonConfig { Text = "仕入マスタ", WindowId="" },
                new ButtonConfig { Text = "商品マスタ(新規登録)", WindowId= typeof(Views.SubDlg01Sho2View).FullName },
                new ButtonConfig { Text = "商品マスタ(修正)", WindowId= typeof(Views.SubDlg01Sho2View).FullName },
                new ButtonConfig { Text = "商品マスタ(照会)", WindowId=typeof(Views.SubDlg01Sho2View).FullName },
                new ButtonConfig { Text = "商品マスタ絵型更新", WindowId="" },
                new ButtonConfig { Text = "社員マスタ", WindowId=typeof(Views.SubDlg01UsrView).FullName },
                new ButtonConfig { Text = "上代一括変更", WindowId="" },
                new ButtonConfig { Text = "上代一括変更取込", WindowId="" },
                new ButtonConfig { Text = "売価一覧印刷", WindowId="" },
                new ButtonConfig { Text = "原価変更登録", WindowId="" },
                new ButtonConfig { Text = "生地付属マスタ", WindowId=typeof(Views.SubDlg01KijiView).FullName },
                new ButtonConfig { Text = "コンバートマスタ", WindowId="" },
                new ButtonConfig { Text = "社員証印刷", WindowId= typeof(Views.SubDlg00UsrlistView).FullName },
            }
        }},
        { "マスタ補助", new PageData {
            Buttons = new List<ButtonConfig> {
                new ButtonConfig { Text = "名称マスタコピー作成", WindowId=typeof(Views.SubDlg01MeiConvView).FullName },
                new ButtonConfig { Text = "取込レイアウト作成", WindowId=typeof(Views.SubDlg00InputCsvView).FullName },
                new ButtonConfig { Text = "外部CSVデータ取込", WindowId="" },
                new ButtonConfig { Text = "JAN一括設定(49JAN含む)", WindowId="" },
                new ButtonConfig { Text = "マスタ復帰処理", WindowId="" },
                new ButtonConfig { Text = "商品サイズメンテ", WindowId="" },
                new ButtonConfig { Text = "下札発行用CSVデータ作成", WindowId="" },
                new ButtonConfig { Text = "社員使用メニュー一覧", WindowId="" },
                new ButtonConfig { Text = "端末UUID設定(Felica用)", WindowId="" },
                new ButtonConfig { Text = "週マスタ", WindowId=typeof(Views.SubDlg01WeekView).FullName },
                new ButtonConfig { Text = "バーコードブック印刷", WindowId="" },
                new ButtonConfig { Text = "各種マスタ印刷", WindowId="" },
                new ButtonConfig { Text = "タックシール印刷", WindowId="" }
            }
        }},
        { "管理メニュー", new PageData {
            Buttons = new List<ButtonConfig> {
                new ButtonConfig { Text = "社員LOGINマスタ(管理者用)", WindowId="" },
                new ButtonConfig { Text = "社員LOGINマスタ一覧", WindowId="" },
                new ButtonConfig { Text = "システム管理マスタ(管理者用)", WindowId= typeof(Views.MasterSystemKanriView1).FullName },
                new ButtonConfig { Text = "名称マスタ(管理者用)", WindowId= typeof(Views.SubDlg01MeiView).FullName,Parameter = "1" },
                new ButtonConfig { Text = "LOGIN履歴情報(管理者用)", WindowId="" },
                new ButtonConfig { Text = "処理履歴情報(管理者用)", WindowId="" },
                new ButtonConfig { Text = "手動・自動実行履歴(管理者用)", WindowId="" },
                new ButtonConfig { Text = "汎用ファイルメンテ(管理者用)", WindowId="" },
                new ButtonConfig { Text = "フラグメンテナンス（DTP用)", WindowId="" },
                new ButtonConfig { Text = "HHT用管理マスタ(DTP用)", WindowId="" },
                new ButtonConfig { Text = "バッチ更新履歴(DTP用)", WindowId="" },
                new ButtonConfig { Text = "ラベル名称マスタ(DTP用)", WindowId="" },
                new ButtonConfig { Text = "帳票管理マスタ(DTP用)", WindowId="" },
                new ButtonConfig { Text = "システムメンテナンス処理", WindowId="" },
                new ButtonConfig { Text = "汎用SQL問い合わせ(管理者用)", WindowId="" },
                new ButtonConfig { Text = "DB定義書出力", WindowId="" },
                new ButtonConfig { Text = "自動実行管理マスタ", WindowId="" },
                new ButtonConfig { Text = "自動実行スケジュール設定", WindowId="" }
            }
        }},
        { "予算", new PageData {
            Buttons = new List<ButtonConfig> {
                new ButtonConfig { Text = "店ブランド予算マスタ(月)", WindowId="" },
                new ButtonConfig { Text = "販売員別予算マスタ(月)", WindowId="" },
                new ButtonConfig { Text = "店別ブランド別予算マスタ", WindowId="" },
                new ButtonConfig { Text = "販売員別予算マスタ", WindowId="" },
                new ButtonConfig { Text = "営業担当別予算マスタ", WindowId="" },
                new ButtonConfig { Text = "月別ﾌﾞﾗﾝﾄﾞｱｲﾃﾑ仕入予算マスタ", WindowId=typeof(Views.SubDlg30InpMbiView).FullName },
                new ButtonConfig { Text = "店舗予算表", WindowId="" },
                new ButtonConfig { Text = "店舗別予算実績対比", WindowId="" },
                new ButtonConfig { Text = "販売員予算表", WindowId="" },
                new ButtonConfig { Text = "担当別売上予算実績半期報", WindowId="" },
                new ButtonConfig { Text = "投入計画表", WindowId="" }
            }
        }},
        { "本発注", new PageData {
            Buttons = new List<ButtonConfig> {
                new ButtonConfig { Text = "本発注入力", WindowId="" },
                new ButtonConfig { Text = "本発注配分入力", WindowId="" },
                new ButtonConfig { Text = "入荷予定入力", WindowId="" },
                new ButtonConfig { Text = "本発注実績表", WindowId="" },
                new ButtonConfig { Text = "入荷予定実績表", WindowId="" },
                new ButtonConfig { Text = "本発注残完了設定", WindowId="" },
                new ButtonConfig { Text = "本発注残管理表", WindowId="" },
                new ButtonConfig { Text = "本発注書", WindowId="" },
                new ButtonConfig { Text = "本発注配分リスト", WindowId="" },
                new ButtonConfig { Text = "MDマップ", WindowId="" },
                new ButtonConfig { Text = "納品予定照会", WindowId="" },
                new ButtonConfig { Text = "発注配分パターン入力", WindowId="" },
                new ButtonConfig { Text = "仕入未受リスト", WindowId="" }
            }
        }},
        { "発注", new PageData {
            Buttons = new List<ButtonConfig> {
                new ButtonConfig { Text = "発注入力", WindowId=typeof(Views.SubDlg13HbnhtykNewView).FullName },
                new ButtonConfig { Text = "発注配分入力", WindowId="" },
                new ButtonConfig { Text = "発注実績表", WindowId="" },
                new ButtonConfig { Text = "発注残完了設定", WindowId="" },
                new ButtonConfig { Text = "発注残管理表", WindowId="" },
                new ButtonConfig { Text = "発注書", WindowId="" },
                new ButtonConfig { Text = "発注配分リスト", WindowId="" },
                new ButtonConfig { Text = "MDマップ", WindowId="" },
                new ButtonConfig { Text = "納品予定照会", WindowId="" },
                new ButtonConfig { Text = "受注発注連携更新", WindowId="" },
                new ButtonConfig { Text = "仕入未受リスト", WindowId="" }
            }
        }},
        { "仕入", new PageData {
            Buttons = new List<ButtonConfig> {
                new ButtonConfig { Text = "商品仕入入力", WindowId=typeof(Views.ShiireInputView).FullName },
                new ButtonConfig { Text = "生地付属仕入入力", WindowId="" },
                new ButtonConfig { Text = "仕入実績表", WindowId="" },
                new ButtonConfig { Text = "ブランド別仕入金額表", WindowId="" },
                new ButtonConfig { Text = "消化仕入リスト", WindowId="" },
                new ButtonConfig { Text = "仕入伝票印刷", WindowId="" },
                new ButtonConfig { Text = "仕入先一括返品", WindowId="" }
            }
        }},
        { "展示会", new PageData {
            Buttons = new List<ButtonConfig> {
                new ButtonConfig { Text = "受注入力", WindowId="" },
                new ButtonConfig { Text = "受注実績表", WindowId="" },
                new ButtonConfig { Text = "受注残完了設定", WindowId="" },
                new ButtonConfig { Text = "受注残管理表", WindowId="" },
                new ButtonConfig { Text = "スワッチデータ一括作成", WindowId="" },
                new ButtonConfig { Text = "スワッチデータメンテ", WindowId="" },
                new ButtonConfig { Text = "スワッチ印刷", WindowId="" },
                new ButtonConfig { Text = "バーコードブック発行", WindowId="" },
                new ButtonConfig { Text = "絵型一覧表", WindowId="" },
                new ButtonConfig { Text = "得意先別売上予定表", WindowId="" },
                new ButtonConfig { Text = "担当別展示会受注合計表", WindowId="" },
                new ButtonConfig { Text = "受注ベスト表", WindowId="" },
                new ButtonConfig { Text = "引当確認表", WindowId="" }
            }
        }},
        { "売上", new PageData {
            Buttons = new List<ButtonConfig> {
                new ButtonConfig { Text = "出荷・売上入力", WindowId="" },
                new ButtonConfig { Text = "店舗売上入力", WindowId="" },
                new ButtonConfig { Text = "精算入力", WindowId="" },
                new ButtonConfig { Text = "出荷売上実績表", WindowId="" },
                new ButtonConfig { Text = "店舗売上実績表", WindowId="" },
                new ButtonConfig { Text = "精算レポート照会", WindowId="" },
                new ButtonConfig { Text = "納品書印刷", WindowId="" },
                new ButtonConfig { Text = "納品書印刷(専用伝票)", WindowId="" },
                new ButtonConfig { Text = "社員別購入履歴", WindowId="" },
                new ButtonConfig { Text = "催事売上入力", WindowId="" }
            }
        }},
        { "配分", new PageData {
            Buttons = new List<ButtonConfig> {
                new ButtonConfig { Text = "店舗配分入力", WindowId="" },
                new ButtonConfig { Text = "受注配分入力", WindowId="" },
                new ButtonConfig { Text = "店舗出荷依頼", WindowId="" },
                new ButtonConfig { Text = "在庫品配分", WindowId="" },
                new ButtonConfig { Text = "取置入力", WindowId="" },
                new ButtonConfig { Text = "配分確定(商品)", WindowId="" },
                new ButtonConfig { Text = "配分確定(得意先)", WindowId="" },
                new ButtonConfig { Text = "配分確定後→伝票作成処理", WindowId="" },
                new ButtonConfig { Text = "配分データメンテ", WindowId="" },
                new ButtonConfig { Text = "出荷指示明細書印刷", WindowId="" },
                new ButtonConfig { Text = "納入一覧表", WindowId="" },
                new ButtonConfig { Text = "取置受付一覧表", WindowId="" },
                new ButtonConfig { Text = "配分チェックリスト", WindowId="" },
                new ButtonConfig { Text = "店舗配分パターン登録", WindowId="" },
                new ButtonConfig { Text = "配分関連メンテナンス", WindowId="" },
                new ButtonConfig { Text = "売上基準:自動補充対象除外品設定", WindowId="" },
                new ButtonConfig { Text = "在庫基準:自動補充メンテナンス", WindowId="" }
            }
        }},
        { "在庫管理", new PageData {
            Buttons = new List<ButtonConfig> {
                new ButtonConfig { Text = "移動入力(即時)", WindowId="" },
                new ButtonConfig { Text = "移動入力(積送)", WindowId="" },
                new ButtonConfig { Text = "移動受入力", WindowId="" },
                new ButtonConfig { Text = "在庫強制調整入力", WindowId="" },
                new ButtonConfig { Text = "移動実績表", WindowId="" },
                new ButtonConfig { Text = "在庫強制調整実績表", WindowId="" },
                new ButtonConfig { Text = "在庫問合せ", WindowId="" },
                new ButtonConfig { Text = "商品履歴問合せ", WindowId="" },
                new ButtonConfig { Text = "取引データ閲覧", WindowId="" },
                new ButtonConfig { Text = "即時移動明細書印刷", WindowId="" },
                new ButtonConfig { Text = "移動明細書印刷", WindowId="" },
                new ButtonConfig { Text = "倉庫別受払表", WindowId="" },
                new ButtonConfig { Text = "商品別受払表", WindowId="" },
                new ButtonConfig { Text = "倉庫別在庫集計表", WindowId="" },
                new ButtonConfig { Text = "汎用在庫表", WindowId="" },
                new ButtonConfig { Text = "在庫移動入力", WindowId="" },
                new ButtonConfig { Text = "移動未受リスト", WindowId="" }
            }
        }},
        { "棚卸", new PageData {
            Buttons = new List<ButtonConfig> {
                new ButtonConfig { Text = "棚卸入力", WindowId="" },
                new ButtonConfig { Text = "棚卸入力(一覧方式)", WindowId="" },
                new ButtonConfig { Text = "棚卸実績表", WindowId="" },
                new ButtonConfig { Text = "棚卸チェックリスト", WindowId="" },
                new ButtonConfig { Text = "倉庫分類別棚卸表", WindowId="" },
                new ButtonConfig { Text = "棚卸明細表", WindowId="" },
                new ButtonConfig { Text = "棚卸日一括メンテナンス", WindowId="" },
                new ButtonConfig { Text = "棚卸開始処理", WindowId="" },
                new ButtonConfig { Text = "棚卸確定", WindowId="" }
            }
        }},
        { "売上分析1", new PageData {
            Buttons = new List<ButtonConfig> {
                new ButtonConfig { Text = "販売動向表", WindowId="" },
                new ButtonConfig { Text = "商品別販売動向表", WindowId="" },
                new ButtonConfig { Text = "投入売上在庫表", WindowId="" },
                new ButtonConfig { Text = "ベスト表", WindowId="" },
                new ButtonConfig { Text = "商品消化率表", WindowId="" },
                new ButtonConfig { Text = "セット売上分析表", WindowId="" },
                new ButtonConfig { Text = "仕入先別店別売上リスト", WindowId="" },
                new ButtonConfig { Text = "店別売上日報", WindowId="" },
                new ButtonConfig { Text = "店舗別売上日計表", WindowId="" },
                new ButtonConfig { Text = "売上速報", WindowId="" },
                new ButtonConfig { Text = "売上週報･月報", WindowId="" },
                new ButtonConfig { Text = "店舗売上ランキング表", WindowId="" }
            }
        }},
        { "売上分析2", new PageData {
            Buttons = new List<ButtonConfig> {
                new ButtonConfig { Text = "得意先別売上日報", WindowId="" },
                new ButtonConfig { Text = "得意先別売上月報", WindowId="" },
                new ButtonConfig { Text = "担当得意先別予算実績対比表", WindowId="" },
                new ButtonConfig { Text = "個人売上ランキング表", WindowId="" },
                new ButtonConfig { Text = "販売員別予算実績対比表", WindowId="" },
                new ButtonConfig { Text = "全社受払表", WindowId="" },
                new ButtonConfig { Text = "卸・店舗売上実績表", WindowId="" },
                new ButtonConfig { Text = "", WindowId="" }
            }
        }},
        { "C.P.A", new PageData {
            Buttons = new List<ButtonConfig> {
                new ButtonConfig { Text = "★T.L-アナライザー★", WindowId="" },
                new ButtonConfig { Text = "★C.G-アナライザー★", WindowId="" },
                new ButtonConfig { Text = "ナンでも？CSV", WindowId="" },
                new ButtonConfig { Text = "ABC分析", WindowId=typeof(Views.SubDlg80GphABC2View).FullName },
                new ButtonConfig { Text = "在庫データ出力", WindowId="" },
                new ButtonConfig { Text = "在庫受払照会", WindowId="" },
                new ButtonConfig { Text = "商品分析ビュー", WindowId="" },
                new ButtonConfig { Text = "店舗稼動ビュー", WindowId="" },
                new ButtonConfig { Text = "売消台帳ビュー", WindowId="" },
                new ButtonConfig { Text = "オンラインモニタ", WindowId="" }
            }
        }},
        { "HHT", new PageData {
            Buttons = new List<ButtonConfig> {
                new ButtonConfig { Text = "HHT用マスタデータ作成(cvnetcom)", WindowId="" },
                new ButtonConfig { Text = "HHT未更新データ印刷", WindowId="" },
                new ButtonConfig { Text = "HHT未更新データ一括削除", WindowId="" },
                new ButtonConfig { Text = "HHT用PATH設定", WindowId="" },
                new ButtonConfig { Text = "HHT手動データ受信(店舗固定)", WindowId="" },
                new ButtonConfig { Text = "HHT手動データ受信(ﾃﾞｰﾀ送信後)", WindowId="" },
                new ButtonConfig { Text = "HHTエラーデータ修正入力", WindowId="" },
                new ButtonConfig { Text = "HHTデータ更新", WindowId="" },
                new ButtonConfig { Text = "HHTエラーデータ削除", WindowId="" },
                new ButtonConfig { Text = "HHT用マスタバーコード印刷", WindowId="" }
            }
        }},
        { "掛管理", new PageData {
            Buttons = new List<ButtonConfig> {
                new ButtonConfig { Text = "入金入力", WindowId="" },
                new ButtonConfig { Text = "入金消込", WindowId="" },
                new ButtonConfig { Text = "入金取込", WindowId="" },
                new ButtonConfig { Text = "得意先元帳", WindowId="" },
                new ButtonConfig { Text = "売掛金管理表", WindowId="" },
                new ButtonConfig { Text = "月別入金予定表", WindowId="" },
                new ButtonConfig { Text = "請求一覧表", WindowId="" },
                new ButtonConfig { Text = "請求書印刷", WindowId="" },
                new ButtonConfig { Text = "請求計算", WindowId="" },
                new ButtonConfig { Text = "売掛消費税計算", WindowId="" },
                new ButtonConfig { Text = "支払入力", WindowId="" },
                new ButtonConfig { Text = "支払消込", WindowId="" },
                new ButtonConfig { Text = "支払取込", WindowId="" },
                new ButtonConfig { Text = "仕入先元帳", WindowId="" },
                new ButtonConfig { Text = "買掛金管理表", WindowId="" },
                new ButtonConfig { Text = "月別支払予定表", WindowId="" },
                new ButtonConfig { Text = "支払一覧表", WindowId="" },
                new ButtonConfig { Text = "支払残高明細書", WindowId="" },
                new ButtonConfig { Text = "支払計算", WindowId="" },
                new ButtonConfig { Text = "買掛消費税計算", WindowId="" }
            }
        }},
        { "月次", new PageData {
            Buttons = new List<ButtonConfig> {
                new ButtonConfig { Text = "締日更新", WindowId="" },
                new ButtonConfig { Text = "諸掛更新", WindowId="" },
                new ButtonConfig { Text = "総平均原価更新", WindowId="" },
                new ButtonConfig { Text = "最終仕入原価更新", WindowId="" },
                new ButtonConfig { Text = "消化仕入更新", WindowId="" },
                new ButtonConfig { Text = "評価替更新(原価版)", WindowId="" },
                new ButtonConfig { Text = "評価替更新(元上代版)", WindowId="" },
                new ButtonConfig { Text = "評価替一覧印刷", WindowId="" },
                new ButtonConfig { Text = "積送中クリア更新", WindowId="" },
                new ButtonConfig { Text = "残高登録処理", WindowId="" },
                new ButtonConfig { Text = "在庫・掛再更新(管理者用)", WindowId="" },
                new ButtonConfig { Text = "在庫累計更新(管理者用)", WindowId="" },
                new ButtonConfig { Text = "消費税再計算更新(管理者用)", WindowId="" },
                new ButtonConfig { Text = "価格保存", WindowId="" },
                new ButtonConfig { Text = "商品分析集計", WindowId="" },
                new ButtonConfig { Text = "月間データ集計", WindowId="" },
                new ButtonConfig { Text = "売上基準:自動補充実行", WindowId="" }
            }
        }},
        { "Loyal", new PageData {
            Buttons = new List<ButtonConfig> {
                new ButtonConfig { Text = "顧客マスタ", WindowId="" },
                new ButtonConfig { Text = "顧客カルテ", WindowId="" },
                new ButtonConfig { Text = "顧客ランク更新（管理者用）", WindowId="" },
                new ButtonConfig { Text = "RFMクロス分析", WindowId="" },
                new ButtonConfig { Text = "デシル分析", WindowId="" },
                new ButtonConfig { Text = "分類分析", WindowId="" },
                new ButtonConfig { Text = "ポイント集計出力", WindowId="" },
                new ButtonConfig { Text = "ポイント累計更新", WindowId="" },
                new ButtonConfig { Text = "ポイント管理メニュー", WindowId="" },
                new ButtonConfig { Text = "顧客ランク更新（管理者用new）", WindowId="" },
                new ButtonConfig { Text = "RFMクロス分析(new)", WindowId="" },
                new ButtonConfig { Text = "クーポン使用一覧表", WindowId="" }
            }
        }},
        { "Time", new PageData {
            Buttons = new List<ButtonConfig> {
                new ButtonConfig { Text = "勤怠用管理マスタ", WindowId="" },
                new ButtonConfig { Text = "社員標準シフトメンテ", WindowId="" },
                new ButtonConfig { Text = "シフト変更データ一括登録", WindowId="" },
                new ButtonConfig { Text = "シフト一括登録", WindowId="" },
                new ButtonConfig { Text = "シフト表作成", WindowId="" },
                new ButtonConfig { Text = "出退勤データ承認", WindowId="" },
                new ButtonConfig { Text = "承認書印刷", WindowId="" },
                new ButtonConfig { Text = "出退勤データ出力", WindowId="" }
            }
        }},
        { "店舗", new PageData {
            Buttons = new List<ButtonConfig> {
                new ButtonConfig { Text = "店舗新規売上入力", WindowId="" },
                new ButtonConfig { Text = "店舗新規売上入力", WindowId="" },
                new ButtonConfig { Text = "デシル分析", WindowId="" },
                new ButtonConfig { Text = "分類分析", WindowId="" },
                new ButtonConfig { Text = "移動指示明細書印刷", WindowId="" },
                new ButtonConfig { Text = "棚卸明細表(原価無)", WindowId="" },
                new ButtonConfig { Text = "汎用在庫表(原価無)", WindowId="" },
                new ButtonConfig { Text = "売上速報(原価無)", WindowId="" },
                new ButtonConfig { Text = "売上週報･月報(原価無)", WindowId="" },
                new ButtonConfig { Text = "分類別店別売上報告(原価無)", WindowId="" },
                new ButtonConfig { Text = "商品履歴問合せ（照会用）", WindowId="" },
                new ButtonConfig { Text = "移動未受リスト", WindowId="" }
            }
        }},
        { "物流", new PageData {
            Buttons = new List<ButtonConfig> {
                new ButtonConfig { Text = "マスタデータ作成", WindowId="" },
                new ButtonConfig { Text = "連携データ手動送信", WindowId="" },
                new ButtonConfig { Text = "連携データ手動受信", WindowId="" },
                new ButtonConfig { Text = "連携エラーデータ照会", WindowId="" }
            }
        }},
        { "CVPOS", new PageData {
            Buttons = new List<ButtonConfig> {
                new ButtonConfig { Text = "売上日報", WindowId="" },
                new ButtonConfig { Text = "金種集計表", WindowId="" },
                new ButtonConfig { Text = "収入印紙集計リスト", WindowId="" },
                new ButtonConfig { Text = "免税売上集計表", WindowId="" },
                new ButtonConfig { Text = "免税データ出力", WindowId="" },
                new ButtonConfig { Text = "ジャーナル照会", WindowId="" }
            }
        }},
        { "オプション", new PageData {
            Buttons = new List<ButtonConfig> {
                new ButtonConfig { Text = "商品仕入入力(外貨版)", WindowId="" },
                new ButtonConfig { Text = "生地付属仕入入力(外貨版)", WindowId="" },
                new ButtonConfig { Text = "諸掛チェックリスト", WindowId="" },
                new ButtonConfig { Text = "レートマスタ", WindowId="" },
                new ButtonConfig { Text = "移動指示確認", WindowId="" },
                new ButtonConfig { Text = "売上週報･月報(回転率)", WindowId="" },
                new ButtonConfig { Text = "汎用在庫表(在庫日数・回転率)", WindowId="" },
                new ButtonConfig { Text = "商品分析アラート設定", WindowId="" },
                new ButtonConfig { Text = "商品分析メッセージ送信", WindowId="" },
                new ButtonConfig { Text = "展示会SD入力初期設定", WindowId="" },
                new ButtonConfig { Text = "展示会SD入力結果出力", WindowId="" },
                new ButtonConfig { Text = "原価変更登録(SKU)", WindowId="" },
                new ButtonConfig { Text = "総平均原価更新(SKU)", WindowId="" },
                new ButtonConfig { Text = "評価替更新(元上代版:SKU)", WindowId="" },
                new ButtonConfig { Text = "評価替更新(原価版:SKU)", WindowId="" },
                new ButtonConfig { Text = "社員LOGINマスタ(管理者用)SD", WindowId="" }
            }
        }},
        { "---", new PageData {
            Buttons = new List<ButtonConfig> {
                new ButtonConfig { Text = "売上与信確認表", WindowId="" },
                new ButtonConfig { Text = "顧客離反・ﾎﾟｲﾝﾄ利用・ｸｰﾎﾟﾝ利用照会", WindowId="" },
                new ButtonConfig { Text = "LTV分析", WindowId="" },
                new ButtonConfig { Text = "CTB分析", WindowId="" },
                new ButtonConfig { Text = "上代一覧", WindowId="" },
                new ButtonConfig { Text = "配分確定", WindowId="" },
                new ButtonConfig { Text = "アナライザ用取引在庫データ集計", WindowId="" },
                new ButtonConfig { Text = "T.L", WindowId="" },
                new ButtonConfig { Text = "C.G", WindowId="" },
                new ButtonConfig { Text = "顧客対応履歴(テスト中)", WindowId="" },
                new ButtonConfig { Text = "店舗商品別 在庫積送売上集計", WindowId="" },
                new ButtonConfig { Text = "出荷指示（店間移動)", WindowId="" },
                new ButtonConfig { Text = "トレンド分析", WindowId="" },
                new ButtonConfig { Text = "店別キャッシュフロー年間推移", WindowId="" },
                new ButtonConfig { Text = "キャッシュフロー入力", WindowId="" },
                new ButtonConfig { Text = "店別キャッシュフロー表", WindowId="" },
                new ButtonConfig { Text = "店別キャッシュフロー一覧", WindowId="" },
                new ButtonConfig { Text = "得意先別配分入力", WindowId="" }
            }
        }}
    };
        public static PageData GetPage(string id) => pages.ContainsKey(id) ? pages[id] : new PageData();
    }

    public class PageData
    {
        public List<ButtonConfig> Buttons { get; set; } = new();
    }

    public class ButtonConfig
    {
        public string Text { get; set; }
        public string WindowId { get; set; }
        public object? Parameter { get; set; }
    }

}
