using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionPlatform.BaseType;

namespace GlassesLocateDemo
{
    public class SystemParams
    {
        /// <summary>
        /// 默认视觉框架
        /// </summary>
        public EVisionFrameType DefaultVisionFrameType { get; set; } = EVisionFrameType.Halcon;

        /// <summary>
        /// 默认相机框架
        /// </summary>
        public ECameraSdkType DefaultCameraSdkType { get; set; } = ECameraSdkType.VirtualCamera;

        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; } = "192.168.0.120";

        /// <summary>
        /// 训练集名称
        /// </summary>
        public string SrDetFilePath { get; set; } = "0903_finished.srDet";

        public string DefaultImageFile { get; set; } = "image.bmp";

    }
}
