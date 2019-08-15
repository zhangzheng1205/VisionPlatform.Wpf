using Caliburn.Micro;
using Framework.Camera;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionPlatform.Core;
using System.Windows.Controls;

namespace VisionPlatform.Wpf
{
    /// <summary>
    /// 场景创建/修改界面
    /// </summary>
    public class SceneViewModel : Screen
    {
        #region 属性

        private bool isEnableSceneConfig = true;

        /// <summary>
        /// 锁场景标志,在设置子窗口时为true
        /// </summary>
        public bool IsEnableSceneConfig
        {
            get
            {
                return isEnableSceneConfig;
            }
            set
            {
                isEnableSceneConfig = value;
                NotifyOfPropertyChange(() => IsEnableSceneConfig);
            }
        }

        private Scene scene;

        /// <summary>
        /// 场景实例
        /// </summary>
        public Scene Scene
        {
            get
            {
                return scene;
            }
            set
            {
                scene = value;
                NotifyOfPropertyChange(() => IsSceneValid);
                NotifyOfPropertyChange(() => CanCreateScene);
            }
        }

        /// <summary>
        /// 场景有效标志
        /// </summary>
        public bool IsSceneValid
        {
            get
            {
                return (Scene == null) ? false : true;
            }
        }

        /// <summary>
        /// 允许创建场景标志
        /// </summary>
        public bool CanCreateScene
        {
            get
            {
                return (Scene == null) ? true : false;
            }
        }

        private string sceneName;

        /// <summary>
        /// 场景名称
        /// </summary>
        public string SceneName
        {
            get
            {
                return sceneName;
            }
            set
            {
                sceneName = value;
                NotifyOfPropertyChange(() => SceneName);
            }
        }

        /// <summary>
        /// 视觉算子文件
        /// </summary>
        public string VisionOperaFile
        {
            get
            {
                return Scene?.VisionOperaFile;
            }
            set
            {
                if (Scene != null)
                {
                    Scene.VisionOperaFile = value;
                }
                NotifyOfPropertyChange(() => VisionOperaFile);
            }
        }

        private object sceneConfigView;

        /// <summary>
        /// 场景配置窗口
        /// </summary>
        public object SceneConfigView
        {
            get
            {
                return sceneConfigView;
            }
            set
            {
                sceneConfigView = value;
                NotifyOfPropertyChange(() => SceneConfigView);
            }
        }

        #region 相机
        
        private ObservableCollection<ICamera> cameras = new ObservableCollection<ICamera>();

        /// <summary>
        /// 相机列表
        /// </summary>
        public ObservableCollection<ICamera> Cameras
        {
            get
            {
                return cameras;
            }
            set
            {
                cameras = value;
                NotifyOfPropertyChange(() => Cameras);
            }
        }

        private ICamera selectedCamera;

        /// <summary>
        /// 选择的相机
        /// </summary>
        public ICamera SelectedCamera
        {
            get
            {
                return selectedCamera;
            }
            set
            {
                selectedCamera = value;
                NotifyOfPropertyChange(() => SelectedCamera);
            }
        }


        #endregion

        #endregion

        #region 事件

        /// <summary>
        /// 场景配置完成事件
        /// </summary>
        public event EventHandler<SceneConfigurationCompletedEventArgs> SceneConfigurationCompleted;

        #endregion

        #region 方法

        /// <summary>
        /// 设置视觉算子文件
        /// </summary>
        /// <param name="file"></param>
        public void SetVisionOperaFile(string file)
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

            Scene.SetVisionOperaFile(file);
            VisionOperaFile = Scene.VisionOperaFile;

        }

        /// <summary>
        /// 创建场景
        /// </summary>
        public void CreateScene(string sceneName)
        {
            if (VisionFrameFactory.DefaultVisionFrameType != BaseType.EVisionFrameType.Unknown)
            {
                Scene = new Scene(sceneName, VisionFrameFactory.DefaultVisionFrameType);

                if (Scene.VisionFrame.IsEnableCamera == true)
                {
                    UpdateCameras();
                }
            }
        }

        /// <summary>
        /// 修改场景
        /// </summary>
        /// <param name="scene"></param>
        public void ModifyScene(Scene scene)
        {
            Scene = scene;
        }

        /// <summary>
        /// 更新相机
        /// </summary>
        public void UpdateCameras()
        {
            Cameras.Clear();
            if ((CameraFactory.Cameras?.Values != null) && (Scene?.VisionFrame?.IsEnableCamera == true))
            {
                Cameras = new ObservableCollection<ICamera>(CameraFactory.Cameras.Values);
            }
        }

        /// <summary>
        /// 打开视觉参数
        /// </summary>
        public void OpenSceneParamDebugView()
        {
            if ((Scene?.VisionFrame.IsEnableCamera == true) && (string.IsNullOrEmpty(SelectedCamera?.Info?.Manufacturer)))
            {
                throw new DriveNotFoundException("没有选定相机");
            }

            Scene.SetCamera(SelectedCamera.Info.Manufacturer);

            var view = new SceneParamDebugView();
            var viewModel = (view.DataContext as SceneParamDebugViewModel);
            viewModel.Scene = Scene;
            viewModel.SceneConfigurationCompleted -= ViewModel_SceneConfigurationCompleted;
            viewModel.SceneConfigurationCompleted += ViewModel_SceneConfigurationCompleted;

            SceneConfigView = view;
            IsEnableSceneConfig = false;
        }

        private void ViewModel_SceneConfigurationCompleted(object sender, SceneConfigurationCompletedEventArgs e)
        {
            SceneConfigView = null;
            IsEnableSceneConfig = true;
        }

        #endregion

    }
}
