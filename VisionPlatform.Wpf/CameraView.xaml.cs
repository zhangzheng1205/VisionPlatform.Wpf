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

namespace VisionPlatform.Wpf
{
    /// <summary>
    /// CameraView.xaml 的交互逻辑
    /// </summary>
    public partial class CameraView : UserControl
    {
        /// <summary>
        /// 相机显示控件
        /// </summary>
        public CameraView()
        {
            InitializeComponent();

            var viewModel = new CameraViewModel();
            DataContext = viewModel;
            viewModel.MessageRaised += ViewModel_MessageRaised;
        }

        private void ViewModel_MessageRaised(object sender, MessageRaisedEventArgs e)
        {
            MessageBox.Show(e.Message);
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            //获取默认路径
            var viewModel = CameraConfigView.DataContext as CameraConfigViewModel;
            string cameraSerial = viewModel?.Camera?.Info?.SerialNumber;
            var directoryInfo = new DirectoryInfo("./");

            if (!string.IsNullOrEmpty(cameraSerial))
            {
                string defaultPath = $"VisionPlatform/Camera/CameraConfig/{cameraSerial}/ConfigFile";

                directoryInfo = new DirectoryInfo(defaultPath);

                //假如目录不存在,则创建对应的目录
                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }
            }
            
           var ofd = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".json",
                Filter = "json file|*.json",
                InitialDirectory = directoryInfo.FullName,
            };

            if (ofd.ShowDialog() == true)
            {
                LoadTextBlock.Text = ofd.FileName;
                return;
            }
            else
            {
                LoadTextBlock.Text = "";
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var inputWindow = new InputWindow();
            inputWindow.Title = "配置文件名输入窗口";
            inputWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            inputWindow.Owner = Window.GetWindow(this);
            inputWindow.InputAccepted += InputWindow_InputAccepted;
            inputWindow.InputCanceled += InputWindow_InputCanceled;
            inputWindow.ShowDialog();

            //var ofd = new Microsoft.Win32.SaveFileDialog
            //{
            //    DefaultExt = ".json",
            //    Filter = "json file|*.json"
            //};
            //
            //if (ofd.ShowDialog() == true)
            //{
            //    SaveTextBlock.Text = ofd.FileName;
            //    return;
            //}
            //else
            //{
            //    SaveTextBlock.Text = "";
            //}
        }

        private void InputWindow_InputCanceled(object sender, EventArgs e)
        {
            SaveTextBlock.Text = "";
        }

        private void InputWindow_InputAccepted(object sender, InputAcceptedEventArgs e)
        {
            SaveTextBlock.Text = e.Input;
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("是否要确认退出?", "退出相机配置窗口", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                AcceptTextBlock.Text = "True";
            }
            else
            {
                AcceptTextBlock.Text = "False";
            }
        }
    }
}
