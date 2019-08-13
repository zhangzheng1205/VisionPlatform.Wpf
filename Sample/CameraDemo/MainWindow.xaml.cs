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
            if (CameraFactory.CameraAssemblys.ContainsKey("Hik"))
            {
                CameraFactory.CameraAssemblyName = "Hik";
            }

            //打开所有的相机
            CameraFactory.AddAllCamera();

            CameraView.DataContext = new CameraViewModel();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CameraFactory.RemoveAllCameras();
        }
    }
}
