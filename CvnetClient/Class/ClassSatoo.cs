using System.Drawing; 
 
public class ClassSatoo
{
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

    public string GetStringFirst(string v_string)
    {
        if (string.IsNullOrEmpty(v_string)) return "";

        var parts = v_string.Split(' ');
        return parts.Length > 0 ? parts[0] : "";
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
