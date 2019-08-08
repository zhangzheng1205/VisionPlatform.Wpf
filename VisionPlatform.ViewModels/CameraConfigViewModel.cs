using Caliburn.Micro;
using Framework.Camera;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Framework.Infrastructure.Serialization;

namespace VisionPlatform.ViewModels
{
    public class CameraConfigViewModel : Screen
    {
        #region 构造函数

        /// <summary>
        /// 创建CameraConfigViewModel新实例
        /// </summary>
        public CameraConfigViewModel() : this(null)
        {

        }

        /// <summary>
        /// 创建CameraConfigViewModel新实例
        /// </summary>
        /// <param name="camera"></param>
        public CameraConfigViewModel(ICamera camera)
        {
            Camera = camera;

            //初始化参数
            PixelFormatList = new ObservableCollection<EPixelFormatType>();
            TriggerModeList = new ObservableCollection<ETriggerModeStatus>();
            TriggerSourceList = new ObservableCollection<ETriggerSource>();
            TriggerActivationList = new ObservableCollection<ETriggerActivation>();

            //读取相机参数列表
            ReadCameraParamList();

            //使能所有的控件
            EnableAllProperty();

        }

        #endregion

        #region 字段

        /// <summary>
        /// 配置参数
        /// </summary>
        private Dictionary<string, object> configParams = new Dictionary<string, object>();

        #endregion

        #region 属性

        /// <summary>
        /// 相机接口
        /// </summary>
        public ICamera Camera { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        private string filePath = "";

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath
        {
            get
            {
                return filePath;
            }
            set
            {
                filePath = value;

                NotifyOfPropertyChange(() => FilePath);
            }
        }

        #region 使能

        /// <summary>
        /// 使能按键可见性
        /// </summary>
        private Visibility toggleButtonVisibility;

        /// <summary>
        /// 使能按键可见性
        /// </summary>
        public Visibility ToggleButtonVisibility
        {
            get
            {
                return toggleButtonVisibility;
            }
            set
            {
                toggleButtonVisibility = value;

                //若隐藏使能按键,则使能所有控件
                if (toggleButtonVisibility != Visibility.Visible)
                {
                    EnableAllProperty();
                }

                NotifyOfPropertyChange(() => ToggleButtonVisibility);
            }
        }

        /// <summary>
        /// 禁止所有属性标志
        /// </summary>
        private bool isDisableAll = false;

        /// <summary>
        /// 像素格式使能
        /// </summary>
        private bool isPixelFormatEnable;

        /// <summary>
        /// 像素格式使能
        /// </summary>
        public bool IsPixelFormatEnable
        {
            get
            {
                return isPixelFormatEnable;
            }
            set
            {
                isPixelFormatEnable = value;
                NotifyOfPropertyChange(() => IsPixelFormatEnable);
            }
        }

        /// <summary>
        /// 宽度使能
        /// </summary>
        private bool isWidthEnable;

        /// <summary>
        /// 宽度使能
        /// </summary>
        public bool IsWidthEnable
        {
            get
            {
                return isWidthEnable;
            }
            set
            {
                isWidthEnable = value;
                NotifyOfPropertyChange(() => IsWidthEnable);
            }
        }

        /// <summary>
        /// 高度使能
        /// </summary>
        private bool isHeihtEnable;

        /// <summary>
        /// 高度使能
        /// </summary>
        public bool IsHeihtEnable
        {
            get
            {
                return isHeihtEnable;
            }
            set
            {
                isHeihtEnable = value;
                NotifyOfPropertyChange(() => IsHeihtEnable);
            }
        }

        /// <summary>
        /// 触发模式使能
        /// </summary>
        private bool isTriggerModeEnable;

        /// <summary>
        /// 触发模式使能
        /// </summary>
        public bool IsTriggerModeEnable
        {
            get
            {
                return isTriggerModeEnable;
            }
            set
            {
                isTriggerModeEnable = value;
                NotifyOfPropertyChange(() => IsTriggerModeEnable);
            }
        }

        /// <summary>
        /// 触发源使能
        /// </summary>
        private bool isTriggerSourceEnable;

        /// <summary>
        /// 触发源使能
        /// </summary>
        public bool IsTriggerSourceEnable
        {
            get
            {
                return isTriggerSourceEnable;
            }
            set
            {
                isTriggerSourceEnable = value;
                NotifyOfPropertyChange(() => IsTriggerSourceEnable);
            }
        }

        /// <summary>
        /// 硬件有效触发信号使能
        /// </summary>
        private bool isTriggerActivationEnable;

        /// <summary>
        /// 硬件有效触发信号使能
        /// </summary>
        public bool IsTriggerActivationEnable
        {
            get
            {
                return isTriggerActivationEnable;
            }
            set
            {
                isTriggerActivationEnable = value;
                NotifyOfPropertyChange(() => IsTriggerActivationEnable);
            }
        }

        /// <summary>
        /// 曝光值使能
        /// </summary>
        private bool isExposureTimeEnable;

        /// <summary>
        /// 曝光值使能
        /// </summary>
        public bool IsExposureTimeEnable
        {
            get
            {
                return isExposureTimeEnable;
            }
            set
            {
                isExposureTimeEnable = value;
                NotifyOfPropertyChange(() => IsExposureTimeEnable);
            }
        }

        /// <summary>
        /// 增益值使能
        /// </summary>
        private bool isGainEnable;

        /// <summary>
        /// 增益值使能
        /// </summary>
        public bool IsGainEnable
        {
            get
            {
                return isGainEnable;
            }
            set
            {
                isGainEnable = value;
                NotifyOfPropertyChange(() => IsGainEnable);
            }
        }

        #endregion

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
            set
            {
                NotifyOfPropertyChange(() => CameraName);
            }
        }

