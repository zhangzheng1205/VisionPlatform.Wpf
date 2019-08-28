using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VisionPlatform.Core;
using VisionPlatform.Robot;

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

            BaseCalibreationViewModel.MessageRaised += BaseCalibreationViewModel_MessageRaised;
            BaseCalibreationViewModel.CalibrationPointListChanged += BaseCalibreationViewModel_CalibrationPointListChanged;
            BaseCalibreationViewModel.CalibrationPointSelectionChanged += BaseCalibreationViewModel_CalibrationPointSelectionChanged;

            NotifyOfPropertyChange(() => BaseCalibreationViewModel);

            pointIndexs.Clear();
            for (int i = 0; i < BaseCalibreationViewModel.CalibPointList.Count + 1; i++)
            {
                pointIndexs.Add(i);
            }
            NotifyOfPropertyChange(() => PointIndexs);
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
            pointIndexs.Clear();
            for (int i = 0; i < e.CalibPointList.Count + 1; i++)
            {
                pointIndexs.Add(i);
            }
            NotifyOfPropertyChange(() => PointIndexs);

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

        #endregion

        #region 机器人相关

        private ObservableCollection<IRobotCommunication> robots;

        /// <summary>
        /// 机器人通信实例列表
        /// </summary>
        public ObservableCollection<IRobotCommunication> Robots
        {
            get
            {
                return robots;
            }
            set
            {
                robots = value;
                NotifyOfPropertyChange(() => Robots);
            }
        }


        private IRobotCommunication robot;

        /// <summary>
        /// 机器人通信实例
        /// </summary>
        public IRobotCommunication Robot
        {
            get
            {
                return robot;
            }
            set
            {
                robot = value;
                NotifyOfPropertyChange(() => Robot);
            }
        }

        #endregion

        #region 标定控件相关

        /// <summary>
        /// 基本标定控件数据模型
        /// </summary>
        public BaseCalibreationViewModel BaseCalibreationViewModel { get; } = new BaseCalibreationViewModel();

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

        #endregion

        #region 方法

        /// <summary>
        /// 更新场景列表
        /// </summary>
        public void UpdateScenes()
        {
            Scenes = new ObservableCollection<Scene>(sceneManager.Scenes.Values);
        }

        /// <summary>
        /// 获取图像位置
        /// </summary>
        /// <param name="index">位置在列表中的索引</param>
        public void GetImageLocation(int index)
        {
            if (IsSceneValid)
            {
                string result;
                Scene.Execute(1000, out result);
            }

        }
        #endregion
    }
}
