using Framework.Camera;
using System;

namespace VisionPlatform.ViewModels
{
    /// <summary>
    /// 相机选择事件参数
    /// </summary>
    public class CameraSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// 创建CameraSelectedEventArgs新实例
        /// </summary>
        /// <param name="Device">设备</param>
        public CameraSelectedEventArgs(DeviceInfo device)
        {
            Device = device;

        }

        /// <summary>
        /// 设备连接状态
        /// </summary>
        public DeviceInfo Device { get; private set; }

    }
}
