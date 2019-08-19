using System;
using System.Collections.Generic;
using System.Globalization;
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
            DataContext = viewModel;
            viewModel.MessageRaised += ViewModel_MessageRaised; ;
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

        /// <summary>
        /// 加载文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadFileButton_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                var ofd = new Microsoft.Win32.OpenFileDialog();

                ofd.DefaultExt = ".json";
                ofd.Filter = "json file|*.json";

                if (ofd.ShowDialog() == true)
                {
                    CalibFileTextBox.Text = ofd.FileName;
                    //Message.SetAttach(sender as Button, "Add");
                }
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateFileButton_Click(object sender, RoutedEventArgs e)
        {
            //创建一个保存文件式的对话框  
            var sfd = new Microsoft.Win32.SaveFileDialog();

            //设置保存的文件的类型，注意过滤器的语法  
            sfd.Filter = "json file|*.json";
            sfd.FileName = "";

            //调用ShowDialog()方法显示该对话框，该方法的返回值代表用户是否点击了确定按钮  
            if (sfd.ShowDialog() == true)
            {
                CalibFileTextBox.Text = sfd.FileName;
            }
        }
    }

    /// <summary>
    /// 标定矩阵转字符串
    /// </summary>
    internal class MatrixConverter : IValueConverter
    {

        /// <summary>
        /// 获取标定矩阵字符串
        /// </summary>
        /// <param name="matrix">矩阵</param>
        private string GetDisplayMatrix(double[] matrix)
        {
            try
            {
                if ((matrix != null) && (matrix.Length > 0))
                {
                    //显示标定结果
                    string MatrixString = "";

                    for (int i = 0; i < matrix.Length - 1; i++)
                    {
                        MatrixString += matrix[i].ToString("F4") + ",";
                    }

                    MatrixString += matrix[matrix.Length - 1].ToString("F4");

                    return MatrixString;
                }
                else
                {

                }
            }
            catch (Exception)
            {

            }
            return "NaN,NaN,NaN,NaN,NaN,NaN";
        }

        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return GetDisplayMatrix(value as double[]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
