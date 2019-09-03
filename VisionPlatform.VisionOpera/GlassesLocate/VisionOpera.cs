using HalconDotNet;
using System;
using System.Diagnostics;
using System.Windows;
using VisionPlatform.BaseType;

namespace GlassesLocate
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
                new ItemBase("HMinThreshold", 15, typeof(int), "H通道最小阈值"),
                new ItemBase("HMaxThreshold", 77, typeof(int), "H通道最大阈值"),
                new ItemBase("SMinThreshold", 14, typeof(int), "S通道最小阈值"),
                new ItemBase("SMaxThreshold", 55, typeof(int), "S通道最大阈值"),
                new ItemBase("IMinThreshold", 171, typeof(int), "I通道最小阈值"),
                new ItemBase("IMaxThreshold", 226, typeof(int), "I通道最大阈值"),

                new ItemBase("OpenRadius", 3.5, typeof(double), "开运算半径"),
                new ItemBase("CloseRadius", 1.0, typeof(double), "闭运算半径"),

                new ItemBase("MinArea", 100000, typeof(int), "最小筛选面积"),
                new ItemBase("MaxArea", 250000, typeof(int), "最大筛选面积"),

                new ItemBase("MinLen1", 600, typeof(int), "最小长边长度"),
                new ItemBase("MaxLen1", 1000, typeof(int), "最小长边长度"),

                new ItemBase("MinLen2", 40, typeof(int), "最小短边长度"),
                new ItemBase("MaxLen2", 200, typeof(int), "最小短边长度"),

                new ItemBase("XLDPhi", 30, typeof(int), "XLD骨骼方向范围(角度)"),
            };

            //配置输出参数
            Outputs = new ItemCollection()
            {
                new ItemBase("ItemLocation", new Location(), typeof(Location), "物料位置")
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

        private HObject ho_ROI_0;

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

            HObject ho_ImageReduced = null;
            HObject ho_R = null, ho_G = null, ho_B = null, ho_Hue = null, ho_Saturation = null;
            HObject ho_Intensity = null, ho_Regions = null, ho_RegionOpening = null;
            HObject ho_RegionClosing = null, ho_ImageReduced2 = null, ho_Regions1 = null;
            HObject ho_RegionOpening1 = null, ho_RegionClosing1 = null;
            HObject ho_ImageReduced3 = null, ho_Regions2 = null, ho_RegionOpening2 = null;
            HObject ho_RegionClosing2 = null, ho_RegionDifference = null;
            HObject ho_RegionOpening3 = null, ho_ConnectedRegions = null;
            HObject ho_SelectedRegions = null, ho_Skeleton = null, ho_EndPoints = null;
            HObject ho_JuncPoints = null, ho_RegionDifference1 = null, ho_ConnectedRegions1 = null;
            HObject ho_SelectedRegions1 = null, ho_Rectangle = null, ho_Contours = null;
            HObject ho_SelectedXLD = null, ho_UnionContours = null, ho_SelectedXLD1 = null;
            HObject ho_Cross = null;

            HTuple hv_Number = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_Phi = new HTuple(), hv_Length1 = new HTuple();
            HTuple hv_Length2 = new HTuple(), hv_Row1 = new HTuple();
            HTuple hv_Col1 = new HTuple(), hv_RowMedian = new HTuple();
            HTuple hv_ColMedian = new HTuple(), hv_Exception = new HTuple();

            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_R);
            HOperatorSet.GenEmptyObj(out ho_G);
            HOperatorSet.GenEmptyObj(out ho_B);
            HOperatorSet.GenEmptyObj(out ho_Hue);
            HOperatorSet.GenEmptyObj(out ho_Saturation);
            HOperatorSet.GenEmptyObj(out ho_Intensity);
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced2);
            HOperatorSet.GenEmptyObj(out ho_Regions1);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening1);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing1);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced3);
            HOperatorSet.GenEmptyObj(out ho_Regions2);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening2);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing2);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening3);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_Skeleton);
            HOperatorSet.GenEmptyObj(out ho_EndPoints);
            HOperatorSet.GenEmptyObj(out ho_JuncPoints);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference1);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_Contours);
            HOperatorSet.GenEmptyObj(out ho_SelectedXLD);
            HOperatorSet.GenEmptyObj(out ho_UnionContours);
            HOperatorSet.GenEmptyObj(out ho_SelectedXLD1);
            HOperatorSet.GenEmptyObj(out ho_Cross);

            try
            {
                HTuple width, height;
                HOperatorSet.GetImageSize(hImage, out width, out height);

                //若未初始化,则进行初始化
                if (!isInit)
                {
                    isInit = true;

                    ho_ROI_0?.Dispose();
                    HOperatorSet.GenRectangle2(out ho_ROI_0, 967.498, 1255, (new HTuple(2.39768)).TupleRad()
                        , 1363.19, 889.424);
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
                HOperatorSet.ReduceDomain(hImage, ho_ROI_0, out ho_ImageReduced);

                //转成HSV格式
                ho_R.Dispose(); ho_G.Dispose(); ho_B.Dispose();
                HOperatorSet.Decompose3(ho_ImageReduced, out ho_R, out ho_G, out ho_B);
                ho_Hue.Dispose(); ho_Saturation.Dispose(); ho_Intensity.Dispose();
                HOperatorSet.TransFromRgb(ho_R, ho_G, ho_B, out ho_Hue, out ho_Saturation,
                    out ho_Intensity, "hsv");

                //提取背景区域
                ho_Regions.Dispose();
                HOperatorSet.Threshold(ho_Hue, out ho_Regions, new HTuple(Inputs["HMinThreshold"].Value), new HTuple(Inputs["HMaxThreshold"].Value));
                ho_RegionOpening.Dispose();
                HOperatorSet.OpeningCircle(ho_Regions, out ho_RegionOpening, new HTuple(Inputs["OpenRadius"].Value));
                ho_RegionClosing.Dispose();
                HOperatorSet.ClosingCircle(ho_RegionOpening, out ho_RegionClosing, new HTuple(Inputs["CloseRadius"].Value));
                ho_ImageReduced2.Dispose();
                HOperatorSet.ReduceDomain(ho_Saturation, ho_RegionClosing, out ho_ImageReduced2
                    );
                ho_Regions1.Dispose();
                HOperatorSet.Threshold(ho_ImageReduced2, out ho_Regions1, new HTuple(Inputs["SMinThreshold"].Value), new HTuple(Inputs["SMaxThreshold"].Value));
                ho_RegionOpening1.Dispose();
                HOperatorSet.OpeningCircle(ho_Regions1, out ho_RegionOpening1, new HTuple(Inputs["OpenRadius"].Value));
                ho_RegionClosing1.Dispose();
                HOperatorSet.ClosingCircle(ho_RegionOpening1, out ho_RegionClosing1, new HTuple(Inputs["CloseRadius"].Value));
                ho_ImageReduced3.Dispose();
                HOperatorSet.ReduceDomain(ho_Intensity, ho_RegionClosing1, out ho_ImageReduced3
                    );
                ho_Regions2.Dispose();
                HOperatorSet.Threshold(ho_ImageReduced3, out ho_Regions2, new HTuple(Inputs["IMinThreshold"].Value), new HTuple(Inputs["IMaxThreshold"].Value));
                ho_RegionOpening2.Dispose();
                HOperatorSet.OpeningCircle(ho_Regions2, out ho_RegionOpening2, new HTuple(Inputs["OpenRadius"].Value));
                ho_RegionClosing2.Dispose();
                HOperatorSet.ClosingCircle(ho_RegionOpening2, out ho_RegionClosing2, new HTuple(Inputs["CloseRadius"].Value));

                //求出眼镜腿区域
                ho_RegionDifference.Dispose();
                HOperatorSet.Difference(ho_ROI_0, ho_RegionClosing2, out ho_RegionDifference
                    );
                ho_RegionOpening3.Dispose();
                HOperatorSet.OpeningCircle(ho_RegionDifference, out ho_RegionOpening3,
                    8);
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_RegionOpening3, out ho_ConnectedRegions);
                ho_SelectedRegions.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, (
                    (new HTuple("area")).TupleConcat("rect2_len1")).TupleConcat("rect2_len2"),
                    "and", ((new HTuple(Inputs["MinArea"].Value)).TupleConcat(new HTuple(Inputs["MinLen1"].Value))).TupleConcat(new HTuple(Inputs["MinLen2"].Value)), ((new HTuple(Inputs["MaxArea"].Value)).TupleConcat(
                    new HTuple(Inputs["MaxLen1"].Value))).TupleConcat(new HTuple(Inputs["MaxLen2"].Value)));

                //校验模板结果
                HOperatorSet.CountObj(ho_SelectedRegions, out hv_Number);
                if (hv_Number.I > 0)
                {
                    //产生骨骼
                    ho_Skeleton.Dispose();
                    HOperatorSet.Skeleton(ho_SelectedRegions, out ho_Skeleton);

                    //计算骨骼端点和关节点
                    ho_EndPoints.Dispose(); ho_JuncPoints.Dispose();
                    HOperatorSet.JunctionsSkeleton(ho_Skeleton, out ho_EndPoints, out ho_JuncPoints
                        );

                    //从骨骼中去除关节点
                    ho_RegionDifference1.Dispose();
                    HOperatorSet.Difference(ho_Skeleton, ho_JuncPoints, out ho_RegionDifference1
                        );

                    //提取最长的骨骼
                    ho_ConnectedRegions1.Dispose();
                    HOperatorSet.Connection(ho_RegionDifference1, out ho_ConnectedRegions1
                        );
                    ho_SelectedRegions1.Dispose();
                    HOperatorSet.SelectShapeStd(ho_ConnectedRegions1, out ho_SelectedRegions1,
                        "max_area", 70);

                    HOperatorSet.SmallestRectangle2(ho_SelectedRegions1, out hv_Row, out hv_Column,
                        out hv_Phi, out hv_Length1, out hv_Length2);
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row, hv_Column, hv_Phi,
                        hv_Length1, hv_Length2);

                    //转成xld
                    ho_Contours.Dispose();
                    HOperatorSet.GenContoursSkeletonXld(ho_Skeleton, out ho_Contours, 1,
                        "filter");

                    ho_SelectedXLD.Dispose();
                    HOperatorSet.SelectShapeXld(ho_Contours, out ho_SelectedXLD, (new HTuple("rect2_phi")).TupleConcat(
                        "rect2_phi"), "or", (((((hv_Phi.TupleDeg()) - new HTuple(Inputs["XLDPhi"].Value))).TupleRad())).TupleConcat(
                        ((((hv_Phi.TupleDeg()) + 180) - new HTuple(Inputs["XLDPhi"].Value))).TupleRad()), (((((hv_Phi.TupleDeg()
                        ) + new HTuple(Inputs["XLDPhi"].Value))).TupleRad())).TupleConcat(((((hv_Phi.TupleDeg()) + 180) + new HTuple(Inputs["XLDPhi"].Value))).TupleRad()
                        ));
                    ho_UnionContours.Dispose();
                    HOperatorSet.UnionAdjacentContoursXld(ho_SelectedXLD, out ho_UnionContours,
                        30, 1, "attr_keep");
                    ho_SelectedXLD1.Dispose();
                    HOperatorSet.SelectShapeXld(ho_UnionContours, out ho_SelectedXLD1, "area",
                        "and", 150, 99999);
                    HOperatorSet.GetContourXld(ho_SelectedXLD1, out hv_Row1, out hv_Col1);
                    HOperatorSet.TupleMedian(hv_Row1, out hv_RowMedian);
                    HOperatorSet.TupleMedian(hv_Col1, out hv_ColMedian);
                    ho_Cross.Dispose();
                    HOperatorSet.GenCrossContourXld(out ho_Cross, hv_RowMedian, hv_ColMedian,
                        100, ((new HTuple(45)).TupleRad()) + hv_Phi);

                    if (runningWindow != null)
                    {
                        HOperatorSet.DispObj(ho_SelectedXLD1, runningWindow);
                        HOperatorSet.DispObj(ho_Cross, runningWindow);
                    }

                    if (configWindow != null)
                    {
                        HOperatorSet.DispObj(ho_SelectedXLD1, configWindow);
                        HOperatorSet.DispObj(ho_Cross, configWindow);
                    }

                    //封装有效结果
                    Outputs["ItemLocation"].Value = new Location(hv_ColMedian[0].D, hv_RowMedian[0].D, hv_Phi[0].D);

                    stopwatch.Stop();
                    RunStatus = new RunStatus(stopwatch.Elapsed.TotalMilliseconds);
                }
                else
                {
                    //没有有效的结果
                    Outputs["ItemLocation"].Value = new Location();

                    stopwatch.Stop();
                    RunStatus = new RunStatus(stopwatch.Elapsed.TotalMilliseconds, EResult.Warning, "没找到有效目标");
                }

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

                ho_ImageReduced.Dispose();
                ho_R.Dispose();
                ho_G.Dispose();
                ho_B.Dispose();
                ho_Hue.Dispose();
                ho_Saturation.Dispose();
                ho_Intensity.Dispose();
                ho_Regions.Dispose();
                ho_RegionOpening.Dispose();
                ho_RegionClosing.Dispose();
                ho_ImageReduced2.Dispose();
                ho_Regions1.Dispose();
                ho_RegionOpening1.Dispose();
                ho_RegionClosing1.Dispose();
                ho_ImageReduced3.Dispose();
                ho_Regions2.Dispose();
                ho_RegionOpening2.Dispose();
                ho_RegionClosing2.Dispose();
                ho_RegionDifference.Dispose();
                ho_RegionOpening3.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_Skeleton.Dispose();
                ho_EndPoints.Dispose();
                ho_JuncPoints.Dispose();
                ho_RegionDifference1.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_Rectangle.Dispose();
                ho_Contours.Dispose();
                ho_SelectedXLD.Dispose();
                ho_UnionContours.Dispose();
                ho_SelectedXLD1.Dispose();
                ho_Cross.Dispose();

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
                ho_ROI_0?.Dispose();

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
