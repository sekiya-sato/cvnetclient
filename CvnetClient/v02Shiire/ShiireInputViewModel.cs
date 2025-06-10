using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;

namespace CvnetClient.ViewModels
{
    public partial class ShiireInputViewModel : ObservableObject
    {
        // ��������
        [ObservableProperty] string? denpyoNoFrom;
        [ObservableProperty] string? denpyoNoTo;
        [ObservableProperty] DateTime? shiireDateFrom;
        [ObservableProperty] DateTime? shiireDateTo;
        [ObservableProperty] ObservableCollection<string>? torihikiKubunList = new() { "10 �d��", "20 �ԕi" };
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

        // ����
        [ObservableProperty] bool isListMode = true;
        [ObservableProperty] bool isDetailMode;
        [ObservableProperty] string statusMessage = "���X�g�I���s�f�[�^�擾";

        // �R�}���h
        [RelayCommand] void OpenSearch() { /* ���� */ }
        [RelayCommand] void OpenEdit() { /* ���� */ }
        [RelayCommand] void SelectShiireSaki() { /* ���� */ }
        [RelayCommand] void Search() { /* ���� */ }
        [RelayCommand] void UpdateSelectedFlag() { /* ���� */ }
        [RelayCommand] void Print() { /* ���� */ }
        [RelayCommand] void ExportCsv() { /* ���� */ }
        [RelayCommand] void Edit() { /* ���� */ }
        [RelayCommand] void Delete() { /* ���� */ }
        [RelayCommand] void Close() { /* ���� */ }
    }

    // DataGrid�p�̍s�f�[�^
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