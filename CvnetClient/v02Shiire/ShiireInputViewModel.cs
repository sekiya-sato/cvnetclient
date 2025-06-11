using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.ObjectModel;
using System.Data;

namespace CvnetClient.ViewModels {
	public partial class ShiireInputViewModel : ObservableObject {
		[ObservableProperty]
		int selectedTabIndex;
		// 検索条件
		[ObservableProperty] long denpyoNoFrom;
		[ObservableProperty] long denpyoNoTo=99999999;
		[ObservableProperty] DateTime? shiireDateFrom= DateTime.Now.AddDays(DateTime.Now.Day - 1).AddDays(-300);
		[ObservableProperty] DateTime? shiireDateTo = DateTime.Now;
		[ObservableProperty] ObservableCollection<string>? torihikiKubunList = new() { "10 仕入", "20 返品" };
		[ObservableProperty] string? selectedTorihikiKubun;
		[ObservableProperty] string? manualInputNo;
		[ObservableProperty] string? shiireSakiCode;
		[ObservableProperty] string? shiireSakiName;
		[ObservableProperty] bool isShuukei;
		[ObservableProperty] bool isMishouninOnly = true;
		[ObservableProperty] bool isAll;
		[ObservableProperty] string pageInfo = "1 / 1";

		// DataGrid
		[ObservableProperty] ObservableCollection<ShiireRow> shiireList = new();
		[ObservableProperty] ShiireRow? selectedShiire;

		// 下部
		[ObservableProperty] bool isListMode = true;
		[ObservableProperty] bool isDetailMode;
		[ObservableProperty] string statusMessage = "リスト選択行データ取得";

		string sqlstr = $"""
			select * from (
			select A.SEQ_NO,A.VDATE_CREATE,A.VDATE_UPDATE,A.手入力伝票NO,A.在庫計上日,A.掛計上日,A.取引区分,A.入力社員CD,A.倉庫CD,
			A.取引先CD1,A.掛率1,A.外税対象金額,A.数量合計,A.明細金額合計,A.内税消費税,A.外税消費税,A.上代合計,A.下代合計,A.メモ,
			A.掛計上FLG,A.伝票処理区分,A.MOD_SEQ,A.関連伝票NO,A.関連伝票NO2,A.来勘FLG,A.在庫計上FLG,A.消費税率,B.名前 担当名,
			C.仕入先名 仕入先名,D.得意先名 倉庫名,C.掛率,C.掛率2,C.消費税CD,C.消費税計算方法,C.消費税端数,
			decode(A.掛計上FLG,1,'○','') 承認FLG,NVL((select max(K.支払日) 支払日 from HC$MANAGE_KAKESIH K where 
			A.取引先CD1=K.仕入先CD),'19010101') 最終締日 from HC$Tran_TORI0 A,HC$MASTER_SHAIN B,HC$Master_SIIRE C,HC$MASTER_TOKUI D 
			where (A.入力社員CD=B.社員CD(+)) and (A.取引先CD1=C.仕入先CD(+)) and (A.倉庫CD=D.得意先CD(+)) and 
			A.SEQ_NO between :1 and :2 and A.在庫計上日 between :3 and :4 and A.取引区分 between :5 and :6 and 
			A.関連伝票NO between :7 and :8 and A.関連伝票NO2 between :9 and :10 and A.取引先CD1 between :11 and :12 and 
			A.倉庫CD between :13 and :14 and A.手入力伝票NO between :15 and :16 and A.入力社員CD between :17 and :18 and 
			A.伝票処理区分=3 ORDER BY A.SEQ_NO DESC) where rownum<={AppData.maxQueryCnt}
			""";
		/*
		string sqlstr = $"""
			select A.*,B.名前 担当名
			from HC$Tran_TORI0 A,HC$MASTER_SHAIN B
			where 
			(A.入力社員CD=B.社員CD(+)) and
			A.SEQ_NO between :1 and :2 and A.在庫計上日 between :3 and :4 and 
			A.伝票処理区分=3 ORDER BY A.SEQ_NO DESC
			""";
		*/


