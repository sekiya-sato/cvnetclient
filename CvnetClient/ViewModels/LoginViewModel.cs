/* ============================================================================
 * CvnetClient.exe : LoginViewModels.cs
 * Created by Sekiya.Sato 2025/05/13
 * 説明: ログイン画面
 * 使用ライブラリ
 *		CommunityToolkit.Mvvm : LICENCE = MIT
 * ============================================================================  */
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using System.Diagnostics;

namespace CvnetClient.ViewModels {
	public partial class LoginViewModel : ObservableObject {
		[ObservableProperty]
		string? loginId;
		[ObservableProperty]
		string? loginPassword;


		[RelayCommand]
		void Init() {
			LoginId = AppData.AppConfig["LoginId"]?? string.Empty;
			LoginPassword = AppData.AppConfig["LoginPass"] ?? string.Empty;
		}
		[RelayCommand]
		void Exit() {
			ClientLib.Exit(this);
		}
		[RelayCommand]
		void DoLogin() {
			var http = AppData.Http ?? new ClassHttp(AppData.Url);
			var ret = http.Login(0, LoginId ?? string.Empty, LoginPassword ?? string.Empty);
			AppData.Http = http;
			Debug.WriteLine($"ログインステータス＝{ret}");
			if(ret.Item1 == 0) { 
                ClientLib.MessageBoxOk(this, "ログイン成功しました");
                AppData.ClassSatoo.SHAIN_CD = ret.Item2.SHAIN_CD;
                AppData.ClassSatoo.SHAIN_Name = ret.Item2.SHAIN_Name;
                AppData.ClassSatoo.SHAIN_Tenpo = ret.Item2.SHAIN_Tenpo;
                AppData.ClassSatoo.AspxRandId = ret.Item2.AspxRandId;
                AppData.MasterSysKanri = http.AspxSqlQuery("select * from HC$MASTER_SYSKANRI", new string[0]);
				AppData.MasterSysTax = http.AspxSqlQuery("select * from HC$MASTER_SYSTAX", new string[0]);

                AppData.ClassCvnet.AspxSqlQuerySysMst();
				AppData.ClassCvnet.AspxSqlQuerySysKintaiMst();
                AppData.ClassCvnet.AspxSqlQuerySysHHTMst();
				AppData.ClassCvnet.AspxSqlQueryConfig(); 

                var win = ClientLib.GetActiveView(this);
				if(win != null) 
					win.DialogResult = true;
				Exit();
			}
			else {
				ClientLib.MessageBoxOk(this, "ログインできませんでした");
			}
		}
		/// <summary>
		/// AppDataからId,Passを取得してログインする
		/// </summary>
		/// <returns></returns>
		public int AutoLogin() {
			Init();
			var http = AppData.Http ?? new ClassHttp(AppData.Url);
			var ret = http.Login(0, LoginId ?? string.Empty, LoginPassword ?? string.Empty);
			AppData.Http = http;
			if(ret.Item1 == 0) {
				AppData.ClassSatoo.SHAIN_CD = ret.Item2.SHAIN_CD;
                AppData.ClassSatoo.SHAIN_Name = ret.Item2.SHAIN_Name;
				AppData.ClassSatoo.SHAIN_Tenpo = ret.Item2.SHAIN_Tenpo;
				AppData.ClassSatoo.AspxRandId = ret.Item2.AspxRandId;
                AppData.MasterSysKanri = http.AspxSqlQuery("select * from HC$MASTER_SYSKANRI", new string[0]);
				AppData.MasterSysTax = http.AspxSqlQuery("select * from HC$MASTER_SYSTAX", new string[0]);
			}
			return ret.Item1;
		}
	}
}
