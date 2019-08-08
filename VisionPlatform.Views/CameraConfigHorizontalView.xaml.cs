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

namespace VisionPlatform.Views
{
    /// <summary>
    /// CameraConfigView.xaml 的交互逻辑
    /// </summary>
    public partial class CameraConfigHorizontalView : UserControl
    {
        public CameraConfigHorizontalView()
        {
            InitializeComponent();
        }

        private void WidthTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BindingExpression be = (sender as TextBox).GetBindingExpression(TextBox.TextProperty);
                be.UpdateSource();
            }
        }

        private void HeihtTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BindingExpression be = (sender as TextBox).GetBindingExpression(TextBox.TextProperty);
                be.UpdateSource();
            }

        }

        private void ExposureTimeTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BindingExpression be = (sender as TextBox).GetBindingExpression(TextBox.TextProperty);
                be.UpdateSource();
            }

        }

        private void GainTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BindingExpression be = (sender as TextBox).GetBindingExpression(TextBox.TextProperty);
                be.UpdateSource();
            }

        }
    }
}
