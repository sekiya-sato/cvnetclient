using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System.Windows;
using static CvnetClient.ViewModels.SubDlg01SetjanViewModel;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.AxHost;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg71RecvManualViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string label4 = string.Empty;

        [ObservableProperty]
        private string text2 = string.Empty;

        [ObservableProperty]
        private int numberEdit1 = 1;

        [ObservableProperty]
        private DateTime dateEdit1 = DateTime.Now;

        public enum OptionButton1
        { POS = 0, WMS = 1 }

        public enum OptionButton2
        { 売上 = 10, 入出庫 = 100 , 棚卸 = 1}

        public enum OptionButton3
        { ワークのみ = 0, 伝票のみ = 1, 両方 = 2 }

        [ObservableProperty]
        private OptionButton1 selectedButton1 = OptionButton1.WMS;

        [ObservableProperty]
        private OptionButton2 selectedButton2 = OptionButton2.売上;

        [ObservableProperty]
        private OptionButton3 selectedButton3 = OptionButton3.ワークのみ;

        public void OnInit(object? init_para = null, string? init_flg = null)
        {
            OnInitBase(init_para, init_flg);
        }

        [RelayCommand]
        void DoList()
        {
            if (!ClientLib.MessageBox(this, "受信データを取り込んでよろしいですか？")) return;
            DateTime start0 = DateTime.Now;
            string[] v_para = new string[5];
            int DoFlg = 1;
            v_para[0] = new string("WMS");
            v_para[1] = new string(DateEdit1.ToString("yyyy/MM/dd HH:mm:ss"));
            v_para[2] = new string("1");
            v_para[3] = new string(NumberEdit1.ToString());
            v_para[4] = new string( ((int)selectedButton3).ToString() );

            var wrk_csv = AppData.Http!.AspxSqlQuery2("Recv_Manual", v_para , "", 39);

            var rows = wrk_csv.Split('\n');
            
            string[] cols0 = new string[4];
            string[] cols1 = new string[4];
            string[] cols2 = new string[4];
            string[] cols3 = new string[4];

            for (int i = 0; i < rows.Length; i++) {
                if (i == 0) cols0 = rows[0].Split(',');
                if (i == 1) cols1 = rows[1].Split(',');
                if (i == 2) cols2 = rows[2].Split(',');
                if (i == 3) cols3 = rows[3].Split(',');
            }

            int ret = 0;
            Int32.TryParse(cols0[0], out ret );
            if (ret <0)
            {
                ClientLib.MessageBoxError(this, "CV-71002 受信データ取込エラー");
                return;
            }
            Text2 += "*****処理開始*****************************\n";
            if (selectedButton1 == OptionButton1.POS)
            {
                for (var j = 0; j < cols0.Length; j++)
                {
                    if (j > 0) Text2 += ",";
                    Text2 += "POSDB取込=" + cols0[j];
                }
                Text2 += "\n";

                for (var j = 0; j < cols1.Length; j++)
                {
                    if (j > 0) Text2 += ",";
                    Text2 += "POS売上=" + cols1[j];
                }
                Text2 += "\n";

                for (var j = 0; j < cols2.Length; j++)
                {
                    if (j > 0) Text2 += ",";
                    Text2 += "POS入出庫=" + cols2[j];
                }
                Text2 += "\n";

                for (var j = 0; j < cols3.Length; j++)
                {
                    if (j > 0) Text2 += ",";
                    Text2 += "POS棚卸=" + cols3[j];
                }
                Text2 += "\n";


                //Text2 += "POSDB取込=" + cols0[0] + "," + cols0[1] + "," + cols0[2] + "\n";
                //Text2 += "POS売上=" + cols1[0] + "," + cols1[1] + "," + cols1[2] + "\n";
                //Text2 += "POS入出庫=" + cols2[0] + "," + cols2[1] + "," + cols2[2] + "\n";
                //Text2 += "POS棚卸=" + cols3[0] + "," + cols3[1] + "," + cols3[2] + "\n";
            }
            else {
                for (var i = 1; i < rows.Length; i++)
                {
                    var cols = rows[i].Split(',');
                    for (var j = 0; j < cols.Length; j++) 
                    { 
                         Text2 += cols[j] + " "; 
                    }
                    Text2 += "\n";
                }
            }
            Text2 += "*****処理終了*****************************\n";
            DateTime end0 = DateTime.Now;
            Label4 = "データ作成しました \n終了時刻:"
                    + DateTime.Now.ToString() + "\n経過時間:" + (end0 - start0).ToString(@"hh\:mm\:ss");
            return;
        }

    }
}
