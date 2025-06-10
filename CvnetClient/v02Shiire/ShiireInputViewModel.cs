using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;

namespace CvnetClient.ViewModels
{
    public partial class ShiireInputViewModel : ObservableObject
    {
        // 検索条件
        [ObservableProperty] string? denpyoNoFrom;
        [ObservableProperty] string? denpyoNoTo;
        [ObservableProperty] DateTime? shiireDateFrom;
        [ObservableProperty] DateTime? shiireDateTo;
        [ObservableProperty] ObservableCollection<string>? torihikiKubunList = new() { "10 仕入", "20 返品" };
        [ObservableProperty] string? selectedTorihikiKubun;
        [ObservableProperty] string? manualInputNo;
        [ObservableProperty] string? shiireSakiCode;
        [ObservableProperty] string? shiireSakiName;
        [ObservableProperty] bool isShuukei;
        [ObservableProperty] bool isMishouninOnly = true;
        [ObservableProperty] bool isAll;
        [ObservableProperty] string pageInfo = "1 / 1";

        // DataGrid
        [ObservableProperty] ObservableCollection<ShiireRow> shiireList = new();
        [ObservableProperty] ShiireRow? selectedShiire;

        // 下部
        [ObservableProperty] bool isListMode = true;
        [ObservableProperty] bool isDetailMode;
        [ObservableProperty] string statusMessage = "リスト選択行データ取得";

        // コマンド
        [RelayCommand] void OpenSearch() { /* 実装 */ }
        [RelayCommand] void OpenEdit() { /* 実装 */ }
        [RelayCommand] void SelectShiireSaki() { /* 実装 */ }
        [RelayCommand] void Search() { /* 実装 */ }
        [RelayCommand] void UpdateSelectedFlag() { /* 実装 */ }
        [RelayCommand] void Print() { /* 実装 */ }
        [RelayCommand] void ExportCsv() { /* 実装 */ }
        [RelayCommand] void Edit() { /* 実装 */ }
        [RelayCommand] void Delete() { /* 実装 */ }
        [RelayCommand] void Close() { /* 実装 */ }
    }

    // DataGrid用の行データ
    public partial class ShiireRow : ObservableObject
    {
        [ObservableProperty] bool isUpdated;
        [ObservableProperty] string? denpyoNo;
        [ObservableProperty] string? shiireDate;
        [ObservableProperty] string? shiireSaki;
        [ObservableProperty] string? shiireSakiName;
        [ObservableProperty] string? sokoName;
        [ObservableProperty] string? torihikiKubun;
        [ObservableProperty] string? suuryou;
        [ObservableProperty] string? kingaku;
        [ObservableProperty] string? kanrenNo1;
    }
}