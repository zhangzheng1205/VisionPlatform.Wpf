using Framework.Camera;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VisionPlatform.BaseType
{
    /// <summary>
    /// 视觉框架接口
    /// </summary>
    public interface IVisionFrame : IDisposable
    {
        #region 属性

        /// <summary>
        /// 视觉框架
        /// </summary>
        EVisionFrame EVisionFrame { get; }

        /// <summary>
        /// 视觉算子接口
        /// </summary>
        IVisionOpera VisionOpera { get; set; }

        /// <summary>
        /// 视觉算子文件类型
        /// </summary>
        EVisionOperaFileType VisionOperaFileType { get; }

        /// <summary>
        /// 输出参数
        /// </summary>
        ItemCollection Inputs { get; }

        /// <summary>
        /// 输出参数
        /// </summary>
        ItemCollection Outputs { get; }

        /// <summary>
        /// 初始化标志
        /// </summary>
        bool IsInit { get; }

        /// <summary>
        /// 运行状态
        /// </summary>
        RunStatus RunStatus { get; set; }

        #region 功能使能

        /// <summary>
        /// 使能相机
        /// </summary>
        bool IsEnableCamera { get; }

        /// <summary>
        /// 使能视觉算子
        /// </summary>
        bool IsEnableVisionOpera { get; }

        /// <summary>
        /// 使能输入
        /// </summary>
        bool IsEnableInput { get; }

        /// <summary>
        /// 使能输出
        /// </summary>
        bool IsEnableOutput { get; }

        #endregion

        #region 控件

        /// <summary>
        /// 运行窗口
        /// </summary>
        UserControl RunningWindow { get; }

        /// <summary>
        /// 配置窗口
        /// </summary>
        UserControl ConfigWindow { get; }

        #endregion

        #endregion

        #region 方法

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="filePath">文件路径</param>
        void Init(string filePath);

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="timeout">处理超时时间,若小于等于0,则无限等待.单位:毫秒</param>
        /// <param name="outputs">输出结果</param>
        void Execute(int timeout, out ItemCollection outputs);

        /// <summary>
        /// 通过图像信息执行
        /// </summary>
        /// <param name="imageInfo">相机信息</param>
        /// <param name="outputs">输出结果</param>
        void ExecuteByImageInfo(ImageInfo imageInfo, out ItemCollection outputs);

        /// <summary>
        /// 通过本地图片执行
        /// </summary>
        /// <param name="file">本地图片路径</param>
        /// <param name="outputs">输出参数</param>
        void ExecuteByFile(string file, out ItemCollection outputs);

        #endregion

    }
}
