using Cognex.VisionPro.ToolBlock;
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
    /// VisionProConfigControl.xaml 的交互逻辑
    /// </summary>
    public partial class VisionProConfigControl : UserControl
    {
        public VisionProConfigControl()
        {
            InitializeComponent();

            CogToolBlockEditV2.FilenameChanged += CogToolBlockEditV2_FilenameChanged;

        }

        /// <summary>
        /// 加载新VPP文件事件
        /// </summary>
        public event EventHandler<NewVppFileLoadedEventArgs> NewVppFileLoaded;

        /// <summary>
        /// 设置工具编辑器内容
        /// </summary>
        /// <param name="subject"></param>
        public void SetCogToolBlockEditSubject(CogToolBlock subject)
        {
            CogToolBlockEditV2.FilenameChanged -= CogToolBlockEditV2_FilenameChanged;
            CogToolBlockEditV2.Subject = subject;
            CogToolBlockEditV2.FilenameChanged += CogToolBlockEditV2_FilenameChanged;

        }

        private void CogToolBlockEditV2_FilenameChanged(object sender, EventArgs e)
        {
            NewVppFileLoaded?.Invoke(this, new NewVppFileLoadedEventArgs(CogToolBlockEditV2.Subject, CogToolBlockEditV2.Filename));

        }

        
    }

    public class NewVppFileLoadedEventArgs : EventArgs
    {
        /// <summary>
        /// VPP ToolBlock实例
        /// </summary>
        public CogToolBlock CogToolBlock { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 创建LoadedNewVppFileEventArgs新实例
        /// </summary>
        /// <param name="cogToolBlock">VPP ToolBlock实例</param>
        /// <param name="fileName">文件名</param>
        public NewVppFileLoadedEventArgs(CogToolBlock cogToolBlock, string fileName)
        {
            CogToolBlock = cogToolBlock;
            FileName = fileName;

        }


    }
}
