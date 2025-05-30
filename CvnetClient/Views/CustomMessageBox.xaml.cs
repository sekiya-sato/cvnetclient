using CvnetClient.ViewModels;
using System;
using System.Windows;

namespace CvnetClient.Views {
    public partial class CustomMessageBox : Window {
        public CustomMessageBox(string message, string title, double fontSize) {
            InitializeComponent();
            var vm = new CustomMessageBoxViewModel(message, title, fontSize);
            vm.RequestClose += result => {
                DialogResult = result;
                Close();
            };
            DataContext = vm;
        }
    }
}