
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using VisionPlatform.BaseType;
using ItemCollection = VisionPlatform.BaseType.ItemCollection;
using Framework.Camera;
using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro;

namespace VisionPlatform.VisionProVisionFrame
{
    public class VisionFrame : IVisionFrame
    {
        #region 构造接口

        public VisionFrame()
        {
            visionProDisplayControl = new VisionProDisplayControl();
            visionProConfigControl = new VisionProConfigControl();

            visionProConfigControl.NewVppFileLoaded += VisionProConfigControl_NewVppFileLoaded;
        }

        /// <summary>
        /// 加载新VPP文件事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VisionProConfigControl_NewVppFileLoaded(object sender, NewVppFileLoadedEventArgs e)
        {
            if (cogToolBlock?.Equals(e.CogToolBlock) == false)
            {
                vppFilePath = e.FileName;
                cogToolBlock?.Dispose();
                cogToolBlock = e.CogToolBlock;
                IsInit = Init(e.CogToolBlock);
            }

        }

        #endregion

        #region 字段

        /// <summary>
        /// 显示控件
        /// </summary>
        private VisionProDisplayControl visionProDisplayControl;

        /// <summary>
        /// 配置控件
        /// </summary>
        private VisionProConfigControl visionProConfigControl;

        /// <summary>
        /// VPP文件路径
        /// </summary>
        private string vppFilePath = "";

        /// <summary>
        /// 工具
        /// </summary>
        private CogToolBlock cogToolBlock;

        /// <summary>
        /// 线程锁
        /// </summary>
        private readonly object threadLock = new object();

        /// <summary>
        /// 全局线程锁
        /// </summary>
        private readonly static object globalThreadLock = new object();

        #endregion

        #region 属性

        /// <summary>
        /// 视觉框架名
        /// </summary>
        public EVisionFrame EVisionFrame { get; } = EVisionFrame.VisionPro;

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
        public bool IsInit { get; private set; } = false;

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
        public object RunningWindow
        {
            get
            {
                return visionProDisplayControl;
            }
        }

        /// <summary>
        /// 配置窗口
        /// </summary>
        public object ConfigWindow
        {
            get
            {
                return visionProConfigControl;
            }
        }

        #endregion

        #endregion

        #region 方法


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="cogToolBlock">ToolBlock实例</param>
        /// <returns>结果</returns>
        private bool Init(CogToolBlock cogToolBlock)
        {
            if (cogToolBlock != null)
            {
                visionProDisplayControl.SetCogToolDisplayTool(cogToolBlock);
                visionProConfigControl.SetCogToolBlockEditSubject(cogToolBlock);

                Outputs.Clear();
                for (int i = 0; i < cogToolBlock.Outputs.Count; i++)
                {
                    Outputs.Add(new ItemBase(cogToolBlock.Outputs[i].Name, cogToolBlock.Outputs[i].Value, cogToolBlock.Outputs[i].ValueType, cogToolBlock.Outputs[i].Description));
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void Init(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException("filePath cannot be null");
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"filePath invalid({filePath})");
            }

            try
            {
                vppFilePath = filePath;

                lock (threadLock)
                {
                    IsInit = false;

                    //释放之前的资源
                    cogToolBlock?.Dispose();
                    cogToolBlock = null;

                    //加载VisionPro工具
                    cogToolBlock = CogSerializer.LoadObjectFromFile(vppFilePath) as CogToolBlock;

                    //加载显示界面
                    IsInit = Init(cogToolBlock);
                }
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
            outputs = new ItemCollection();

            //全局线程锁(静态),避免不同线程间同时调用VisionPro的资源
            lock (globalThreadLock)
            {
                try
                {
                    //线程锁,避免在不同线程中分别调用init和run,导致isInit变量冲突
                    lock (threadLock)
                    {
                        if (!IsInit)
                        {
                            throw new ArgumentException("VisionFrame is uninit");
                        }
                    }

                    if (cogToolBlock != null)
                    {
                        cogToolBlock.Run();

                        Outputs.Clear();
                        for (int i = 0; i < cogToolBlock.Outputs.Count; i++)
                        {
                            Outputs.Add(new ItemBase(cogToolBlock.Outputs[i].Name, cogToolBlock.Outputs[i].Value, cogToolBlock.Outputs[i].ValueType, cogToolBlock.Outputs[i].Description));
                        }
                        outputs = new ItemCollection(Outputs);

                        EResult result = EResult.Accept;
                        switch (cogToolBlock.RunStatus.Result)
                        {
                            case CogToolResultConstants.Accept:
                                result = EResult.Accept;
                                break;
                            case CogToolResultConstants.Warning:
                                result = EResult.Warning;
                                break;
                            case CogToolResultConstants.Reject:
                                result = EResult.Reject;
                                break;
                            case CogToolResultConstants.Error:
                                result = EResult.Error;
                                break;
                            default:
                                break;
                        }

                        RunStatus = new RunStatus(cogToolBlock.RunStatus.ProcessingTime, result, cogToolBlock.RunStatus.Message, cogToolBlock.RunStatus.Exception);

                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 通过图像信息执行
        /// </summary>
        /// <param name="imageInfo">相机信息</param>
        /// <param name="outputs">输出结果</param>
        public void ExecuteByImageInfo(ImageInfo imageInfo, out ItemCollection outputs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 通过本地图片执行
        /// </summary>
        /// <param name="file">本地图片路径</param>
        /// <param name="outputs">输出参数</param>
        public void ExecuteByFile(string file, out ItemCollection outputs)
        {
            throw new NotImplementedException();
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

                cogToolBlock?.Dispose();
                cogToolBlock = null;

                visionProDisplayControl = null;
                visionProConfigControl = null;

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
