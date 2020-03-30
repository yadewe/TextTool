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
            string style = "0";
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
                string lastKey = "";
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
                            style = value;
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
                            new TextHandler().Printhelp(lastKey);
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

                    lastKey = key;
                }
            }

            #endregion

            ClipboardTextHandle(style, lineCount, separator, prefix, suffix, itemReg);
        }

        private static void ClipboardTextHandle(
            string style,
            int lineCount,
            string separator,
            string prefix,
            string suffix,
            string itemReg)
        {
            try
            {
                IDataObject dataObject = Clipboard.GetDataObject();
                string input = (string)dataObject.GetData(DataFormats.Text);

                string output = "";
                if (style == "2")
                    output = new TextHandler().TextSplitHandle(input, prefix, suffix, itemReg);
                else
                    output = new TextHandler().TextJoinHandle(input, style == "1", lineCount, separator, prefix, suffix, itemReg);

                Clipboard.SetDataObject(output, copy: true);
            }
            catch (Exception)
            {
            }
        }

    }
}
