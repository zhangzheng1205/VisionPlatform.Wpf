using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using VisionPlatform.Views;

namespace VisionPlatform.ViewModels
{
    public class SceneManageViewModel : Screen
    {
        #region 构造函数

        /// <summary>
        /// 创建SceneManageViewModel新实例
        /// </summary>
        public SceneManageViewModel()
        {

        }

        #endregion

        #region 属性

        /// <summary>
        /// 相机列表(序列号)
        /// </summary>
        private ObservableCollection<string> cameraList;

        /// <summary>
        /// 相机列表
        /// </summary>
        public ObservableCollection<string> CameraList
        {
            get
            {
                return cameraList;
            }
            set
            {
                cameraList = value;
                NotifyOfPropertyChange(() => CameraList);
            }
        }

        /// <summary>
        /// 场景列表(场景名)
        /// </summary>
        private ObservableCollection<string> sceneList;

        /// <summary>
        /// 场景列表(场景名)
        /// </summary>
        public ObservableCollection<string> SceneListList
        {
            get
            {
                return sceneList;
            }
            set
            {
                sceneList = value;
                NotifyOfPropertyChange(() => SceneListList);
            }
        }

        #endregion

        #region 方法


        /// <summary>
        /// 显示相机选择窗口
        /// </summary>
        /// <param name="fileName"></param>
        private void ShowCameraSelectWindow()
        {
            //打开标定窗口
            Window window = new EmptyMetroWindow();
            CameraSelectView control = new CameraSelectView
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                //DataContext = new CameraSelectViewModel(new Camera())
            };
            window.MinWidth = control.MinWidth + 50;
            window.MinHeight = control.MinHeight + 50;
            window.Width = control.MinWidth + 50;
            window.Height = control.MinHeight + 50;
            window.Content = control;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //window.Owner = Window.GetWindow(this);
            window.Title = "相机选择窗口";

            (control.DataContext as CameraSelectViewModel).SelectedCameraEventHandler += delegate (object sender, SelectedCameraEventArgs e)
            {
                window.Close();
            };

            (control.DataContext as CameraSelectViewModel).CancelEventHandler += delegate (object sender, EventArgs e)
            {
                window.Close();
            };

            window.ShowDialog();

        }

        #endregion
    }
}
