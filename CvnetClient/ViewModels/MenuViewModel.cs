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
rd=446BF4D40D3&qs=select%20A.SEQ_NO,A.VDATE_CREATE,A.VDATE_UPDATE,A.%8e%e8%93%fc%97%cd%93%60%95%5bNO,A.%8d%dd%8c%c9%8c%76%8f%e3%93%fa,A.%8a%7c%8c%76%8f%e3%93%fa,A.%8e%e6%88%f8%8b%e6%95%aa,A.%93%fc%97%cd%8e%d0%88%f5CD,A.%91%71%8c%c9CD,A.%8e%e6%88%f8%90%e6CD1,A.%8a%7c%97%a61,A.%8a%4f%90%c5%91%ce%8f%db%8b%e0%8a%7a,A.%90%94%97%ca%8d%87%8c%76,A.%96%be%8d%d7%8b%e0%8a%7a%8d%87%8c%76,A.%93%e0%90%c5%8f%c1%94%ef%90%c5,A.%8a%4f%90%c5%8f%c1%94%ef%90%c5,A.%8f%e3%91%e3%8d%87%8c%76,A.%89%ba%91%e3%8d%87%8c%76,A.%83%81%83%82,A.%8a%7c%8c%76%8f%e3FLG,A.%93%60%95%5b%8f%88%97%9d%8b%e6%95%aa,A.MOD_SEQ,A.%8a%d6%98%41%93%60%95%5bNO,A.%8a%d6%98%41%93%60%95%5bNO2,A.%97%88%8a%a8FLG,A.%8d%dd%8c%c9%8c%76%8f%e3FLG,A.%8f%c1%94%ef%90%c5%97%a6,B.%96%bc%91%4f%20%92%53%93%96%96%bc,C.%8e%64%93%fc%90%e6%96%bc%20%8e%64%93%fc%90%e6%96%bc,D.%93%be%88%d3%90%e6%96%bc%20%91%71%8c%c9%96%bc,C.%8a%7c%97%a6,C.%8a%7c%97%a62,C.%8f%c1%94%ef%90%c5CD,C.%8f%c1%94%ef%90%c5%8c%76%8e%5a%95%fb%96%40,C.%8f%c1%94%ef%90%c5%92%5b%90%94,decode(A.%8a%7c%8c%76%8f%e3FLG,1,'%81%9b','')%20%8f%b3%94%46FLG,NVL((select%20max(K.%8e%78%95%a5%93%fa)%20%8e%78%95%a5%93%fa%20from%20HC$MANAGE_KAKESIH%20K%20where%20A.%8e%e6%88%f8%90%e6CD1%3dK.%8e%64%93%fc%90%e6CD),'19010101')%20%8d%c5%8f%49%92%f7%93%fa%20from%20HC$Tran_TORI0%20A,HC$MASTER_SHAIN%20B,HC$Master_SIIRE%20C,HC$MASTER_TOKUI%20D%20where%20(A.%93%fc%97%cd%8e%d0%88%f5CD%3dB.%8e%d0%88%f5CD(%2b))%20and%20(A.%8e%e6%88%f8%90%e6CD1%3dC.%8e%64%93%fc%90%e6CD(%2b))%20and%20(A.%91%71%8c%c9CD%3dD.%93%be%88%d3%90%e6CD(%2b))%20and%20A.SEQ_NO%20between%20%3a1%20and%20%3a2%20and%20A.%8d%dd%8c%c9%8c%76%8f%e3%93%fa%20between%20%3a3%20and%20%3a4%20and%20A.%8e%e6%88%f8%8b%e6%95%aa%20between%20%3a5%20and%20%3a6%20and%20A.%8a%d6%98%41%93%60%95%5bNO%20between%20%3a7%20and%20%3a8%20and%20A.%8a%d6%98%41%93%60%95%5bNO2%20between%20%3a9%20and%20%3a10%20and%20A.%8e%e6%88%f8%90%e6CD1%20between%20%3a11%20and%20%3a12%20and%20A.%91%71%8c%c9CD%20between%20%3a13%20and%20%3a14%20and%20A.%8e%e8%93%fc%97%cd%93%60%95%5bNO%20between%20%3a15%20and%20%3a16%20and%20A.%93%fc%97%cd%8e%d0%88%f5CD%20between%20%3a17%20and%20%3a18%20and%20A.%93%60%95%5b%8f%88%97%9d%8b%e6%95%aa%3d3%20ORDER%20BY%20A.SEQ_NO%20DESC&ps=1&pdf=1&p1=0&p2=9999999999&p3=20250601&p4=20991231&p5=00&p6=99&p7=0&p8=99999999999999&p9=0&p10=99999999999999&p11=&p12=99999999&p13=&p14=99999999&p15=&p16=%df&p17=&p18=99999999
rd=446BF4D40D3&qs=select%20*%20from%20(select%20A.SEQ_NO,A.VDATE_CREATE,A.VDATE_UPDATE,A.%8e%e8%93%fc%97%cd%93%60%95%5bNO,A.%8d%dd%8c%c9%8c%76%8f%e3%93%fa,A.%8a%7c%8c%76%8f%e3%93%fa,A.%8e%e6%88%f8%8b%e6%95%aa,A.%93%fc%97%cd%8e%d0%88%f5CD,A.%91%71%8c%c9CD,A.%8e%e6%88%f8%90%e6CD1,A.%8a%7c%97%a61,A.%8a%4f%90%c5%91%ce%8f%db%8b%e0%8a%7a,A.%90%94%97%ca%8d%87%8c%76,A.%96%be%8d%d7%8b%e0%8a%7a%8d%87%8c%76,A.%93%e0%90%c5%8f%c1%94%ef%90%c5,A.%8a%4f%90%c5%8f%c1%94%ef%90%c5,A.%8f%e3%91%e3%8d%87%8c%76,A.%89%ba%91%e3%8d%87%8c%76,A.%83%81%83%82,A.%8a%7c%8c%76%8f%e3FLG,A.%93%60%95%5b%8f%88%97%9d%8b%e6%95%aa,A.MOD_SEQ,A.%8a%d6%98%41%93%60%95%5bNO,A.%8a%d6%98%41%93%60%95%5bNO2,A.%97%88%8a%a8FLG,A.%8d%dd%8c%c9%8c%76%8f%e3FLG,A.%8f%c1%94%ef%90%c5%97%a6,B.%96%bc%91%4f%20%92%53%93%96%96%bc,C.%8e%64%93%fc%90%e6%96%bc%20%8e%64%93%fc%90%e6%96%bc,D.%93%be%88%d3%90%e6%96%bc%20%91%71%8c%c9%96%bc,C.%8a%7c%97%a6,C.%8a%7c%97%a62,C.%8f%c1%94%ef%90%c5CD,C.%8f%c1%94%ef%90%c5%8c%76%8e%5a%95%fb%96%40,C.%8f%c1%94%ef%90%c5%92%5b%90%94,decode(A.%8a%7c%8c%76%8f%e3FLG,1,'%81%9b','')%20%8f%b3%94%46FLG,NVL((select%20max(K.%8e%78%95%a5%93%fa)%20%8e%78%95%a5%93%fa%20from%20HC$MANAGE_KAKESIH%20K%20where%20A.%8e%e6%88%f8%90%e6CD1%3dK.%8e%64%93%fc%90%e6CD),'19010101')%20%8d%c5%8f%49%92%f7%93%fa%20from%20HC$Tran_TORI0%20A,HC$MASTER_SHAIN%20B,HC$Master_SIIRE%20C,HC$MASTER_TOKUI%20D%20where%20(A.%93%fc%97%cd%8e%d0%88%f5CD%3dB.%8e%d0%88%f5CD(%2b))%20and%20(A.%8e%e6%88%f8%90%e6CD1%3dC.%8e%64%93%fc%90%e6CD(%2b))%20and%20(A.%91%71%8c%c9CD%3dD.%93%be%88%d3%90%e6CD(%2b))%20and%20A.SEQ_NO%20between%20%3a1%20and%20%3a2%20and%20A.%8d%dd%8c%c9%8c%76%8f%e3%93%fa%20between%20%3a3%20and%20%3a4%20and%20A.%8e%e6%88%f8%8b%e6%95%aa%20between%20%3a5%20and%20%3a6%20and%20A.%8a%d6%98%41%93%60%95%5bNO%20between%20%3a7%20and%20%3a8%20and%20A.%8a%d6%98%41%93%60%95%5bNO2%20between%20%3a9%20and%20%3a10%20and%20A.%8e%e6%88%f8%90%e6CD1%20between%20%3a11%20and%20%3a12%20and%20A.%91%71%8c%c9CD%20between%20%3a13%20and%20%3a14%20and%20A.%8e%e8%93%fc%97%cd%93%60%95%5bNO%20between%20%3a15%20and%20%3a16%20and%20A.%93%fc%97%cd%8e%d0%88%f5CD%20between%20%3a17%20and%20%3a18%20and%20A.%93%60%95%5b%8f%88%97%9d%8b%e6%95%aa%3d3%20and%20A.SEQ_NO%20%3c%3d%20'10532513'%20ORDER%20BY%20A.SEQ_NO%20DESC)%20where%20rownum%3c%3d40&ps=1&pdf=1&p1=0&p2=9999999999&p3=20250601&p4=20991231&p5=00&p6=99&p7=0&p8=99999999999999&p9=0&p10=99999999999999&p11=&p12=99999999&p13=&p14=99999999&p15=&p16=%df&p17=&p18=99999999
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
