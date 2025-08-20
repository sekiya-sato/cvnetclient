using System.Drawing; 
 
public class ClassSatoo
{
    public static int SysTest = 0; /* デバッグ用ﾃｽﾄﾌﾗｸﾞ .*/
    public static int ImgLoad = 1; /* 一覧表示Imageﾌﾗｸﾞ(0:無,1:有,2:バックグラウンド)  .*/
    public static string Version = "20060404"; /* Version .*/
    public static string AspxPath = string.Empty; /* aspxのパス(例=http://www.etc.co.jp/cv.net/あるいは/cv.net/) .*/
    public static string AspxRandId = "";	/* Session変数 .*/
    public static LoginHeader LoginHeader = new LoginHeader(); /* 最終ログイン試行時のヘッダー情報 .*/
    public static ResHeader ResHeader = new ResHeader(); /* Query試行時のヘッダー情報 .*/

    public static int LoginKubun = 0;	/* 入力されたLOGIN区分 .*/
    public static string LoginId = ""; /* 入力されたLOGIN ID .*/
    public static string LoginSEC = ""; /* 取得したSEC .*/
    public static string LoginMG_FLG = ""; /* 取得したMG_FLG .*/
    public static string SHAIN_CD = ""; /* 取得した社員CD .*/
    public static string SHAIN_Name = "";	/* 取得した社員名 .*/
    public static string SHAIN_Tenpo = "";	/* 取得した社員店舗 .*/
    public static int? USER_CV = null;	/* EC用CV使用FLG */
    public static string HelpDialog = ""; /* F1でヘルプ表示するCRSファイル .*/
    public static string ComboKey = "NF3";	/* ComboBox系はF3でzoom可(NF3) .*/
    public static string CrsErrorMsg = "\nセッションがリセットされました。\nもう一度操作し直すか、再LOGINして下さい。"; /* セッションエラーメッセージ */
    public static string CrsErrorMsg2 = "\nサーバーエラーです。Bizを一旦終了して下さい。\nWebサーバーが正常に動作しているか確認して下さい"; /* サーバーエラーメッセージ */
    public static int CrsSession = 0; /* タイムアウト警告メッセージを出すための内部カウンタ */
    public static DateTime CrsSessionLastDate = DateTime.Now;
    public static string AspxOpt1 = ""; /* isqlqry2などで返されるCRS-OPT1 .*/
    public static string AspxOpt2 = ""; /* isqlqry2などで返されるCRS-OPT2 .*/
    public static string AspxOpt3 = ""; /* isqlqry2などで返されるCRS-OPT3 .*/
    public static string AspxDubug1 = ""; /* isqlqry2などで返されるCRS-DEBUG1 .*/
    public static string AspxDubug2 = ""; /* isqlqry2などで返されるCRS-DEBUG2 .*/
    public static int BgFlag = 0; /* 0:なにもしない、1:背景統一を実行 */
    public static Color BgColor = Color.DarkGray;
    public static Color FgColor = Color.DarkGray;
    public static int RetXml = 0; /* aspxからの戻りをXML形式にする */
    public static string BgSvg = "";
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
