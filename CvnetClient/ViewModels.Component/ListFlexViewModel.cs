using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvnetClient.ViewModels.Component
{
    public class ListFlexItem
    {
        public int No { get; set; }
        public string SelectedItem { get; set; } = "";
        public string FromValue { get; set; } = "";
        public string ToValue { get; set; } = "";
    }
}
