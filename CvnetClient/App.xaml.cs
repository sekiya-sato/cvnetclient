
/* ============================================================================
 * CvnetClient.exe : App.xaml.cs
 * Created by Sekiya.Sato 2025/05/21
 * 説明: App.xaml コードビハインド
 * 使用ライブラリ: なし
 *	開発メモ [Development notes]:
 *		アプリケーション共通でXAMLでの言語表示を変更
 * ============================================================================  */
using CvnetBaseCore;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Markup;

namespace CvnetClient {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {
		public App() {
			//XAMLでの言語表示を変更。各xamlで Language="ja"などを指定しなくても、自動で言語を切り替える
			if (Thread.CurrentThread.CurrentCulture != null) {
				var culture = Thread.CurrentThread.CurrentCulture; // "ja-JP" or "en-US"
				XmlLanguage language = XmlLanguage.GetLanguage(culture.IetfLanguageTag);
				FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(language));
			}
		}
		/// <summary>
		/// catch 漏れの例外が発生した時に呼ばれます。
		/// </summary>
		/// <param name="sender">呼び出し元）</param>
		/// <param name="e">Handled フラグを立てないとアプリが強制終了します。原因は Exception プロパティに記載。</param>
		private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
			var log = NLog.LogManager.GetCurrentClassLogger();
			log.Error(e.Exception, string.Format("catch漏れの例外による終了 ----------.\n"));
			e.Handled = true;
			App.Current.Shutdown(-9999);
		}
		/// <summary>
		/// 例外が発生する前にとらえる（「ストア メタデータ "CurrentBind" が無効です」が多発する？）
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CurrentDomain_FirstChanceException(
						  object sender,
						  System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e) {
			var log = NLog.LogManager.GetCurrentClassLogger();
				log.Error(e.Exception, "FirstChanceException例外が{0}で発生。/{1}/{2}/",
					e.Exception?.TargetSite?.Name,
					(string.IsNullOrEmpty(e.Exception?.Message) ? "" : "【Msg】" + e.Exception.Message),
					(string.IsNullOrEmpty(e.Exception?.Source) ? "" : "【Src】" + e.Exception.Source),
					(string.IsNullOrEmpty(e.Exception?.StackTrace) ? "" : "【Trace】" + e.Exception.StackTrace) // Traceは長いので書き出さない
					);
		}
	}
}




