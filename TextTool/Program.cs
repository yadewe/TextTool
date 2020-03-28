using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace TextFormatTool
{

	internal static class Program
	{
		/// <summary>
		/// 格式化剪切板中的字符串，用逗号分隔
		/// </summary>
		/// <param name="args">参数支持：
		/// style：格式化样式，0 逗号拼接，1 逗号拼接+单引号
		/// count：每行数量
		/// 例子：sylte=1 count=50</param>
		[STAThread]
		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(defaultValue: false);
			bool isAddSingleQuote = false;
			int lineCount = 20;

            #region 参数处理

            if (args != null)
			{
				foreach (var item in args)
				{
					var arr = item.Split('=');
					var key = arr[0];
					var value = arr.Last();
					switch(key)
						{
						case "style":
							// 加单引号
							isAddSingleQuote = value == "1";
							break;
						case "count":
							if(!int.TryParse(value, out lineCount))
								lineCount = 20;
							break;
					}
				}
			}

            #endregion

            CopyText(isAddSingleQuote, lineCount);
		}

		public static void CopyText(bool isAddSingleQuote, int lineCount)
		{
			try
			{
				IDataObject dataObject = Clipboard.GetDataObject();
				string input = (string)dataObject.GetData(DataFormats.Text);
				MatchCollection matchCollection = Regex.Matches(input, "[a-zA-Z0-9\\._-]{1,}");
				List<string> list = new List<string>();
				foreach (Match item in matchCollection)
				{
					list.Add(item.Value);
				}
				list = list.Distinct().ToList();

				// TODO count 20 every line

				StringBuilder result = new StringBuilder();
				IEnumerable<string> items = list;
				while (items.Count() > 0)
				{
					var lineList = items.Take(lineCount);
					items = items.Skip(lineList.Count());
					if (result.Length != 0)
						result.Append(Environment.NewLine + ",");

					if (isAddSingleQuote)
						result.Append(string.Join(",", lineList.Select(p => $"'{p}'")));
					else
						result.Append(string.Join(",", lineList));
				}
			
				Clipboard.SetDataObject(result.ToString(), copy: true);
			}
			catch (Exception)
			{
			}
		}

	}
}
