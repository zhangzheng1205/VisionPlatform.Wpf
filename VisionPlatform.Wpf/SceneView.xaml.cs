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
    /// SceneView.xaml 的交互逻辑
    /// </summary>
    public partial class SceneView : UserControl
    {
        /// <summary>
        /// 创建SceneView新实例
        /// </summary>
        public SceneView()
        {
            InitializeComponent();

            var viewModel = new SceneViewModel();
            DataContext = viewModel;
            viewModel.MessageRaised += ViewModel_MessageRaised;
        }

        /// <summary>
        /// 创建SceneView新实例
        /// </summary>
        /// <param name="scene">场景实例</param>
        public SceneView(Scene scene)
        {
            InitializeComponent();

            var viewModel = new SceneViewModel(scene);
            DataContext = viewModel;
            viewModel.MessageRaised += ViewModel_MessageRaised;
        }

        /// <summary>
        /// 创建SceneView新实例
        /// </summary>
        /// <param name="scene">场景实例</param>
        /// <param name="isSceneNameReadOnly">场景名只读标志</param>
        public SceneView(Scene scene, bool isSceneNameReadOnly) : this(scene)
        {
            var viewModel = DataContext as SceneViewModel;
            viewModel.IsSceneNameReadOnly = isSceneNameReadOnly;
        }

        private void ViewModel_MessageRaised(object sender, MessageRaisedEventArgs e)
        {
            MessageBox.Show(e.Message);
        }

        /// <summary>
        /// 选择视觉算子文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectVisionOperaFileButton_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".dll",
                Filter = "常用格式|*.dll;*.vpp|其他|*.*"
            };

            if (ofd.ShowDialog() == true)
            {
                VisionOperaFileTextBlock.Text = ofd.FileName;
                return;
            }
            else
            {
                VisionOperaFileTextBlock.Text = "";
            }

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
