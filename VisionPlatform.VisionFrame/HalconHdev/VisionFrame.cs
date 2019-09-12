using Framework.Camera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionPlatform.BaseType;
using HalconDotNet;
using HalconHdev;
using System.Windows;
using System.Diagnostics;

namespace HalconHdev
{
    public class VisionFrame : IVisionFrame
    {
        #region 构造接口

        public VisionFrame()
        {
            //创建运行时/配置窗口控件
            var runningSmartWindow = new HSmartWindowControlWPF();
            runningSmartWindow.HInitWindow += RunningSmartWindow_HInitWindow;
            runningSmartWindow.Unloaded += RunningSmartWindow_Unloaded;
            RunningWindow = runningSmartWindow;
            ConfigWindow = runningSmartWindow;

            //var configSmartWindow = new HSmartWindowControlWPF();
            //configSmartWindow.HInitWindow += ConfigSmartWindow_HInitWindow;
            //configSmartWindow.Unloaded += ConfigSmartWindow_Unloaded;
            //ConfigWindow = configSmartWindow;

            //配置输入参数
            Inputs.Clear();

            //配置输出参数
            Outputs.Clear();
        }

        private void RunningSmartWindow_HInitWindow(object sender, EventArgs e)
        {
            runningWindow = (sender as HSmartWindowControlWPF).HalconWindow;
            HOperatorSet.SetColor(runningWindow, "lime green");
            HOperatorSet.SetLineWidth(runningWindow, 3);
            HOperatorSet.SetDraw(runningWindow, "margin");

            hDevEngine.SetHDevOperators(new HDevOpMultiWindowImpl(runningWindow));
        }

        //private void ConfigSmartWindow_HInitWindow(object sender, EventArgs e)
        //{
        //    configWindow = (sender as HSmartWindowControlWPF).HalconWindow;
        //    HOperatorSet.SetColor(configWindow, "lime green");
        //    HOperatorSet.SetLineWidth(configWindow, 3);
        //    HOperatorSet.SetDraw(configWindow, "margin");

        //    hDevEngine.SetHDevOperators(new HDevOpMultiWindowImpl(configWindow));
        //}

        private void RunningSmartWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            runningWindow = null;
            var runningSmartWindow = new HSmartWindowControlWPF();
            runningSmartWindow.HInitWindow += RunningSmartWindow_HInitWindow;
            runningSmartWindow.Unloaded += RunningSmartWindow_Unloaded;
            RunningWindow = runningSmartWindow;
        }

        //private void ConfigSmartWindow_Unloaded(object sender, RoutedEventArgs e)
        //{
        //    configWindow = null;
        //    var configSmartWindow = new HSmartWindowControlWPF();
        //    configSmartWindow.HInitWindow += ConfigSmartWindow_HInitWindow;
        //    configSmartWindow.Unloaded += ConfigSmartWindow_Unloaded;
        //    ConfigWindow = configSmartWindow;
        //}

        #endregion

        #region 字段

        /// <summary>
        /// 运行时窗口
        /// </summary>
        private HWindow runningWindow;

        ///// <summary>
        ///// 配置窗口
        ///// </summary>
        //private HWindow configWindow;

        /// <summary>
        /// 计时器
        /// </summary>
        private readonly Stopwatch stopwatch = new Stopwatch();

        HDevEngine hDevEngine = new HDevEngine();

        /// <summary>
        /// HDev程序
        /// </summary>
        private HDevProgram hDevProgram;

        /// <summary>
        /// HDev程序调用
        /// </summary>
        private HDevProgramCall hDevProgramCall;

        /// <summary>
        /// 输出参数数量
        /// </summary>
        private int valueCount;

        /// <summary>
        /// 输出参数名
        /// </summary>
        private string[] valueName;

        #endregion

        #region 属性

        /// <summary>
        /// 视觉框架名
        /// </summary>
        public EVisionFrameType EVisionFrameType { get; } = EVisionFrameType.HalconHdev;

        /// <summary>
        /// 视觉算子文件类型
        /// </summary>
        public EVisionOperaFileType VisionOperaFileType { get; } = EVisionOperaFileType.BinFile;

        /// <summary>
        /// 输出参数
        /// </summary>
        public ItemCollection Inputs { get; } = new ItemCollection();

        /// <summary>
        /// 输出参数
        /// </summary>
        public ItemCollection Outputs { get; } = new ItemCollection();

