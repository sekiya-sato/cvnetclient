using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Converters;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01MeiConvViewModel : BaseViewModel
    {

        [ObservableProperty]
        ObservableCollection<Templ>? copyFrom;
        [ObservableProperty]
        ObservableCollection<Templ>? copyTo;
        [ObservableProperty]
        Templ? selectedCopyFrom;
        [ObservableProperty]
        Templ? selectedCopyTo;
        private BizArray para;
        private BizArray v_flg;

        string sql = """"
            select 名称CD,名称CD||' '||名称,'HC$MASTER_MEISHO' マスタ名,'名称区分='''||名称CD||'''' aaa from hc$master_meisho where 名称区分='IDX' order by 名称CD
            """";
        public void OnInit(object? init_para = null, object? init_flg = null) 
        {
            if (init_para != null)
            {
                if (init_para is string s)
                {
                    var v_para = new string[] { s };
                    para = new BizArray(v_para);
                }
                else if (init_para is string[] arr)
                    para = new BizArray(arr);
                else para = new BizArray();
            }
            else para = new BizArray();

            if (init_flg != null)
            {
                if (init_flg is string s)
                {
                    var v_para = new string[] { s };
                    v_flg = new BizArray(v_para);
                }
                else if (init_flg is string[] arr)
                    v_flg = new BizArray(arr);
                else v_flg = new BizArray();
            }
            else v_flg = new BizArray();

            var retData = AppData.Http?.AspxSqlQuery(sql, new string[] {});
            if (retData == null || retData.Rows.Count == 0) return;
            var list = (from DataRow dr in retData.Rows
                        select new Templ
                        {
                            Value = dr["名称CD"].ToString() ?? string.Empty,
                            Title = dr["名称CD||''||名称"].ToString() ?? string.Empty,
                            Tb_name = dr["マスタ名"].ToString() ?? string.Empty,
                            Where_str = dr["aaa"].ToString() ?? string.Empty
                        }).ToList();
            Common.ConvertDotStringDel(list);
            CopyTo = new ObservableCollection<Templ>(list);
            
            var staticList = new List<Templ>
            {
                new Templ { Title = "得意先マスタ", Tb_name = "HC$MASTER_TOKUI" },
                new Templ { Title = "仕入マスタ", Tb_name = "HC$MASTER_SIIRE" },
                new Templ { Title = "商品マスタ", Tb_name = "HC$MASTER_SHOHIN" },
                new Templ { Title = "委託倉庫", Tb_name = "HC$MASTER_TOKUI", Where_str = "店種区分=0 and 倉庫区分=2" },
                new Templ { Title = "店舗", Tb_name = "HC$MASTER_TOKUI", Where_str = "店種区分 in (3,6)" },
                new Templ { Title = "社員マスタ", Tb_name = "HC$MASTER_SHAIN" }
            };
            list.InsertRange(0, staticList);
            CopyFrom = new ObservableCollection<Templ>(list);
        }

        [RelayCommand]
        void DoCopy() 
        {
            if (SelectedCopyFrom == null) return;
            if(SelectedCopyTo == null) return;
            if (!ClientLib.MessageBox(this, SelectedCopyFrom.Title + "から" + SelectedCopyTo.Title + "にコピーを実行します\nよろしいですか？")) return;

            string[] v_para = new string[4];
            v_para[0] = SelectedCopyFrom.Tb_name;
            var col_str = string.Empty;
            if (v_para[0] == "HC$MASTER_TOKUI") 
            {
                col_str = "得意先CD,得意先名,略称";
            }
            else if (v_para[0] == "HC$MASTER_SIIRE")
            {
                col_str = "仕入先CD,仕入先名,略称";
            }
            else if (v_para[0] == "HC$MASTER_SHOHIN")
            {
                col_str = "商品CD,商品名";
            }
            else if (v_para[0] == "HC$MASTER_MEISHO")
            {
                col_str = "名称CD,名称,略称";
            }
            else if (v_para[0] == "HC$MASTER_SHAIN")
            {
                col_str = "社員CD,名前,フリガナ";
            }
            v_para[1] = col_str;
            v_para[2] = SelectedCopyFrom.Where_str;
            v_para[3] = SelectedCopyTo.Value;

            var ret = AppData.Http!.AspxSqlQuery2("Mei_Copy", v_para, "", -1);
            if (ret.Split(',')[0].Trim() != "0")
            {
                ClientLib.MessageBoxError(this, "更新エラー");
            }
            else {
                ClientLib.MessageBoxOk(this, "正常更新終了");
            }
            
        }

        public partial class Templ : ObservableObject
        {

            [ObservableProperty]
            public string? value;

            [ObservableProperty]
            public string? title;

            [ObservableProperty]
            public string? tb_name;

            [ObservableProperty]
            public string? where_str;            
        }
    }
}
