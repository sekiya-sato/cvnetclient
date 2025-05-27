/* ============================================================================
 * Cvnet8wpfclient.exe : AppData.cs
 * Created by Sekiya.Sato 2025/05/20
 * 説明: アプリケーション共通データの定義
 * 使用ライブラリ [Library used]:
 *		CommunityToolkit.Mvvm : LICENCE = MIT
 * ============================================================================  */
using CvnetBaseCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;

namespace CvnetClient.Models {
	public class AppData {
		public static string AppName { get; set; } = "CvnetClient";
		public static string AppVer { get; set; } = "1.0.0";
		public static string AppPath { get; set; } = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? string.Empty;
		public static string AppExePath { get; set; } = System.IO.Path.Combine(AppPath, AppName + ".exe");
		public static string AppDataPath { get; set; } = System.IO.Path.Combine(AppPath, "data");
		public static string AppConfigPath { get; set; } = System.IO.Path.Combine(AppDataPath, "appsettings.json");
		public static IConfigurationSection AppConfig { get; set; }
		public static string Url { get; set; }
		public static ClassHttp? Http { get; set; }
		public static int maxQueryCnt { get; set; } = 40;
		public static int maxPrintCnt { get; set; } = 110;
		public static DataTable MasterSysKanri { get; set; } = new DataTable();
		public static DataTable MasterSysTax { get; set; } = new DataTable();

		static AppData() {
			var settingfile = "appsettings.json";
			// 設定ファイル読み込み
			IConfiguration config = new ConfigurationBuilder()
				.AddJsonFile(settingfile, optional: true, reloadOnChange: true)
				.Build();
			AppConfig = config.GetSection("AppSetting");
			Url = AppConfig["Url"] ?? "https://localhost/";
			Http = new ClassHttp(AppData.Url);
		}
	}
}
