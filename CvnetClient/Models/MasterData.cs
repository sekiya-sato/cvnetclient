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
		string? workerCD;	//社員CD
		[ObservableProperty]
		string? name;	//名前
		[ObservableProperty]
		string? department; //部門
        [ObservableProperty]
		string? shopCD; //店舗CD
		[ObservableProperty]
		string? shopName;
        [ObservableProperty]
		int? salesFlg;  //営業FLG
        [ObservableProperty]
		string? mail;   //メール
        [ObservableProperty]
		string? telNo;  //携帯TEL
        [ObservableProperty]
		string? specialFlg; //特権FLG
        [ObservableProperty]
		string? furigana;   //フリガナ
        [ObservableProperty]
		string? positionCD; //役職CD
        [ObservableProperty]
		string? employmentFLG;	//就業FLG
        [ObservableProperty]
        int? outputFLG; //出力FLG
        [ObservableProperty]
        string? notes;  //備考
        [ObservableProperty]
        string? felica;
        [ObservableProperty]
        DateTime? joiningDate;	//入社日
        [ObservableProperty]
        string? vacationRemaining;	//有給残
        [ObservableProperty]
        string? salaryCate; //給与区分
        [ObservableProperty]
        int? salaryAmount;	//給与支給額
        [ObservableProperty]
        string? transExpCate;   //交通費区分
        [ObservableProperty]
        int? transExpAmount;	//交通費支給額
        [ObservableProperty]
        string? sectionCD;	//部課CD
        [ObservableProperty]
        string? nameCD01;	//名称CD01
        [ObservableProperty]
        string? nameCD02;	//名称CD02
        [ObservableProperty]
        string? nameCD03;	//名称CD03
        [ObservableProperty]
        string? nameCD04;	//名称CD04
        [ObservableProperty]
        string? nameCD05;   //名称CD05
        [ObservableProperty]
        int? posCate;	//POS区分
        [ObservableProperty]
        int? emailFLG;	//メールFLG
        [ObservableProperty]
        string? employeeInpCD;  //入力社員CD
        [ObservableProperty]
        int? specHolidayRemain;	//特休残
        [ObservableProperty]
        string? endDate;	//退勤日
        [ObservableProperty]
        DateTime? retireDate;	//退職日
        [ObservableProperty]
        string? profile;	//プロフィール
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
		string? tradingCD;  //得意先CD
        [ObservableProperty]
		string? oldCD;  //旧コード
        [ObservableProperty]
		string? abbr;   //略称
        [ObservableProperty]
		string? tradingName;    //得意先名
        [ObservableProperty]
		string? katakana;   //カナ
        [ObservableProperty]
		string? postal; //郵便番号
        [ObservableProperty]
		string? address1;   //住所1
        [ObservableProperty]
		string? address2;   //住所2
        [ObservableProperty]
		string? address3;   //住所3
        [ObservableProperty]
		string? telNo;  //TEL
        [ObservableProperty]
		string? faxNo;  //FAX
        [ObservableProperty]
		string? salesRepCD; //営業担当CD
        [ObservableProperty]
		int? storeCate;  //店種区分
        [ObservableProperty]
		long? area;   //坪数
        [ObservableProperty]
		double? commissionRate; //掛率
        [ObservableProperty]
		double? saleCommRate;   //セール掛率
		[ObservableProperty]
		double? inStoreSaleRate;	//店頭セール掛率
        [ObservableProperty]
		string? invoiceAddrCD;  //請求先CD
        [ObservableProperty]
		int? invoicePrint;   //請求印刷
        [ObservableProperty]
		int? closingDate;    //締日
        [ObservableProperty]
		int? expectedPayMonth;   //入金予定月
        [ObservableProperty]
		int? expectedPayDate;    //入金予定日
        [ObservableProperty]
		string? payMethod;  //入金方法
        [ObservableProperty]
		int? lowerPriceCutoffSpec;   //下代桁切指定
        [ObservableProperty]
		int? lowerPriceFracCate; //下代端数区分
        [ObservableProperty]
		int? lowerPriceCalcFLG;  //下代計算FLG
        [ObservableProperty]
		int? consumpTaxCD;   //消費税CD
        [ObservableProperty]
		int? consumpTaxCalc; //消費税計算方法
        [ObservableProperty]
		int? consumpTaxFrac; //消費税端数
        [ObservableProperty]
		int? creditLimit;    //与信限度額
        [ObservableProperty]
		int? paymentRate;    //入金率
        [ObservableProperty]
		int? shipmentStopFlg;    //出荷停止FLG
        [ObservableProperty]
		string? addressFLG1;    //宛名FLG1
        [ObservableProperty]
		string? addressFLG2;    //宛名FLG2
        [ObservableProperty]
		string? addressFLG3;    //宛名FLG3
        [ObservableProperty]
		string? addressName1;   //宛名名称1
        [ObservableProperty]
		string? addressName2;   //宛名名称2
        [ObservableProperty]
		int? invoiceIssueCate;   //伝票発行区分
        [ObservableProperty]
		string? invoicePrint1;  //伝票印字1
        [ObservableProperty]
		string? invoicePrint2;  //伝票印字2
        [ObservableProperty]
		string? invoicePrint3;  //伝票印字3
        [ObservableProperty]
		string? invoicePrint4;  //伝票印字4
        [ObservableProperty]
		string? department; //部門
        [ObservableProperty]
		string? notes;  //備考
        [ObservableProperty]
		int? invManageFLG;   //在庫管理FLG
        [ObservableProperty]
		int? autoAllocFLG;   //自動配分FLG
        [ObservableProperty]
		string? startDate;  //開始日
        [ObservableProperty]
		string? endDate;    //終了日
        [ObservableProperty]
		string? nameCD01;   //名称CD01
        [ObservableProperty]
		string? nameCD02;   //名称CD02
        [ObservableProperty]
		string? nameCD03;   //名称CD03
        [ObservableProperty]
		string? nameCD04;   //名称CD04
        [ObservableProperty]
		string? nameCD05;   //名称CD05
        [ObservableProperty]
		string? nameCD06;   //名称CD06
        [ObservableProperty]
		string? nameCD07;   //名称CD07
        [ObservableProperty]
		string? nameCD08;   //名称CD08
        [ObservableProperty]
		string? nameCD09;   //名称CD09
        [ObservableProperty]
		string? nameCD10;   //名称CD10
        [ObservableProperty]
		string? invDate;    //棚卸日
        [ObservableProperty]
		string? startTime;  //開始時刻
        [ObservableProperty]
		string? endTime;    //終了時刻
        [ObservableProperty]
		string? terminalID; //端末ID
        [ObservableProperty]
		int? warehouseCate;  //倉庫区分
        [ObservableProperty]
		string? businHours1;    //営業時間1
        [ObservableProperty]
		string? businHours2;    //営業時間2
        [ObservableProperty]
		string? businHours3;    //営業時間3
        [ObservableProperty]
		string? contractInfo;   //施工業者情報
        [ObservableProperty]
		string? developer;  //デベロッパ
        [ObservableProperty]
		double? businessHours;  //営業時間
        [ObservableProperty]
		string? invDateEND; //棚卸日END
        [ObservableProperty]
		string? exchangeCate;   //為替区分
        [ObservableProperty]
		int? exDeciCutoff;   //為替桁切指定
        [ObservableProperty]
		int? exFracCate; //為替端数区分
        [ObservableProperty]
		string? allocRank01;    //配分ランク01
        [ObservableProperty]
		string? allocRank02;    //配分ランク02
        [ObservableProperty]
		int? shippingFLG;    //出荷FLG
        [ObservableProperty]
		int? baseWarehouseFLG;   //基準倉庫FLG
        [ObservableProperty]
		string? baseWarehouseCD;    //基準倉庫CD
        [ObservableProperty]
		int? allocMethodFLG; //配分方法FLG
        [ObservableProperty]
		int? posCate;    //POS区分
        [ObservableProperty]
		string? slipPrint5; //伝票印字5
        [ObservableProperty]
		string? slipPrint6; //伝票印字6
        [ObservableProperty]
		string? slipPrint7; //伝票印字7
        [ObservableProperty]
		string? slipPrint8; //伝票印字8
        [ObservableProperty]
		string? enterEmployeeCD;    //入力社員CD
        [ObservableProperty]
		string? storeSalesFloorCode;    //店舗売場コード
        [ObservableProperty]
		int? ecFLG;
        [ObservableProperty]
		long? baseSales01;    //基準売上01
        [ObservableProperty]
		long? baseSales02;    //基準売上02
        [ObservableProperty]
		long? baseSales03;    //基準売上03
        [ObservableProperty]
		double? commissionRate01;   //歩率01
        [ObservableProperty]
		double? commissionRate02;   //歩率02
        [ObservableProperty]
		double? commissionRate03;   //歩率03
        [ObservableProperty]
		double? otherRatio01;   //他比率01
        [ObservableProperty]
		double? otherRatio02;   //他比率02
        [ObservableProperty]
		double? otherRatio03;   //他比率03
        [ObservableProperty]
		double? otherExpenses01;    //他費用01
        [ObservableProperty]
		double? otherExpenses02;    //他費用02
        [ObservableProperty]
        double? otherExpenses03;    //他費用03
        [ObservableProperty]
        string? nameCD11;   //名称CD11
        [ObservableProperty]
        string? nameCD12;   //名称CD12
        [ObservableProperty]
        string? nameCD13;   //名称CD13
        [ObservableProperty]
        string? nameCD14;   //名称CD14
        [ObservableProperty]
        string? nameCD15;   //名称CD15
        [ObservableProperty]
        string? nameCD16;   //名称CD16
        [ObservableProperty]
        string? nameCD17;   //名称CD17
        [ObservableProperty]
        string? nameCD18;   //名称CD18
        [ObservableProperty]
        string? nameCD19;   //名称CD19
        [ObservableProperty]
        string? nameCD20;   //名称CD20
        [ObservableProperty]
        int? rentCalcFLG;        //賃料計算FLG
        [ObservableProperty]
        long? minRent;       //最低保障家賃
        [ObservableProperty]
        int? closeDate2;    //締日2
		[ObservableProperty]
		int? closeDate3;        //締日3
		[ObservableProperty]
		int? xpPayMon2;     //入金予定月2
		[ObservableProperty]
		int? xpPayDate2;        //入金予定日2
		[ObservableProperty]
		int? xpPayMon3;     //入金予定月3
		[ObservableProperty]
		int? xpPayDate3;        //入金予定日3
		[ObservableProperty]
		string? invoCompName;       //伝票社名
		[ObservableProperty]
		string? invoStoreName;      //伝票店名
		[ObservableProperty]
		int? transCate;     //移動区分
		[ObservableProperty]
		string? corpCD;     //法人CD
		[ObservableProperty]
		string? affiCD;     //連携CD
		[ObservableProperty]
		string? affiCorpCD;     //連携先法人CD
		[ObservableProperty]
		string? payDesti1;      //振込先1
		[ObservableProperty]
		string? payDesti2;      //振込先2
		[ObservableProperty]
		string? payDesti3;      //振込先3
		[ObservableProperty]
		string? custBrdCD;      //得意先ブランドCD
		[ObservableProperty]
		int? dueDate;       //期日
		[ObservableProperty]
		int? minAm;     //下限額
		[ObservableProperty]
		int? transDays;     //移動日数
		[ObservableProperty]
		string? custEmail;      //得意先MAIL
		[ObservableProperty]
		string? storeImgName;       //店舗画像名
		[ObservableProperty]
		string? closedDays;     //定休日
		[ObservableProperty]
		string? appeal;     //アピール
		[ObservableProperty]
		string? statName;       //駅名
		[ObservableProperty]
		long? longitude;        //経度
		[ObservableProperty]
		long? latitude;     //緯度
		[ObservableProperty]
		int? eCDisp;        //EC表示
		[ObservableProperty]
		string? eCDispName;     //EC表示名
		[ObservableProperty]
		int? area2;      //地域
		[ObservableProperty]
		string? brand;      //ブランド
		[ObservableProperty]
		int? reservation;       //取置
		[ObservableProperty]
		int? order;     //取寄
		[ObservableProperty]
		int? orderSeq;      //取寄順
		[ObservableProperty]
		string? eCBusHours;     //EC営業時間
		[ObservableProperty]
		string? iNSTA;      //INSTA
		[ObservableProperty]
		string? wEAR;       //WEAR
		[ObservableProperty]
		string? bLOG;       //BLOG
		[ObservableProperty]
		string? mAPURL;     //MAPURL
		[ObservableProperty]
		string? eCAppeal;       //ECアピール
		[ObservableProperty]
		string? site;        //サイト
		[ObservableProperty]
		int? adjustedInv;       //調整在庫
		[ObservableProperty]
		string? registNum;      //登録番号
		[ObservableProperty]
		string? aPPBusHours;        //APP営業時間
		[ObservableProperty]
		string? oriStoreImgName;        //元店舗画像名
		[ObservableProperty]
		int? dispOrder;		//表示順

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
        string? supplierName; //仕入先名
        [ObservableProperty]
        string? supplierProdCd; //仕入先商品CD
        [ObservableProperty]
        decimal unitPrice; //単価
        [ObservableProperty]
        string? memo; //メモ
        [ObservableProperty]
        string? inpStaffCD; //入力社員CD 
    }

	public partial class MasterWeek : ObservableObject
	{
        [ObservableProperty] 
        long seqNo;
        [ObservableProperty]
        decimal vdateCreate;
        [ObservableProperty]
        decimal vdateUpdate;
		[ObservableProperty]
		int year; //年
		[ObservableProperty]
		int weekNo; //週NO
		[ObservableProperty]
		DateTime startDate; //開始日
		[ObservableProperty]
		DateTime endDate; //終了日
		[ObservableProperty]
		string? memo; //メモ
    }

	public partial class MasterSupplier : ObservableObject
	{
        [ObservableProperty]
        long seqNo;
        [ObservableProperty]
        decimal vdateCreate;
        [ObservableProperty]
        decimal vdateUpdate;
		[ObservableProperty]
		string? supplierCD; //仕入先CD
		[ObservableProperty]
		string? oldCD; //旧コード
        [ObservableProperty]
        string? abbreviation; //略称
        [ObservableProperty]
        string? supplierName; //仕入先名
		[ObservableProperty]
		string? kana; //カナ
        [ObservableProperty]
        string? postalCode; // 郵便番号
        [ObservableProperty]
        string? address1; // 住所1
        [ObservableProperty]
        string? address2; // 住所2
        [ObservableProperty]
        string? address3; // 住所3
        [ObservableProperty]
        string? tel; // TEL
        [ObservableProperty]
        string? fax; // FAX
        [ObservableProperty]
        decimal? rate1; // 掛率
        [ObservableProperty]
        decimal? rate2; // 掛率2
        [ObservableProperty]
        string? paymentDestinationCd; // 支払先CD
        [ObservableProperty]
        int? paymentPrintFlag; // 支払印刷
        [ObservableProperty]
        int? closingDay; // 締日
        [ObservableProperty]
        int? scheduledPaymentMonth; // 支払予定月
        [ObservableProperty]
        int? scheduledPaymentDay; // 支払予定日
        [ObservableProperty]
        string? paymentMethod; // 支払方法
        [ObservableProperty]
        decimal? taxCd; // 消費税CD
        [ObservableProperty]
        int? taxCalculationMethod; // 消費税計算方法
        [ObservableProperty]
        int? taxRounding; // 消費税端数
        [ObservableProperty]
        decimal? paymentRate; // 支払率
        [ObservableProperty]
        string? recipientFlag1; // 宛名FLG1
        [ObservableProperty]
        string? recipientFlag2; // 宛名FLG2
        [ObservableProperty]
        string? recipientFlag3; // 宛名FLG3
        [ObservableProperty]
        string? recipientName1; // 宛名名称1
        [ObservableProperty]
        string? recipientName2; // 宛名名称2
        [ObservableProperty]
        int? slipIssueType; // 伝票発行区分
        [ObservableProperty]
        string? slipPrint1; // 伝票印字1
        [ObservableProperty]
        string? slipPrint2; // 伝票印字2
        [ObservableProperty]
        string? slipPrint3; // 伝票印字3
        [ObservableProperty]
        string? slipPrint4; // 伝票印字4
        [ObservableProperty]
        string? department; // 部門
        [ObservableProperty]
        string? remarks; // 備考
        [ObservableProperty]
        string? nameCd01; // 名称CD01
        [ObservableProperty]
        string? nameCd02; // 名称CD02
        [ObservableProperty]
        string? nameCd03; // 名称CD03
        [ObservableProperty]
        string? nameCd04; // 名称CD04
        [ObservableProperty]
        string? nameCd05; // 名称CD05
        [ObservableProperty]
        string? nameCd06; // 名称CD06
        [ObservableProperty]
        string? nameCd07; // 名称CD07
        [ObservableProperty]
        string? nameCd08; // 名称CD08
        [ObservableProperty]
        string? nameCd09; // 名称CD09
        [ObservableProperty]
        string? nameCd10; // 名称CD10
        [ObservableProperty]
        string? bankName; // 振込銀行
        [ObservableProperty]
        string? branchName; // 振込支店
        [ObservableProperty]
        string? transferType; // 振込種別
        [ObservableProperty]
        string? accountNumber; // 振込口座
        [ObservableProperty]
        string? remarks2; // 備考2
        [ObservableProperty]
        int? orderFlag; // 発注FLG
        [ObservableProperty]
        int? productionFlag; // 生産FLG
        [ObservableProperty]
        string? currencyType; // 為替区分
        [ObservableProperty]
        int? currencyRoundingDigit; // 為替桁切指定
        [ObservableProperty]
        int? currencyFractionType; // 為替端数区分
        [ObservableProperty]
        int? posType; // POS区分
        [ObservableProperty]
        string? inputEmployeeCd; // 入力社員CD
        [ObservableProperty]
        int? orderStopFlag; // 発注停止FLG
        [ObservableProperty]
        int? purchaseType; // 仕入区分
        [ObservableProperty]
        string? corporationCd; // 法人CD
        [ObservableProperty]
        string? linkCd; // 連携CD
        [ObservableProperty]
        string? linkedCorporationCd; // 連携先法人CD
        [ObservableProperty]
        string? supplierMail; // 仕入先MAIL
        [ObservableProperty]
        int? dueDate; // 期日
        [ObservableProperty]
        decimal? minimumAmount; // 下限額
        [ObservableProperty]
        string? warehousePostalCode; // 倉庫郵便番号
        [ObservableProperty]
        string? warehouseAddress1; // 倉庫住所1
        [ObservableProperty]
        string? warehouseAddress2; // 倉庫住所2
        [ObservableProperty]
        string? warehouseAddress3; // 倉庫住所3
        [ObservableProperty]
        string? warehouseTel; // 倉庫TEL
        [ObservableProperty]
        string? warehouseFax; // 倉庫FAX
        [ObservableProperty]
        int? closingDay2; // 締日2
        [ObservableProperty]
        int? scheduledPaymentMonth2; // 支払予定月2
        [ObservableProperty]
        int? scheduledPaymentDay2; // 支払予定日2
        [ObservableProperty]
        int? closingDay3; // 締日3
        [ObservableProperty]
        int? scheduledPaymentMonth3; // 支払予定月3
        [ObservableProperty]
        int? scheduledPaymentDay3; // 支払予定日3
        [ObservableProperty]
        string? registrationNumber; // 登録番号
        [ObservableProperty]
        string? lastModifier; // 最終修正者
    }

	public partial class MasterShohinJan : ObservableObject
	{
        [ObservableProperty]
        long seqNo;
        [ObservableProperty]
        decimal vdateCreate;
        [ObservableProperty]
        decimal vdateUpdate;
		[ObservableProperty]
		string? productCD;					// 商品CD
		[ObservableProperty]
		string? colorCD;					// 色CD
		[ObservableProperty]
		string? sizeCD;						// ｻｲｽﾞCD
		[ObservableProperty]
		string? janCode1;					// JANｺｰﾄﾞ1
		[ObservableProperty]
		string? janCode2;					// JANｺｰﾄﾞ2
		[ObservableProperty]
		string? janCode3;					// JANｺｰﾄﾞ3
		[ObservableProperty]
		string? memo;						// ﾒﾓ
		[ObservableProperty]
		int? useFlag;						// 使用FLG
		[ObservableProperty]
		decimal? scheduledProductionQuantity;   // 生産予定数
		[ObservableProperty]
		decimal? cuttingQuantity;           // 裁断数
		[ObservableProperty]
		decimal? tagNumber;                 // 下札枚数
		[ObservableProperty]
		decimal? retailPrice;               // 上代
		[ObservableProperty]
		int? autoAllocationFlag;            // 自動配分FLG
		[ObservableProperty]
		int? posType;                       // POS区分
		[ObservableProperty]
		string? inputWorkerCD;              // 入力社員CD
		[ObservableProperty]
		string? ecJan;                      // ECJAN
		[ObservableProperty]
		int? ecDisplayFlag;                 // EC表示FLG
		[ObservableProperty]
		int? collabOutputType;              // ｺﾗﾎﾞ出力区分
		[ObservableProperty]
		int? productSpecID;                 // 商品規格ID
		[ObservableProperty]
		string? colorName;                  // 色名
		[ObservableProperty]
		string? sizeName;                   // ｻｲｽﾞ名
		[ObservableProperty]
		int? salesType;                     // ｾｰﾙ区分
		[ObservableProperty]
		string? productName;                // 商品名
		[ObservableProperty]
		string? remarksCD01;                // 備考CD01
		[ObservableProperty]
		string? remarks01;                  // 備考01
		[ObservableProperty]
		string? remarksCD02;                // 備考CD02
		[ObservableProperty]
		string? remarks02;                  // 備考02
        [ObservableProperty]
        string? remarksCD03;                // 備考CD02
        [ObservableProperty]
        string? remarks03;                  // 備考02
		[ObservableProperty]
		decimal? costPrice;                 // 原価
		[ObservableProperty]
		string? partnerProductNO;           // 相手商品NO
		[ObservableProperty]
		int? detailPurchaseType;            // 明細仕入区分
		[ObservableProperty]
		int? supplierPrice;                 // 仕入価格
		[ObservableProperty]
		int? originalRetailPrice;           // 元上代
		[ObservableProperty]
		string? supplierCD;                 // 仕入先CD
		[ObservableProperty]
		string? purchaseDate;               // 仕入日
		[ObservableProperty]
		string? finalInvWarehouseCD;        // 最終在庫倉庫CD
		[ObservableProperty]
		string? finalDeliveryDate;          // 最終出庫日
		[ObservableProperty]
		int? tagType;                       // タグ種
		[ObservableProperty]
		int? endSeasonDiscountType;         // 期末値引区分
		[ObservableProperty]
		decimal? endSeasonDiscountRate;     // 期末値引率
		[ObservableProperty]
		string? endTermDate;                // 期末日
		[ObservableProperty]
		string? endTermProcessDate;         // 期末処理日
		[ObservableProperty]
		int? revaluationType;               // 評価替区分
		[ObservableProperty]
		decimal? revaluationRate;           // 評価替率
		[ObservableProperty]
		string? revaluationProcessDate;     // 評価替処理日
		[ObservableProperty]
		decimal? latestTaxIncludedPrice;    // 最新税込上代
		[ObservableProperty]
		decimal? latestCostPrice;           // 最新原価
		[ObservableProperty]
		decimal? latestRetailPrice;         // 最新上代
		[ObservableProperty]
		int? responseType;                  // 対応区分
		[ObservableProperty]
		decimal? salesPrice;                // セール金額
		[ObservableProperty]
		string? skuCategoryCD01;            // SKU分類CD01
        [ObservableProperty]
        string? skuCategoryCD02;            // SKU分類CD02
        [ObservableProperty]
        string? skuCategoryCD03;            // SKU分類CD03
        [ObservableProperty]
        string? skuCategoryCD04;            // SKU分類CD04
        [ObservableProperty]
        string? skuCategoryCD05;            // SKU分類CD05
        [ObservableProperty]
        string? skuCategoryCD06;            // SKU分類CD06
        [ObservableProperty]
        string? skuCategoryCD07;            // SKU分類CD07
        [ObservableProperty]
        string? skuCategoryCD08;            // SKU分類CD08
        [ObservableProperty]
        string? skuCategoryCD09;            // SKU分類CD09
        [ObservableProperty]
        string? skuCategoryCD10;            // SKU分類CD10
        [ObservableProperty]
        int? ecReservation;                 // EC予約
		[ObservableProperty]
		int? foreignCurrencyPrice;			// 外貨仕入価格

    }

}
