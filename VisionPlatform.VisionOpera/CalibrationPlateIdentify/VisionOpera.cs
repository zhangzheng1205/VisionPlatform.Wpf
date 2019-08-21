using HalconDotNet;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using VisionPlatform.BaseType;

namespace CalibrationPlateIdentify
{
    public class VisionOpera : IVisionOpera
    {
        #region 构造函数

        public VisionOpera()
        {
            //创建运行时/配置窗口控件
            var runningSmartWindow = new HSmartWindowControlWPF();
            runningSmartWindow.HInitWindow += RunningSmartWindow_HInitWindow;
            RunningWindow = runningSmartWindow;

            var configSmartWindow = new HSmartWindowControlWPF();
            configSmartWindow.HInitWindow += ConfigSmartWindow_HInitWindow; ;
            ConfigWindow = configSmartWindow;

            //配置输入参数
            Inputs.Clear();

            //配置输出参数
            Outputs = new ItemCollection()
            {
                new ItemBase("BaseX", typeof(double), "基准点X"),
                new ItemBase("BaseY", typeof(double), "基准点Y"),
                new ItemBase("Angle", typeof(double), "角度(弧度)"),
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

        #endregion

        #region 字段

        private HWindow runningWindow;

        private HWindow configWindow;

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
        public object RunningWindow { get; }

        /// <summary>
        /// 配置窗口
        /// </summary>
        public object ConfigWindow { get; }

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

            //初始化变量
            HObject ho_GrayImage, ho_Regions;
            HObject ho_ConnectedRegions, ho_SelectedRegions, ho_SelectedRegions1;
            HObject ho_Cross1 = null, ho_Cross2 = null;

            // Local control variables 
            HTuple hv_Area = null, hv_Row = null, hv_Column = null;
            HTuple hv_Area1 = null, hv_Row1 = null, hv_Column1 = null;
            HTuple hv_DistanceMin = new HTuple(), hv_DistanceMax = new HTuple();
            HTuple hv_Index = new HTuple(), hv_TempDistance = new HTuple();
            HTuple hv_Distance = new HTuple(), hv_Indices = new HTuple();
            HTuple hv_Angle = new HTuple(), hv_Deg = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_GrayImage);
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_Cross1);
            HOperatorSet.GenEmptyObj(out ho_Cross2);

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

                HOperatorSet.SetSystem("flush_graphic", "false");
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
                ho_GrayImage.Dispose();
                HOperatorSet.Rgb1ToGray(hImage, out ho_GrayImage);

                //Blob
                ho_Regions.Dispose();
                HOperatorSet.Threshold(ho_GrayImage, out ho_Regions, 100, 255);
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_Regions, out ho_ConnectedRegions);

