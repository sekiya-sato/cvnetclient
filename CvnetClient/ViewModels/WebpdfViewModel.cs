/* ============================================================================
 * CvnetClient.exe : WebPdfViewModel.cs
 * Created by Sekiya.Sato 2025/05/22
 * 説明: PDF表示画面
 * 使用ライブラリ
 *		CommunityToolkit.Mvvm : LICENCE = MIT
 *		Microsoft.Web.WebView2 : LICENCE = https://www.nuget.org/packages/Microsoft.Web.WebView2/1.0.3240.44/license
 * ============================================================================  */
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvnetClient.ViewModels {
	public partial class WebpdfViewModel : BaseViewModel {
		[ObservableProperty]
		string? pdfdata;
	}
}
