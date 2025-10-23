using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvnetClient.ViewModels
{
    public enum OutPutType { 正規商品, 中止商品CD, 全て }
    public enum BarcodeType2 { JAN,CODE39, NW7 }
    public partial class SubDlg05PrnBcbookViewModel : BaseViewModel
    {
        public void OnInit() 
        { 
        
        }
    }

    public partial class Condition : ObservableObject 
    { 
    
    
    }
}
