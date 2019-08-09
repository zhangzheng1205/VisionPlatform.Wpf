using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionPlatform.BaseType
{
    /// <summary>
    /// 视觉平台枚举
    /// </summary>
    public enum EVisionFrame
    {
        /// <summary>
        /// Halcon平台
        /// </summary>
        Halcon,

        /// <summary>
        /// VisionPro平台
        /// </summary>
        VisionPro,

        /// <summary>
        /// NIVision平台
        /// </summary>
        NIVision,

        /// <summary>
        /// 未知
        /// </summary>
        Unknown,
    }
}
