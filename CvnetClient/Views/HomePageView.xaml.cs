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

namespace CvnetClient.Views
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                string title = btn.Content.ToString();
                string pageId = btn.Tag?.ToString();

                // Navigate ke DynamicPage sambil pass Title & PageId
                this.NavigationService?.Navigate(new DynamicPageView(title, pageId));
            }
        }
    }
}