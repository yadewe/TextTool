using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextTool
{
    public class TextHandler
    {

        public void Printhelp(string param)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion;
            //Application.SetCompatibleTextRenderingDefault(defaultValue: true);
            Dictionary<string, string> dic = new Dictionary<string, string>() {
                {"style", "格式化样式，0 逗号拼接，1 逗号拼接+单引号，2 拆分成多行" },
                {"pre", "prefix 前缀，每一项的前缀，默认是单引号" },
                {"suf", "suffix 后缀，每一项的后缀，默认是单引号" },
                {"count", "每行数量" },
                {"sepa", "separator 用什么字符拼接，如," },
                {"item_reg", @"每项的正则表达式，注意值最好是加双引号，默认是item_reg=""[a-zA-Z0-9\.+_-]{1,}""" },
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
TextTool sylte=1 count=50 sepa=,";
            }

            MessageBox.Show(message, $"{Application.ProductName} v{version} 使用说明");
        }

        public string TextJoinHandle(
            string input,
            bool isAddSingleQuote,
            int lineCount,
            string separator,
            string prefix,
            string suffix,
            string itemReg)
        {
            List<string> list = new List<string>();
            MatchCollection matchCollection = Regex.Matches(input, itemReg);
            foreach (Match item in matchCollection)
            {
                list.Add(item.Value);
            }
            list = list.Distinct().ToList();

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

            return result.ToString();
        }

        /// <summary>
        /// 拆解成多行
        /// </summary>
        /// <param name="input"></param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        /// <param name="itemReg"></param>
        /// <returns></returns>
        public string TextSplitHandle(
            string input,
            string prefix,
            string suffix,
            string itemReg)
        {
            List<string> list = new List<string>();
            prefix = Regex.Replace(prefix, @"[-[\]{}()*+?.,\\^$|#]", @"\$&");
            suffix = Regex.Replace(suffix, @"[-[\]{}()*+?.,\\^$|#]", @"\$&");
            var newItemReg = $"({prefix})?(?<item>{itemReg})({suffix})?";
            MatchCollection matchCollection = Regex.Matches(input, newItemReg);
            foreach (Match item in matchCollection)
            {
                list.Add(item.Groups["item"].Value);
            }
            list = list.Distinct().ToList();

            return string.Join(Environment.NewLine, list);
        }
    }
}
