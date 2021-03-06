﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextTool
{
    public class TextHandler
    {
        public string HandledOutput;
        public int HandledItemCount;

        public TextHandlerOption Option { get; set; }
        public string HandledTip { get; internal set; }

        public TextHandler(TextHandlerOption option)
        {
            Option = option;
        }

        /// <summary>
        /// 处理文本
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string Handle(string input)
        {
            if (Option.Type == TextTypeEnum.Split)
                return TextSplitHandle(input, Option.Prefix, Option.Suffix, Option.ItemReg, Option.IsKeepRepeat);
            else
                return TextJoinHandle(input, Option.Type == TextTypeEnum.PreSufJoin, Option.LineCount, Option.Separator, Option.Prefix, Option.Suffix, Option.ItemReg, Option.IsKeepRepeat);
        }

        public void Printhelp(string param)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion;
            //Application.SetCompatibleTextRenderingDefault(defaultValue: true);
            Dictionary<string, string> dic = new Dictionary<string, string>() {
                {"type/style", "格式化类型，0 用分隔符拼接，1 加前、后缀再拼接，2 拆分成多行" },
                {"pre", "prefix 前缀，每一项的前缀，默认是单引号" },
                {"suf", "suffix 后缀，每一项的后缀，默认是单引号" },
                {"count", "每行数量，默认20，小于等于0时不换行" },
                {"sepa", "separator 分隔符，用什么字符拼接，如," },
                {"item_reg", @"每项的正则表达式，注意值最好是加双引号，默认是item_reg=""[a-zA-Z0-9\.+_-]{1,}""" },
                {"tip", "显示通知的时间，默认0，不显示" },
                {"rep", "repeat 是否要去重，默认0 去重，1 保持重复的项，不去重" },
                {"?", "显示帮助" },
                {"help", "显示帮助" },
            };
            string message = "";
            if (dic.ContainsKey(param))
            {
                message = $"{param}：{dic[param]}";
            }
            else
            {
                message = $@"格式化剪切板中的字符串，用逗号分隔。
程序运行之后会读取剪切板的文本内容，格式完之后写回剪切板。
参数支持：
{string.Join(Environment.NewLine, dic.Select(p => $"{p.Key}：{p.Value}"))}
【例子】
TextTool type=1 count=50 sepa=, pre=' suf=' count=20 item_reg=""[a-zA-Z0-9\.+_-]{1,}"" tip=0 rep=0";
            }

            MessageBox.Show(message, $"{Application.ProductName} v{version} 使用说明");
            Console.WriteLine(message);
        }

        private string TextJoinHandle(
            string input,
            bool isAddSingleQuote,
            int lineCount,
            string separator,
            string prefix,
            string suffix,
            string itemReg,
            bool isKeepRepeat)
        {
            List<string> list = new List<string>();
            MatchCollection matchCollection = Regex.Matches(input, itemReg);
            foreach (Match item in matchCollection)
            {
                list.Add(item.Value);
            }
            var repeatCount = list.Count;
            if (!isKeepRepeat)
                list = list.Distinct().ToList();

            // 大于0时才换行
            if (lineCount <= 0)
                lineCount = list.Count;

            StringBuilder result = new StringBuilder();
            int index = 0;
            while (index <= list.Count - 1)
            {
                var remainCount = list.Count - index;
                var lineList = list.GetRange(index, remainCount > lineCount ? lineCount : remainCount);
                if (result.Length != 0)
                    result.Append(separator + Environment.NewLine);

                if (isAddSingleQuote)
                    result.Append(string.Join(separator, lineList.Select(p => $"{prefix}{p}{suffix}")));
                else
                    result.Append(string.Join(separator, lineList));

                index += lineCount;
            }

            HandledItemCount = list.Count;
            HandledOutput = result.ToString();
            HandledTip = $"已经 拼接 {HandledItemCount} 个{(repeatCount == HandledItemCount ? "" : $"，去重前{repeatCount}个")}";
            return HandledOutput;
        }

        /// <summary>
        /// 拆解成多行
        /// </summary>
        /// <param name="input"></param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        /// <param name="itemReg"></param>
        /// <returns></returns>
        private string TextSplitHandle(
            string input,
            string prefix,
            string suffix,
            string itemReg,
            bool isKeepRepeat)
        {
            List<string> list = new List<string>();
            prefix = Regex.Replace(prefix, @"[-[\]{}()*+?.,\\^$|#]", @"\$&");
            suffix = Regex.Replace(suffix, @"[-[\]{}()*+?.,\\^$|#]", @"\$&");
            var newItemReg = $"({prefix})(?<item>{itemReg})({suffix})|(?<item2>{itemReg})";
            MatchCollection matchCollection = Regex.Matches(input, newItemReg);
            foreach (Match item in matchCollection)
            {
                if (item.Groups["item"].Success)
                    list.Add(item.Groups["item"].Value);
                else
                    list.Add(item.Groups["item2"].Value);
            }
            var repeatCount = list.Count;
            if (!isKeepRepeat)
                list = list.Distinct().ToList();

            HandledItemCount = list.Count;
            HandledOutput = string.Join(Environment.NewLine, list);
            HandledTip = $"已经 拆解 {HandledItemCount} 个{(repeatCount == HandledItemCount ? "" : $"，去重前{repeatCount}个")}";
            return HandledOutput;
        }


    }

    public class TextHandlerOption
    {
        public TextHandlerOption()
        {
            Type = TextTypeEnum.Join;
            LineCount = 20;
            Separator = ",";
            Prefix = "'";
            Suffix = "'";
            ItemReg = @"[a-zA-Z0-9\.+_-]{1,}";
        }
        /// <summary>
        /// 处理样式
        /// </summary>
        public TextTypeEnum Type { get; set; }
        /// <summary>
        /// 每行的项目数量
        /// </summary>
        public int LineCount { get; set; }
        /// <summary>
        /// 分隔符，拼接字符
        /// </summary>
        public string Separator { get; set; }
        /// <summary>
        /// 每项的前缀
        /// </summary>
        public string Prefix { get; set; }
        /// <summary>
        /// 每项的后缀
        /// </summary>
        public string Suffix { get; set; }
        /// <summary>
        /// 每一项的正则
        /// </summary>
        public string ItemReg { get; set; }
        /// <summary>
        /// 显示结果通知的时间
        /// </summary>
        public int ShowTipSeconds { get; set; }
        /// <summary>
        /// 是否要去重
        /// </summary>
        public bool IsKeepRepeat { get; set; }
    }

    /// <summary>
    /// 处理操作的类型
    /// </summary>
    public enum TextTypeEnum
    {
        /// <summary>
        /// 用分隔符拼接
        /// </summary>
        Join = 0,
        /// <summary>
        /// 加前/后缀之后拼接
        /// </summary>
        PreSufJoin = 1,
        /// <summary>
        /// 拆解
        /// </summary>
        Split = 2
    }
}
