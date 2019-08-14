using Framework.Camera;

namespace VisionPlatform.Wpf
{
    /// <summary>
    /// 相机配置参数
    /// </summary>
    public struct CameraConfigParam
    {
        /// <summary>
        /// 像素格式
        /// </summary>
        public EPixelFormatType PixelFormat;

        /// <summary>
        /// 触发模式
        /// </summary>
        public ETriggerModeStatus TriggerMode;

        /// <summary>
        /// 触发源
        /// </summary>
        public ETriggerSource TriggerSource;

        /// <summary>
        /// 有效触发信号(硬件)
        /// </summary>
        public ETriggerActivation TriggerActivation;

        /// <summary>
        /// 曝光值
        /// </summary>
        public double ExposureTime;

        /// <summary>
        /// 增益值
        /// </summary>
        public double Gain;

    }
}
