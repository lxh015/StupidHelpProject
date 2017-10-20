using System;

namespace Stupid.TxtLog
{
    /// <summary>
    /// 日志操作
    /// </summary>
    public class Logger
    {
        private static readonly ILogger _errorLogger;

        static Logger()
        {
            _errorLogger = new CommonLogger();
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="message"></param>
        public static void Write(object message)
        {
            _errorLogger.Write(message);
        }

        /// <summary>
        /// 写入异常日志
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="errormessage">当该项不为空时，日志消息内容以该项为主</param>
        public static void Write(Exception ex, string errormessage)
        {
            string message = string.Format("发生时间：{4};消息类型：{0};消息内容：{1};引发异常的方法：{2};引发异常源：{3}"
                    , ex.GetType().Name
                    , errormessage == "" ? ex.Message : errormessage
                    , ex.TargetSite
                    , ex.Source /*+ ex.StackTrace*/
                    , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    );
            Logger.Write(message);
        }
    }
}
