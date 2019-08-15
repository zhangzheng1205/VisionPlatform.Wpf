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

namespace VisionPlatform.Wpf
{
    /// <summary>
    /// SceneParamDebugView.xaml 的交互逻辑
    /// </summary>
    public partial class SceneParamDebugView : UserControl
    {
        /// <summary>
        /// 场景参数调试控件
        /// </summary>
        public SceneParamDebugView()
        {
            InitializeComponent();

            DataContext = new SceneParamDebugViewModel();
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
