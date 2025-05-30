using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text.Json;
using System.Windows.Input;

namespace CvnetClient.ViewModels
{
    public partial class SystemSettingViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? url;

        [ObservableProperty]
        private string? userId;

        [ObservableProperty]
        private string? password;

        [RelayCommand]
        void Init()
        {
            // appsettings.jsonが存在すれば読み込む
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            if (File.Exists(filePath))
            {
                try
                {
					var settingfile = "appsettings.json";
					// 設定ファイル読み込み
					IConfiguration config = new ConfigurationBuilder()
						.AddJsonFile(settingfile, optional: true, reloadOnChange: true)
						.Build();
                    var appConfig = config.GetSection("AppSetting");
                    Url = appConfig["Url"];
					UserId = appConfig["LoginId"];
					Password = appConfig["LoginPass"];
				}
				catch
                {
                    // 必要に応じてエラー処理
                }
            }
        }

        [RelayCommand]
        void Ok()
        {
            var settings = new
            {
                AppSetting = new
                {
                    Url = Url ?? "",
                    LoginId = UserId ?? "",
                    LoginPass = Password ?? ""
                }
            };

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(settings, options);
            using (var writer = new StreamWriter(filePath, false))
            {
                writer.Write(json);
            }
            ClientLib.MessageBox(this, "設定を保存しました。アプリケーションを再起動してください。", "設定保存完了");
            ClientLib.Exit(this);
		}

        [RelayCommand]
        void Cancel()
        {
            ClientLib.Exit(this);
        }
    }
}