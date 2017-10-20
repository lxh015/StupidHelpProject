using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LogHandle.LogEnum;

namespace LogHandle
{
    /// <summary>
    /// 日志接口
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// 写入Log
        /// </summary>
        /// <param name="type">日志类型</param>
        /// <param name="message">日志信息</param>
        void Write(LogType type, string message);

        /// <summary>
        /// 写入Log
        /// </summary>
        /// <param name="type">日志类型</param>
        /// <param name="exception">异常信息</param>
        void Write(LogType type, Exception exception);

        /// <summary>
        /// 写入Log
        /// </summary>
        /// <param name="type">日志类型</param>
        /// <param name="message">描述信息</param>
        /// <param name="exception">异常信息</param>
        void Write(LogType type, string message, Exception exception);
    }
}
