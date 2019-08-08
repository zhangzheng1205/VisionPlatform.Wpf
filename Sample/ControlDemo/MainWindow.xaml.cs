using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisionPlatform.ViewModels;
using VisionPlatform.Views;

namespace ControlDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 显示标定窗口
        /// </summary>
        /// <param name="fileName"></param>
        private void ShowCalibrationWindow(string fileName)
        {
            //打开标定窗口
            Window window = new Window();
            CalibrationView control = new CalibrationView
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            window.MinWidth = control.MinWidth + 50;
            window.MinHeight = control.MinHeight + 50;
            window.Width = control.MinWidth + 50;
            window.Height = control.MinHeight + 50;
            window.Content = control;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Owner = Window.GetWindow(this);
            window.Title = "标定窗口";

            control.DataContext = new CalibrationViewModel();
            //if (File.Exists(fileName))
            //{
            //    control.LoadFile(fileName);
            //}
            
            //(control.DataContext as CalibrationViewModel).CreateCalibFileEventHandler += delegate (object sender, CreateCalibFileEventArgs e)
            //{
            //    window.Close();
            //};
            //
            //(control.DataContext as CalibrationViewModel).CancelEventHandler += delegate (object sender, EventArgs e)
            //{
            //    window.Close();
            //};

            window.ShowDialog();

        }

        /// <summary>
        /// 显示相机选择窗口
        /// </summary>
        /// <param name="fileName"></param>
        private void ShowCameraSelectWindow()
        {
            ////打开标定窗口
            //Window window = new EmptyMetroWindow();
            //CameraSelectView control = new CameraSelectView
            //{
            //    HorizontalAlignment = HorizontalAlignment.Stretch,
            //    VerticalAlignment = VerticalAlignment.Stretch,
            //    DataContext = new CameraSelectViewModel(new Camera())
            //};
            //window.MinWidth = control.MinWidth + 50;
            //window.MinHeight = control.MinHeight + 50;
            //window.Width = control.MinWidth + 50;
            //window.Height = control.MinHeight + 50;
            //window.Content = control;
            //window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //window.Owner = Window.GetWindow(this);
            //window.Title = "相机选择窗口";

            //(control.DataContext as CameraSelectViewModel).SelectedCameraEventHandler += delegate (object sender, SelectedCameraEventArgs e)
            //{
            //    window.Close();
            //};

            //(control.DataContext as CameraSelectViewModel).CancelEventHandler += delegate (object sender, EventArgs e)
            //{
            //    window.Close();
            //};

            //window.ShowDialog();

        }

        /// <summary>
        /// 显示相机配置窗口
        /// </summary>
        /// <param name="fileName"></param>
        private void ShowCameraConfigWindow()
        {
            ////打开标定窗口
            //Window window = new EmptyMetroWindow();
            //CameraConfigView control = new CameraConfigView()
            //{
            //    HorizontalAlignment = HorizontalAlignment.Stretch,
            //    VerticalAlignment = VerticalAlignment.Stretch,
            //    DataContext = new CameraConfigViewModel(new Camera())
            //};
            //window.MinWidth = control.MinWidth + 50;
            //window.MinHeight = control.MinHeight + 50;
            //window.Width = control.MinWidth + 50;
            //window.Height = control.MinHeight + 50;
            //window.Content = control;
            //window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //window.Owner = Window.GetWindow(this);
            //window.Title = "相机选择窗口";

            //window.ShowDialog();

        }

        /// <summary>
        /// 显示参数配置窗口
        /// </summary>
        /// <param name="fileName"></param>
        private void ShowParamConfigWindow()
        {
            ////打开标定窗口
            //Window window = new EmptyMetroWindow();
            //ParamConfigView control = new ParamConfigView()
            //{
            //    HorizontalAlignment = HorizontalAlignment.Stretch,
            //    VerticalAlignment = VerticalAlignment.Stretch,
            //    DataContext = new ParamConfigViewModel()
            //};
            //window.MinWidth = control.MinWidth + 50;
            //window.MinHeight = control.MinHeight + 50;
            //window.Width = control.MinWidth + 50;
            //window.Height = control.MinHeight + 50;
            //window.Content = control;
            //window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //window.Owner = Window.GetWindow(this);
            //window.Title = "参数配置窗口";

            //List<ItemBase> paramList = new List<ItemBase>
            //{
            //    new ItemBase("1", 1.1, "描述1")
            //    {
            //        IsAvailable=false,
            //        IsWritable=false
            //    },
            //    new ItemBase("2", true, "描述2"),
            //    new ItemBase("3", "字符串数值", "这是一段很长很长很长很长很长很长很长很长很长很长很长很长的描述"),
            //    new ItemBase("2", 123, "描述4")
            //};

            //(control.DataContext as ParamConfigViewModel).ConfigParamList = paramList;

            //(control.DataContext as ParamConfigViewModel).CompletedConfigurationEventHandler += delegate (object sender, CompletedConfigurationEventArgs e)
            //{
            //    window.Close();
            //};

            //(control.DataContext as ParamConfigViewModel).CancelEventHandler += delegate (object sender, EventArgs e)
            //{
            //    window.Close();
            //};

            //window.ShowDialog();
        }

        /// <summary>
        /// 显示场景配置窗口
        /// </summary>
        /// <param name="fileName"></param>
        private void ShowSceneConfigWindow()
        {
            ////打开标定窗口
            //Window window = new EmptyMetroWindow();
            //SceneConfigView control = new SceneConfigView()
            //{
            //    HorizontalAlignment = HorizontalAlignment.Stretch,
            //    VerticalAlignment = VerticalAlignment.Stretch,
            //    DataContext = new SceneConfigViewModel("DemoScene")
            //};
            //control.HorizontalAlignment = HorizontalAlignment.Stretch;
            //control.VerticalAlignment = VerticalAlignment.Stretch;
            //window.MinWidth = control.MinWidth + 50;
            //window.MinHeight = control.MinHeight + 50;
            //window.Width = control.MinWidth + 50;
            //window.Height = control.MinHeight + 50;
            //window.Content = control;
            //window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //window.Owner = Window.GetWindow(this);
            //window.Title = "参数配置窗口";

            //window.ShowDialog();
        }

        /// <summary>
        /// 显示场景配置窗口
        /// </summary>
        /// <param name="fileName"></param>
        private void ShowSceneManageWindow()
        {
            ////打开标定窗口
            //Window window = new EmptyMetroWindow();
            //SceneManageView control = new SceneManageView()
            //{
            //    HorizontalAlignment = HorizontalAlignment.Stretch,
            //    VerticalAlignment = VerticalAlignment.Stretch,
            //    DataContext = new SceneManageViewModel()
            //};
            //control.HorizontalAlignment = HorizontalAlignment.Stretch;
            //control.VerticalAlignment = VerticalAlignment.Stretch;
            //window.MinWidth = control.MinWidth + 50;
            //window.MinHeight = control.MinHeight + 50;
            //window.Width = control.MinWidth + 50;
            //window.Height = control.MinHeight + 50;
            //window.Content = control;
            //window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //window.Owner = Window.GetWindow(this);
            //window.Title = "参数配置窗口";

            //window.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //打开标定窗口
            ShowCalibrationWindow(null);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ShowCameraSelectWindow();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ShowCameraConfigWindow();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ShowParamConfigWindow();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            ShowSceneConfigWindow();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            ShowSceneManageWindow();
        }
    }
}
