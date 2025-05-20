#pragma warning disable CS8600
#pragma warning disable CS8601
#pragma warning disable CS8602
#pragma warning disable CS8603
#pragma warning disable CS8604
#pragma warning disable CS8622
#pragma warning disable CS8625
#pragma warning disable SYSLIB0014
#pragma warning disable SYSLIB0057
#pragma warning disable CA1416
#pragma warning disable CA2022
/* ============================================================================
 * CvnetBaseCore.dll : ClassHttp.cs
 * Created by Sekiya.Sato 2025/05/13
 * 説明: サーバアクセス用クラス CVPOS3互換(.net framework 4) 既存ロジック流用のためpragmaでエラー抑制
 *		2025/05時点のCV.netサーバ用
 * 使用ライブラリ
 *		NLog : LICENCE = BSD 3-Clause
 * ============================================================================  */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CvnetBaseCore {
	/// <summary>
	/// サーバアクセス用共通Class(Cv.net互換)
	/// </summary>
	public class ClassHttp {
		private string v_URLroot = "";
		private string v_AddUrl = "";
		private string v_randid = "";
		private CookieContainer v_cc = new CookieContainer();

		/// <summary>
		/// SQLEXE戻り値用struct
		/// </summary>
		public struct CvRetAspx {
			/// <summary>
			/// 戻り値
			/// </summary>
			public int Code;
			/// <summary>
			/// 新SEQ_NO
			/// </summary>
			public long NewSeq;
			/// <summary>
			/// 新VDATE_UPDATE(string)
			/// </summary>
			public string VDate;
			/// <summary>
			/// オプション1
			/// </summary>
			public String Opt1;
			/// <summary>
			/// オプション2
			/// </summary>
			public String Opt2;
			/// <summary>
			/// テーブル名
			/// </summary>
			public String TableName;
		}

		/// <summary>
		/// 初期化のときにしか設定できない(http://..../)
		/// </summary>
		/// <example>https://ap03.dtpnet.co.jp/</example>
		public string URLroot {
			get { return v_URLroot; }
		}
		/// <summary>
		/// 初期化のときにしか設定できない(/isql.../)
		/// </summary>
		/// <example>/isql/</example>
		public string AddUrl {
			get { return v_AddUrl; }
		}
		/// <summary>
		/// ログインした際のRandID
		/// </summary>
		public string RandID {
			get { return v_randid; }
		}
		private DataTable v_infologin = new DataTable();
		/// <summary>
		/// ログインした際に取得したDataTable
		/// </summary>
		public DataTable InfoLogin {
			get { return v_infologin; }
		}
		Encoding v_defaultEncode = Encoding.UTF8;
		/// <summary>
		/// デフォルトエンコード
		/// </summary>
		public Encoding DefaultEncode {
			get { return v_defaultEncode; }
		}
		string v_shainCD = "";
		/// <summary>
		/// デフォルト社員コード
		/// </summary>
		public string ShainCD {
			get { return v_shainCD; }
		}
		string v_loginid = "";
		/// <summary>
		/// ログインしたときのID
		/// </summary>
		public string LoginID {
			get { return v_loginid; }
		}
		/// <summary>
		/// Webでのエラー
		/// </summary>
		public WebException WebError = null;
		/// <summary>
		/// 何かのエラー
		/// </summary>
		public Exception WebError2 = null;
		/// <summary>
		/// 最後のヘッダ
		/// </summary>
		public Hashtable LastHeader = new Hashtable();
		/// <summary>
		/// 最後のパラメータ(エラーしたときのみセット)
		/// </summary>
		public string LastParam = null;
		/// <summary>
		/// デフォルトMAX件数
		/// </summary>
		public int MaxCnt = 1000;
		/// <summary>
		/// ログを出すかどうか(エラーはフラグにかかわらず出す)
		/// </summary>
		public bool IsLogging = false;
		/// <summary>
		/// デバッグ用URLに変換するかどうか(iupload2->iupload2dev,isqlqry200->isqlqry200dev)
		/// </summary>
		public bool IsDebug = false;

		/// <summary>
		/// 内部的にデバッグ用URLに変換するかどうか(iupload2->iupload2dev,isqlqry200->isqlqry200dev)
		/// </summary>
		public bool IsDebugInner = false;

		#region AddAgent変更通知プロパティ(エージェント追加文字列)
		private string _AddAgent = "";
		/// <summary>
		/// AddAgent (エージェント追加文字列)
		/// </summary>
		public string AddAgent {
			get { return _AddAgent; }
			set {
				if (_AddAgent == value)
					return;
				_AddAgent = value;
			}
		}
		#endregion
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ClassHttp() {
		}
		static bool isEncLoad = false;
		/// <summary>
		/// 標準のコンストラクタ
		/// </summary>
		/// <param name="p_URLroot">aspxを呼ぶURL(例=https://ap01.co.jp/)</param>
		/// <param name="pAddUrl">追加URL(例=/isql/)</param>
		public ClassHttp(string p_URLroot, string pAddUrl = "/isql/") {
			v_URLroot = p_URLroot; // URLルートの設定
			v_AddUrl = pAddUrl;
			if (p_URLroot.Substring(p_URLroot.Length - 1) != "/") {
				v_URLroot += "/";
			}
			if (!isEncLoad) {
				Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
				isEncLoad = true;
			}
			v_defaultEncode = Encoding.GetEncoding("Shift_JIS");

		}
		/// <summary>
		/// ネットワーク、サーバが有効かどうかのチェック(正常は"OK")
		/// </summary>
		/// <param name="aspxname"></param>
		/// <returns></returns>
		public string CheckDefault(string aspxname = "default.aspx") {
			bool isNetOK = true; //System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
			if (!isNetOK) return "NG:ネットワーク通信エラー";
			string retStr = "";
			try {
				retStr = HttpGet(aspxname, 1, 10000);
			}
			catch (Exception ex) {
				return ex.Message;
			}
			if (WebError == null && WebError2 == null) return "OK";
			if (WebError != null) return WebError.Message;
			if (WebError2 != null) return WebError2.Message;
			else return retStr;
		}
		public bool IsCheckDefaultOk() {
			var ret = CheckDefault();
			return (ret == "OK");
		}
		private string getUserAgent() {
			return string.Format("CVnet-Client(WPF2025-05){0}", AddAgent);
		}
		/// <summary>
		/// ログインしているかどうかを簡易チェック
		/// </summary>
		/// <returns></returns>
		public bool IsLogin() {
			if (string.IsNullOrEmpty(RandID)) return false;
			if (string.IsNullOrEmpty(URLroot)) return false;
			return true;
		}
		/// <summary>
		/// エラーであればtrue (WebExceptionのみチェック)
		/// </summary>
		/// <returns></returns>
		public bool HasError() {
			if (WebError != null) return true;
			return false;
		}
		/// <summary>
		///  GETリクエストを発行する(aspxに対して)
		/// </summary>
		/// <param name="v_url">URLルートを除いた文字列 ex)isqlqry.aspx</param>
		/// <param name="v_flag">AddUrlをつけるかどうか(1:つける,0:つけない)</param>
		/// <param name="timeoutMilliseconds">タイムアウトまでのミリ秒数</param>
		/// <returns>レスポンスを格納した文字列</returns>
		public string HttpGet(string v_url, int v_flag, int timeoutMilliseconds) {
			WebError = null;
			WebError2 = null;
			// オレオレ証明対応
			if (System.Net.ServicePointManager.ServerCertificateValidationCallback == null) {
				System.Net.ServicePointManager.ServerCertificateValidationCallback +=
					new System.Net.Security.RemoteCertificateValidationCallback(OnRemoteCertificateValidationCallback);
			}
			string result = "";
			// リクエストの作成
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(getUrlString(v_URLroot + ((v_flag == 1) ? v_AddUrl : "") + v_url));
			req.Timeout = timeoutMilliseconds;
			req.UserAgent = getUserAgent();
			req.CookieContainer = v_cc;
			try {
				WebResponse res = req.GetResponse();
				// レスポンスの読み取り
				using (Stream resStream = res.GetResponseStream()) {
					using (StreamReader sr = new StreamReader(resStream, v_defaultEncode)) {
						result = sr.ReadToEnd();
					}
				}
			}
			catch (WebException ex) {
				req = null;
				WebError = ex;
				System.Diagnostics.Debug.WriteLine(ex.Message);
				var log = NLog.LogManager.GetCurrentClassLogger();
				if (v_url != "default.aspx")
					log.Error(ex,string.Format("HttpGetエラー Urlルート={0},AdUrl={1},HttpGet(v_url={2},v_flag={3}):", v_URLroot, v_AddUrl, v_url, v_flag));
			}
			catch (Exception ex) {
				req = null;
				WebError2 = ex;
				System.Diagnostics.Debug.WriteLine(ex.Message);
				var log = NLog.LogManager.GetCurrentClassLogger();
				log.Error(ex, string.Format("HttpGetエラー Urlルート={0},AdUrl={1},HttpGet(v_url={2},v_flag={3}):", v_URLroot, v_AddUrl, v_url, v_flag));
			}
			return result;
		}

		private string getUrlString(string inputString) {
			if (inputString.Length < 8) return inputString;
			string outString = inputString.Substring(0, 8) + inputString.Substring(8).Replace("//", "/");
			return outString;
		}

		/// <summary>
		///  POSTリクエストを発行する(CV.netサーバ用Shift_JISとUTF8)
		/// </summary>
		/// <param name="v_url">URLルートを除いた文字列 ex)isqlqry.aspx</param>
		/// <param name="v_vals">パラメータ</param>
		/// <param name="v_flag">AddUrlをつけるかどうか(1:つける,0:つけない)</param>
		/// <returns>レスポンスを格納した文字列</returns>
		public string HttpPost(string v_url, Hashtable v_vals, int v_flag, int timeout) {
			WebError = null;
			WebError2 = null;
			string param = "";
			int cnt = 0;
			foreach (string k in v_vals.Keys) {
				param += String.Format("{0}{1}={2}", ((cnt > 0) ? "&" : ""), k, v_vals[k]);
				cnt++;
			}
			byte[] data = Encoding.GetEncoding("Shift_JIS").GetBytes(param);
			// オレオレ証明対応
			if (System.Net.ServicePointManager.ServerCertificateValidationCallback == null) {
				System.Net.ServicePointManager.ServerCertificateValidationCallback +=
					new System.Net.Security.RemoteCertificateValidationCallback(OnRemoteCertificateValidationCallback);
			}
			// リクエストの作成
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(getUrlString(v_URLroot + ((v_flag == 1) ? v_AddUrl : "") + v_url));
			req.Method = "POST";
			req.ContentType = "application/x-www-form-urlencoded";
			req.ContentLength = data.Length;
			req.CookieContainer = v_cc;
			req.UserAgent = getUserAgent();
			req.Timeout = timeout;
			string result = "";
			var log = NLog.LogManager.GetCurrentClassLogger();
			try {
				// ポスト・データの書き込み
				Stream reqStream = req.GetRequestStream();
				reqStream.Write(data, 0, data.Length);
				reqStream.Close();
				WebResponse res = req.GetResponse();
				LastHeader.Clear();
				foreach (string keys in res.Headers.Keys) {
					if (keys == "CRS-COLLIST") {
						byte[] wrkbytes0 = res.Headers.ToByteArray();
						string str0 = Encoding.UTF8.GetString(wrkbytes0);
						// string[] str1 = str0.Split('\n', '\r');
						int find0 = str0.IndexOf("CRS-COLLIST: ");
						string wrk1 = str0.Substring(find0 + "CRS-COLLIST: ".Length);
						int find1 = wrk1.IndexOf('\r');
						wrk1 = wrk1.Substring(0, find1);
						LastHeader.Add(keys, wrk1);
					}
					else {
						LastHeader.Add(keys, res.Headers[keys]);
					}
				}
				using (Stream resStream = res.GetResponseStream()) {
					if (true /* res.ContentLength > 0 */) {
						using (StreamReader sr = new StreamReader(resStream, DefaultEncode)) {
							result = sr.ReadToEnd();
						}
					}
				}
				//System.Diagnostics.Debug.WriteLine("HttpPostのヘッダ情報(UTF-8)==" + req.RequestUri.ToString());
				byte[] wrkbytes = res.Headers.ToByteArray();
				System.Diagnostics.Debug.WriteLine(Encoding.UTF8.GetString(wrkbytes));
				res.Close();
				res = null;
				req = null;
				//foreach (string strkey in res.Headers.Keys) {}
			}
			catch (WebException ex) {
				req = null;
				WebError = ex;
				LastParam = param;
				System.Diagnostics.Debug.WriteLine(ex.Message);
				var qs = (v_vals["qs"] != null) ? (",qs=" + v_vals["qs"].ToString()) : "";
				log.Error(ex, string.Format("HttpPostエラー qs={0},HttpPost(v_url={1}):prm={2}", qs, v_url, (param.Length > 30) ? param.Substring(0, 30) : param));
			}
			catch (Exception ex) {
				req = null;
				WebError2 = ex;
				System.Diagnostics.Debug.WriteLine(ex.Message);
				var qs = (v_vals["qs"] != null) ? (",qs=" + v_vals["qs"].ToString()) : "";
				log.Error(ex, string.Format("HttpPostエラー qs={0},HttpPost(v_url={1}):prm={2}", qs, v_url, (param.Length > 30) ? param.Substring(0, 30) : param));
			}
			return result;
		}
		/// <summary>
		///  POSTリクエストを発行する(CV.netサーバ用Shift_JISとUTF8)
		/// </summary>
		/// <param name="v_url">URLルートを除いた文字列 ex)isqlqry.aspx</param>
		/// <param name="v_vals">パラメータ</param>
		/// <param name="v_flag">AddUrlをつけるかどうか(1:つける,0:つけない)</param>
		/// <returns>レスポンスを格納した文字列</returns>
		public string HttpPost(string v_url, Hashtable v_vals, int v_flag) {
			if (IsDebug) {
				if (v_url == "iupload2.aspx")
					v_url = "iupload2dev.aspx";
				if (v_url == "user/isqlqry200.aspx")
					v_url = "user/isqlqry200dev.aspx";
			}
			if (IsDebugInner) {
				if (v_url == "iupload2.aspx")
					v_url = "iupload2dev.aspx";
				if (v_url == "user/isqlqry200.aspx")
					v_url = "user/isqlqry200dev.aspx";
			}
			return HttpPost(v_url, v_vals, v_flag, 150000); // 150sec
		}

		/// <summary>
		///  POSTリクエストを発行する(CV.netサーバ用Shift_JISとUTF8)
		/// </summary>
		/// <param name="v_url">URLルートを除いた文字列 ex)isqlqry.aspx</param>
		/// <param name="v_vals">パラメータ</param>
		/// <returns>レスポンスを格納した文字列</returns>
		public DataTable HttpPost(string v_url, Hashtable v_vals) {
			DataTable retTable = new DataTable();
			WebError = null;
			WebError2 = null;
			string param = "";
			int cnt = 0;
			foreach (string k in v_vals.Keys) {
				param += String.Format("{0}{1}={2}", ((cnt > 0) ? "&" : ""), k, v_vals[k]);
				cnt++;
			}
			byte[] data = Encoding.GetEncoding("Shift_JIS").GetBytes(param);
			// オレオレ証明対応
			if (System.Net.ServicePointManager.ServerCertificateValidationCallback == null) {
				System.Net.ServicePointManager.ServerCertificateValidationCallback +=
					new System.Net.Security.RemoteCertificateValidationCallback(OnRemoteCertificateValidationCallback);
			}
			// リクエストの作成
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(getUrlString(v_URLroot + v_AddUrl + v_url));
			req.Method = "POST";
			req.ContentType = "application/x-www-form-urlencoded";
			req.ContentLength = data.Length;
			req.CookieContainer = v_cc;
			req.UserAgent = getUserAgent();
			req.Timeout = 180000 * 4;
			string buffers = "";
			var log = NLog.LogManager.GetCurrentClassLogger();
			try {
				// ポスト・データの書き込み
				Stream reqStream = req.GetRequestStream();
				reqStream.Write(data, 0, data.Length);
				reqStream.Close();
				WebResponse res = req.GetResponse();
				LastHeader.Clear();
				foreach (string keys in res.Headers.Keys) {
					if (keys == "CRS-COLLIST") {
						byte[] wrkbytes0 = res.Headers.ToByteArray();
						string str0 = Encoding.UTF8.GetString(wrkbytes0);
						// string[] str1 = str0.Split('\n', '\r');
						int find0 = str0.IndexOf("CRS-COLLIST: ");
						string wrk1 = str0.Substring(find0 + "CRS-COLLIST: ".Length);
						int find1 = wrk1.IndexOf('\r');
						wrk1 = wrk1.Substring(0, find1);
						LastHeader.Add(keys, wrk1);
					}
					else {
						LastHeader.Add(keys, res.Headers[keys]);
					}
				}
				using (Stream resStream = res.GetResponseStream()) {
					if (res.ContentLength > 5 || res.ContentLength < 0) {
						using (StreamReader sr = new StreamReader(resStream, Encoding.UTF8)) {
							retTable.ReadXml(sr);
						}
					}
				}
				// System.Diagnostics.Debug.WriteLine("HttpPostのヘッダ情報(UTF-8)==" + req.RequestUri.ToString());
				byte[] wrkbytes = res.Headers.ToByteArray();
				System.Diagnostics.Debug.WriteLine(Encoding.UTF8.GetString(wrkbytes));
				res.Close();
				res = null;
				req = null;
			}
			catch (WebException ex) {
				req = null;
				WebError = ex;
				LastParam = param;
				System.Diagnostics.Debug.WriteLine(ex.Message);
				System.Diagnostics.Debug.WriteLine("エラー時のResponse内容");
				System.Diagnostics.Debug.WriteLine(buffers);
				var qs = (v_vals["qs"] != null) ? (",qs=" + v_vals["qs"].ToString()) : "";
				log.Error(ex, string.Format("HttpPostエラー qs={0},HttpPost(v_url={1}):prm={2}", qs, v_url, (param.Length > 30) ? param.Substring(0, 30) : param));
			}
			catch (System.Xml.XmlException ex) { // XML解釈エラーは無視する
				req = null;
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
			catch (Exception ex) {
				req = null;
				WebError2 = ex;
				System.Diagnostics.Debug.WriteLine(ex.Message);
				System.Diagnostics.Debug.WriteLine("エラー時のResponse内容");
				System.Diagnostics.Debug.WriteLine(buffers);
				var qs = (v_vals["qs"] != null) ? (",qs=" + v_vals["qs"].ToString()) : "";
				log.Error(ex, string.Format("HttpPostエラー qs={0},HttpPost(v_url={1}):prm={2}", qs, v_url, (param.Length > 30) ? param.Substring(0, 30) : param));
			}
			return retTable;
		}
		/// <summary>
		/// クライアント証明書つきのHttpPost
		/// </summary>
		/// <param name="v_url"></param>
		/// <param name="v_body"></param>
		/// <param name="clientCertificatePath"></param>
		/// <param name="clientCertificatePassword"></param>
		/// <param name="retStr"></param>
		/// <returns></returns>
		public int HttpPost(string v_url, string v_body, string clientCertificatePath, string clientCertificatePassword, out string retStr) {
			WebError = null;
			WebError2 = null;
			retStr = "";
			// オレオレ証明対応
			if (System.Net.ServicePointManager.ServerCertificateValidationCallback == null) {
				System.Net.ServicePointManager.ServerCertificateValidationCallback +=
					new System.Net.Security.RemoteCertificateValidationCallback(OnRemoteCertificateValidationCallback);
			}
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(getUrlString(v_URLroot + v_AddUrl + v_url));
			req.Method = "POST";
			req.ContentType = "application/json";
			var x509 = CreateX509(clientCertificatePath, clientCertificatePassword);
			req.ClientCertificates.Add(x509); // クライアント証明書
			req.UserAgent = getUserAgent();
			var log = NLog.LogManager.GetCurrentClassLogger();
			try {
				using (var streamWriter = new StreamWriter(req.GetRequestStream())) {
					streamWriter.Write(v_body);
				}
				var res = (System.Net.HttpWebResponse)req.GetResponse();
				Stream s = res.GetResponseStream();
				StreamReader sr = new StreamReader(s);
				retStr = sr.ReadToEnd();
				log.Info(string.Format("HttpPost成功 url={0} ,body={0} ,res={1}", v_url, v_body, v_url, retStr));
			}
			catch (WebException ex) {
				WebError = ex;
				var resErr = (System.Net.HttpWebResponse)ex.Response;
				string exRes = null;
				if (resErr != null)
					exRes = new StreamReader(resErr.GetResponseStream()).ReadToEnd();
				retStr = exRes;
				log.Error(ex, string.Format("HttpPostエラー url={0} ,body={0} ,res={1}", v_url, v_body, exRes));
				return -1;
			}
			catch (Exception ex) {
				WebError2 = ex;
				log.Error(ex, string.Format("HttpPostエラー url={0} ,body={0}", v_url, v_body));
				return -2;
			}
			return 0;
		}
		private static bool OnRemoteCertificateValidationCallback(Object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
			System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) {
			// オレオレ証明を信用したことにする
			if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None) {
				return true;
			}
			if (sender is System.Net.WebRequest) {
				return true;
			}
			return false;
		}
		/// <summary>
		/// CSV形式の文字列からDataTableを作成する
		/// </summary>
		/// <param name="v_res">CSV形式の文字列</param>
		/// <returns>結果のDataTable</returns>
		public DataTable ConvStr2Table(string v_res) {
			DataTable wrk_tb = new DataTable();
			string[] wrk_rec = v_res.Split('\n');
			string[] wrk_rec0 = null, wrk_rec1 = null;
			DataRow wrk_row = null;
			if (wrk_rec.Length > 0) {
				wrk_rec0 = wrk_rec[0].Split(',');
				for (int i = 0; i < wrk_rec0.Length; i++) {
					string col_name = wrk_rec0[i];
					if (col_name.Length > 2) {
						if (col_name.Substring(0, 1) == "\"" && col_name.Substring(col_name.Length - 1, 1) == "\"") {
							col_name = col_name.Substring(1, col_name.Length - 2);
						}
					}
					wrk_tb.Columns.Add(col_name);
				}
				for (int i = 1; i < wrk_rec.Length; i++) {
					wrk_row = wrk_tb.NewRow();
					wrk_rec1 = wrk_rec[i].Split(',');
					if (wrk_rec1.Length != wrk_rec0.Length) break;
					for (int j = 0; j < wrk_rec0.Length; j++) {
						wrk_row[j] = wrk_rec1[j];
						int length0 = wrk_rec1[j].Length;
						if (length0 > 2) {
							if (wrk_rec1[j].Substring(0, 1) == "\"" && wrk_rec1[j].Substring(length0 - 1, 1) == "\"") {
								wrk_row[j] = wrk_rec1[j].Substring(1, length0 - 2);
							}
						}
					}
					wrk_tb.Rows.Add(wrk_row);
				}
			}
			return wrk_tb;
		}
		/// <summary>
		/// LOGIN処理を実行する
		/// </summary>
		/// <param name="v_kubun">区分</param>
		/// <param name="v_id">ID</param>
		/// <param name="v_pass">PASS</param>
		/// <returns>戻り文字列</returns>
		public int Login(int v_kubun, string v_id, string v_pass) {
			if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
				return -638; // ネットワークエラー(net)
			Hashtable vals = new Hashtable();
			int ret_code = -1;
			vals["p_id"] = v_id;
			vals["p_pass"] = System.Web.HttpUtility.UrlEncode(v_pass); // 2021/11/09 パスワード内の特殊文字に対応
			vals["p_kubun"] = v_kubun.ToString();
			vals["cvnet"] = "30";
			CookieContainer save_cc = v_cc;
			v_cc = new CookieContainer();
			DataTable wrk_ret = null;
			try {
				wrk_ret = HttpPost("login.aspx", vals);
			}
			catch (Exception) {
				return -640; // リモート名が解決できないetc System.Web.Exception
			}
			v_infologin = wrk_ret;
			if (v_infologin.Rows.Count > 0) {
				v_randid = v_infologin.Rows[0][0].ToString();
				v_loginid = v_id;
				ret_code = 0;
				return ret_code;
			}
			v_cc = save_cc;
			return ret_code;
		}
		/* MEMO RADOM
		 * AspxGetImage(image_str) : 未使用
		 * 
		 */
		/// <summary>
		/// .NETサーバーにSQL問い合わせを行う(classsatoo.car互換)
		/// </summary>
		/// <param name="sqlstr_cmd">SQL文 (トークン"__DATA__"は実際のサーバーパスに変換される：画像img用)</param>
		/// <param name="p_param">パラメータのリスト</param>
		/// <param name="p_form">PrintStreamフォーム名(ex. nouhin.qfm)</param>
		/// <param name="AspxUserFlg">UserFlg</param>
		/// <returns></returns>
		public DataTable AspxSqlQuery(string sqlstr_cmd, string[] p_param, string p_form = null, int AspxUserFlg = -1) {
			Hashtable vals = new Hashtable();
			vals.Add("rd", v_randid);
			vals.Add("qs", sqlstr_cmd);
			vals.Add("xml", "123");
			string aspxfile = "isqlqry.aspx";
			if (p_form != null) {
				aspxfile = "isqlqfw.aspx";
				if (AspxUserFlg > 0) {
					aspxfile = "user/isqlqfw" + AspxUserFlg.ToString("00") + ".aspx";
				}
				vals["fm"] = p_form;
			}
			if (p_param != null && p_param.Length > 0) {
				for (int i = 0; i < p_param.Length; i++) {
					string keyname = "p" + (i + 1).ToString();
					vals[keyname] = p_param[i];
				}
			}
			// DEBUGメッセージ
			if (IsLogging) {
				var log = NLog.LogManager.GetCurrentClassLogger();
				log.Debug(string.Format("SQL問合せ:sql=({0})(p={1}) [{2}]", sqlstr_cmd, string.Join(",",p_param), aspxfile));
			}
			return HttpPost(aspxfile, vals);

		}
		public string AspxSqlQueryCsv(string sqlstr_cmd, string[] p_param, string p_form = null, int AspxUserFlg = -1) {
			Hashtable vals = new Hashtable();
			vals.Add("rd", v_randid);
			vals.Add("qs", sqlstr_cmd);
			string aspxfile = "isqlqry.aspx";
			if (p_form != null) {
				aspxfile = "isqlqfw.aspx";
				if (AspxUserFlg > 0) {
					aspxfile = "user/isqlqfw" + AspxUserFlg.ToString("00") + ".aspx";
				}
				vals["fm"] = p_form;
			}
			if (p_param != null && p_param.Length > 0) {
				for (int i = 0; i < p_param.Length; i++) {
					string keyname = "p" + (i + 1).ToString();
					vals[keyname] = p_param[i];
				}
			}
			// DEBUGメッセージ
			if (IsLogging) {
				var log = NLog.LogManager.GetCurrentClassLogger();
				log.Debug(string.Format("SQL問合せ:sql=({0})(p={1}) [{2}]", sqlstr_cmd, string.Join(",",p_param), aspxfile));
			}
			return HttpPost(aspxfile, vals, 1);
		}
		/// <summary>
		/// .NETサーバーに決まったSQL問い合わせを行う(classsatoo.car互換)
		/// </summary>
		/// <param name="sqlstr_cmd">SQL文 (トークン"__DATA__"は実際のサーバーパスに変換される：画像img用)</param>
		/// <param name="p_param">パラメータのリスト</param>
		/// <param name="p_form">PrintStreamフォーム名(ex. nouhin.qfm)</param>
		/// <param name="AspxUserFlg">UserFlg(-1が通常、0以上が番号つき)</param>
		/// <returns></returns>
		public string AspxSqlQuery2(string sqlstr_cmd, string[] p_param, string p_form, int AspxUserFlg, string yobi0 = "") {
			Hashtable vals = new Hashtable();
			vals["rd"] = v_randid;
			vals["qs"] = sqlstr_cmd;
			vals["xml"] = "123";
			if (!string.IsNullOrEmpty(yobi0))
				vals["yobi0"] = yobi0;
			string aspxfile = "isqlqry2.aspx";
			if (AspxUserFlg >= 0) {
				aspxfile = "user/isqlqry2" + AspxUserFlg.ToString("00") + ".aspx";
			}
			vals["fm"] = p_form;
			if (p_param != null && p_param.Length > 0) {
				for (int i = 0; i < p_param.Length; i++) {
					string keyname = "p" + (i + 1).ToString();
					vals[keyname] = p_param[i];
				}
			}
			// DEBUGメッセージ
			if (IsLogging) {
				var log = NLog.LogManager.GetCurrentClassLogger();
				log.Debug(string.Format("SQL問合せ2:sql=({0})(p={1}) [{2}]", sqlstr_cmd, string.Join(",",p_param), aspxfile));
			}
			return HttpPost(aspxfile, vals, 1);
		}
		/// <summary>
		/// .NETサーバーに決まったSQL問い合わせを行う(戻り値 DataTable)
		/// </summary>
		/// <param name="sqlstr_cmd"></param>
		/// <param name="p_param"></param>
		/// <param name="p_form"></param>
		/// <param name="AspxUserFlg"></param>
		/// <returns></returns>
		/// <remarks>[Obsolete("別の処理へ 戻り値DataTableは使用しない")]</remarks>
		public DataTable AspxSqlQuery2dt(string sqlstr_cmd, string[] p_param, string p_form, int AspxUserFlg) {
			Hashtable vals = new Hashtable();
			vals["rd"] = v_randid;
			vals["qs"] = sqlstr_cmd;
			vals["xml"] = "123";
			string aspxfile = "isqlqry2.aspx";
			if (AspxUserFlg >= 0) {
				aspxfile = "user/isqlqry2" + AspxUserFlg.ToString("00") + ".aspx";
			}
			vals["fm"] = p_form;
			if (p_param != null && p_param.Length > 0) {
				for (int i = 0; i < p_param.Length; i++) {
					string keyname = "p" + (i + 1).ToString();
					vals[keyname] = p_param[i];
				}
			}
			// DEBUGメッセージ
			if (IsLogging) {
				var log = NLog.LogManager.GetCurrentClassLogger();
				log.Debug(string.Format("SQL問合せ2:sql=({0})(p={1}) [{2}]", sqlstr_cmd, string.Join(",", p_param), aspxfile));
			}
			return HttpPost(aspxfile, vals);
		}
		/// <summary>
		/// .NETサーバーに決まったSystem系SQL問い合わせを行う(classsatoo.car互換)
		/// </summary>
		/// <returns></returns>
		public DataTable AspxSqlQuery3() {
			string[] p_param = null;
			string sqlstr_cmd = "shain";
			Hashtable vals = new Hashtable();
			vals["rd"] = v_randid;
			vals["qs"] = sqlstr_cmd;
			vals["qs2"] = "90000";
			vals["p1"] = "";
			vals["p2"] = @"select 社員CD from HC$master_shain where 社員CD between '.' and 'ZZZZZZZZ'";
			vals["xml"] = "123";
			string aspxfile = "isqlqry3.aspx";
			if (p_param != null && p_param.Length > 0) {
				for (int i = 0; i < p_param.Length; i++) {
					string keyname = "p" + (i + 1).ToString();
					vals[keyname] = p_param[i];
				}
			}
			// DEBUGメッセージ
			if (IsLogging) {
				var log = NLog.LogManager.GetCurrentClassLogger();
				log.Debug(string.Format("SQL問合せ2:sql=({0})(p={1}) [{2}]", sqlstr_cmd, string.Join(",", p_param), aspxfile));
			}
			return HttpPost(aspxfile, vals);
		}
		/// <summary>
		/// SQL文の実行を行う
		/// </summary>
		/// <param name="p_md">[コマンド] 'I'insert,'U'update,'D'delete,'L'lock,'N'unlock</param>
		/// <param name="p_tb">[テーブル名] UserTableIdを抜いた表の名前</param>
		/// <param name="p_no">対象行のSEQ_NO</param>
		/// <param name="p_vd">仮想日付</param>
		/// <param name="p_clist">項目名のリスト</param>
		/// <param name="p_dlist">項目値のリスト</param>
		/// <returns></returns>
		public CvRetAspx AspxSqlExe(DBDef.DB_DML p_md, string p_tb, long p_no, string p_vd, string[] p_clist, string[] p_dlist) {
			DataTable retTable = AspxSqlExeTable(p_md, p_tb, p_no, p_vd, p_clist, p_dlist);
			CvRetAspx retCode = new CvRetAspx();
			retCode.Code = -100;
			retCode.Opt1 = "";
			retCode.Opt2 = "";
			retCode.TableName = "";
			if (retTable.Rows.Count > 0) {
				retCode.Code = (int)retTable.Rows[0]["ret_code"];
				if (!(retTable.Rows[0]["v_out_no"] is System.DBNull)) {
					if (retTable.Rows[0]["v_out_no"] is System.Int32) {
						retCode.NewSeq = (int)retTable.Rows[0]["v_out_no"];
					}
					else {
						retCode.NewSeq = (long)retTable.Rows[0]["v_out_no"];
					}
				}
				if (!(retTable.Rows[0]["v_out_vdate"] is System.DBNull)) {
					retCode.VDate = retTable.Rows[0]["v_out_vdate"].ToString();
				}
				if (!(retTable.Rows[0]["v_tb"] is System.DBNull)) {
					retCode.TableName = (string)retTable.Rows[0]["v_tb"];
				}
				if (!(retTable.Rows[0]["v_mod_seq"] is System.DBNull)) {
					retCode.Opt1 = retTable.Rows[0]["v_mod_seq"].ToString();
				}
				if (!(retTable.Rows[0]["err_pos"] is System.DBNull)) {
					retCode.Opt2 = (string)retTable.Rows[0]["err_pos"];
				}
			}
			// DEBUGメッセージ
			if (IsLogging) {
				var log = NLog.LogManager.GetCurrentClassLogger();
				log.Debug(string.Format("SQL実行:{0},Tbl={1},Seq={2},Vdate={3},col={4}, ret={5}", p_md.ToString(), p_tb, p_no, p_vd, string.Join(",", p_clist), retCode.Code));
			}
			return retCode;
		}
		/// <summary>
		/// SQL文の実行を行う
		/// </summary>
		/// <param name="p_md">[コマンド] 'I'insert,'U'update,'D'delete,'L'lock,'N'unlock</param>
		/// <param name="p_tb">[テーブル名] UserTableIdを抜いた表の名前</param>
		/// <param name="p_no">対象行のSEQ_NO</param>
		/// <param name="p_vd">仮想日付</param>
		/// <param name="p_clist">項目名のリスト</param>
		/// <param name="p_dlist">項目値のリスト</param>
		/// <returns></returns>
		public DataTable AspxSqlExeTable(DBDef.DB_DML p_md, string p_tb, long p_no, string p_vd, string[] p_clist, string[] p_dlist) {
			Hashtable vals = new Hashtable();
			string aspxfile = "isqlexe.aspx";
			vals["rd"] = v_randid;
			vals["tb"] = p_tb;
			vals["no"] = p_no.ToString();
			vals["vd"] = p_vd.ToString();
			vals["xml"] = "123";
			if (p_md == DBDef.DB_DML.INSERT || p_md == DBDef.DB_DML.UPDATE || p_md == DBDef.DB_DML.DELETE
				|| p_md == DBDef.DB_DML.LOCK) {
				vals["md"] = p_md.ToString().Substring(0, 1);
			}
			else if (p_md == DBDef.DB_DML.UNLOCK) {
				vals["md"] = "N";
			}
			if (v_shainCD != "") {
				vals["shain"] = v_shainCD;
			}
			if (p_clist != null && p_clist.Length > 0) {
				int max_cnt = (p_clist.Length <= p_dlist.Length) ? p_clist.Length : p_dlist.Length;
				for (int i = 0; i < max_cnt; i++) {
					vals["c" + i.ToString()] = p_clist[i];
					vals["d" + i.ToString()] = (p_dlist[i] == "") ? "." : p_dlist[i];
				}
			}
			return HttpPost(aspxfile, vals);
		}
		/// <summary>
		/// 最大取得件数(default1000)を制限したSQL文を返す
		/// </summary>
		/// <param name="sqlstr"></param>
		/// <returns></returns>
		public string GetSqlStringUsingMax(string sqlstr) {
			return ("select * from (" + sqlstr + ") where rownum<=" + MaxCnt.ToString());
		}
#if false
		private static bool ColumnEqual(object A, object B) {
			if (A == DBNull.Value && B == DBNull.Value) //  both are DBNull.Value
				return true;
			if (A == DBNull.Value || B == DBNull.Value) //  only one is DBNull.Value
				return false;
			return (A.Equals(B));  // value type standard comparison
		}
#endif
		/// <summary>
		///	サーバ上のイメージDirにあるイメージファイルを取得する
		/// </summary>
		/// <param name="imagename"></param>
		/// <returns></returns>
		public Bitmap GetImageFile(string imagename) {
			string url = URLroot + "/Data/img/" + imagename;
			Bitmap bitmap = null;
			WebClient wc = new WebClient();
			try {
				Stream stream = wc.OpenRead(url);
				bitmap = new Bitmap(stream);
				stream.Close();
			}
			catch (Exception) {
			}
			return bitmap;
		}
		/// <summary>
		/// JSON文字列(UTF8)をアップロードする(圧縮無し)
		/// </summary>
		/// <param name="jsondata"></param>
		/// <param name="remotename"></param>
		/// <param name="mess"></param>
		/// <returns></returns>
		public bool UploadJsonData(string jsondata, string remotename, out string mess) {
			mess = "";
			Hashtable vals = new Hashtable();
			string base64String = "";
			bool retval = true;
			try {
				byte[] bs = Encoding.UTF8.GetBytes(jsondata);
				base64String = Convert.ToBase64String(bs, System.Base64FormattingOptions.InsertLineBreaks);
				base64String = System.Web.HttpUtility.UrlEncode(base64String); // 要System.Web.DLL
				vals.Add("fname", remotename);
				vals.Add("file", base64String);
				vals.Add("DIR", "cvpos");
				string retStr = HttpPost("iupload2.aspx", vals, 1);
				if (retStr.ToUpper().IndexOf("OK") < 0) retval = false;
			}
			catch (System.Exception ex) {
				mess = ex.Message;
				retval = false;
			}
			return retval;
		}
		/// <summary>
		/// JSONファイル(UTF8)をアップロードする(圧縮無し)
		/// </summary>
		/// <param name="localname"></param>
		/// <param name="remotename"></param>
		/// <param name="mess"></param>
		/// <returns></returns>
		public bool UploadFile(string localname, string remotename, out string mess) {
			mess = "";
			Hashtable vals = new Hashtable();
			string base64String = "";
			bool retval = true;
			try {
				using (var rw = new StreamReader(localname, Encoding.UTF8)) {
					string fstr = rw.ReadToEnd();
					rw.Close();
					byte[] bs = Encoding.UTF8.GetBytes(fstr);
					base64String = Convert.ToBase64String(bs, System.Base64FormattingOptions.InsertLineBreaks);
					base64String = System.Web.HttpUtility.UrlEncode(base64String); // 要System.Web.DLL
				}
				vals.Add("fname", remotename);
				vals.Add("file", base64String);
				vals.Add("DIR", "cvpos");
				string retStr = HttpPost("iupload2.aspx", vals, 1);
				if (retStr.ToUpper().IndexOf("OK") < 0) retval = false;
			}
			catch (System.Exception ex) {
				mess = ex.Message;
				retval = false;
			}
			return retval;
		}
		/// <summary>
		/// JSONファイル(UTF8)をアップロードする(圧縮あり)
		/// </summary>
		/// <param name="http"></param>
		/// <param name="localname"></param>
		/// <param name="remotename"></param>
		/// <param name="mess"></param>
		/// <returns></returns>
		public bool UploadFileDeflate(string localname, string remotename, out string mess) {
			mess = "";
			var vals = new Hashtable();
			string base64String = "";
			bool retval = true;
			try {
				using (var rw = new StreamReader(localname, Encoding.UTF8)) {
					string fstr = rw.ReadToEnd();
					rw.Close();
					byte[] bs = Encoding.UTF8.GetBytes(fstr);
					// 圧縮ルーチン
					byte[] compressed = convertByteCompress(bs);
					base64String = System.Convert.ToBase64String(compressed, System.Base64FormattingOptions.InsertLineBreaks);
					base64String = System.Web.HttpUtility.UrlEncode(base64String); // 要System.Web.DLL
				}
				vals.Add("fname", remotename);
				vals.Add("file", base64String);
				vals.Add("DIR", "cvpos");
				vals.Add("compress", "Deflate"); // 圧縮ありフラグ
				string retStr = HttpPost("iupload2.aspx", vals, 1);
				if (retStr.ToUpper().IndexOf("OK") < 0) retval = false;
			}
			catch (System.Exception ex) {
				mess = ex.Message;
				retval = false;
			}
			return retval;
		}
		public bool UploadZip(byte[] bs, string remotename, out string mess) {
			mess = "";
			Hashtable vals = new Hashtable();
			string base64String = "";
			bool retval = true;
			try {
				base64String = Convert.ToBase64String(bs, System.Base64FormattingOptions.InsertLineBreaks);
				base64String = System.Web.HttpUtility.UrlEncode(base64String); // 要System.Web.DLL
				vals.Add("fname", remotename);
				vals.Add("file", base64String);
				vals.Add("DIR", "cvpos");
				string retStr = HttpPost("iupload2.aspx", vals, 1);
				if (retStr.ToUpper().IndexOf("OK") < 0) retval = false;
			}
			catch (System.Exception ex) {
				mess = ex.Message;
				retval = false;
			}
			return retval;
		}

		#region 圧縮、解凍系ロジック
		/// <summary>
		/// バイト配列を圧縮して返す
		/// </summary>
		/// <param name="inbyte"></param>
		/// <returns></returns>
		public static byte[] convertByteCompress(byte[] inbyte) {
			var ms = new MemoryStream();
			var CompressedStream = new System.IO.Compression.DeflateStream(ms,
				System.IO.Compression.CompressionMode.Compress, true);
			// ストリームに圧縮するデータを書き込みます
			CompressedStream.Write(inbyte, 0, inbyte.Length);
			CompressedStream.Close();
			// 圧縮されたデータを バイト配列で取得します
			byte[] destination = ms.ToArray();
			// System.Diagnostics.Debug.WriteLine(string.Format("XML Data 元バイト数={0} 圧縮後={1}", inbyte.Length, destination.Length));
			ms.Close();
			return destination;
		}
		/// <summary>
		/// streamを解凍したバイト配列を返す
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static byte[] convertByteDeCompress(System.IO.Stream stream) {
			var CompressedStream = new System.IO.Compression.DeflateStream(stream,
				System.IO.Compression.CompressionMode.Decompress);
			var msOutput = new System.IO.MemoryStream();
			while (true) {
				int rb = CompressedStream.ReadByte();
				if (rb == -1) break;
				msOutput.WriteByte((byte)rb);
			}
			byte[] destination = msOutput.ToArray();
			CompressedStream.Close();
			msOutput.Close();
			return destination;
		}
		/// <summary>
		/// バイト配列をBase64エンコードする
		/// </summary>
		/// <param name="inByte"></param>
		/// <returns></returns>
		public static string convertBase64(byte[] inByte) {
			string base64String = System.Convert.ToBase64String(inByte, System.Base64FormattingOptions.InsertLineBreaks);
			base64String = System.Web.HttpUtility.UrlEncode(base64String); // 要System.Web.DLL
			return base64String;
		}
		/// <summary>
		/// 圧縮されBase64エンコードされたUTF8文字列データ(改行つき)をデコード＆解凍したバイト列に変換する
		/// </summary>
		/// <param name="indata"></param>
		/// <returns></returns>
		public static byte[] convertBase64Compressed(string indata) {
			byte[] dataBytes = Encoding.UTF8.GetBytes(indata);
			var inputstream = new MemoryStream(dataBytes);
			long maxbufferlength = inputstream.Length * 3 / 4;
			byte[] v_file = new byte[maxbufferlength];
			byte[] data = new byte[78 * 1024]; //chunk size is 4k
			int read = inputstream.Read(data, 0, data.Length);
			byte[] chunk = new byte[57 * 1024]; // 76+CRLF -> 57
			int nowposition = 0;
			while (read > 0) {
				chunk = System.Convert.FromBase64String(Encoding.ASCII.GetString(data, 0, read));
				System.Array.Copy(chunk, 0, v_file, nowposition, chunk.Length);
				nowposition += chunk.Length;
				read = inputstream.Read(data, 0, data.Length);
			}
			inputstream.Close();
			var dataCompressed = new MemoryStream(v_file);
			byte[] outputBytes = convertByteDeCompress(dataCompressed);
			dataCompressed.Close();
			inputstream.Dispose();
			dataCompressed.Dispose();
			return outputBytes;
		}
		#endregion
		/// <summary>
		/// X509クライアント証明書の読み込み
		/// </summary>
		/// <param name="filename">クライアント証明書の場所</param>
		/// <param name="password">パスワード</param>
		/// <returns></returns>
		static public System.Security.Cryptography.X509Certificates.X509Certificate2 CreateX509(string filename, string password) {
			byte[] rowData;
			using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read)) {
				rowData = new byte[(int)stream.Length];
				stream.Read(rowData, 0, rowData.Length);
			}
			return new System.Security.Cryptography.X509Certificates.X509Certificate2(rowData, password,
				System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.MachineKeySet);
		}
	}
	/// <summary>
	/// DB定義用Class (string[,] DB定義配列を扱う)
	/// </summary>
	public class DBDef {
		public DBDef() {
			/*
			CREATE USER "QS_XX"  PROFILE "DEFAULT" 
			IDENTIFIED BY "QS_XX" DEFAULT TABLESPACE "USER02" QUOTA 100 M 
			ON "USER02" ACCOUNT UNLOCK; GRANT "CONNECT" TO "QS_XX";
			// ユーザ権限は、CONNECTと、デフォルト表領域に対する割当許可のみ
			// (TEMP領域が必要な場合は割当追加)
			*/
		}
		/// <summary>
		/// DB定義用：文字型初期値1文字string
		/// </summary>
		public const string Default_A1 = ".";
		/// <summary>
		/// DBのSQL簡易実行用：DMLコマンドのタイプ
		/// </summary>
		public enum DB_DML {
			/// <summary>
			/// 問い合わせ用(別ルーチン)
			/// </summary>
			SELECT,
			/// <summary>
			/// 行の追加
			/// </summary>
			INSERT,
			/// <summary>
			/// 行の修正
			/// </summary>
			UPDATE,
			/// <summary>
			/// 行の削除
			/// </summary>
			DELETE,
			/// <summary>
			/// 明示的なロック
			/// </summary>
			LOCK,
			/// <summary>
			/// 明示的なロック解除
			/// </summary>
			UNLOCK
		}
	}
}

#pragma warning restore CS8600
#pragma warning restore CS8601
#pragma warning restore CS8602
#pragma warning restore CS8603
#pragma warning restore CS8604
#pragma warning restore CS8622
#pragma warning restore CS8625
#pragma warning restore SYSLIB0014
#pragma warning restore SYSLIB0057
#pragma warning restore CA1416
#pragma warning restore CA2022
