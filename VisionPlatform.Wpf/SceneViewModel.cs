using Caliburn.Micro;
using Framework.Camera;
using System;
using System.Collections.ObjectModel;
using System.IO;
using VisionPlatform.Core;

namespace VisionPlatform.Wpf
{
    /// <summary>
    /// 场景创建/修改界面
    /// </summary>
    public class SceneViewModel : Screen
    {
        /// <summary>
        /// 创建SceneViewModel
        /// </summary>
        public SceneViewModel()
        {

        }

        /// <summary>
        /// 创建SceneViewModel
        /// </summary>
        /// <param name="scene"></param>
        public SceneViewModel(Scene scene)
        {
            if (scene != null)
            {
                Scene = scene;

                SceneName = scene.Name;
                UpdateCameras();

                if (Cameras.Contains(CameraFactory.Cameras[Scene.CameraSerial]))
                {
                    SelectedCamera = CameraFactory.Cameras[Scene.CameraSerial];
                }
            }
        }

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
                NotifyOfPropertyChange(() => SceneName);
                NotifyOfPropertyChange(() => IsEnableCamera);
                NotifyOfPropertyChange(() => IsVisionFrameValid);
                NotifyOfPropertyChange(() => IsSceneValid);
                NotifyOfPropertyChange(() => CanCreateScene);
                NotifyOfPropertyChange(() => SeparatorChar);
                NotifyOfPropertyChange(() => TerminatorChar);
                NotifyOfPropertyChange(() => Cameras);
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
        /// 视觉框架有效标志
        /// </summary>
        public bool IsVisionFrameValid
        {
            get
            {
                return Scene?.IsVisionFrameInit == true;
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
                NotifyOfPropertyChange(() => IsVisionFrameValid);
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

        /// <summary>
        /// 使能相机
        /// </summary>
        public bool IsEnableCamera
        {
            get
            {
                return Scene?.VisionFrame?.IsEnableCamera == true;
            }
        }

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

        /// <summary>
        /// 非虚拟相机
        /// </summary>
        public bool IsNotVirtualCamera
        {
            get
            {
                return CameraFactory.DefaultCameraSdkType != BaseType.ECameraSdkType.VirtualCamera;
            }
        }

        #endregion

        #region 分隔符

        /// <summary>
        /// 分隔符
        /// </summary>
        public string SeparatorChar
        {
            get
            {
                return Scene?.SeparatorChar.ToString();
            }
            set
            {
                var chars = value.ToCharArray();

                if ((chars?.Length > 0) && (Scene != null))
                {
                    Scene.SeparatorChar = chars[0];
                }
                NotifyOfPropertyChange(() => SeparatorChar);
            }
        }

        /// <summary>
        /// 结束符
        /// </summary>
        public string TerminatorChar
        {
            get
            {
                return Scene?.TerminatorChar.ToString();
            }
            set
            {
                var chars = value.ToCharArray();

                if ((chars?.Length > 0) && (Scene != null))
                {
                    Scene.TerminatorChar = chars[0];
                }
                NotifyOfPropertyChange(() => TerminatorChar);
            }
        }

        #endregion

        #endregion

        #region 事件

        /// <summary>
        /// 场景配置完成事件
        /// </summary>
        public event EventHandler<SceneConfigurationCompletedEventArgs> SceneConfigurationCompleted;

        /// <summary>
        /// 消息触发事件
        /// </summary>
        internal event EventHandler<MessageRaisedEventArgs> MessageRaised;

        #endregion

        #region 方法

        internal void OnMessageRaised(MessageLevel messageLevel, string message, Exception exception = null)
        {
            MessageRaised?.Invoke(this, new MessageRaisedEventArgs(messageLevel, message, exception));
        }

        /// <summary>
        /// 设置视觉算子文件
        /// </summary>
        /// <param name="file"></param>
        public void SetVisionOperaFile(string file)
        {
            try
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
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }

        }

        /// <summary>
        /// 创建场景
        /// </summary>
        public void CreateScene(string sceneName)
        {
            try
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
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
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
        /// 打开相机显示控件
        /// </summary>
        public void OpenCameraView()
        {
            try
            {
                if ((Scene?.VisionFrame.IsEnableCamera == true) && (string.IsNullOrEmpty(SelectedCamera?.Info?.Manufacturer)))
                {
                    throw new DriveNotFoundException("没有选定相机");
                }

                Scene.SetCamera(SelectedCamera.Info.Manufacturer);

                var view = new CameraView();
                var viewModel = (view.DataContext as CameraViewModel);
                viewModel.CameraConfigurationCompleted -= ViewModel_CameraConfigurationCompleted;
                viewModel.CameraConfigurationCompleted += ViewModel_CameraConfigurationCompleted;
                viewModel.SetCamera(Scene.Camera);

                SceneConfigView = view;
                IsEnableSceneConfig = false;
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
        }

        private void ViewModel_CameraConfigurationCompleted(object sender, CameraConfigurationCompletedEventArgs e)
        {
            SceneConfigView = null;
            IsEnableSceneConfig = true;
        }

        /// <summary>
        /// 打开标定控件
        /// </summary>
        public void OpenCalibrationView()
        {
            if ((Scene?.VisionFrame.IsEnableCamera == true) && (string.IsNullOrEmpty(SelectedCamera?.Info?.Manufacturer)))
            {
                throw new DriveNotFoundException("没有选定相机");
            }

            try
            {
                var view = new CalibrationView();
                var viewModel = (view.DataContext as CalibrationViewModel);
                viewModel.CalibrationConfigurationCompleted -= ViewModel_CalibrationConfigurationCompleted; ;
                viewModel.CalibrationConfigurationCompleted += ViewModel_CalibrationConfigurationCompleted; ;

                SceneConfigView = view;
                IsEnableSceneConfig = false;
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
        }

        private void ViewModel_CalibrationConfigurationCompleted(object sender, CalibrationConfigurationCompletedEventArgs e)
        {
            SceneConfigView = null;
            IsEnableSceneConfig = true;
        }

        /// <summary>
        /// 打开视觉参数
        /// </summary>
        public void OpenSceneParamDebugView()
        {
            try
            {
                if ((Scene?.VisionFrame.IsEnableCamera == true) && (!string.IsNullOrEmpty(SelectedCamera?.Info?.Manufacturer)))
                {
                    Scene.SetCamera(SelectedCamera.Info.Manufacturer);
                }

                var view = new SceneParamDebugView();
                var viewModel = (view.DataContext as SceneParamDebugViewModel);
                viewModel.Scene = Scene;
                viewModel.SceneConfigurationCompleted -= ViewModel_SceneConfigurationCompleted;
                viewModel.SceneConfigurationCompleted += ViewModel_SceneConfigurationCompleted;

                SceneConfigView = view;
                IsEnableSceneConfig = false;
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
        }

        private void ViewModel_SceneConfigurationCompleted(object sender, SceneConfigurationCompletedEventArgs e)
        {
            SceneConfigView = null;
            IsEnableSceneConfig = true;
        }

        /// <summary>
        /// 触发场景配置完成事件
        /// </summary>
        /// <param name="scene"></param>
        protected void OnSceneConfigurationCompleted(Scene scene)
        {
            SceneConfigurationCompleted?.Invoke(this, new SceneConfigurationCompletedEventArgs(scene));
        }

        /// <summary>
        /// 确认退出
        /// </summary>
        public void Accept()
        {
            //if (Scene != null)
            //{
            //    string file = $"VisionPlatform/Scene/{Scene.EVisionFrameType}/{Scene.Name}/Scene.json";
            //    Scene.Serialize(Scene, file);
            //}
            
            OnSceneConfigurationCompleted(Scene);
        }

        #endregion
    }
}
