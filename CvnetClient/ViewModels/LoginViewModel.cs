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
	public partial class LoginViewModel : ObservableObject {
		[ObservableProperty]
		string? loginId;
		[ObservableProperty]
		string? loginPassword;


		[RelayCommand]
		public void Init() {
			LoginId = AppData.AppConfig["LoginId"]?? string.Empty;
			LoginPassword = AppData.AppConfig["LoginPass"] ?? string.Empty;
		}
		[RelayCommand]
		public void Exit() {
			ClientLib.Exit(this);
		}
		[RelayCommand]
		public void DoLogin() {
			var http = new ClassHttp(AppData.Url);
			var ret = http.Login(0, LoginId??  string.Empty,LoginPassword?? string.Empty);
			AppData.Http = http;
			Debug.WriteLine($"ログインステータス＝{ret}");
			if(ret == 0) {
				MessageBox.Show("ログイン成功しました");
				AppData.MasterSysKanri = http.AspxSqlQuery("select * from HC$MASTER_SYSKANRI", new string[0]);
				AppData.MasterSysTax = http.AspxSqlQuery("select * from HC$MASTER_SYSTAX", new string[0]);
				Exit();
			}
			else {
				MessageBox.Show("ログインできませんでした");
			}
		}
	}
}
