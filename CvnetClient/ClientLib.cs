/* ============================================================================
 * CvnetClient.exe : ClientLib.cs
 * Created by Sekiya.Sato 2025/05/13
 * 説明: ViewModel側からViewを操作するためのクラス
 * 使用ライブラリ
 *		NLog : LICENCE = BSD 3-Clause
 * 開発メモ
 * ============================================================================  */
using CvnetClient.ViewModels;
using System.Windows;

namespace CvnetClient {
	/// <summary>
	/// 主にViewModel側からViewを操作するためのクラス
	/// [Class mainly for manipulating the View from the ViewModel]
	/// </summary>
	public class ClientLib
    {
		/// <summary>
		/// 自分のViewを閉じる
		/// [Close the active Window]
		/// </summary>
		/// <param name="vm">ViewModelインスタンス</param>
		public static void Exit(object vm)
        {
			var win = GetActiveView(vm);
			if (win != null) {
				win.Close();
				if (win.Owner != null)
					win.Owner.Activate();
			}
		}
        /// <summary>
        /// 全てのWindowを閉じる
        /// [Close all Windows]
        /// </summary>
        public static void ExitAll()
        {
            foreach (var win in Application.Current.Windows.OfType<Window>().Reverse())
            {
                win.Close();
            }
        }
		/// <summary>
		/// 自分と親以外全てのWindowを閉じる
		/// [Close all Windows except for the current and parent ones]
		/// </summary>
		/// <param name="vm">ViewModelインスタンス</param>
		public static void ExitAllWithoutMe(object vm) {
			var myview = GetActiveView(vm);
			var parent = myview?.Owner;
			foreach (var win in Application.Current.Windows.OfType<Window>().Reverse()) {
				if (win != myview && win != parent)
					win.Close();
			}
		}
		/// <summary>
		/// ViewModelが紐づけられてるViewを取得する
		/// [Retrieve the View associated with the ViewModel]
		/// </summary>
		/// <param name="vm">ViewModelインスタンス</param>
		/// <returns>Viewインスタンス</returns>
		public static Window? GetActiveView(object vm) {
			Window? myWin = null;
			var activeWins = Application.Current.Windows.OfType<Window>().Reverse();
			foreach (var ac in activeWins) {
				var myVm = ac.DataContext;
				if (myVm == vm) {
					myWin = ac;
					return myWin;
				}
			}
			return myWin;
		}
		/// <summary>
		/// ViewのDialogResultを設定して閉じる
		/// [Set the DialogResult of the View and close it]
		/// </summary>
		/// <param name="vm">ViewModelインスタンス</param>
		/// <param name="result"></param>
		public static void ExitDialogResult(object vm, bool result) {
			var win = GetActiveView(vm);
			if (win != null) {
				try {
					win.DialogResult = result; 
				}
				catch (Exception) {
                    /* ShowかShowDialogか自分でわかってない。Showの場合にはExceptionが生じる*/
                    //[Whether to use Show or ShowDialog is not determined by this code]
                }
                Exit(vm);
			}
		}
		/// <summary>
		/// OK,Cancelの確認メッセージを表示する
		/// </summary>
		/// <param name="vm"></param>
		/// <param name="message"></param>
		/// <param name="title"></param>
		/// <param name="fontSize"></param>
		/// <returns></returns>
		public static bool MessageBox(object vm, string message, string title = "確認", double fontSize = 18) {
			var v = new Views.CustomMessageBox();
			v.Owner = GetActiveView(vm);
			var subvm = v.DataContext as CustomMessageBoxViewModel;
			if(subvm != null) {
				subvm.Message = message; // メッセージを設定 [Set the message]
				subvm.Title = title; // タイトルを設定 [Set the title]
				subvm.FontSize = fontSize; // フォントサイズを設定 [Set the font size]
			}
			var ret = v.ShowDialog();
			if (ret == true)
				return true;
			else 
				return false; // キャンセルの場合はfalseを返す [Return false for cancel]
		}
		/// <summary>
		/// Yes,Noの確認メッセージを表示する
		/// </summary>
		/// <param name="vm"></param>
		/// <param name="message"></param>
		/// <param name="title"></param>
		/// <param name="fontSize"></param>
		/// <returns></returns>
		public static bool MessageBoxYesno(object vm, string message, string title = "確認", double fontSize = 18) {
			var v = new Views.CustomMessageBox();
			v.Owner = GetActiveView(vm);
			var subvm = v.DataContext as CustomMessageBoxViewModel;
			if (subvm != null) {
				subvm.Message = message; // メッセージを設定 [Set the message]
				subvm.Title = title; // タイトルを設定 [Set the title]
				subvm.FontSize = fontSize; // フォントサイズを設定 [Set the font size]
				subvm.BtnOkText = "Yes"; // Yes button text
				subvm.BtnCancelText = "No"; // No button text
			}
			var ret = v.ShowDialog();
			if (ret == true)
				return true;
			else
				return false; // キャンセルの場合はfalseを返す [Return false for cancel]
		}
		/// <summary>
		/// OKボタンのみの確認メッセージを表示する
		/// </summary>
		/// <param name="vm"></param>
		/// <param name="message"></param>
		/// <param name="fontSize"></param>
		/// <returns></returns>
		public static bool MessageBoxOk(object vm, string message, string title = "確認", double fontSize = 18) {
			var v = new Views.CustomMessageBox();
			v.Owner = GetActiveView(vm);
			var subvm = v.DataContext as CustomMessageBoxViewModel;
			if (subvm != null) {
				subvm.Message = message; // メッセージを設定 [Set the message]
				subvm.Title = title; // タイトルを設定 [Set the title]
				subvm.FontSize = fontSize; // フォントサイズを設定 [Set the font size]
				subvm.BtnOkEnabled = false; // 位置的にCancelボタンのみ表示
				subvm.BtnCancelEnabled = true;
				subvm.BtnCancelText = "OK";
			}
			v.ShowDialog();
			return true;
		}
		/// <summary>
		/// OKボタンのみのエラー確認メッセージを表示する
		/// </summary>
		/// <param name="vm"></param>
		/// <param name="message"></param>
		/// <param name="fontSize"></param>
		/// <returns></returns>
		public static bool MessageBoxError(object vm, string message, string title = "エラー", double fontSize = 18) {
			var v = new Views.CustomMessageBox();
			v.Owner = GetActiveView(vm);
			var subvm = v.DataContext as CustomMessageBoxViewModel;
			if (subvm != null) {
				subvm.Message = message; // メッセージを設定 [Set the message]
				subvm.Title = title; // タイトルを設定 [Set the title]
				subvm.FontSize = fontSize; // フォントサイズを設定 [Set the font size]
				subvm.BtnOkEnabled = false; // 位置的にCancelボタンのみ表示
				subvm.BtnCancelEnabled = true;
				subvm.BtnCancelText = "OK";
			}
			v.ShowDialog();
			return true;
		}
		/// <summary>
		/// Viewを親として子Windowをオープンする
		/// [Open a child Window with the View as its parent]
		/// </summary>
		/// <param name="childWin">子Window</param> [Child Window]
		/// <param name="loc">表示位置</param> [Display position]
		/// <param name="IsDialog">true=ダイアログとして表示 false=独立Windowsとして表示</param> 
		/// [true = Display as a dialog, false = Display as an independent Window]
		/// <param name="IsShowTaskbar">true=タスクバーに表示 false=表示しない</param>
		/// [true = Display in the taskbar, false = Do not display]
		/// IsShowTaskbar
		public static bool? ShowDialogView(Window childWin,object? myVm, WindowStartupLocation loc = WindowStartupLocation.CenterOwner, bool IsDialog = true, bool IsShowTaskbar=false) {
			if(myVm !=null)
	            childWin.Owner = GetActiveView(myVm);
            childWin.WindowStartupLocation = loc;
			childWin.ShowInTaskbar = IsShowTaskbar;
			if (IsDialog) 
				return childWin.ShowDialog();
			else {
				childWin.Show();
				return null;
			}
		}
        /// <summary>
        /// DataGridに対しDictionary型を参照して列を作成する
        /// [Create columns in a DataGrid by referring to a Dictionary type]
        /// </summary>
        /// <param name="dg"></param>
        /// <param name="obj"></param>
        public static void SetDataGridDic(System.Windows.Controls.DataGrid dg, Dictionary<string, object> obj) {
			dg.Columns.Clear();
			dg.AutoGenerateColumns = false;
			foreach (var item in obj.Keys) { // 列を追加する [Add columns]
				var textColumn = new System.Windows.Controls.DataGridTextColumn();
				textColumn.Header = item;
				textColumn.Binding = new System.Windows.Data.Binding($"Item[{item}]");
				dg.Columns.Add(textColumn);
			}
		}

        /// <summary>
        /// 使用可能なデータフォルダを取得
        /// [Retrieve the available data folder]
        /// </summary>
        /// <returns>データフォルダ</returns> [Data folder]
        public static string GetDataDir() {
			try {
				string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                //string folder = appData + "\\" + System.Windows.Forms.Application.CompanyName + "\\" + System.Windows.Forms.Application.ProductName;
                // 固定で返す(バージョンを大きく変える場合には変更する)
                // [Return a fixed value (change if a major version update is made)]
                string folder = appData + @"\cvnetclient";
				if (!System.IO.Directory.Exists(folder)) {
					System.IO.Directory.CreateDirectory(folder);
				}
				return folder;
			}
			catch (Exception ex) {
				var log = NLog.LogManager.GetCurrentClassLogger();
				log.Error(ex, "GetDataDirエラー");
				return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			}
		}
		/// <summary>
		/// マウスカーソルをWiaitに変更する(重そうな処理の前に実行)
		/// </summary>
		public static void CursorToWait() {
					System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
		}
		/// <summary>
		/// マウスカーソルをもとに戻す
		/// </summary>
		public static void CursorToNormal() {
			System.Windows.Input.Mouse.OverrideCursor = null;
		}


	}
}
