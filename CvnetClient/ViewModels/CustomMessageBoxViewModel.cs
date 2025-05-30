using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace CvnetClient.ViewModels
{
    public partial class CustomMessageBoxViewModel : ObservableObject
    {
        [ObservableProperty]
        string message;

        [ObservableProperty]
        string title;

        [ObservableProperty]
        double fontSize = 16;

		[ObservableProperty]
		string btnOkText = "OK";
		[ObservableProperty]
		string btnCancelText = "キャンセル";

		[ObservableProperty]
		bool btnOkEnabled = true;
		[ObservableProperty]
		bool btnCancelEnabled = true;

		public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public CustomMessageBoxViewModel(string message, string title = "メッセージ", double fontSize = 16)
        {
            Message = message;
            Title = title;
            FontSize = fontSize;
            OkCommand = new RelayCommand(OnOk);
            CancelCommand = new RelayCommand(OnCancel);
        }

        public event Action<bool?>? RequestClose;

        void OnOk() => RequestClose?.Invoke(true);
        void OnCancel() => RequestClose?.Invoke(false);
    }
}