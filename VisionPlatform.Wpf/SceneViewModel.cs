using Caliburn.Micro;
using Framework.Camera;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using VisionPlatform.BaseType;
using VisionPlatform.Core;

namespace VisionPlatform.Wpf
{
    /// <summary>
    /// 场景创建/修改界面
    /// </summary>
    public class SceneViewModel : Screen
    {
        #region 构造函数

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

                //还原已选择的相机
                if ((Scene?.VisionFrame?.IsEnableCamera == true) && (Scene?.IsCameraInit == true))
                {
                    if ((CameraFactory.Cameras.ContainsKey(Scene.CameraSerial ?? "")) && (Cameras.Contains(CameraFactory.Cameras[Scene.CameraSerial])))
                    {
                        SelectedCamera = CameraFactory.Cameras[Scene.CameraSerial];
                    }
                }
            }
        }

        #endregion

        #region 属性

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

                if (scene?.VisionFrame?.IsEnableCamera == true)
                {
                    UpdateCameras();
                }

                NotifyOfPropertyChange(() => SceneName);
                NotifyOfPropertyChange(() => IsEnableCamera);
                NotifyOfPropertyChange(() => IsVisionFrameValid);
                NotifyOfPropertyChange(() => IsSceneValid);
                NotifyOfPropertyChange(() => CanCreateScene);
                NotifyOfPropertyChange(() => MainSeparatorChar);
                NotifyOfPropertyChange(() => SubSeparatorChar);
                NotifyOfPropertyChange(() => Cameras);
                NotifyOfPropertyChange(() => SceneRunningWindow);
            }
        }

        private bool isSceneNameReadOnly = false;

        /// <summary>
        /// 场景名只读标志
        /// </summary>
        public bool IsSceneNameReadOnly
        {
            get
            {
                return isSceneNameReadOnly;
            }
            set
            {
                isSceneNameReadOnly = value;
                NotifyOfPropertyChange(() => IsSceneNameReadOnly);
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

        /// <summary>
        /// 场景名称
        /// </summary>
        public string SceneName
        {
            get
            {
                return Scene?.Name ?? "EmptyScene";
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

        /// <summary>
        /// 场景配置窗口
        /// </summary>
        public object SceneRunningWindow
        {
            get
            {
                return Scene?.VisionFrame?.RunningWindow ?? new Grid();
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

                if (value != null)
                {
                    Scene?.SetCamera(value.Info.SerialNumber);
                    UpdateCameraFile();
                }
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

        private ObservableCollection<string> cameraConfigFiles;

        /// <summary>
        /// 相机配置文件列表
        /// </summary>
        public ObservableCollection<string> CameraConfigFiles
        {
            get
            {
                return cameraConfigFiles;
            }
            set
            {
                cameraConfigFiles = value;
                NotifyOfPropertyChange(() => CameraConfigFiles);
            }
        }

        /// <summary>
        /// 选择的配置文件
        /// </summary>
        public string SelectedCameraConfigFile
        {
            get
            {
                return Scene?.CameraConfigFile;
            }
            set
            {
                if (Scene != null)
                {
                    Scene.SetCameraConfigFile(value);
                }
                NotifyOfPropertyChange(() => CameraConfigFiles);
            }
        }

        private ObservableCollection<string> cameraCalibrationFiles;

        /// <summary>
        /// 相机标定文件列表
        /// </summary>
        public ObservableCollection<string> CameraCalibrationFiles
        {
            get
            {
                return cameraCalibrationFiles;
            }
            set
            {
                cameraCalibrationFiles = value;
                NotifyOfPropertyChange(() => CameraCalibrationFiles);
            }
        }

        /// <summary>
        /// 选择的标定文件
        /// </summary>
        public string SelectedCalibrationFile
        {
            get
            {
                return Scene?.CameraConfigFile;
            }
            set
            {
                if ((Scene != null) && (value != "不选择任何文件"))
                {
                    Scene.SetCameraCalibrationFile(value);
                }
                NotifyOfPropertyChange(() => SelectedCalibrationFile);
            }
        }

        #endregion

        #region 分隔符

        /// <summary>
        /// 分隔符
        /// </summary>
        public string MainSeparatorChar
        {
            get
            {
                return Scene?.MainSeparatorChar.ToString();
            }
            set
            {
                var chars = value.ToCharArray();

                if ((chars?.Length > 0) && (Scene != null))
                {
                    Scene.MainSeparatorChar = chars[0];
                }
                NotifyOfPropertyChange(() => MainSeparatorChar);
            }
        }

        /// <summary>
        /// 结束符
        /// </summary>
        public string SubSeparatorChar
        {
            get
            {
                return Scene?.SubSeparatorChar.ToString();
            }
            set
            {
                var chars = value.ToCharArray();

                if ((chars?.Length > 0) && (Scene != null))
                {
                    Scene.SubSeparatorChar = chars[0];
                }
                NotifyOfPropertyChange(() => SubSeparatorChar);
            }
        }

        #endregion

        #region 场景控制


        private string visionResult;

        /// <summary>
        /// 视觉结果
        /// </summary>
        public string VisionResult
        {
            get
            {
                return visionResult;
            }
            set
            {
                visionResult = value;
                NotifyOfPropertyChange(() => VisionResult);
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
                if (string.IsNullOrEmpty(SceneName) || (Scene == null))
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

                if (Scene.IsVisionFrameInit)
                {
                    NotifyOfPropertyChange(() => SceneRunningWindow);
                }

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
                CameraFactory.AddAllCamera();
                Cameras = new ObservableCollection<ICamera>(CameraFactory.Cameras.Values);
            }
        }

        /// <summary>
        /// 更新相机文件
        /// </summary>
        private void UpdateCameraFile()
        {
            if (SelectedCamera != null)
            {
                //获取相机配置文件
                FileInfo[] configFileInfos = CameraFactory.GetCameraConfigFile(SelectedCamera?.Info.SerialNumber);
                CameraConfigFiles = new ObservableCollection<string>(configFileInfos.ToList().ConvertAll(x=>x.Name));

                //获取相机标定文件
                FileInfo[] calibrationFileInfos = CameraFactory.GetCameraCalibrationFile(SelectedCamera?.Info.SerialNumber);
                CameraCalibrationFiles = new ObservableCollection<string>(calibrationFileInfos.ToList().ConvertAll(x => x.Name));
                CameraCalibrationFiles.Add("不选择任何文件");
            }
            
        }

        private Window cameraViewWindow;

        /// <summary>
        /// 打开相机显示控件
        /// </summary>
        public void OpenCameraView()
        {
            try
            {
                if ((Scene?.VisionFrame.IsEnableCamera == true) && (string.IsNullOrEmpty(SelectedCamera?.Info?.SerialNumber)))
                {
                    throw new DriveNotFoundException("没有选定相机");
                }

                Scene.SetCamera(SelectedCamera.Info.SerialNumber);

                var view = new CameraView();
                var viewModel = (view.DataContext as CameraViewModel);
                viewModel.CameraConfigurationCompleted -= ViewModel_CameraConfigurationCompleted;
                viewModel.CameraConfigurationCompleted += ViewModel_CameraConfigurationCompleted;
                viewModel.SetCamera(Scene.Camera);

                //将控件嵌入窗口之中
                cameraViewWindow = new Window();
                cameraViewWindow.MinWidth = view.MinWidth;
                cameraViewWindow.MinHeight = view.MinHeight;
                cameraViewWindow.Width = view.MinWidth + 100;
                cameraViewWindow.Height = view.MinHeight + 100;
                cameraViewWindow.Content = view;
                cameraViewWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                //SceneParamDebugWindow.Owner = Window.GetWindow(this);
                cameraViewWindow.Title = "场景参数配置窗口";

                cameraViewWindow.ShowDialog();

            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
        }

        private void ViewModel_CameraConfigurationCompleted(object sender, CameraConfigurationCompletedEventArgs e)
        {
            cameraViewWindow.Close();

            //刷新相机配置文件
            UpdateCameraFile();

            //重新从配置文件中加载配置参数
            Scene.SetCameraConfigFile(SelectedCameraConfigFile);
        }

        /// <summary>
        /// 打开标定控件
        /// </summary>
        public void OpenCalibrationView()
        {
            try
            {
                if ((Scene?.VisionFrame.IsEnableCamera == true) && (string.IsNullOrEmpty(SelectedCamera?.Info?.SerialNumber)))
                {
                    throw new DriveNotFoundException("没有选定相机");
                }

                //var view = new CalibrationView();
                //var viewModel = (view.DataContext as CalibrationViewModel);
                //viewModel.CalibrationConfigurationCompleted -= ViewModel_CalibrationConfigurationCompleted;
                //viewModel.CalibrationConfigurationCompleted += ViewModel_CalibrationConfigurationCompleted;

            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
        }

        private void ViewModel_CalibrationConfigurationCompleted(object sender, CalibrationConfigurationCompletedEventArgs e)
        {
        }

        private Window SceneParamDebugWindow;

        /// <summary>
        /// 打开视觉参数
        /// </summary>
        public void OpenSceneParamDebugView()
        {
            try
            {
                var view = new SceneParamDebugView()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };
                var viewModel = (view.DataContext as SceneParamDebugViewModel);
                viewModel.Scene = Scene;
                viewModel.SceneConfigurationCompleted -= ViewModel_SceneConfigurationCompleted;
                viewModel.SceneConfigurationCompleted += ViewModel_SceneConfigurationCompleted;

                //将控件嵌入窗口之中
                SceneParamDebugWindow = new Window();
                SceneParamDebugWindow.MinWidth = view.MinWidth;
                SceneParamDebugWindow.MinHeight = view.MinHeight;
                SceneParamDebugWindow.Width = view.MinWidth + 600;
                SceneParamDebugWindow.Height = view.MinHeight + 200;
                SceneParamDebugWindow.Content = view;
                SceneParamDebugWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                //SceneParamDebugWindow.Owner = Window.GetWindow(this);
                SceneParamDebugWindow.Title = "场景参数配置窗口";
                //SceneParamDebugWindow.WindowState = WindowState.Maximized;

                SceneParamDebugWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
        }

        private void ViewModel_SceneConfigurationCompleted(object sender, SceneConfigurationCompletedEventArgs e)
        {
            SceneParamDebugWindow.Close();

            NotifyOfPropertyChange(() => SceneRunningWindow);
        }

        /// <summary>
        /// 通过本地图片执行
        /// </summary>
        /// <param name="file"></param>
        public void ExecuteByFile(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                return;
            }

            try
            {
                string visionResult = "";
                RunStatus runStatus = Scene?.ExecuteByFile(file, out visionResult);
                if ((runStatus.Result == EResult.Accept) || (runStatus.Result == EResult.Warning))
                {
                    VisionResult = visionResult;
                }
                else if ((runStatus.Result == EResult.Error) || (runStatus.Result == EResult.Reject))
                {
                    OnMessageRaised(MessageLevel.Err, runStatus.Message, runStatus.Exception);
                }
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
        }

        /// <summary>
        /// 执行场景
        /// </summary>
        public void Execute()
        {
            try
            {
                string visionResult = "";
                RunStatus runStatus = Scene?.Execute(2000, out visionResult);
                if ((runStatus.Result == EResult.Accept) || (runStatus.Result == EResult.Warning))
                {
                    VisionResult = visionResult;
                }
                else if ((runStatus.Result == EResult.Error) || (runStatus.Result == EResult.Reject))
                {
                    OnMessageRaised(MessageLevel.Err, runStatus.Message, runStatus.Exception);
                }
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
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
            //保存配置信息
            if (Scene != null)
            {
                if (Scene.VisionFrame?.IsEnableCamera == true)
                {
                    Scene.CameraSerial = SelectedCamera?.Info.SerialNumber;
                }

                try
                {
                    Scene.Init();
                }
                catch (Exception)
                {

                }
            }
            
            OnSceneConfigurationCompleted(Scene);
        }

        #endregion
    }
}
