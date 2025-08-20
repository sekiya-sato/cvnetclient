using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvnetClient.Models
{
    /// <summary>
    /// View Class - Sel00View
    /// </summary>
    public partial class Sel00Model : ObservableObject
    {
        [ObservableProperty]
        long code;
        [ObservableProperty]
        string? name;
    }
}
