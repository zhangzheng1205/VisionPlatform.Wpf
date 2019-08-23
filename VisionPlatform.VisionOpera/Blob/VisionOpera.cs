using HalconDotNet;
using System;
using System.Diagnostics;
using System.Windows;
using VisionPlatform.BaseType;

namespace Blob
{

    public class VisionOpera : IVisionOpera
    {
        #region 构造函数

        public VisionOpera()
        {
            //创建运行时/配置窗口控件
            var runningSmartWindow = new HSmartWindowControlWPF();
            runningSmartWindow.HInitWindow += RunningSmartWindow_HInitWindow;
            runningSmartWindow.Unloaded += RunningSmartWindow_Unloaded;
            RunningWindow = runningSmartWindow;

            var configSmartWindow = new HSmartWindowControlWPF();
            configSmartWindow.HInitWindow += ConfigSmartWindow_HInitWindow;
            configSmartWindow.Unloaded += ConfigSmartWindow_Unloaded;
            ConfigWindow = configSmartWindow;

            //配置输入参数
            Inputs = new ItemCollection()
            {
                new ItemBase("MinGray", (int)0, "最小灰度(阈值分割)"),
                new ItemBase("MaxGray", (int)120, "最大灰度(阈值分割)"),
                new ItemBase("MorphologicalKernalWidth", (int)20, "形态学运算核宽度"),
                new ItemBase("MorphologicalKernalHeight", (int)50, "形态学运算核高度"),
                new ItemBase("ErosionRectangleWidth", (int)10, "腐蚀矩形宽度"),
                new ItemBase("ErosionRectangleHeight", (int)90, "腐蚀矩形高度"),
            };

            //配置输出参数
            Outputs = new ItemCollection()
            {
                new ItemBase("X", typeof(double[]), "X坐标"),
                new ItemBase("Y", typeof(double[]), "Y坐标"),
            };
        }

        private void RunningSmartWindow_HInitWindow(object sender, EventArgs e)
        {
            runningWindow = (sender as HSmartWindowControlWPF).HalconWindow;
            HOperatorSet.SetColor(runningWindow, "lime green");
            HOperatorSet.SetLineWidth(runningWindow, 3);
            HOperatorSet.SetDraw(runningWindow, "margin");
        }

        private void ConfigSmartWindow_HInitWindow(object sender, EventArgs e)
        {
            configWindow = (sender as HSmartWindowControlWPF).HalconWindow;
            HOperatorSet.SetColor(configWindow, "lime green");
            HOperatorSet.SetLineWidth(configWindow, 3);
            HOperatorSet.SetDraw(configWindow, "margin");
        }

        private void RunningSmartWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            runningWindow = null;
            var runningSmartWindow = new HSmartWindowControlWPF();
            runningSmartWindow.HInitWindow += RunningSmartWindow_HInitWindow;
            runningSmartWindow.Unloaded += RunningSmartWindow_Unloaded;
            RunningWindow = runningSmartWindow;
        }