		/// 一覧表示
		/// </summary>
		[RelayCommand]
		void DoList() {
			subList();
			if (ShiireList == null || ShiireList.Count == 0)
				ClientLib.MessageBoxOk(this, "データがありません");
		}
		void subList() {
			var para = new string[] {
				DenpyoNoFrom.ToString(), DenpyoNoTo.ToString(),
				ShiireDateFrom?.ToString("yyyyMMdd") ?? "19010101", ShiireDateTo?.ToString("yyyyMMdd") ?? "99991231",
				"10","99", // 取引区分 10=仕入,20=返品
				"0", "9999999999", // 関連伝票NO1
				"0", "9999999999", // 関連伝票NO2
				ShiireSakiCode ?? "0", ShiireSakiCode ??"9999999999", // 仕入先CD1
				".","9999999999", // 倉庫CD
				ManualInputNo ?? ".", ManualInputNo ?? "9999999999", // 手入力伝票NO
				".", ".9999999999", // 入力社員CD
				};
			var retData = AppData.Http?.AspxSqlQuery(sqlstr, para);

			if (retData == null || retData.Rows.Count == 0) return;
			var list = (from DataRow dr in retData.Rows
						select new ShiireRow {
							DenpyoNo = dr["SEQ_NO"].ToString(),
							ShiireDate = dr["在庫計上日"].ToString(),
							ShiireSaki = dr["取引先CD1"].ToString(),
							ShiireSakiName = dr["仕入先名"].ToString(),
							SokoName = dr["倉庫名"].ToString(),
							TorihikiKubun = dr["取引区分"].ToString(),
							Suuryou = dr["数量合計"].ToString(),
							Kingaku = dr["明細金額合計"].ToString(),
							KanrenNo1 = dr["関連伝票NO"].ToString(),
						}).ToList();
			Common.ConvertDotStringDel(list);
			ShiireList = new ObservableCollection<ShiireRow>(list);
			if (ShiireList.Count > 0) {
				SelectedShiire = ShiireList[0];
			}
		}


		// コマンド
		[RelayCommand]
		void OpenSearch() {
			/* 実装 */
		}
		[RelayCommand] void OpenEdit() { /* 実装 */ }
		[RelayCommand] void SelectShiireSaki() { /* 実装 */ }
		[RelayCommand] void Search() { /* 実装 */ }
		[RelayCommand] void UpdateSelectedFlag() { /* 実装 */ }
		[RelayCommand] void Print() { /* 実装 */ }
		[RelayCommand] void ExportCsv() { /* 実装 */ }
		[RelayCommand] void Edit() { /* 実装 */ }
		[RelayCommand] void Delete() { /* 実装 */ }
		[RelayCommand]
		void Close() {
			ClientLib.Exit(this);
		}
		[RelayCommand]
		void RowDoubleClick() {
			// 詳細モードに切り替え
			IsListMode = false;
			IsDetailMode = true;
			SelectedTabIndex = 1; // 詳細タブに切り替え
			StatusMessage = "選択行データ取得";
			// 選択行のデータを取得
			// ここで詳細画面にデータをセットする処理を追加する
			// 例: LoadShiireDetails(current);
		}

	}

	// DataGrid用の行データ
	public partial class ShiireRow : ObservableObject {
		[ObservableProperty] bool isUpdated;
		[ObservableProperty] string? denpyoNo;
		[ObservableProperty] string? shiireDate;
		[ObservableProperty] string? shiireSaki;
		[ObservableProperty] string? shiireSakiName;
		[ObservableProperty] string? sokoName;
		[ObservableProperty] string? torihikiKubun;
		[ObservableProperty] string? suuryou;
		[ObservableProperty] string? kingaku;
		[ObservableProperty] string? kanrenNo1;
	}
}