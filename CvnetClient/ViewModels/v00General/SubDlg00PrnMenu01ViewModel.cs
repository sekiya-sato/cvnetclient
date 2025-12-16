using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg00PrnMenu01ViewModel : BaseViewModel
    {
        [ObservableProperty]
        private MasterWorkerShopMenu? selectWorkerShop = new();
        [ObservableProperty]
        public Dictionary<string, string>? selectedCondition;
        public ObservableCollection<SelectConditionItem> SelectConditionList { get; }
        = new ObservableCollection<SelectConditionItem>();
        public ObservableCollection<string> SortConditionList { get; }
    = new ObservableCollection<string>();

        [ObservableProperty]
        private SelectConditionItem? selectedSelectCondition;
        [ObservableProperty]
        private string? selectedSortCondition;
        [ObservableProperty]
        BtListHelper findFromWorkerShopCd = new();
        [ObservableProperty]
        BtListHelper findToWorkerShopCd = new();
        [ObservableProperty]
        private string mstName = "担当";   // default

        // ラベル文字列（社員CD範囲 or 店舗CD範囲）
        [ObservableProperty]
        private string rangeCdLabel = "社員CD範囲";   // default

        public bool IsAllEmployees { get; set; }      // 全社員
        public bool IsWorkingOnly { get; set; }       // 就業者のみ

        public void OnInit(object? init_para = null, string? init_flg = null)
        {
            OnInitBase(init_para, init_flg);

            SelectConditionList.Clear();

            SelectConditionList.Add(new SelectConditionItem("0", "社員CD"));
            SelectConditionList.Add(new SelectConditionItem("1", "店舗CD"));
            // default selection:
            SelectedSelectCondition = SelectConditionList[0];
            // Optional initial default sort options
            SortConditionList.Add("社員CD順");
            SortConditionList.Add("氏名順");
        }

        private void ClearWorkerShopBindings()
        {
            if (SelectWorkerShop == null)
            {
                // ensure it’s never null, just in case OnMstNameChanged fires earlier
                SelectWorkerShop = new MasterWorkerShopMenu();
            }

            SelectWorkerShop.FromWorkerShopCd = string.Empty;
            SelectWorkerShop.FromWorkerShopName = string.Empty;
            SelectWorkerShop.ToWorkerShopCd = string.Empty;
            SelectWorkerShop.ToWorkerShopName = string.Empty;

            // clear the selected values for the list buttons:
            FindFromWorkerShopCd = null;
            FindToWorkerShopCd = null;
        }

        [RelayCommand]
        public void FindFromWorkerShop(SelValueModel value)
        {
            if (value == null) return;

            FindFromWorkerShopCd = new BtListHelper(value.Code, value.Name);

            SelectWorkerShop.FromWorkerShopCd = value.Code;
            SelectWorkerShop.FromWorkerShopName = value.Name;
            SelectWorkerShop.ToWorkerShopCd = value.Code;
            SelectWorkerShop.ToWorkerShopName = value.Name;
        }

        [RelayCommand]
        public void FindToWorkerShop(SelValueModel value)
        {
            if (value == null) return;

            FindToWorkerShopCd = new BtListHelper(value.Code, value.Name);

            SelectWorkerShop.ToWorkerShopCd = value.Code;
            SelectWorkerShop.ToWorkerShopName = value.Name;
        }

        // This method is auto-called whenever SelectedSelectCondition changes
        partial void OnSelectedSelectConditionChanged(SelectConditionItem? value)
        {
            if (value?.Code == "1")          // 店舗CD
                RangeCdLabel = "店舗CD範囲";
            else
                RangeCdLabel = "社員CD範囲";

            ClearWorkerShopBindings();
            // >>> Update sort conditions <<<
            UpdateSortConditions(value?.Code);
            // >>> Update search item <<<
            UpdateMstName(value?.Code);
        }


        // for 印刷 button
        [RelayCommand]
        async Task DoPrintAsync()
        {
            var wrk_para = BuildWrkPara();  
            string sqlQuery = "_mshain01";
            string qfmFile = "cvnet00prn_menu01.qfm";
            //Confirmation
            if (!ClientLib.MessageBox(this, "印刷しますか？")) return;
            ClientLib.CursorToWait();

             // Execute with parameters
            var ret = AppData.Http!.AspxSqlQueryCsv(sqlQuery, wrk_para.ToArray(), qfmFile, 0);
            if (ret.Split('\n').Length < 2)
            {
                ClientLib.MessageBoxError(this, "PDFデータがありません");
                return;
            }
            var ret1 = ret.Split('\n');
            var url = AppData.Http.URLroot + ret1[0] + "/data.pdf";
            await Task.Delay(1500); // PDF生成待ち
            var win = new WebpdfView();
            var vm = win.DataContext as WebpdfViewModel;
            if (vm == null) return;
            vm.Pdfdata = url;
            ClientLib.CursorToNormal();
            ClientLib.ShowDialogView(win, this);
        }

        private BizArray BuildWrkPara()
        {
            var v_para = new BizArray();

            // [0] 範囲FROM
            v_para[0] = SelectWorkerShop?.FromWorkerShopCd?.Trim() ?? "";
            // [1] 範囲TO
            v_para[1] = SelectWorkerShop?.ToWorkerShopCd?.Trim() ?? "";
            // [2] ソート条件
            v_para[2] = SelectedSortCondition?.Trim() ?? "";
            // [3] 選択条件 (0 or 1)
            v_para[3] = SelectedSelectCondition?.Code?.Trim() ?? "";
            // [4] 就業者のみ (1) / 全社員 (0)
            v_para[4] = IsWorkingOnly ? "1" : "0";

            // [5], [6], [7] MUST BE "0"
            v_para[5] = "500";
            v_para[6] = "マスタ@マスタ@マスタ@マスタ@マスタ@マスタ@マスタ@マスタ@マスタ@マスタ@マスタ@マスタ@マスタ@マスタ@マスタ@マスタ@マスタ@マスタ@マスタ@マスタ@マスタ補助@マスタ補助@マスタ補助@マスタ補助@マスタ補助@マスタ補助@マスタ補助@マスタ補助@マスタ補助@マスタ補助@マスタ補助@マスタ補助@マスタ補助@マスタ補助@マスタ補助@マスタ補助@マスタ補助@マスタ補助@マスタ補助@マスタ補助@管理メニュー@管理メニュー@管理メニュー@管理メニュー@管理メニュー@管理メニュー@管理メニュー@管理メニュー@管理メニュー@管理メニュー@管理メニュー@管理メニュー@管理メニュー@管理メニュー@管理メニュー@管理メニュー@管理メニュー@管理メニュー@管理メニュー@管理メニュー@予算@予算@予算@予算@予算@予算@予算@予算@予算@予算@予算@予算@予算@予算@予算@予算@予算@予算@予算@予算@本発注/入荷予定@本発注/入荷予定@本発注/入荷予定@本発注/入荷予定@本発注/入荷予定@本発注/入荷予定@本発注/入荷予定@本発注/入荷予定@本発注/入荷予定@本発注/入荷予定@本発注/入荷予定@本発注/入荷予定@本発注/入荷予定@本発注/入荷予定@本発注/入荷予定@本発注/入荷予定@本発注/入荷予定@本発注/入荷予定@本発注/入荷予定@本発注/入荷予定@発注@発注@発注@発注@発注@発注@発注@発注@発注@発注@発注@発注@発注@発注@発注@発注@発注@発注@発注@発注@仕入@仕入@仕入@仕入@仕入@仕入@仕入@仕入@仕入@仕入@仕入@仕入@仕入@仕入@仕入@仕入@仕入@仕入@仕入@仕入@受注/展示会@受注/展示会@受注/展示会@受注/展示会@受注/展示会@受注/展示会@受注/展示会@受注/展示会@受注/展示会@受注/展示会@受注/展示会@受注/展示会@受注/展示会@受注/展示会@受注/展示会@受注/展示会@受注/展示会@受注/展示会@受注/展示会@受注/展示会@売上@売上@売上@売上@売上@売上@売上@売上@売上@売上@売上@売上@売上@売上@売上@売上@売上@売上@売上@売上@配分@配分@配分@配分@配分@配分@配分@配分@配分@配分@配分@配分@配分@配分@配分@配分@配分@配分@配分@配分@在庫管理@在庫管理@在庫管理@在庫管理@在庫管理@在庫管理@在庫管理@在庫管理@在庫管理@在庫管理@在庫管理@在庫管理@在庫管理@在庫管理@在庫管理@在庫管理@在庫管理@在庫管理@在庫管理@在庫管理@棚卸@棚卸@棚卸@棚卸@棚卸@棚卸@棚卸@棚卸@棚卸@棚卸@棚卸@棚卸@棚卸@棚卸@棚卸@棚卸@棚卸@棚卸@棚卸@棚卸@売上分析1@売上分析1@売上分析1@売上分析1@売上分析1@売上分析1@売上分析1@売上分析1@売上分析1@売上分析1@売上分析1@売上分析1@売上分析1@売上分析1@売上分析1@売上分析1@売上分析1@売上分析1@売上分析1@売上分析1@売上分析2@売上分析2@売上分析2@売上分析2@売上分析2@売上分析2@売上分析2@売上分析2@売上分析2@売上分析2@売上分析2@売上分析2@売上分析2@売上分析2@売上分析2@売上分析2@売上分析2@売上分析2@売上分析2@売上分析2@C.P.A@C.P.A@C.P.A@C.P.A@C.P.A@C.P.A@C.P.A@C.P.A@C.P.A@C.P.A@C.P.A@C.P.A@C.P.A@C.P.A@C.P.A@C.P.A@C.P.A@C.P.A@C.P.A@C.P.A@HHT@HHT@HHT@HHT@HHT@HHT@HHT@HHT@HHT@HHT@HHT@HHT@HHT@HHT@HHT@HHT@HHT@HHT@HHT@HHT@掛管理@掛管理@掛管理@掛管理@掛管理@掛管理@掛管理@掛管理@掛管理@掛管理@掛管理@掛管理@掛管理@掛管理@掛管理@掛管理@掛管理@掛管理@掛管理@掛管理@月次/更新処理@月次/更新処理@月次/更新処理@月次/更新処理@月次/更新処理@月次/更新処理@月次/更新処理@月次/更新処理@月次/更新処理@月次/更新処理@月次/更新処理@月次/更新処理@月次/更新処理@月次/更新処理@月次/更新処理@月次/更新処理@月次/更新処理@月次/更新処理@月次/更新処理@月次/更新処理@Loyal Customer@Loyal Customer@Loyal Customer@Loyal Customer@Loyal Customer@Loyal Customer@Loyal Customer@Loyal Customer@Loyal Customer@Loyal Customer@Loyal Customer@Loyal Customer@Loyal Customer@Loyal Customer@Loyal Customer@Loyal Customer@Loyal Customer@Loyal Customer@Loyal Customer@Loyal Customer@Time-Navi@Time-Navi@Time-Navi@Time-Navi@Time-Navi@Time-Navi@Time-Navi@Time-Navi@Time-Navi@Time-Navi@Time-Navi@Time-Navi@Time-Navi@Time-Navi@Time-Navi@Time-Navi@Time-Navi@Time-Navi@Time-Navi@Time-Navi@店舗@店舗@店舗@店舗@店舗@店舗@店舗@店舗@店舗@店舗@店舗@店舗@店舗@店舗@店舗@店舗@店舗@店舗@店舗@店舗@物流@物流@物流@物流@物流@物流@物流@物流@物流@物流@物流@物流@物流@物流@物流@物流@物流@物流@物流@物流@CVPOS用帳票・免税@CVPOS用帳票・免税@CVPOS用帳票・免税@CVPOS用帳票・免税@CVPOS用帳票・免税@CVPOS用帳票・免税@CVPOS用帳票・免税@CVPOS用帳票・免税@CVPOS用帳票・免税@CVPOS用帳票・免税@CVPOS用帳票・免税@CVPOS用帳票・免税@CVPOS用帳票・免税@CVPOS用帳票・免税@CVPOS用帳票・免税@CVPOS用帳票・免税@CVPOS用帳票・免税@CVPOS用帳票・免税@CVPOS用帳票・免税@CVPOS用帳票・免税@オプション@オプション@オプション@オプション@オプション@オプション@オプション@オプション@オプション@オプション@オプション@オプション@オプション@オプション@オプション@オプション@オプション@オプション@オプション@オプション@---@---@---@---@---@---@---@---@---@---@---@---@---@---@---@---@---@---@---@---@";
            v_para[7] = "名称マスタ@@得意先マスタ@仕入先マスタ@@商品マスタ(新規登録)@商品マスタ(照会・修正）@商品マスタ(照会）@商品マスタ絵型更新@社員マスタ@上代一括変更@上代一括変更取込@売価一覧印刷@@原価変更登録@@生地付属マスタ@コンバートマスタ@@社員証印刷@名称マスタコピー作成@@取込レイアウト作成@外部CSVデータ取込@@JAN一括設定(49JAN含む)@マスタ復帰処理@商品色サイズメンテ@@下札発行用CSVデータ作成@社員使用メニュー一覧@端末UUID設定(Felica用)@@@@週マスタ@@バーコードブック印刷@各種マスタ印刷@タックシール印刷@社員LOGINマスタ(管理者用)@社員LOGINマスタ一覧@システム管理マスタ(管理者用)@名称マスタ(管理者用)@@LOGIN履歴情報(管理者用)@処理履歴情報(管理者用)@手動・自動実行履歴(管理者用)@@汎用ファイルメンテ(管理者用)@フラグメンテナンス（DTP用)@HHT用管理マスタ(DTP用)@バッチ更新履歴(DTP用)@ラベル名称マスタ(DTP用)@帳票管理マスタ(DTP用)@システムメンテナンス処理@汎用SQL問い合わせ(管理者用)@DB定義書出力@自動実行管理マスタ@自動実行スケジュール設定@店ブランド予算マスタ(月)@販売員別予算マスタ(月)@@店別ブランド別予算マスタ@販売員別予算マスタ@営業担当別予算マスタ@@@月別ﾌﾞﾗﾝﾄﾞｱｲﾃﾑ仕入予算マスタ@@店舗予算表@店舗別予算実績対比@@@販売員予算表@担当別売上予算実績半期報@@@投入計画表@@本発注入力@本発注配分入力@入荷予定入力@@@本発注実績表@入荷予定実績表@@本発注残完了設定@本発注残管理表@本発注書@本発注配分リスト@MDマップ@納品予定照会@@発注配分パターン入力@@@@仕入未受リスト@発注入力@発注配分入力@@@@発注実績表@@@発注残完了設定@発注残管理表@発注書@発注配分リスト@MDマップ@納品予定照会@@@　@　@受注発注連携更新@仕入未受リスト@商品仕入入力@生地付属仕入入力@@@@仕入実績表@ブランド別仕入金額表@消化仕入リスト@@@仕入伝票印刷@@@@@@@@@仕入先一括返品@受注入力@@@@@受注実績表@@@受注残完了設定@受注残管理表@スワッチデータ一括作成@スワッチデータメンテ@スワッチ印刷@バーコードブック発行@絵型一覧表@@得意先別売上予定表@担当別展示会受注合計表@受注ベスト表@引当確認表@出荷・売上入力@店舗売上入力@精算入力@@@出荷売上実績表@店舗売上実績表@精算レポート照会@@@納品書印刷@納品書印刷(専用伝票)@@@@@@社員別購入履歴@@催事売上入力@店舗配分入力@受注配分入力@店舗出荷依頼@在庫品配分@取置入力@@配分確定(商品)@配分確定(得意先)@配分確定後→伝票作成処理@配分データメンテ@出荷指示明細書印刷@納入一覧表@取置受付一覧表@配分チェックリスト@@@店舗配分パターン登録@配分関連メンテナンス@売上基準:自動補充対象除外品設定@在庫基準:自動補充メンテナンス@移動入力(即時)@移動入力(積送)@移動受入力@在庫強制調整入力@@移動実績表@在庫強制調整実績表@在庫問合せ@商品履歴問合せ@取引データ閲覧@即時移動明細書印刷@移動明細書印刷@@倉庫別受払表@商品別受払表@倉庫別在庫集計表@汎用在庫表@@在庫移動入力@移動未受リスト@棚卸入力@棚卸入力(一覧方式)@@@@棚卸実績表@棚卸チェックリスト@倉庫分類別棚卸表@棚卸明細表@@棚卸日一括メンテナンス@棚卸開始処理@棚卸確定@@@@@@@@販売動向表@商品別販売動向表@投入売上在庫表@ベスト表@商品消化率表@セット売上分析表@仕入先別店別売上リスト@@@@店別売上日報@店舗別売上日計表@売上速報@売上週報･月報@店舗売上ランキング表@@@@@@得意先別売上日報@得意先別売上月報@@担当得意先別予算実績対比表@@@個人売上ランキング表@販売員別予算実績対比表@@@@@全社受払表@@卸・店舗売上実績表@@@@@ @★T.L-アナライザー★@★C.G-アナライザー★@@ナンでも？CSV@@ABC分析@@@在庫データ出力@在庫受払照会@商品分析ビュー@店舗稼動ビュー@売消台帳ビュー@@@@@オンラインモニタ@@@HHT用マスタデータ作成(cvnetcom)@@@@@@HHT未更新データ印刷@HHT未更新データ一括削除@@HHT用PATH設定@HHT手動データ受信(店舗固定)@HHT手動データ受信(ﾃﾞｰﾀ送信後)@@HHTエラーデータ修正入力@HHTデータ更新@@@HHTエラーデータ削除@@HHT用マスタバーコード印刷@入金入力@入金消込@入金取込@得意先元帳@売掛金管理表@月別入金予定表@請求一覧表@請求書印刷@請求計算@売掛消費税計算@支払入力@支払消込@支払取込@仕入先元帳@買掛金管理表@月別支払予定表@支払一覧表@支払残高明細書@支払計算@買掛消費税計算@締日更新@諸掛更新@総平均原価更新@最終仕入原価更新@消化仕入更新@評価替更新(原価版)@評価替更新(元上代版)@評価替一覧印刷@@積送中クリア更新@残高登録処理@@在庫・掛再更新(管理者用)@在庫累計更新(管理者用)@消費税再計算更新(管理者用)@価格保存@商品分析集計@月間データ集計@売上基準:自動補充実行@@顧客マスタ@顧客カルテ@@顧客ランク更新（管理者用）@RFMクロス分析@デシル分析@分類分析@@ポイント集計出力@ポイント累計更新@ポイント管理メニュー@@@顧客ランク更新（管理者用new）@RFMクロス分析(new)@接客履歴@顧客マスタ(new)@@@クーポン使用一覧表@勤怠用管理マスタ@社員標準シフトメンテ@シフトデータメンテ@@@@@シフト変更データ一括登録@シフト一括登録@シフト表作成@出退勤データ承認@承認書印刷@@@@@@@@出退勤データ出力@店舗新規売上入力@店舗新規売上入力@@デシル分析@分類分析@@@@@移動指示明細書印刷@棚卸明細表(原価無)@汎用在庫表(原価無)@売上速報(原価無)@売上週報･月報(原価無)@分類別店別売上報告(原価無)@商品履歴問合せ（照会用）@@@仕入未受リスト@移動未受リスト@マスタデータ作成@@連携データ手動送信@連携データ手動受信@@連携エラーデータ照会@@@@@@@@@@@@@@@売上日報@金種集計表@収入印紙集計リスト@免税売上集計表@免税データ出力@ジャーナル照会@@@@@@@@@@@@@@@商品仕入入力(外貨版)@生地付属仕入入力(外貨版)@諸掛チェックリスト@レートマスタ@@移動指示確認@売上週報･月報(回転率)@汎用在庫表(在庫日数・回転率)@商品分析アラート設定@商品分析メッセージ送信@展示会SD入力初期設定@展示会SD入力結果出力@原価変更登録(SKU)@総平均原価更新(SKU)@評価替更新(元上代版:SKU)@評価替更新(原価版:SKU)@@@社員LOGINマスタ(管理者用)SD@@売上与信確認表@顧客離反・ﾎﾟｲﾝﾄ利用・ｸｰﾎﾟﾝ利用照会@LTV分析@CTB分析@上代一覧@配分確定@アナライザ用取引在庫データ集計@T.L@C.G@顧客対応履歴(テスト中)@店舗商品別 在庫積送売上集計@出荷指示（店間移動)@バスケット分析@トレンド分析@@店別キャッシュフロー年間推移@キャッシュフロー入力@店別キャッシュフロー表@店別キャッシュフロー一覧@得意先別配分入力@";

            return v_para;
        }

        private void UpdateSortConditions(string? code)
        {
            SortConditionList.Clear();

            if (code == "1")   // 店舗CD
            {
                SortConditionList.Add("店舗CD");
                SortConditionList.Add("略称");
                SortConditionList.Add("カナ");
            }
            else               // 社員CD
            {
                SortConditionList.Add("社員CD");
                SortConditionList.Add("名前");
                SortConditionList.Add("店舗CD");
            }

            // Reset selected item
            SelectedSortCondition = SortConditionList.FirstOrDefault();
        }

        private void UpdateMstName(string? code)
        {
            if (code == "1")        // 店舗CD
                MstName = "移動倉庫";
            else                    // 社員CD
                MstName = "担当";   // 
        }

        public partial class MasterWorkerShopMenu : ObservableObject
        {
            [ObservableProperty]
            private string? fromWorkerShopCd;
            [ObservableProperty]
            private string? fromWorkerShopName;
            [ObservableProperty]
            private string? toWorkerShopCd;
            [ObservableProperty]
            private string? toWorkerShopName;
        }

        public class SelectConditionItem
        {
            public string Code { get; set; } = "";
            public string Name { get; set; } = "";

            public SelectConditionItem(string code, string name)
            {
                Code = code;
                Name = name;
            }
        }
    }
}
