using System.Windows;
using VisionPlatform.Core;
using VisionPlatform.Wpf;

namespace CameraDemo
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //设置相机平台
            if (CameraFactory.CameraAssemblys.ContainsKey(VisionPlatform.BaseType.ECameraSDK.Pylon))
            {
                CameraFactory.ECameraSDK = VisionPlatform.BaseType.ECameraSDK.Pylon;
            }

            //打开所有的相机
            //CameraFactory.AddCamera(@"C:\Users\Public\Documents\MVTec\HALCON-17.12-Progress\examples\images");
            CameraFactory.AddAllCamera();

            CameraView.DataContext = new CameraViewModel();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CameraFactory.RemoveAllCameras();
        }
    }
}
