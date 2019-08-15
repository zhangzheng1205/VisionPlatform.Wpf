using System;
using VisionPlatform.Core;

namespace VisionPlatform.Wpf
{
    /// <summary>
    /// 场景配置完成事件
    /// </summary>
    public class SceneConfigurationCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// 创建CompletedSceneConfigEventArgs新实例
        /// </summary>
        /// <param name="scene">场景参数</param>
        public SceneConfigurationCompletedEventArgs(Scene scene)
        {
            Scene = scene;

        }

        /// <summary>
        /// 场景参数
        /// </summary>
        public Scene Scene { get; set; }

    }
}
