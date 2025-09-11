using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Class;
using CvnetClient.ViewModels.Component;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgSelShoViewModel : BaseViewModel
    {
        [ObservableProperty]
        ObservableCollection<ListFlexItem>? listFlex = new ObservableCollection<ListFlexItem>();

        [ObservableProperty]
        ListFlexConfig? listConfig;

        [ObservableProperty]
        string? whereClaus;

        [RelayCommand]
        void Init()
        {
            ListConfig = new ListFlexConfig();
            ListConfig.col_alias = "a.";
        }

        string GetConvKubun(string kubun)
        {
            string cd_name = "";
            if (kubun == "ITM")
            {
                cd_name = "アイテムCD";
            }
            else if (kubun == "BRD")
            {
                cd_name = "ブランドCD";
            }
            else if (kubun == "DZN")
            {
                cd_name = "デザイナーCD";
            }
            else if (kubun == "SZN")
            {
                cd_name = "シーズンCD";
            }
            else if (kubun == "TNJ")
            {
                cd_name = "展示会CD";
            }
            else if (kubun == "SZI")
            {
                cd_name = "素材CD";
            }
            else if (kubun == "MKR")
            {
                cd_name = "メーカーCD";
            }
            else if (kubun == "GEN")
            {
                cd_name = "原産国CD";
            }
            else
            {
                if (Regex.IsMatch(kubun, @"B[0-9]{2}"))
                {
                    cd_name = "名称CD" + kubun.Substring(1, 2);
                }
            }
            return cd_name;
        }
    }
}
