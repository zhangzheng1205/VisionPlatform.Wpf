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
    /// InputWindow.xaml 的交互逻辑
    /// </summary>
    public partial class InputWindow : Window
    {
        /// <summary>
        /// 创建InputWindow新实例
        /// </summary>
        public InputWindow()
        {
            InitializeComponent();
        }

        #region 事件

        /// <summary>
        /// 输入确认
        /// </summary>
        internal event EventHandler<InputAcceptedEventArgs> InputAccepted;

        internal event EventHandler<EventArgs> InputCanceled;

        #endregion

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(InputTextBox.Text))
            {
                MessageBox.Show("输入无效", "输入无效");
                return;
            }
            
            InputAccepted?.Invoke(this, new InputAcceptedEventArgs(InputTextBox.Text));
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            InputCanceled?.Invoke(this, new EventArgs());
            Close();
        }
    }

    internal class InputAcceptedEventArgs : EventArgs
    {
        public InputAcceptedEventArgs(string input)
        {
            Input = input;
        }

        public string Input { get; set; }
    }
}
