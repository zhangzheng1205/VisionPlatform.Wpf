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
    /// AdvanceCalibrationView.xaml 的交互逻辑
    /// </summary>
    public partial class AdvanceCalibrationView : UserControl
    {
        /// <summary>
        /// 创建AdvanceCalibrationView新实例
        /// </summary>
        public AdvanceCalibrationView()
        {
            InitializeComponent();
            var viewModel = new AdvanceCalibrationViewModel();
            DataContext = viewModel;
            viewModel.MessageRaised += ViewModel_MessageRaised;
        }

        private void ViewModel_MessageRaised(object sender, MessageRaisedEventArgs e)
        {
            MessageBox.Show(e.Message);
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ofd = new Microsoft.Win32.OpenFileDialog();

                ofd.DefaultExt = ".json";
                ofd.Filter = "json file|*.json";

                if (ofd.ShowDialog() == true)
                {
                    CalibrationFileTextBlock.Text = ofd.FileName;
                }
            }
            catch (Exception)
            {

            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //创建一个保存文件式的对话框  
            var sfd = new Microsoft.Win32.SaveFileDialog();

            //设置保存的文件的类型，注意过滤器的语法  
            sfd.Filter = "json file|*.json";
            sfd.FileName = "";

            //调用ShowDialog()方法显示该对话框，该方法的返回值代表用户是否点击了确定按钮  
            if (sfd.ShowDialog() == true)
            {
                CalibrationFileTextBlock.Text = sfd.FileName;
            }
        }
    }
}
