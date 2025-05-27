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
	fm=cvnet_meisho.qfm&rd=6E0BF4D0723&qs=select%20A.SEQ_NO,SUBSTR(GET_VDATE(a.VDATE_CREATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_CREATE),10,6)%20%8d%ec%90%ac%93%fa%8e%9e,SUBSTR(GET_VDATE(a.VDATE_UPDATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_UPDATE),10,6)%20%8d%58%90%56%93%fa%8e%9e,A.%96%bc%8f%cc%8b%e6%95%aa%20||'%20'||%20(SELECT%20m.%96%bc%8f%cc%20FROM%20HC$Master_MEISHO%20m%20WHERE%20m.%96%bc%8f%cc%8b%e6%95%aa%3d'IDX'%20AND%20m.%96%bc%8f%ccCD%3dA.%96%bc%8f%cc%8b%e6%95%aa)%20%96%bc%8f%cc%8b%e6%95%aa,A.%96%bc%8f%ccCD,A.%96%bc%8f%cc,A.%97%aa%8f%cc,A.%83%89%83%93%83%4e,A.%98%41%94%d4,A.%83%4a%83%69,CASE%20WHEN%20(A.POS%8b%e6%95%aa%3d0)%20THEN%20'0%20%8f%6f%97%cd%82%b7%82%e9'%20WHEN%20(A.POS%8b%e6%95%aa%3d9)%20THEN%20'9%20%8d%ed%8f%9c%8e%77%8e%a6'%20WHEN%20(A.POS%8b%e6%95%aa%3d10)%20THEN%20'10%20%8f%6f%97%cd%82%b5%82%c8%82%a2'%20ELSE%20'.'%20END%20POS%8b%e6%95%aa,(A.%93%fc%97%cd%8e%d0%88%f5CD%20||'%20'||%20(select%20B.%96%bc%91%4f%20from%20HC$MASTER_SHAIN%20B%20where%20B.%8e%d0%88%f5CD%3dA.%93%fc%97%cd%8e%d0%88%f5CD))%20%8d%c5%8f%49%8f%43%90%b3%8e%d2%20from%20HC$Master_MEISHO%20A%20%20where%20A.%96%bc%8f%cc%8b%e6%95%aa%20%3d%3a1%20%20order%20by%20A.%96%bc%8f%ccCD&ps=1&pdf=1&p1=ATT
	fm=cvnet_meisho.qfm&rd=6E0BF4D0723&qs=select%20A.SEQ_NO,SUBSTR(GET_VDATE(a.VDATE_CREATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_CREATE),10,6)%20%8d%ec%90%ac%93%fa%8e%9e,SUBSTR(GET_VDATE(a.VDATE_UPDATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_UPDATE),10,6)%20%8d%58%90%56%93%fa%8e%9e,A.%96%bc%8f%cc%8b%e6%95%aa%20||'%20'||%20(SELECT%20m.%96%bc%8f%cc%20FROM%20HC$Master_MEISHO%20m%20WHERE%20m.%96%bc%8f%cc%8b%e6%95%aa%3d'IDX'%20AND%20m.%96%bc%8f%ccCD%3dA.%96%bc%8f%cc%8b%e6%95%aa)%20%96%bc%8f%cc%8b%e6%95%aa,A.%96%bc%8f%ccCD,A.%96%bc%8f%cc,A.%97%aa%8f%cc,A.%83%89%83%93%83%4e,A.%98%41%94%d4,A.%83%4a%83%69,CASE%20WHEN%20(A.POS%8b%e6%95%aa%3d0)%20THEN%20'0%20%8f%6f%97%cd%82%b7%82%e9'%20WHEN%20(A.POS%8b%e6%95%aa%3d9)%20THEN%20'9%20%8d%ed%8f%9c%8e%77%8e%a6'%20WHEN%20(A.POS%8b%e6%95%aa%3d10)%20THEN%20'10%20%8f%6f%97%cd%82%b5%82%c8%82%a2'%20ELSE%20'.'%20END%20POS%8b%e6%95%aa,(A.%93%fc%97%cd%8e%d0%88%f5CD%20||'%20'||%20(select%20B.%96%bc%91%4f%20from%20HC$MASTER_SHAIN%20B%20where%20B.%8e%d0%88%f5CD%3dA.%93%fc%97%cd%8e%d0%88%f5CD))%20%8d%c5%8f%49%8f%43%90%b3%8e%d2%20from%20HC$Master_MEISHO%20A%20%20where%20A.%96%bc%8f%cc%8b%e6%95%aa%20%3d%3a1%20%20and%20A.%96%bc%8f%ccCD%20in('100','1130','12345678901234','19990123','2','3','4','50','55','56','57','58','99','99994','99996','99997','10012')%20order%20by%20A.%96%bc%8f%ccCD&ps=1&pdf=1&p1=ATT
""";
			if (AppData.Http != null) {
				string decoded = System.Web.HttpUtility.UrlDecode(encoded, AppData.Http.DefaultEncode);
				Debug.WriteLine("***** HTTPのquery文字列のデコード *****");
				Debug.WriteLine(decoded);
			}
		}
		#endregion
	}
}
