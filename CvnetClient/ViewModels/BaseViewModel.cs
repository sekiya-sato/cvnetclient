using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Utils;
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
        #region Variable
        public BizArray para;
        #endregion

        /// <summary>
        /// 終了
        /// </summary>
        [RelayCommand]
		void Exit() {
			//
			ClientLib.Exit(this);
		}
        [RelayCommand]
        public void Minimize() => ClientLib.Minimize(this);

        [RelayCommand]
        public void Maximize() => ClientLib.Maximize(this);

        // Command to close the window
        [RelayCommand]
        public void Close() => ClientLib.Exit(this);

        // Command to DragMove the window
        [RelayCommand]
        public void DragMove() => ClientLib.DragMove(this);

        #region Additional Method
        public void OnInitBase(object? init_para = null)
        {
            if (init_para != null)
            {
                if (init_para is string s)
                {
                    var v_para = new string[] { s };
                    para = new BizArray(v_para);
                }
                else if (init_para is string[] arr)
                    para = new BizArray(arr);
                else para = new BizArray();
            }
            else para = new BizArray();
        }
        #endregion
    }
}
