using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CvnetClient.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CvnetClient.Views
{
    /// <summary>
    /// Interaction logic for DynamicPageView.xaml
    /// </summary>
    public partial class DynamicPageView : System.Windows.Controls.Page
    {
        private readonly string _pageTitle;
        private readonly string _pageId;
        public DynamicPageView(string pageTitle, string pageId)
        {
            InitializeComponent();
            _pageTitle = pageTitle;
            _pageId = pageId;
            LoadPage();
        }
        private void LoadPage()
        {
            PageTitle.Text = _pageTitle;

            ButtonPanel.Children.Clear();
            var config = PageConfig.GetPage(_pageId);

            foreach (var btn in config.Buttons)
            {
                var button = new System.Windows.Controls.Button
                {
                    Content = btn.Text,
                    Style = (Style)FindResource("MenuButtonStyle"),
                    Tag = btn
                };
                button.Click += SubButton_Click;
                ButtonPanel.Children.Add(button);
            }
        }

        private void SubButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.Tag is ButtonConfig cfg)
            {
                try
                {
                    // Load type dari nama class (string)
                    var type = Type.GetType(cfg.WindowId);
                    if (type == null)
                    {
                        MessageBox.Show($"Window class '{cfg.WindowId}' not found.");
                        return;
                    }

                    // Create instance
                    var window = (System.Windows.Window)Activator.CreateInstance(type)!;
                    window.Title = cfg.Text;
                    window.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading window: {ex.Message}");
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }
    }
}
