using HalconDotNet;
using System;
using System.Diagnostics;
using System.Windows;
using VisionPlatform.BaseType;

namespace AgvLocate
{

    public partial class VisionOperation : IVisionOperation
    {
        #region 构造函数

        public VisionOperation()
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
            //配置输入参数
            Inputs = new ItemCollection()
            {
                new ItemBase("MeanImageMaskWidth", 15, typeof(int), "均值滤波掩码宽度"),
                new ItemBase("MeanImageMaskHeight", 15, typeof(int), "均值滤波掩码高度"),
                new ItemBase("DynThresholdOffset", 5.0, typeof(double), "动态阈值偏移量"),

                new ItemBase("Blob1MinArea", 60000, typeof(int), "大圆最小面积"),
                new ItemBase("Blob1MaxArea", 80000, typeof(int), "大圆最大面积"),
                new ItemBase("Blob1MincirCularity", 0.8, typeof(double), "大圆最小圆度"),

                new ItemBase("Blob1MinArea", 20000, typeof(int), "小圆最小面积"),
                new ItemBase("Blob1MaxArea", 35000, typeof(int), "小圆最大面积"),
                new ItemBase("Blob1MincirCularity", 0.8, typeof(double), "小圆最小圆度"),

                new ItemBase("Elements", 30, typeof(int), "圆拟合边缘点数"),
                new ItemBase("DetectHeight", 60, typeof(int), "卡尺工具高度"),
                new ItemBase("DetectWidth", 15, typeof(int), "卡尺工具宽度"),
                new ItemBase("Sigma", 1.0, typeof(double), "高斯滤波因子"),
                new ItemBase("Threshold", 20, typeof(int), "边缘幅度阈值"),
                new ItemBase("Transition", "positive", typeof(string), "过度颜色"),
                new ItemBase("Select", "'max", typeof(string), "有效点位选择"),
                new ItemBase("ActiveNum", 20, typeof(int), "有效点位数"),

                new ItemBase("BaseX", 1.0, typeof(double), "基础点"),
                new ItemBase("BaseY", 1.0, typeof(double), "基础点"),
                new ItemBase("LocationX1", 1.0, typeof(double), "点位1X"),
                new ItemBase("LocationY1", 1.0, typeof(double), "点位1Y"),
                new ItemBase("LocationX2", 1.0, typeof(double), "点位2X"),
                new ItemBase("LocationY2", 1.0, typeof(double), "点位2Y"),
                new ItemBase("LocationX3", 1.0, typeof(double), "点位3X"),
                new ItemBase("LocationY3", 1.0, typeof(double), "点位3Y"),

            };

