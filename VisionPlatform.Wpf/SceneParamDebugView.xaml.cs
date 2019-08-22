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

namespace VisionPlatform.Wpf
{
    /// <summary>
    /// SceneParamDebugView.xaml 的交互逻辑
    /// </summary>
    public partial class SceneParamDebugView : UserControl
    {
        /// <summary>
        /// 创建SceneParamDebugView新实例
        /// </summary>
        public SceneParamDebugView()
        {
            InitializeComponent();

            var viewModel = new SceneParamDebugViewModel();
            DataContext = viewModel;
            viewModel.MessageRaised += ViewModel_MessageRaised;
        }

        /// <summary>
        /// 创建SceneParamDebugView新实例
        /// </summary>
        /// <param name="scene"></param>
        public SceneParamDebugView(Scene scene)
        {
            InitializeComponent();

            var viewModel = new SceneParamDebugViewModel(scene);
            DataContext = viewModel;
            viewModel.MessageRaised += ViewModel_MessageRaised;
        }

        private void ViewModel_MessageRaised(object sender, MessageRaisedEventArgs e)
        {
            MessageBox.Show(e.Message);
        }

        private void ExecuteImageButton_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "图像文件|*.bmp;*.png;*.jpg;*.jpgtif;*.tiff;*.giff;*.bmpf;*.jpgf;*.jpegf;*.jp2f;*.pngf;*.pcxf;*.pgmf;*.ppmf;*.pbmf;*.xwdf;*.ima|其他|*.*"
            };

            if (ofd.ShowDialog() == true)
            {
                ImagePathTextBlock.Text = ofd.FileName;
                return;
            }
            else
            {
                ImagePathTextBlock.Text = "";
            }
        }
    }
}
