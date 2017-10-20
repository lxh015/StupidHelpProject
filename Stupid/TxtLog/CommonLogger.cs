using System;
using System.IO;

namespace Stupid.TxtLog
{
    /// <summary>
    /// 日志帮助类
    /// </summary>
    public class CommonLogger : ILogger
    {
        /// <summary>
        /// 根地址
        /// </summary>
        protected string RootPath { get; private set; }

        /// <summary>
        /// 日志帮助类
        /// </summary>
        public CommonLogger()
        {
            RootPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        }

        /// <summary>
        /// 设置地址
        /// </summary>
        /// <returns></returns>
        protected virtual string GetPath()
        {
            var now = DateTime.Now;
            var path = string.Format("Log/{0}/{1}", now.Year, now.Month);
            return Path.Combine(RootPath, path);
        }

        /// <summary>
        /// 设置文件地址
        /// </summary>
        /// <returns></returns>
        protected virtual string GetFilePath()
        {
            var now = DateTime.Now;
            var path = GetPath();
            var filename = string.Format("{0}.log", now.Day);
            return Path.Combine(path, filename);
        }

        /// <summary>
        /// 设置日志内容
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual string GetLogContext(object message)
        {
            return string.Format("{0}{1}", message.ToString(), Environment.NewLine);
        }

        /// <summary>
        /// 写入日志数据
        /// </summary>
        /// <param name="message"></param>
        public void Write(object message)
        {
            try
            {
                var path = GetPath();
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                var file = GetFilePath();
                if (!File.Exists(file))
                    File.Create(file).Close();
                File.AppendAllText(file, GetLogContext(message));
            }
            catch (Exception)
            {

            }
        }
    }
}