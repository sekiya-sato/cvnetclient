/* ============================================================================
 * CvnetBaseCore.dll : Extra.cs
 * Created by Sekiya.Sato 2025/05/13
 * 説明: その他旧Cvnetサーバ用クラス
 * 使用ライブラリ
 *		なし
 * ============================================================================  */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvnetBaseCore {
	public class Extra {
		public static string Description = "Cvnetサーバ用class (.NET Framework4.7以前)";
		public static string NetFramework = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;

		public static void DoShomething() {
			NLog.LogManager.GetCurrentClassLogger().Info("Hello from CvnetBaseCore");
			Console.WriteLine("Hello from CvnetBaseCore");
			var srv = new ClassHttp("https://ap03.dtpnet.co.jp/cv.net_dev03/");
			var ret = srv.Login(0, "123", "123");
		}
	}
}