        private void ConfigSmartWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            configWindow = null;
            var configSmartWindow = new HSmartWindowControlWPF();
            configSmartWindow.HInitWindow += ConfigSmartWindow_HInitWindow;
            configSmartWindow.Unloaded += ConfigSmartWindow_Unloaded;
            ConfigWindow = configSmartWindow;
        }

        #endregion

        #region 字段

        /// <summary>
        /// 运行时窗口
        /// </summary>
        private HWindow runningWindow;

        /// <summary>
        /// 配置窗口
        /// </summary>
        private HWindow configWindow;

        /// <summary>
        /// 初始化标志
        /// </summary>
        private bool isInit = false;

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
        public ItemCollection Inputs { get; } = new ItemCollection();

        /// <summary>
        /// 输出参数
        /// </summary>
        public ItemCollection Outputs { get; } = new ItemCollection();

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
        /// 执行
        /// </summary>
        /// <param name="image">图像</param>
        /// <param name="outputs">输出结果</param>
        /// <returns>执行结果</returns>
        public void Execute(object image, out ItemCollection outputs)
        {
            HObject hImage = image as HObject;
            outputs = new ItemCollection();

            stopwatch.Restart();

            HObject ho_GrayImage = null, ho_Regions = null;
            HObject ho_RegionClosing = null, ho_RegionFillUp = null, ho_ConnectedRegions = null;
            HObject ho_SelectedRegions = null, ho_RegionOpening = null;
            HObject ho_Rectangle = null, ho_RegionErosion = null, ho_ConnectedRegions1 = null;
            HObject ho_SelectedRegions1 = null, ho_Rectangle1 = null, ho_Cross = null;

            // Local control variables 
            HTuple hv_Row1 = new HTuple();
            HTuple hv_Column1 = new HTuple(), hv_Phi = new HTuple();
            HTuple hv_Length1 = new HTuple(), hv_Length2 = new HTuple();
            HTuple hv_Area = new HTuple(), hv_Row = new HTuple(), hv_Column = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_GrayImage);
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_RegionErosion);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_Cross);

            try
            {
                HTuple width, height;
                HOperatorSet.GetImageSize(hImage, out width, out height);

                //若未初始化,则进行初始化
                if (!isInit)
                {
                    isInit = true;
                }

                if (runningWindow == null)
                {
                    try
                    {
                        runningWindow = (RunningWindow as HSmartWindowControlWPF).HalconWindow;
                    }
                    catch (Exception)
                    {
                    }
                }

                if (configWindow == null)
                {
                    try
                    {
                        configWindow = (ConfigWindow as HSmartWindowControlWPF).HalconWindow;
                    }
                    catch (Exception)
                    {
                    }
                }

                if (runningWindow != null)
                {
                    SetWindowPart(runningWindow, width, height);
                    HOperatorSet.ClearWindow(runningWindow);
                    HOperatorSet.DispObj(hImage, runningWindow);
                }

                if (configWindow != null)
                {
                    SetWindowPart(configWindow, width, height);
                    HOperatorSet.ClearWindow(configWindow);
                    HOperatorSet.DispObj(hImage, configWindow);
                }

                //执行主任务
                //转灰度图
                ho_GrayImage.Dispose();
                HOperatorSet.Rgb1ToGray(hImage, out ho_GrayImage);

                //动态阈值的方式提取眼镜区域
                ho_Regions.Dispose();
                HOperatorSet.Threshold(ho_GrayImage, out ho_Regions, new HTuple(Inputs["MinGray"].Value), new HTuple(Inputs["MaxGray"].Value));

                //提取眼镜区域
                ho_RegionClosing.Dispose();
                HOperatorSet.ClosingRectangle1(ho_Regions, out ho_RegionClosing, new HTuple(Inputs["MorphologicalKernalWidth"].Value), new HTuple(Inputs["MorphologicalKernalHeight"].Value));
                ho_RegionFillUp.Dispose();
                HOperatorSet.FillUp(ho_RegionClosing, out ho_RegionFillUp);
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_RegionFillUp, out ho_ConnectedRegions);

                HOperatorSet.AreaCenter(ho_ConnectedRegions, out hv_Area, out hv_Row, out hv_Column);
                ho_Cross.Dispose();
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 35, 0.785398);

                //显示结果
                if (runningWindow != null)
                {
                    HOperatorSet.DispObj(ho_GrayImage, runningWindow);
                    HOperatorSet.DispObj(ho_ConnectedRegions, runningWindow);
                    HOperatorSet.DispObj(ho_Cross, runningWindow);
                }

                if (configWindow != null)
                {
                    HOperatorSet.DispObj(ho_GrayImage, configWindow);
                    HOperatorSet.DispObj(ho_ConnectedRegions, configWindow);
                    HOperatorSet.DispObj(ho_Cross, configWindow);
                }

                Outputs["X"].Value = hv_Column.DArr;
                Outputs["Y"].Value = hv_Column.DArr;

                stopwatch.Stop();
                RunStatus = new RunStatus(stopwatch.Elapsed.TotalMilliseconds);

                outputs = new ItemCollection(Outputs);

            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                RunStatus = new RunStatus(stopwatch.Elapsed.TotalMilliseconds, EResult.Error, ex.Message, ex);
                throw;
            }
            finally
            {
                hImage.Dispose();
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
            // GC.SuppressFinalize(this);
        }
        #endregion

        #endregion
    }
}
