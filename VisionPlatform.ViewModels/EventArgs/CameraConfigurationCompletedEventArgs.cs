using Framework.Camera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionPlatform.ViewModels
{
    public class CameraConfigurationCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// 创建CameraConfigurationCompletedEventArgs新实例
        /// </summary>
        /// <param name="camera">相机</param>
        /// <param name="configParams">配置参数</param>
        /// <param name="filePath">文件路径</param>
        public CameraConfigurationCompletedEventArgs(ICamera camera, Dictionary<string, object> configParams, string filePath)
        {
            Camera = camera;
            ConfigParams = configParams;
            FilePath = filePath;
        }

        /// <summary>
        /// 相机接口
        /// </summary>
        public ICamera Camera { get; set; }

        /// <summary>
        /// 配置参数
        /// </summary>
        public Dictionary<string, object> ConfigParams { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }


    }
}
