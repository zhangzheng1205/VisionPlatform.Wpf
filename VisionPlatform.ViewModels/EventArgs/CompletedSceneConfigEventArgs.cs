using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionPlatform.Base;

namespace VisionPlatform.ViewModels
{
    public class CompletedSceneConfigEventArgs : EventArgs
    {
        /// <summary>
        /// 创建CompletedSceneConfigEventArgs新实例
        /// </summary>
        /// <param name="scene">场景参数</param>
        public CompletedSceneConfigEventArgs(IScene scene)
        {
            Scene = scene;

        }

        /// <summary>
        /// 场景参数
        /// </summary>
        public IScene Scene { get; set; }

    }
}
