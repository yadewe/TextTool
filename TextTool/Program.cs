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
				string result;

				// TODO count 20 every line

				if (isAddSingleQuote)
					result = string.Join(",", list.Select(p => $"'{p}'"));
				else
					result = string.Join(",", list);
				Clipboard.SetDataObject(result, copy: true);
			}
			catch (Exception)
			{
			}
		}
	}
}
