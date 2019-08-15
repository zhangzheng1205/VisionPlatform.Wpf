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

            DataContext = new CameraViewModel();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".json",
                Filter = "json file|*.json"
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
            var ofd = new Microsoft.Win32.SaveFileDialog
            {
                DefaultExt = ".json",
                Filter = "json file|*.json"
            };

            if (ofd.ShowDialog() == true)
            {
                SaveTextBlock.Text = ofd.FileName;
                return;
            }
            else
            {
                SaveTextBlock.Text = "";
            }
        }

    }
}
