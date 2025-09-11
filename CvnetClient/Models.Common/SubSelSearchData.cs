using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvnetClient.Models
{
    /// <summary>
    /// View Class - SubDlgSelShoView
    /// </summary>
    public partial class Sel00Model : ObservableObject
    {
        [ObservableProperty]
        string? code;
        [ObservableProperty]
        string? name;
    }

    /// <summary>
    /// View Class - SubDlg80gphSelView
    /// </summary>
    public partial class SubDlg80gphSelModel : ObservableObject
    {
        [ObservableProperty]
        string? code;
        [ObservableProperty]
        string? name;
        [ObservableProperty]
        string? abbreaviate;
    }
}
