using Framework.Camera;

namespace VisionPlatform.BaseType
{
    /// <summary>
    /// 相机配置参数
    /// </summary>
    public struct CameraConfigParam
    {
        /// <summary>
        /// 像素格式
        /// </summary>
        public EPixelFormatType PixelFormat { get; set; }

        /// <summary>
        /// 触发模式
        /// </summary>
        public ETriggerModeStatus TriggerMode { get; set; }

        /// <summary>
        /// 触发源
        /// </summary>
        public ETriggerSource TriggerSource { get; set; }

        /// <summary>
        /// 有效触发信号(硬件)
        /// </summary>
        public ETriggerActivation TriggerActivation { get; set; }

        /// <summary>
        /// 曝光值
        /// </summary>
        public double ExposureTime { get; set; }

        /// <summary>
        /// 增益值
        /// </summary>
        public double Gain { get; set; }

    }
}