                //选择圆
                ho_SelectedRegions.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, (new HTuple("circularity")).TupleConcat(
                    "area"), "and", (new HTuple(0.85)).TupleConcat(7000), (new HTuple(1)).TupleConcat(
                    8000));
                HOperatorSet.AreaCenter(ho_SelectedRegions, out hv_Area, out hv_Row, out hv_Column);

                //选择Mask点
                ho_SelectedRegions1.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions1, ((new HTuple("height")).TupleConcat(
                    "area")).TupleConcat("width"), "and", ((new HTuple(35)).TupleConcat(1000)).TupleConcat(
                    35), ((new HTuple(70)).TupleConcat(1600)).TupleConcat(70));
                HOperatorSet.AreaCenter(ho_SelectedRegions1, out hv_Area1, out hv_Row1, out hv_Column1);

                if ((int)((new HTuple((new HTuple(hv_Row.TupleLength())).TupleGreater(1))).TupleAnd(
                    new HTuple((new HTuple(hv_Row1.TupleLength())).TupleGreater(0)))) != 0)
                {

                    //找出与Mask点距离最短的原点
                    HOperatorSet.DistancePr(ho_SelectedRegions, hv_Row1, hv_Column1, out hv_DistanceMin,
                        out hv_DistanceMax);
                    for (hv_Index = 1; (int)hv_Index <= (int)(new HTuple(hv_Row.TupleLength())); hv_Index = (int)hv_Index + 1)
                    {
                        HOperatorSet.DistancePp(hv_Row.TupleSelect(hv_Index - 1), hv_Column.TupleSelect(
                            hv_Index - 1), hv_Row1.TupleSelect(0), hv_Column1.TupleSelect(0), out hv_TempDistance);
                        hv_Distance = hv_Distance.TupleConcat(hv_TempDistance);
                    }
                    HOperatorSet.TupleSortIndex(hv_Distance, out hv_Indices);

                    //原点角度
                    HOperatorSet.AngleLx(hv_Row.TupleSelect(hv_Indices.TupleSelect(0)), hv_Column.TupleSelect(
                        hv_Indices.TupleSelect(0)), hv_Row.TupleSelect(hv_Indices.TupleSelect((new HTuple(hv_Indices.TupleLength()
                        )) - 1)), hv_Column.TupleSelect(hv_Indices.TupleSelect((new HTuple(hv_Indices.TupleLength()
                        )) - 1)), out hv_Angle);
                    HOperatorSet.TupleDeg(hv_Angle, out hv_Deg);

                    Outputs["BaseX"].Value = hv_Column.TupleSelect(hv_Indices.TupleSelect(0)).D;
                    Outputs["BaseY"].Value = hv_Row.TupleSelect(hv_Indices.TupleSelect(0)).D;
                    Outputs["Angle"].Value = hv_Angle;

                    //显示结果
                    ho_Cross1.Dispose();
                    HOperatorSet.GenCrossContourXld(out ho_Cross1, hv_Row.TupleSelect(hv_Indices.TupleSelect(
                        0)), hv_Column.TupleSelect(hv_Indices.TupleSelect(0)), 24, 0.785398);
                    ho_Cross2.Dispose();
                    HOperatorSet.GenCrossContourXld(out ho_Cross2, hv_Row.TupleSelect(hv_Indices.TupleSelect(
                        (new HTuple(hv_Indices.TupleLength())) - 1)), hv_Column.TupleSelect(hv_Indices.TupleSelect(
                        (new HTuple(hv_Indices.TupleLength())) - 1)), 24, 0.785398);


                    if (runningWindow != null)
                    {
                        HOperatorSet.DispObj(hImage, runningWindow);
                        HOperatorSet.DispObj(ho_SelectedRegions, runningWindow);
                        HOperatorSet.DispObj(ho_SelectedRegions1, runningWindow);
                        HOperatorSet.DispObj(ho_Cross1, runningWindow);
                        HOperatorSet.DispObj(ho_Cross2, runningWindow);
                        HOperatorSet.DispLine(runningWindow, hv_Row.TupleSelect(hv_Indices.TupleSelect(
                            0)), hv_Column.TupleSelect(hv_Indices.TupleSelect(0)), hv_Row.TupleSelect(
                            hv_Indices.TupleSelect((new HTuple(hv_Indices.TupleLength())) - 1)), hv_Column.TupleSelect(
                            hv_Indices.TupleSelect((new HTuple(hv_Indices.TupleLength())) - 1)));
                    }

                    if (configWindow != null)
                    {
                        HOperatorSet.DispObj(ho_SelectedRegions, configWindow);
                        HOperatorSet.DispObj(ho_SelectedRegions1, configWindow);
                        HOperatorSet.DispObj(ho_Cross1, configWindow);
                        HOperatorSet.DispObj(ho_Cross2, configWindow);
                        HOperatorSet.DispLine(configWindow, hv_Row.TupleSelect(hv_Indices.TupleSelect(
                            0)), hv_Column.TupleSelect(hv_Indices.TupleSelect(0)), hv_Row.TupleSelect(
                            hv_Indices.TupleSelect((new HTuple(hv_Indices.TupleLength())) - 1)), hv_Column.TupleSelect(
                            hv_Indices.TupleSelect((new HTuple(hv_Indices.TupleLength())) - 1)));
                    }
                }

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
                HOperatorSet.SetSystem("flush_graphic", "true");

                //释放本次运行的临时资源
                hImage.Dispose();
                ho_GrayImage.Dispose();
                ho_Regions.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_Cross1.Dispose();
                ho_Cross2.Dispose();
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
