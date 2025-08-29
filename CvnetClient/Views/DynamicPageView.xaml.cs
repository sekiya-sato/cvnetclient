using System.Windows.Controls;
using CvnetClient.ViewModels;

namespace CvnetClient.Views
{
    public partial class DynamicPageView : Page
    {
        public DynamicPageView(string pageTitle, string pageId)
        {
            InitializeComponent();
            this.DataContext = new DynamicPageViewModel(pageTitle, pageId);
        }
    }
}
