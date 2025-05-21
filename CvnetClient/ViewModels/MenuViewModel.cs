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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

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
		public void Init() {
			// 設定ファイル読み込み
			BottomMessage = AppData.Url;
			// ログイン画面の表示
			ClientLib.ShowDialogView(new Views.LoginView(), this);
		}
		[RelayCommand]
		public void Exit() {
			ClientLib.Exit(this);
		}
		[RelayCommand]
		public void DoMenu(MenuData param) {
			if(param == null || param.ViewType == null) return;
			var view = Activator.CreateInstance(param.ViewType) as Window;
			if (view == null) return;
			ClientLib.ShowDialogView(view, this);
		}
		[RelayCommand]
		public void DoLogin() {
			ClientLib.ShowDialogView(new Views.LoginView(), this);
		}
		#region テストしたいときに使う Shift+F11で実行
		[RelayCommand]
		public void DoTest() {
		}
		#endregion
	}
}
