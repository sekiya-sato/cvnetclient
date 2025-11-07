using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01PrintViewModel : BaseViewModel
    {
        [ObservableProperty]
        ObservableCollection<MasterManagePrt>? listPrt;
        [ObservableProperty]
        MasterManagePrt? editPrt;
        [ObservableProperty]
        MasterManagePrt? selectedPrt;
        [ObservableProperty]
        private string? prtCD;
        [ObservableProperty]
        private string? startCode;
        public void OnInit() 
        {
        }

        [RelayCommand]
        void DoList() 
        {
            StartCode = PrtCD;
            OnQuery("");
            if (ListPrt == null || ListPrt.Count == 0)
                ClientLib.MessageBoxOk(this, "データがありません");
            return;
        }

        [RelayCommand]
        void NextList() 
        {
            if (ListPrt != null && ListPrt.Count > 0)
            {
                StartCode = ListPrt.Max(c => c.FormCD);
            }
            else
            {
                DoList();
            }
            OnQuery("");
            if (ListPrt == null || ListPrt.Count == 0)
                ClientLib.MessageBoxOk(this, "データがありません");
            
        }

        [RelayCommand]
        void BackList()
        {
            if (ListPrt != null && ListPrt.Count > 0)
            {
                StartCode = ListPrt.Min(c => c.FormCD);
            }
            else
            {
                DoList();
            }
            OnQuery("DESC");
            if (ListPrt == null || ListPrt.Count == 0)
                ClientLib.MessageBoxOk(this, "データがありません");
        }

        string sql_collist = ",A.帳票CD,A.QFM名,A.メニュー名,A.帳票名"+
                             ",A.項目01,A.項目02,A.項目03,A.項目04,A.項目05,A.項目06,A.項目07,A.項目08,A.項目09,A.項目10"+
                             ",A.項目11,A.項目12,A.項目13,A.項目14,A.項目15,A.項目16,A.項目17,A.項目18,A.項目19,A.項目20"+
                             ",A.項目21,A.項目22,A.項目23,A.項目24,A.項目25,A.項目26,A.項目27,A.項目28,A.項目29,A.項目30"+
                             ",A.項目31,A.項目32,A.項目33,A.項目34,A.項目35,A.項目36,A.項目37,A.項目38,A.項目39,A.項目40"+
                             ",A.項目41,A.項目42,A.項目43,A.項目44,A.項目45,A.項目46,A.項目47,A.項目48,A.項目49,A.項目50"+
                             ",A.項目51,A.項目52,A.項目53,A.項目54,A.項目55,A.項目56,A.項目57,A.項目58,A.項目59,A.項目60"+
                             ",A.項目61,A.項目62,A.項目63,A.項目64,A.項目65,A.項目66,A.項目67,A.項目68,A.項目69,A.項目70"+
                             ",A.項目71,A.項目72,A.項目73,A.項目74,A.項目75,A.項目76,A.項目77,A.項目78,A.項目79,A.項目80"+
                             ",A.項目81,A.項目82,A.項目83,A.項目84,A.項目85,A.項目86,A.項目87,A.項目88,A.項目89,A.項目90";
        void OnQuery(string? p_sort) 
        {           
            var v_sort = "ASC";
            var v_hugo = ">=";
            if (p_sort != "")
            {
                v_sort = p_sort;
                v_hugo = "<=";
            }

            var sql_query = "select A.SEQ_NO,A.VDATE_CREATE,A.VDATE_UPDATE";
            sql_query += sql_collist;
            sql_query += ",(A.入力社員CD ||' '|| (select B.名前 from HC$MASTER_SHAIN B where B.社員CD=A.入力社員CD)) 最終修正者";
            sql_query += " from HC$MASTER_PRT_KANRI A";

            if (StartCode != "" && StartCode != null) { 
                sql_query += " where A.帳票CD " + v_hugo + StartCode;
            } 
            sql_query += " order by A.帳票CD " + v_sort;
            sql_query = "Select * From(" + sql_query + ") WHERE ROWNUM <=" + AppData.maxQueryCnt;

            var ret_data = AppData.Http!.AspxSqlQuery(sql_query, new string[] {});
            if (ret_data == null || ret_data.Rows.Count == 0) return;
            var list = (from DataRow dr in ret_data.Rows
                        select new MasterManagePrt
                        {
                            SeqNo = Convert.ToInt64(dr["SEQ_NO"]),
                            VdateCreate = Convert.ToDecimal(dr["VDATE_CREATE"]),
                            VdateUpdate = Convert.ToDecimal(dr["VDATE_UPDATE"]),
                            FormCD = dr["帳票CD"].ToString() ?? string.Empty,
                            QfmName = dr["QFM名"].ToString() ?? string.Empty,
                            MenuName = dr["メニュー名"].ToString() ?? string.Empty,
                            FormName = dr["帳票名"].ToString() ?? string.Empty,
                            InpEmployeeCD = dr["最終修正者"].ToString() ?? string.Empty,
                            Item01 = dr["項目01"] == DBNull.Value ? null : dr["項目01"].ToString(),
                            Item02 = dr["項目02"] == DBNull.Value ? null : dr["項目02"].ToString(),
                            Item03 = dr["項目03"] == DBNull.Value ? null : dr["項目03"].ToString(),
                            Item04 = dr["項目04"] == DBNull.Value ? null : dr["項目04"].ToString(),
                            Item05 = dr["項目05"] == DBNull.Value ? null : dr["項目05"].ToString(),
                            Item06 = dr["項目06"] == DBNull.Value ? null : dr["項目06"].ToString(),
                            Item07 = dr["項目07"] == DBNull.Value ? null : dr["項目07"].ToString(),
                            Item08 = dr["項目08"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item09 = dr["項目09"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item10 = dr["項目10"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item11 = dr["項目11"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item12 = dr["項目12"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item13 = dr["項目13"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item14 = dr["項目14"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item15 = dr["項目15"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item16 = dr["項目16"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item17 = dr["項目17"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item18 = dr["項目18"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item19 = dr["項目19"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item20 = dr["項目20"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item21 = dr["項目21"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item22 = dr["項目22"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item23 = dr["項目23"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item24 = dr["項目24"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item25 = dr["項目25"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item26 = dr["項目26"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item27 = dr["項目27"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item28 = dr["項目28"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item29 = dr["項目29"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item30 = dr["項目30"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item31 = dr["項目31"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item32 = dr["項目32"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item33 = dr["項目33"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item34 = dr["項目34"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item35 = dr["項目35"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item36 = dr["項目36"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item37 = dr["項目37"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item38 = dr["項目38"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item39 = dr["項目39"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item40 = dr["項目40"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item41 = dr["項目41"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item42 = dr["項目42"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item43 = dr["項目43"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item44 = dr["項目44"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item45 = dr["項目45"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item46 = dr["項目46"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item47 = dr["項目47"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item48 = dr["項目48"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item49 = dr["項目49"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item50 = dr["項目50"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item51 = dr["項目51"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item52 = dr["項目52"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item53 = dr["項目53"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item54 = dr["項目54"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item55 = dr["項目55"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item56 = dr["項目56"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item57 = dr["項目57"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item58 = dr["項目58"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item59 = dr["項目59"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item60 = dr["項目60"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item61 = dr["項目61"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item62 = dr["項目62"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item63 = dr["項目63"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item64 = dr["項目64"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item65 = dr["項目65"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item66 = dr["項目66"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item67 = dr["項目67"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item68 = dr["項目68"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item69 = dr["項目69"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item70 = dr["項目70"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item71 = dr["項目71"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item72 = dr["項目72"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item73 = dr["項目73"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item74 = dr["項目74"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item75 = dr["項目75"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item76 = dr["項目76"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item77 = dr["項目77"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item78 = dr["項目78"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item79 = dr["項目79"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item80 = dr["項目80"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item81 = dr["項目81"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item82 = dr["項目82"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item83 = dr["項目83"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item84 = dr["項目84"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item85 = dr["項目85"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item86 = dr["項目86"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item87 = dr["項目87"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item88 = dr["項目88"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item89 = dr["項目89"] == DBNull.Value ? null : dr["項目08"].ToString(),
                            Item90 = dr["項目90"] == DBNull.Value ? null : dr["項目08"].ToString(),
                        }).OrderBy(c => c.FormCD).ToList();
            Common.ConvertDotStringDel(list);
            ListPrt = new ObservableCollection<MasterManagePrt>(list);
            if (ListPrt.Count > 0)
            {
                SelectedPrt = ListPrt[0];
            }
        }
        [RelayCommand]
        void DoInsert()
        {
            if (!ClientLib.MessageBox(this, "新規登録しますか？")) return;
            var item = Common.CloneObject(EditPrt);
            Common.ConvertDotStringAdd(item);
            if (item == null) return;
            var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.INSERT, "Master_PRT_KANRI", 0, "0",
                new string[] { "帳票CD","QFM名","メニュー名","帳票名","項目01","項目02","項目03","項目04","項目05","項目06","項目07","項目08","項目09","項目10","項目11","項目12","項目13","項目14","項目15","項目16","項目17","項目18","項目19","項目20","項目21","項目22","項目23","項目24","項目25","項目26","項目27","項目28","項目29","項目30","項目31","項目32","項目33","項目34","項目35","項目36","項目37","項目38","項目39","項目40","項目41","項目42","項目43","項目44","項目45","項目46","項目47","項目48","項目49","項目50","項目51","項目52","項目53","項目54","項目55","項目56","項目57","項目58","項目59","項目60","項目61","項目62","項目63","項目64","項目65","項目66","項目67","項目68","項目69","項目70","項目71","項目72","項目73","項目74","項目75","項目76","項目77","項目78","項目79","項目80","項目81","項目82","項目83","項目84","項目85","項目86","項目87","項目88","項目89","項目90","入力社員CD"},
                new string[] { item.FormCD,item.QfmName,item.MenuName,item.FormName,item.Item01,item.Item02,item.Item03,item.Item04,item.Item05,item.Item06,item.Item07,item.Item08,item.Item09,item.Item10,item.Item11,item.Item12,item.Item13,item.Item14,item.Item15,item.Item16,item.Item17,item.Item18,item.Item19,item.Item20,item.Item21,item.Item22,item.Item23,item.Item24,item.Item25,item.Item26,item.Item27,item.Item28,item.Item29,item.Item30,item.Item31,item.Item32,item.Item33,item.Item34,item.Item35,item.Item36,item.Item37,item.Item38,item.Item39,item.Item40,item.Item41,item.Item42,item.Item43,item.Item44,item.Item45,item.Item46,item.Item47,item.Item48,item.Item49,item.Item50,item.Item51,item.Item52,item.Item53,item.Item54,item.Item55,item.Item56,item.Item57,item.Item58,item.Item59,item.Item60,item.Item61,item.Item62,item.Item63,item.Item64,item.Item65,item.Item66,item.Item67,item.Item68,item.Item69,item.Item70,item.Item71,item.Item72,item.Item73,item.Item74,item.Item75,item.Item76,item.Item77,item.Item78,item.Item79,item.Item80,item.Item81,item.Item82,item.Item83,item.Item84,item.Item85,item.Item86,item.Item87,item.Item88,item.Item89,item.Item90,item.InpEmployeeCD});
            if (ret.Code == 0)
            {
                item.SeqNo = ret.NewSeq;
                item.VdateUpdate = decimal.Parse(ret.VDate);
                item.VdateCreate = item.VdateUpdate;
                Common.ConvertDotStringDel(item);
                ListPrt!.Add(item);
                SelectedPrt = item;
                ClientLib.MessageBoxOk(this, "正常に追加しました。");
            }
            else
            {
                ClientLib.MessageBoxError(this, "データが追加出来ませんでした");
            }
        }

        [RelayCommand]
        void DoUpdate()
        {
            if (!ClientLib.MessageBox(this, "修正しますか？")) return;
            var item = Common.CloneObject(EditPrt);
            Common.ConvertDotStringAdd(item);
            if (item == null) return;
            var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.UPDATE, "Master_PRT_KANRI", item.SeqNo, item.VdateUpdate.ToString(),
                new string[] { "帳票CD","QFM名","メニュー名","帳票名","項目01","項目02","項目03","項目04","項目05","項目06","項目07","項目08","項目09","項目10","項目11","項目12","項目13","項目14","項目15","項目16","項目17","項目18","項目19","項目20","項目21","項目22","項目23","項目24","項目25","項目26","項目27","項目28","項目29","項目30","項目31","項目32","項目33","項目34","項目35","項目36","項目37","項目38","項目39","項目40","項目41","項目42","項目43","項目44","項目45","項目46","項目47","項目48","項目49","項目50","項目51","項目52","項目53","項目54","項目55","項目56","項目57","項目58","項目59","項目60","項目61","項目62","項目63","項目64","項目65","項目66","項目67","項目68","項目69","項目70","項目71","項目72","項目73","項目74","項目75","項目76","項目77","項目78","項目79","項目80","項目81","項目82","項目83","項目84","項目85","項目86","項目87","項目88","項目89","項目90","入力社員CD"},
                new string[] { item.FormCD, item.QfmName, item.MenuName, item.FormName, item.Item01, item.Item02, item.Item03, item.Item04, item.Item05, item.Item06, item.Item07, item.Item08, item.Item09, item.Item10, item.Item11, item.Item12, item.Item13, item.Item14, item.Item15, item.Item16, item.Item17, item.Item18, item.Item19, item.Item20, item.Item21, item.Item22, item.Item23, item.Item24, item.Item25, item.Item26, item.Item27, item.Item28, item.Item29, item.Item30, item.Item31, item.Item32, item.Item33, item.Item34, item.Item35, item.Item36, item.Item37, item.Item38, item.Item39, item.Item40, item.Item41, item.Item42, item.Item43, item.Item44, item.Item45, item.Item46, item.Item47, item.Item48, item.Item49, item.Item50, item.Item51, item.Item52, item.Item53, item.Item54, item.Item55, item.Item56, item.Item57, item.Item58, item.Item59, item.Item60, item.Item61, item.Item62, item.Item63, item.Item64, item.Item65, item.Item66, item.Item67, item.Item68, item.Item69, item.Item70, item.Item71, item.Item72, item.Item73, item.Item74, item.Item75, item.Item76, item.Item77, item.Item78, item.Item79, item.Item80, item.Item81, item.Item82, item.Item83, item.Item84, item.Item85, item.Item86, item.Item87, item.Item88, item.Item89, item.Item90, item.InpEmployeeCD });
            if (ret.Code == 0)
            {
                Common.ConvertDotStringDel(item);
                if (SelectedPrt != null)
                {
                    SelectedPrt.VdateUpdate = VDateHelper.ToVDate(DateTime.Now);
                    SelectedPrt.FormCD = item.FormCD;
                    SelectedPrt.QfmName = item.QfmName;
                    SelectedPrt.MenuName = item.MenuName;
                    SelectedPrt.FormName = item.FormName;
                    SelectedPrt.Item01 = item.Item01;
                    SelectedPrt.Item02 = item.Item02;
                    SelectedPrt.Item03 = item.Item03;
                    SelectedPrt.Item04 = item.Item04;
                    SelectedPrt.Item05 = item.Item05;
                    SelectedPrt.Item06 = item.Item06;
                    SelectedPrt.Item07 = item.Item07;
                    SelectedPrt.Item08 = item.Item08;
                    SelectedPrt.Item09 = item.Item09;
                    SelectedPrt.Item10 = item.Item10;
                    SelectedPrt.Item11 = item.Item11;
                    SelectedPrt.Item12 = item.Item12;
                    SelectedPrt.Item13 = item.Item13;
                    SelectedPrt.Item14 = item.Item14;
                    SelectedPrt.Item15 = item.Item15;
                    SelectedPrt.Item16 = item.Item16;
                    SelectedPrt.Item17 = item.Item17;
                    SelectedPrt.Item18 = item.Item18;
                    SelectedPrt.Item19 = item.Item19;
                    SelectedPrt.Item20 = item.Item20;
                    SelectedPrt.Item21 = item.Item21;
                    SelectedPrt.Item22 = item.Item22;
                    SelectedPrt.Item23 = item.Item23;
                    SelectedPrt.Item24 = item.Item24;
                    SelectedPrt.Item25 = item.Item25;
                    SelectedPrt.Item26 = item.Item26;
                    SelectedPrt.Item27 = item.Item27;
                    SelectedPrt.Item28 = item.Item28;
                    SelectedPrt.Item29 = item.Item29;
                    SelectedPrt.Item30 = item.Item30;
                    SelectedPrt.Item31 = item.Item31;
                    SelectedPrt.Item32 = item.Item32;
                    SelectedPrt.Item33 = item.Item33;
                    SelectedPrt.Item34 = item.Item34;
                    SelectedPrt.Item35 = item.Item35;
                    SelectedPrt.Item36 = item.Item36;
                    SelectedPrt.Item37 = item.Item37;
                    SelectedPrt.Item38 = item.Item38;
                    SelectedPrt.Item39 = item.Item39;
                    SelectedPrt.Item40 = item.Item40;
                    SelectedPrt.Item41 = item.Item41;
                    SelectedPrt.Item42 = item.Item42;
                    SelectedPrt.Item43 = item.Item43;
                    SelectedPrt.Item44 = item.Item44;
                    SelectedPrt.Item45 = item.Item45;
                    SelectedPrt.Item46 = item.Item46;
                    SelectedPrt.Item47 = item.Item47;
                    SelectedPrt.Item48 = item.Item48;
                    SelectedPrt.Item49 = item.Item49;
                    SelectedPrt.Item50 = item.Item50;
                    SelectedPrt.Item51 = item.Item51;
                    SelectedPrt.Item52 = item.Item52;
                    SelectedPrt.Item53 = item.Item53;
                    SelectedPrt.Item54 = item.Item54;
                    SelectedPrt.Item55 = item.Item55;
                    SelectedPrt.Item56 = item.Item56;
                    SelectedPrt.Item57 = item.Item57;
                    SelectedPrt.Item58 = item.Item58;
                    SelectedPrt.Item59 = item.Item59;
                    SelectedPrt.Item60 = item.Item60;
                    SelectedPrt.Item61 = item.Item61;
                    SelectedPrt.Item62 = item.Item62;
                    SelectedPrt.Item63 = item.Item63;
                    SelectedPrt.Item64 = item.Item64;
                    SelectedPrt.Item65 = item.Item65;
                    SelectedPrt.Item66 = item.Item66;
                    SelectedPrt.Item67 = item.Item67;
                    SelectedPrt.Item68 = item.Item68;
                    SelectedPrt.Item69 = item.Item69;
                    SelectedPrt.Item70 = item.Item70;
                    SelectedPrt.Item71 = item.Item71;
                    SelectedPrt.Item72 = item.Item72;
                    SelectedPrt.Item73 = item.Item73;
                    SelectedPrt.Item74 = item.Item74;
                    SelectedPrt.Item75 = item.Item75;
                    SelectedPrt.Item76 = item.Item76;
                    SelectedPrt.Item77 = item.Item77;
                    SelectedPrt.Item78 = item.Item78;
                    SelectedPrt.Item79 = item.Item79;
                    SelectedPrt.Item80 = item.Item80;
                    SelectedPrt.Item81 = item.Item81;
                    SelectedPrt.Item82 = item.Item82;
                    SelectedPrt.Item83 = item.Item83;
                    SelectedPrt.Item84 = item.Item84;
                    SelectedPrt.Item85 = item.Item85;
                    SelectedPrt.Item86 = item.Item86;
                    SelectedPrt.Item87 = item.Item87;
                    SelectedPrt.Item88 = item.Item88;
                    SelectedPrt.Item89 = item.Item89;
                    SelectedPrt.Item90 = item.Item90;
                    SelectedPrt.InpEmployeeCD = item.InpEmployeeCD;
                    EditPrt = Common.CloneObject(SelectedPrt);
                    ClientLib.MessageBoxOk(this, "正常に修正しました。");
                }
            }
            else
            {
                ClientLib.MessageBoxError(this, "データが修正出来ませんでした");
            }
        }

        [RelayCommand]
        void DoDelete()
        {
            if (!ClientLib.MessageBox(this, "削除しますか？")) return;
            if (EditPrt == null) return;
            var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.DELETE, "Master_PRT_KANRI", EditPrt.SeqNo, EditPrt.VdateUpdate.ToString(),
                new string[0], new string[0]);
            if (ret.Code == 0)
            {
                if (SelectedPrt != null)
                {
                    ListPrt!.Remove(SelectedPrt);
                    var item = ListPrt.Where(c => c.FormCD == ListPrt.Min(c => c.FormCD)).FirstOrDefault();
                    SelectedPrt = item;
                }
                ClientLib.MessageBoxOk(this, "正常に削除しました。");
            }
            else
            {
                ClientLib.MessageBoxError(this, "データが削除出来ませんでした");
            }
        }

        [RelayCommand]
        void DoUpd() 
        {
            var vm = new SubDlg01Print1ViewModel();
            var window = new SubDlg01Print1View { };
            window.ShowDialog();
        }

        [RelayCommand]
        async Task DoPrintAsync()
        {
            if (!ClientLib.MessageBox(this, "印刷しますか？")) return;
            ClientLib.CursorToWait();
            if (ListPrt == null || ListPrt.Count == 0) { ClientLib.CursorToNormal(); return; }
            var paramNames = ListPrt.Select((c, i) => $":p{i}||''").ToList();
            var sql_query = "select A.SEQ_NO";
            sql_query += ",SUBSTR(GET_VDATE(a.VDATE_CREATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_CREATE),10,6) 作成日時";
            sql_query += ",SUBSTR(GET_VDATE(a.VDATE_UPDATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_UPDATE),10,6) 更新日時";
            sql_query += sql_collist;
            sql_query += ",(A.入力社員CD ||' '|| (select B.名前 from HC$MASTER_SHAIN B where B.社員CD=A.入力社員CD)) 最終修正者";
            sql_query += " from HC$MASTER_PRT_KANRI A";
            sql_query += $" where A.帳票CD in ({ string.Join(",", paramNames)}) order by A.帳票CD";
            sql_query = "select * from (" + sql_query + ")";
            var parameters = ListPrt.Select(c => c.FormCD).ToArray();
            var ret_csv = AppData.Http!.AspxSqlQueryCsv(sql_query, parameters, "cvnet_prtkanri.qfm");
            var lines = ret_csv.Split('\n');

            if (lines.Length < 2 || lines[1] == "0")
            {
                ClientLib.MessageBoxError(this, "PDFデータがありません");
                return;
            }

            string pdfPath = lines[0];
            string url = AppData.Http.URLroot + pdfPath + "/data.pdf";

            bool ready = await Utils.GlobalFunc.WaitForPdfAsync(url, TimeSpan.FromSeconds(30));
            if (!ready)
            {
                ClientLib.MessageBoxError(this, "PDF生成に時間がかかりすぎています。\n 条件を絞ってください。");
                return;
            }

            var win = new WebpdfView();
            if (win.DataContext is WebpdfViewModel vm)
            {
                vm.Pdfdata = url;
            }
            ClientLib.CursorToNormal();
            ClientLib.ShowDialogView(win, this);
        }
        partial void OnSelectedPrtChanged(MasterManagePrt? value)
        {
            if (value != null)
            {
                EditPrt = CvnetBaseCore.Common.CloneObject(value);
            }

            else
                EditPrt = null;
        }
    }
}
