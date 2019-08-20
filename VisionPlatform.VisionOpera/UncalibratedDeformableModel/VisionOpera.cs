using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using VisionPlatform.BaseType;

namespace UncalibratedDeformableModel
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
            Inputs.Add(new ItemBase("ModelPath", @"C:\Users\Public\Documents\MVTec\HALCON-17.12-Progress\examples\hdevelop\Matching\Deformable\brake_disk_bike.dxf", typeof(string), "模板文件(.dxf)路径"));

            //配置输出参数
            Outputs.Clear();
            Outputs.Add(new ItemBase("MatchCount", typeof(int), "匹配数量"));
            Outputs.Add(new ItemBase("Scores", typeof(double[]), "匹配分数(List列表)"));

        }

        private void RunningSmartWindow_HInitWindow(object sender, EventArgs e)
        {
            runningWindowHandle = (sender as HSmartWindowControlWPF).HalconID;
            HOperatorSet.SetColor(runningWindowHandle, "lime green");
            HOperatorSet.SetLineWidth(runningWindowHandle, 3);
        }

        private void ConfigSmartWindow_HInitWindow(object sender, EventArgs e)
        {
            configWindowHandle = (sender as HSmartWindowControlWPF).HalconID;
            HOperatorSet.SetColor(configWindowHandle, "lime green");
            HOperatorSet.SetLineWidth(configWindowHandle, 3);
        }

        #endregion

        #region 字段

        private IntPtr runningWindowHandle;

        private IntPtr configWindowHandle;

        private bool isInit = false;

        /// <summary>
        /// 图像句柄
        /// </summary>
        private HObject hImage;

        /// <summary>
        /// 计时器
        /// </summary>
        private readonly Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// 模板骨架
        /// </summary>
        private HObject ho_Contours;

        /// <summary>
        /// 模板骨架
        /// </summary>
        private HObject ho_ModelContours;

        /// <summary>
        /// 模板ID
        /// </summary>
        private HTuple hv_ModelID = new HTuple();


        private HTuple hv_GenParamValue = new HTuple();

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

            HObject ho_ContoursProjTrans;
            HOperatorSet.GenEmptyObj(out ho_ContoursProjTrans);

            try
            {
                HTuple width, height;
                HOperatorSet.GetImageSize(hImage, out width, out height);

                if (runningWindowHandle != IntPtr.Zero)
                {
                    new Thread(delegate ()
                    {
                        ThreadPool.QueueUserWorkItem(delegate
                        {
                            System.Threading.SynchronizationContext.SetSynchronizationContext(new System.Windows.Threading.DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));
                            System.Threading.SynchronizationContext.Current.Send(pl =>
                            {
                                //执行相关任务.....
                                SetWindowPart((RunningWindow as HSmartWindowControlWPF).HalconWindow, width, height);
                            }, null);
                        });
                    }).Start();

                    HOperatorSet.DispObj(hImage, runningWindowHandle);
                }

                if (configWindowHandle != IntPtr.Zero)
                {
                    new Thread(delegate ()
                    {
                        ThreadPool.QueueUserWorkItem(delegate
                        {
                            System.Threading.SynchronizationContext.SetSynchronizationContext(new System.Windows.Threading.DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));
                            System.Threading.SynchronizationContext.Current.Send(pl =>
                            {
                                SetWindowPart((ConfigWindow as HSmartWindowControlWPF).HalconWindow, width, height);

                            }, null);
                        });
                    }).Start();

                    //SetWindowPart(new HWindow(configWindowHandle), width, height);
                    HOperatorSet.DispObj(hImage, configWindowHandle);
                }

                //若未初始化,则进行初始化
                if (!isInit)
                {
                    HTuple hv_DxfStatus = new HTuple();

                    if (!File.Exists(Inputs["ModelPath"].Value as string))
                    {
                        throw new FileNotFoundException("ModelPath is no found");
                    }

                    HOperatorSet.GenEmptyObj(out ho_Contours);
                    HOperatorSet.GenEmptyObj(out ho_ModelContours);

                    //读取骨架
                    HOperatorSet.ReadContourXldDxf(out ho_Contours, (Inputs["ModelPath"].Value as string), new HTuple(), new HTuple(), out hv_DxfStatus);

                    //创建模板
                    HOperatorSet.CreatePlanarUncalibDeformableModelXld(ho_Contours, "auto", new HTuple(),
                        new HTuple(), "auto", 1.0, new HTuple(), "auto", 1.0, new HTuple(), "auto",
                        "point_reduction_high", "ignore_part_polarity", 5, new HTuple(), new HTuple(),
                        out hv_ModelID);
                    if (hv_ModelID < 0)
                    {
                        return;
                    }

                    HOperatorSet.GetDeformableModelParams(hv_ModelID, "num_levels", out hv_GenParamValue);
                    ho_ModelContours.Dispose();
                    HOperatorSet.GetDeformableModelContours(out ho_ModelContours, hv_ModelID, 1);

                    isInit = true;
                }

                HTuple hv_HomMat2D, hv_Score;
                HOperatorSet.FindPlanarUncalibDeformableModel(hImage, hv_ModelID, 0, 0.78, 1, 1, 1, 1, 0.6, 1, 0.5, 0, 0.9, "subpixel", "least_squares", out hv_HomMat2D, out hv_Score);

                Outputs["MatchCount"].Value = hv_Score.Length;
                Outputs["Scores"].Value = hv_Score.DArr;

                for (int i = 0; i < hv_Score.Length; i++)
                {
                    HTuple hv_HomMatSelected;

                    HOperatorSet.TupleSelectRange(hv_HomMat2D, i * 9, ((i + 1) * 9) - 1, out hv_HomMatSelected);
                    ho_ContoursProjTrans.Dispose();
                    HOperatorSet.ProjectiveTransContourXld(ho_ModelContours, out ho_ContoursProjTrans,
                        hv_HomMatSelected);

                    if (runningWindowHandle != IntPtr.Zero)
                    {
                        HOperatorSet.DispObj(hImage, runningWindowHandle);
                        HOperatorSet.DispObj(ho_ContoursProjTrans, runningWindowHandle);
                    }

                    if (configWindowHandle != IntPtr.Zero)
                    {
                        HOperatorSet.DispObj(hImage, configWindowHandle);
                        HOperatorSet.DispObj(ho_ContoursProjTrans, configWindowHandle);
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
                hImage.Dispose();
                ho_ContoursProjTrans.Dispose();
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
