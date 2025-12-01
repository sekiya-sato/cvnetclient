using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static CvnetClient.ViewModels.SubDlg00PrnMenu01ViewModel;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg00TerminalIdViewModel : BaseViewModel
    {
        [ObservableProperty]
        public Dictionary<string, string>? selectedCondition;
        [ObservableProperty]
        private string mstName = "店舗";   // default
        [ObservableProperty]
        private MasterCodeSettingMenu? selectCodeSetting = new();
        public string getCode;
        [ObservableProperty]
        private string codeLabel;   // UUID label code
        [ObservableProperty]
        BtListHelper findShopCd = new();
        [ObservableProperty]
        BtListHelper findStockCd = new();

        private static string? _cachedMachineUuid;

        private string? printsql;

        public void OnInit(object? init_para = null, string? init_flg = null)
        {
            OnInitBase(init_para, init_flg);

        }

        string[] sql_col_list =
        {
            "端末ID", "得意先CD", "メモ1", "メモ2"
        };


        [RelayCommand]
        void DoGenerateIIUD()
        {
            CodeLabel = GetMachineUUID();
        }

        [RelayCommand]
        void DoAssignIIUD()
        {
            //var wrk_csv = OnQuery();
            if (!ClientLib.MessageBox(this, "新規登録しますか？")) return;
            var item = Common.CloneObject(SelectCodeSetting);
            Common.ConvertDotStringAdd(item);


            if (item == null) return;

            var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.INSERT, "Master_UUID", 0, "0",
            new string[] { "端末ID", "得意先CD" },
            new string[] { item.DeviceID!, item.StockCd!});

            if (ret.Code == 0)
            {
                item.SeqNo = ret.NewSeq;
                item.VdateUpdate = decimal.Parse(ret.VDate);
                item.VdateCreate = item.VdateUpdate;
                Common.ConvertDotStringDel(item);
                //ListShohinJan!.Add(item);
                //SelectedProduct = item;
                ClientLib.MessageBoxOk(this, "登録しました");
            }
            else
            {
                ClientLib.MessageBoxError(this, ret.Code.ToString());
            }
        }

        [RelayCommand]
        public void FindShop(SelValueModel value)
        {
            if (value == null) return;

            FindShopCd = new BtListHelper(value.Code, value.Name);

            SelectCodeSetting.ShopCd = value.Code;
            SelectCodeSetting.ShopName = value.Name;
        }

        [RelayCommand]
        public void FindStock(SelValueModel value)
        {
            if (value == null) return;

            FindStockCd = new BtListHelper(value.Code, value.Name);

            SelectCodeSetting.StockCd = value.Code;
            SelectCodeSetting.StockName = value.Name;
        }

        [RelayCommand]
        public void UpdateCode(SelValueModel value)
        {
            var v_para = new BizArray();
            var qs = "select a.seq_no,a.vdate_create,a.vdate_update,a.得意先CD,a.端末ID,";
            qs += " nvl((select b.得意先名 from hc$master_tokui b where b.得意先CD = a.得意先CD),'該当無し') 得意先名 from hc$master_uuid a where a.端末ID = :1";
            
        }

        // for 追加 button
        [RelayCommand]
        void DoInsert()
        {
            //if (!ClientLib.MessageBox(this, "新規登録しますか？")) return;
            //var item = Common.CloneObject(EditProduct);
            //Common.ConvertDotStringAdd(item);

            //if (item == null) return;

            //var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.INSERT, "Master_UUID", 0, "0",
            //new string[] { "端末ID", "得意先CD"},
            //new string[] { item.ProductCD!, item.ColorCD!, item.SizeCD!, item.JanCode1!, item.JanCode2!,
            //                    item.JanCode3!, item.Memo!, item.UseFlag.ToString()!, item.ScheduledProductionQuantity.ToString()!, item.CuttingQuantity.ToString()!, item.TagNumber.ToString()!, item.AutoAllocationFlag.ToString()!, item.RetailPrice.ToString()!, item.SupplierPrice.ToString()!, item.ForeignCurrencyPrice.ToString()!, item.CostPrice.ToString()!});

            //var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.INSERT, "Master_SHOHIN_JAN", 0, "0",
            //       new string[] { "商品CD", "色CD", "サイズCD", "JANコード1", "JANコード2", "JANコード3",
            //                    "メモ", "使用FLG", "生産予定数", "裁断数", "下札枚数",
            //                    "自動配分FLG", "上代", "仕入価格", "外貨仕入価格", "原価"},
            //       new string[] { item.ProductCD!, item.ColorCD!, item.SizeCD!, item.JanCode1!, item.JanCode2!,
            //                    item.JanCode3!, item.Memo!, item.UseFlag.ToString()!, item.ScheduledProductionQuantity.ToString()!, item.CuttingQuantity.ToString()!, item.TagNumber.ToString()!, item.AutoAllocationFlag.ToString()!, item.RetailPrice.ToString()!, item.SupplierPrice.ToString()!, item.ForeignCurrencyPrice.ToString()!, item.CostPrice.ToString()!});

            //if (ret.Code == 0)
            //{
            //    item.SeqNo = ret.NewSeq;
            //    item.VdateUpdate = decimal.Parse(ret.VDate);
            //    item.VdateCreate = item.VdateUpdate;
            //    Common.ConvertDotStringDel(item);
            //    ListShohinJan!.Add(item);
            //    SelectedProduct = item;
            //    ClientLib.MessageBoxOk(this, "登録しました");
            //}
            //else
            //{
            //    ClientLib.MessageBoxError(this, ret.Code.ToString());
            //}
        }


        public static string GetMachineUUID()
        {
            if (_cachedMachineUuid != null)
                return _cachedMachineUuid;

            // 1. Get first active MAC address
            var macBytes = NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(nic =>
                    nic.OperationalStatus == OperationalStatus.Up &&
                    nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .Select(nic => nic.GetPhysicalAddress().GetAddressBytes())
                .FirstOrDefault();

            // fallback if MAC unavailable
            if (macBytes == null || macBytes.Length == 0)
                macBytes = Encoding.UTF8.GetBytes(Environment.MachineName);

            // 2. SHA-1 hash of MAC → long consistent byte array
            using var sha1 = SHA1.Create();
            byte[] hash = sha1.ComputeHash(macBytes);  // 20 bytes

            // 3. Use first 16 bytes as UUID
            byte[] uuidBytes = hash.Take(16).ToArray();

            // 4. Convert to UUID format 8-4-4-4-12 with "-"
            string hex = BitConverter.ToString(uuidBytes).Replace("-", "").ToUpper();

            string uuid =
                hex.Substring(0, 8) + "-" +
                hex.Substring(8, 4) + "-" +
                hex.Substring(12, 4) + "-" +
                hex.Substring(16, 4) + "-" +
                hex.Substring(20, 12);

            _cachedMachineUuid = uuid;
            return uuid;
        }

        private DataTable OnQuery()
        {
            string sql_query1 = "SELECT t.seq_no, t.vdate_create,t.vdate_update,t.得意先CD, nvl((select t0.得意先名 from hc$master_tokui t0 where t0.得意先CD = t.得意先CD),'') 得意先名,端末ID,";
            sql_query1 += "decode(メモ1,'.','',メモ1)||' '||nvl((select a.得意先名 from hc$master_tokui a where a.得意先CD=メモ1),'') メモ1 ,";
            sql_query1 += "decode(メモ2,'.','',メモ2)||' '||nvl((select a.得意先名 from hc$master_tokui a where a.得意先CD=メモ2),'') メモ2 from HC$MASTER_UUID t where t.得意先CD = :1";

            sql_query1 = AppData.ClassCvnet.GetSqlDisp(sql_query1);

            var ret_csv = AppData.Http?.AspxSqlQuery(sql_query1);

            return ret_csv;
        }

        public partial class MasterCodeSettingMenu : ObservableObject
        {
            [ObservableProperty]
            private string? shopCd;
            [ObservableProperty]
            private string? shopName;
            [ObservableProperty]
            private string? deviceID;
            [ObservableProperty]
            private string? stockCd;
            [ObservableProperty]
            private string? stockName;
            [ObservableProperty]
            private string? clientCd;
            [ObservableProperty]
            private long seqNo;
            [ObservableProperty]
            private decimal vdateCreate;
            [ObservableProperty]
            private decimal vdateUpdate;
        }
    }
}
