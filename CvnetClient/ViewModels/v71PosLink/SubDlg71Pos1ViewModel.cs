using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using System;
using System.Buffers.Text;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg71Pos1ViewModel : BaseViewModel
    {
        public enum MasterType { POSマスタ,WMSマスタ,両方}
        [ObservableProperty]
        private MasterType selectedMaster;
        [ObservableProperty]
        private DateTime? dateFrom;
        [ObservableProperty]
        private double? dayBefore;
        [ObservableProperty]
        private string? result;
        [ObservableProperty]
        private FlowDocument logDocument;
        [ObservableProperty]
        private string resultText;
        public void OnInit() 
        { 
            SelectedMaster = MasterType.WMSマスタ;
            DateFrom = DateTime.Now.AddDays(-1);
            DayBefore = Math.Round((DateTime.Now - DateFrom.Value).TotalDays, 3);
            LogDocument = new FlowDocument();
            Result = string.Empty;
            ResultText = string.Empty;
        }

        [RelayCommand]
        public void DoExecute() 
        {
            
            var start0 = DateTime.Now;

            var wrk2 = AppData.Url;
            var baseUrl = wrk2.Substring(0, wrk2.IndexOf("/", 7));


            var v_para = new string[2];
            DayBefore = Math.Round((DateTime.Now - DateFrom.Value).TotalDays, 3);
            v_para[0] = DayBefore.ToString();
            if (SelectedMaster == MasterType.POSマスタ) {
                v_para[1] = "0";
            } else if (SelectedMaster == MasterType.WMSマスタ)
            {
                v_para[1] = "1";
            }
            else {
                v_para[1] = "2";
            }

            var flg = new int[1];
            if (SelectedMaster == MasterType.両方)
            {
                flg = new int[2];
                flg[0] = 0;
                flg[1] = 1;
            }
            else
            {
                flg[0] = int.Parse(v_para[1]) ;
            }

            for (var j = 0; j < flg.Length; j++)
            {
                v_para[1] = flg[j].ToString();
                var v_syori = "posout";
                if (AppData.ClassCvnet.config.UserFlg == 56) v_syori = "posout56";

                var wrk_csv = AppData.Http!.AspxSqlQuery2(v_syori, v_para, "", 39);
                var lines = wrk_csv.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                ResultText += "*********処理開始************\n";

                List<string> urlList = new();
                List<string> resultList = new();

                // Pisahkan URL dan result line
                foreach (var line in lines)
                {
                    var trimmed = line.Trim('"', ' ', '\r', '\n');

                    if (trimmed.Contains("/Data/ExtCoop/"))   // baris URL
                        urlList.Add(trimmed);
                    else if (trimmed.Contains("件"))           // baris result
                        resultList.Add(trimmed);
                }

                // 🟢 Bahagian URL
                foreach (var (url, index) in urlList.Select((v, i) => (v, i)))
                {
                    ResultText += $"{baseUrl}{url}";
                    // Setiap 1 baris link → newline
                    ResultText += "\n";
                }

                // 🟢 Bahagian Result Count (format 5 item / baris)
                for (int i = 0; i < resultList.Count; i++)
                {
                    ResultText += resultList[i];
                    if ((i + 1) % 5 == 0 || i == resultList.Count - 1)
                        ResultText += "\n";
                    else
                        ResultText += " / ";
                }

                ResultText += "\n*********処理終了************\n";
            }
            UpdateLogDocument();
            var end0 = DateTime.Now;

            var wrk_mess = "データ作成しました \n終了時刻:" + DateTime.Now.ToString("HH:mm:ss");
            var elapsed = end0 - start0;
            wrk_mess += "\n経過時間：" + elapsed.ToString(@"hh\:mm\:ss");

            Result = wrk_mess;                 
            return;
        }

        public void UpdateLogDocument()
        {
            var doc = new FlowDocument
            {
                PagePadding = new Thickness(0),
                FontFamily = new FontFamily("Consolas"),
                FontSize = 13
            };

            var lines = ResultText.Split('\n');

            foreach (var line in lines)
            {
                var p = new Paragraph
                {
                    Margin = new Thickness(0),
                    LineHeight = 14
                };

                if (line.Contains("処理開始"))
                {
                    p.Inlines.Add(new Run(line)
                    {
                        Foreground = Brushes.DarkGreen,
                        FontWeight = FontWeights.Bold
                    });
                }
                else if (line.Contains("処理終了"))
                {
                    p.Inlines.Add(new Run(line)
                    {
                        Foreground = Brushes.DarkRed,
                        FontWeight = FontWeights.Bold
                    });
                }
                else if (Regex.IsMatch(line, @"https?://\S+"))
                {
                    foreach (Match m in Regex.Matches(line, @"https?://\S+"))
                    {
                        var link = new Hyperlink(new Run(m.Value))
                        {
                            NavigateUri = new Uri(m.Value),
                            Foreground = Brushes.Blue
                        };
                        link.RequestNavigate += (s, e) =>
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri)
                            {
                                UseShellExecute = true
                            });
                        };
                        p.Inlines.Add(link);
                        p.Inlines.Add(new Run(" "));
                    }
                }
                else
                {
                    p.Inlines.Add(new Run(line));
                }

                doc.Blocks.Add(p);
            }

            LogDocument = doc;
        }
    }
}

