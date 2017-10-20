namespace Stupid.TxtLog
{
    /// <summary>
    /// 日志接口
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="message"></param>
        void Write(object message);
    }
}