        /// <summary>
        /// 像素类型列表
        /// </summary>
        private ObservableCollection<EPixelFormatType> pixelFormatList;

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
        private ObservableCollection<ETriggerModeStatus> triggerModeList;

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
        private ObservableCollection<ETriggerSource> triggerSourceList;

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
        private ObservableCollection<ETriggerActivation> triggerActivationList;

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
        /// 完成相机配置事件
        /// </summary>
        public event EventHandler<CameraConfigurationCompletedEventArgs> CameraConfigurationCompleted;

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
        /// 使能所有的属性
        /// </summary>
        public void EnableAllProperty()
        {
            IsPixelFormatEnable = true;
            IsWidthEnable = true;
            IsHeihtEnable = true;
            IsTriggerModeEnable = true;
            IsTriggerSourceEnable = true;
            IsTriggerActivationEnable = true;
            IsExposureTimeEnable = true;
            IsGainEnable = true;

            isDisableAll = false;
        }

        /// <summary>
        /// 禁止所有的属性
        /// </summary>
        public void DisableAllProperty()
        {
            IsPixelFormatEnable = false;
            IsWidthEnable = false;
            IsHeihtEnable = false;
            IsTriggerModeEnable = false;
            IsTriggerSourceEnable = false;
            IsTriggerActivationEnable = false;
            IsExposureTimeEnable = false;
            IsGainEnable = false;

            isDisableAll = true;
        }

        /// <summary>
        /// 软触发
        /// </summary>
        public void TriggerSoftware()
        {
            Camera?.TriggerSoftware();
        }

        /// <summary>
        /// 全选/全不选
        /// </summary>
        public void ResetOrSelectAll()
        {
            if (isDisableAll)
            {
                EnableAllProperty();
            }
            else
            {
                DisableAllProperty();
            }

        }

        /// <summary>
        /// 触发完成相机配置事件
        /// </summary>
        protected void OnCameraConfigurationCompleted(ICamera camera, Dictionary<string, object> configParams, string filePath)
        {
            CameraConfigurationCompleted?.Invoke(this, new CameraConfigurationCompletedEventArgs(camera, configParams, filePath));
        }

        /// <summary>
        /// 保存
        /// </summary>
        public void AcceptCameraConfig()
        {
            if (!string.IsNullOrWhiteSpace(FilePath))
            {
                configParams.Clear();
                //configParams.Add("PixelFormat", PixelFormat);
                //configParams.Add("TriggerSource", TriggerSource);
                //configParams.Add("TriggerActivation", TriggerActivation);
                configParams.Add("ExposureTime", ExposureTime);
                configParams.Add("Gain", Gain);

                JsonSerialization.SerializeObjectToFile(configParams, FilePath);

                OnCameraConfigurationCompleted(Camera, configParams, FilePath);
            }

        }

        /// <summary>
        /// 加载文件
        /// </summary>
        /// <param name="FilePath"></param>
        public void LoadFile(string filePath)
        {
            try
            {
                FilePath = filePath;
                configParams = JsonSerialization.DeserializeObjectFromFile<Dictionary<string, object>>(filePath);

                if (configParams != null)
                {
                    foreach (var item in configParams)
                    {
                        try
                        {
                            var property = this.GetType().GetProperty(item.Key);

                            object value;

                            if (property.PropertyType.IsEnum)
                            {
                                value = Enum.ToObject(property.PropertyType, item.Value);
                            }
                            else
                            {
                                value = item.Value;
                            }
                            
                            property.SetValue(this, value);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }

                

            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="filePath"></param>
        public void CreateFile(string filePath)
        {
            try
            {
                FilePath = filePath;
            }
            catch (Exception)
            {

            }
        }

        #endregion

    }
}
