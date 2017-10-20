using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogHandle
{
    /// <summary>
    /// 日志枚举
    /// </summary>
    public static class LogEnum
    {
        /// <summary>
        /// 程序类型
        /// </summary>
        public enum AppType
        {
            /// <summary>
            /// 网站类程序
            /// </summary>
            [Description("网站类程序")]
            web,
            /// <summary>
            /// 应用类程序
            /// </summary>
            [Description("应用类程序")]
            exe,
        }

        /// <summary>
        /// 日志字符串主体类型（分别为数据库，用户，和默认为日志类型主体的字符串），
        /// 主要用于选择日志中默认字符串。
        /// </summary>
        public enum LogMainType
        {
            /// <summary>
            /// 默认
            /// </summary>
            [Description("默认")]
            def,
            /// <summary>
            /// 用户
            /// </summary>
            [Description("用户")]
            user,
            /// <summary>
            /// 表名
            /// </summary>
            [Description("表名")]
            tablename,
            /// <summary>
            /// 用户自定义
            /// </summary>
            [Description("用户自定义")]
            custom,
        }

        /// <summary>
        /// 日志类型
        /// </summary>
        public enum LogType
        {
            /// <summary>
            /// 简单日志
            /// </summary>
            [Description("Log")]
            log,
            /// <summary>
            /// 登录日志
            /// </summary>
            [Description("Login")]
            login,
            /// <summary>
            /// 操作日志
            /// </summary>
            [Description("Operation")]
            operation,
            /// <summary>
            /// 错误日志
            /// </summary>
            [Description("Error")]
            error,
            /// <summary>
            /// 致命日志
            /// </summary>
            [Description("Denger")]
            denger,
        }
    }
}
