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

            try
            {
                var option = new TextHandlerOption();
                var handler = new TextHandler(option);

                #region 参数处理

                if (args != null)
                {
                    string lastKey = "";
                    foreach (var item in args)
                    {
                        var arr = item.Split(new char[] { '=' }, 2);
                        var key = arr[0].Trim().ToLower();
                        var value = "";
                        if (arr.Length > 1)
                            value = arr.Last();

                        switch (key)
                        {
                            case "type":
                            case "style":
                                // 加单引号
                                TextTypeEnum type;
                                if (Enum.TryParse(value.Trim(), out type))
                                    option.Type = type;
                                break;
                            case "count":
                                int lineCount;
                                if (int.TryParse(value.Trim(), out lineCount))
                                    option.LineCount = lineCount;
                                break;
                            case "sepa":
                                option.Separator = value;
                                break;
                            case "?":
                            case "/?":
                            case "help":
                            case "/help":
                                handler.Printhelp(lastKey);
                                return;
                            case "pre":
                                option.Prefix = value;
                                break;
                            case "suf":
                                option.Suffix = value;
                                break;
                            case "item_reg":
                                option.ItemReg = value;
                                break;
                            case "tip":
                                int showTipSeconds;
                                if (int.TryParse(value, out showTipSeconds))
                                    option.ShowTipSeconds = showTipSeconds;
                                break;
                            case "rep":
                                option.IsKeepRepeat = value == "1" || value?.ToLower() == "true";
                                break;
                        }

                        lastKey = key;
                    }
                }

                #endregion

                #region 剪切板的文本处理

                Stopwatch sw = new Stopwatch();
                sw.Start();

                IDataObject dataObject = Clipboard.GetDataObject();
                string input = (string)dataObject.GetData(DataFormats.Text);
                if (string.IsNullOrWhiteSpace(input))
                {
                    string title = "没有文本需要处理";
                    NotificationTool.ShowWindowsTip("空", title, 5, ToolTipIcon.Warning);
                    return;
                }

                string output = handler.Handle(input);

                Clipboard.SetDataObject(output, copy: true);

                sw.Stop();
                // 显示Tip
                if (option.ShowTipSeconds > 0)
                {
                    string title = "文本处理完成";
                    string message = handler.HandledTip;
                    if (sw.ElapsedMilliseconds > 1)
                    {
                        // 精确到小数点后x位
                        var length = (sw.ElapsedMilliseconds % 1000).ToString().Length;
                        int? num = Math.Max(4 - length, 1);
                        message += $" 花费{sw.Elapsed.TotalSeconds.ToString($"N{num}")}s";
                    }
                    NotificationTool.ShowWindowsTip(message, title, option.ShowTipSeconds);
                }

                #endregion
            }
            catch (Exception ex)
            {
                try
                {
                    NotificationTool.ShowWindowsTip(ex.Message, "处理出现异常", 5, ToolTipIcon.Error);
                }
                catch (Exception)
                {
                }
            }
        }

    }
}
