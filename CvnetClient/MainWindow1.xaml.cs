using CvnetClient.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CvnetClient
{
    /// <summary>
    /// Interaction logic for MainWindow1.xaml
    /// </summary>
    public partial class MainWindow1 : Window
    {
        public MainWindow1()
        {
            InitializeComponent();
        }

        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new HomePage());
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SettingPage());
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigated += MainFrame_Navigated;
            BtnHome_Click(BtnHome, null);
        }

        private void SidebarButton_Click(object sender, RoutedEventArgs e)
        {
            var clicked = sender as ToggleButton;

            foreach (var child in Sidebar.Children)
            {
                if (child is ToggleButton btn && btn != clicked)
                {
                    btn.IsChecked = false;
                }
            }

            if (clicked.Content.ToString() == "🏠")
            {
                MainFrame.Navigated += MainFrame_Navigated;
                MainFrame.Navigate(new HomePage());
            }
            else if (clicked.Content.ToString() == "📊")
            {
                MainFrame.Navigated += MainFrame_Navigated;
                MainFrame.Navigate(new SettingPage());
            }
            else if (clicked.Content.ToString() == "⚙")
            {
                MainFrame.Navigated += MainFrame_Navigated;
                MainFrame.Navigate(new SettingPage());
            }
        }

        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.ContextMenu != null)
            {
                btn.ContextMenu.PlacementTarget = btn;
                btn.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
                btn.ContextMenu.IsOpen = true;
            }
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Profile clicked!");
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Logout clicked!");
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            var fade = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(400),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            if (MainFrame.Content is Page page)
            {
                page.BeginAnimation(UIElement.OpacityProperty, fade);
            }
        }


    }
}
