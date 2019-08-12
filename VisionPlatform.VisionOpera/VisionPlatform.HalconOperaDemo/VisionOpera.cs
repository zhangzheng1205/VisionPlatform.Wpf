using HalconDotNet;
using System;
using System.Diagnostics;
using System.Windows.Controls;
using VisionPlatform.BaseType;
using ItemCollection = VisionPlatform.BaseType.ItemCollection;

namespace VisionPlatform.HalconOperaDemo
{
    public class VisionOpera : IVisionOpera
    {
        #region 构造函数

        /// <summary>
        /// 创建VisionOpera新实例
        /// </summary>
        public VisionOpera()
        {
            RunningWindow = new HSmartWindowControlWPF();
            ConfigWindow = new ConfigWindow();

            Inputs = new ItemCollection
            {
                new ItemBase("Custom", false, "自定义参数")
            };

            Outputs = new ItemCollection
            {
                new ItemBase("ImageWidth", typeof(int), "图像宽度"),
                new ItemBase("ImageHeight", typeof(int), "图像高度"),
                new ItemBase("ImageType", typeof(string), "图像类型"),
                new ItemBase("Custom", typeof(bool), "自定义参数"),
                new ItemBase("Random", typeof(int), "随机数,范围为0-100")
            };

        }

        #endregion

        #region 字段

        /// <summary>
        /// 运行窗口句柄
        /// </summary>
        private IntPtr HRunningWindowHande
        {
            get
            {
                var hSmartWindow = RunningWindow as HSmartWindowControlWPF;
                if (hSmartWindow?.IsLoaded == true)
                {
                    return hSmartWindow.HalconID;
                }

                return IntPtr.Zero;
            }
        }

        /// <summary>
        /// 配置窗口句柄
        /// </summary>
        private IntPtr HConfigWindowHande
        {
            get
            {
                var hSmartWindow = (ConfigWindow as ConfigWindow)?.HSmartWindowControlWPF;
                if (hSmartWindow?.IsLoaded == true)
                {
                    return hSmartWindow.HalconID;
                }

                return IntPtr.Zero;
            }
        }

        private bool isRunningWindowInit = false;

        private bool isConfigWindowInit = false;

        /// <summary>
        /// 图像句柄
        /// </summary>
        private HObject hImage;

        /// <summary>
        /// 计时器
        /// </summary>
        private readonly Stopwatch stopwatch = new Stopwatch();

        #endregion

        #region 属性

        /// <summary>
        /// 输入图像
        /// </summary>
        public object InputImage
        {
            get
            {
                return hImage;
            }
            set
            {
                hImage = value as HObject;
            }
        }

        /// <summary>
        /// 输出参数
        /// </summary>
        public ItemCollection Inputs { get; }

        /// <summary>
        /// 输出参数
        /// </summary>
        public ItemCollection Outputs { get; }

        /// <summary>
        /// 运行状态
        /// </summary>
        public RunStatus RunStatus { get; private set; }

        /// <summary>
        /// 运行窗口
        /// </summary>
        public object RunningWindow { get; private set; }

        /// <summary>
        /// 配置窗口
        /// </summary>
        public object ConfigWindow { get; private set; }

        #endregion

        #region 方法

        /// <summary>
        /// 设置halcon窗口布局
        /// </summary>
        /// <param name="hWindow">halcon窗口</param>
        /// <param name="imageWidth">图像宽度</param>
        /// <param name="imageHeiht">图像高度</param>
        private static void SetWindowPart(HWindow hWindow, int imageWidth, int imageHeiht)
        {
            int winRow, winCol, winWidth, winHeight, partWidth, partHeight;

            HOperatorSet.SetSystem("width", imageWidth);
            HOperatorSet.SetSystem("height", imageHeiht);

            if ((imageHeiht > 0) && (imageWidth > 0))
            {
                hWindow.GetWindowExtents(out winRow, out winCol, out winWidth, out winHeight);
                if (winWidth < winHeight)
                {
                    partWidth = imageWidth;
                    partHeight = imageWidth * winHeight / winWidth;

                    HOperatorSet.SetPart(hWindow, (imageHeiht - partHeight) / 2.0, 0, partHeight - 1 + ((imageHeiht - partHeight) / 2.0), partWidth - 1);
                }
                else
                {
                    partWidth = imageHeiht * winWidth / winHeight;
                    partHeight = imageHeiht;

                    HOperatorSet.SetPart(hWindow, 0, (imageWidth - partWidth) / 2.0, partHeight - 1, partWidth - 1 + ((imageWidth - partWidth) / 2.0));
                }
            }

        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="windowHandle">窗口句柄</param>
        public void Execute(object image, out ItemCollection outputs)
        {
            outputs = null;

            stopwatch.Restart();
            HObject thresholdImage = null;

            try
            {
                //初始化
                if ((!isRunningWindowInit) && (HRunningWindowHande != IntPtr.Zero))
                {
                    HOperatorSet.SetDraw(HRunningWindowHande, "margin");
                    HOperatorSet.SetLineWidth(HRunningWindowHande, 2);
                    HOperatorSet.SetColor(HRunningWindowHande, "red");

                    isRunningWindowInit = true;
                }

                //初始化
                if ((!isConfigWindowInit) && (HConfigWindowHande != IntPtr.Zero))
                {
                    HOperatorSet.SetDraw(HConfigWindowHande, "margin");
                    HOperatorSet.SetLineWidth(HConfigWindowHande, 2);
                    HOperatorSet.SetColor(HConfigWindowHande, "red");

                    isConfigWindowInit = true;
                }

                var random = new Random();

                HTuple width;
                HTuple height;
                HTuple type;

                hImage?.Dispose();
                hImage = image as HObject;

                HOperatorSet.GetImageSize(hImage, out width, out height);
                HOperatorSet.GetImageType(hImage, out type);
                HOperatorSet.Threshold(hImage, out thresholdImage, 0, 150);

                if (HRunningWindowHande != IntPtr.Zero)
                {
                    SetWindowPart((RunningWindow as HSmartWindowControlWPF)?.HalconWindow, width, height);
                    HOperatorSet.ClearWindow(HRunningWindowHande);
                    HOperatorSet.DispObj(hImage, HRunningWindowHande);
                }

                if (HConfigWindowHande != IntPtr.Zero)
                {
                    SetWindowPart((ConfigWindow as ConfigWindow)?.HSmartWindowControlWPF.HalconWindow, width, height);
                    HOperatorSet.ClearWindow(HConfigWindowHande);
                    HOperatorSet.DispObj(hImage, HConfigWindowHande);
                    HOperatorSet.DispObj(thresholdImage, HConfigWindowHande);
                }

                Outputs["ImageWidth"].Value = width.I;
                Outputs["ImageHeight"].Value = height.I;
                Outputs["ImageType"].Value = type.S;
                Outputs["Custom"].Value = Inputs["Custom"].Value;
                Outputs["Random"].Value = random.Next(0, 100);

                outputs = new ItemCollection(Outputs);

                stopwatch.Stop();
                RunStatus = new RunStatus(stopwatch.Elapsed.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                RunStatus = new RunStatus(stopwatch.Elapsed.TotalMilliseconds, EResult.Error, ex.Message, ex);
                throw;
            }
            finally
            {
                thresholdImage?.Dispose();
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                hImage?.Dispose();
                hImage = null;

                RunningWindow = null;
                ConfigWindow = null;

                Inputs.Clear();
                Outputs.Clear();
                RunStatus = null;

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~VisionOpera()
        // {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            //GC.SuppressFinalize(this);
        }
        #endregion

        #endregion

    }
}
