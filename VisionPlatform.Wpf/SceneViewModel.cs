using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO;
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

        /// <summary>
        /// 设置相机算子文件
        /// </summary>
        /// <param name="file"></param>
        public void SetCameraOperaFile(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                return;
            }

            //校验场景实例
            if (string.IsNullOrEmpty(SceneName))
            {
                throw new ArgumentException("Scene invalid");
            }

            //校验文件路径合法性
            if (!File.Exists(file))
            {
                throw new FileNotFoundException("Vision opera file invalid");
            }

            //确认文件是否本地路径,如果不是,则复制到本地路径下
            string dstFile = $"VisionPlatform/Scene/{Scene.EVisionFrameType}/{Scene.Name}/Opera/{Path.GetFileName(file)}";
            if (Path.GetFullPath(dstFile) != file)
            {
                try
                {
                    File.Copy(file, dstFile, true);
                }
                catch (IOException)
                {
                    //如果是同一个文件,则会报IO异常,过滤掉此异常
                }
            }

            Scene.VisionOperaFile = dstFile;
        }

        /// <summary>
        /// 创建场景
        /// </summary>
        public void CreateScene()
        {
            Scene = new Scene();
        }

        /// <summary>
        /// 修改场景
        /// </summary>
        /// <param name="scene"></param>
        public void ModifyScene(Scene scene)
        {
            Scene = scene;
        }

        #endregion

    }
}
