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
using System.Windows.Shapes;
using VisionPlatform.Wpf;

namespace VisionPlatformDemo
{
    /// <summary>
    /// SceneCreateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SceneCreateWindow : Window
    {
        public SceneCreateWindow()
        {
            InitializeComponent();
        }

        private void OpenCameraViewButton_Click(object sender, RoutedEventArgs e)
        {
            ConfigViewContentControl.Content = new CameraView
            {
                DataContext = new CameraViewModel(),
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ConfigViewContentControl.Content = new SceneParamDebugView
            {
                DataContext = new SceneParamDebugViewModel(),
            };
        }
    }
}
