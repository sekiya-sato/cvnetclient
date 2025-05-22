using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvnetClient.ViewModels {
	/// <summary>
	/// メンテ画面を想定したViewModelの基底クラス
	/// </summary>
	public partial class BaseViewModel : ObservableObject {
		/// <summary>
		/// 終了
		/// </summary>
		[RelayCommand]
		void Exit() {
			ClientLib.Exit(this);
		}
	}
}
