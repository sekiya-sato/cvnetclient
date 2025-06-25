using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Media;
using static CvnetClient.ViewModels.MeishoSelectViewModel;

namespace CvnetClient.ViewModels {
	public partial class MeishoSelectViewModel : ObservableObject {
		public enum SearchType {
			None = 0,
			Meisho = 1,
			Shiire = 2,
			Tokui = 3,
			Tenpo = 4,
			Soko = 5,
			Shohin = 6,
			Zetc = 9999
		}
		[ObservableProperty]
		private string? titleCode;
		[ObservableProperty]
		private string? titleName;

		[ObservableProperty]
		private ObservableCollection<MasterMeisho> list = new();

		[ObservableProperty]
		private MasterMeisho? selectedItem;

		[ObservableProperty]
		private string? searchText = ".";

		string sqlStr = "select * from MasterMeisho where rownum<100";
		string subItem = "ITM"; // 初期値はITM（商品）
		SearchType SearchTp = SearchType.Meisho; // 初期値はMeisho（名称）

		[RelayCommand]
		void Search() {
			// 検索ロジック例（必要に応じて実装）
			if (string.IsNullOrWhiteSpace(SearchText)) {
				SearchText = ".";
			}
			if (SearchTp == SearchType.Meisho) {
				var dt = AppData.Http?.AspxSqlQuery(sqlStr, new string[] { SearchText });
				if (dt != null && dt.Rows.Count > 0) {
					List = new ObservableCollection<MasterMeisho>(dt.Rows.Cast<DataRow>().Select(row => new MasterMeisho {
						MeishoCd = row.Field<string>("名称CD"),
						Meisho = row.Field<string>("名称"),
						RyakuShou = row.Field<string>("略称")
					}));
					SelectedItem = List.FirstOrDefault();
				}
			}
		}

		[RelayCommand]
		void Select() {
			// 選択確定時の処理（ダイアログCloseなど）
			ClientLib.ExitDialogResult(this, true);
		}

		[RelayCommand]
		void Next() {
			if (List != null && List.Count > 0) {
				var lastItem = List.LastOrDefault();
				SearchText = lastItem?.MeishoCd ?? ".";
				Search();
			}
		}

		[RelayCommand]
		void Exit() {
			ClientLib.Exit(this);
		}

		public void Init(SearchType searchTp, string startCode, string sub = "ITM") {
			SearchTp = searchTp;
			SearchText = startCode;
			if (SearchTp == SearchType.Meisho) {
				subItem = sub;
				sqlStr = $"select 名称CD,名称,略称 from HC$MASTER_MEISHO where 名称区分='{subItem}' and 名称CD>=:1 order by 名称CD";
				sqlStr = Common.GetSqlStringUsingMax(sqlStr, AppData.maxQueryCnt);
				TitleCode = "名称CD";
				TitleName = "名称";
				Search();
			}
			else if (SearchTp == SearchType.Shiire) {
				sqlStr = $"select 仕入先CD 名称CD,仕入先名 名称,略称 from HC$MASTER_SIIRE where 仕入先CD>=:1 order by 仕入先CD";
				sqlStr = Common.GetSqlStringUsingMax(sqlStr, AppData.maxQueryCnt);
				TitleCode = "仕入先CD";
				TitleName = "仕入先名";
				Search();
			}
			else if (SearchTp == SearchType.Tokui) {
				sqlStr = $"select 得意先CD 名称CD,得意先名 名称,略称 from HC$MASTER_TOKUI where 得意先CD>=:1 and 店種区分=1 order by 得意先CD";
				sqlStr = Common.GetSqlStringUsingMax(sqlStr, AppData.maxQueryCnt);
				TitleCode = "得意先CD";
				TitleName = "得意先名";
				Search();
			}
			else if (SearchTp == SearchType.Tenpo) {
				sqlStr = $"select 得意先CD 名称CD,得意先名 名称,略称 from HC$MASTER_TOKUI where 得意先CD>=:1 and 店種区分 in (3,6) order by 得意先CD";
				sqlStr = Common.GetSqlStringUsingMax(sqlStr, AppData.maxQueryCnt);
				TitleCode = "店舗CD";
				TitleName = "店舗名";
				Search();
			}
			else if (SearchTp == SearchType.Soko) {
				sqlStr = $"select 得意先CD 名称CD,得意先名 名称,略称 from HC$MASTER_TOKUI where 得意先CD>=:1 and 店種区分=0 order by 得意先CD";
				sqlStr = Common.GetSqlStringUsingMax(sqlStr, AppData.maxQueryCnt);
				TitleCode = "倉庫CD";
				TitleName = "倉庫名";
				Search();
			}
			else if (SearchTp == SearchType.Shohin) {
				sqlStr = $"select 商品CD 名称CD,商品名 名称,略称 from HC$MASTER_SHOHIN where 商品CD>=:1 order by 商品CD";
				sqlStr = Common.GetSqlStringUsingMax(sqlStr, AppData.maxQueryCnt);
				TitleCode = "商品CD";
				TitleName = "商品名";
				Search();
			}

		}
	}
}