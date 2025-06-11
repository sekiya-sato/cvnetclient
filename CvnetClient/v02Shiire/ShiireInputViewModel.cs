using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.ObjectModel;
using System.Data;

namespace CvnetClient.ViewModels {
	public partial class ShiireInputViewModel : ObservableObject {
		[ObservableProperty]
		int selectedTabIndex;
		// ��������
		[ObservableProperty] long denpyoNoFrom;
		[ObservableProperty] long denpyoNoTo=99999999;
		[ObservableProperty] DateTime? shiireDateFrom= DateTime.Now.AddDays(DateTime.Now.Day - 1).AddDays(-300);
		[ObservableProperty] DateTime? shiireDateTo = DateTime.Now;
		[ObservableProperty] ObservableCollection<string>? torihikiKubunList = new() { "10 �d��", "20 �ԕi" };
		[ObservableProperty] string? selectedTorihikiKubun;
		[ObservableProperty] string? manualInputNo;
		[ObservableProperty] string? shiireSakiCode;
		[ObservableProperty] string? shiireSakiName;
		[ObservableProperty] bool isShuukei;
		[ObservableProperty] bool isMishouninOnly = true;
		[ObservableProperty] bool isAll;
		[ObservableProperty] string pageInfo = "1 / 1";

		// DataGrid
		[ObservableProperty] ObservableCollection<ShiireRow> shiireList = new();
		[ObservableProperty] ShiireRow? selectedShiire;

		// ����
		[ObservableProperty] bool isListMode = true;
		[ObservableProperty] bool isDetailMode;
		[ObservableProperty] string statusMessage = "���X�g�I���s�f�[�^�擾";

		string sqlstr = $"""
			select * from (
			select A.SEQ_NO,A.VDATE_CREATE,A.VDATE_UPDATE,A.����͓`�[NO,A.�݌Ɍv���,A.�|�v���,A.����敪,A.���͎Ј�CD,A.�q��CD,
			A.�����CD1,A.�|��1,A.�O�őΏۋ��z,A.���ʍ��v,A.���׋��z���v,A.���ŏ����,A.�O�ŏ����,A.��㍇�v,A.���㍇�v,A.����,
			A.�|�v��FLG,A.�`�[�����敪,A.MOD_SEQ,A.�֘A�`�[NO,A.�֘A�`�[NO2,A.����FLG,A.�݌Ɍv��FLG,A.����ŗ�,B.���O �S����,
			C.�d���於 �d���於,D.���Ӑ於 �q�ɖ�,C.�|��,C.�|��2,C.�����CD,C.����Ōv�Z���@,C.����Œ[��,
			decode(A.�|�v��FLG,1,'��','') ���FFLG,NVL((select max(K.�x����) �x���� from HC$MANAGE_KAKESIH K where 
			A.�����CD1=K.�d����CD),'19010101') �ŏI���� from HC$Tran_TORI0 A,HC$MASTER_SHAIN B,HC$Master_SIIRE C,HC$MASTER_TOKUI D 
			where (A.���͎Ј�CD=B.�Ј�CD(+)) and (A.�����CD1=C.�d����CD(+)) and (A.�q��CD=D.���Ӑ�CD(+)) and 
			A.SEQ_NO between :1 and :2 and A.�݌Ɍv��� between :3 and :4 and A.����敪 between :5 and :6 and 
			A.�֘A�`�[NO between :7 and :8 and A.�֘A�`�[NO2 between :9 and :10 and A.�����CD1 between :11 and :12 and 
			A.�q��CD between :13 and :14 and A.����͓`�[NO between :15 and :16 and A.���͎Ј�CD between :17 and :18 and 
			A.�`�[�����敪=3 ORDER BY A.SEQ_NO DESC) where rownum<={AppData.maxQueryCnt}
			""";
		/*
		string sqlstr = $"""
			select A.*,B.���O �S����
			from HC$Tran_TORI0 A,HC$MASTER_SHAIN B
			where 
			(A.���͎Ј�CD=B.�Ј�CD(+)) and
			A.SEQ_NO between :1 and :2 and A.�݌Ɍv��� between :3 and :4 and 
			A.�`�[�����敪=3 ORDER BY A.SEQ_NO DESC
			""";
		*/


