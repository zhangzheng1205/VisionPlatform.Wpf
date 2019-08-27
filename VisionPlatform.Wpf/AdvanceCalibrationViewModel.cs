using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionPlatform.Core;

namespace VisionPlatform.Wpf
{
    /// <summary>
    /// AdvanceCalibrationView模型
    /// </summary>
    public class AdvanceCalibrationViewModel : Screen
    {

        #region 属性

        private ObservableCollection<Scene> scenes;

        /// <summary>
        /// 场景列表
        /// </summary>
        public ObservableCollection<Scene> Scenes
        {
            get { return scenes; }
            set { scenes = value; }
        }

        private Scene scene;

        /// <summary>
        /// 当前场景实例
        /// </summary>
        public Scene Scene
        {
            get { return scene; }
            set { scene = value; }
        }

        #endregion

        #region 事件

        internal void OnMessageRaised(MessageLevel messageLevel, string message, Exception exception = null)
        {
            MessageRaised?.Invoke(this, new MessageRaisedEventArgs(messageLevel, message, exception));
        }

        /// <summary>
        /// 消息触发事件
        /// </summary>
        internal event EventHandler<MessageRaisedEventArgs> MessageRaised;

        #endregion

    }
}
