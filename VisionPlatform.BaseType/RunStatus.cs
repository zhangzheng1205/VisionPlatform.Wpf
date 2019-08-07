using System;

namespace VisionPlatform.BaseType
{
    /// <summary>
    /// 运行状态
    /// </summary>
    public class RunStatus
    {
        /// <summary>
        /// 处理时间(单位:MS)
        /// </summary>
        public double ProcessingTime { get; } = 0;

        /// <summary>
        /// 执行结果
        /// </summary>
        public EResult Result { get; } = EResult.Accept;

        /// <summary>
        /// 消息
        /// </summary>
        /// <remarks>
        /// 当Result为Accept时为null,其他情况时为警告/错误消息;
        /// </remarks>
        public string Message { get; } = null;

        /// <summary>
        /// 异常信息
        /// </summary>
        /// <remarks>
        /// 执行过程的异常信息,若无异常则为null;
        /// </remarks>
        public Exception Exception { get; } = null;

        /// <summary>
        /// 创建RunStatus新实例
        /// </summary>
        public RunStatus()
        {

        }

        /// <summary>
        /// 创建RunStatus新实例
        /// </summary>
        /// <param name="processingTime">处理时间</param>
        public RunStatus(double processingTime)
        {
            Result = EResult.Accept;
            ProcessingTime = processingTime;
            Message = null;
            Exception = null;

        }

        /// <summary>
        /// 创建RunStatus新实例
        /// </summary>
        /// <param name="processingTime">处理时间</param>
        /// <param name="result">执行结果</param>
        /// <param name="message">结果信息</param>
        public RunStatus(double processingTime, EResult result, string message) : this(processingTime)
        {
            Result = result;
            Message = message;
        }

        /// <summary>
        /// 创建RunStatus新实例
        /// </summary>
        /// <param name="processingTime">处理时间</param>
        /// <param name="result">执行结果</param>
        /// <param name="message">结果信息</param>
        /// <param name="exception">异常信息</param>
        public RunStatus(double processingTime, EResult result, string message, Exception exception) : this(processingTime, result, message)
        {
            Exception = exception;
        }

    }

    /// <summary>
    /// 执行结果枚举
    /// </summary>
    public enum EResult
    {
        /// <summary>
        /// 错误
        /// </summary>
        Error = -1,

        /// <summary>
        /// 接受
        /// </summary>
        Accept = 0,

        /// <summary>
        /// 警告
        /// </summary>
        Warning = 1,

        /// <summary>
        /// 拒绝
        /// </summary>
        Reject = 2
    }
}
