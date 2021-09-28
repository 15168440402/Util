using System;

namespace Util
{
    /// <summary>
    /// 标识异常是Util框架里抛出的异常
    /// </summary>
    public class UtilException : Exception
    {
        private UtilException(string message, string? suggest, Exception? innerException) : base($"[Util内部异常]：{message}", innerException)
        {
            Suggest = suggest is null ? $"[内部建议]：{suggest}" : "无建议";
        }
        public UtilException() : this("内部错误", suggest: null, null)
        {

        }
        public UtilException(string message, string? suggest = null) : this(message, suggest, null)
        {

        }
        public UtilException(string message, Exception innerException, string? suggest = null) : this(message, suggest, innerException)
        {

        }
        /// <summary>
        /// 建议
        /// <para>一般会给出解决异常问题的建议或指出代码正确的使用姿势</para>
        /// </summary>
        public string Suggest { get; set; }
    }
}
