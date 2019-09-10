using Caliburn.Micro;
using Framework.Camera;
using Framework.Infrastructure.Image;
using Framework.Infrastructure.Serialization;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using VisionPlatform.BaseType;
using VisionPlatform.Core;

namespace VisionPlatform.Wpf
{
    /// <summary>
    /// 相机控件模式
    /// </summary>
    /// <remarks>
    /// 此相机控件只会对相机列表中已打开的相机进行控制
    /// </remarks>
    public class CameraViewModel : Screen
    {
        #region 构造函数

        /// <summary>
        /// 创建CameraViewModel新实例
        /// </summary>
        public CameraViewModel()
        {
            UpdateCameras();
            CameraConfigViewModel.NewImageEvent += CameraConfigViewModel_NewImageEvent;
            NotifyOfPropertyChange(() => IsCameraValid);
        }

        #endregion

        #region 属性

        private ObservableCollection<ICamera> cameras = new ObservableCollection<ICamera>();

        /// <summary>
        /// 相机列表
        /// </summary>
        public ObservableCollection<ICamera> Cameras
        {
            get
            {
                return cameras;
            }
            set
            {
                cameras = value;
                NotifyOfPropertyChange(() => Cameras);
            }
        }

        /// <summary>
        /// 相机有效性
        /// </summary>
        public bool IsCameraValid
        {
            get
            {
                if (CameraConfigViewModel?.Camera?.IsOpen == true)
                {
                    return true;
                }

                return false;
            }
        }

        private CameraConfigViewModel cameraConfigViewModel = new CameraConfigViewModel();

        /// <summary>
        /// 相机配置控件模型
        /// </summary>
        public CameraConfigViewModel CameraConfigViewModel
        {
            get
            {
                return cameraConfigViewModel;
            }
            set
            {
                cameraConfigViewModel = value;
                NotifyOfPropertyChange(() => CameraConfigViewModel);
            }
        }

        /// <summary>
        /// 相机图像
        /// </summary>
        private BitmapImage cameraImage;

        /// <summary>
        /// 相机图像
        /// </summary>
        public BitmapImage CameraImage
        {
            get
            {
                return cameraImage;
            }
            set
            {
                //cameraImage?.StreamSource?.Dispose();
                cameraImage = value;
                NotifyOfPropertyChange(() => CameraImage);
            }
        }

        /// <summary>
        /// 相机实例
        /// </summary>
        public ICamera Camera
        {
            get
            {
                return CameraConfigViewModel.Camera;
            }
            set
            {
                CameraConfigViewModel.Camera = value;
                NotifyOfPropertyChange(() => IsCameraValid);
                NotifyOfPropertyChange(() => Camera);
                NotifyOfPropertyChange(() => IsContinuesGrap);
            }
        }

