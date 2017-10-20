using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using static LogHandle.LogEnum;
using System.Text;

namespace LogHandle
{
    /// <summary>
    /// 日志处理
    /// </summary>
    public class LogHandle : ILog
    {
        #region 内容主体字符串
        /// <summary>
        /// 默认内容字符串
        /// </summary>
        private readonly string defaultString = @"[datetime] 日志类型：type，内容：
    context";
        /// <summary>
        /// 用户为主体的内容字符串
        /// </summary>
        private readonly string hasUserString = @"[datetime] 用户：user，日志类型：type，内容：
    context";
        /// <summary>
        /// 表名为主体的内容字符串
        /// </summary>
        private readonly string hasDbTypeString = @"[datetime] 数据库表名：tablename，用户：user，日志类型：type，内容：
    context";

        /// <summary>
        /// 替换名称datatime
        /// </summary>
        private readonly string ctDString = "datetime";
        /// <summary>
        /// 替换名称type
        /// </summary>
        private readonly string ctTString = "type";
        /// <summary>
        /// 替换名称context
        /// </summary>
        private readonly string ctCString = "context";
        /// <summary>
        /// 替换名称user
        /// </summary>
        private readonly string ctUString = "user";
        /// <summary>
        /// 替换名称tablename
        /// </summary>
        private readonly string ctTBString = "tablename";
        #endregion

        /// <summary>
        /// 默认将执行替换的string。
        /// </summary>
        public List<string> replaceStringArray = new List<string>();

        /// <summary>
        /// 日志内容。
        /// 将在xml文档中自动获取Key为StupidContext关键字的字符串做为主体，也可以不添加该关键字，直接使用自带默认值。
        /// </summary>
        public string contextText { get; private set; }

        /// <summary>
        /// 将默认加载内容字符串要替换的字符串。
        /// </summary>
        public LogHandle()
        {
            taskToSetXml = new Task(() =>
            {
                SetContext();
            });
            taskToSetXml.Start();

            replaceStringArray.Add(this.ctCString);
            replaceStringArray.Add(this.ctDString);
            replaceStringArray.Add(this.ctTBString);
            replaceStringArray.Add(this.ctTString);
            replaceStringArray.Add(this.ctUString);

            Task.WaitAll(taskToSetXml);
            taskToSetXml.Dispose();
            this.mainType = LogMainType.def;
        }

        //public LogHandle() : this()
        //{

        //    if (string.IsNullOrEmpty(this.custom))
        //        if (mainType == LogMainType.custom)

        //}

        #region 基础属性及字段
        Task taskToSetXml;
        private LogMainType mainType { get; set; }

        /// <summary>
        /// 程序类型
        /// </summary>
        private AppType appType { get; set; }
        /// <summary>
        /// 保存文件路径，示例：/log/log.txt。
        /// <remark>请严格按照示例添加，并使用txt文本格式。</remark>
        /// </summary>
        private string savePath { get; set; }
        /// <summary>
        /// 文件基础名称（根据配置信息读取）
        /// </summary>
        private string fileName
        {
            get
            {
                if (string.IsNullOrEmpty(this.savePath))
                    return "";
                string result = String.Empty;
                result = this.savePath.Remove(0, this.savePath.LastIndexOf(@"/") + 1);

                return result.Replace(this.FileTypeName, "");
            }
        }

        //private string logFileName { get; set; }
        /// <summary>
        /// 默认保存路径
        /// </summary>
        private readonly string _savePath = "/log/log.txt";
        /// <summary>
        /// 默认Cache保存路径关键字
        /// </summary>
        private readonly string _saveKey = "LogSP";

        /// <summary>
        /// 设定记录格式
        /// </summary>
        private string custom { get; set; }
        /// <summary>
        /// 默认Cache保存记录格式关键字
        /// </summary>
        private readonly string _customKey = "Custom";

        /// <summary>
        /// 单个文件最大字节(kb)
        /// </summary>
        private readonly int maxSize = 1024;

        /// <summary>
        /// 文件类型
        /// </summary>
        private readonly string FileTypeName = ".txt";
        #endregion

        #region 基础信息设置
        /// <summary>
        /// 获取Log设置信息
        /// </summary>
        private void SetContext()
        {
            if (!MemoryCache.Default.Contains(this._saveKey) || !MemoryCache.Default.Contains(this._customKey) || MemoryCache.Default.Count() == 0)
                SetContextFromXml();
            else
                SetContextFromCache();
        }

