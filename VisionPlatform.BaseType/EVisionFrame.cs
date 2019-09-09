namespace VisionPlatform.BaseType
{
    /// <summary>
    /// 视觉平台枚举
    /// </summary>
    public enum EVisionFrameType
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 基于DLL的Halcon平台
        /// </summary>
        HalconDLL,

        /// <summary>
        /// 基于HDevelop的Halcon平台
        /// </summary>
        HalconHdev,

        /// <summary>
        /// 基于DLL的VisionPro平台
        /// </summary>
        VisionProDLL,

        /// <summary>
        /// 基于VPP加载的VisionPro平台
        /// </summary>
        VisionProVpp,

        /// <summary>
        /// NIVision平台
        /// </summary>
        NIVision,

    }
}
