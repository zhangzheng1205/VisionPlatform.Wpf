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

            DataContext = new SceneViewModel();
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
                Filter = "DLL文件|*.dll|VPP文件|*.vpp|其他|*.*"
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
    }
}
