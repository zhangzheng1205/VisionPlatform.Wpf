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
using VisionPlatform.BaseType;
using VisionPlatform.Core;
using VisionPlatform.Wpf;

namespace VisionPlatformDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        public SceneManager SceneManager { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            //更新视觉框架集合
            VisionFrameFactory.UpdateAssembly();

            //更新相机框架集合
            CameraFactory.UpdateAssembly();

            //获取场景管理器实例(单例)
            SceneManager = SceneManager.GetInstance();

            //更新控件
            VisionFrameComboBox.Items.Clear();
            VisionFrameComboBox.ItemsSource = VisionFrameFactory.VisionFrameAssemblys.Keys;
            VisionFrameComboBox.SelectedIndex = 0;
            CameraSDKComboBox.Items.Clear();
            CameraSDKComboBox.ItemsSource = CameraFactory.CameraAssemblys.Keys;
            CameraSDKComboBox.SelectedIndex = 0;
        }


        private void ConfigFrameButton_Click(object sender, RoutedEventArgs e)
        {
            CameraFactory.DefaultCameraSdkType = (ECameraSdkType)CameraSDKComboBox.SelectedItem;
            VisionFrameFactory.DefaultVisionFrameType = (EVisionFrameType)VisionFrameComboBox.SelectedItem;
        }

        private void VisionFrameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                if ((EVisionFrameType)e.AddedItems[0] == EVisionFrameType.VisionPro)
                {
                    CameraSdkDockPanel.Visibility = Visibility.Hidden;
                }
                else
                {
                    CameraSdkDockPanel.Visibility = Visibility.Visible;
                }
            }

        }

        /// <summary>
        /// 恢复场景列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecoverScenesButton_Click(object sender, RoutedEventArgs e)
        {
            SceneManager.RecoverScenes();
        }

        /// <summary>
        /// 打开创建场景窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ConfigFrameButton_Click(null, null);

            if ((VisionFrameFactory.DefaultVisionFrameType != EVisionFrameType.VisionPro) && (CameraFactory.DefaultCameraSdkType == ECameraSdkType.VirtualCamera))
            {
                CameraFactory.AddCamera(@"C:\Users\Public\Documents\MVTec\HALCON-17.12-Progress\examples\images");
                CameraFactory.AddCamera(@"C:\Users\Public\Documents\MVTec\HALCON-17.12-Progress\examples\images\alpha1.png");
                CameraFactory.AddCamera(@"C:\Users\Public\Documents\MVTec\HALCON-17.12-Progress\examples\images\autobahn.png");
            }

            Window window = new Window();
            SceneView control = new SceneView
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            window.MinWidth = 800;
            window.MinHeight = 500;
            window.Width = 800;
            window.Height = 500;
            window.Content = control;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Owner = Window.GetWindow(this);
            window.Title = "场景配置窗口";
            window.WindowState = WindowState.Maximized;

            window.ShowDialog();
        }
    }
}
