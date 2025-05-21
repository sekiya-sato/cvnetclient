/* ============================================================================
 * CvnetClient.exe : MasterMeishoViewModel.cs
 * Created by Sekiya.Sato 2025/05/21
 * 説明: 名称マスター画面
 * 使用ライブラリ
 *		CommunityToolkit.Mvvm : LICENCE = MIT
 * ============================================================================  */
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CvnetClient.ViewModels {
	public partial class MasterMeishoViewModel : ObservableObject {
		[ObservableProperty]
		List<string>? listKubun;
		[ObservableProperty]
		string? selectKubun;

		[RelayCommand]
		public void Init() {
			var kubun = AppData.Http?.AspxSqlQuery("""
				select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='IDX'  and a.ランク>= '1' order by a.名称CD
				""", new string[0]);
			if(kubun != null) {
				ListKubun = (from DataRow dr in kubun.Rows
							 select string.Format($"{dr["名称CD"]} {dr["名称"]}")
							 ).ToList();
				SelectKubun = ListKubun[0];
			}

		}
		[RelayCommand]
		public void Exit() {
			ClientLib.Exit(this);
		}
	}

}
