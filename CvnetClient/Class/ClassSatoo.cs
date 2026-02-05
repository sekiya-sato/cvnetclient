using CvnetClient.Models;
using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
public class ClassSatoo
{
    private static readonly HttpClient httpClient = new HttpClient();
    public int SysTest = 0; /* デバッグ用ﾃｽﾄﾌﾗｸﾞ .*/
    public int ImgLoad = 1; /* 一覧表示Imageﾌﾗｸﾞ(0:無,1:有,2:バックグラウンド)  .*/
    public string Version = "20060404"; /* Version .*/
    public string AspxPath = string.Empty; /* aspxのパス(例=http://www.etc.co.jp/cv.net/あるいは/cv.net/) .*/
    public string AspxRandId = "";	/* Session変数 .*/
    public LoginHeader LoginHeader = new LoginHeader(); /* 最終ログイン試行時のヘッダー情報 .*/
    public ResHeader ResHeader = new ResHeader(); /* Query試行時のヘッダー情報 .*/

    public int LoginKubun = 0;	/* 入力されたLOGIN区分 .*/
    public string LoginId = ""; /* 入力されたLOGIN ID .*/
    public string LoginSEC = ""; /* 取得したSEC .*/
    public string LoginMG_FLG = ""; /* 取得したMG_FLG .*/
    public string SHAIN_CD = ""; /* 取得した社員CD .*/
    public string SHAIN_Name = "";	/* 取得した社員名 .*/
    public string SHAIN_Tenpo = "";	/* 取得した社員店舗 .*/
    public int? USER_CV = null;	/* EC用CV使用FLG */
    public string HelpDialog = ""; /* F1でヘルプ表示するCRSファイル .*/
    public string ComboKey = "NF3";	/* ComboBox系はF3でzoom可(NF3) .*/
    public string CrsErrorMsg = "\nセッションがリセットされました。\nもう一度操作し直すか、再LOGINして下さい。"; /* セッションエラーメッセージ */
    public string CrsErrorMsg2 = "\nサーバーエラーです。Bizを一旦終了して下さい。\nWebサーバーが正常に動作しているか確認して下さい"; /* サーバーエラーメッセージ */
    public int CrsSession = 0; /* タイムアウト警告メッセージを出すための内部カウンタ */
    public DateTime CrsSessionLastDate = DateTime.Now;
    public string AspxOpt1 = ""; /* isqlqry2などで返されるCRS-OPT1 .*/
    public string AspxOpt2 = ""; /* isqlqry2などで返されるCRS-OPT2 .*/
    public string AspxOpt3 = ""; /* isqlqry2などで返されるCRS-OPT3 .*/
    public string AspxDubug1 = ""; /* isqlqry2などで返されるCRS-DEBUG1 .*/
    public string AspxDubug2 = ""; /* isqlqry2などで返されるCRS-DEBUG2 .*/
    public int BgFlag = 0; /* 0:なにもしない、1:背景統一を実行 */
    public Color BgColor = Color.DarkGray;
    public Color FgColor = Color.DarkGray;
    public int RetXml = 0; /* aspxからの戻りをXML形式にする */
    public string BgSvg = "";

    public string DataAddPath = "/img/";   /* たとえばルートではなく "/img/"などに配置する場合 */
    public int PrintStreamVer = 30; /* もしVer3.1-を使用する場合には31をセット */
    public int MoreCoop = 0; /* 複数法人対応するなら1をセットする */
    public int PrintPDFFlg = 0; /* PDFで印刷を実行する */

    /// <summary>
    /// ■関数 GetVdate = 仮想日付の数値から日付を求める
    /// </summary>
    /// <param name="v_value">Number = 仮想日付数値</param>
    /// <returns>Date = 日付</returns>
    public DateTime GetVdate(double v_value)
    {
        try
        {
            // Add back the offset
            double adjusted = v_value + 366;

            // Use OADate conversion (matches Excel-style serial date system)
            DateTime result = DateTime.FromOADate(v_value);

            return result;
        }
        catch
        {
            // Fallback in case of invalid OADate
            return new DateTime(1900, 1, 1);
        }
    }

