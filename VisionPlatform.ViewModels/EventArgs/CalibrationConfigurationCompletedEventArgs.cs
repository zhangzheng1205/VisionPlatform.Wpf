using Framework.Vision;
using System;

namespace VisionPlatform.ViewModels
{
    /// <summary>
    /// 创建标定文件事件参数
    /// </summary>
    public class CalibrationConfigurationCompletedEventArgs : EventArgs
    {
        #region 构造/析构接口

        /// <summary>
        /// 创建CalibrationConfigurationCompletedEventArgs新实例
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="calibParam">标定参数</param>
        /// <param name="isSuccess">成功标志</param>
        public CalibrationConfigurationCompletedEventArgs(string filePath, CalibParam calibParam, bool isSuccess)
        {
            FilePath = filePath;
            CalibParam = calibParam;
            IsSuccess = isSuccess;

        }

        #endregion

        #region 属性

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// 标定参数
        /// </summary>
        public CalibParam CalibParam { get; private set; }

        /// <summary>
        /// 成功标志
        /// </summary>
        public bool IsSuccess { get; private set; }

        #endregion
    }

}