        /// <summary>
        /// 当前连续拍照标志位
        /// </summary>
        public bool IsContinuesGrap
        {
            get
            {
                if ((Camera?.IsOpen == true) && (Camera.IsGrabbing == true) && (Camera?.TriggerMode == ETriggerModeStatus.Off))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion

        #region 事件

        /// <summary>
        /// 相机配置完成事件
        /// </summary>
        public event EventHandler<CameraConfigurationCompletedEventArgs> CameraConfigurationCompleted;

        internal void OnMessageRaised(MessageLevel messageLevel, string message, Exception exception = null)
        {
            MessageRaised?.Invoke(this, new MessageRaisedEventArgs(messageLevel, message, exception));
        }

        /// <summary>
        /// 消息触发事件
        /// </summary>
        internal event EventHandler<MessageRaisedEventArgs> MessageRaised;

        #endregion

        #region 方法

        /// <summary>
        /// 创建Bmp图像
        /// </summary>
        private Bitmap CreateBmpImage(EPixelFormatType ePixelFormat, int width, int height, IntPtr imagePtr)
        {
            Bitmap bitmap = null;

            try
            {
                //生成Bitmap(这一句不可以放到异步委托里面去,否则可能触发内存保护异常)
                if (ePixelFormat == EPixelFormatType.GVSP_PIX_MONO8)
                {
                    bitmap = BitmapAPI.CreateGrayBitmap(imagePtr, width, height);
                }
                else if (ePixelFormat == EPixelFormatType.GVSP_PIX_BAYGB8)
                {

                    bitmap = BitmapAPI.CreateGrayBitmap(imagePtr, width, height);
                }
                else if (ePixelFormat == EPixelFormatType.GVSP_PIX_BGR8_PACKED)
                {
                    bitmap = BitmapAPI.CreateRGBBitmap(imagePtr, width, height);
                }
                else if (ePixelFormat == EPixelFormatType.GVSP_PIX_BGRA8_PACKED)
                {
                    bitmap = BitmapAPI.CreateRGBABitmap(imagePtr, width, height);
                }
                else if (ePixelFormat == EPixelFormatType.GVSP_PIX_RGB8_PACKED)
                {
                    bitmap = BitmapAPI.CreateRGBBitmap(imagePtr, width, height);
                }
                else if (ePixelFormat == EPixelFormatType.GVSP_PIX_RGBA8_PACKED)
                {
                    bitmap = BitmapAPI.CreateRGBABitmap(imagePtr, width, height);
                }

            }
            catch
            {
                throw;
            }

            return bitmap;
        }

        /// <summary>
        /// Bitmap转BitmapImage
        /// </summary>
        /// <param name="bitmap">Bitmap格式图像</param>
        /// <returns>BitmapImage格式图像</returns>
        private BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            var bitmapImage = new BitmapImage();

            using (var ms = new System.IO.MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }

            return bitmapImage;
        }

        /// <summary>
        /// 图像采集完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraConfigViewModel_NewImageEvent(object sender, NewImageEventArgs e)
        {
            try
            {
                using (Bitmap bitmap = CreateBmpImage(e.ImageInfo.PixelFormat, e.ImageInfo.Width, e.ImageInfo.Height, e.ImageInfo.ImagePtr))
                {
                    if (bitmap != null)
                    {
                        BitmapImage bitmapToBitmapImage = BitmapToBitmapImage(bitmap);
                        CameraImage = bitmapToBitmapImage;
                    }

                }
            }
            finally
            {
                e.ImageInfo.DisposeImageIntPtr?.Invoke(e.ImageInfo.ImagePtr);

                //由于涉及到image控件的绘制,所以必须要手动GC,否则会存在内存泄漏的风险
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        /// <summary>
        /// 触发相机配置完成事件
        /// </summary>
        /// <param name="camera">相机实例</param>
        protected void OnCameraConfigurationCompleted(ICamera camera)
        {
            CameraConfigurationCompleted?.Invoke(this, new CameraConfigurationCompletedEventArgs(camera));
        }

        /// <summary>
        /// 更新相机
        /// </summary>
        public void UpdateCameras()
        {
            Cameras = new ObservableCollection<ICamera>(CameraFactory.Cameras.Values);

        }

        /// <summary>
        /// 设置相机
        /// </summary>
        public void SetCamera(ICamera camera)
        {
            Camera = camera;

            //更新配置文件
            UpdateCameraConfigFiles();

            if (!string.IsNullOrEmpty(CameraConfigFile))
            {
                LoadFromCurrentFile(CameraConfigFile);
            }

        }

        /// <summary>
        /// 单帧采集
        /// </summary>
        public void GrapOnce()
        {
            try
            {
                if (Camera?.IsOpen == true)
                {
                    Camera.TriggerMode = ETriggerModeStatus.On;
                    Camera?.TriggerSoftware();
                }
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
        }

        /// <summary>
        /// 连续采集
        /// </summary>
        public void ContinuesGrap()
        {
            try
            {
                if (Camera?.IsOpen == true)
                {
                    Camera.StopGrab();
                    Camera.TriggerMode = ETriggerModeStatus.Off;
                    Camera.Grab();
                }
                NotifyOfPropertyChange(() => IsContinuesGrap);
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
        }

        /// <summary>
        /// 停止连续采集
        /// </summary>
        public void StopContinuesGrap()
        {
            try
            {
                if ((Camera?.IsOpen == true) && (Camera?.TriggerMode == ETriggerModeStatus.Off))
                {
                    Camera.StopGrab();
                    Camera.TriggerMode = ETriggerModeStatus.On;

                    if (Camera.TriggerSource == ETriggerSource.Unknown)
                    {
                        Camera.TriggerSource = ETriggerSource.Software;
                    }
                    Camera.Grab();
                }
                NotifyOfPropertyChange(() => IsContinuesGrap);
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
        }

        /// <summary>
        /// 确认
        /// </summary>
        public void Accept(bool isAccpet)
        {
            if (isAccpet)
            {
                if ((Camera?.IsOpen == true) && (Camera?.TriggerMode == ETriggerModeStatus.Off))
                {
                    StopContinuesGrap();
                }

                OnCameraConfigurationCompleted(Camera);
            }
        }

        #endregion

        #region 相机文件配置

        private ObservableCollection<string> cameraConfigFiles;

        /// <summary>
        /// 相机配置文件列表
        /// </summary>
        public ObservableCollection<string> CameraConfigFiles
        {
            get
            {
                return cameraConfigFiles;
            }
            set
            {
                cameraConfigFiles = value;
                NotifyOfPropertyChange(() => CameraConfigFiles);
                NotifyOfPropertyChange(() => CameraConfigFile);
            }
        }

        private string cameraConfigFile;

        /// <summary>
        /// 相机配置文件列表
        /// </summary>
        public string CameraConfigFile
        {
            get
            {
                return cameraConfigFile;
            }
            set
            {
                cameraConfigFile = value;
                NotifyOfPropertyChange(() => CameraConfigFile);
            }
        }

        /// <summary>
        /// 更新相机配置文件
        /// </summary>
        public void UpdateCameraConfigFiles()
        {
            if (Camera?.IsOpen == true)
            {
                //获取相机配置文件
                FileInfo[] configFileInfos = CameraFactory.GetCameraConfigFiles(Camera?.Info.SerialNumber);
                CameraConfigFiles = new ObservableCollection<string>(configFileInfos.ToList().ConvertAll(x => x.Name));
            }

        }

        /// <summary>
        /// 保存到当前文件
        /// </summary>
        /// <param name="fileName">配置文件名(不包含路径)</param>
        public void SaveToCurrentFile(string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    throw new ArgumentException("文件名无效");
                }

                if (Camera?.IsOpen == true)
                {
                    string file = $"VisionPlatform/Camera/CameraConfig/{Camera.Info.SerialNumber}/ConfigFile/{Path.GetFileNameWithoutExtension(fileName)}.json";
                    SaveToFile(file);
                }
                else
                {
                    OnMessageRaised(MessageLevel.Warning, "相机无效");
                }
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }

        }

        /// <summary>
        /// 保存到文件中
        /// </summary>
        /// <param name="file">文件路径(全路径)</param>
        public void SaveToFile(string file)
        {
            try
            {
                if (string.IsNullOrEmpty(file))
                {
                    throw new ArgumentException("文件名无效");
                }

                if (Camera?.IsOpen == true)
                {
                    var cameraConfigParam = new CameraConfigParam();

                    cameraConfigParam.PixelFormat = Camera.PixelFormat;
                    cameraConfigParam.TriggerMode = Camera.TriggerMode;
                    cameraConfigParam.TriggerSource = Camera.TriggerSource;
                    cameraConfigParam.TriggerActivation = Camera.TriggerActivation;
                    cameraConfigParam.ExposureTime = Camera.ExposureTime;
                    cameraConfigParam.Gain = Camera.Gain;

                    JsonSerialization.SerializeObjectToFile(cameraConfigParam, file);
                }
                else
                {
                    OnMessageRaised(MessageLevel.Warning, "相机无效");
                }
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
        }

        /// <summary>
        /// 从当前文件中加载
        /// </summary>
        /// <param name="fileName">配置文件名(不包含路径)</param>
        public void LoadFromCurrentFile(string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    throw new ArgumentException("无效路径/文件不存在");
                }

                if (Camera?.IsOpen == true)
                {
                    string file = $"VisionPlatform/Camera/CameraConfig/{Camera.Info.SerialNumber}/ConfigFile/{Path.GetFileNameWithoutExtension(fileName)}.json";
                    LoadFromFile(file);
                }
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <param name="file">文件路径</param>
        public void LoadFromFile(string file)
        {
            try
            {
                if (string.IsNullOrEmpty(file))
                {
                    throw new ArgumentException("无效路径/文件不存在");
                }

                CameraConfigParam cameraConfigParam = JsonSerialization.DeserializeObjectFromFile<CameraConfigParam>(file);

                if (Camera?.IsOpen == true)
                {
                    CameraFactory.ConfigurateCamera(Camera.Info.SerialNumber, cameraConfigParam);
                }
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
            finally
            {
                if (CameraConfigViewModel?.Camera?.IsOpen == true)
                {
                    Camera.Grab();

                    //刷新控件显示
                    Camera = Camera;
                }
            }
        }

        /// <summary>
        /// 更新当前配置
        /// </summary>
        public void UpdateConfig(string fileName)
        {
            if (Camera?.IsOpen == true)
            {
                string file = $"VisionPlatform/Camera/CameraConfig/{Camera.Info.SerialNumber}/ConfigFile/{Path.GetFileNameWithoutExtension(fileName)}.json";

                if (File.Exists(file))
                {
                    LoadFromFile(file);
                }
            }
        }

        /// <summary>
        /// 删除当前文件
        /// </summary>
        /// <param name="fileName"></param>
        public void DeleteCurrentFile(string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    throw new ArgumentException("无效路径/文件不存在");
                }

                if (Camera?.IsOpen == true)
                {
                    string file = $"VisionPlatform/Camera/CameraConfig/{Camera.Info.SerialNumber}/ConfigFile/{Path.GetFileNameWithoutExtension(fileName)}.json";

                    if (File.Exists(file))
                    {
                        File.Delete(file);
                        UpdateCameraConfigFiles();
                    }
                    else
                    {
                        throw new FileNotFoundException($"{nameof(file)}:{file}");
                    }
                }
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
        }
        #endregion

    }
}
