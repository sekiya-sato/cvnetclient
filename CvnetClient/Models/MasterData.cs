/* ============================================================================
 * Cvnet8wpfclient.exe : MasterData.cs
 * Created by Sekiya.Sato 2025/05/21
 * 説明: マスターの定義
 * 使用ライブラリ [Library used]:
 *		CommunityToolkit.Mvvm : LICENCE = MIT
 * ============================================================================  */
using CommunityToolkit.Mvvm.ComponentModel;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace CvnetClient.Models
{
	internal class MasterData
	{
	}
	public partial class MasterMeisho : ObservableObject
	{
		[ObservableProperty]
		long seqNo;
		[ObservableProperty]
		decimal vdateCreate;
		[ObservableProperty]
		decimal vdateUpdate;
		[ObservableProperty]
		string? kubun;
		[ObservableProperty]
		string? meishoCd;
		[ObservableProperty]
		string? meisho;
		[ObservableProperty]
		string? ryakuShou;
		[ObservableProperty]
		string? rank;
		[ObservableProperty]
		string? renban;
		[ObservableProperty]
		string? kana;
		[ObservableProperty]
		string? posKubun;
		[ObservableProperty]
		string? saishuuShuuseiSha;
	}
	
	public partial class MasterShohin : ObservableObject
	{
		[ObservableProperty]
		long seqNo;
		[ObservableProperty]
		decimal vdateCreate;
		[ObservableProperty]
		decimal vdateUpdate;
		[ObservableProperty]
		string? productCD; //商品CD
		[ObservableProperty]
		string? productName; //商品名
		[ObservableProperty]
		string? abbreviation; //略称
		[ObservableProperty]
		string? oldCode; //旧コード
		[ObservableProperty]
		string? exhibitCD; // 展示会CD
		[ObservableProperty]
		string? brandCD; //ブランドCD
		[ObservableProperty]
		string? itemCD; //アイテムCD
		[ObservableProperty]
		string? seasonCD; //シーズンCD
		[ObservableProperty]
		string? materialCD;//素材CD
		[ObservableProperty]
		string? designCD; //デザイナーCD
		[ObservableProperty]
		string? manufactCD; //メーカーCD
		[ObservableProperty]
		string? madeInCD; //原産国CD 
		[ObservableProperty]
		decimal oriPrice; //元上代
		[ObservableProperty]
        decimal price; //上代
		[ObservableProperty]
		DateTime? priceChgDate; //売変日
		[ObservableProperty]
        decimal cost; //原価
		[ObservableProperty]
        decimal opCostPrice; //営業原価
		[ObservableProperty]
        decimal manufactFee; //加工工賃
		[ObservableProperty]
		DateTime? custDeliDate; //デリバリー日
		[ObservableProperty]
		DateTime? deliveryDate; //納品日
		[ObservableProperty]
		DateTime? initLaunchDate; //店頭投入日
		[ObservableProperty]
		string? janCode1; //JANコード1
		[ObservableProperty]
		string? janCode2; //JANコード2
		[ObservableProperty]
		string? janCode3; //JANコード3
		[ObservableProperty]
		string? careLabel; //洗濯表示
		[ObservableProperty]
		string? imgName; //絵型名
		[ObservableProperty]
		string? memo; //メモ
		[ObservableProperty]
		int taxCalcMethod; //消費税計算方法
		[ObservableProperty]
		int invMngmentFLG; //在庫管理FLG
		[ObservableProperty]
		long taxCD; //消費税CD
		[ObservableProperty]
		string? nameCD01; //名称CD01
		[ObservableProperty]
		string? nameCD02; //名称CD02
		[ObservableProperty]
		string? nameCD03; //名称CD03
		[ObservableProperty]
		string? nameCD04; //名称CD04
		[ObservableProperty]
		string? nameCD05; //名称CD05
		[ObservableProperty]
		string? nameCD06; //名称CD06
		[ObservableProperty]
		string? nameCD07; //名称CD07
		[ObservableProperty]
		string? nameCD08; //名称CD08
		[ObservableProperty]
		string? nameCD09; //名称CD09
		[ObservableProperty]
		string? nameCD10; //名称CD10
		[ObservableProperty]
		int autoDistFLG; //自動配分FLG
		[ObservableProperty]
		string? salesPeriod; //販売期限
		[ObservableProperty]
		int prodCateFLG; //商品区分FLG
		[ObservableProperty]
		string? prodSizeCate; //商品サイズ区分
		[ObservableProperty]
		string? standWareCD; //基準倉庫CD
		[ObservableProperty]
		int zeroPriceCate; //ゼロ単価区分
		[ObservableProperty]
		string? jan1stdigit; //JAN先頭桁
		[ObservableProperty]
		int posCate; //POS区分
		[ObservableProperty]
		string? reserve01; //予備01
		[ObservableProperty]
		string? reserve02; //予備02
		[ObservableProperty]
		string? reserve03; //予備03
		[ObservableProperty]
		string? reserve04; //予備04
		[ObservableProperty]
		string? reserve05; //予備05
		[ObservableProperty]
		string? reserve06; //予備06
		[ObservableProperty]
		string? reserve07; //予備07
		[ObservableProperty]
		string? reserve08; //予備08
		[ObservableProperty]
		string? reserve09; //予備09
		[ObservableProperty]
		string? reserve10; //予備10
		[ObservableProperty]
		string? reserve11; //予備11
		[ObservableProperty]
		string? reserve12; //予備12
		[ObservableProperty]
		string? reserve13; //予備13
		[ObservableProperty]
		string? reserve14; //予備14
		[ObservableProperty]
		string? reserve15; //予備15
		[ObservableProperty]
		string? reserve16; //予備16
		[ObservableProperty]
		string? reserve17; //予備17
		[ObservableProperty]
		string? reserve18; //予備18
		[ObservableProperty]
		string? reserve19; //予備19
		[ObservableProperty]
		string? reserve20; //予備20
		[ObservableProperty]
		int purchaseCate;  //仕入区分
		[ObservableProperty]
		int dgCutOffSpec; //消化桁切指定
		[ObservableProperty]
		int consignPurcCate; //消化端数区分
		[ObservableProperty]
		int dgCalcCate; //消化計算区分
		[ObservableProperty]
		int consignPurcRate; //消化掛率
		[ObservableProperty]
		string genderCate; //男女区分
		[ObservableProperty]
		int collabOutCate; //コラボ出力区分
		[ObservableProperty]
		int representNoFLG; //代表品番FLG
		[ObservableProperty]
		int salesCate; //セール区分
		[ObservableProperty]
		DateTime? repeatDate; //リピート日
		[ObservableProperty]
		string? makerNo; //メーカー品番
		[ObservableProperty]
        decimal purchasePrice; //仕入価格
		[ObservableProperty]
		int deliveryCate; //納品区分
		[ObservableProperty]
		string? imgName2; //絵型名2
		[ObservableProperty]
		DateTime? saleStDate; //販売開始日
        [ObservableProperty]
        decimal foreignCurPrice; //外貨単価
		[ObservableProperty]
		int ecConnect; //EC連携
		[ObservableProperty]
		int ecReserve; //EC取置

		[ObservableProperty]
		string? exhibitName; //展示会名
		[ObservableProperty]
		string? brandName; //ブランド名
		[ObservableProperty]
		string? itemName; //アイテム名
		[ObservableProperty]
		string? seasonName; //シーズン名
		[ObservableProperty]
		string? materialName; //素材名
		[ObservableProperty]
		string? designName; //デザイナー名
		[ObservableProperty]
		string? manufactName; //メーカー名
		[ObservableProperty]
		string? madeInName; //原産国名
		[ObservableProperty]
		string? subName01; //補足01名
		[ObservableProperty]
		string? subName02; //補足02名
		[ObservableProperty]
		string? subName03; //補足03名
		[ObservableProperty]
		string? subName04; //補足04名
		[ObservableProperty]
		string? subName05; //補足05名
		[ObservableProperty]
		string? subName06; //補足06名
		[ObservableProperty]
		string? subName07; //補足07名
		[ObservableProperty]
		string? subName08; //補足08名
		[ObservableProperty]
		string? subName09; //補足09名
		[ObservableProperty]
		string? subName10; //補足10名
		[ObservableProperty]
		string? stockName; //倉庫名
		[ObservableProperty]
		string? modifiedBy; //最終修正者
		[ObservableProperty]
		string? genderCateName; //男女区分名
		[ObservableProperty]
		string? careLabelName; //洗濯表示名
		[ObservableProperty]
		string? approval; //承認
		[ObservableProperty]
		int costChgHistFLG; //商品原価変更履歴有無FLG
		[ObservableProperty]
		string? reserveName01; //予備01名
		[ObservableProperty]
		string? reserveName02; //予備02名
		[ObservableProperty]
		string? reserveName03; //予備03名
		[ObservableProperty]
		string? reserveName04; //予備04名
		[ObservableProperty]
		string? reserveName05; //予備05名
		[ObservableProperty]
		string? reserveName06; //予備06名
		[ObservableProperty]
		string? reserveName07; //予備07名
		[ObservableProperty]
		string? reserveName08; //予備08名
		[ObservableProperty]
		string? reserveName09; //予備09名
		[ObservableProperty]
		string? reserveName10; //予備10名
		[ObservableProperty]
		string? reserveName11; //予備11名
		[ObservableProperty]
		string? reserveName12; //予備12名
		[ObservableProperty]
		string? reserveName13; //予備13名
		[ObservableProperty]
		string? reserveName14; //予備14名
		[ObservableProperty]
		string? reserveName15; //予備15名
		[ObservableProperty]
		string? reserveName16; //予備16名
		[ObservableProperty]
		string? reserveName17; //予備17名
		[ObservableProperty]
		string? reserveName18; //予備18名
		[ObservableProperty]
		string? reserveName19; //予備19名
		[ObservableProperty]
		string? reserveName20; //予備20名
	}

    public partial class MasterWorker : ObservableObject
	{
        [ObservableProperty]
        long seqNo;
        [ObservableProperty]
        decimal vdateCreate;
        [ObservableProperty]
        decimal vdateUpdate;
		[ObservableProperty]
		string? workerCD;
		[ObservableProperty]
		string? name;
		[ObservableProperty]
		string? department;
		[ObservableProperty]
		string? shopCD;
		[ObservableProperty]
		int? salesFlg;
		[ObservableProperty]
		string? mail;
		[ObservableProperty]
		string? telNo;
		[ObservableProperty]
		string? specialFlg;
		[ObservableProperty]
		string? furigana;
		[ObservableProperty]
		string? positionCD;
		[ObservableProperty]
		string? employmentFLG;
        [ObservableProperty]
        int? outputFLG;
		[ObservableProperty]
        string? notes;
		[ObservableProperty]
        string? felica;
        [ObservableProperty]
        string? joiningDate;
        [ObservableProperty]
        string? vacationRemaining;
        [ObservableProperty]
        string? salaryCate;
		[ObservableProperty]
        int? salaryAmount;
        [ObservableProperty]
        string? transExpCate;
		[ObservableProperty]
        int? transExpAmount;
        [ObservableProperty]
        string? sectionCD;
        [ObservableProperty]
        string? nameCD01;
        [ObservableProperty]
        string? nameCD02;
        [ObservableProperty]
        string? nameCD03;
        [ObservableProperty]
        string? nameCD04;
        [ObservableProperty]
        string? nameCD05;
		[ObservableProperty]
        int? posCate;
        [ObservableProperty]
        int? emailFLG;
        [ObservableProperty]
        string? employeeInpCD;
		[ObservableProperty]
        int? specHolidayRemain;
        [ObservableProperty]
        string? endDate;
        [ObservableProperty]
        string? retireDate;
        [ObservableProperty]
        string? profile;
    }

	public partial class MasterShop : ObservableObject
	{
        [ObservableProperty]
        long seqNo;
        [ObservableProperty]
        decimal vdateCreate;
        [ObservableProperty]
        decimal vdateUpdate;
		[ObservableProperty]
		string? tradingCD;
		[ObservableProperty]
		string? oldCD;
		[ObservableProperty]
		string? abbr;
		[ObservableProperty]
		string? tradingName;
		[ObservableProperty]
		string? katakana;
		[ObservableProperty]
		string? postal;
		[ObservableProperty]
		string? address1;
		[ObservableProperty]
		string? address2;
		[ObservableProperty]
		string? address3;
		[ObservableProperty]
		string? telNo;
		[ObservableProperty]
		string? faxNo;
		[ObservableProperty]
		string? salesRepCD;
		[ObservableProperty]
		string? storeCate;
		[ObservableProperty]
		string? area;
		[ObservableProperty]
		string? commissionRate;
		[ObservableProperty]
		string? saleCommRate;
		[ObservableProperty]
		string? invoiceAddrCD;
		[ObservableProperty]
		string? invoicePrint;
		[ObservableProperty]
		string? closingDate;
		[ObservableProperty]
		string? expectedPayMonth;
		[ObservableProperty]
		string? expectedPayDate;
		[ObservableProperty]
		string? payMethod;
		[ObservableProperty]
		string? lowerPriceCutoffSpec;
		[ObservableProperty]
		string? lowerPriceFracCate;
		[ObservableProperty]
		string? lowerPriceCalcFLG;
		[ObservableProperty]
		string? consumpTaxCD;
		[ObservableProperty]
		string? consumpTaxCalc;
		[ObservableProperty]
		string? consumpTaxFrac;
		[ObservableProperty]
		string? creditLimit;
		[ObservableProperty]
		string? paymentRate;
		[ObservableProperty]
		string? shipmentStopFlg;
		[ObservableProperty]
		string? addressFLG1;
		[ObservableProperty]
		string? addressFLG2;
		[ObservableProperty]
		string? addressFLG3;
		[ObservableProperty]
		string? addressName1;
		[ObservableProperty]
		string? addressName2;
		[ObservableProperty]
		string? invoiceIssueCate;
		[ObservableProperty]
		string? invoicePrint1;
		[ObservableProperty]
		string? invoicePrint2;
		[ObservableProperty]
		string? invoicePrint3;
		[ObservableProperty]
		string? invoicePrint4;
		[ObservableProperty]
		string? department;
		[ObservableProperty]
		string? notes;
		[ObservableProperty]
		string? invManageFLG;
		[ObservableProperty]
		string? autoAllocFLG;
		[ObservableProperty]
		string? startDate;
		[ObservableProperty]
		string? endDate;
		[ObservableProperty]
		string? nameCD01;
		[ObservableProperty]
		string? nameCD02;
		[ObservableProperty]
		string? nameCD03;
		[ObservableProperty]
		string? nameCD04;
		[ObservableProperty]
		string? nameCD05;
		[ObservableProperty]
		string? nameCD06;
		[ObservableProperty]
		string? nameCD07;
		[ObservableProperty]
		string? nameCD08;
		[ObservableProperty]
		string? nameCD09;
		[ObservableProperty]
		string? nameCD10;
		[ObservableProperty]
		string? invDate;
		[ObservableProperty]
		string? startTime;
		[ObservableProperty]
		string? endTime;
		[ObservableProperty]
		string? terminalID;
		[ObservableProperty]
		string? warehouseCate;
		[ObservableProperty]
		string? businHours1;
		[ObservableProperty]
		string? businHours2;
		[ObservableProperty]
		string? businHours3;
		[ObservableProperty]
		string? contractInfo;
		[ObservableProperty]
		string? developer;
		[ObservableProperty]
		string? businessHours;
		[ObservableProperty]
		string? invDateEND;
		[ObservableProperty]
		string? exchangeCate;
		[ObservableProperty]
		string? exDeciCutoff;
		[ObservableProperty]
		string? exFracCate;
		[ObservableProperty]
		string? allocRank01;
		[ObservableProperty]
		string? allocRank02;
		[ObservableProperty]
		string? shippingFLG;
		[ObservableProperty]
		string? baseWarehouseFLG;
		[ObservableProperty]
		string? baseWarehouseCD;
		[ObservableProperty]
		string? allocMethodFLG;
		[ObservableProperty]
		string? posCate;
		[ObservableProperty]
		string? slipPrint5;
		[ObservableProperty]
		string? slipPrint6;
		[ObservableProperty]
		string? slipPrint7;
		[ObservableProperty]
		string? slipPrint8;
		[ObservableProperty]
		string? enterEmployeeCD;
		[ObservableProperty]
		string? storeSalesFloorCode;
		[ObservableProperty]
		string? ecFLG;
		[ObservableProperty]
		string? baseSales01;
		[ObservableProperty]
		string? baseSales02;
		[ObservableProperty]
		string? baseSales03;
		[ObservableProperty]
		string? commissionRate01;
		[ObservableProperty]
		string? commissionRate02;
		[ObservableProperty]
		string? commissionRate03;
		[ObservableProperty]
		string? otherRatio01;
		[ObservableProperty]
		string? otherRatio02;
		[ObservableProperty]
		string? otherRatio03;
		[ObservableProperty]
		string? otherExpenses01;
		[ObservableProperty]
		string? otherExpenses02;
    }

    public partial class MasterSHKiji : ObservableObject
    {
        [ObservableProperty]
        long seqNo;
        [ObservableProperty]
        decimal vdateCreate;
        [ObservableProperty]
        decimal vdateUpdate;
        [ObservableProperty]
        string? product; //商品CD
        [ObservableProperty]
        string? oldCD; //旧コード
		[ObservableProperty]
		string? abbreviation; //略称
        [ObservableProperty]
        string? productName; //商品名
        [ObservableProperty]
        string? cateCd; //区分CD
        [ObservableProperty]
        string? supplierCd; //仕入先CD
        [ObservableProperty]
        string? supplierProdCd; //仕入先商品CD
        [ObservableProperty]
        decimal unitPrice; //単価
        [ObservableProperty]
        string? memo; //メモ
        [ObservableProperty]
        string? inpStaffCD; //入力社員CD

    }

}