            //配置输出参数
            Outputs = new ItemCollection()
            {
                new ItemBase("BaseX", 1.0, typeof(double), "基础点"),
                new ItemBase("BaseY", 1.0, typeof(double), "基础点"),
                new ItemBase("LocationX1", 1.0, typeof(double), "点位1X"),
                new ItemBase("LocationY1", 1.0, typeof(double), "点位1Y"),
                new ItemBase("LocationX2", 1.0, typeof(double), "点位2X"),
                new ItemBase("LocationY2", 1.0, typeof(double), "点位2Y"),
                new ItemBase("LocationX3", 1.0, typeof(double), "点位3X"),
                new ItemBase("LocationY3", 1.0, typeof(double), "点位3Y"),
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
        /// 更新窗口布局
        /// </summary>
        /// <param name="hWindow">窗口</param>
        private void UpdatePart(HWindow hWindow)
        {
            if (hWindow == null)
            {
                return;
            }

            HTuple row, column, row2, column2;
            hWindow.SetPart(0, 0, -2, -2);
            hWindow.GetPart(out row, out column, out row2, out column2);
            var rect = new Rect(column, row, column2 - column + 1, row2 - row + 1);
            hWindow.SetPart(rect.Top, rect.Left, rect.Bottom - 1.0, rect.Right - 1.0);
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

            // Local iconic variables 

            HObject ho_GrayImage, ho_ImageMean, ho_RegionDynThresh;
            HObject ho_RegionFillUp, ho_ConnectedRegions, ho_SelectedRegions1;
            HObject ho_SelectedRegions2, ho_Regions1 = null, ho_Regions2 = null;
            HObject ho_Circle1 = null, ho_Circle2 = null, ho_Cross1 = null;
            HObject ho_Arrow = null;

            // Local control variables 

            HTuple hv_Number1 = null, hv_Number2 = null, hv_Row1 = new HTuple();
            HTuple hv_Column1 = new HTuple(), hv_Radius1 = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Column2 = new HTuple();
            HTuple hv_Radius2 = new HTuple(), hv_RowArray1 = new HTuple();
            HTuple hv_ColumnArray1 = new HTuple(), hv_RowArray2 = new HTuple();
            HTuple hv_ColumnArray2 = new HTuple(), hv_ResultRow1 = new HTuple();
            HTuple hv_ResultColumn1 = new HTuple(), hv_ArcType1 = new HTuple();
            HTuple hv_ResultRow2 = new HTuple(), hv_ResultColumn2 = new HTuple();
            HTuple hv_ArcType2 = new HTuple(), hv_RowCenter1 = new HTuple();
            HTuple hv_ColCenter1 = new HTuple(), hv_StartPhi1 = new HTuple();
            HTuple hv_EndPhi1 = new HTuple(), hv_PointOrder1 = new HTuple();
            HTuple hv_ArcAngle1 = new HTuple(), hv_RowCenter2 = new HTuple();
            HTuple hv_ColCenter2 = new HTuple(), hv_StartPhi2 = new HTuple();
            HTuple hv_EndPhi2 = new HTuple(), hv_PointOrder2 = new HTuple();
            HTuple hv_ArcAngle2 = new HTuple(), hv_Angle = new HTuple();
            HTuple hv_HomMat2D = new HTuple(), hv_Qy = new HTuple();
            HTuple hv_Qx = new HTuple();

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_GrayImage);
            HOperatorSet.GenEmptyObj(out ho_ImageMean);
            HOperatorSet.GenEmptyObj(out ho_RegionDynThresh);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions2);
            HOperatorSet.GenEmptyObj(out ho_Regions1);
            HOperatorSet.GenEmptyObj(out ho_Regions2);
            HOperatorSet.GenEmptyObj(out ho_Circle1);
            HOperatorSet.GenEmptyObj(out ho_Circle2);
            HOperatorSet.GenEmptyObj(out ho_Cross1);
            HOperatorSet.GenEmptyObj(out ho_Arrow);

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
                    HOperatorSet.ClearWindow(runningWindow);
                    HOperatorSet.DispObj(hImage, runningWindow);
                    UpdatePart(runningWindow);
                }

                if (configWindow != null)
                {
                    HOperatorSet.ClearWindow(configWindow);
                    HOperatorSet.DispObj(hImage, configWindow);
                    UpdatePart(runningWindow);
                }

                //执行主任务
                //转为灰度图
                ho_GrayImage.Dispose();
                HOperatorSet.Rgb1ToGray(hImage, out ho_GrayImage);

                //动态阈值分割
                ho_ImageMean.Dispose();
                HOperatorSet.MeanImage(ho_GrayImage, out ho_ImageMean, 15, 15);
                ho_RegionDynThresh.Dispose();
                HOperatorSet.DynThreshold(ho_GrayImage, ho_ImageMean, out ho_RegionDynThresh,
                    5, "dark");

