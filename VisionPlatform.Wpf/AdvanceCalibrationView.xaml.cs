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
            viewModel.MessageRaised += ViewModel_MessageRaised; ;
        }

        private void ViewModel_MessageRaised(object sender, MessageRaisedEventArgs e)
        {
            MessageBox.Show(e.Message);
        }
    }
}
