﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VisionPlatform.BaseType;
using VisionPlatform.Core;
using VisionPlatform.IRobot;
using Framework.Infrastructure.Serialization;
using Framework.Vision;

namespace VisionPlatform.Wpf
{
    /// <summary>
    /// AdvanceCalibrationView模型
    /// </summary>
    public class AdvanceCalibrationViewModel : Screen
    {
        #region 构造函数

        /// <summary>
        /// 创建AdvanceCalibrationViewModel新实例
        /// </summary>
        public AdvanceCalibrationViewModel()
        {
            sceneManager = SceneManager.GetInstance();

            UpdateScenes();
            UpdateRobotAssembly();

            BaseCalibreationViewModel = new BaseCalibreationViewModel();
            BaseCalibreationViewModel.MessageRaised += BaseCalibreationViewModel_MessageRaised;
            BaseCalibreationViewModel.CalibrationPointListChanged += BaseCalibreationViewModel_CalibrationPointListChanged;
            BaseCalibreationViewModel.CalibrationPointSelectionChanged += BaseCalibreationViewModel_CalibrationPointSelectionChanged;

            NotifyOfPropertyChange(() => BaseCalibreationViewModel);

            PointIndexs.Clear();
            for (int i = 0; i < BaseCalibreationViewModel.CalibPointList.Count + 1; i++)
            {
                PointIndexs.Add(i);
            }
            RobotPointIndex = ImagePointIndex = PointIndexs[PointIndexs.Count - 1];
        }

        /// <summary>
        /// 标定点选择项改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BaseCalibreationViewModel_CalibrationPointSelectionChanged(object sender, CalibrationPointSelectionChangedEventArgs e)
        {
            if ((e.Index >= 0) && (e.Index < e.CalibPointList.Count))
            {
                ImagePointIndex = e.Index;
                RobotPointIndex = e.Index;
            }
            else
            {
                ImagePointIndex = e.CalibPointList.Count;
                RobotPointIndex = e.CalibPointList.Count;
            }
        }

        /// <summary>
        /// 标定点列表改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BaseCalibreationViewModel_CalibrationPointListChanged(object sender, CalibrationPointListChangedEventArgs e)
        {
            PointIndexs.Clear();
            for (int i = 0; i < e.CalibPointList.Count + 1; i++)
            {
                PointIndexs.Add(i);
            }
            RobotPointIndex = ImagePointIndex = PointIndexs[PointIndexs.Count - 1];
        }

        /// <summary>
        /// 消息触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BaseCalibreationViewModel_MessageRaised(object sender, MessageRaisedEventArgs e)
        {
            OnMessageRaised(e.MessageLevel, e.Message, e.Exception);
        }

        #endregion

        #region 字段

        SceneManager sceneManager;

        #endregion

        #region 属性

        #region 场景相关

        private ObservableCollection<Scene> scenes;

        /// <summary>
        /// 场景列表
        /// </summary>
        public ObservableCollection<Scene> Scenes
        {
            get
            {
                return scenes;
            }
            set
            {
                scenes = value;
                NotifyOfPropertyChange(() => Scenes);
            }
        }

        private Scene scene;

