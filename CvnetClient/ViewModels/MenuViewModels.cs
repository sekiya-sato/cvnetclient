/* ============================================================================
 * CvnetClient.exe : MenuViewModels.cs
 * Created by Sekiya.Sato 2025/05/13
 * 説明: メインメニュー画面
 * 使用ライブラリ
 *		CommunityToolkit.Mvvm : LICENCE = MIT
 * ============================================================================  */
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cvnet8client.Models;
using CvnetBaseCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using CvnetClient.Models;
using System.Diagnostics;

namespace CvnetClient.ViewModels {
	public partial class MenuViewModels : ObservableObject {
		[ObservableProperty]
		string exeVer = ClassHttp.NetFramework;
		[ObservableProperty]
		List<MenuData>? listMenu;
		[ObservableProperty]
		MenuData? selectedMenu;
		[ObservableProperty]
		string? bottomMessage;

		[RelayCommand]
		public void Init() {
			// メニュー初期化
			ListMenu = MenuData.Initmenu();
			// 選択メニュー初期化
			SelectedMenu = ListMenu[0];
			// 設定ファイル読み込み
			BottomMessage = AppData.Url;
		}
		[RelayCommand]
		public void Exit() {
			ClientLib.Exit(this);
		}
		[RelayCommand]
		public void DoTest() {
		}
		[RelayCommand]
		public void DoTest2() {
			ClientLib.ShowDialogView(new Views.MasterMeishoView(), this);
		}
		[RelayCommand]
		public void DoLogin() {
			ClientLib.ShowDialogView(new Views.LoginView(), this);
		}
		[RelayCommand]
		public void DoServerLogin() {
			var http= new ClassHttp(AppData.Url);
			var ret = http.Login(0, AppData.AppConfig["LoginId"]??string.Empty, AppData.AppConfig["LoginPass"]?? string.Empty);
			Debug.WriteLine($"ログインステータス＝{ret}");
		}
	}
}
