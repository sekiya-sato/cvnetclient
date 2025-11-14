using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;

namespace CvnetClient.ViewModels
{
    public partial class DynamicPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private string pageTitle;

        public ObservableCollection<ButtonConfig> Buttons { get; } = new();

        public DynamicPageViewModel(string pageTitle, string pageId)
        {
            PageTitle = pageTitle;

            var config = PageConfig.GetPage(pageId);
            foreach (var btn in config.Buttons)
            {
                Buttons.Add(btn);
            }
        }

        [RelayCommand]
        private void ButtonClick(ButtonConfig cfg)
        {
            try
            {
                var type = Type.GetType(cfg.WindowId);
                if (type == null)
                {
                    MessageBox.Show($"Window class '{cfg.WindowId}' not found.");
                    return;
                }

                var existing = Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w.GetType() == type);

                if (existing != null)
                {
                    if (existing.WindowState == WindowState.Minimized)
                        existing.WindowState = WindowState.Normal;

                    existing.Activate();
                    return;
                }
                else
                {
                    var window = (Window)Activator.CreateInstance(type)!;
                    window.Title = cfg.Text;

                    if (window.DataContext is BaseViewModel vm )
                    {
                        var method = vm.GetType().GetMethod("OnInit");
                        if (method != null)
                        {
                            var parameters = method.GetParameters();
                            if (parameters.Length == 0)
                            {
                                method.Invoke(vm, null);
                            }
                            else
                            {
                                method.Invoke(vm, new object?[] { cfg.Parameter,cfg.Flag });
                            }
                        }
                    }

                    window.Show();
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading window: {ex.Message}");
            }
        }

        [RelayCommand]
        private void Back()
        {
            var nav = System.Windows.Application.Current.MainWindow
                ?.FindName("MainFrame") as System.Windows.Controls.Frame;

            if (nav != null && nav.CanGoBack)
                nav.GoBack();
        }
    }
}