                //填充并获取圆
                ho_RegionFillUp.Dispose();
                HOperatorSet.FillUp(ho_RegionDynThresh, out ho_RegionFillUp);
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_RegionFillUp, out ho_ConnectedRegions);
                ho_SelectedRegions1.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions1, (new HTuple("circularity")).TupleConcat(
                    "area"), "and", (new HTuple(0.85)).TupleConcat(60000), (new HTuple(1.2)).TupleConcat(
                    80000));
                ho_SelectedRegions2.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions2, (new HTuple("circularity")).TupleConcat(
                    "area"), "and", (new HTuple(0.85)).TupleConcat(20000), (new HTuple(1.2)).TupleConcat(
                    35000));

                HOperatorSet.CountObj(ho_SelectedRegions1, out hv_Number1);
                HOperatorSet.CountObj(ho_SelectedRegions2, out hv_Number2);

                if ((hv_Number1.I == 1) && (hv_Number2.I == 1))
                {
                    //产生最小外接圆
                    HOperatorSet.SmallestCircle(ho_SelectedRegions1, out hv_Row1, out hv_Column1,
                        out hv_Radius1);
                    HOperatorSet.SmallestCircle(ho_SelectedRegions2, out hv_Row2, out hv_Column2,
                        out hv_Radius2);

                    hv_RowArray1 = new HTuple(new double[] { hv_Row1 + hv_Radius1, hv_Row1, hv_Row1 - hv_Radius1, hv_Row1, hv_Row1 + hv_Radius1 });
                    hv_ColumnArray1 = new HTuple(new double[] { hv_Column1, hv_Column1 + hv_Radius1, hv_Column1, hv_Column1 - hv_Radius1, hv_Column1 });
                    hv_RowArray2 = new HTuple(new double[] { hv_Row2 + hv_Radius2, hv_Row2, hv_Row2 - hv_Radius2, hv_Row2, hv_Row2 + hv_Radius2 });
                    hv_ColumnArray2 = new HTuple(new double[] { hv_Column2, hv_Column2 + hv_Radius2, hv_Column2, hv_Column2 - hv_Radius2, hv_Column2 });

                    //对最小外接圆的位置进行圆拟合
                    ho_Regions1.Dispose();
                    spoke(ho_GrayImage, out ho_Regions1, 30, 60, 15, 1, 20, "positive", "max",
                        hv_RowArray1, hv_ColumnArray1, "outer", out hv_ResultRow1, out hv_ResultColumn1,
                        out hv_ArcType1);
                    ho_Regions2.Dispose();
                    spoke(ho_GrayImage, out ho_Regions2, 30, 60, 15, 1, 20, "positive", "max",
                        hv_RowArray2, hv_ColumnArray2, "outer", out hv_ResultRow2, out hv_ResultColumn2,
                        out hv_ArcType2);
                    ho_Circle1.Dispose();
                    pts_to_best_circle(out ho_Circle1, hv_ResultRow1, hv_ResultColumn1, 20, "circle",
                        out hv_RowCenter1, out hv_ColCenter1, out hv_Radius1, out hv_StartPhi1,
                        out hv_EndPhi1, out hv_PointOrder1, out hv_ArcAngle1);
                    ho_Circle2.Dispose();
                    pts_to_best_circle(out ho_Circle2, hv_ResultRow2, hv_ResultColumn2, 20, "circle",
                        out hv_RowCenter2, out hv_ColCenter2, out hv_Radius2, out hv_StartPhi2,
                        out hv_EndPhi2, out hv_PointOrder2, out hv_ArcAngle2);

                    //计算角度
                    HOperatorSet.AngleLx(hv_RowCenter1, hv_ColCenter1, hv_RowCenter2, hv_ColCenter2,
                        out hv_Angle);

                    ho_Cross1.Dispose();
                    HOperatorSet.GenCrossContourXld(out ho_Cross1, hv_RowCenter1, hv_ColCenter1,
                        32, hv_Angle + ((new HTuple(45)).TupleRad()));
                    ho_Arrow.Dispose();
                    gen_arrow_contour_xld(out ho_Arrow, hv_RowCenter1, hv_ColCenter1, hv_RowCenter2,
                        hv_ColCenter2, 25, 25);

                    //计算仿射变换矩阵
                    //HOperatorSet.VectorAngleToRigid(hv_BaseRow, hv_BaseCol, hv_BaseAngle, hv_RowCenter1,
                    //    hv_ColCenter1, hv_Angle, out hv_HomMat2D);
                    //
                    ////计算仿射变换结果
                    //HOperatorSet.AffineTransPoint2d(hv_HomMat2D, hv_ResultRow, hv_ResultCol, out hv_Qy,
                    //    out hv_Qx);

                    //显示结果
                    //HOperatorSet.DispObj(ho_Image, hv_ExpDefaultWinHandle);
                    //HOperatorSet.DispObj(ho_SelectedRegions1, hv_ExpDefaultWinHandle);
                    //HOperatorSet.DispObj(ho_SelectedRegions2, hv_ExpDefaultWinHandle);
                    //HOperatorSet.DispObj(ho_Arrow, hv_ExpDefaultWinHandle);
                    //HOperatorSet.DispObj(ho_Cross1, hv_ExpDefaultWinHandle);
                }

                //显示结果
                if (runningWindow != null)
                {
                    UpdatePart(runningWindow);
                }

                if (configWindow != null)
                {
                    UpdatePart(runningWindow);
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
                hImage.Dispose();

                ho_GrayImage.Dispose();
                ho_ImageMean.Dispose();
                ho_RegionDynThresh.Dispose();
                ho_RegionFillUp.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_SelectedRegions2.Dispose();
                ho_Regions1.Dispose();
                ho_Regions2.Dispose();
                ho_Circle1.Dispose();
                ho_Circle2.Dispose();
                ho_Cross1.Dispose();
                ho_Arrow.Dispose();
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
        // ~VisionOperation()
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
