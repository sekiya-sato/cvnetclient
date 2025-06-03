/* ============================================================================
 * Cvnet8wpfclient.exe : CustomMessageBoxViewModel.cs
 * Created by Sekiya.Sato 2025/06/02
 * ����: �ėp���b�Z�[�W�{�b�N�X
 * �g�p���C�u���� [Library used]:
 *		CommunityToolkit.Mvvm : LICENCE = MIT
 * ============================================================================  */
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CvnetClient.ViewModels;

public partial class CustomMessageBoxViewModel : ObservableObject {
    [ObservableProperty]
    string? message;

    [ObservableProperty]
    string? title;

    [ObservableProperty]
    double fontSize = 16;

    [ObservableProperty]
    string btnOkText = "OK";
    [ObservableProperty]
    string btnCancelText = "�L�����Z��";

    [ObservableProperty]
    bool btnOkEnabled = true;
    [ObservableProperty]
    bool btnCancelEnabled = true;

    [RelayCommand]
    void Ok() {
			ClientLib.ExitDialogResult(this, true);
		}

		[RelayCommand]
    void Cancel() {
        ClientLib.Exit(this);
		}
}