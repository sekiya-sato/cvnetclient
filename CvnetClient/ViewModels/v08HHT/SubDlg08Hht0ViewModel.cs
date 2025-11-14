using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg08Hht0ViewModel : BaseViewModel
    {
        [ObservableProperty]
        string? pathStr;

        [ObservableProperty]
        string? hhtPath;

        [ObservableProperty]
        string? message;

        public void OnInit()
        {
            try { 
                if(AppData.ClassCvnet.HHT_Csv.GetEnv() == "") message="初期設定がされてません";
            }
            catch {
                message ="初期設定がされてません";
            }

            if(AppData.ClassCvnet.SysHhtMst.Rows[0][33].ToString() == "1") PathStr = "C:\\";
        }

        [RelayCommand]
        public void OnSet()
        {
            AppData.ClassCvnet.HHT_Csv.SetEnv(pathStr);
            hhtPath = "";
            var fs = new FileSystem();
            for (var i=0; i< AppData.ClassCvnet.HHT_Csv._data.Rows.Count; i++) {
                hhtPath += AppData.ClassCvnet.HHT_Csv._data.Rows[i][1] + "\n";
                hhtPath += " " + fs.MakePath(AppData.ClassCvnet.HHT_Csv.GetEnv(), AppData.ClassCvnet.HHT_Csv._data.Rows[i][2].ToString()) + "\n";
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
