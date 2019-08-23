using Framework.Camera;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using VisionPlatform.BaseType;

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
        public static Dictionary<ECameraSdkType, Assembly> CameraAssemblys { get; private set; } = new Dictionary<ECameraSdkType, Assembly>();

        /// <summary>
        /// 静态构造
        /// </summary>
        static CameraFactory()
        {
            UpdateAssembly();
        }

        /// <summary>
        /// 目录名转换为ECameraSdkType
        /// </summary>
        /// <param name="directoryName">目录名</param>
        /// <returns>ECameraSdkType</returns>
        private static ECameraSdkType ConvertToECameraSDK(string directoryName)
        {
            switch (directoryName.ToLower())
            {
                case "pylon": return ECameraSdkType.Pylon;
                case "ueyesdk": return ECameraSdkType.uEye;
                case "hik": return ECameraSdkType.Hik;
                case "commonsdk": return ECameraSdkType.Common;
                case "cognex": return ECameraSdkType.Cognex;
                case "daheng": return ECameraSdkType.Daheng;
                case "dalsa": return ECameraSdkType.DALSA;
                case "imagingsource": return ECameraSdkType.ImagingSource;
                case "virtualcamera": return ECameraSdkType.VirtualCamera;
                default: return ECameraSdkType.Unknown;
            }
        }

        /// <summary>
        /// 转换ECameraSdkType为目录名
        /// </summary>
        /// <param name="eCameraSDK">相机SDK类型</param>
        /// <returns>目录名</returns>
        private static string ConvertToDirectoryName(ECameraSdkType eCameraSDK)
        {
            switch (eCameraSDK)
            {
                case ECameraSdkType.Pylon: return "Pylon";
                case ECameraSdkType.uEye: return "uEyeSDK";
                case ECameraSdkType.Hik: return "Hik";
                case ECameraSdkType.Common: return "CommonSDK";
                case ECameraSdkType.Cognex: return "Cognex";
                case ECameraSdkType.Daheng: return "Daheng";
                case ECameraSdkType.DALSA: return "DALSA";
                case ECameraSdkType.ImagingSource: return "ImagingSource";
                case ECameraSdkType.VirtualCamera: return "VirtualCamera";
                default: return "";
            }
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
                        CameraAssemblys.Add(ConvertToECameraSDK(item.Name), assembly);
                    }

                }
            }
        }

        /// <summary>
        /// 创建相机实例
        /// </summary>
        /// <param name="eCameraSDK">相机SDK类型</param>
        /// <returns>相机实例</returns>
        public static ICamera CreateInstance(ECameraSdkType eCameraSDK)
        {
            try
            {
                if (CameraAssemblys.ContainsKey(eCameraSDK))
                {
                    //创建视觉框架实例
                    foreach (var item in CameraAssemblys[eCameraSDK].ExportedTypes)
                    {
                        if (item.Name == "Camera")
                        {
                            object obj = CameraAssemblys[eCameraSDK].CreateInstance(item.FullName);

                            if (obj is ICamera)
                            {
                                return obj as ICamera;
                            }
                        }
                    }
                }

                throw new FileNotFoundException($"{nameof(eCameraSDK)} is not found");
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
        public static ECameraSdkType DefaultCameraSdkType { get; set; } = ECameraSdkType.Unknown;

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
            //校验相机SDK是否有效
            if (!CameraAssemblys.ContainsKey(DefaultCameraSdkType))
            {
                throw new ArgumentException("ECameraSdkType invalid");
            }

            return CreateInstance(DefaultCameraSdkType).GetDeviceList();
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
            //校验相机SDK是否有效
            if (!CameraAssemblys.ContainsKey(DefaultCameraSdkType))
            {
                throw new ArgumentException("ECameraSdkType invalid");
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

            ICamera camera = CreateInstance(DefaultCameraSdkType);

            if (camera.Connect(cameraSerial))
            {
                Cameras.Add(cameraSerial, camera);
                SetToDefaultConfiguration(camera);
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
            if (!CameraAssemblys.ContainsKey(DefaultCameraSdkType))
            {
                throw new ArgumentException("ECameraSdkType invalid");
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

                if (camera.ExposureAuto == EExposureAutoContorl.Continuous)
                {
                    camera.ExposureAuto = EExposureAutoContorl.Off;
                }

                if (camera.GainAuto == EGainAutoContorl.Continuous)
                {
                    camera.GainAuto = EGainAutoContorl.Off;
                }

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
                    return Cameras[cameraSerial];
                }
                else
                {
                    AddCamera(cameraSerial);

                    if (Cameras.ContainsKey(cameraSerial))
                    {
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

        #region 配置管理

        /// <summary>
        /// 获取相机配置文件
        /// </summary>
        /// <param name="cameraSerial">相机配置文件</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:不捕获常规异常类型", Justification = "<挂起>")]
        public static FileInfo[] GetCameraConfigFile(string cameraSerial)
        {
            FileInfo[] fileInfos = new FileInfo[0];
            
            try
            {
                if (CameraFactory.DefaultCameraSdkType != ECameraSdkType.VirtualCamera)
                {
                    var directory = new DirectoryInfo($"VisionPlatform/Camera/CameraConfig/{cameraSerial}/ConfigFile");

                    fileInfos = directory?.GetFiles("*.json", SearchOption.TopDirectoryOnly) ?? new FileInfo[0];
                }
            }
            catch (Exception)
            {
            }

            return fileInfos;
        }

        /// <summary>
        /// 配置相机
        /// </summary>
        /// <param name="cameraSerial">相机序列号</param>
        /// <param name="cameraConfigParam">相机配置参数</param>
        public static void ConfigurateCamera(string cameraSerial, CameraConfigParam cameraConfigParam)
        {
            if (Cameras.ContainsKey(cameraSerial ?? "") && (Cameras[cameraSerial]?.IsOpen == true))
            {
                //配置像素格式
                if (Cameras[cameraSerial].PixelFormat != cameraConfigParam.PixelFormat)
                {
                    if (Cameras[cameraSerial].IsGrabbing)
                    {
                        Cameras[cameraSerial].StopGrab();
                        Cameras[cameraSerial].PixelFormat = cameraConfigParam.PixelFormat;
                        Cameras[cameraSerial].Grab();
                    }
                    else
                    {
                        Cameras[cameraSerial].PixelFormat = cameraConfigParam.PixelFormat;
                    }
                }

                if (Cameras[cameraSerial].TriggerMode != cameraConfigParam.TriggerMode)
                {
                    Cameras[cameraSerial].TriggerMode = cameraConfigParam.TriggerMode;
                }

                if (cameraConfigParam.TriggerMode == ETriggerModeStatus.On)
                {
                    //配置触发源
                    if (Cameras[cameraSerial].TriggerSource != cameraConfigParam.TriggerSource)
                    {
                        Cameras[cameraSerial].TriggerSource = cameraConfigParam.TriggerSource;
                    }

                    if (cameraConfigParam.TriggerSource != ETriggerSource.Software)
                    {
                        //配置有效触发信号
                        if (Cameras[cameraSerial].TriggerActivation != cameraConfigParam.TriggerActivation)
                        {
                            Cameras[cameraSerial].TriggerActivation = cameraConfigParam.TriggerActivation;
                        }
                    }
                }

                //配置曝光值
                if (Cameras[cameraSerial].ExposureTime != cameraConfigParam.ExposureTime)
                {
                    Cameras[cameraSerial].ExposureTime = cameraConfigParam.ExposureTime;
                }

                //配置增益值
                if (Cameras[cameraSerial].Gain != cameraConfigParam.Gain)
                {
                    Cameras[cameraSerial].Gain = cameraConfigParam.Gain;
                }
            }

        }

        /// <summary>
        /// 获取相机配置
        /// </summary>
        /// <param name="cameraSerial">相机序列号</param>
        /// <returns></returns>
        public static CameraConfigParam GetCameraConfigration(string cameraSerial)
        {
            var cameraConfigParam = new CameraConfigParam();

            if (Cameras.ContainsKey(cameraSerial ?? "") && (Cameras[cameraSerial]?.IsOpen == true))
            {
                cameraConfigParam.PixelFormat = Cameras[cameraSerial].PixelFormat;
                cameraConfigParam.TriggerMode = Cameras[cameraSerial].TriggerMode;
                cameraConfigParam.TriggerSource = Cameras[cameraSerial].TriggerSource;
                cameraConfigParam.TriggerActivation = Cameras[cameraSerial].TriggerActivation;
                cameraConfigParam.ExposureTime = Cameras[cameraSerial].ExposureTime;
                cameraConfigParam.Gain = Cameras[cameraSerial].Gain;
            }

            return cameraConfigParam;
        }
        #endregion
    }
}
