using Caliburn.Micro;
using Framework.Camera;
using Framework.Infrastructure.Image;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Media.Imaging;
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

                return false; ;
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
                using (var bitmap = CreateBmpImage(e.ImageInfo.PixelFormat, e.ImageInfo.Width, e.ImageInfo.Height, e.ImageInfo.ImagePtr))
                {
                    if (bitmap != null)
                    {
                        var bitmapToBitmapImage = BitmapToBitmapImage(bitmap);
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
            CameraConfigViewModel.Camera = camera;
            NotifyOfPropertyChange(() => IsCameraValid);
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadFromFile()
        {

        }

        #endregion

    }
}
