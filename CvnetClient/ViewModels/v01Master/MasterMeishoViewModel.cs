/* ============================================================================
 * CvnetClient.exe : MasterMeishoViewModel.cs
 * Created by Sekiya.Sato 2025/05/21
 * 説明: 名称マスター画面
 *			SubDlg_01_mei.crsと互換
 * 使用ライブラリ
 *		CommunityToolkit.Mvvm : LICENCE = MIT
 * ============================================================================  */
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Views;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CvnetClient.ViewModels {
	public partial class MasterMeishoViewModel : BaseViewModel {
		[ObservableProperty]
		List<string>? listKubun;
		[ObservableProperty]
		string? selectKubun;
		partial void OnSelectKubunChanged(string? value) {
			// 区分が変わったらStartCodeを初期化
			if (value != null) {
				StartCode = string.Empty;
			}
			var kubun = SelectKubun?.Split(' ')[0].ToString() ?? " ";
			if (kubun.StartsWith("IDX")) {
				LabelRyakusho = "略称";
				LabelDispFlg = "表示FLG";
				LabelRenban = "連番";
				LabelKana = "カナ";
			}
			else if (kubun.StartsWith("COL") || kubun.StartsWith("GTI")) {
				LabelRyakusho = "略称";
				LabelDispFlg = "表示色No";
				LabelRenban = "連番";
				LabelKana = "カナ";
			}
			else if (kubun.StartsWith("BRD")) {
				LabelRyakusho = "略称";
				LabelDispFlg = "JANﾒｰｶCD";
				LabelRenban = "連番";
				LabelKana = "ｻｲｽﾞ区分";
			}
			else if (kubun.StartsWith("ITM")) {
				LabelRyakusho = "略称";
				LabelDispFlg = "表示FLG";
				LabelRenban = "連番";
				LabelKana = "ｻｲｽﾞ区分";
			}
			else if (kubun.StartsWith("ZCL")) {
				LabelRyakusho = "略称";
				LabelDispFlg = "警告日数";
				LabelRenban = "有効期限日数";
				LabelKana = "カナ";
			}
			else if (kubun.StartsWith("ISS")) {
				LabelRyakusho = "略称";
				LabelDispFlg = "受取金額";
				LabelRenban = "印紙金額";
				LabelKana = "カナ";
			}
			else if (kubun.StartsWith("PRK")) {
				LabelRyakusho = "しきい額(00円以上)";
				LabelDispFlg = "ランク";
				LabelRenban = "連番";
				LabelKana = "カナ";
			}
			else if (kubun.StartsWith("PT1")) {
				LabelRyakusho = "P付与倍率";
				LabelDispFlg = "S付与倍率";
				LabelRenban = "連番";
			}
			else if (kubun.StartsWith("RK1")) {
				LabelRyakusho = "日from";
				LabelDispFlg = "日to";
				LabelRenban = "連番";
				LabelKana = "カナ";
			}
			else if (kubun.StartsWith("RK2")) {
				LabelRyakusho = "回数from";
				LabelDispFlg = "回数to";
				LabelRenban = "連番";
				LabelKana = "カナ";
			}
			else if (kubun.StartsWith("RK3")) {
				LabelRyakusho = "金額from";
				LabelDispFlg = "金額to";
				LabelRenban = "連番";
				LabelKana = "カナ";
			
			}
			else {
				LabelRyakusho = "略称";
				LabelDispFlg = "表示FLG";
				LabelRenban = "連番";
				LabelKana = "カナ";

			}
		}

		[ObservableProperty]
		string? startCode;
		[ObservableProperty]
		ObservableCollection<MasterMeisho>? listMeisho;
		/// <summary>
		/// 現在選択されているMeishoオブジェクト
		/// </summary>
		[ObservableProperty]
		MasterMeisho? selectedMeisho;
		partial void OnSelectedMeishoChanged(MasterMeisho? value) {
			// 選択行が変わったらEditMeishoにディープコピー
			if (value != null)
				EditMeisho = Common.CloneObject(value);
			else
				EditMeisho = null;
		}
		/// <summary>
		/// 修正用の一時的なMeishoオブジェクト
		/// </summary>
		[ObservableProperty]
		MasterMeisho? editMeisho;

		[ObservableProperty]
		string labelRyakusho ="略称"; // ラベルの初期値
		[ObservableProperty]
		string labelDispFlg = "表示FLG";
		[ObservableProperty]
		string labelRenban = "連番";
		[ObservableProperty]
		string labelKana = "カナ";


		string sql_p3 = """
				select * from(select A.SEQ_NO, A.VDATE_CREATE, A.VDATE_UPDATE, A.名称CD, A.名称, A.略称, A.ランク, A.連番, A.カナ, A.POS区分,
				(A.入力社員CD || ' ' || (select B.名前 from HC$MASTER_SHAIN B where B.社員CD = A.入力社員CD)) 最終修正者
				from HC$Master_MEISHO A where A.名称区分=:1 and A.名称CD{0}:2 order by A.名称CD {1}) where rownum<={2}
			""";
		// 前検索： {0} は <= {1}は desc {2}はAppData.maxQueryCnt
		// 後検索および通常： {0} は >= {1}は asc {2}はAppData.maxQueryCnt
		/// <summary>
		/// 初期化
		/// </summary>
		[RelayCommand]
		void Init() {
			var kubun = AppData.Http?.AspxSqlQuery("""
				select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='IDX'  and a.ランク>= '1' order by a.名称CD
				""", new string[0]);
			if (kubun != null) {
				ListKubun = (from DataRow dr in kubun.Rows
							 select string.Format($"{dr["名称CD"]} {dr["名称"]}")
							 ).ToList();
				SelectKubun = ListKubun[0];
			}
		}
		/// <summary>
		/// 一覧表示
		/// </summary>
		[RelayCommand]
		void DoList() {
			if (string.IsNullOrEmpty(SelectKubun)) return;
			subList(string.IsNullOrEmpty(StartCode) ? "." : StartCode, ">=", "asc", AppData.maxQueryCnt);
			if(ListMeisho == null || ListMeisho.Count == 0) 
				ClientLib.MessageBoxOk(this, "データがありません");
		}
		void subList(string startCd, string sql_P1, string sql_P2, int sql_P3) {
			if (string.IsNullOrEmpty(SelectKubun)) return;
			var kubun = SelectKubun.Split(' ')[0];
			var sql = string.Format(sql_p3, sql_P1, sql_P2, sql_P3);
			var retData = AppData.Http?.AspxSqlQuery(sql, new string[] { kubun, startCd });
			if (retData == null || retData.Rows.Count == 0) return;
			var list = (from DataRow dr in retData.Rows
						select new MasterMeisho {
							SeqNo = Convert.ToInt64(dr["SEQ_NO"]),
							VdateCreate = Convert.ToDecimal(dr["VDATE_CREATE"]),
							VdateUpdate = Convert.ToDecimal(dr["VDATE_UPDATE"]),
							Kubun = kubun,
							MeishoCd = dr["名称CD"].ToString() ?? string.Empty,
							Meisho = dr["名称"].ToString() ?? string.Empty,
							RyakuShou = dr["略称"].ToString() ?? string.Empty,
							Rank = dr["ランク"].ToString() ?? string.Empty,
							Renban = dr["連番"].ToString() ?? string.Empty,
							Kana = dr["カナ"].ToString() ?? string.Empty,
							PosKubun = dr["POS区分"].ToString() ?? string.Empty,
							SaishuuShuuseiSha = dr["最終修正者"].ToString() ?? string.Empty
						}).OrderBy(c => c.MeishoCd).ToList();
			Common.ConvertDotStringDel(list);
			ListMeisho = new ObservableCollection<MasterMeisho>(list);
			if (ListMeisho.Count > 0) {
				SelectedMeisho = ListMeisho[0];
			}
		}
		[RelayCommand]
		void BackList() {
			if (string.IsNullOrEmpty(SelectKubun)) return;
			var startcd = string.IsNullOrEmpty(StartCode) ? "." : StartCode;
			if(ListMeisho != null && ListMeisho.Count>0) {
				startcd = ListMeisho.Min(c=>c.MeishoCd);
			}
			subList(startcd!, "<=", "desc", AppData.maxQueryCnt);
			if (ListMeisho == null || ListMeisho.Count == 0)
				ClientLib.MessageBoxOk(this, "データがありません");
		}
		[RelayCommand]
		void NextList() {
			if (string.IsNullOrEmpty(SelectKubun)) return;
			var startcd = string.IsNullOrEmpty(StartCode) ? "." : StartCode;
			if (ListMeisho != null && ListMeisho.Count > 0) {
				startcd = ListMeisho.Max(c => c.MeishoCd);
			}
			subList(startcd!, ">=", "asc", AppData.maxQueryCnt);
			if (ListMeisho == null || ListMeisho.Count == 0)
				ClientLib.MessageBoxOk(this, "データがありません");
		}


		/// <summary>
		/// レコード挿入
		/// </summary>
		[RelayCommand]
		void DoInsert() {
			if(!ClientLib.MessageBox(this, "新規登録しますか？")) return;

			var item = Common.CloneObject(EditMeisho);
			Common.ConvertDotStringAdd(item);
			if (item == null) return;
			var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.INSERT, "Master_MEISHO", 0, "0",
				new string[] { "名称区分", "名称CD", "名称", "略称", "ランク", "連番", "カナ", "POS区分" },
				new string[] { item.Kubun!, item.MeishoCd!, item.Meisho!,item.RyakuShou!, item.Rank!, item.Renban!, item.Kana!, item.PosKubun! });
			if (ret.Code == 0) {
				item.SeqNo = ret.NewSeq;
				item.VdateUpdate = decimal.Parse(ret.VDate);
				item.VdateCreate = item.VdateUpdate;
				Common.ConvertDotStringDel(item);
				ListMeisho!.Add(item);
				SelectedMeisho = item;
			}
			else {
				ClientLib.MessageBoxError(this, ret.Code.ToString());
			}
		}
		/// <summary>
		/// レコード修正
		/// </summary>
		[RelayCommand]
		void DoUpdate() {
			if (!ClientLib.MessageBox(this, "修正しますか？")) return;
			var item = Common.CloneObject(EditMeisho);
			Common.ConvertDotStringAdd(item);
			if(item == null) return;
			var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.UPDATE, "Master_MEISHO", item.SeqNo, item.VdateUpdate.ToString(),
				new string[] { "名称区分", "名称CD", "名称", "略称", "ランク", "連番", "カナ", "POS区分" },
				new string[] { item!.Kubun!, item.MeishoCd!, item.Meisho!, item.RyakuShou!, item.Rank!, item.Renban!, item.Kana!, item.PosKubun! });
			if (ret.Code == 0) {
				Common.ConvertDotStringDel(item);
				if(SelectedMeisho != null) {
					SelectedMeisho.VdateUpdate = decimal.Parse(ret.VDate);
					SelectedMeisho.Kubun = item.Kubun;
					SelectedMeisho.MeishoCd = item.MeishoCd;
					SelectedMeisho.Meisho = item.Meisho;
					SelectedMeisho.RyakuShou = item.RyakuShou;
					SelectedMeisho.Rank = item.Rank;
					SelectedMeisho.Renban = item.Renban;
					SelectedMeisho.Kana = item.Kana;
					SelectedMeisho.PosKubun = item.PosKubun;
					EditMeisho = Common.CloneObject(SelectedMeisho); // 選択行の内容をEditMeishoに反映
				}
			}
			else {
				ClientLib.MessageBoxError(this, ret.Code.ToString());
			}
		}
		/// <summary>
		/// レコード削除
		/// </summary>
		[RelayCommand]
		void DoDelete() {
			if (!ClientLib.MessageBox(this, "削除しますか？")) return;
			if (EditMeisho == null) return;
			var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.DELETE, "Master_MEISHO", EditMeisho.SeqNo, EditMeisho.VdateUpdate.ToString(),
				new string[0], new string[0] );
			if (ret.Code == 0) {
				if(SelectedMeisho!= null) {
					ListMeisho!.Remove(SelectedMeisho);
					var item = ListMeisho.Where(c => c.MeishoCd == ListMeisho.Min(c => c.MeishoCd)).FirstOrDefault();
					SelectedMeisho = item;
				}
			}
			else {
				ClientLib.MessageBoxError(this, ret.Code.ToString());
			}
		}
		string printsql = """
				select A.SEQ_NO,SUBSTR(GET_VDATE(a.VDATE_CREATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_CREATE),10,6) 作成日時,
				SUBSTR(GET_VDATE(a.VDATE_UPDATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_UPDATE),10,6) 更新日時,
				A.名称区分 ||' '|| (SELECT m.名称 FROM HC$Master_MEISHO m WHERE m.名称区分='IDX' AND m.名称CD=A.名称区分) 名称区分,
				A.名称CD,A.名称,A.略称,A.ランク,A.連番,A.カナ,CASE WHEN (A.POS区分=0) THEN '0 出力する' WHEN (A.POS区分=9) 
				THEN '9 削除指示' WHEN (A.POS区分=10) THEN '10 出力しない' ELSE '.' END POS区分,
				(A.入力社員CD ||' '|| (select B.名前 from HC$MASTER_SHAIN B where B.社員CD=A.入力社員CD)) 最終修正者 
				from HC$Master_MEISHO A  where A.名称区分 =:1  
				""";
		/// <summary>
		/// PDF印刷
		/// </summary>
		[RelayCommand]
		async Task DoPrintAsync() {
			if (!ClientLib.MessageBox(this, "印刷しますか？")) return;
			ClientLib.CursorToWait();
			if (ListMeisho == null || ListMeisho.Count == 0 || SelectKubun == null) return;
			var cdlist = string.Join(",", ListMeisho.Select(c => $"'{c.MeishoCd}'"));
			var ret = AppData.Http!.AspxSqlQueryCsv(string.Format(printsql+ " and A.名称CD in({0}) order by A.名称CD", cdlist), new string[] {SelectKubun.Split(' ')[0] }, "cvnet_meisho.qfm");
			if(ret.Split('\n').Length < 2) {
				ClientLib.MessageBoxError(this, "PDFデータがありません");
				return;
			}
			var ret1 = ret.Split('\n');
			var url = AppData.Http.URLroot + ret1[0]+"/data.pdf";
			await Task.Delay(1500); // PDF生成待ち
			var win = new WebpdfView();
			var vm = win.DataContext as WebpdfViewModel;
			if(vm == null) return;
			vm.Pdfdata = url;
			ClientLib.CursorToNormal();
			ClientLib.ShowDialogView(win, this);
		}
		[RelayCommand]
		async Task DoPrintAllAsync() {
			if (!ClientLib.MessageBox(this, "全件印刷しますか？")) return;
			ClientLib.CursorToWait();
			if (ListMeisho == null || ListMeisho.Count == 0 || SelectKubun == null) return;
			var ret = AppData.Http!.AspxSqlQueryCsv(printsql + " order by A.名称CD", new string[] { SelectKubun.Split(' ')[0] }, "cvnet_meisho.qfm");
			if (ret.Split('\n').Length < 2) {
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
	}
}
