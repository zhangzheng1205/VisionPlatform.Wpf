using Caliburn.Micro;
using Framework.Camera;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using VisionPlatform.BaseType;

namespace VisionPlatform.ViewModels
{
    public class CameraSelectViewModel : Screen
    {
        #region 构造函数

        /// <summary>
        /// 创建CameraSelectViewModel新实例
        /// </summary>
        public CameraSelectViewModel() : this(null)
        {

        }

        /// <summary>
        /// 创建CameraSelectViewModel新实例
        /// </summary>
        /// <param name="camera"></param>
        public CameraSelectViewModel(ICamera camera)
        {
            Camera = camera;

            IsSreaching = false;
            CameraList = new ObservableCollection<ItemBase>();
            CameraInfoList = new ObservableCollection<ItemBase>();
            DisplayCameraInfo(null);
        }

        #endregion

        #region 属性

        /// <summary>
        /// 相机接口
        /// </summary>
        public ICamera Camera { get; set; }

        /// <summary>
        /// 相机列表
        /// </summary>
        private ObservableCollection<ItemBase> cameraList;

        /// <summary>
        /// 相机列表
        /// </summary>
        public ObservableCollection<ItemBase> CameraList
        {
            get
            {
                return cameraList;
            }
            set
            {
                cameraList = value;
                NotifyOfPropertyChange(() => CameraList);
            }
        }

        /// <summary>
        /// 相机列表
        /// </summary>
        private ObservableCollection<ItemBase> cameraInfoList;

        /// <summary>
        /// 相机列表
        /// </summary>
        public ObservableCollection<ItemBase> CameraInfoList
        {
            get
            {
                return cameraInfoList;
            }
            set
            {
                cameraInfoList = value;
                NotifyOfPropertyChange(() => CameraInfoList);
            }
        }

        /// <summary>
        /// 正在搜寻标志
        /// </summary>
        private bool isSreaching;

        /// <summary>
        /// 正在搜寻标志
        /// </summary>
        public bool IsSreaching
        {
            get
            {
                return isSreaching;
            }
            set
            {
                isSreaching = value;
                NotifyOfPropertyChange(() => IsSreaching);
            }
        }

        /// <summary>
        /// 选择项有效标志
        /// </summary>
        private bool isSelectionValid;

        /// <summary>
        /// 选择项有效标志
        /// </summary>
        public bool IsSelectionValid
        {
            get
            {
                return isSelectionValid;
            }
            set
            {
                isSelectionValid = value;
                NotifyOfPropertyChange(() => IsSelectionValid);
            }
        }

        #endregion

        #region 事件

        /// <summary>
        /// 相机选择事件
        /// </summary>
        public event EventHandler<CameraSelectedEventArgs> CameraSelected;

        /// <summary>
        /// 取消事件
        /// </summary>
        public event EventHandler<EventArgs> Canceled;

        #endregion

        #region 方法

        /// <summary>
        /// 显示相机信息
        /// </summary>
        /// <param name="Device">设备信息</param>
        private void DisplayCameraInfo(DeviceInfo devInfo)
        {
            CameraInfoList.Clear();

            CameraInfoList.Add(new ItemBase("MAC地址", devInfo?.MACAddress ?? "----"));
            CameraInfoList.Add(new ItemBase("IP地址", devInfo?.IPAddress ?? "----"));
            CameraInfoList.Add(new ItemBase("子网掩码", devInfo?.SubnetMask ?? "----"));
            CameraInfoList.Add(new ItemBase("默认网关", devInfo?.GatewayAddress ?? "----"));
            CameraInfoList.Add(new ItemBase("模块名", devInfo?.ModelName ?? "----"));
            CameraInfoList.Add(new ItemBase("生产商", devInfo?.Manufacturer ?? "----"));
            CameraInfoList.Add(new ItemBase("序列号", devInfo?.SerialNumber ?? "----"));
            CameraInfoList.Add(new ItemBase("自定义名", devInfo?.UserName ?? "----"));

        }

        /// <summary>
        /// 显示相机信息
        /// </summary>
        /// <param name="index">索引</param>
        public void DisplayCameraInfo(int index)
        {
            if ((index >= 0) && (index < CameraList.Count))
            {
                IsSelectionValid = true;
                DisplayCameraInfo(CameraList[index].Value as DeviceInfo);
            }
            else
            {
                IsSelectionValid = false;
            }
        }

        /// <summary>
        /// 更新相机列表
        /// </summary>
        public void UpdateCameraList()
        {
            IsSreaching = true;
            CameraList.Clear();
            DisplayCameraInfo(null);

            new Thread(delegate ()
            {
                // 开始搜索
                var cameraList = Camera?.GetDeviceList();

                ThreadPool.QueueUserWorkItem(delegate
                {
                    System.Threading.SynchronizationContext.SetSynchronizationContext(new System.Windows.Threading.DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));
                    System.Threading.SynchronizationContext.Current.Send(pl =>
                    {
                        // 更新控件
                        if (cameraList != null)
                        {
                            CameraList.Clear();

                            foreach (var item in cameraList)
                            {
                                CameraList.Add(new ItemBase(item.ToString(), item));
                            }
                        }

                        IsSreaching = false;
                    }, null);
                });
            }).Start();

        }

        /// <summary>
        /// 触发相机选择事件
        /// </summary>
        /// <param name="device">选择的相机</param>
        protected void OnCameraSelected(DeviceInfo device)
        {
            CameraSelected?.Invoke(this, new CameraSelectedEventArgs(device));
        }

        /// <summary>
        /// 选择相机
        /// </summary>
        /// <param name="index">相机索引</param>
        public void SelectedCamera(int index)
        {
            if ((index >= 0) && (index < CameraList.Count) && (CameraList[index].Value is DeviceInfo))
            {
                OnCameraSelected(CameraList[index].Value as DeviceInfo);
            }
        }

        /// <summary>
        /// 触发取消事件
        /// </summary>
        protected void OnCanceled()
        {
            Canceled?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// 取消
        /// </summary>
        public void Cancel()
        {
            OnCanceled();
        }

        #endregion

    }
}
