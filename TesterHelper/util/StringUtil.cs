using System;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.International.Converters.PinYinConverter;
using TesterHelper.exception;
using TesterHelper.util.logger;

namespace TesterHelper.util
{
    public class StringUtil
    {
        private const string chinese = "[\\u4e00-\\u9fa5]+";
        private const string english = "[a-zA-z]+";
        public static bool isEmpties(params string[] str) {
            bool flag = false;
            foreach (string  s in str) {
                if (string.IsNullOrEmpty(s)) {
                    flag = true;
                    break;
                }
            }

            return flag;
        }

        public static string createCode() {
            Random random = new Random(10);
            StringBuilder si = new StringBuilder();
            for (int i = 0; i < 5; i++) {
                si.Append(random.Next().ToString());
            }
            return si.ToString();
        }

        /// <summary>
        /// 将占位符替换字段
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string logFormatConvert(string pattern) {
            var si = new StringBuilder(pattern.Length + 20);
            int count = 0;
            foreach (char c in pattern) {
                si.Append(c);
                if (c == '{') {
                    si.Append(count++);
                }
            }
            return si.ToString();
        }

        public static bool isChinese(string str) {
            var reg = new Regex(chinese);
            return reg.Match(str).Success;
        }

        /// <summary>
        /// 中文转拼音
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string convertToAllSpell(string str) {
            if (!isChinese(str)) {
                return str;
            }
            StringBuilder si = new StringBuilder();
            try {
                if (chinese.Length != 0) {
                    foreach (var c in str) {
                        si.Append(getSpell(c));
                    }
                }
            }
            catch (Exception e) {
                throw new ServiceException("字符转换拼音失败", e);
            }
            return si.ToString();
        }
        /// <summary>
        /// 拼接拼音首字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <exception cref="ServiceException"></exception>
        public static string convertToFirstSpell(string str) {
            StringBuilder si;
            if (!isChinese(str)) {
                return str;
            }
            si = new StringBuilder();
            try {
                if (chinese.Length != 0) {
                    foreach (var c in str) {
                        si.Append(getSpell(c)[0]);
                    }
                }
            }
            catch (Exception e) {
                throw new ServiceException("字符转换拼音失败", e);
            }
            return si.ToString().ToUpper();
        }
        
        /// <summary>
        /// 检查ip是否合规
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool checkIp(string ip)
        {
            bool result = ip.Contains(".");
            ip.Trim('.');
            if (result)
            {
                string[] ips = ip.Split('.');
                result = ips.Length == 4;
                if (result)
                {

                }
            }
            return result;
        }

        public static void checkStr(string str) {
            if (string.IsNullOrEmpty(str)) {
                throw new ServiceException(RCode.PARAM_NOTFOUND);
            }
        }
        public static void checkStr(params string[] strs) {
            foreach (var s in strs) {
                if (string.IsNullOrEmpty(s)) {
                    throw new ServiceException(RCode.PARAM_NOTFOUND);
                }
            }
        }

        /// <summary>
        /// 将所有字符变大写并去除所有特殊字符
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string convertToLegal(string s)
        {
            s = s.ToUpper();
            // 寻找第一位字母
            StringBuilder si = new StringBuilder();
            foreach (var c in s) {
                if (checkLegalChar(c)) {
                    si.Append(c);
                }
                else {
                    si.Append('_');
                }
            }
            return si.ToString();
        }

        #region 私有方法

        private static bool checkLegalChar(char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9');
        }
        private static string reverseStr(string str)
        {
            if (str == null)
            {
                return null;
            }
            StringBuilder si = new StringBuilder();
            return si.ToString();
        }

        public static string getSpell(char chr) {
            var coverchr = NPinyin.Pinyin.GetPinyin(chr);

            bool isChineses = ChineseChar.IsValidChar(coverchr[0]);
            if (isChineses)
            {
                ChineseChar chineseChar = new ChineseChar(coverchr[0]);
                foreach (string value in chineseChar.Pinyins)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value.Remove(value.Length - 1, 1);
                    }
                }
            }

            return coverchr;
        }
        #endregion
        
    }
}