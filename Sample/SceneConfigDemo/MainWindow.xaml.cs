using System;
using System.Collections.Generic;
using System.IO;
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

namespace SceneConfigDemo
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

        private Scene scene;
        private Scene scene2;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CameraFactory.CameraAssemblys.ContainsKey("VirtualCamera"))
            {
                CameraFactory.CameraAssemblyName = "VirtualCamera";
            }
            
            scene?.Dispose();
            
            //if (File.Exists("查找5边型.json"))
            //{
            //    scene = Scene.Deserialize("查找5边型.json");
            //}
            //else
            //{
            //    string dllFile = @"..\..\..\..\VisionPlatform.VisionOpera\VisionPlatform.HalconOperaDemo\bin\Debug\VisionPlatform.HalconOperaDemo.dll";
            //    string serial = @"C:\Users\Public\Documents\MVTec\HALCON-17.12-Progress\examples\images";
            //    //string serial = @"00575388468";
            //    scene = new Scene("查找5边型", EVisionFrame.Halcon, dllFile, serial);
            //}

            if (File.Exists("查找6边型.json"))
            {
                scene2 = Scene.Deserialize("查找6边型.json");
            }
            else
            {
                scene2 = new Scene("查找6边型", EVisionFrame.VisionPro, @"E:\0. 临时目录\TestVpp.vpp");
            }

            var viewModel = new SceneParamDebugViewModel();
            SceneParamDebugView.DataContext = viewModel;
            viewModel.Scene = scene2;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (scene != null)
            {
                Scene.Serialize(scene, $"{scene.Name}.json");
                scene.Dispose();
            }

            if (scene2 != null)
            {
                Scene.Serialize(scene2, $"{scene2.Name}.json");
                scene2.Dispose();
            }

            CameraFactory.RemoveAllCameras();

            Environment.Exit(0);

        }
    }
}
