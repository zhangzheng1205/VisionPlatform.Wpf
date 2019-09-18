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
    /// CalibrationView.xaml 的交互逻辑
    /// </summary>
    public partial class CalibrationView : UserControl
    {
        /// <summary>
        /// 标定控件
        /// </summary>
        public CalibrationView()
        {
            InitializeComponent();

            var viewModel = new CalibrationViewModel();
            viewModel.MessageRaised += ViewModel_MessageRaised;
            DataContext = viewModel;
        }

        private void ViewModel_MessageRaised(object sender, MessageRaisedEventArgs e)
        {
            MessageBox.Show(e.Message);
        }

        #region 输入栏点击全选功能实现

        private void PxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.PreviewMouseDown += new MouseButtonEventHandler(PxTextBox_PreviewMouseDown);
        }

        private void PxTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.Focus();
            e.Handled = true;
        }

        private void PxTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.SelectAll();
            textBox.PreviewMouseDown -= new MouseButtonEventHandler(PxTextBox_PreviewMouseDown);
        }

        private void QxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.PreviewMouseDown += new MouseButtonEventHandler(QxTextBox_PreviewMouseDown);
        }

        private void QxTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.Focus();
            e.Handled = true;
        }

        private void QxTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.SelectAll();
            textBox.PreviewMouseDown -= new MouseButtonEventHandler(QxTextBox_PreviewMouseDown);
        }

        private void PyTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.PreviewMouseDown += new MouseButtonEventHandler(PyTextBox_PreviewMouseDown);
        }

        private void PyTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.Focus();
            e.Handled = true;
        }

        private void PyTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.SelectAll();
            textBox.PreviewMouseDown -= new MouseButtonEventHandler(PyTextBox_PreviewMouseDown);
        }

        private void QyTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.PreviewMouseDown += new MouseButtonEventHandler(QyTextBox_PreviewMouseDown);
        }

        private void QyTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.Focus();
            e.Handled = true;
        }

        private void QyTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.SelectAll();
            textBox.PreviewMouseDown -= new MouseButtonEventHandler(QyTextBox_PreviewMouseDown);
        }

        #endregion

    }
}