		/// �ꗗ�\��
		/// </summary>
		[RelayCommand]
		void DoList() {
			subList();
			if (ShiireList == null || ShiireList.Count == 0)
				ClientLib.MessageBoxOk(this, "�f�[�^������܂���");
		}
		void subList() {
			var para = new string[] {
				DenpyoNoFrom.ToString(), DenpyoNoTo.ToString(),
				ShiireDateFrom?.ToString("yyyyMMdd") ?? "19010101", ShiireDateTo?.ToString("yyyyMMdd") ?? "99991231",
				"10","99", // ����敪 10=�d��,20=�ԕi
				"0", "9999999999", // �֘A�`�[NO1
				"0", "9999999999", // �֘A�`�[NO2
				ShiireSakiCode ?? "0", ShiireSakiCode ??"9999999999", // �d����CD1
				".","9999999999", // �q��CD
				ManualInputNo ?? ".", ManualInputNo ?? "9999999999", // ����͓`�[NO
				".", ".9999999999", // ���͎Ј�CD
				};
			var retData = AppData.Http?.AspxSqlQuery(sqlstr, para);

			if (retData == null || retData.Rows.Count == 0) return;
			var list = (from DataRow dr in retData.Rows
						select new ShiireRow {
							DenpyoNo = dr["SEQ_NO"].ToString(),
							ShiireDate = dr["�݌Ɍv���"].ToString(),
							ShiireSaki = dr["�����CD1"].ToString(),
							ShiireSakiName = dr["�d���於"].ToString(),
							SokoName = dr["�q�ɖ�"].ToString(),
							TorihikiKubun = dr["����敪"].ToString(),
							Suuryou = dr["���ʍ��v"].ToString(),
							Kingaku = dr["���׋��z���v"].ToString(),
							KanrenNo1 = dr["�֘A�`�[NO"].ToString(),
						}).ToList();
			Common.ConvertDotStringDel(list);
			ShiireList = new ObservableCollection<ShiireRow>(list);
			if (ShiireList.Count > 0) {
				SelectedShiire = ShiireList[0];
			}
		}


		// �R�}���h
		[RelayCommand]
		void OpenSearch() {
			/* ���� */
		}
		[RelayCommand] void OpenEdit() { /* ���� */ }
		[RelayCommand] void SelectShiireSaki() { /* ���� */ }
		[RelayCommand] void Search() { /* ���� */ }
		[RelayCommand] void UpdateSelectedFlag() { /* ���� */ }
		[RelayCommand] void Print() { /* ���� */ }
		[RelayCommand] void ExportCsv() { /* ���� */ }
		[RelayCommand] void Edit() { /* ���� */ }
		[RelayCommand] void Delete() { /* ���� */ }
		[RelayCommand]
		void Close() {
			ClientLib.Exit(this);
		}
		[RelayCommand]
		void RowDoubleClick() {
			// �ڍ׃��[�h�ɐ؂�ւ�
			IsListMode = false;
			IsDetailMode = true;
			SelectedTabIndex = 1; // �ڍ׃^�u�ɐ؂�ւ�
			StatusMessage = "�I���s�f�[�^�擾";
			// �I���s�̃f�[�^���擾
			// �����ŏڍ׉�ʂɃf�[�^���Z�b�g���鏈����ǉ�����
			// ��: LoadShiireDetails(current);
		}

	}

	// DataGrid�p�̍s�f�[�^
	public partial class ShiireRow : ObservableObject {
		[ObservableProperty] bool isUpdated;
		[ObservableProperty] string? denpyoNo;
		[ObservableProperty] string? shiireDate;
		[ObservableProperty] string? shiireSaki;
		[ObservableProperty] string? shiireSakiName;
		[ObservableProperty] string? sokoName;
		[ObservableProperty] string? torihikiKubun;
		[ObservableProperty] string? suuryou;
		[ObservableProperty] string? kingaku;
		[ObservableProperty] string? kanrenNo1;
	}
}