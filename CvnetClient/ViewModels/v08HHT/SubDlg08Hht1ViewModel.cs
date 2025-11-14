using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using System.Data;
using System.IO;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg08Hht1ViewModel : BaseViewModel
    {
        #region Declare
        public enum OutPutType { Local, HandyServer }
        public enum OutPutHowType { All , Corporate }
        [ObservableProperty]
        SearchCondition? condition;
        #endregion
        #region Initialize
        public void OnInit(object? init_para = null, string? init_flg = null)
        {

            OnInitBase(init_para, init_flg);

            Condition = new SearchCondition();

            try{
                var v_name = AppData.ClassCvnet.HHT_Csv._data.Rows[0][2].ToString();
                var fs = new FileSystem();
                Condition.File = fs.MakePath(AppData.ClassCvnet.HHT_Csv.GetEnv(), v_name);
            }
		    catch(CustomException ex)
            {

                if (ex.Method == "RTM" && ex.Code == 3)
                {
                    ClientLib.MessageBoxError(this,"まず最初に「HHT用PATH設定」を行って下さい","エラー");
                    return;
                }

            }
            catch (Exception ex)
            {
                ClientLib.MessageBoxError(this,ex.Message,"エラー");
            }

            /* 法人対応　10.11.29 */
            if (AppData.ClassCvnet.config.MultiCoop > 0)
            {
                Condition.ShowOrNot = "Visible";
            }

            /* BT1000対応 11.04.18 */
            if (AppData.ClassCvnet.config.hhtkisyu == 1) Condition.SelectedOutPut = OutPutType.HandyServer;

        }
        #endregion
        #region Function
        [RelayCommand]
        public void DoExecute() 
        {
            if (Condition.SelectedOutPut == OutPutType.Local)
            {

                if (AppData.ClassCvnet.HHT_Csv.GetPath("") == "")
                {
                    ClientLib.MessageBoxError(this,"初期設定されていません。","エラー");
                    return;
                }
                
                var start0 = DateTime.Now;
                
                /* ここにマスタ処理を入れる */
                var ret_csv0 = GetMaster(int.TryParse(AppData.ClassCvnet.SysHhtMst.Rows[0][33].ToString(), out var _intParse) ? _intParse:0);

                /* ユーザー対応 */
                var v_fname = "";

                var end0 = DateTime.Now;
                

                /* ファイル名設定 13.09.06 */
                if (para[0] == "1")
                {
                    v_fname = ".txt";
                }

                /* return; */
                var fs = new FileSystem();
                var wrk1 = new BizCsvDocument(ret_csv0);
                try
                {
                    var fp = fs.Open("/hht/hksnds1" + v_fname, FileSystem.OPEN_WRITE);
                    var v_format = new string[91];
                    for (int i = 0; i < v_format.Length; i++) v_format[i] = "10";
                    wrk1.Save2(fp, v_format, 1, "\r\n");
                    Condition.Label6 = wrk1.GetTable().Rows.Count;
                    
                    fp.Close();
                    
                    fs.DeleteFile("/hht/dummy.dat", FileSystem.NOCONFIRM + FileSystem.NOERRORUI);
                    fs.DeleteFile("/hht/create.dat", FileSystem.NOCONFIRM + FileSystem.NOERRORUI);

                    Condition.Result = "データ作成しました \n終了時刻:"
                        + DateTime.Now.ToString() + "\n経過時間:" + (end0 - start0).ToString("HH24:MI:SS");
                }
                catch (Exception ex)
                {
                    ClientLib.MessageBoxError(this,ex.Message,"エラー");
                    return;
                }
                /* ハンディサーバー用	 */
            }
            else
            {
                var start0 = DateTime.Now;

                /* ユーザー対応 */
                var wrk_para = new BizArray();
                var v_syori = "hhtout00";

                var ret_csv = AppData.Http!.AspxSqlQuery2(v_syori, wrk_para.ToArray());
                var end0 = DateTime.Now;
                if (ret_csv.Split(',')[0].ToString() == "0")
                {
                    Condition.Result = "データ作成しました \n終了時刻:"
                    + DateTime.Now.ToString() + "\n経過時間:" + (end0 - start0).ToString("HH:mm:SS");
                }
                else
                {
                    Condition.Result = "作成出来ませんでした \n終了時刻:"
                    + DateTime.Now.ToString() + "\n経過時間:" + (end0 - start0).ToString("HH:mm:SS");
                }
            }
        }
        private DataTable GetMaster(int? flg) 
        {
            var ret_csv0 = new DataTable();
            var tmp_str = "×★―－、。，．・：；？！゛゜´｀¨＾￣＿ヽヾゝゞ〃仝々〆〇ー―‐／＼～∥｜…‥‘’“”（）〔〕［］｛｝〈〉《》「」『』【】＋－±×÷＝≠＜＞≦≧∞∴♂♀°′″℃￥＄￠￡％＃＆＊＠§☆★○●◎◇◆□■△▲▽▼※〒→←↑↓〓∈∋⊆⊇⊂⊃∪∩∧∨￢⇒⇔∀∃∠⊥⌒∂∇≡≒≪≫√∽∝∵∫Å‰♯♭♪†‡¶◯";

            /* 法人対応　10.11.29 */
            var v_hjin = "";
            if (AppData.ClassCvnet.config.MultiCoop > 0)
            {
                if (Condition.SelectedOutPutHow == OutPutHowType.Corporate)
                {
                    v_hjin = AppData.ClassCvnet.GetQueryStrHoujin().ToString();
                }
            }

            switch (flg)
            {
                case 1: /* VULCAN */
                    var sql_str = "";
                    if (AppData.ClassCvnet.config.OutMasterMei == 1)
                    {
                        sql_str = "select * from ( "
                            + " SELECT 'SIR'||lpad(仕入先CD,8,'0')||rpad(decode(略称,'.',TRANSLATE(仕入先名,'" + tmp_str + "',' '),TRANSLATE(略称,'" + tmp_str + "',' ')),40)||rpad(decode(略称,'.',' ',TRANSLATE(略称,'" + tmp_str + "',' ')),40)||'*' マスタ "
                            + " FROM  hc$master_siire where 発注停止FLG=0 " + v_hjin
                            + " union "
                            + " SELECT 'SOK'||lpad(得意先CD,8,'0')||rpad(decode(略称,'.',TRANSLATE(得意先名,'" + tmp_str + "',' '),TRANSLATE(略称,'" + tmp_str + "',' ')),40)||rpad(decode(略称,'.',' ',TRANSLATE(略称,'" + tmp_str + "',' ')),40)||'*' マスタ "
                            + " FROM hc$master_tokui where ( (店種区分 in (0,3,6)) or (店種区分 in (1) and 在庫管理FLG=1) ) and 出荷停止FLG=0 " + v_hjin
                            + " union "
                            + " SELECT 'TAN'||lpad(社員CD,6,'0')||'  '||rpad(decode(名前,'.',' ',TRANSLATE(名前,'" + tmp_str + "',' ')),40)||rpad(' ',40)||'*' マスタ "
                            + " FROM hc$master_shain where 出力FLG != 99 "
                            + " union "
                            + " SELECT 'TOK'||lpad(得意先CD,8,'0')||rpad(decode(略称,'.',TRANSLATE(得意先名,'" + tmp_str + "',' '),TRANSLATE(略称,'" + tmp_str + "',' ')),40)||rpad(decode(略称,'.',' ',TRANSLATE(略称,'" + tmp_str + "',' ')),40)||'*' マスタ "
                            + " FROM hc$master_tokui where 店種区分 in (0,1,3,6) and 出荷停止FLG=0 " + v_hjin
                            + " ) order by マスタ ";
                    }
                    else if (AppData.ClassCvnet.config.OutMasterMei == 2)
                    {
                        sql_str = "select * from ( "
                            + " SELECT 'SIR'||lpad(仕入先CD,8,'0')||rpad(decode(カナ,'.',TRANSLATE(仕入先名,'" + tmp_str + "',' '),TRANSLATE(カナ,'" + tmp_str + "',' ')),40)||rpad(decode(略称,'.',' ',TRANSLATE(略称,'" + tmp_str + "',' ')),40)||'*' マスタ "
                            + " FROM  hc$master_siire where 発注停止FLG=0 " + v_hjin
                            + " union "
                            + " SELECT 'SOK'||lpad(得意先CD,8,'0')||rpad(decode(カナ,'.',TRANSLATE(得意先名,'" + tmp_str + "',' '),TRANSLATE(カナ,'" + tmp_str + "',' ')),40)||rpad(decode(略称,'.',' ',TRANSLATE(略称,'" + tmp_str + "',' ')),40)||'*' マスタ "
                            + " FROM hc$master_tokui where ( (店種区分 in (0,3,6)) or (店種区分 in (1) and 在庫管理FLG=1) ) and 出荷停止FLG=0 " + v_hjin
                            + " union "
                            + " SELECT 'TAN'||lpad(社員CD,6,'0')||'  '||rpad(decode(名前,'.',' ',TRANSLATE(名前,'" + tmp_str + "',' ')),40)||rpad(' ',40)||'*' マスタ "
                            + " FROM hc$master_shain where 出力FLG != 99 "
                            + " union "
                            + " SELECT 'TOK'||lpad(得意先CD,8,'0')||rpad(decode(カナ,'.',TRANSLATE(得意先名,'" + tmp_str + "',' '),TRANSLATE(カナ,'" + tmp_str + "',' ')),40)||rpad(decode(略称,'.',' ',TRANSLATE(略称,'" + tmp_str + "',' ')),40)||'*' マスタ "
                            + " FROM hc$master_tokui where 店種区分 in (0,1,3,6) and 出荷停止FLG=0 " + v_hjin
                            + " ) order by マスタ ";
                    }
                    else
                    {
                        sql_str = "select * from ( "
                            + " SELECT 'SIR'||lpad(仕入先CD,8,'0')||rpad(decode(仕入先名,'.',' ',TRANSLATE(仕入先名,'" + tmp_str + "',' ')),40)||rpad(decode(略称,'.',' ',TRANSLATE(略称,'" + tmp_str + "',' ')),40)||'*' マスタ "
                            + " FROM  hc$master_siire where 発注停止FLG=0 " + v_hjin
                            + " union "
                            + " SELECT 'SOK'||lpad(得意先CD,8,'0')||rpad(decode(得意先名,'.',' ',TRANSLATE(得意先名,'" + tmp_str + "',' ')),40)||rpad(decode(略称,'.',' ',TRANSLATE(略称,'" + tmp_str + "',' ')),40)||'*' マスタ "
                            + " FROM hc$master_tokui where ( (店種区分 in (0,3,6)) or (店種区分 in (1) and 在庫管理FLG=1) ) and 出荷停止FLG=0 " + v_hjin
                            + " union "
                            + " SELECT 'TAN'||lpad(社員CD,6,'0')||'  '||rpad(decode(名前,'.',' ',TRANSLATE(名前,'" + tmp_str + "',' ')),40)||rpad(' ',40)||'*' マスタ "
                            + " FROM hc$master_shain where 出力FLG != 99 "
                            + " union "
                            + " SELECT 'TOK'||lpad(得意先CD,8,'0')||rpad(decode(得意先名,'.',' ',TRANSLATE(得意先名,'" + tmp_str + "',' ')),40)||rpad(decode(略称,'.',' ',TRANSLATE(略称,'" + tmp_str + "',' ')),40)||'*' マスタ "
                            + " FROM hc$master_tokui where 店種区分 in (0,1,3,6) and 出荷停止FLG=0 " + v_hjin
                            + " ) order by マスタ ";
                    }


                    ret_csv0 = AppData.Http!.AspxSqlQuery(sql_str);
                    break;
                    ret_csv0 = new DataTable();
            }

            return ret_csv0;
        }
        #endregion
        public class FileSystem
        {
            public const int OPEN_READ = 1;
            public const int OPEN_WRITE = 2;
            public const int OPEN_APPEND = 3;
            public const int NOCONFIRM = 1;
            public const int NOERRORUI = 2;

            public StreamWriter Open(string path, int mode)
            {
                switch (mode)
                {
                    case OPEN_WRITE:
                        return new StreamWriter(path, false); // overwrite
                    case OPEN_APPEND:
                        return new StreamWriter(path, true);  // append
                    case OPEN_READ:
                        throw new InvalidOperationException("Use StreamReader for read mode.");
                    default:
                        throw new ArgumentException("Invalid open mode.");
                }
            }

            public string MakePath(string folder, string file)
            {
                return Path.Combine(folder, file);
            }

            public void CopyFile(string source, string dest, bool overwrite = false)
            {
                File.Copy(source, dest, overwrite);
            }

            public void DeleteFile(string path, int flags = 0)
            {
                try
                {
                    if (File.Exists(path))
                        File.Delete(path);
                }
                catch
                {
                    if ((flags & NOERRORUI) == 0)
                        throw; 
                }
            }

            public bool FileExists(string path)
            {
                return File.Exists(path);
            }
        }
        public class CustomException : Exception
        {
            public string Method { get; }
            public int Code { get; }

            public CustomException(string method, int code, string message)
                : base(message)
            {
                Method = method;
                Code = code;
            }
        }
        public partial class SearchCondition : ObservableObject 
        {
            [ObservableProperty]
            private OutPutType selectedOutPut = OutPutType.Local;
            [ObservableProperty]
            private string? file;
            [ObservableProperty]
            private string? showOrNot = "Hidden";
            [ObservableProperty]
            private OutPutHowType selectedOutPutHow = OutPutHowType.Corporate;
            [ObservableProperty]
            private string? result;
            [ObservableProperty]
            private int? label6;
        }
    }
}
