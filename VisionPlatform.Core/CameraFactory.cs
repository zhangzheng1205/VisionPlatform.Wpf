using Framework.Camera;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VisionPlatform.Core
{
    /// <summary>
    /// 相机工厂静态类
    /// </summary>
    public static class CameraFactory
    {
        #region 相机集合管理

        /// <summary>
        /// 相机DLL根目录
        /// </summary>
        public static string CameraDllRootPath { get; } = "VisionPlatform/Camera/CameraSDK";

        /// <summary>
        /// 相机集合字典
        /// </summary>
        public static Dictionary<string, Assembly> CameraAssemblys { get; private set; } = new Dictionary<string, Assembly>();

        /// <summary>
        /// 静态构造
        /// </summary>
        static CameraFactory()
        {
            UpdateAssembly();
        }

        /// <summary>
        /// 更新集合
        /// </summary>
        public static void UpdateAssembly()
        {
            CameraAssemblys.Clear();

            //遍历目录
            if (Directory.Exists(CameraDllRootPath))
            {
                var directoryInfo = new DirectoryInfo(CameraDllRootPath);

                foreach (var item in directoryInfo.GetDirectories())
                {
                    //获取集合
                    var dllPath = $"{CameraDllRootPath}/{item.Name}/Framework.Camera.{item.Name}.dll";

                    if (File.Exists(dllPath))
                    {
                        var assembly = Assembly.LoadFrom(dllPath);

                        //将dll添加到集合字典中
                        CameraAssemblys.Add(item.Name, assembly);
                    }

                }
            }
        }

        /// <summary>
        /// 创建相机实例
        /// </summary>
        /// <param name="name">相机接口名</param>
        /// <returns>相机实例</returns>
        public static ICamera CreateInstance(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("name cannot be null");
            }

            try
            {
                if (CameraAssemblys.ContainsKey(name))
                {
                    //创建视觉框架实例
                    foreach (var item in CameraAssemblys[name].ExportedTypes)
                    {
                        if (item.Name == "Camera")
                        {
                            object obj = CameraAssemblys[name].CreateInstance(item.FullName);

                            if (obj is ICamera)
                            {
                                return obj as ICamera;
                            }
                        }
                    }
                }

                throw new FileNotFoundException($"{nameof(name)} is not found");
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region 相机资源管理

        /// <summary>
        /// 相机集合名
        /// </summary>
        public static string CameraAssemblyName { get; set; }

        /// <summary>
        /// 相机列表
        /// </summary>
        public static Dictionary<string, ICamera> Cameras { get; } = new Dictionary<string, ICamera>();


        /// <summary>
        /// 获取所有的相机
        /// </summary>
        /// <returns>相机信息列表</returns>
        public static List<DeviceInfo> GetAllCameras()
        {
            if (string.IsNullOrEmpty(CameraAssemblyName) || !CameraAssemblys.ContainsKey(CameraAssemblyName))
            {
                throw new ArgumentException("CameraAssemblyName invalid");
            }

            return CreateInstance(CameraAssemblyName).GetDeviceList();
        }

        /// <summary>
        /// 添加相机
        /// </summary>
        /// <param name="cameraSerial">相机序列号</param>
        /// <exception cref="InvalidOperationException">
        /// 打开相机失败
        /// </exception>
        public static void AddCamera(string cameraSerial)
        {
            //校验相机SDK名是否有效
            if (string.IsNullOrEmpty(CameraAssemblyName))
            {
                throw new ArgumentException("CameraAssemblyName cannot be null");
            }

            if (string.IsNullOrEmpty(cameraSerial))
            {
                throw new ArgumentException("cameraSerial cannot be null");
            }

            //如果当前相机已经注册了,则直接返回
            if (Cameras.ContainsKey(cameraSerial))
            {
                return;
            }

            ICamera camera = CreateInstance(CameraAssemblyName);

            if (camera.Connect(cameraSerial))
            {
                Cameras.Add(cameraSerial, camera);
                return;
            }
            else
            {
                throw new InvalidOperationException($"open camera[{cameraSerial}] err!");
            }
        }

        /// <summary>
        /// 添加所有的相机
        /// </summary>
        /// <returns>执行结果</returns>
        public static void AddAllCamera()
        {
            //校验相机SDK名是否有效
            if (string.IsNullOrEmpty(CameraAssemblyName))
            {
                throw new ArgumentException("CameraAssemblyName cannot be null");
            }

            try
            {
                //添加所有的相机
                var devices = GetAllCameras();
                foreach (var item in devices)
                {
                    AddCamera(item.SerialNumber);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 移除相机
        /// </summary>
        /// <param name="cameraSerial">相机序列号</param>
        public static void RemoveCamera(string cameraSerial)
        {
            if (Cameras.ContainsKey(cameraSerial))
            {
                //关闭相机
                Cameras[cameraSerial].StopGrab();
                Cameras[cameraSerial].DisConnect();
                Cameras[cameraSerial].Dispose();

                //从列表中移除
                Cameras.Remove(cameraSerial);
            }
        }

        /// <summary>
        /// 移除所有的相机
        /// </summary>
        public static void RemoveAllCameras()
        {
            foreach (var item in Cameras.Values)
            {
                item.StopGrab();
                item.DisConnect();
                item.Dispose();
            }

            Cameras.Clear();
        }

        /// <summary>
        /// 设置到默认配置
        /// </summary>
        /// <param name="camera"></param>
        private static void SetToDefaultConfiguration(ICamera camera)
        {
            if (camera?.IsOpen != true)
            {
                throw new ArgumentException("camera is invalid");
            }

            if (!camera.IsGrabbing)
            {
                camera.TriggerMode = ETriggerModeStatus.On;
                camera.TriggerSource = ETriggerSource.Software;

                if (camera.PixelFormatTypeEnum?.Contains(EPixelFormatType.GVSP_PIX_MONO8) == true)
                {
                    camera.PixelFormat = EPixelFormatType.GVSP_PIX_MONO8;
                }
                
                camera.Grab();
            }
        }

        /// <summary>
        /// 获取相机接口实例
        /// </summary>
        /// <param name="cameraSerial">相机序列号</param>
        /// <returns>相机接口实例</returns>
        /// <exception cref="InvalidOperationException">
        /// 打开相机失败
        /// </exception>
        public static ICamera GetCamera(string cameraSerial)
        {
            try
            {
                if (Cameras.ContainsKey(cameraSerial))
                {
                    SetToDefaultConfiguration(Cameras[cameraSerial]);
                    return Cameras[cameraSerial];
                }
                else
                {
                    AddCamera(cameraSerial);

                    if (Cameras.ContainsKey(cameraSerial))
                    {
                        SetToDefaultConfiguration(Cameras[cameraSerial]);
                        return Cameras[cameraSerial];
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }

        #endregion
    }
}
