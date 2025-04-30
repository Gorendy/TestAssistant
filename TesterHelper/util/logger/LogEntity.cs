using System;
using System.Text;
using log4net;

namespace TesterHelper.util.logger
{

    /// <summary>
    /// 记录日志类
    /// </summary>
    public class Logger : LogBase
    {
        public delegate string MessageSend();
        public Logger(ILog log) : base(log) {
        }

        public Logger(string name) : base(name) {
        }

        public Logger(Type type) : base(type) {
        }

       

        public override void debug(RCode code, string message = null)
        {
            if (!log.IsDebugEnabled)
            {
                return;
            }
            log.Debug(msg(code, message));
        }
        public void debug(RCode code, MessageSend send)
        {
            if (!log.IsDebugEnabled)
            {
                return;
            }
            if (send != null)
                log.Debug(msg(code, send()));
        }

        public override void debug(RCode code, string message, Exception e)
        {
            if (!log.IsDebugEnabled)
            {
                return;
            }

            string s = msg(code, message);

            if (e == null)
            {
                log.Debug(s);
            }
            else
            {
                log.Debug(s, e);
            }
        }

        public override void debug(RCode code, string f, params object[] pars) {
            if (!log.IsDebugEnabled)
            {
                return;
            }
            log.Debug(format($"{Result.detail(code)},{string.Format(StringUtil.logFormatConvert(f), pars)}"));
        }

        

        public override void error(RCode code, string message = null)
        {
            if (!log.IsErrorEnabled)
            {
                return;
            }
            log.Error(msg(code, message));
        }

        public override void error(RCode code, string message, Exception e)
        {
            
            string s = errorMsg(code, message);

            if (e == null)
            {
                log.Error(s);
            }
            else
            {
                log.Error(s, e);
            }
        }

        public override void error(RCode code, string fmt, params object[] pars) {
            if (!log.IsErrorEnabled)
            {
                return;
            }
            log.Error(format($"{Result.detail(code)},{string.Format(StringUtil.logFormatConvert(fmt), pars)}"));
        }

        public override void info(RCode code, string message = null)
        {
            if (!log.IsInfoEnabled)
            {
                return;
            }

            log.Info(msg(code, message));
        }

        public override void info(RCode code, string message, Exception e)
        {
            string s = msg(code, message);

            if (e == null)
            {
                log.Info(s);
            }
            else
            {
                log.Info(s, e);
            }
        }

        public override void info(RCode code, string fmt, params object[] pars) {
            if (!log.IsInfoEnabled)
            {
                return;
            }
            log.Info(format($"{Result.detail(code)},{string.Format(StringUtil.logFormatConvert(fmt), pars)}"));
        }

        public override void warn(RCode code, string message = null)
        {
            if (!log.IsWarnEnabled)
            {
                return;
            }

            log.Warn(warnMsg(code, message));
        }

        public override void warn(RCode code, string message, Exception e)
        {
            string s = warnMsg(code, message);

            if (e == null)
            {
                log.Warn(s);
            }
            else
            {
                log.Warn(s, e);
            }
        }
        public override void warn(RCode code, string fmt, params object[] pars) {
            if (!log.IsWarnEnabled)
            {
                return;
            }
            log.Warn(format($"{Result.detail(code)},{string.Format(StringUtil.logFormatConvert(fmt), pars)}"));
        }
    }
    public abstract class LogBase: ILogger
    {
        protected readonly ILog log;
        private static int errorCount = 0;
        private static int warnCount = 0;
        protected LogBase(ILog log) {
            this.log = log;
        }

        protected LogBase(string name) {
            this.log = LogManager.GetLogger(name);
        }

        protected LogBase(Type type) {
            log = LogManager.GetLogger(type);
        }

        public abstract void debug(RCode code, string message = null);
        public void debug(string message) {
            if (!log.IsDebugEnabled) {
                return;
            }
            log.Debug(format(message));
        }

        public void debug(string f, params object[] pars) {
            if (!log.IsDebugEnabled) {
                return;
            }
            log.Debug(format(string.Format(StringUtil.logFormatConvert(f), pars)));
        }

        public abstract void debug(RCode code, string message, Exception e);
        public abstract void debug(RCode code, string format, params object[] pars);

        

        public abstract void error(RCode code, string message = null);
        public void error(string message) {
            if (!log.IsErrorEnabled) {
                return;
            }
            log.Error(format(message));
        }

        public void error(string message, Exception e) {
            if (!log.IsErrorEnabled) {
                return;
            }
            log.Error(format(message), e);
        }

        public abstract void error(RCode code, string message, Exception e);
        public abstract void error(RCode code, string format, params object[] pars);

        public int getErrorNum() {
            return errorCount;
        }

        public int getWarnNum() {
            return warnCount;
        }

        public void reset() {
            lock (this) {
                errorCount = 0;
                warnCount = 0;
            }
        }

        public abstract void info(RCode code, string message = null);
        public void info(string message) {
            if (!log.IsInfoEnabled) {
                return;
            }
            log.Info(format(message));
        }

        public void info(string f, params object[] pars) {
            if (!log.IsInfoEnabled) {
                return;
            }
            log.Info(format(string.Format(StringUtil.logFormatConvert(f), pars)));
        }

        public abstract void info(RCode code, string message, Exception e);
        public abstract void info(RCode code, string format, params object[] pars);

        public abstract void warn(RCode code, string message = null);
        public void warn(string message) {
            if (!log.IsWarnEnabled) {
                return;
            }
            log.Warn(message);
        }

        public abstract void warn(RCode code, string message, Exception e);
        public abstract void warn(RCode code, string format, params object[] pars);

        protected string msg(RCode code, string message = null)
        {
            var messageBlock = Result.detail(code);
            string s = null;
            if (string.IsNullOrEmpty(message))
            {
                s = format(messageBlock);
            }
            else
            {
                s = format(messageBlock, message);
            }
            return format(s);
        }
        protected string warnMsg(RCode code, string message = null)
        {
            var messageBlock = Result.detail(code);
            string s = null;
            if (string.IsNullOrEmpty(message))
            {
                s = format(messageBlock);
            }
            else
            {
                s = format(messageBlock, message);
            }

            warnCount++;
            return format(s);
        }
        protected string errorMsg(RCode code, string message = null)
        {
            var messageBlock = Result.detail(code);
            string s = null;
            if (string.IsNullOrEmpty(message))
            {
                s = format(messageBlock);
            } else
            {
                s = format(messageBlock, message);
            }

            errorCount++;
            return format(s);
        }

        protected string format(string str) {
            if (str.Length <= 256) {
                return str;
            }

            int count = str.Length / 256 + 1;
            StringBuilder si = new StringBuilder(str.Length + 5 * count);
            si.Append(str[0]);
            for (int i = 1; i < str.Length; i++) {
                if ((i & 255) == 0) {
                    si.Append('\n').Append("   -");
                }
                si.Append(str[i]);
            }
            return si.ToString();
        }
        private string format(Result.MessageBlock message)
        {
            return $"ErCode: #{message.code},msg:{message.message}";
        }

        private string format(Result.MessageBlock message, string msg)
        {
            return $"ErCode: #{message.code},msg:{message.message},tips:{msg}";
        }
    }
}