        /// <summary>
        /// 从缓存中获取设置信息
        /// </summary>
        private void SetContextFromCache()
        {
            var tempCustom = CacheGet(this._customKey);
            this.custom = tempCustom == null ? this.defaultString : tempCustom.ToString();
            var tempSavePath = CacheGet(this._saveKey);
            this.savePath = tempSavePath == null ? this._savePath : tempSavePath.ToString();
        }

        /// <summary>
        /// 从xml数据中获取设置信息
        /// </summary>
        private void SetContextFromXml()
        {
            string path = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
            xml.Load(path);

            var appSettings = xml.GetElementsByTagName("appSettings")[0].ChildNodes;
            int allSet = 0;
            foreach (System.Xml.XmlNode item in appSettings)
            {
                string key = item.Attributes[0].Value;
                if (key == "savePath")
                {
                    if (null != item.Attributes[1])
                        this.savePath = item.Attributes[1].Value;

                    allSet++;
                    goto Next;
                }

                if (key == "mainType")
                {
                    if (null != item.Attributes[1])
                        this.custom = item.Attributes[1].Value;

                    allSet++;
                    goto Next;
                }

                Next:
                if (allSet == 2)
                    break;
            }
            if (string.IsNullOrEmpty(this.savePath))
                this.savePath = this._savePath;
            if (null == this.custom)//用以保证不为Null
                this.custom = this.defaultString;

            CacheInsert(this._saveKey, this.savePath);
            CacheInsert(this._customKey, this.custom);
        }

        /// <summary>
        /// 添加或设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void CacheInsert(string key, string value)
        {
            var item = new CacheItem(key, value);
            var policy = CreatePolicy(null, DateTime.MaxValue);
            if (!MemoryCache.Default.Contains(key))
                MemoryCache.Default.Add(item, policy);
            else
                MemoryCache.Default.Set(item, policy);
        }

        /// <summary>
        /// 获取缓存信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private object CacheGet(string key)
        {
            if (MemoryCache.Default.Contains(key))
                return MemoryCache.Default[key];
            else
                return null;
        }

        /// <summary>
        /// 建立新的缓存过期信息
        /// </summary>
        /// <param name="slidingExpiration"></param>
        /// <param name="absoluteExpiration"></param>
        /// <returns></returns>
        private CacheItemPolicy CreatePolicy(TimeSpan? slidingExpiration, DateTime? absoluteExpiration)
        {
            var policy = new CacheItemPolicy();
            if (absoluteExpiration.HasValue)
                policy.AbsoluteExpiration = absoluteExpiration.Value;
            if (slidingExpiration.HasValue)
                policy.SlidingExpiration = slidingExpiration.Value;
            policy.Priority = CacheItemPriority.Default;
            return policy;
        }

        /// <summary>
        /// 判别程序类型
        /// </summary>
        private void DetermineAppType()
        {
            string configFileName = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            var tempArray = MakePathToArray(configFileName);
            if (tempArray.Last().Contains("exe"))
                this.appType = AppType.exe;
            else
                this.appType = AppType.web;
        }

        private string[] MakePathToArray(string configPath)
        {
            try
            {
                string[] temp = configPath.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);
                if (temp.Length == 1)
                {
                    temp = configPath.Split(new string[] { "//" }, StringSplitOptions.RemoveEmptyEntries);
                    if (temp.Length == 1)
                    {
                        temp = configPath.Split(new string[] { @"\\" }, StringSplitOptions.RemoveEmptyEntries);
                        if (temp.Length == 1)
                            temp = new string[] { configPath };
                    }
                }
                return temp;
            }
            catch
            {
                return new string[] { configPath };
            }
        }
        #endregion

        #region 接口实现
        /// <summary>
        /// 获取保存文件信息。
        /// 根据文件夹下文件数量及文件大小自动生成下一个文件名称。
        /// </summary>
        private string GetPathFile()
        {
            if (!this.savePath.Contains(this.FileTypeName))
                return this.fileName + this.FileTypeName;

            string temp = this.savePath.Remove(this.savePath.LastIndexOf("/")).Remove(0, 1);
            string fullPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + temp;

            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);

            string fileName = GetFileName(fullPath);

