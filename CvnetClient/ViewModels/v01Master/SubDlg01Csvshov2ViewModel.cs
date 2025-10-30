using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01Csvshov2ViewModel : BaseViewModel
    {
        public BizArray col_str;
        public void OnInit(object[] para = null) 
        { 
        
        }

        [RelayCommand]
        public void GetErrorCommand() 
        {
            var wrk_para = new BizArray();
            wrk_para = col_str;
            var sql_str = "select " + wrk_para.ToString() + ",チェック from HC$WORK_SHOHIN_JAN01 w where チェック!='.'";
            sql_str = "select * from (" + sql_str + ")";
            var ret_csv = AppData.Http!.AspxSqlQuery(sql_str, null);

            var total_cnt = ret_csv.Rows.Count;
            if (total_cnt <= 0)
            {
                ClientLib.MessageBoxError(this,"エラーはありませんでした","エラー");
                return;
            }

            DataRow newRow = ret_csv.NewRow();
            for (int i = 0; i < ret_csv.Columns.Count; i++)
            {
                newRow[i] = ret_csv.Columns[i].ColumnName;
            }

            ret_csv.Rows.InsertAt(newRow, 0);
            if (!ClientLib.MessageBoxYesno("エラー件数：" + (ret_csv.Rows.Count-1).ToString() + "件\n保存します。", "エラー") == false) return;

            try
            {
                var save_csv = new BizCsvDocument(ret_csv);
                save_csv.SaveCsv("商品マスタ_" + DateTime.Now.ToString("YYYYMMDDHH24MISS"));
            }
            catch (Exception ex)
            {
                ClientLib.MessageBoxError(this,"データ保存エラー\n" + ex.Message, "エラー");
            }

        }

        [RelayCommand]
        public void DoCSV() 
        {
            try
            {
                var openDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Title = "CSV読み込み",
                    Filter = "CSVファイル (*.csv;*.txt)|*.csv;*.txt",
                    DefaultExt = ".csv"
                };
                openDialog.ShowDialog();
                string sourceFile = openDialog.FileName;
                string fileName = Path.GetFileName(sourceFile);

                string workDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WorkShohin");
                Directory.CreateDirectory(workDir);

                string copiedPath = Path.Combine(workDir, fileName);
                File.Copy(sourceFile, copiedPath, true);

                string upname = Path.GetFileNameWithoutExtension(fileName)
                                + "_" + DateTime.Now.ToString("yyyyMMddHHmmss")
                                + Path.GetExtension(fileName);
                string[] para = new string[1];
                para[0] = upname;
                bool uploadOk = true;

                // --- 5️⃣ Query server (contoh panggilan ke DB / API) ---
                var retCsv = AppData.Http!.AspxSqlQuery2("mi_csv2", para, null, 1);

                // --- 6️⃣ Papar keputusan ---
                string message = "";
                if (retCsv.Split('\n')[0].ToString() != "0")
                {
                    message = "更新エラー";

                    if (retCsv.Split('\n').Length >= 2)
                        message += "\n" + retCsv.Split('\n')[0].ToString()[0].ToString() + " " + retCsv.Split('\n')[1].ToString();

                    if (!string.IsNullOrEmpty(AppData.ClassSatoo.AspxOpt2))
                        message += "\n" + AppData.ClassSatoo.AspxOpt2;

                    ClientLib.MessageBoxError(this,message,"エラー");
                }
                else
                {
                    string cell1 = retCsv.Split('\n')[0].ToString() ?? "";
                    string cell2 = retCsv.Split('\n')[1].ToString() ?? "";

                    if (cell1.Contains(":"))
                    {
                        message = $"{cell1} {cell2}\n※エラーがある場合、HHTエラーデータ修正画面で確認してください";
                    }
                    else
                    {
                        message = "取込件数:" + cell1;
                    }

                    ClientLib.MessageBox(this, message, "確認");
                    
                }

                string retStr = retCsv.Split('\n')[0].ToString()?.ToString() ?? "";
                var mes = retStr.Split('@');
                if (mes.Length >= 3 && mes[0] == "0")
                {
                    ClientLib.MessageBox(this,$"{mes[1]}\n{mes[2]}", "確認");
                }

                string deletePath = Path.Combine(workDir, fileName);
                if (File.Exists(deletePath))
                    File.Delete(deletePath);
            }
            catch (Exception ex)
            {
                ClientLib.MessageBoxError(this, ex.Message, "エラー");
            }
        }
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
    }
}
