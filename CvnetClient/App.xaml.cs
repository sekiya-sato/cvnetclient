
/* ============================================================================
 * CvnetClient.exe : App.xaml.cs
 * Created by Sekiya.Sato 2025/05/21
 * 説明: App.xaml コードビハインド
 * 使用ライブラリ: なし
 *	開発メモ [Development notes]:
 *		アプリケーション共通でXAMLでの言語表示を変更
 * ============================================================================  */
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
	}
}




