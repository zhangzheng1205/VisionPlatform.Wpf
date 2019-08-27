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
    /// BaseCalibreationView.xaml 的交互逻辑
    /// </summary>
    public partial class BaseCalibreationView : UserControl
    {
        /// <summary>
        /// 创建BaseCalibreationView新实例
        /// </summary>
        public BaseCalibreationView()
        {
            InitializeComponent();

            var viewModel = new BaseCalibreationViewModel();
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
        
        private void CalibPointListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            if (e.AddedItems.Count == 0)
            {
                if (listView.Items.Count > 0)
                {
                    listView.SelectedIndex = listView.Items.Count - 1;
                }
            }
            else
            {
                if (e.AddedItems[0] is Framework.Vision.CalibPointData)
                {
                    var calibPointData = (e.AddedItems[0] as Framework.Vision.CalibPointData);
                    PxTextBox.Text = calibPointData.Px.ToString("0.###");
                    PyTextBox.Text = calibPointData.Py.ToString("0.###");
                    QxTextBox.Text = calibPointData.Qx.ToString("0.###");
                    QyTextBox.Text = calibPointData.Qy.ToString("0.###");

                }
            }
            
        }
    }
}
