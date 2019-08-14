using Caliburn.Micro;
using Framework.Camera;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionPlatform.Wpf
{
    /// <summary>
    /// 相机配置控件模型
    /// </summary>
    public class CameraConfigViewModel : Screen
    {
        #region 构造函数

        /// <summary>
        /// 创建CameraConfigViewModel新实例
        /// </summary>
        public CameraConfigViewModel()
        {

        }

        /// <summary>
        /// 创建CameraConfigViewModel新实例
        /// </summary>
        /// <param name="camera">相机实例</param>
        public CameraConfigViewModel(ICamera camera)
        {
            Camera = camera;
        }

        #endregion

        #region 属性

        private ICamera camera;

        /// <summary>
        /// 相机接口
        /// </summary>
        public ICamera Camera
        {
            get
            {
                return camera;
            }
            set
            {
                if (camera != null)
                {
                    camera.NewImageEvent -= Camera_NewImageEvent;
                }

                camera = value;

                if (camera != null)
                {
                    camera.NewImageEvent += Camera_NewImageEvent;
                }

                //读取相机参数列表
                ReadCameraParamList();

                //更新相机参数
                UpdateCameraParam();
            }
        }

        #region 相机参数

        /// <summary>
        /// 相机名
        /// </summary>
        public string CameraName
        {
            get
            {
                return Camera?.ToString() ?? "";
            }
        }

        /// <summary>
        /// 像素类型列表
        /// </summary>
        private ObservableCollection<EPixelFormatType> pixelFormatList = new ObservableCollection<EPixelFormatType>();

        /// <summary>
        /// 像素类型列表
        /// </summary>
        public ObservableCollection<EPixelFormatType> PixelFormatList
        {
            get
            {
                return pixelFormatList;
            }
            set
            {
                pixelFormatList = value;
                NotifyOfPropertyChange(() => PixelFormatList);
            }
        }

        /// <summary>
        /// 触发模式列表
        /// </summary>
        private ObservableCollection<ETriggerModeStatus> triggerModeList = new ObservableCollection<ETriggerModeStatus>();

        /// <summary>
        /// 触发模式列表
        /// </summary>
        public ObservableCollection<ETriggerModeStatus> TriggerModeList
        {
            get
            {
                return triggerModeList;
            }
            set
            {
                triggerModeList = value;
                NotifyOfPropertyChange(() => TriggerModeList);
            }
        }

        /// <summary>
        /// 触发模式列表
        /// </summary>
        private ObservableCollection<ETriggerSource> triggerSourceList = new ObservableCollection<ETriggerSource>();

        /// <summary>
        /// 触发模式列表
        /// </summary>
        public ObservableCollection<ETriggerSource> TriggerSourceList
        {
            get
            {
                return triggerSourceList;
            }
            set
            {
                triggerSourceList = value;
                NotifyOfPropertyChange(() => TriggerSourceList);
            }
        }

        /// <summary>
        /// 硬件有效触发信号列表
        /// </summary>
        private ObservableCollection<ETriggerActivation> triggerActivationList = new ObservableCollection<ETriggerActivation>();

        /// <summary>
        /// 硬件有效触发信号列表
        /// </summary>
        public ObservableCollection<ETriggerActivation> TriggerActivationList
        {
            get
            {
                return triggerActivationList;
            }
            set
            {
                triggerActivationList = value;
                NotifyOfPropertyChange(() => TriggerActivationList);
            }
        }

        /// <summary>
        /// 像素格式
        /// </summary>
        public EPixelFormatType PixelFormat
        {
            get
            {
                return Camera?.PixelFormat ?? EPixelFormatType.Unknown;
            }
            set
            {
                if (Camera != null)
                {
                    if (Camera.IsGrabbing)
                    {
                        Camera.StopGrab();
                        Camera.PixelFormat = value;
                        Camera.Grab();
                    }
                    else
                    {
                        Camera.PixelFormat = value;
                    }
                }
                UpdateCameraParam();
            }
        }

        /// <summary>
        /// 触发模式
        /// </summary>
        public ETriggerModeStatus TriggerMode
        {
            get
            {
                return Camera?.TriggerMode ?? ETriggerModeStatus.Unknown;
            }
            set
            {
                if (Camera != null)
                {
                    if (Camera.IsGrabbing)
                    {
                        Camera.StopGrab();
                        Camera.TriggerMode = value;
                        Camera.Grab();
                    }
                    else
                    {
                        Camera.TriggerMode = value;
                    }
                }
                UpdateCameraParam();
            }
        }

        /// <summary>
        /// 触发源
        /// </summary>
        public ETriggerSource TriggerSource
        {
            get
            {
                return Camera?.TriggerSource ?? ETriggerSource.Unknown;
            }
            set
            {
                if (Camera != null)
                {
                    if (Camera.IsGrabbing)
                    {
                        Camera.StopGrab();
                        Camera.TriggerSource = value;
                        Camera.Grab();
                    }
                    else
                    {
                        Camera.TriggerSource = value;
                    }
                }
                UpdateCameraParam();
            }
        }

        /// <summary>
        /// 硬件有效触发信号
        /// </summary>
        public ETriggerActivation TriggerActivation
        {
            get
            {
                return Camera?.TriggerActivation ?? ETriggerActivation.Unknown;
            }
            set
            {
                if (Camera != null)
                {
                    if (Camera.IsGrabbing)
                    {
                        Camera.StopGrab();
                        Camera.TriggerActivation = value;
                        Camera.Grab();
                    }
                    else
                    {
                        Camera.TriggerActivation = value;
                    }
                }
                UpdateCameraParam();
            }
        }

        /// <summary>
        /// 宽度
        /// </summary>
        public int Width
        {
            get
            {
                return (int?)Camera?.Width ?? -1;
            }
            set
            {
                if (Camera != null)
                {
                    if (Camera.IsGrabbing)
                    {
                        Camera.StopGrab();
                        Camera.Width = value;
                        Camera.Grab();
                    }
                    else
                    {
                        Camera.Width = value;
                    }
                }
                UpdateCameraParam();
            }
        }

        /// <summary>
        /// 高度
        /// </summary>
        public int Height
        {
            get
            {
                return (int?)Camera?.Height ?? -1;
            }
            set
            {
                if (Camera != null)
                {
                    if (Camera.IsGrabbing)
                    {
                        Camera.StopGrab();
                        Camera.Height = value;
                        Camera.Grab();
                    }
                    else
                    {
                        Camera.Height = value;
                    }
                }
                UpdateCameraParam();
            }
        }

        /// <summary>
        /// 曝光时间
        /// </summary>
        public double ExposureTime
        {
            get
            {
                return Camera?.ExposureTime ?? -1;
            }
            set
            {
                if (Camera != null)
                {
                    Camera.ExposureTime = value;
                }
                UpdateCameraParam();
            }
        }

        /// <summary>
        /// 增益
        /// </summary>
        public double Gain
        {
            get
            {
                return Camera?.Gain ?? -1;
            }
            set
            {
                if (Camera != null)
                {
                    Camera.Gain = value;
                }
                UpdateCameraParam();
            }
        }

        #endregion

        #endregion

        #region 事件

        /// <summary>
        /// 图像采集完成事件
        /// </summary>
        public event EventHandler<NewImageEventArgs> NewImageEvent;

        /// <summary>
        /// 触发图像采集完成事件
        /// </summary>
        /// <param name="imageInfo">图像信息</param>
        protected void OnNewImageEvent(ImageInfo imageInfo)
        {
            NewImageEvent?.Invoke(this, new NewImageEventArgs(imageInfo));
        }

        #endregion

        #region 方法

        /// <summary>
        /// 读取相机参数
        /// </summary>
        private void ReadCameraParamList()
        {
            if (Camera?.IsOpen == true)
            {
                //获取像素类型列表
                if (Camera.PixelFormatTypeEnum != null)
                {
                    //设置列表
                    if (PixelFormatList?.Count == 0)
                    {
                        foreach (var item in Camera.PixelFormatTypeEnum)
                        {
                            PixelFormatList.Add(item);
                        }
                    }
                }

                //获取触发模式列表
                if (Camera.TriggerModeEnum != null)
                {
                    //设置列表
                    if (TriggerModeList?.Count == 0)
                    {
                        foreach (var item in Camera.TriggerModeEnum)
                        {
                            TriggerModeList.Add(item);
                        }
                    }
                }

                //获取触发源列表
                if (Camera.TriggerSourceEnum != null)
                {
                    //设置列表
                    if (TriggerSourceList?.Count == 0)
                    {
                        foreach (var item in Camera.TriggerSourceEnum)
                        {
                            TriggerSourceList.Add(item);
                        }
                    }
                }

                //获取硬件有效触发列表
                if (Camera.TriggerActivationEnum != null)
                {
                    //设置列表
                    if (TriggerActivationList?.Count == 0)
                    {
                        foreach (var item in Camera.TriggerActivationEnum)
                        {
                            TriggerActivationList.Add(item);
                        }
                    }
                }

            }
            else
            {
                PixelFormatList.Clear();
                TriggerModeList.Clear();
                TriggerSourceList.Clear();
                TriggerActivationList.Clear();
            }
        }

        /// <summary>
        /// 更新相机参数
        /// </summary>
        private void UpdateCameraParam()
        {
            NotifyOfPropertyChange(() => CameraName);
            NotifyOfPropertyChange(() => PixelFormat);
            NotifyOfPropertyChange(() => TriggerMode);
            NotifyOfPropertyChange(() => TriggerSource);
            NotifyOfPropertyChange(() => TriggerActivation);
            NotifyOfPropertyChange(() => Width);
            NotifyOfPropertyChange(() => Height);
            NotifyOfPropertyChange(() => ExposureTime);
            NotifyOfPropertyChange(() => Gain);
        }

        /// <summary>
        /// 软触发
        /// </summary>
        public void TriggerSoftware()
        {
            Camera?.TriggerSoftware();

            UpdateCameraParam();
        }

        private void Camera_NewImageEvent(object sender, NewImageEventArgs e)
        {
            OnNewImageEvent(e.ImageInfo);
        }

        /// <summary>
        /// 卸载
        /// </summary>
        public void Unloaded()
        {
            if (camera != null)
            {
                camera.NewImageEvent -= Camera_NewImageEvent;
            }
        }

        #endregion

    }
}
