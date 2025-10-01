using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using System.Collections.ObjectModel;
using System.Data;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg80gphSelViewModel : BaseViewModel
    {  
        [ObservableProperty]
        string? title = "選択画面";
         
        List<SubDlg80gphSelModel>? List80gphOri;

        [ObservableProperty]
        BizArray wrk_para = null;

        [ObservableProperty]
        ObservableCollection<SubDlg80gphSelModel> list80gph;

        [ObservableProperty]
        SubDlg80gphSelModel? select80gphItem;

        [ObservableProperty]
        string? text80gphSel;

        [RelayCommand]
        void Init()
        {
            if (wrk_para == null) return;
            List80gphOri = new List<SubDlg80gphSelModel>(Init(wrk_para));
        }

        List<SubDlg80gphSelModel> Init(BizArray init_para = null, string v_hugo = "", string v_sort = "")
        {
            var _list = new List<SubDlg80gphSelModel>();
            if (v_hugo == "") v_hugo = ">=";
            string sql_query = " select 名称CD,名称,略称 from  hc$master_meisho ";
            sql_query += " where 名称区分=:1 and 名称CD" + v_hugo + ":2 order by 名称CD " + v_sort;
            var v_para = new BizArray();
            if (init_para.Count > 0)
            {
                if (init_para[0] == "HC$MASTER_MEISHO")
                { 
                    Title = "名称を選択して下さい";
                    sql_query = " select 名称CD,名称,略称 from  hc$master_meisho "; 
                    if (!string.IsNullOrEmpty(init_para[1]) || 
                        !string.IsNullOrEmpty(init_para[2]))
                    {
                        sql_query += " where ";
                        if (!string.IsNullOrEmpty(init_para[1]))
                        {
                            sql_query += " 名称区分=:1 ";
                            v_para.Set(0, init_para[1]);
                        }
                        if (!string.IsNullOrEmpty(init_para[2]))
                        {
                            sql_query += " and 名称CD" + v_hugo + ":2 ";
                            v_para.Set(1, init_para[2]);
                        }
                    }
                    sql_query += "order by 名称CD " + v_sort;
                }
                else if (init_para[0] == "HC$MASTER_TOKUI")
                { 
                    sql_query = " select 得意先CD,得意先名,カナ from  hc$master_tokui ";
                    sql_query += " where 得意先CD" + v_hugo + ":1 ";
                    if (init_para.Count > 2 && init_para[3] != null) sql_query += init_para[3];
                    sql_query += " order by 得意先CD " + v_sort; 
                    v_para.Set(0, init_para[2]); 
                }
                else if (init_para[0] == "HC$MASTER_SIIRE")
                {
                    sql_query = "select 仕入先CD,仕入先名,カナ from  hc$master_siire ";
                    sql_query += " where 仕入先CD" + v_hugo + ":1 ";
                    if (init_para.Count > 2 && init_para[3] != null) sql_query += init_para[3];
                    sql_query += " order by 仕入先CD " + v_sort;
                    v_para.Set(0, init_para[2]);
                }
                else if (init_para[0] == "HC$MASTER_SHOHIN")
                { 
                    sql_query = "select 商品CD,商品名,略称 from  hc$master_shohin ";
                    sql_query += " where 商品CD" + v_hugo + ":1 order by 商品CD " + v_sort;
                    v_para.Set(0, init_para[2]);
                }
                else if (init_para[0] == "HC$MASTER_SHAIN")
                { 
                    sql_query = "select 社員CD,名前 from  hc$master_shain ";
                    sql_query += " where 社員CD" + v_hugo + ":1 and 営業FLG = 1 order by 社員CD " + v_sort;
                    v_para.Set(0, init_para[2]);
                }
                else if (init_para[0] == "HC$MASTER_KOKYAKU")
                { 
                    sql_query = "select 顧客CD,顧客名 from  hc$master_kokyaku ";
                    sql_query += " where 顧客CD" + v_hugo + ":1 order by 顧客CD " + v_sort;
                    v_para.Set(0, init_para[2]);
                }
                sql_query = AppData.ClassCvnet.GetSqlDisp(sql_query);
                var ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para.ToArray()); 
                foreach (DataRow row in ret_csv.Rows)
                {
                    var model = new SubDlg80gphSelModel
                    {
                        Code = row[0] != DBNull.Value ? row[0].ToString().Trim() : string.Empty,
                        Name = row[1] != DBNull.Value ? row[1].ToString().Trim() : string.Empty,
                        Abbreaviate = row.Table.Columns.Count > 2 && row[2] != DBNull.Value ? row[2].ToString() : string.Empty,
                    };
                    _list.Add(model);
                }
                if (_list.Count > 0 && !string.IsNullOrEmpty(v_sort))
                    _list = _list.OrderBy(x => x.Code).ToList(); 
                List80gph = new ObservableCollection<SubDlg80gphSelModel>(_list); 
            }
            return _list;
        }

        [RelayCommand]
        void PrevList()
        {
            if (List80gph != null && List80gph.Count == 0) return;
            string v_hugo = "<="; string v_sort = "desc"; 
            wrk_para.Set(2, List80gph.FirstOrDefault().Code);
            Init(wrk_para, v_hugo, v_sort);
        }

        [RelayCommand]
        void NextList()
        {
            if (List80gph != null && List80gph.Count == 0) return;
            string v_hugo = ">="; string v_sort = "";
            wrk_para.Set(2, List80gph.LastOrDefault().Code);
            Init(wrk_para, v_hugo, v_sort);
        }

        [RelayCommand]
        void TopList()
        {
            if (List80gphOri != null && List80gphOri.Count == 0) return; 
            List80gph = new ObservableCollection<SubDlg80gphSelModel>(List80gphOri);
        }

        [RelayCommand]
        void NameSearch()
        { 
            if (string.IsNullOrEmpty(Text80gphSel))
                List80gph = new ObservableCollection<SubDlg80gphSelModel>(List80gphOri);
            else List80gph = new ObservableCollection<SubDlg80gphSelModel>(List80gph.Where(x => x.Name.Contains(Text80gphSel)));
        }

        [RelayCommand]
        void DoSearch()
        {
            if (Select80gphItem == null) return;
            //confirm and close window
            ClientLib.ExitDialogResult(this, true);
        }

        [RelayCommand]
        void DoExit()
        {
            //confirm and close window
            ClientLib.ExitDialogResult(this, true);
        }
    }
}
