/* ============================================================================
 * Cvnet8wpfclient.exe : MenuData.cs
 * Created by Sekiya.Sato 2024/09/01
 * 説明: メインメニューの定義
 * [Description: Definition of main menu]
 * 使用ライブラリ [Library used]:
 *		CommunityToolkit.Mvvm : LICENCE = MIT
 * ============================================================================  */
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvnetClient.Models {
	public partial class MenuData : ObservableObject {
		public static Action<MenuData>? SelectSelectedChangedAction;
		[ObservableProperty]
		string? m_Header;
		[ObservableProperty]
		bool m_isSelected;
		[ObservableProperty]
		ObservableCollection<MenuData>? m_SubItems;
		[ObservableProperty]
		string m_Color = "Black";
		[ObservableProperty]
		string? m_AddInfo;
		[ObservableProperty]
		string? m_Icon;

		/* ViewModelで使用するフィールド */
		/* [Fields used in the ViewModel] */
		public int InitParam;
		public Type? ViewType;
		public bool IsDialog = false;

		static public List<MenuData> Initmenu() { // ctl+Hで^\s*\nを正規表現検索して空行を削除 
			var menu = new List<MenuData>();
			menu = [
				new MenuData {
					Header = "■ マスタ",
					Color = "Blue",
					SubItems=[
						new MenuData{
							Header="名称マスタメンテ",
							ViewType = typeof(Views.MasterMeishoView),
							IsDialog = false,
						},
						new MenuData{
							Header="得意先マスタメンテ",
							IsDialog = false,
						},
						new MenuData{
							Header="仕入先マスタメンテ",
							IsDialog = false,
						},
							new MenuData{
							Header="商品マスタ(新規登録) / (照会・修正）",
						},
							new MenuData{
							Header="商品マスタ絵型更新",
						},
							new MenuData{
							Header="生地付属マスタ",
							IsDialog = false,
						},
							new MenuData{
							Header="上代一括変更",
						},
							new MenuData{
							Header="上代一括変更取込",
						},
							new MenuData{
							Header="売価一覧印刷",
						},
							new MenuData{
							Header="原価変更登録",
						},
							new MenuData{
							Header="評価替",
						},
							new MenuData{
							Header="評価替（原価基準)",
						},
							new MenuData{
							Header="コンバートマスタ",
						},
							new MenuData{
							Header="社員マスタ",
							IsDialog = false,
						},
							new MenuData{
							Header="社員証印刷(Felica用)",
						},
					]
				},
				new MenuData {
					Header = "■ マスタ補助",
					Color = "Blue",
					SubItems=[
						new MenuData{
							Header="名称マスタコピー作成",
						},
						new MenuData{
							Header="タックシール印刷",
						},
						new MenuData{
							Header="取込レイアウト作成",
						},
						new MenuData{
							Header="外部CSVデータ取込",
						},
						new MenuData{
							Header="下札発行用CSVデータ作成",
						},
						new MenuData{
							Header="JAN一括設定(49JAN含む)",
						},
						new MenuData{
							Header="マスタ復帰処理",
						},
						new MenuData{
							Header="商品色サイズメンテ",
						},
						new MenuData{
							Header="社員使用メニュー一覧",
						},
						new MenuData{
							Header="端末UUID設定(Felica用)",
						},
						new MenuData{
							Header="週マスタ",
						},
						new MenuData{
							Header="バーコードブック",
						},
						new MenuData{
							Header="各種マスタ印刷",
						},
						new MenuData{
							Header="各種伝票印刷",
						},
					]
				},
				new MenuData {
					Header = "■ 管理メニュー",
					Color = "Red",
					SubItems=[
						new MenuData{
							Header="社員LOGINマスタ(管理者用)",
							IsDialog = false,
						},
						new MenuData{
							Header="HHT用管理マスタ",
						},
						new MenuData{
							Header="システム管理マスタ(管理者用)",
							ViewType = typeof(Views.MasterSystemKanriView),
							IsDialog = false,
						},
						new MenuData{
							Header="社員LOGINマスタ一覧",
						},
						new MenuData{
							Header="名称マスタ(管理者用)",
						},
						new MenuData{
							Header="処理履歴情報(管理者用)",
						},
						new MenuData{
							Header="自動実行履歴(管理者用)",
						},
						new MenuData{
							Header="LOGIN履歴情報(管理者用)",
						},
						new MenuData{
							Header="汎用ファイルメンテ(管理者用)",
						},
						new MenuData{
							Header="フラグメンテナンス（DTP専用)",
						},
						new MenuData{
							Header="システムメンテナンス処理",
						},
						new MenuData{
							Header="汎用SQL問い合わせ(管理者用)",
						},
						new MenuData{
							Header="DB定義書出力",
						},
						new MenuData{
							Header="帳票管理マスタ",
						},
						new MenuData{
							Header="ラベル名称マスタ",
						},
						new MenuData{
							Header="手動更新履歴(管理者用)",
						},
						new MenuData{
							Header="自動実行管理マスタ(管理者用)",
						},
						new MenuData{
							Header="自動実行スケジュール設定",
						},
					]
				},
				new MenuData{
					Header = "■予算",
					Color = "Blue",
					SubItems=[
						new MenuData{
							Header="店ブランド予算マスタ(月)",
						},
						new MenuData{
							Header="販売員別予算マスタ(月)",
						},
						new MenuData{
							Header="店別ブランド別予算マスタ",
						},
						new MenuData{
							Header="販売員別予算マスタ",
						},
						new MenuData{
							Header="営業担当別予算マスタ",
						},
						new MenuData{
							Header="店舗予算表",
						},
						new MenuData{
							Header="店舗別予算実績対比",
						},
						new MenuData{
							Header="店舗ブランド別予算実績対比",
						},
						new MenuData{
							Header="日別店別予算表",
						},
						new MenuData{
							Header="販売員予算表",
						},
					],
				},
				new MenuData{
					Header = "■発注",
					Color = "Blue",
					SubItems=[
						new MenuData{
							Header="契約発注入力",
						},
						new MenuData{
							Header="発注配分入力",
						},
						new MenuData{
							Header="発注入力",
						},
						new MenuData{
							Header="発注配分入力(簡易版)",
						},
						new MenuData{
							Header="納品予定表",
						},
						new MenuData{
							Header="仕入先別発注表",
						},
						new MenuData{
							Header="商品別発注表",
						},
						new MenuData{
							Header="商品別発注集計表",
						},
						new MenuData{
							Header="週間納品予定表",
						},
						new MenuData{
							Header="納品予定照会",
						},
						new MenuData{
							Header="仕入未受リスト",
						},
						new MenuData{
							Header="発注書",
						},
						new MenuData{
							Header="発注配分リスト",
						},
						new MenuData{
							Header="受注発注連携更新",
						},
						new MenuData{
							Header="契約残管理表",
						},
						new MenuData{
							Header="契約残完了設定",
						},
						new MenuData{
							Header="発注残管理表",
						},
						new MenuData{
							Header="発注残完了設定",
						},
					],
				},
				new MenuData{
					Header = "■受注 / 展示会",
					Color = "Blue",
					SubItems=[
						new MenuData{
							Header="展示会受注入力",
						},
						new MenuData{
							Header="納品予定表",
						},
						new MenuData{
							Header="得意先別受注表",
						},
						new MenuData{
							Header="商品別受注表",
						},
						new MenuData{
							Header="商品別受注集計表",
						},
						new MenuData{
							Header="受注残管理表",
						},
						new MenuData{
							Header="受注残完了設定",
						},
						new MenuData{
							Header="展示会スワッチ",
						},
						new MenuData{
							Header="スワッチデータ作成",
						},
						new MenuData{
							Header="スワッチデータ一括作成",
						},
						new MenuData{
							Header="絵型一覧表",
						},
						new MenuData{
							Header="得意先別売上予定表",
						},
						new MenuData{
							Header="担当別展示会受注合計表",
						},
						new MenuData{
							Header="受注ベスト表",
						},
						new MenuData{
							Header="配分出荷リスト",
						},
						new MenuData{
							Header="バーコードブック発行",
						},
					],
				},
				new MenuData{
					Header = "■仕入",
					Color = "Blue",
					SubItems=[
						new MenuData{
							Header="生地付属仕入",
						},
						new MenuData{
							Header="商品仕入入力",
						},
						new MenuData{
							Header="仕入返品入力",
						},
						new MenuData{
							Header="品番別仕入チェックリスト",
						},
						new MenuData{
							Header="ブランド別仕入金額表",
						},
						new MenuData{
							Header="消化仕入リスト",
						},
						new MenuData{
							Header="仕入伝票印刷",
						},
						new MenuData{
							Header="仕入先別仕入推移表",
						},
						new MenuData{
							Header="支払入力",
						},
						new MenuData{
							Header="支払消込",
						},
						new MenuData{
							Header="仕入先元帳",
						},
						new MenuData{
							Header="買掛金管理表",
						},
						new MenuData{
							Header="支払一覧表",
						},
						new MenuData{
							Header="月別支払予定表",
						},
						new MenuData{
							Header="支払残高明細書",
						},
					],
				},
				new MenuData{
					Header = "■売上",
					Color = "Blue",
					SubItems=[
						new MenuData{
							Header="出荷・売上入力",
						},
						new MenuData{
							Header="店舗売上入力",
						},
						new MenuData{
							Header="客数入力",
						},
						new MenuData{
							Header="クレジット商品券データ入力",
						},
						new MenuData{
							Header="売上金種Viewer",
						},
						new MenuData{
							Header="品番別売上チェックリスト",
						},
						new MenuData{
							Header="売上チェックリスト",
						},
						new MenuData{
							Header="納品書印刷",
						},
						new MenuData{
							Header="納品書印刷(専用伝票)",
						},
						new MenuData{
							Header="納品書未発行チェックリスト",
						},
						new MenuData{
							Header="入金入力",
						},
						new MenuData{
							Header="入金消込",
						},
						new MenuData{
							Header="社員別購入履歴",
						},
						new MenuData{
							Header="催事売上入力",
						},
						new MenuData{
							Header="得意先元帳",
						},
						new MenuData{
							Header="売掛金管理表",
						},
						new MenuData{
							Header="月別入金予定表",
						},
						new MenuData{
							Header="請求一覧表",
						},
						new MenuData{
							Header="請求書印刷",
						},
						new MenuData{
							Header="得意先別売上推移表",
						},
					],
				},
				new MenuData{
					Header = "■配分",
					Color = "Blue",
					SubItems=[
						new MenuData{
							Header="店舗配分入力",
						},
						new MenuData{
							Header="受注配分入力",
						},
						new MenuData{
							Header="店舗出荷依頼",
						},
						new MenuData{
							Header="在庫品配分",
						},
						new MenuData{
							Header="得意先別配分入力",
						},
						new MenuData{
							Header="出荷指示確定(商品)",
						},
						new MenuData{
							Header="出荷指示確定(得意先)",
						},
						new MenuData{
							Header="出荷処理入力",
						},
						new MenuData{
							Header="配分データメンテ",
						},
						new MenuData{
							Header="取置入力",
						},
						new MenuData{
							Header="移動指示(SKU)",
						},
						new MenuData{
							Header="移動指示(商品)",
						},
						new MenuData{
							Header="出荷指示明細書印刷",
						},
						new MenuData{
							Header="納入一覧表",
						},
						new MenuData{
							Header="出荷指示一覧印刷",
						},
						new MenuData{
							Header="配分関連メンテナンス",
						},
						new MenuData{
							Header="自動発注・補充対象除外品設定",
						},
						new MenuData{
							Header="在庫基準自動補充メンテナンス",
						},
					]
				},
				new MenuData{
					Header = "■在庫管理",
					Color = "Blue",
					SubItems=[
						new MenuData{
							Header="棚卸入力",
						},
						new MenuData{
							Header="移動入力(即時)",
						},
						new MenuData{
							Header="移動入力(積送)",
						},
						new MenuData{
							Header="移動受入力",
						},
						new MenuData{
							Header="棚卸差異問合せ",
						},
						new MenuData{
							Header="在庫問合せ",
						},
						new MenuData{
							Header="商品履歴問合せ",
						},
						new MenuData{
							Header="棚卸入力(一覧方式)",
						},
						new MenuData{
							Header="在庫強制調整入力",
						},
						new MenuData{
							Header="在庫移動入力",
						},
						new MenuData{
							Header="倉庫分類別棚卸表",
						},
						new MenuData{
							Header="倉庫別受払表",
						},
						new MenuData{
							Header="商品別受払表",
						},
						new MenuData{
							Header="倉庫別在庫集計表",
						},
						new MenuData{
							Header="汎用在庫表",
						},
						new MenuData{
							Header="棚卸明細表",
						},
						new MenuData{
							Header="棚卸日一括メンテナンス",
						},
						new MenuData{
							Header="棚卸チェックリスト",
						},
						new MenuData{
							Header="品番別移動チェックリスト",
						},
						new MenuData{
							Header="移動未受リスト",
						},
					]
				},
				new MenuData{
					Header = "■売上分析",
					Color = "Blue",
					SubItems=[
						new MenuData{
							Header="販売動向表",
						},
						new MenuData{
							Header="品番別販売動向表",
						},
						new MenuData{
							Header="投入売上在庫表",
						},
						new MenuData{
							Header="ベスト表",
						},
						new MenuData{
							Header="商品消化率表",
						},
						new MenuData{
							Header="セット売上分析表",
						},
						new MenuData{
							Header="店別売上日報",
						},
						new MenuData{
							Header="店舗別売上日計表",
						},
						new MenuData{
							Header="売上速報",
						},
						new MenuData{
							Header="売上週報･月報",
						},
						new MenuData{
							Header="売上予算構成比",
						},
						new MenuData{
							Header="分類別売上消化率表",
						},
						new MenuData{
							Header="分類別店別売上報告",
						},
						new MenuData{
							Header="店舗売上ランキング表",
						},
					]
				},
				new MenuData{
					Header = "■卸・販売員・経営分析",
					Color = "Blue",
					SubItems=[
						new MenuData{
							Header="得意先別売上日報",
						},
						new MenuData{
							Header="得意先別売上月報",
						},
						new MenuData{
							Header="担当別売上実績半期報",
						},
						new MenuData{
							Header="担当得意先別予算実績対比表",
						},
						new MenuData{
							Header="個人売上ランキング表",
						},
						new MenuData{
							Header="販売員別予算実績対比表",
						},
						new MenuData{
							Header="半期報",
						},
						new MenuData{
							Header="全社受払表",
						},
						new MenuData{
							Header="卸・店舗売上実績表",
						},
					]
				},
				new MenuData{
					Header = "■C.P.A",
					Color = "Blue",
					SubItems=[
						new MenuData{
							Header="★T.L - アナライザー★",
						},
						new MenuData{
							Header="★C.G - アナライザー★",
						},
						new MenuData{
							Header="ナンでも？CSV",
						},
						new MenuData{
							Header="ABC分析(全社)",
						},
						new MenuData{
							Header="ABC分析(店舗)",
						},
						new MenuData{
							Header="在庫データ出力",
						},
						new MenuData{
							Header="在庫受け払い照会",
						},
						new MenuData{
							Header="販売動向ビュー",
						},
						new MenuData{
							Header="店舗稼動ビュー",
						},
						new MenuData{
							Header="売消台帳ビュー",
						},
						new MenuData{
							Header="商品分析",
						},
						new MenuData{
							Header="オンラインモニタ",
						},
						new MenuData{
							Header="売上・在庫問合せ",
						},
						new MenuData{
							Header="ベストレポート",
						},
					],
				},
				new MenuData{
					Header = "■HHT / POS連携",
					Color = "Blue",
					SubItems=[
						new MenuData{
							Header="HHT用マスタデータ作成(VulcanCOM)",
						},
						new MenuData{
							Header="HHT用マスタデータ作成(cvnetcom)",
						},
						new MenuData{
							Header="HHT手動データ受信",
						},
						new MenuData{
							Header="HHTエラーデータ修正入力",
						},
						new MenuData{
							Header="HHTデータ更新",
						},
						new MenuData{
							Header="HHT未更新データ印刷",
						},
						new MenuData{
							Header="HHT未更新データ一括削除",
						},
						new MenuData{
							Header="HHT用PATH設定",
						},
						new MenuData{
							Header="出荷指示明細書印刷",
						},
						new MenuData{
							Header="移動明細書印刷",
						},
						new MenuData{
							Header="即時移動明細書",
						},
						new MenuData{
							Header="HHT手動データ受信(ﾃﾞｰﾀ送信後)",
						},
						new MenuData{
							Header="HHT手動データ受信(店舗固定)",
						},
						new MenuData{
							Header="POSデータ取込",
						},
						new MenuData{
							Header="HHT用マスタバーコード印刷",
						},
					],
				},
				new MenuData{
					Header = "■月次 / 更新処理",
					Color = "Blue",
					SubItems=[
						new MenuData{
							Header="請求計算",
						},
						new MenuData{
							Header="支払計算",
						},
						new MenuData{
							Header="棚卸開始処理",
						},
						new MenuData{
							Header="棚卸確定",
						},
						new MenuData{
							Header="在庫・掛再更新",
						},
						new MenuData{
							Header="在庫累計更新",
						},
						new MenuData{
							Header="締日更新",
						},
						new MenuData{
							Header="諸掛更新",
						},
						new MenuData{
							Header="一時処理用(管理者用)",
						},
						new MenuData{
							Header="残高登録処理",
						},
						new MenuData{
							Header="データ整理更新",
						},
						new MenuData{
							Header="消費税再計算",
						},
						new MenuData{
							Header="最終仕入原価更新",
						},
						new MenuData{
							Header="総平均原価更新",
						},
						new MenuData{
							Header="消化仕入更新",
						},
						new MenuData{
							Header="積送中クリア",
						},
						new MenuData{
							Header="月間データ集計",
						},
						new MenuData{
							Header="自動発注・補充の実行",
						},
					],
				},
				new MenuData{
					Header = "■Loyal Customer",
					Color = "Blue",
					SubItems=[
						new MenuData{
							Header="顧客マスタ",
							IsDialog = false,
						},
						new MenuData{
							Header="顧客ランク設定（管理者用)",
						},
						new MenuData{
							Header="ポイントマスタ（ベース）（管理者用)",
						},
						new MenuData{
							Header="ポイントマスタ（キャンペーン）",
						},
						new MenuData{
							Header="ポイントマスタ（ボーナス）",
						},
						new MenuData{
							Header="店舗別キャンペーン設定",
						},
						new MenuData{
							Header="商品店舗別ポイント設定",
						},
						new MenuData{
							Header="ポイント集計",
						},
						new MenuData{
							Header="顧客カルテ",
						},
						new MenuData{
							Header="RFMクロス分析表",
						},
						new MenuData{
							Header="配信用簡易抽出",
						},
						new MenuData{
							Header="配信ファイル変換",
						},
						new MenuData{
							Header="顧客カルテ",
						},
						new MenuData{
							Header="顧客ランク更新（管理者用）",
						},
						new MenuData{
							Header="RFMクロス分析表",
						},
						new MenuData{
							Header="配信用簡易抽出",
						},
						new MenuData{
							Header="配信ファイル変換",
						},
						new MenuData{
							Header="ポイント集計",
						},
					],
				},
				new MenuData{
					Header = "■店舗",
					Color = "Blue",
					SubItems = [
						new MenuData{
							Header="店舗売上入力",
						},
						new MenuData{
							Header="棚卸明細表(原価無)",
						},
						new MenuData{
							Header="汎用在庫表(原価無)",
						},
						new MenuData{
							Header="売上速報(原価無)",
						},
						new MenuData{
							Header="売上週報･月報(原価無)",
						},
						new MenuData{
							Header="分類別店別売上報告(原価無)",
						},
					],
				},
				new MenuData{
					Header = "■物流",
					Color = "Blue",
					SubItems=[
						new MenuData{
							Header="マスタデータ作成",
						},
						new MenuData{
							Header="連携データ手動送信",
						},
						new MenuData{
							Header="連携データ手動受信",
						},
						new MenuData{
							Header="連携エラーデータ照会",
						},
					],
				},
				new MenuData{
					Header = "　　　　　"
				}
			];
			return menu;
		}
	}
}