        /// <summary>
        /// 初始化标志
        /// </summary>
        public bool IsInit
        {
            get
            {
                if (hDevProgramCall?.IsInitialized() == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 运行状态
        /// </summary>
        public RunStatus RunStatus { get; set; } = new RunStatus();

        #region 功能使能

        /// <summary>
        /// 使能相机
        /// </summary>
        public bool IsEnableCamera { get; } = false;

        /// <summary>
        /// 使能视觉算子
        /// </summary>
        public bool IsEnableVisionOpera { get; } = false;

        /// <summary>
        /// 使能输入
        /// </summary>
        public bool IsEnableInput { get; } = false;

        /// <summary>
        /// 使能输出
        /// </summary>
        public bool IsEnableOutput { get; } = true;

        #endregion

        #region 控件

        /// <summary>
        /// 运行窗口
        /// </summary>
        public object RunningWindow { get; private set; }

        /// <summary>
        /// 配置窗口
        /// </summary>
        public object ConfigWindow { get; private set; }

        #endregion

        #endregion

        #region 方法

        /// <summary>
        /// 更新页面
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
        /// 初始化
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void Init(string filePath)
        {
            try
            {
                hDevProgram = new HDevProgram(filePath);
                hDevProgramCall = hDevProgram.CreateCall();

                valueCount = hDevProgram.GetCtrlVarCount();
                valueName = hDevProgram.GetCtrlVarNames().SArr;

                Outputs.Clear();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="timeout">处理超时时间,若小于等于0,则无限等待.单位:毫秒</param>
        /// <param name="outputs">输出结果</param>
        public void Execute(int timeout, out ItemCollection outputs)
        {
            try
            {
                stopwatch.Restart();

                if (runningWindow == null)
                {
                    try
                    {
                        runningWindow = (RunningWindow as HSmartWindowControlWPF).HalconWindow;
                        hDevEngine.SetHDevOperators(new HDevOpMultiWindowImpl(runningWindow));
                    }
                    catch (Exception)
                    {
                    }
                }

                //执行程序
                hDevProgramCall.Execute();

                //拼接输出结果
                Outputs.Clear();
                foreach (var item in valueName)
                {
                    HTuple valueTuple = hDevProgramCall.GetCtrlVarTuple(item);

                    switch (valueTuple.Type)
                    {
                        case HTupleType.EMPTY:
                            break;
                        case HTupleType.INTEGER:
                            Outputs.Add(new ItemBase(item, valueTuple.IArr, item));
                            break;
                        case HTupleType.LONG:
                            Outputs.Add(new ItemBase(item, valueTuple.LArr, item));
                            break;
                        case HTupleType.DOUBLE:
                            Outputs.Add(new ItemBase(item, valueTuple.DArr, item));
                            break;
                        case HTupleType.STRING:
                            Outputs.Add(new ItemBase(item, valueTuple.SArr, item));
                            break;
                        case HTupleType.MIXED:
                            break;
                        default:
                            break;
                    }
                }

                stopwatch.Stop();
                RunStatus = new RunStatus(stopwatch.Elapsed.TotalMilliseconds);

                outputs = new ItemCollection(Outputs);

                UpdatePart(runningWindow);

            }
            catch (Exception ex)
            {
                RunStatus = new RunStatus(0, EResult.Error, ex.Message, ex);
                throw;
            }
        
        }

        /// <summary>
        /// 通过图像信息执行
        /// </summary>
        /// <param name="imageInfo">相机信息</param>
        /// <param name="outputs">输出结果</param>
        public void ExecuteByImageInfo(ImageInfo imageInfo, out ItemCollection outputs)
        {
            Exception exception = new NotImplementedException("当前视觉框架不支持此方法(HalconHdev)");
            RunStatus = new RunStatus(0, EResult.Error, exception.Message, exception);

            throw exception;
        }

        /// <summary>
        /// 通过本地图片执行
        /// </summary>
        /// <param name="file">本地图片路径</param>
        /// <param name="outputs">输出参数</param>
        public void ExecuteByFile(string file, out ItemCollection outputs)
        {
            Exception exception = new NotImplementedException("当前视觉框架不支持此方法(HalconHdev)");
            RunStatus = new RunStatus(0, EResult.Error, exception.Message, exception);

            throw exception;
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
                hDevProgram?.Dispose();
                hDevProgramCall?.Dispose();

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~VisionFrame()
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
