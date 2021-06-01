using System;
using TextCopy;

namespace dotnet5
{
    class Program
    {
        static void Main(string[] args)
        {

            var text = ClipboardService.GetText();
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