            string full = $"{fullPath}\\{fileName}";

            return full;
        }

        /// <summary>
        /// 获取最新文件名
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="fileNumber"></param>
        private string GetFileName(string fullPath)
        {
            string result = this.fileName + this.FileTypeName;
            var directoryInfo = new DirectoryInfo(fullPath);
            var fileArray = directoryInfo.GetFiles();
            if (fileArray.Length > 1)
            {
                var lastFile = fileArray.Last();
                if (lastFile.Length > this.maxSize * 1024)
                {
                    var fileNameArray = lastFile.Name.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                    if (fileNameArray.Length == 2)
                    {
                        int no = Convert.ToInt32(fileNameArray[1].Replace(".txt", ""));
                        result = $"{this.fileName}_{no + 1}.txt";
                    }
                    else
                        result = $"{this.fileName}_1.txt";
                }
                else
                    result = lastFile.Name;
            }
            return result;
        }

        private string GetWriteMessage(string message, LogType type)
        {
            string fullString = this.custom;
            foreach (var item in this.replaceStringArray)
            {
                if (fullString.Contains(item))
                {
                    switch (item)
                    {
                        case "datetime":
                            fullString = fullString.Replace(this.ctDString, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            break;
                        case "user":
                            fullString = fullString.Replace(this.ctUString, "");
                            break;
                        case "type":
                            fullString = fullString.Replace(this.ctTString, GetEnumString(type));
                            break;
                        case "context":
                            fullString = fullString.Replace(this.ctCString, message);
                            break;
                        case "tablename":
                            fullString = fullString.Replace(this.ctTBString, "");
                            break;
                        default:
                            break;
                    }
                }
            }

            return fullString;
        }

        /// <summary>
        /// 获取枚举值中Descript特性中的描述信息。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetEnumString(Enum value)
        {
            var members = value.GetType().GetMembers();
            foreach (var item in members)
            {
                if (item.Name == value.ToString())
                {
                    var attribute = item.GetCustomAttributes(false);
                    if (attribute.Where(p => p is DescriptionAttribute).Count() > 0)
                    {
                        return (attribute.Where(p => p is DescriptionAttribute).First() as DescriptionAttribute).Description;
                    }
                }
            }

            return value.ToString();
        }

        /// <summary>
        /// 释放FileStream资源
        /// </summary>
        /// <param name="fileStream"></param>
        private void FSClose(FileStream fileStream)
        {
            fileStream.Flush();
            fileStream.Dispose();
            fileStream.Close();
        }

        private void WriteMessage(string message)
        {
            Task taskToWriteLog = new Task(async () =>
            {
                string path = GetPathFile();
                FileStream fs;

                FileIsOpen:
                try
                {
                    fs = new FileStream(path, FileMode.Append);
                    string writeMessage = $"{Environment.NewLine}{message}";
                    byte[] fMessage = System.Text.Encoding.UTF8.GetBytes(writeMessage);

                    await fs.WriteAsync(fMessage, 0, fMessage.Length);
                    FSClose(fs);
                }
                catch
                {
                    await Task.Delay(500);
                    goto FileIsOpen;
                }
            });
            taskToWriteLog.Start();
        }

        public void Write(LogType type, string message)
        {
            string writeMessage = GetWriteMessage(message, type);
            WriteMessage(writeMessage);
        }

        public void Write(LogType type, Exception exception)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"异常对象：{exception.Source}{Environment.NewLine}");
            sb.Append($"引发异常的方法函数：{exception.TargetSite}{Environment.NewLine}");
            sb.Append($"异常信息：{exception.Message}{Environment.NewLine}");
            sb.Append($"描述调用堆栈的直接帧的字符串：{exception.StackTrace}{Environment.NewLine}");
            Write(type, sb.ToString());
        }

        public void Write(LogType type, string message, Exception exception)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"错误信息：{message}{Environment.NewLine}");
            sb.Append($"异常对象：{exception.Source}{Environment.NewLine}");
            sb.Append($"引发异常的方法函数：{exception.TargetSite}{Environment.NewLine}");
            sb.Append($"异常信息：{exception.Message}{Environment.NewLine}");
            sb.Append($"描述调用堆栈的直接帧的字符串：{exception.StackTrace}{Environment.NewLine}");
            Write(type, sb.ToString());
        }
        #endregion
    }
}
