using Framework.Camera;
using System;
using VisionPlatform.Core;

namespace VisionPlatform.Wpf
{
    /// <summary>
    /// 场景配置完成事件
    /// </summary>
    public class CameraConfigurationCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// 创建CameraConfigurationCompletedEventArgs新实例
        /// </summary>
        public CameraConfigurationCompletedEventArgs()
        {

        }

        /// <summary>
        /// 创建CameraConfigurationCompletedEventArgs新实例
        /// </summary>
        /// <param name="camera">相机实例</param>
        public CameraConfigurationCompletedEventArgs(ICamera camera)
        {
            Camera = camera;

        }

        /// <summary>
        /// 相机实例
        /// </summary>
        public ICamera Camera { get; }

    }
}
