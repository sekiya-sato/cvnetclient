using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static CvnetClient.ViewModels.SubDlgInp13ViewModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg13Hbnpatten150ViewModel : BaseViewModel
    {
        [ObservableProperty]
        SearchCondition? condition;
        string sql_collist = "パターンNO,パターン名,ブランドCD,入力社員CD";
        string sql_collist2 = "得意先CD,メモ,配分率";
        string[] col_list;
        string[] col_list2;
        [ObservableProperty]
        ObservableCollection<PatternHeader>? patternList;
        [ObservableProperty]
        ObservableCollection<PatternDetail>? patternDetailList;
        [ObservableProperty]
        PatternHeader? selectedPattern;
        [ObservableProperty]
        PatternHeader? editPattern;
        private int Bcheck_flg = 0;
        public void OnInit() 
        {
            Condition = new SearchCondition();
            Condition.BrdCDTo = new BtListHelper("99999999","");
            Condition.PatternNoFrom = 0;
            Condition.PatternNoTo = 9999999999;
            col_list = sql_collist.Split(',');
            col_list2 = sql_collist2.Split(',');
        }
        partial void OnSelectedPatternChanged(PatternHeader? value)
        {
            if (value != null)
            {
                EditPattern = Common.CloneObject(value);
                var para = new string[1];
                para[0] = value.PatternNO.ToString();
                OnQueryDetail(para);
            }               
            else
                EditPattern = null;
        }
        [RelayCommand]
        public void DoList() 
        {
            var para = new string[4];
            para[0] = Condition?.PatternNoFrom.ToString() ?? string.Empty;
            para[1] = Condition?.PatternNoTo.ToString() ?? string.Empty;
            para[2] = Condition.BrdCDFrom?.Code ?? string.Empty;
            para[3] = Condition.BrdCDTo?.Code ?? string.Empty;
            OnQuery(para);
        }
        [RelayCommand]
        void BackList()
        {
            long startcd = 0;
            if (PatternList != null && PatternList.Count > 0)
            {
                startcd = PatternList.Min(c => c.PatternNO);
            }
            var para = new string[4];
            para[0] = Condition?.PatternNoFrom.ToString() ?? string.Empty;
            para[1] = Condition?.PatternNoTo.ToString() ?? string.Empty;
            para[2] = Condition.BrdCDFrom?.Code ?? string.Empty;
            para[3] = Condition.BrdCDTo?.Code ?? string.Empty;
            OnQuery(para,startcd);
            if (PatternList == null || PatternList.Count == 0)
                ClientLib.MessageBoxOk(this, "データがありません");
        }
        [RelayCommand]
        void NextList()
        {
            long startcd = 0;
            if (PatternList != null && PatternList.Count > 0)
            {
                startcd = PatternList.Max(c => c.PatternNO);
            }
            var para = new string[4];
            para[0] = Condition?.PatternNoFrom.ToString() ?? string.Empty;
            para[1] = Condition?.PatternNoTo.ToString() ?? string.Empty;
            para[2] = Condition.BrdCDFrom?.Code ?? string.Empty;
            para[3] = Condition.BrdCDTo?.Code ?? string.Empty;
            OnQuery(para,startcd, ">=");
            if (PatternList == null || PatternList.Count == 0)
                ClientLib.MessageBoxOk(this, "データがありません");
        }
        void OnQuery(string[] v_para, long? wrk_para = null , string? p_sort = "") 
        {
            var v_hugo = "<=";
            if (p_sort != "") v_hugo = ">=";

            var sql_query = "select max(A.SEQ_NO) SEQ_NO, max(A.VDATE_CREATE) VDATE_CREATE, max(A.VDATE_UPDATE) VDATE_UPDATE,";
            for (var i = 0; i < col_list.Length; i++) sql_query += "A." + col_list[i] + ",";
            sql_query += "NVL((SELECT m.名称 FROM HC$MASTER_MEISHO m WHERE m.名称区分='BRD' and m.名称CD=A.ブランドCD),'') ブランド名,";
            sql_query += "NVL((SELECT s.名前 FROM HC$MASTER_SHAIN s WHERE s.社員CD=A.入力社員CD),'') 入力社員名";
            sql_query += " from HC$MASTER_HYUHIBN_PTN A";
            sql_query += " where A.パターンNO between :1 and :2 and A.ブランドCD between :3 and :4";
            if (wrk_para != null)
            {
                v_para[v_para.Length] = wrk_para.ToString();
                sql_query += " and A.SEQ_NO " + v_hugo + " :" + v_para.Length.ToString();
            }
            sql_query += " group by A.パターンNO, A.パターン名, A.ブランドCD, A.入力社員CD";
            sql_query += " order by A.パターンNO";

            sql_query = AppData.ClassCvnet.GetSqlDisp(sql_query);

            var ret_csv = AppData.Http!.AspxSqlQuery(sql_query, v_para);
            if (ret_csv == null || ret_csv.Rows.Count == 0)
            {
                ClientLib.MessageBox(this, "データがありません!");
                return;
            }
            var list = (from DataRow dr in ret_csv.Rows
                        select new PatternHeader
                        {
                            SeqNO = Convert.ToInt64(dr["SEQ_NO"]),
                            VdateCreate = Convert.ToDecimal(dr["VDATE_CREATE"]),
                            VdateUpdate = Convert.ToDecimal(dr["VDATE_UPDATE"]),
                            PatternNO = Convert.ToInt64(dr["パターンNO"]),
                            PatternName = dr["パターン名"].ToString() ?? string.Empty,
                            Brand = new BtListHelper(dr["ブランドCD"].ToString(), dr["ブランド名"].ToString()),
                            EntEmp = new BtListHelper(dr["入力社員CD"].ToString(), dr["入力社員名"].ToString())

                        }).OrderByDescending(c => c.SeqNO).ToList();
            Common.ConvertDotStringDel(list);
            PatternList = new ObservableCollection<PatternHeader>(list);
            if (PatternList.Count > 0)
            {
                SelectedPattern = PatternList[0];
            }
        }
        void OnQueryDetail(string[] v_para) 
        {
            var sql_query = "select ";
            for (var i = 0; i < col_list2.Length; i++) sql_query += "A." + col_list2[i] + ",";
            sql_query += " NVL((SELECT t.得意先名 FROM HC$MASTER_TOKUI t WHERE t.得意先CD=A.得意先CD),'') 店舗名";
            sql_query += " from HC$MASTER_HYUHIBN_PTN A";
            sql_query += " where A.パターンNO=:1 order by A.得意先CD";

            var ret_data = AppData.Http!.AspxSqlQuery(sql_query, v_para);
            if (ret_data == null || ret_data.Rows.Count == 0)
            {
                ClientLib.MessageBox(this, "データがありません!");
                return;
            }
            var list = (from DataRow dr in ret_data.Rows
                        select new PatternDetail
                        {        
                            Cust = new BtListHelper(dr["得意先CD"].ToString(), dr["店舗名"].ToString()),
                            Memo = Convert.ToDecimal(dr["メモ"]),
                            AllocRate = Convert.ToDecimal(dr["配分率"]),

                        }).OrderByDescending(c => c.Cust.Code).ToList();
            Common.ConvertDotStringDel(list);
            PatternDetailList = new ObservableCollection<PatternDetail>(list);
            
        }
        [RelayCommand]
        void CellEdited() 
        {
            Hibnritsu();
        }
        void Hibnritsu()
        {
            if (PatternDetailList.Count < 1) return;
            decimal kei = 0;

            for (var i = 0; i < PatternDetailList.Count; i++) {
                kei += PatternDetailList[i].Memo;
            }
            if (kei == 0) return;

            for (var i = 0; i < PatternDetailList.Count; i++)
            {
                PatternDetailList[i].AllocRate = Math.Round((PatternDetailList[i].Memo / kei) * 100, 2);
            }
        }

        [RelayCommand]
        public void SelBrd1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                Condition.BrdCDFrom = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelBrd2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                Condition.BrdCDTo = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelBrd(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditPattern != null)
            {
                EditPattern.Brand = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }        
        [RelayCommand]
        public void SelUser(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditPattern != null)
            {
                EditPattern.EntEmp = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelTenpo(object value)
        {
            if (value is not PatternDetail row)
                return;
            int index = PatternDetailList.IndexOf(row);
            
            var get_sel00 = AppData.DlgService.GetSelTok();
            if(get_sel00 != null && get_sel00.SelTokResult0 != null)
            {
                PatternDetailList[index].Cust =new BtListHelper(get_sel00.SelTokResult0[0], get_sel00.SelTokResult0[1]);
            }
        }
        [RelayCommand]
        void AddRow() 
        {
            if (PatternDetailList == null)
                PatternDetailList = new ObservableCollection<PatternDetail>();

            var newRow = new PatternDetail
            {
                Cust = new BtListHelper("", ""),
                Memo = 0,
                AllocRate = 0
            };

            PatternDetailList.Add(newRow);
        }
        [RelayCommand]
        void ClearRow() 
        {
            if (!ClientLib.MessageBox(this, "クリアしますか？")) return;
            PatternDetailList = new ObservableCollection<PatternDetail> { };
        }
        [RelayCommand]
        void DoClear() 
        {
            if (EditPattern == null) return;
            if (!ClientLib.MessageBox(this, "行をクリアしますか？")) return;
            EditPattern = null;
            PatternDetailList = new ObservableCollection<PatternDetail> { };
        }
        [RelayCommand]
        void DoInsert() 
        {
            if (OnCheckError("1") < 0) return;
            if (Bcheck_flg == 1)
            {
                Bcheck_flg = 0;
                if (!ClientLib.MessageBox(this, "入力されたブランドではない店舗がありますが、データを修正しますか？")) return;
            }
            else
            {
                if (!ClientLib.MessageBox(this, "データを修正しますか？")) return;
            }

            var item = Common.CloneObject(EditPattern);
            var item2 = Common.CloneObject(PatternDetailList); 
            Common.ConvertDotStringAdd(item);
            Common.ConvertDotStringAdd(item2);
            if (item == null || item2 == null ) return;
            var retCSV = GenerateCSV(item,item2);
            var v_para = new string[3];
            v_para[0] = "MASTER_HYUHIBN_PTN";
            v_para[1] = retCSV.SaveStr();
            v_para[2] = "パターンNO = '" + item.PatternNO.ToString() + "'";
            var ret = AppData.Http!.AspxSqlQuery2("mi", v_para);
            if (int.Parse(ret.Split(",")[0].ToString()) < 0) 
            {
                ClientLib.MessageBoxError(this, "データ追加エラー");
            }
            else
            {
                string sql = "select CUR_NUM from ZZMGR$0T_NUMBER WHERE KUBUN = 'HC$MASTER_HYUHIBN_PTN'";
                var ret_csv = AppData.Http!.AspxSqlQuery(sql);
                item.SeqNO = long.Parse(ret_csv.Rows[0][0].ToString()) + 1;
                item.VdateUpdate = VDateHelper.ToVDate(DateTime.Now);
                item.VdateCreate = item.VdateUpdate;
                Common.ConvertDotStringDel(item);
                Common.ConvertDotStringDel(item2);
                PatternList!.Add(item);
                SelectedPattern = item;
                ClientLib.MessageBox(this, "データを追加しました");
            }
        }        
        [RelayCommand]
        void DoUpdate()
        {
            if (OnCheckError("0") < 0) return;
            if (Bcheck_flg == 1)
            {
                Bcheck_flg = 0;
                if (!ClientLib.MessageBox(this, "入力されたブランドではない店舗がありますが、データを修正しますか？")) return;
            }
            else
            {
                if (!ClientLib.MessageBox(this, "データを修正しますか？")) return;
            }
            var item = Common.CloneObject(EditPattern);
            var item2 = Common.CloneObject(PatternDetailList);
            Common.ConvertDotStringAdd(item);
            Common.ConvertDotStringAdd(item2);
            if (item == null || item2 == null) return;
            var retCSV = GenerateCSV(item, item2);
            var v_para = new string[3];
            v_para[0] = "MASTER_HYUHIBN_PTN";
            v_para[1] = retCSV.SaveStr();
            v_para[2] = "パターンNO = '" + item.PatternNO.ToString() + "'";
            var ret = AppData.Http!.AspxSqlQuery2("mi", v_para);
            if (int.Parse(ret.Split(",")[0].ToString()) < 0)
            {
                ClientLib.MessageBoxError(this, "データ修正エラー");
            }
            else
            {
                Common.ConvertDotStringDel(item);
                if (SelectedPattern != null)
                {
                    SelectedPattern.VdateUpdate = VDateHelper.ToVDate(DateTime.Now);
                    SelectedPattern.PatternNO = item.PatternNO;
                    SelectedPattern.PatternName = item.PatternName;
                    SelectedPattern.Brand = new BtListHelper(item.Brand.Code,item.Brand.Name);
                    SelectedPattern.EntEmp = new BtListHelper(item.EntEmp.Code,item.EntEmp.Name);
                    EditPattern = Common.CloneObject(SelectedPattern);
                }
                ClientLib.MessageBox(this, "データを修正しました");
            }
        }
        private BizCsvDocument GenerateCSV(PatternHeader para1, ObservableCollection<PatternDetail> para2)
        {
            var dt = new DataTable();
            dt.Columns.Add("パターンNO", typeof(long));
            dt.Columns.Add("パターン名", typeof(string));
            dt.Columns.Add("ブランドCD", typeof(string));
            dt.Columns.Add("入力社員CD", typeof(string));
            dt.Columns.Add("得意先CD", typeof(string));
            dt.Columns.Add("メモ", typeof(decimal));
            dt.Columns.Add("配分率", typeof(decimal));
            for (var i = 0; i < para2.Count; i++)
            {
                var row = dt.NewRow();
                row["パターンNO"] = para1.PatternNO;
                row["パターン名"] = para1.PatternName;
                row["ブランドCD"] = para1.Brand.Code;
                row["入力社員CD"] = para1.EntEmp.Code;
                row["得意先CD"] = para2[i].Cust.Code;
                row["メモ"] = para2[i].Memo;
                row["配分率"] = para2[i].AllocRate;
                dt.Rows.Add(row);
            }
            var ret_csv = new BizCsvDocument(dt);
            return ret_csv;
        }
        [RelayCommand]
        void DoDelete()
        {
            if (!ClientLib.MessageBox(this, "データを削除しますか？")) return;
            if (EditPattern == null) return;
            var wrk_para = new string[2];
            wrk_para[0] = "MASTER_HYUHIBN_PTN";
            wrk_para[1] = "パターンNO = " + EditPattern.PatternNO.ToString() + "";
            var ret = AppData.Http!.AspxSqlQuery2("mi_del", wrk_para);
            if (ret.Split(",")[0].ToString() == "0")
            {
                if (SelectedPattern != null)
                {
                    PatternList!.Remove(SelectedPattern);
                    var item = PatternList.Where(c => c.SeqNO == PatternList.Min(c => c.SeqNO)).FirstOrDefault();
                    SelectedPattern = item;
                    ClientLib.MessageBoxOk(this, "削除しました");
                }
            }
            else
            {
                ClientLib.MessageBoxError(this, ret.Split(",")[0].ToString());
            }
        }

        int OnCheckError(string check_flg) 
        {
            if (PatternDetailList.Count < 1) return -1;

            if (EditPattern.PatternNO == 0)
            {
                ClientLib.MessageBoxError(this,"パターンNOを入力してください！");
                return -1;
            }           
            var wrk_para = new BizArray();
            wrk_para[0] = EditPattern.PatternNO.ToString();
            var sql_query = "SELECT パターンNO FROM HC$MASTER_HYUHIBN_PTN WHERE パターンNO=:1";
            var ret_csv = AppData.Http!.AspxSqlQuery(sql_query, wrk_para.ToArray());
            if (check_flg == "1" && ret_csv.Rows.Count > 0)
            {
                ClientLib.MessageBoxError(this, "このパターンNOは登録済みです！");
                return -1;
            }
            else if (check_flg == "0" && ret_csv.Rows.Count <= 0)
            {
                ClientLib.MessageBoxError(this, "対象となる伝票がありません！");
                return -1;
            }

            var check_para = new BizArray();
            var check_str = "";
            var check_csv = new DataTable();
            for (var i = 0; i < PatternDetailList.Count;i++)
            {
                if (PatternDetailList[i].Cust.Code == "")
                {
                    ClientLib.MessageBoxError(this, "店舗CDを入力して下さい！");
                    return -1;
                }
                for (var j = 0; j < PatternDetailList.Count; j++)
                {
                    if (i != j && PatternDetailList[i].Cust.Code == PatternDetailList[j].Cust.Code)
                    {
                        ClientLib.MessageBoxError(this, "店舗CDが重複しています！");
                        return -1;
                    }
                }
                check_str = "SELECT 得意先CD FROM HC$MASTER_TOKUI WHERE 得意先CD='" + PatternDetailList[i].Cust.Code.ToString() + "' and POS区分=0";
                check_csv = AppData.Http!.AspxSqlQuery(check_str);
                if (check_csv.Rows.Count <= 0)
                {
                    ClientLib.MessageBoxError(this, "既に閉店している店舗があります！");
                    return -1;
                }
                if (PatternDetailList[i].Memo == 0)
                {
                    ClientLib.MessageBoxError(this, "指数が設定されてない店舗があります！");
                    return -1;
                }
            }
            return 0;
        }
        public partial class SearchCondition : ObservableObject
        {
            [ObservableProperty]
            private long? patternNoFrom;
            [ObservableProperty]
            private long? patternNoTo;
            [ObservableProperty]
            private BtListHelper? brdCDFrom;
            [ObservableProperty]
            private BtListHelper? brdCDTo;
        }
        public partial class PatternHeader : ObservableObject
        {
            [ObservableProperty]
            private long seqNO;
            [ObservableProperty]
            private decimal vdateCreate;
            [ObservableProperty]
            private decimal vdateUpdate;
            [ObservableProperty]
            private long patternNO; // パターンNO
            [ObservableProperty]
            private string? patternName; //パターン名
            [ObservableProperty]
            private BtListHelper? brand; //ブランドCD
            [ObservableProperty]
            private BtListHelper? entEmp; //入力社員CD
        }
        public partial class PatternDetail : ObservableObject 
        {
            [ObservableProperty]
            private BtListHelper? cust;
            [ObservableProperty]
            private decimal memo;
            [ObservableProperty]
            private decimal allocRate;
        }
    }    
}
