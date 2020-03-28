using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TextFormatTool
{

	internal static class Program
	{
		/// <summary>
		/// 格式化剪切板中的字符串，用逗号分隔
		/// </summary>
		/// <param name="args"></param>
		[STAThread]
		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(defaultValue: false);
			CopyText(args?.FirstOrDefault()?.ToLower() == "-style=1");
		}

		public static void CopyText(bool isAddSingleQuote)
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

				int num = 20;
				StringBuilder result = new StringBuilder();
				IEnumerable<string> items = list;
				while (items.Count() > 0)
				{
					var lineList = items.Take(num);
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
