using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Diagnostics;

namespace TextTool
{

    internal static class Program
    {
        /// <summary>
        /// 格式化剪切板中的字符串，用指定字符（默认逗号）拼接
        /// </summary>
        /// <param name="args">参数支持：
        /// style：格式化样式，0 逗号拼接，1 逗号拼接+单引号
        /// count：每行数量
        /// sepa：用什么字符拼接，如,
        /// 例子：sylte=1 count=50 sepa=,</param>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(defaultValue: false);

            // 参数
            bool isAddSingleQuote = false;
            // 每行的项目数量
            int lineCount = 20;
            // 拼接字段
            string separator = ",";
            // 每项的前缀
            string prefix = "'";
            // 每项的后缀
            string suffix = "'";
            // 每一项的正则
            string itemReg = @"[a-zA-Z0-9\.+_-]{1,}";

            #region 参数处理

            if (args != null)
            {
                foreach (var item in args)
                {
                    var arr = item.Split(new char[] { '=' }, 2);
                    var key = arr[0].ToLower();
                    var value = "";
                    if (arr.Length > 1)
                        value = arr.Last();

                    switch (key)
                    {
                        case "style":
                            // 加单引号
                            isAddSingleQuote = value == "1";
                            break;
                        case "count":
                            if (!int.TryParse(value, out lineCount))
                                lineCount = 20;
                            break;
                        case "sepa":
                            separator = value;
                            break;
                        case "?":
                        case "help":
                            Printhelp(value);
                            return;
                        case "pre":
                            prefix = value;
                            break;
                        case "suf":
                            suffix = value;
                            break;
                        case "item_reg":
                            itemReg = value;
                            break;
                    }
                }
            }

            #endregion

            ClipboardTextHandle(isAddSingleQuote, lineCount, separator, prefix, suffix, itemReg);
        }

        private static void Printhelp(string param)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion;
            //Application.SetCompatibleTextRenderingDefault(defaultValue: true);
            MessageBox.Show(@"格式化剪切板中的字符串，用逗号分隔
参数支持：
style：格式化样式，0 逗号拼接，1 逗号拼接+单引号
pre：prefix 前缀，每一项的前缀，默认是单引号
suf：suffix 后缀，每一项的后缀，默认是单引号
count：每行数量
sepa：separator 用什么字符拼接，如,
item_reg：每项的正则表达式，注意值最好是加双引号，默认是item_reg=""[a-zA-Z0-9\.+_-]{1,}""
?：显示帮助
help：显示帮助
【例子】：TextTool sylte=1 count=50 sepa=,", $"{Application.ProductName} v{version} 使用说明");
        }
        private static void ClipboardTextHandle(bool isAddSingleQuote, int lineCount, string separator,
            string prefix,
            string suffix,
            string itemReg)
        {
            try
            {
                IDataObject dataObject = Clipboard.GetDataObject();
                string input = (string)dataObject.GetData(DataFormats.Text);
                MatchCollection matchCollection = Regex.Matches(input, itemReg);
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
                        result.Append(Environment.NewLine + separator);

                    if (isAddSingleQuote)
                        result.Append(string.Join(separator, lineList.Select(p => $"{prefix}{p}{suffix}")));
                    else
                        result.Append(string.Join(separator, lineList));
                }

                Clipboard.SetDataObject(result.ToString(), copy: true);
            }
            catch (Exception)
            {
            }
        }

    }
}
