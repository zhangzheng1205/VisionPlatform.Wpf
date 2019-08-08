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

namespace VisionPlatform.Views
{
    /// <summary>
    /// SceneConfigView.xaml 的交互逻辑
    /// </summary>
    public partial class SceneConfigView : UserControl
    {
        public SceneConfigView()
        {
            InitializeComponent();
        }

        private void SelectVisionOperaDllButton_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new Microsoft.Win32.OpenFileDialog();

            ofd.DefaultExt = ".dll";
            ofd.Filter = "DLL文件|*.dll";

            if (ofd.ShowDialog() == true)
            {
                //此处做你想做的事 ...=ofd.SafeFileName; 
                VisionOperaDllPathTextBlock.Text = ofd.FileName;
            }
        }
    }

    /// <summary>
    /// 数值转换器
    /// </summary>
    internal class ItemCountConverter : IValueConverter
    {
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
            if (value is int)
            {
                return (int)value > 1;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
