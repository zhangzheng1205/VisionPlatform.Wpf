using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionPlatform.Core;

namespace VisionPlatform.Wpf
{
    /// <summary>
    /// 场景创建/修改界面
    /// </summary>
    public class SceneViewModel : Screen
    {

        #region 属性

        /// <summary>
        /// 场景实例
        /// </summary>
        public Scene Scene
        {
            get;
            set;
        }

        /// <summary>
        /// 场景名称
        /// </summary>
        public string SceneName
        {
            get
            {
                return Scene?.Name;
            }
            set
            {
                if (Scene != null)
                {
                    Scene.Name = value;
                }
                NotifyOfPropertyChange(() => SceneName);
            }
        }


        #endregion

        #region 事件

        /// <summary>
        /// 场景配置完成事件
        /// </summary>
        public event EventHandler<SceneConfigurationCompletedEventArgs> SceneConfigurationCompleted;

        #endregion

        #region 方法



        #endregion

    }
}
