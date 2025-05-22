/* ============================================================================
 * Cvnet8wpfclient.exe : MasterData.cs
 * Created by Sekiya.Sato 2025/05/21
 * 説明: マスターの定義
 * 使用ライブラリ [Library used]:
 *		CommunityToolkit.Mvvm : LICENCE = MIT
 * ============================================================================  */
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvnetClient.Models {
	internal class MasterData {
	}
	public partial class  MasterMeisho : ObservableObject {
		[ObservableProperty]
		long seqNo;
		[ObservableProperty]
		string kubun;
		[ObservableProperty]
		string meishoCd;
		[ObservableProperty]
		string meisho;
		[ObservableProperty]
		string ryakuShou;
		[ObservableProperty]
		string rank;
		[ObservableProperty]
		string renban;
		[ObservableProperty]
		string kana;
		[ObservableProperty]
		string posKubun;
		[ObservableProperty]
		string saishuuShuuseiSha;
		[ObservableProperty]
		decimal vdateCreate;
		[ObservableProperty]
		decimal vdateUpdate;
	}


}
