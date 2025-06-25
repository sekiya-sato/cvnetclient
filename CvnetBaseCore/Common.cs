/* ============================================================================
 * CvnetBaseCore.dll : Common.cs
 * Created by Sekiya.Sato 2025/05/22
 * 説明: 共通ロジッククラス CVPOS3互換(.net framework 4)
 * 使用ライブラリ
 * ============================================================================  */
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CvnetBaseCore {
	public class Common {
		#region DataTableとIListの相互変換
		/// <summary>
		/// ListからDataTableへの変換
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		public static DataTable ConvertToDataTable<T>(T list) where T : IList {
			var table = new DataTable(typeof(T).GetGenericArguments()[0].Name);
			typeof(T).GetGenericArguments()[0].GetProperties().
				ToList().ForEach(p => table.Columns.Add(p.Name, p.PropertyType));
			foreach (var item in list) {
				var row = table.NewRow();
				item.GetType().GetProperties().
					ToList().ForEach(p => row[p.Name] = p.GetValue(item, null));
				table.Rows.Add(row);
			}
			return table;
		}
#pragma warning disable CS8602
		/// <summary>
		/// DataTableからListへの変換(列名の大文字小文字は無視、データ型はListに合わせて自動変換)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="table"></param>
		/// <returns></returns>
		public static T ConvertToList<T>(DataTable table) where T : IList, new() {
			var list = new T();
			foreach (DataRow row in table.Rows) {
				var item = Activator.CreateInstance(typeof(T).GetGenericArguments()[0]);
				list.GetType().GetGenericArguments()[0].GetProperties().ToList().
					ForEach(p => {
						if (table.Columns.Contains(p.Name) && !(row[p.Name] is DBNull)) { // 含まれる列かつDBNull以外
							if (table.Columns[p.Name].DataType != p.PropertyType) { // 型が違う
								var tmp_val = row[p.Name].ToString();
								switch (p.PropertyType.ToString()) {
									case "System.Int16":
										if (string.IsNullOrEmpty(tmp_val) || tmp_val == ".")
											tmp_val = "0";
										p.SetValue(item, short.Parse(tmp_val), null);
										break;
									case "System.Int32":
										if (string.IsNullOrEmpty(tmp_val) || tmp_val == ".")
											tmp_val = "0";
										p.SetValue(item, int.Parse(tmp_val), null);
										break;
									case "System.Int64":
										if (string.IsNullOrEmpty(tmp_val) || tmp_val == ".")
											tmp_val = "0";
										p.SetValue(item, long.Parse(tmp_val), null);
										break;
									case "System.Decimal":
										if (string.IsNullOrEmpty(tmp_val) || tmp_val == ".")
											tmp_val = "0";
										p.SetValue(item, decimal.Parse(tmp_val), null);
										break;
									case "System.Double":
										if (string.IsNullOrEmpty(tmp_val) || tmp_val == ".")
											tmp_val = "0";
										p.SetValue(item, double.Parse(tmp_val), null);
										break;
									case "System.String":
										p.SetValue(item, tmp_val, null);
										break;
									default:
										p.SetValue(item, row[p.Name], null);
										break;
								}
							}
							else {
								if (p.PropertyType.ToString() == "System.String") {
									var tmp_val = row[p.Name].ToString();
									if (tmp_val == ".") tmp_val = "";
									p.SetValue(item, tmp_val, null);
								}
								else {
									p.SetValue(item, row[p.Name], null);
								}
							}
						}
					});
				list.Add(item);
			}
			return list;
		}
#pragma warning restore CS8602
		#endregion
		/// <summary>
		/// リストのプロパティの文字列型の値が"."の場合、""に変換する
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="inTable"></param>
		public static void ConvertDotStringDel<T>(List<T> inTable) where T:new()  {
			List<PropertyInfo> pinfo= typeof(T).GetProperties().ToList();
			foreach (var item in inTable) {
				if (item == null)
					continue;
				pinfo.ForEach(p => {
					if (p.PropertyType.ToString() == "System.String") {
						var tmp_val = p.GetValue(item, null).ToString();
						if(string.IsNullOrEmpty(tmp_val)) return;
						tmp_val = tmp_val.Trim();
						if (tmp_val == ".") tmp_val = "";
						p.SetValue(item, tmp_val, null);
					}
				});
			}
		}
#pragma warning disable CS8602
		/// <summary>
		/// 文字列プロパティの値が空白の場合、"."に変換する
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="item"></param>
		public static void ConvertDotStringAdd<T>(T item) where T : new() {
			if (item == null) return;
			List<PropertyInfo> pinfo = typeof(T).GetProperties().ToList();
			StringBuilder sb = new StringBuilder();
			pinfo.ForEach(p => {
				if (p.PropertyType.ToString() == "System.String") {
					var tmp_val = p.GetValue(item, null).ToString();
					if (string.IsNullOrEmpty(tmp_val)) return;
					tmp_val = tmp_val.Trim();
					if (string.IsNullOrEmpty(tmp_val)) tmp_val = ".";
					p.SetValue(item, tmp_val, null);
				}
			});
		}
		/// <summary>
		/// 文字列プロパティの値が"."の場合、空白に変換する
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="item"></param>
		public static void ConvertDotStringDel<T>(T item) where T : new() {
			if (item == null) return;
			List<PropertyInfo> pinfo = typeof(T).GetProperties().ToList();
			StringBuilder sb = new StringBuilder();
			pinfo.ForEach(p => {
				if (p.PropertyType.ToString() == "System.String") {
					var tmp_val2 = p.GetValue(item, null);
					if(tmp_val2 != null) {
						var tmp_val = tmp_val2.ToString().Trim();
						if (tmp_val == ".") {
							tmp_val = "";
							p.SetValue(item, tmp_val, null);
						}
					}
				}
			});
		}
#pragma warning restore CS8602
		/// <summary>
		/// オブジェクトのコピーを作成する
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="src"></param>
		/// <returns></returns>
		public static T CloneObject<T>(T src) where T : new() {
			if (src == null) return new T();
			var jsonstr = JsonConvert.SerializeObject(src);
			var dst = JsonConvert.DeserializeObject<T>(jsonstr);
			return dst ?? new T();
		}
		/// <summary>
		/// 最大取得件数(default1000)を制限したSQL文を返す
		/// </summary>
		/// <param name="sqlstr"></param>
		/// <param name="maxcnt"></param>
		/// <returns></returns>
		public static string GetSqlStringUsingMax(string sqlstr, int maxcnt) {
			return ("select * from (" + sqlstr + ") where rownum<=" + maxcnt.ToString());
		}


	}
}
