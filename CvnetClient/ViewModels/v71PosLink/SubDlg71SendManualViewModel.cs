using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg71SendManualViewModel : BaseViewModel
    {
        public enum WMSType { WMSOUT , WMSIN }
        [ObservableProperty]
        private WMSType selectedWMS;
        [ObservableProperty]
        private int dayAfter;
        [ObservableProperty]
        private string? result;
        [ObservableProperty]
        private FlowDocument? logDocument;
        [ObservableProperty]
        private string? resultText;
        [ObservableProperty]
        private string? subject;

        public void OnInit() 
        {
            SelectedWMS = WMSType.WMSOUT;
            if (AppData.ClassCvnet.config.UserFlg == 23)
            {
                SelectedWMS = WMSType.WMSIN;
            }
            LogDocument = new FlowDocument();
            DayAfter = 1;            
            Result = string.Empty;
            ResultText = string.Empty;
            
        }

        partial void OnDayAfterChanged(int value)
        {
            Subject = "送信対象：～" + DateTime.Now.AddDays(value).ToString("yyyy/MM/dd");
        }

        [RelayCommand]
        public void DoExecute()
        {
            if (!ClientLib.MessageBox(this, "送信データを作成してよろしいですか？")) return;

            var start0 = DateTime.Now;

            var wrk2 = AppData.Url;
            var wrk3 = wrk2.Substring(0, wrk2.IndexOf("/", 7));

            var v_para = new string[2];
            v_para[0] = SelectedWMS.ToString();
            v_para[1] = DayAfter.ToString();
            var wrk_csv = AppData.Http!.AspxSqlQuery2("Send_Manual", v_para, "", 39);
            var splited_csv = wrk_csv.Split(",");
            if (int.Parse(splited_csv[0].ToString()!.Split("=")[1]) < 0)
            {
                ClientLib.MessageBoxError(this, "CV-71001 送信データ作成エラー");
                return;
            }
            ResultText += "*****処理開始*****************************\n";
            ResultText += splited_csv[0] + "\n";
            ResultText += wrk3 + splited_csv[1] + "\n";
            ResultText += "*****処理終了*****************************\n";
            AppendCR(ResultText);
            UpdateLogDocument();
            var end0 = DateTime.Now;
            
            var wrk_mess = "データ作成しました \n終了時刻:" + DateTime.Now.ToString("HH:mm:ss");
            var elapsed = end0 - start0;
            wrk_mess += "\n経過時間：" + elapsed.ToString(@"hh\:mm\:ss");

            Result = wrk_mess;
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

        private string AppendCR(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            // Normalize all newline to LF, then convert LF → CR+LF
            return input
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Replace("\n", "\r\n");
        }
    }
}
