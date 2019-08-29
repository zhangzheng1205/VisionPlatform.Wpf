using Framework.Vision;
using System;

namespace VisionPlatform.Wpf
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
        /// <param name="calibParam">标定参数</param>
        public CalibrationConfigurationCompletedEventArgs(CalibParam calibParam)
        {
            CalibParam = calibParam;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 标定参数
        /// </summary>
        public CalibParam CalibParam { get; private set; }

        #endregion
    }

}
