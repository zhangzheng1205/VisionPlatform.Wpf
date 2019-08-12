using System;

namespace VisionPlatform.BaseType
{
    /// <summary>
    /// 视觉算子接口
    /// </summary>
    public interface IVisionOpera : IDisposable
    {
        #region 属性

        /// <summary>
        /// 输入图像
        /// </summary>
        object InputImage { get; set; }

        /// <summary>
        /// 输出参数
        /// </summary>
        ItemCollection Inputs { get; }

        /// <summary>
        /// 输出参数
        /// </summary>
        ItemCollection Outputs { get; }

        /// <summary>
        /// 运行状态
        /// </summary>
        RunStatus RunStatus { get; }

        /// <summary>
        /// 运行窗口
        /// </summary>
        object RunningWindow { get; }

        /// <summary>
        /// 配置窗口
        /// </summary>
        object ConfigWindow { get; }

        #endregion

        #region 方法

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="image">图像</param>
        /// <param name="outputs">输出结果</param>
        /// <returns>执行结果</returns>
        void Execute(object image, out ItemCollection outputs);

        #endregion
    }
}
