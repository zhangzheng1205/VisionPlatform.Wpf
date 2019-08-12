using Framework.Camera;
using HalconDotNet;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using VisionPlatform.BaseType;
using ItemCollection = VisionPlatform.BaseType.ItemCollection;

namespace VisionPlatform.HalconVisionFrame
{
    public class VisionFrame : IVisionFrame
    {
        #region 字段

        /// <summary>
        /// 图像变量
        /// </summary>
        private HObject hImage;

        #endregion

        #region 属性

        /// <summary>
        /// 视觉框架名
        /// </summary>
        public EVisionFrame EVisionFrame { get; } = EVisionFrame.Halcon;

        /// <summary>
        /// 视觉算子接口
        /// </summary>
        public IVisionOpera VisionOpera { get; set; }

        /// <summary>
        /// 视觉算子文件类型
        /// </summary>
        public EVisionOperaFileType VisionOperaFileType { get; } = EVisionOperaFileType.DLL;

        /// <summary>
        /// 输出参数
        /// </summary>
        public ItemCollection Inputs
        {
            get
            {
                return VisionOpera?.Inputs;
            }
        }

        /// <summary>
        /// 输出参数
        /// </summary>
        public ItemCollection Outputs
        {
            get
            {
                return VisionOpera?.Outputs;
            }
        }

        /// <summary>
        /// 初始化标志
        /// </summary>
        public bool IsInit
        {
            get
            {
                if (VisionOpera != null)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 运行状态
        /// </summary>
        public RunStatus RunStatus { get; set; }

        #region 功能使能

        /// <summary>
        /// 使能相机
        /// </summary>
        public bool IsEnableCamera { get; } = true;

        /// <summary>
        /// 使能视觉算子
        /// </summary>
        public bool IsEnableVisionOpera { get; } = true;

        /// <summary>
        /// 使能输入
        /// </summary>
        public bool IsEnableInput { get; } = true;

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
                return VisionOpera?.RunningWindow;
            }
        }

        /// <summary>
        /// 配置窗口
        /// </summary>
        public object ConfigWindow
        {
            get
            {
                return VisionOpera?.ConfigWindow;
            }
        }

        #endregion

        #endregion

        #region 方法

        /// <summary>
        /// 获取视觉算子实例
        /// </summary>
        /// <param name="dllPath">dll路径</param>
        /// <returns>视觉算子实例</returns>
        private static IVisionOpera GetVisionOperaInstance(string dllPath)
        {

            if (string.IsNullOrEmpty(dllPath))
            {
                throw new ArgumentNullException("filePath cannot be null");
            }

            if (!File.Exists(dllPath))
            {
                throw new FileNotFoundException($"filePath invalid({dllPath})");
            }

            object obj = new object();

            try
            {
                //实例化视觉算子
                var visionOperaAssembly = Assembly.LoadFrom(dllPath);
                var types = visionOperaAssembly.GetTypes();

                foreach (var item in types)
                {
                    if (item.Name == "VisionOpera")
                    {
                        obj = visionOperaAssembly.CreateInstance(item.FullName);
                        break;
                    }
                }

                //若视觉算子DLL有效,则创建默认配置
                if (!(obj is IVisionOpera))
                {
                    throw new Exception($"VisionOperaDll 数据类型异常! {obj.GetType()}");
                }
            }
            catch (Exception)
            {
                throw;
            }

            //创建默认配置
            return obj as IVisionOpera;
        }

