using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using CvnetClient.Models;

namespace CvnetClient.Views
{
    /// <summary>
    /// Interaction logic for SettingPage.xaml
    /// </summary>
    public partial class SettingPage : Page
    {
        public List<ThemeOption> Themes { get; set; }

        public SettingPage()
        {
            InitializeComponent();

            //Themes = new List<ThemeOption>
            //{
            //    new ThemeOption { Name = "Light", File = "Resources/Light.xaml" },
            //    new ThemeOption { Name = "Dark", File = "Resources/Dark.xaml" },
            //    new ThemeOption { Name = "Ocean", File = "Resources/Ocean.xaml" }
            //};

            //ThemeComboBox.ItemsSource = Themes;
            //ThemeComboBox.DisplayMemberPath = "Name";
            //ThemeComboBox.SelectedIndex = 0;
        }

        //private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (ThemeComboBox.SelectedItem is ThemeOption selectedTheme)
        //    {
        //        var dict = new ResourceDictionary { Source = new System.Uri(selectedTheme.File, System.UriKind.Relative) };

        //        Application.Current.Resources.MergedDictionaries.Clear();
        //        Application.Current.Resources.MergedDictionaries.Add(dict);
        //    }
        //}
    }
}
