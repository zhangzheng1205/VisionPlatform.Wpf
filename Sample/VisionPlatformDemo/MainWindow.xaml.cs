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
            ConfigFrameButton_Click(null, null);

            CameraFactory.AddCamera(@"C:\Users\Public\Documents\MVTec\HALCON-17.12-Progress\examples\images");
            CameraFactory.AddCamera(@"C:\Users\Public\Documents\MVTec\HALCON-17.12-Progress\examples\images\alpha1.png");
            CameraFactory.AddCamera(@"C:\Users\Public\Documents\MVTec\HALCON-17.12-Progress\examples\images\autobahn.png");

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

            //(control.DataContext as SceneViewModel).CreateScene();

            window.ShowDialog();
        }

        private void ConfigFrameButton_Click(object sender, RoutedEventArgs e)
        {
            switch ((CameraSDKComboBox.SelectedItem as ComboBoxItem).Content)
            {
                case "Pylon":
                    CameraFactory.DefaultCameraSdkType = ECameraSdkType.Pylon;
                    break;
                case "VirtualCamera":
                    CameraFactory.DefaultCameraSdkType = ECameraSdkType.VirtualCamera;
                    break;
                default:
                    CameraFactory.DefaultCameraSdkType = ECameraSdkType.Unknown;
                    break;
            }

            switch ((VisionFrameComboBox.SelectedItem as ComboBoxItem).Content)
            {
                case "Halcon":
                    VisionFrameFactory.DefaultVisionFrameType = EVisionFrameType.Halcon;
                    break;
                case "VisionPro":
                    VisionFrameFactory.DefaultVisionFrameType = EVisionFrameType.VisionPro;
                    break;
                default:
                    break;
            }

        }
    }
}
