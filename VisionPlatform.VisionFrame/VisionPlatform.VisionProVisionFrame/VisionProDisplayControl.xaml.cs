using Cognex.VisionPro;
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

namespace VisionPlatform.VisionProVisionFrame
{
    /// <summary>
    /// VisionProDisplayControl.xaml 的交互逻辑
    /// </summary>
    public partial class VisionProDisplayControl : UserControl
    {
        public VisionProDisplayControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 设置显示控件工具
        /// </summary>
        /// <param name="cogTool"></param>
        public void SetCogToolDisplayTool(ICogTool cogTool)
        {
            CogToolDisplay.Tool = cogTool;
        }

    }
}
