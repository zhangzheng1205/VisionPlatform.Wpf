using Cognex.VisionPro;
using System.Windows.Controls;

namespace VisionProVpp
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
