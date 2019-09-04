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
                new ItemBase("MinThreshold", 0, typeof(int), "最小阈值"),
                new ItemBase("MaxThreshold", 70, typeof(int), "最大阈值"),
                new ItemBase("BlobArea", 10000, typeof(int), "过滤杂斑面积"),
            };

            //配置输出参数
            Outputs = new ItemCollection()
            {
                new ItemBase("ItemLocation", new Location[0], typeof(Location[]), "物料位置(数组)")
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

        // Procedures 
        // Chapter: Graphics / Text
        // Short Description: This procedure writes a text message. 
        public void disp_message(HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem,
            HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_GenParamName = null, hv_GenParamValue = null;
            HTuple hv_Color_COPY_INP_TMP = hv_Color.Clone();
            HTuple hv_Column_COPY_INP_TMP = hv_Column.Clone();
            HTuple hv_CoordSystem_COPY_INP_TMP = hv_CoordSystem.Clone();
            HTuple hv_Row_COPY_INP_TMP = hv_Row.Clone();

            // Initialize local and output iconic variables 
            //This procedure displays text in a graphics window.
            //
            //Input parameters:
            //WindowHandle: The WindowHandle of the graphics window, where
            //   the message should be displayed
            //String: A tuple of strings containing the text message to be displayed
            //CoordSystem: If set to 'window', the text position is given
            //   with respect to the window coordinate system.
            //   If set to 'image', image coordinates are used.
            //   (This may be useful in zoomed images.)
            //Row: The row coordinate of the desired text position
            //   A tuple of values is allowed to display text at different
            //   positions.
            //Column: The column coordinate of the desired text position
            //   A tuple of values is allowed to display text at different
            //   positions.
            //Color: defines the color of the text as string.
            //   If set to [], '' or 'auto' the currently set color is used.
            //   If a tuple of strings is passed, the colors are used cyclically...
            //   - if |Row| == |Column| == 1: for each new textline
            //   = else for each text position.
            //Box: If Box[0] is set to 'true', the text is written within an orange box.
            //     If set to' false', no box is displayed.
            //     If set to a color string (e.g. 'white', '#FF00CC', etc.),
            //       the text is written in a box of that color.
            //     An optional second value for Box (Box[1]) controls if a shadow is displayed:
            //       'true' -> display a shadow in a default color
            //       'false' -> display no shadow
            //       otherwise -> use given string as color string for the shadow color
            //
            //It is possible to display multiple text strings in a single call.
            //In this case, some restrictions apply:
            //- Multiple text positions can be defined by specifying a tuple
            //  with multiple Row and/or Column coordinates, i.e.:
            //  - |Row| == n, |Column| == n
            //  - |Row| == n, |Column| == 1
            //  - |Row| == 1, |Column| == n
            //- If |Row| == |Column| == 1,
            //  each element of String is display in a new textline.
            //- If multiple positions or specified, the number of Strings
            //  must match the number of positions, i.e.:
            //  - Either |String| == n (each string is displayed at the
            //                          corresponding position),
            //  - or     |String| == 1 (The string is displayed n times).
            //
            //
            //Convert the parameters for disp_text.
            if ((int)((new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
                new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(new HTuple())))) != 0)
            {

                return;
            }
            if ((int)(new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(-1))) != 0)
            {
                hv_Row_COPY_INP_TMP = 12;
            }
            if ((int)(new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(-1))) != 0)
            {
                hv_Column_COPY_INP_TMP = 12;
            }
            //
            //Convert the parameter Box to generic parameters.
            hv_GenParamName = new HTuple();
            hv_GenParamValue = new HTuple();
            if ((int)(new HTuple((new HTuple(hv_Box.TupleLength())).TupleGreater(0))) != 0)
            {
                if ((int)(new HTuple(((hv_Box.TupleSelect(0))).TupleEqual("false"))) != 0)
                {
                    //Display no box
                    hv_GenParamName = hv_GenParamName.TupleConcat("box");
                    hv_GenParamValue = hv_GenParamValue.TupleConcat("false");
                }
                else if ((int)(new HTuple(((hv_Box.TupleSelect(0))).TupleNotEqual("true"))) != 0)
                {
                    //Set a color other than the default.
                    hv_GenParamName = hv_GenParamName.TupleConcat("box_color");
                    hv_GenParamValue = hv_GenParamValue.TupleConcat(hv_Box.TupleSelect(0));
                }
            }
            if ((int)(new HTuple((new HTuple(hv_Box.TupleLength())).TupleGreater(1))) != 0)
            {
                if ((int)(new HTuple(((hv_Box.TupleSelect(1))).TupleEqual("false"))) != 0)
                {
                    //Display no shadow.
                    hv_GenParamName = hv_GenParamName.TupleConcat("shadow");
                    hv_GenParamValue = hv_GenParamValue.TupleConcat("false");
                }
                else if ((int)(new HTuple(((hv_Box.TupleSelect(1))).TupleNotEqual("true"))) != 0)
                {
                    //Set a shadow color other than the default.
                    hv_GenParamName = hv_GenParamName.TupleConcat("shadow_color");
                    hv_GenParamValue = hv_GenParamValue.TupleConcat(hv_Box.TupleSelect(1));
                }
            }
            //Restore default CoordSystem behavior.
            if ((int)(new HTuple(hv_CoordSystem_COPY_INP_TMP.TupleNotEqual("window"))) != 0)
            {
                hv_CoordSystem_COPY_INP_TMP = "image";
            }
            //
            if ((int)(new HTuple(hv_Color_COPY_INP_TMP.TupleEqual(""))) != 0)
            {
                //disp_text does not accept an empty string for Color.
                hv_Color_COPY_INP_TMP = new HTuple();
            }
            //
            HOperatorSet.DispText(hv_WindowHandle, hv_String, hv_CoordSystem_COPY_INP_TMP,
                hv_Row_COPY_INP_TMP, hv_Column_COPY_INP_TMP, hv_Color_COPY_INP_TMP, hv_GenParamName,
                hv_GenParamValue);

            return;
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


            HObject ho_Image20190904144527, ho_GrayImage;
            HObject ho_Regions, ho_ConnectedRegions, ho_SelectedRegions;
            HObject ho_ContCircle;

            // Local control variables 
            HTuple hv_Number = new HTuple();
            HTuple hv_Row = null, hv_Column = null, hv_Radius = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image20190904144527);
            HOperatorSet.GenEmptyObj(out ho_GrayImage);
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_ContCircle);

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

                //阈值分割
                ho_Regions.Dispose();
                HOperatorSet.Threshold(ho_GrayImage, out ho_Regions, new HTuple(Inputs["MinThreshold"].Value), new HTuple(Inputs["MaxThreshold"].Value));

                //提取有效区域
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_Regions, out ho_ConnectedRegions);
                ho_SelectedRegions.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                    "and", new HTuple(Inputs["BlobArea"].Value), 999999999);

                HOperatorSet.CountObj(ho_SelectedRegions, out hv_Number);
                if (hv_Number.I > 0)
                {

                    //提取有效区域
                    HOperatorSet.SmallestCircle(ho_SelectedRegions, out hv_Row, out hv_Column, out hv_Radius);
                    ho_ContCircle.Dispose();
                    HOperatorSet.GenCircleContourXld(out ho_ContCircle, hv_Row, hv_Column, hv_Radius + 20,
                        0, 6.28318, "positive", 1);

                    var locations = new Location[hv_Column.Length];
                    for (int i = 0; i < hv_Column.Length; i++)
                    {
                        locations[i] = new Location(hv_Column[i].D, hv_Row[i].D, 0);
                    }
                    Outputs["ItemLocation"].Value = locations;

                    //显示结果
                    if (runningWindow != null)
                    {
                        HOperatorSet.DispObj(ho_GrayImage, runningWindow);
                        HOperatorSet.DispObj(ho_ContCircle, runningWindow);
                        disp_message(runningWindow, (hv_Column + new HTuple(",")) + hv_Row, "image",
            hv_Row - 100, hv_Column + 100, "black", "true");
                    }

                    if (configWindow != null)
                    {
                        HOperatorSet.DispObj(ho_GrayImage, configWindow);
                        HOperatorSet.DispObj(ho_ContCircle, configWindow);
                        disp_message(configWindow, (hv_Column + new HTuple(",")) + hv_Row, "image",
            hv_Row - 100, hv_Column + 100, "black", "true");
                    }

                    stopwatch.Stop();
                    RunStatus = new RunStatus(stopwatch.Elapsed.TotalMilliseconds);
                }
                else
                {
                    //没有有效的结果
                    Outputs["ItemLocation"].Value = new Location[0];

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

                ho_GrayImage.Dispose();
                ho_Regions.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_ContCircle.Dispose();
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
