/* ============================================================================
 * CvnetClient.exe : WebPdfViewModel.cs
 * Created by Sekiya.Sato 2025/05/22
 * 説明: PDF表示画面
 * 使用ライブラリ
 *		CommunityToolkit.Mvvm : LICENCE = MIT
 *		Microsoft.Web.WebView2 : LICENCE = https://www.nuget.org/packages/Microsoft.Web.WebView2/1.0.3240.44/license
 * ============================================================================  */
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvnetClient.ViewModels {
	public partial class WebTextViewModel : BaseViewModel {
		[ObservableProperty]
		string? data;
		[ObservableProperty]
		string? title;

		[RelayCommand]
		void Init() {
			// "ClickOnce" で始まる環境変数をすべて取得し、Dataにセット
			var envVars = Environment.GetEnvironmentVariables();
			var sb = new StringBuilder();
			foreach (System.Collections.DictionaryEntry entry in envVars) {
				string key = entry.Key?.ToString() ?? "";
				if (key.StartsWith("ClickOnce", StringComparison.OrdinalIgnoreCase)) {
					sb.AppendLine($"{key} = {entry.Value}");
				}
			}
			Data = sb.Length > 0 ? sb.ToString() : "ClickOnce環境変数は見つかりませんでした。";
		}
	}
}
