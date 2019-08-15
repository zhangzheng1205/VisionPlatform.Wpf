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


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window();
            SceneView control = new SceneView
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
            window.WindowState = WindowState.Maximized;
            window.ShowDialog();
        }

        private void ConfigFrameButton_Click(object sender, RoutedEventArgs e)
        {
            string cameraSDK = CameraSDKComboBox.SelectedItem as string;
            ECameraSdkType eCameraSDK;
            switch (CameraSDKComboBox.SelectedItem)
            {
                case "Pylon":
                    eCameraSDK = ECameraSdkType.Pylon;
                    break;
                case "VirtualCamera":
                    eCameraSDK = ECameraSdkType.VirtualCamera;
                    break;
                default:
                    eCameraSDK = ECameraSdkType.Unknown;
                    break;
            }

            CameraFactory.DefaultCameraSdkType = eCameraSDK;
        }
    }
}
