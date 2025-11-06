using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Class;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg_prn_kakumstViewModel : BaseViewModel
    {
        // -------------------------
        // RangeValue: shared structure for Code + Name
        // -------------------------
        public class RangeValue
        {
            public string Code { get; set; } = "";
            public string Name { get; set; } = "";
        }

        // -------------------------
        // RangeRow: now holds TWO range sets (and two labels)
        // -------------------------
        public class RangeRow
        {
            // ---- Labels ----
            public string Label1 { get; set; } = "";
            public string Label2 { get; set; } = "";
            public string Label3 { get; set; } = "";
            public string Label4 { get; set; } = "";
            public string Label5 { get; set; } = "";
            public string Label6 { get; set; } = "";
            public string Label7 { get; set; } = "";
            public string Label8 { get; set; } = "";
            public string Label9 { get; set; } = "";
            public string Label10 { get; set; } = "";

            // ---- Range Values ----
            public RangeValue RangeFrom1 { get; set; } = new();
            public RangeValue RangeTo1 { get; set; } = new();

            public RangeValue RangeFrom2 { get; set; } = new();
            public RangeValue RangeTo2 { get; set; } = new();

            public RangeValue RangeFrom3 { get; set; } = new();
            public RangeValue RangeTo3 { get; set; } = new();

            public RangeValue RangeFrom4 { get; set; } = new();
            public RangeValue RangeTo4 { get; set; } = new();

            public RangeValue RangeFrom5 { get; set; } = new();
            public RangeValue RangeTo5 { get; set; } = new();

            public RangeValue RangeFrom6 { get; set; } = new();
            public RangeValue RangeTo6 { get; set; } = new();

            public RangeValue RangeFrom7 { get; set; } = new();
            public RangeValue RangeTo7 { get; set; } = new();

            public RangeValue RangeFrom8 { get; set; } = new();
            public RangeValue RangeTo8 { get; set; } = new();

            public RangeValue RangeFrom9 { get; set; } = new();
            public RangeValue RangeTo9 { get; set; } = new();

            public RangeValue RangeFrom10 { get; set; } = new();
            public RangeValue RangeTo10 { get; set; } = new();

            // ---- Commands ----
            public ICommand SelRangeFrom1Command { get; set; }
            public ICommand SelRangeTo1Command { get; set; }

            public ICommand SelRangeFrom2Command { get; set; }
            public ICommand SelRangeTo2Command { get; set; }

            public ICommand SelRangeFrom3Command { get; set; }
            public ICommand SelRangeTo3Command { get; set; }

            public ICommand SelRangeFrom4Command { get; set; }
            public ICommand SelRangeTo4Command { get; set; }

            public ICommand SelRangeFrom5Command { get; set; }
            public ICommand SelRangeTo5Command { get; set; }

            public ICommand SelRangeFrom6Command { get; set; }
            public ICommand SelRangeTo6Command { get; set; }

            public ICommand SelRangeFrom7Command { get; set; }
            public ICommand SelRangeTo7Command { get; set; }

            public ICommand SelRangeFrom8Command { get; set; }
            public ICommand SelRangeTo8Command { get; set; }

            public ICommand SelRangeFrom9Command { get; set; }
            public ICommand SelRangeTo9Command { get; set; }

            public ICommand SelRangeFrom10Command { get; set; }
            public ICommand SelRangeTo10Command { get; set; }
        }

        // ---- Tab 1 ----
        [ObservableProperty] private ObservableCollection<RangeRow> tab1Rows_SectionA = new(); // Label1 & Label2
        [ObservableProperty] private ObservableCollection<RangeRow> tab1Rows_SectionB = new(); // Label3 & Label4

        // ---- Tab 2 ----
        [ObservableProperty] private ObservableCollection<RangeRow> tab2Rows_SectionA = new();

        // ---- Tab 3 ---- 
        [ObservableProperty] private ObservableCollection<RangeRow> tab3Rows_SectionA = new();

        // ---- Tab 5 ---- 
        [ObservableProperty] private ObservableCollection<RangeRow> tab5Rows_SectionA = new();

        // ---- Tab 6 ----
        [ObservableProperty] private ObservableCollection<RangeRow> tab6Rows_SectionA = new();
        [ObservableProperty] private ObservableCollection<RangeRow> tab6Rows_SectionB = new();

        private readonly (string Label1, string Label2)[] _labelTab1 =
        {
            ("ﾌﾞﾗﾝﾄ", "ｱｲﾃﾑ"),
            ("ｼｰｽﾞﾝ", "ﾃﾞｻﾞｲﾅｰ"),
            ("展示会", "素材"),
            ("原産国", "ﾒｰｶｰ"),
        };

        private readonly string[] _labelTab2 =
        {
            "得意先CD",
            "営業担当CD",
            "請求先CD"
        };
        private readonly string[] _labelTab3 =
        {
            "仕入先CD",
            "支払先CD",
        };

        private readonly string[] _labelTab5 =
        {
            "生地付属CD",
            "仕入先CD"
        };
        private readonly string[] _labelTab6 =
        {
            "社員CD",
            "所属店舗CD"
        };

        public SubDlg_prn_kakumstViewModel()
        {
            for (int i = 0; i < _labelTab1.Length ; i++)
            {
                tab1Rows_SectionA.Add(new RangeRow
                {
                    Label1 = _labelTab1[i].Label1,
                    Label2 = _labelTab1[i].Label2,

                    // 1st pair of range buttons
                    SelRangeFrom1Command = new RelayCommand<object>(_ => OnSelectFrom1(i)),
                    SelRangeTo1Command = new RelayCommand<object>(_ => OnSelectTo1(i)),

                    // 2nd pair of range buttons
                    SelRangeFrom2Command = new RelayCommand<object>(_ => OnSelectFrom2(i)),
                    SelRangeTo2Command = new RelayCommand<object>(_ => OnSelectTo2(i)),

                });
            }
            for (int i = 0; i < 5; i++)
            {
                tab1Rows_SectionB.Add(new RangeRow
                {
                    Label3 = $"補足{i + 1}",
                    Label4 = $"補足{i + 6}",

                    // 3rd pair of range buttons
                    SelRangeFrom3Command = new RelayCommand<object>(_ => OnSelectFrom3(i)),
                    SelRangeTo3Command = new RelayCommand<object>(_ => OnSelectTo3(i)),

                    // 4th pair of range buttons
                    SelRangeFrom4Command = new RelayCommand<object>(_ => OnSelectFrom4(i)),
                    SelRangeTo4Command = new RelayCommand<object>(_ => OnSelectTo4(i))
                });
            }
            for (int i = 0; i < _labelTab2.Length ; i++)
            {
                tab2Rows_SectionA.Add(new RangeRow
                {
                    Label5 = _labelTab2[i],

                    // 5th pair of range buttons
                    SelRangeFrom5Command = new RelayCommand<object>(_ => OnSelectFrom5(i)),
                    SelRangeTo5Command = new RelayCommand<object>(_ => OnSelectTo5(i)),
                });
            }
            for (int i = 0; i < 2; i++)
            {
                tab3Rows_SectionA.Add(new RangeRow
                {
                    Label6 = _labelTab3[i],

                    // 6th pair of range buttons
                    SelRangeFrom6Command = new RelayCommand<object>(_ => OnSelectFrom6(i)),
                    SelRangeTo6Command = new RelayCommand<object>(_ => OnSelectTo6(i))
                });
            }
            for (int i = 0; i < 2; i++)
            {
                tab5Rows_SectionA.Add(new RangeRow
                {
                    Label8 = _labelTab5[i],

                    // 8th pair of range buttons
                    SelRangeFrom8Command = new RelayCommand<object>(_ => OnSelectFrom8(i)),
                    SelRangeTo8Command = new RelayCommand<object>(_ => OnSelectTo8(i)),
                });
            }
            for (int i = 0; i < 2; i++)
            {
                tab6Rows_SectionA.Add(new RangeRow
                {
                    Label9 = _labelTab6[i],

                    // 9th pair of range buttons
                    SelRangeFrom9Command = new RelayCommand<object>(_ => OnSelectFrom9(i)),
                    SelRangeTo9Command = new RelayCommand<object>(_ => OnSelectTo9(i))
                });
            }
            for (int i = 0; i < 5; i++)
            {
                tab6Rows_SectionB.Add(new RangeRow
                {
                    Label10 = $"社員分類{i + 1}",

                    // 10th pair of range buttons
                    SelRangeFrom10Command = new RelayCommand<object>(_ => OnSelectFrom10(i)),
                    SelRangeTo10Command = new RelayCommand<object>(_ => OnSelectTo10(i)),
                });
            }
        }

        // --- 1st label set ---
        private void OnSelectFrom1(int index)
        {
            // handle first label’s "From" selection
        }

        private void OnSelectTo1(int index)
        {
            // handle first label’s "To" selection
        }

        // --- 2nd label set ---
        private void OnSelectFrom2(int index)
        {
            // handle second label’s "From" selection
        }

        private void OnSelectTo2(int index)
        {
            // handle second label’s "To" selection
        }
        // --- 3rd label set ---
        private void OnSelectFrom3(int index)
        {
            // handle first label’s "From" selection
        }

        private void OnSelectTo3(int index)
        {
            // handle first label’s "To" selection
        }

        // --- 4th label set ---
        private void OnSelectFrom4(int index)
        {
            // handle second label’s "From" selection
        }

        private void OnSelectTo4(int index)
        {
            // handle second label’s "To" selection
        }

        // --- 5th label set ---
        private void OnSelectFrom5(int index)
        {
            // handle second label’s "From" selection
        }

        private void OnSelectTo5(int index)
        {
            // handle second label’s "To" selection
        }

        // --- 6th label set ---
        private void OnSelectFrom6(int index)
        {
            // handle second label’s "From" selection
        }

        private void OnSelectTo6(int index)
        {
            // handle second label’s "To" selection
        }
        private void OnSelectFrom7(int index)
        {
            // handle label7's "From" selection
        }

        private void OnSelectTo7(int index)
        {
            // handle label7's "To" selection
        }

        private void OnSelectFrom8(int index)
        {
            // handle label8's "From" selection
        }

        private void OnSelectTo8(int index)
        {
            // handle label8's "To" selection
        }

        private void OnSelectFrom9(int index)
        {
            // handle label9's "From" selection
        }

        private void OnSelectTo9(int index)
        {
            // handle label9's "To" selection
        }

        private void OnSelectFrom10(int index)
        {
            // handle label10's "From" selection
        }

        private void OnSelectTo10(int index)
        {
            // handle label10's "To" selection
        }

    }
}
