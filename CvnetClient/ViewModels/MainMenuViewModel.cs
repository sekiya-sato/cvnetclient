using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace CvnetClient.ViewModels
{
    public partial class MainMenuViewModel : ObservableObject{

        [ObservableProperty]
        string exeVer = ClassHttp.NetFramework;
        [ObservableProperty]
        string? bottomMessage;
        [ObservableProperty]
        DateTime? dateNow;

        DispatcherTimer timer = new();

        [ObservableProperty]
        Page? currentPage;

        partial void OnCurrentPageChanged(Page? value)
        {
            OnPropertyChanged(nameof(IsHomeSelected));
            OnPropertyChanged(nameof(IsReportSelected));
            OnPropertyChanged(nameof(IsSettingsSelected));
        }

        public bool IsHomeSelected => CurrentPage is HomePage;
        public bool IsReportSelected => CurrentPage is ReportPageView;
        public bool IsSettingsSelected => CurrentPage is SettingPage;

        public MainMenuViewModel() {
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) => DateNow = DateTime.Now;
            timer.Start();
            CurrentPage = new HomePage();
        }
        [RelayCommand]
        void Init()
        {
            // 設定ファイル読み込み
            BottomMessage = AppData.Url;
            var isLogin = false;
            debugPreLogin();
            // ClickOnceかどうかの判定
            if (Environment.GetEnvironmentVariable("ClickOnce_IsNetworkDeployed")?.ToLower() != "true")
            {
                if (AppData.AppConfig["AutoLogin"] == "True")
                {
                    var loginvm = new LoginViewModel();
                    var ret = loginvm.AutoLogin();
                    if (ret == 0)
                    {
                        isLogin = true;
                        BottomMessage += $" ログイン時間{DateTime.Now:yyyy/MM/dd HH:mm:ss}";
                    }
                }
            }
            if (!isLogin)
            { // ログインしていない場合、ログイン画面の表示
                if (ClientLib.ShowDialogView(new Views.LoginView(), this) == true)
                {
                    BottomMessage += $" ログイン時間{DateTime.Now:yyyy/MM/dd HH:mm:ss}";
                }
            }
            debugPostLogin();
        }
        [RelayCommand]
        void Exit()
        {
            ClientLib.Exit(this);
        }
        [RelayCommand]
        void DoMenu(MenuData param)
        {
            if (param == null || param.ViewType == null) return;
            var view = Activator.CreateInstance(param.ViewType) as Window;
            if (view == null) return;
            ClientLib.ShowDialogView(view, this);
        }
        [RelayCommand]
        void DoLogin()
        {
            ClientLib.ShowDialogView(new Views.LoginView(), this);
        }
        [RelayCommand]
        void DoSetting()
        {
            ClientLib.ShowDialogView(new Views.SystemSettingView(), this);
        }
        #region テストしたいときに使う Shift+F11で実行
        [RelayCommand]
        void DoTest()
        {
            test_DecodeQueryString();
        }
        /// <summary>
        /// Biz/DesignerでのBrowserからサーバへの送信内容をデコードし解析するためのテストコード encodedに通信ログの送信内容を貼り付ける
        /// </summary>
        void test_DecodeQueryString()
        {
            string encoded = """
SELECT%20A.*%20FROM%20(select%20A.SEQ_NO,A.VDATE_CREATE
""";
            if (AppData.Http != null)
            {
                string decoded = System.Web.HttpUtility.UrlDecode(encoded, AppData.Http.DefaultEncode);
                //Debug.WriteLine("***** HTTPのquery文字列のデコード *****");
                //Debug.WriteLine(decoded);
            }
        }
        #endregion
        /// <summary>
        /// ログイン処理の前にデバッグ用の処理を追加する
        /// </summary>
        void debugPreLogin()
        {

        }
        /// <summary>
        /// ログイン処理の後にデバッグ用の処理を追加する
        /// </summary>
        void debugPostLogin()
        {
            
        }

        [RelayCommand]
        void GoHome() => CurrentPage = new HomePage();

        [RelayCommand]
        void GoReport() => CurrentPage = new ReportPageView();

        [RelayCommand]
        void GoSettings() => CurrentPage = new SettingPage();
        [RelayCommand]
        void ShowProfile()
        {
            System.Windows.MessageBox.Show("Profile clicked!");
        }

        [RelayCommand]
        void Logout()
        {
            System.Windows.MessageBox.Show("Logout clicked!");
        }

        public void OnNavigated(NavigationEventArgs e)
        {
            if (e.Content is Page page)
            {
                var fade = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(400),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };
                page.BeginAnimation(Page.OpacityProperty, fade);
            }
        }

    }
}