        /// <summary>
        /// 创建HImage
        /// </summary>
        /// <param name="ePixelFormat">像素类型</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="imagePtr">图像指针</param>
        /// <param name="hImage">H图像</param>
        /// <returns></returns>
        private static bool CreateHImage(ImageInfo imageInfo, out HObject hImage)
        {
            HOperatorSet.GenEmptyObj(out hImage);

            try
            {
                switch (imageInfo.PixelFormat)
                {
                    case EPixelFormatType.GVSP_PIX_MONO8:
                        hImage?.Dispose();
                        HOperatorSet.GenImage1(out hImage, "byte", imageInfo.Width, imageInfo.Height, imageInfo.ImagePtr);
                        break;
                    case EPixelFormatType.GVSP_PIX_RGB8_PACKED:
                        hImage?.Dispose();
                        HOperatorSet.GenImageInterleaved(out hImage, imageInfo.ImagePtr, "rgb", imageInfo.Width, imageInfo.Height, -1, "byte", 0, 0, 0, 0, -1, 0);
                        break;
                    case EPixelFormatType.GVSP_PIX_BGR8_PACKED:
                        hImage?.Dispose();
                        HOperatorSet.GenImageInterleaved(out hImage, imageInfo.ImagePtr, "bgr", imageInfo.Width, imageInfo.Height, -1, "byte", 0, 0, 0, 0, -1, 0);
                        break;
                    case EPixelFormatType.GVSP_PIX_RGBA8_PACKED:
                        hImage?.Dispose();
                        HOperatorSet.GenImageInterleaved(out hImage, imageInfo.ImagePtr, "rgbx", imageInfo.Width, imageInfo.Height, -1, "byte", 0, 0, 0, 0, -1, 0);
                        break;
                    case EPixelFormatType.GVSP_PIX_BGRA8_PACKED:
                        hImage?.Dispose();
                        HOperatorSet.GenImageInterleaved(out hImage, imageInfo.ImagePtr, "bgrx", imageInfo.Width, imageInfo.Height, -1, "byte", 0, 0, 0, 0, -1, 0);
                        break;
                    case EPixelFormatType.GVSP_PIX_BAYGB8:
                        {
                            hImage?.Dispose();

                            //生成bayer图像
                            HObject hBayerImage;
                            HOperatorSet.GenImage1(out hBayerImage, "byte", imageInfo.Width, imageInfo.Height, imageInfo.ImagePtr);
                            HOperatorSet.CfaToRgb(hBayerImage, out hImage, "bayer_gb", "bilinear");
                            hBayerImage?.Dispose();
                            break;
                        }
                    case EPixelFormatType.GVSP_PIX_BAYGR8:
                        {
                            hImage?.Dispose();

                            //生成bayer图像
                            HObject hBayerImage;
                            HOperatorSet.GenImage1(out hBayerImage, "byte", imageInfo.Width, imageInfo.Height, imageInfo.ImagePtr);
                            HOperatorSet.CfaToRgb(hBayerImage, out hImage, "bayer_gr", "bilinear");
                            hBayerImage?.Dispose();
                            break;
                        }
                    case EPixelFormatType.GVSP_PIX_BAYRG8:
                        {
                            hImage?.Dispose();

                            //生成bayer图像
                            HObject hBayerImage;
                            HOperatorSet.GenImage1(out hBayerImage, "byte", imageInfo.Width, imageInfo.Height, imageInfo.ImagePtr);
                            HOperatorSet.CfaToRgb(hBayerImage, out hImage, "bayer_rg", "bilinear");
                            hBayerImage?.Dispose();
                            break;
                        }
                    case EPixelFormatType.GVSP_PIX_BAYBG8:
                        {
                            hImage?.Dispose();

                            //生成bayer图像
                            HObject hBayerImage;
                            HOperatorSet.GenImage1(out hBayerImage, "byte", imageInfo.Width, imageInfo.Height, imageInfo.ImagePtr);
                            HOperatorSet.CfaToRgb(hBayerImage, out hImage, "bayer_bg", "bilinear");
                            hBayerImage?.Dispose();
                            break;
                        }
                    default:
                        return false;
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void Init(string filePath)
        {
            try
            {
                VisionOpera = GetVisionOperaInstance(filePath);
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
            Exception exception = new NotImplementedException("当前视觉框架不支持此方法(HalconVisionFrame)");
            RunStatus = new RunStatus(0, EResult.Error, exception.Message, exception);

            throw exception;
        }

        /// <summary>
        /// 通过图像信息执行
        /// </summary>
        /// <param name="imageInfo">相机信息</param>
        /// <param name="outputs">输出结果</param>
        public void ExecuteByImageInfo(ImageInfo imageInfo, out ItemCollection outputs)
        {
            if (VisionOpera == null)
            {
                Exception exception = new NullReferenceException("VisionOpera invalid");
                RunStatus = new RunStatus(0, EResult.Error, exception.Message, exception);

                throw exception;
            }

            try
            {
                //转换成HImage图像
                hImage?.Dispose();
                CreateHImage(imageInfo, out hImage);

                VisionOpera.Execute(hImage, out outputs);
                RunStatus = VisionOpera.RunStatus;
            }
            catch (Exception ex)
            {
                RunStatus = new RunStatus(0, EResult.Error, ex.Message, ex);
                throw;
            }

        }

        /// <summary>
        /// 通过本地图片执行
        /// </summary>
        /// <param name="file">本地图片路径</param>
        /// <param name="outputs">输出参数</param>
        public void ExecuteByFile(string file, out ItemCollection outputs)
        {
            if (VisionOpera == null)
            {
                Exception exception = new NullReferenceException("VisionOpera invalid");
                RunStatus = new RunStatus(0, EResult.Error, exception.Message, exception);

                throw exception;
            }

            try
            {
                hImage?.Dispose();
                HOperatorSet.ReadImage(out hImage, file);
                VisionOpera.Execute(hImage, out outputs);
                RunStatus = VisionOpera.RunStatus;
            }
            catch (Exception ex)
            {
                RunStatus = new RunStatus(0, EResult.Error, ex.Message, ex);
                throw;
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
                VisionOpera?.Dispose();
                VisionOpera = null;

                //Camera = null;

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