        /// <summary>
        /// 当前场景实例
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
                NotifyOfPropertyChange(() => Scene);
                NotifyOfPropertyChange(() => IsSceneValid);
                NotifyOfPropertyChange(() => CalibrationOperationViewWindow);
                NotifyOfPropertyChange(() => CameraSerial);
                NotifyOfPropertyChange(() => CameraName);
            }
        }

        /// <summary>
        /// 场景有效标志
        /// </summary>
        public bool IsSceneValid
        {
            get
            {
                return Scene?.IsInit ?? false;
            }
        }

        /// <summary>
        /// 标定算子显示窗口
        /// </summary>
        public object CalibrationOperationViewWindow
        {
            get
            {
                if (IsSceneValid)
                {
                    return Scene.VisionFrame.RunningWindow;
                }

                return new Grid();
            }
        }

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

        /// <summary>
        /// 相机序列号
        /// </summary>
        public string CameraSerial
        {
            get
            {
                if (IsSceneValid && (CameraFactory.DefaultCameraSdkType != ECameraSdkType.VirtualCamera))
                {
                    return Scene.CameraSerial;
                }

                return "";
            }
        }

        /// <summary>
        /// 相机名
        /// </summary>
        public string CameraName
        {
            get
            {
                if (IsSceneValid && (Scene?.VisionFrame.IsEnableCamera == true))
                {
                    return Scene.Camera.ToString();
                }
                return "";
            }
        }

        #endregion

        #region 机器人相关

        private ObservableCollection<Assembly> robotAssemblys = new ObservableCollection<Assembly>();

        /// <summary>
        /// 机器人通信实例列表
        /// </summary>
        public ObservableCollection<Assembly> RobotAssemblys
        {
            get
            {
                return robotAssemblys;
            }
            set
            {
                robotAssemblys = value;
                NotifyOfPropertyChange(() => RobotAssemblys);
            }
        }

        private Assembly robotAssembly;

        /// <summary>
        /// 机器人通信实例
        /// </summary>
        public Assembly RobotAssembly
        {
            get
            {
                return robotAssembly;
            }
            set
            {
                robotAssembly = value;
                NotifyOfPropertyChange(() => RobotAssembly);
                NotifyOfPropertyChange(() => IsRobotAssemblyValid);
            }
        }

        /// <summary>
        /// 机器人DLL集合有效标志
        /// </summary>
        public bool IsRobotAssemblyValid
        {
            get
            {
                return RobotAssembly != null;
            }
        }

        /// <summary>
        /// 机器人通信接口
        /// </summary>
        private IRobotCommunication robotCommunication;

        /// <summary>
        /// 机器人连接状态标志
        /// </summary>
        public bool IsRobotConnect
        {
            get
            {
                return robotCommunication?.IsConnect ?? false;
            }
        }

        private string robotConnectButtonContent = "连接";

        /// <summary>
        /// 机器人连接按钮内容
        /// </summary>
        public string RobotConnectButtonContent
        {
            get
            {
                return robotConnectButtonContent;
            }
            set
            {
                robotConnectButtonContent = value;
                NotifyOfPropertyChange(() => RobotConnectButtonContent);
            }
        }


        #endregion

        #region 标定控件相关

        /// <summary>
        /// 基本标定控件数据模型
        /// </summary>
        private BaseCalibreationViewModel baseCalibreationViewModel = new BaseCalibreationViewModel();

        /// <summary>
        /// 基本标定控件数据模型
        /// </summary>
        public BaseCalibreationViewModel BaseCalibreationViewModel
        {
            get
            {
                return baseCalibreationViewModel;
            }
            set
            {
                baseCalibreationViewModel = value;
                NotifyOfPropertyChange(() => BaseCalibreationViewModel);
            }
        }

        private ObservableCollection<int> pointIndexs = new ObservableCollection<int>();

        /// <summary>
        /// 图像点位索引集合
        /// </summary>
        public ObservableCollection<int> PointIndexs
        {
            get
            {
                return pointIndexs;
            }
            set
            {
                pointIndexs = value;
                NotifyOfPropertyChange(() => PointIndexs);
            }
        }

        private int imagePointIndex;

        /// <summary>
        /// 图像位置索引
        /// </summary>
        public int ImagePointIndex
        {
            get
            {
                return imagePointIndex;
            }
            set
            {
                imagePointIndex = value;
                NotifyOfPropertyChange(() => ImagePointIndex);
            }
        }

        private int robotPointIndex;

        /// <summary>
        /// 图像位置索引
        /// </summary>
        public int RobotPointIndex
        {
            get
            {
                return robotPointIndex;
            }
            set
            {
                robotPointIndex = value;
                NotifyOfPropertyChange(() => RobotPointIndex);
            }
        }

        #endregion

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

        /// <summary>
        /// 触发标定配置完成事件
        /// </summary>
        /// <param name="calibParam"></param>
        protected void OnCalibrationConfigurationCompleted(CalibParam calibParam)
        {
            CalibrationConfigurationCompleted?.Invoke(this, new CalibrationConfigurationCompletedEventArgs(calibParam));
        }

        /// <summary>
        /// 标定配置完成事件
        /// </summary>
        public event EventHandler<CalibrationConfigurationCompletedEventArgs> CalibrationConfigurationCompleted;

        #endregion

        #region 方法

        /// <summary>
        /// 更新场景列表
        /// </summary>
        public void UpdateScenes()
        {
            try
            {
                Scenes = new ObservableCollection<Scene>(sceneManager.Scenes.Values);
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
        }

        /// <summary>
        /// 更新机器人集合
        /// </summary>
        public void UpdateRobotAssembly()
        {
            try
            {
                RobotAssemblys.Clear();

                string robotDllRootPath = $"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}/VisionPlatform/RobotComunication";

                //遍历目录
                if (Directory.Exists(robotDllRootPath))
                {
                    var directoryInfo = new DirectoryInfo(robotDllRootPath);

                    foreach (var item in directoryInfo.GetDirectories())
                    {
                        //获取集合
                        var dllPath = $"{robotDllRootPath}/{item.Name}/{item.Name}.dll";

                        if (File.Exists(dllPath))
                        {
                            var assembly = Assembly.LoadFrom(dllPath);

                            //将dll添加到集合字典中
                            RobotAssemblys.Add(assembly);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
        }

        /// <summary>
        /// 设置图像标定点
        /// </summary>
        /// <param name="index">点位索引</param>
        /// <param name="px">图像坐标X</param>
        /// <param name="py">图像坐标Y</param>
        public void SetImageCalibrationPoint(int index, double px, double py)
        {
            try
            {
                if (index < BaseCalibreationViewModel.CalibPointList.Count)
                {
                    //覆盖
                    BaseCalibreationViewModel.Cover(index, px, py, BaseCalibreationViewModel.CalibPointList[index].Qx, BaseCalibreationViewModel.CalibPointList[index].Qy);
                }
                else if (index == BaseCalibreationViewModel.CalibPointList.Count)
                {
                    //追加
                    BaseCalibreationViewModel.Add(px, py, 0, 0);
                }
                else
                {
                    //无效输入
                }
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
        }

        /// <summary>
        /// 设置机器人标定点
        /// </summary>
        /// <param name="index">点位索引</param>
        /// <param name="qx">机器人X</param>
        /// <param name="qy">机器人Y</param>
        public void SetRobotCalibrationPoint(int index, double qx, double qy)
        {
            try
            {
                if (index < BaseCalibreationViewModel.CalibPointList.Count)
                {
                    //覆盖
                    BaseCalibreationViewModel.Cover(index, BaseCalibreationViewModel.CalibPointList[index].Px, BaseCalibreationViewModel.CalibPointList[index].Py, qx, qy);
                }
                else if (index == BaseCalibreationViewModel.CalibPointList.Count)
                {
                    //追加
                    BaseCalibreationViewModel.Add(0, 0, qx, qy);
                }
                else
                {
                    //无效输入
                }
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
        }

        /// <summary>
        /// 获取图像位置
        /// </summary>
        /// <param name="index">位置在列表中的索引</param>
        public void GetImageLocation(int index)
        {
            try
            {
                if (index < 0)
                {
                    return;
                }

                if (IsSceneValid)
                {
                    string result;
                    RunStatus runStatus = Scene.Execute(1000, out result);

                    if (runStatus.Result == EResult.Accept)
                    {
                        VisionResult = result;

                        foreach (var item in Scene.VisionFrame.Outputs)
                        {
                            if (item.Value is Location[])
                            {
                                var locations = item.Value as Location[];
                                for (int i = 0; i < locations.Length; i++)
                                {
                                    SetImageCalibrationPoint(index++, locations[i].X, locations[i].Y);
                                }
                            }
                            else if (item.Value is Location)
                            {
                                var location = (Location)item.Value;
                                SetImageCalibrationPoint(index++, location.X, location.Y);
                            }
                        }
                    }
                    else
                    {
                        VisionResult = $"{runStatus.Result}: {runStatus.Message}";
                    }
                }
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }

        }

        /// <summary>
        /// 获取机器人位置
        /// </summary>
        /// <param name="index">位置在列表中的索引</param>
        public void GetRobotLocation(int index)
        {
            try
            {
                if (index < 0)
                {
                    return;
                }

                if (robotCommunication != null)
                {
                    double x, y, z;
                    robotCommunication.GetRobotLocation(out x, out y, out z);
                    SetRobotCalibrationPoint(index, x, y);

                    if (index < baseCalibreationViewModel.CalibPointList.Count - 1)
                    {
                        baseCalibreationViewModel.SelectedIndex = index + 1;
                    }
                    else
                    {
                        baseCalibreationViewModel.SelectedIndex = -1;
                    }
                    RobotPointIndex = index + 1;
                }
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
        }

        /// <summary>
        /// 连接机器人
        /// </summary>
        /// <param name="ip">机器人IP</param>
        /// <param name="port">机器人端口号</param>
        public void ConnectRobot(string ip, int port)
        {
            try
            {
                if (robotCommunication?.IsConnect == true)
                {
                    //断开连接
                    robotCommunication.Disconnect();

                    RobotConnectButtonContent = "连接";
                }
                else
                {
                    //从集合中获取实例
                    if ((robotCommunication == null) && (RobotAssembly != null))
                    {
                        //创建视觉框架实例
                        foreach (var item in RobotAssembly.ExportedTypes)
                        {
                            if (item.Name == "RobotComunication")
                            {
                                object obj = RobotAssembly.CreateInstance(item.FullName);

                                if (obj is IRobotCommunication)
                                {
                                    robotCommunication = obj as IRobotCommunication;
                                }
                            }
                        }
                    }

                    if (robotCommunication != null)
                    {
                        robotCommunication.Connect(ip, port);
                        RobotConnectButtonContent = "断开";
                    }
                }
                NotifyOfPropertyChange(() => IsRobotConnect);
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }

        }

        /// <summary>
        /// 加载标定文件
        /// </summary>
        /// <param name="file">标定文件路径</param>
        public void LoadCalibrationFile(string file)
        {
            try
            {
                if (string.IsNullOrEmpty(file) || (!File.Exists(file)))
                {
                    throw new ArgumentException("无效文件路径!");
                }

                BaseCalibreationViewModel.CalibParam = JsonSerialization.DeserializeObjectFromFile<CalibParam>(file);

            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
        }

        /// <summary>
        /// 保存配置文件
        /// </summary>
        /// <param name="file">标定文件路径</param>
        public void SaveCalibrationFile(string file)
        {
            try
            {
                if (string.IsNullOrEmpty(file))
                {
                    throw new ArgumentException("无效文件路径!");
                }
                if (!JsonSerialization.SerializeObjectToFile(BaseCalibreationViewModel.CalibParam, file))
                {
                    throw new Exception("保存文件失败!");
                }
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
        }

        /// <summary>
        /// 确认
        /// </summary>
        public void Accept()
        {
            OnCalibrationConfigurationCompleted(BaseCalibreationViewModel.CalibParam);
        }

        #endregion
    }
}
