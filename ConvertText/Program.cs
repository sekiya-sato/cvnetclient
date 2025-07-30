// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
var defaultEncode = Encoding.GetEncoding("Shift_JIS");
while (true) {
	Console.WriteLine("**********\nURLエンコードされた文字列を入力してください(Ctrl+Cで終了)");
	var inputstr = Console.ReadLine();
	if(string.IsNullOrEmpty(inputstr)) {
		Console.WriteLine("入力が空です。");
		continue;
	}
	string decoded = System.Web.HttpUtility.UrlDecode(inputstr, defaultEncode);
	Console.WriteLine("***** デコード文字列 *****");
	Console.WriteLine(decoded);
}



