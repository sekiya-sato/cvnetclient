using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg71Pos1ViewModel : BaseViewModel
    {
        #region Declare
        public enum MasterType { POS,WMS,Both}
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
        private BizArray para;
        private BizArray v_flg;
        #endregion
        #region Initialize
        public void OnInit(object? init_para = null, object? init_flg = null) 
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

            if (init_flg != null)
            {
                if (init_flg is string s)
                {
                    var v_para = new string[] { s };
                    v_flg = new BizArray(v_para);
                }
                else if (init_flg is string[] arr)
                    v_flg = new BizArray(arr);
                else v_flg = new BizArray();
            }
            else v_flg = new BizArray();
            SelectedMaster = MasterType.WMS;
            DateFrom = DateTime.Now.AddDays(-1);
            LogDocument = new FlowDocument();
            Result = string.Empty;
            ResultText = string.Empty;
        }
        #endregion
        #region Function
        partial void OnDateFromChanged(DateTime? value) 
        {
            DayBefore = Math.Round((DateTime.Now - value.Value).TotalDays, 3);
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
            if (SelectedMaster == MasterType.POS) {
                v_para[1] = "0";
            } else if (SelectedMaster == MasterType.WMS)
            {
                v_para[1] = "1";
            }
            else {
                v_para[1] = "2";
            }

            var flg = new int[1];
            if (SelectedMaster == MasterType.Both)
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

                foreach (var line in lines)
                {
                    var trimmed = line.Trim('"', ' ', '\r', '\n');

                    if (trimmed.Contains("/Data/ExtCoop/"))
                        urlList.Add(trimmed);
                    else if (trimmed.Contains("件"))       
                        resultList.Add(trimmed);
                }

                foreach (var (url, index) in urlList.Select((v, i) => (v, i)))
                {
                    ResultText += $"{baseUrl}{url}";
                    ResultText += "\n";
                }

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
        #endregion
    }
}

