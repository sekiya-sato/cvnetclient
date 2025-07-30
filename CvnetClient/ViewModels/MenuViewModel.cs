/* ============================================================================
 * CvnetClient.exe : MenuViewModels.cs
 * Created by Sekiya.Sato 2025/05/13
 * 説明: メインメニュー画面
 * 使用ライブラリ
 *		CommunityToolkit.Mvvm : LICENCE = MIT
 * ============================================================================  */
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using static System.Net.WebRequestMethods;

namespace CvnetClient.ViewModels {
	public partial class MenuViewModel : ObservableObject {
		[ObservableProperty]
		string exeVer = ClassHttp.NetFramework;
		[ObservableProperty]
		List<MenuData>? listMenu;
		[ObservableProperty]
		MenuData? selectedMenu;
		[ObservableProperty]
		string? bottomMessage;
		[ObservableProperty]
		DateTime? dateNow;

		DispatcherTimer timer = new ();

		public MenuViewModel() {
			// デザイン時にも使用するため、コンストラクタで初期化処理を行う
			// メニュー初期化
			ListMenu = MenuData.Initmenu();
			// 選択メニュー初期化
			if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) 
				SelectedMenu = ListMenu[1];
			else
				SelectedMenu = ListMenu[0];
			// 日時更新タイマー初期化
			timer.Interval = TimeSpan.FromSeconds(1);
			timer.Tick += (s, e) => DateNow = DateTime.Now;
			timer.Start();
		}

		[RelayCommand]
		void Init() {
			// 設定ファイル読み込み
			BottomMessage = AppData.Url;
			var isLogin = false;
			debugPreLogin();
			// ClickOnceかどうかの判定
			if (Environment.GetEnvironmentVariable("ClickOnce_IsNetworkDeployed")?.ToLower() != "true") {
				if (AppData.AppConfig["AutoLogin"] == "True") {
					var loginvm = new LoginViewModel();
					var ret = loginvm.AutoLogin();
					if (ret == 0) {
						isLogin = true;
						BottomMessage += $" ログイン時間{DateTime.Now:yyyy/MM/dd HH:mm:ss}";
					}
				}
			}
			if (!isLogin) { // ログインしていない場合、ログイン画面の表示
				if (ClientLib.ShowDialogView(new Views.LoginView(), this) == true) {
					BottomMessage += $" ログイン時間{DateTime.Now:yyyy/MM/dd HH:mm:ss}";
				}
			}
			debugPostLogin();
		}
		[RelayCommand]
		void Exit() {
			ClientLib.Exit(this);
		}
		[RelayCommand]
		void DoMenu(MenuData param) {
			if(param == null || param.ViewType == null) return;
			var view = Activator.CreateInstance(param.ViewType) as Window;
			if (view == null) return;
			ClientLib.ShowDialogView(view, this);
		}
		[RelayCommand]
		void DoLogin() {
			ClientLib.ShowDialogView(new Views.LoginView(), this);
		}
		[RelayCommand]
		void DoSetting() {
			ClientLib.ShowDialogView(new Views.SystemSettingView(), this);
		}
		#region テストしたいときに使う Shift+F11で実行
		[RelayCommand]
		void DoTest() {
			test_DecodeQueryString();
		}
		/// <summary>
		/// Biz/DesignerでのBrowserからサーバへの送信内容をデコードし解析するためのテストコード encodedに通信ログの送信内容を貼り付ける
		/// </summary>
		void test_DecodeQueryString() {
			string encoded = """
SELECT%20A.*%20FROM%20(select%20A.SEQ_NO,A.VDATE_CREATE
""";
			if (AppData.Http != null) {
				string decoded = System.Web.HttpUtility.UrlDecode(encoded, AppData.Http.DefaultEncode);
				Debug.WriteLine("***** HTTPのquery文字列のデコード *****");
				Debug.WriteLine(decoded);
			}
		}
		#endregion
		/// <summary>
		/// ログイン処理の前にデバッグ用の処理を追加する
		/// </summary>
		void debugPreLogin() {
		
		}
		/// <summary>
		/// ログイン処理の後にデバッグ用の処理を追加する
		/// </summary>
		void debugPostLogin() {
			/*
			var view = new Views.MeishoSelectView();
			var vm = view.DataContext as MeishoSelectViewModel;
			vm.Init(MeishoSelectViewModel.SearchType.Meisho, "1", "COL");
			if (vm != null) {
				ClientLib.ShowDialogView(view, this);
			}
			*/
		}
	}
}