    /// <summary>
    /// ■関数 GetVdate = 日付から仮想日付の数値を求める
    /// </summary>
    /// <param name="v_date">String = 日付文字列(2004/01/01)</param>
    /// <returns>Number = 仮想日付数値</returns>
    public decimal GetVdateValue(string v_date)
    {
        try
        {
            if (DateTime.TryParse(v_date, out DateTime parsed))
            {
                // Biz Designer Date numeric = days from base (OADate is common)
                // Here I’ll use .NET’s DateTime.ToOADate (double) → cast to long
                decimal value = (decimal)parsed.ToOADate();
                value -= 366; // subtract 366 as Biz Designer does
                return value;
            }
            else return 0; 
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// ■関数 Compress = 圧縮文字列と展開文字列の変換
    /// </summary>
    /// <param name="v_flag">Number = 0=圧縮文字列へ,1=展開文字列へ</param>
    /// <param name="v_instr">String = 入力文字列</param>
    /// <returns>String = 処理結果文字列</returns>
    public string Compress(int v_flag, string v_instr)
    {
        const string str64 = ".DEFGHIJKL_MNOPQRSTUVWXYZBC0123456789axybcdefghijklAmnopqrstuvwz";

        if (string.IsNullOrEmpty(v_instr))
            return string.Empty;

        string instr2 = v_instr;

        // Pad for compression to multiple of 6
        if (v_flag == 0 && v_instr.Length % 6 != 0)
        {
            instr2 = v_instr + new string('0', 6 - (v_instr.Length % 6));
        }

        int maxLength = (v_flag == 0) ? instr2.Length / 6 : instr2.Length;
        var outStr = new System.Text.StringBuilder();

        for (int i = 0; i < maxLength; i++)
        {
            if (v_flag == 0) // Compress: group 6 bits into one symbol
            {
                int j = i * 6;
                int nowChar = 0x00;

                nowChar += (instr2[j] == '0' || instr2[j] == ' ') ? 0 : 0x20;
                nowChar += (instr2[j + 1] == '0' || instr2[j + 1] == ' ') ? 0 : 0x10;
                nowChar += (instr2[j + 2] == '0' || instr2[j + 2] == ' ') ? 0 : 0x8;
                nowChar += (instr2[j + 3] == '0' || instr2[j + 3] == ' ') ? 0 : 0x4;
                nowChar += (instr2[j + 4] == '0' || instr2[j + 4] == ' ') ? 0 : 0x2;
                nowChar += (instr2[j + 5] == '0' || instr2[j + 5] == ' ') ? 0 : 0x1;

                // map to custom table
                outStr.Append(str64[nowChar]);
            }
            else if (v_flag == 1) // Expand: decode symbol into 6-bit string
            {
                int nowChar = str64.IndexOf(instr2[i]);
                if (nowChar < 0 || nowChar > 63) nowChar = 0;

                outStr.Append((nowChar >= 0x20) ? "1" : "0");
                nowChar -= (nowChar >= 0x20) ? 0x20 : 0;

                outStr.Append((nowChar >= 0x10) ? "1" : "0");
                nowChar -= (nowChar >= 0x10) ? 0x10 : 0;

                outStr.Append((nowChar >= 0x08) ? "1" : "0");
                nowChar -= (nowChar >= 0x08) ? 0x08 : 0;

                outStr.Append((nowChar >= 0x04) ? "1" : "0");
                nowChar -= (nowChar >= 0x04) ? 0x04 : 0;

                outStr.Append((nowChar >= 0x02) ? "1" : "0");
                nowChar -= (nowChar >= 0x02) ? 0x02 : 0;

                outStr.Append((nowChar >= 0x01) ? "1" : "0");
            }
        }

        return outStr.ToString();
    }

    /// <summary>
    /// ■関数 GetJanCD = JAN13桁のチェックデジットを求める(モジュラス10のウェイト3)
    /// </summary>
    /// <param name="v_jan13">String = 13桁か12桁のJANコードor任意桁</param>
    /// <param name="v_col">String = NULLなら13桁JANを。指定あればその桁数+CD1桁で出力(NW7 M10W3用)</param>
    /// <returns>String = 正しいチェックデジット入りのJAN13桁コード(or12桁未満は0追加)</returns>
    public string GetJanCD(string v_jan13, int? v_col = null)
    {
        if (string.IsNullOrEmpty(v_jan13))
            v_jan13 = "";

        // Pad with zeros if less than 12 digits
        string retStr = v_jan13 + new string('0', 18);
        int vLen = 12;

        if (v_col.HasValue)
        {
            vLen = v_col.Value;
        }

        // Cut to desired length
        retStr = retStr.Substring(0, vLen);

        int vChk1 = 0;
        int vChk2 = 0;

        for (int i = 0; i < vLen; i++)
        {
            int digit = int.TryParse(retStr.Substring(i, 1), out int d) ? d : 0;

            // Weighting rule: odd/even depends on (vLen - i) % 2
            if ((vLen - i) % 2 == 0)
            {
                vChk1 += digit;       // Weight 1
            }
            else
            {
                vChk2 += digit * 3;   // Weight 3
            }
        }

        int vDgt = (vChk1 + vChk2) % 10;
        if (vDgt != 0) vDgt = 10 - vDgt;

        // Append check digit
        retStr += vDgt.ToString();

        return retStr;
    }

    /// <summary>
    /// ■関数 GetStringFirst = 文字列から空白区切りで見た先頭の文字列を取り出す
    /// </summary>
    /// <param name="v_string">String = 入力文字列("aaa bbb ccc ddd")</param>
    /// <returns>String = 文字列</returns>
    public string GetStringFirst(string v_string)
    {
        if (string.IsNullOrEmpty(v_string)) return "";

        var parts = v_string.Split(' ');
        return parts.Length > 0 ? parts[0] : "";
    }
    /* ===================================================
		■関数 AspxUpload = .Netサーバーへファイルをアップロードする
			引数1:I	String = ファイル名
			引数2:I	(Readメソッドを持つオブジェクト) = D&Dのe.DataやFileオブジェクトなど
			引数3:I	String = nullか "1"か"souko"か"tenpo"=HHT用Uploadパラメータ
			引数4:I	String = nullか "CRS"か"PSS"=Biz/Designer用Uploadパラメータ
			戻値		0=成功, -1=失敗
		=================================================== */

    public static async Task<int> AspxUploadAsync(
        string filePath,
        Stream fileData,
        string? v_ht = null,
        string? v_design = null)
    {
        try
        {
            string uploadUrl = AppData.Http!.URLroot + "isql/iupload2.aspx";

            string base64Data;
            using (var ms = new MemoryStream())
            {
                await fileData.CopyToAsync(ms);
                base64Data = Convert.ToBase64String(ms.ToArray());
            }

            // Sediakan parameter POST
            var content = new MultipartFormDataContent();
            content.Add(new StringContent("RAND12345"), "rd"); // contoh random/session ID
            content.Add(new StringContent(Path.GetFileName(filePath)), "fname");
            content.Add(new StringContent(base64Data), "file");

            // Optional params
            if (!string.IsNullOrEmpty(v_ht))
            {
                if (v_ht == "img")
                    content.Add(new StringContent(v_ht), "img");
                else
                    content.Add(new StringContent(v_ht), "hht");
            }

            if (!string.IsNullOrEmpty(v_design))
            {
                content.Add(new StringContent(v_design), "DIR");
            }

            // Hantar request
            HttpResponseMessage response = await httpClient.PostAsync(uploadUrl, content);
            string result = (await response.Content.ReadAsStringAsync()).ToUpperInvariant();

            // Semak hasil
            if (result.Contains("OK"))
            {
                // Success
                return 0;
            }

            return -1; // Failed
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Upload failed: {ex.Message}");
            return -1;
        }
    }
    /// <summary>
    /// ■関数 FullStr = 文字列の空白補完
    /// </summary>
    /// <param name="in_str">String = 入力文字列</param>
    /// <param name="in_str_length">Number = 結果の長さ(byte)</param>
    /// <param name="in_str_flag">Number = フラグ(0:左詰,1:右詰)</param>
    /// <param name="in_str_type">String = タイプ (A,N,D)</param>
    /// <param name="in_str_format">String = フォーマット文字列</param>
    /// <returns>戻値 結果文字列</returns>
    public string FullStr(string in_str, int in_str_length, int in_str_flag, string in_str_type, string in_str_format)
    {
        string outStr = string.Empty;

        if (!string.IsNullOrEmpty(in_str))
        {
            outStr = in_str.Trim();
            object wrkVal;

            if (!string.IsNullOrEmpty(in_str_type))
            {
                switch (in_str_type.ToUpper())
                {
                    case "A": // Alphanumeric
                        wrkVal = outStr;
                        outStr = string.IsNullOrEmpty(in_str_format)
                            ? wrkVal.ToString()
                            : string.Format("{0:" + in_str_format + "}", wrkVal);
                        break;

                    case "N": // Numeric
                        if (double.TryParse(outStr, out double numVal))
                        {
                            wrkVal = numVal;
                            outStr = string.IsNullOrEmpty(in_str_format)
                                ? numVal.ToString()
                                : numVal.ToString(in_str_format);
                            outStr = outStr.Replace(" ", "");
                        }
                        break;

                    case "D": // Date
                        if (DateTime.TryParse(outStr, out DateTime dateVal))
                        {
                            wrkVal = dateVal;
                            outStr = string.IsNullOrEmpty(in_str_format)
                                ? dateVal.ToString("yyyy/MM/dd")
                                : dateVal.ToString(in_str_format);
                        }
                        break;
                }
            }
        }

        int orgLen = outStr.Length;

        if (orgLen < in_str_length)
        {
            string spaceStr = new string(' ', in_str_length - orgLen);
            outStr = in_str_flag == 0 ? outStr + spaceStr : spaceStr + outStr;
        }
        else if (orgLen > in_str_length)
        {
            outStr = outStr.Substring(0, in_str_length);
            if (in_str_length < outStr.Length)
            {
                outStr = outStr.Substring(0, in_str_length - 1) + " ";
            }
        }

        return outStr;
    }
     
    /// <summary>
    /// ■関数 ConvString = 文字列の変換
    /// </summary>
    /// <param name="p_flag">Number = 0:Biz→サーバー 1:サーバー→Biz</param>
    /// <param name="p_str">String = 変換対象文字列</param>
    /// <returns>String = 変換後の文字列</returns>
    public string ConvString(int p_flag, string p_str)
    {
        if (p_str == null) return string.Empty;

        string retStr = p_str;

        if (p_flag == 0)
        {
            // Biz → Server
            retStr = retStr.Replace(",", ((char)27).ToString());
            retStr = retStr.Replace("\"", ((char)28).ToString());
            retStr = retStr.Replace("\n", ((char)29).ToString());
            retStr = retStr.Replace("\r", ((char)30).ToString());
        }
        else if (p_flag == 1)
        {
            // Server → Biz
            retStr = retStr.Replace(((char)27).ToString(), ",");
            retStr = retStr.Replace(((char)28).ToString(), "\"");
            retStr = retStr.Replace(((char)29).ToString(), "\n");
            retStr = retStr.Replace(((char)30).ToString(), "\r"); 
        }

        return retStr;
    }
     
    /// <summary>
    /// ■関数 getDateVal = 指定された年月日の月初日あるいは月末日を求める
    /// </summary>
    /// <param name="date_obj">Date = Dateオブジェクト</param>
    /// <param name="flag">Number = 0:月初, 1:月末</param>
    /// <param name="v_format"></param>
    /// <returns>Date = 月初日あるいは月末日</returns>
    public string GetDateVal(DateTime date_obj, int? flag, string v_format = null)
    {
        // Normalize to first day of the month
        DateTime wrk1 = new DateTime(date_obj.Year, date_obj.Month, 1);
        if (flag == null || flag == 0)
        {
            // Month start
            return v_format == null
                ? wrk1.ToString("yyyy/MM/dd")
                : wrk1.ToString("yyyyMMdd");
        }
        else
        {
            // Month end: last day of month
            wrk1 = new DateTime(date_obj.Year, date_obj.Month, DateTime.DaysInMonth(date_obj.Year, date_obj.Month));
            return v_format == null
                ? wrk1.ToString("yyyy/MM/dd")
                : wrk1.ToString("yyyyMMdd");
        } 
    }

    /// <summary>
    /// ■関数 getDateVal2 = 指定された年月日に月数を足し引きする(1日に正規化)
    /// </summary>
    /// <param name="date_obj">Date = Dateオブジェクト</param>
    /// <param name="v_mon">Number = 月</param>
    /// <returns>Date = 計算した年月日</returns>
    public DateTime GetDateVal2(DateTime date_obj, int v_mon)
    {
        // Normalize to the first day of the current month
        var wrk1 = new DateTime(date_obj.Year, date_obj.Month, 1);

        // Add v_mon months
        return wrk1.AddMonths(v_mon);
    }

    /// <summary>
    /// ■関数 getDateVal3 = 指定された年月の締日基準の範囲を求める
    /// </summary>
    /// <param name="date_obj">Date = Dateオブジェクト</param>
    /// <param name="v_sime">Number = 締日</param>
    /// <param name="flag">Number = 0:開始年月日,1:終了年月日</param>
    /// <returns>Date = 計算した年月日</returns>
    public DateTime GetDateVal3(DateTime date_obj, int v_sime, int flag)
    {
        // Normalize to the first day of the current month
        var wrk1 = new DateTime(date_obj.Year, date_obj.Month, 1);
        var wrk2 = v_sime;
        if (v_sime >= 29)
        {
            if (flag == 0)
            {
                // 月初
                return GetDateValDate(wrk1, 0);
            }
            else
            {
                // 月末
                return GetDateValDate(wrk1, 1);
            }
        }

        if (flag == 0)
        {
            // 締日の開始日 (前月 + v_sime 日)
            var prevMonth = GetDateVal2(wrk1, -1); // 前月1日
            return prevMonth.AddDays(v_sime);
        }
        else
        {
            // 締日の終了日 (当月 + v_sime - 1 日)
            return wrk1.AddDays(v_sime - 1);
        }
    }
    public DateTime GetDateValDate(DateTime dateObj, int flag)
    {
        var firstDay = new DateTime(dateObj.Year, dateObj.Month, 1);
        if (flag == 0)
            return firstDay;
        else
            return firstDay.AddMonths(1).AddDays(-1);
    }

    /// <summary>
    /// ■関数 getDateVal4 = 指定された年月の締日が存在する基準の範囲を締日求める
    /// </summary>
    /// <param name="date_obj">Date = Dateオブジェクト</param>
    /// <param name="v_sime">Number = 締日</param>
    /// <param name="flag">Number = 0:開始年月日,1:終了年月日</param>
    /// <returns>Date = 計算した年月日</returns>
    public string GetDateVal4(DateTime date_obj, int v_sime, int flag)
    {
        var wrk1 = date_obj;
        int day = wrk1.Day;

        // If current day > 締日 → move to next month
        if (day > v_sime)
        {
            wrk1 = GetDateVal2(wrk1, 1); // Next month 1st day
        }

        // Normalize to 1st day of month
        wrk1 = new DateTime(wrk1.Year, wrk1.Month, 1);

        if (v_sime >= 29)
        {
            if (flag == 0)
            {
                return GetDateVal(wrk1, 0, "yyyyMMdd"); // 月初
            }
            else
            {
                return GetDateVal(wrk1, 1, "yyyyMMdd"); // 月末
            }
        }

        if (flag == 0)
        {
            // 前月 + 締日
            var prevMonth = GetDateVal2(wrk1, -1);
            wrk1 = prevMonth.AddDays(v_sime - 1);
        }
        else
        {
            // 当月 + 締日 - 1
            wrk1 = wrk1.AddDays(v_sime - 1);
        }

        return wrk1.ToString("yyyyMMdd");
    }
     
    /// <summary>
    /// ■関数 getDateDiff = 指定された年月日の差を求める(obj1 - obj2)
    /// </summary>
    /// <param name="date_obj1">Date = Dateオブジェクト1</param>
    /// <param name="date_obj2">Date = Dateオブジェクト2</param>
    /// <returns>String = 差の時間を表す文字列(HHHH:MI:SS)</returns>
    public string GetDateDiff(DateTime date_obj1, DateTime date_obj2)
    {
        // Step 1: difference in seconds
        var diffSeconds = (date_obj1 - date_obj2).TotalSeconds;

        // Step 2: absolute difference (positive duration)
        var absSeconds = Math.Abs(diffSeconds);

        // Step 3: compute hours (preserve sign)
        int hours = diffSeconds >= 0
            ? (int)Math.Floor(diffSeconds / 3600)
            : (int)Math.Ceiling(diffSeconds / 3600);

        // Step 4: remainder for minutes/seconds
        int minutes = (int)(absSeconds % 3600 / 60);
        int seconds = (int)(absSeconds % 60);

        // Step 5: format HHHH:MM:SS (hours can exceed 24)
        return $"{hours:D4}:{minutes:D2}:{seconds:D2}";
    }

    /// <summary>
    /// ■関数 getDateVal = 指定された年月日が含まれる年度の開始日を返す
    /// </summary>
    /// <param name="date_obj">Date = Dateオブジェクト</param>
    /// <param name="v_mon">Number = 月</param>
    /// <returns>String = 年度開始年月日</returns>
    public DateTime GetDateYear(DateTime date_obj, int v_mon)
    {
        // Step 1 & 2: Normalize to first day of the month
        var wrk1 = new DateTime(date_obj.Year, date_obj.Month, 1);

        // Step 3: Get year
        int wrkY = wrk1.Year;

        // Step 4: If month < vMon, fiscal year is previous year
        if (wrk1.Month < v_mon)
        {
            wrkY--;
        }

        // Step 5: Return fiscal year start (note: C# months are 1-based)
        return new DateTime(wrkY, v_mon, 1);
    }

    public double GetHexValue(string v_hex_para)
    {
        /* ===================================================
            ■関数 GetHexValue = 16進を表す文字列を数値に変換する
                引数1:I	String = 16進を表す文字列
                戻値		Number
            =================================================== */
        var v_hex = v_hex_para;
        var wrk00 = "0123456789ABCDEF";
        double ret_val = 0;
        int now_hex;
        var j = v_hex.Length;
        for (var i = 0; i < v_hex.Length; i++)
        {
            j--;
            if ((now_hex = wrk00.IndexOf(v_hex.Substring(i, 1).ToUpper(), 0)) >= 0)
            {
                ret_val += (now_hex) * Math.Pow(16, j);
            }
        }
        return ret_val;
    }

    public int AspxGetSESS_ID()
    {
        /* ===================================================
            ■関数 AspxGetSESS_ID = 内部のAspxRandId文字列からSESS_IDの数値を求める
                戻値		Number
            =================================================== */
        var seed_value = 200000000;
        var rand_id = AspxRandId;
        var wrk_id_length = int.Parse(rand_id.Substring(rand_id.Length - 1, 1));
        var rand_id_sub = rand_id.Substring(0, wrk_id_length);
        double wrk_id;
        double wrk_id2;
        if (rand_id_sub.Substring(0, 1) != "-")
        {
            wrk_id = GetHexValue(rand_id.Substring(0, wrk_id_length).ToString());
        }
        else
        {
            wrk_id = GetHexValue(rand_id.Substring(0, wrk_id_length)) * -1;
        }
        wrk_id2 = GetHexValue(rand_id.Substring(wrk_id_length, rand_id.Length - wrk_id_length - 1));
        var ret_val = int.Parse((wrk_id2 + wrk_id - seed_value).ToString());
        return ret_val;
    }
    private static DateTime start_date = new DateTime(1901, 1, 1, 0, 0, 0); // 1901/01/01 00:00:00
    /// <summary>
    /// 日付から、日付を表す値を求める
    /// </summary>
    /// <param name="date_value">DateTime型</param>
    /// <returns type="double">日付を表すdouble型</returns>
    public static double DateToValue(DateTime date_value)
    {
        TimeSpan span = date_value - start_date;
        decimal span_d1 = (decimal)span.TotalDays; // 現在時刻
        double span_d2 = (double)span_d1;
        return span_d2;
    }
    /// <summary>
    /// 日付を表すvdate値から日付を求める
    /// </summary>
    /// <param name="date_value"></param>
    /// <returns></returns>
    public static DateTime DateFromValue(double date_value)
    {
        return start_date.AddDays(date_value);
    }
}
public class LoginHeader
{ 
    public string HeaderName { get; set; } = string.Empty;
    public string HeaderValue { get; set; } = string.Empty;
}
public class ResHeader
{
    public string HeaderName { get; set; } = string.Empty;
    public string HeaderValue { get; set; } = string.Empty;
} 
