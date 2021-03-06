﻿using Framework.Camera;
using Framework.Infrastructure.Serialization;
using Framework.Vision;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using VisionPlatform.BaseType;

namespace VisionPlatform.Core
{
    /// <summary>
    /// 场景实例
    /// </summary>
    /// <remarks>
    /// 场景(Scene)主要是对视觉框架(IVisionFrame)的进一步封装,要求能把视觉框架实例的参数转换成配置文件,
    /// 以及能从配置文件中还原回一个视觉框架(IVisionFrame)实例;
    /// 同时,场景作为最基本的运行单元供应用层调用.应用层不关心视觉框架实现细节,只关心场景所提供的接口;
    /// </remarks>
    public class Scene : IDisposable
    {
        #region 构造函数

        /// <summary>
        /// 创建Scene新实例
        /// </summary>
        public Scene()
        {

        }

        /// <summary>
        /// 创建Scene新实例
        /// </summary>
        /// <param name="sceneName">场景名</param>
        public Scene(string sceneName) : this()
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                throw new ArgumentException("sceneName cannot be null");
            }

            Name = sceneName;

        }

        /// <summary>
        /// 创建Scene新实例
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="eVisionFrame">视觉框架枚举</param>
        public Scene(string sceneName, EVisionFrameType eVisionFrame) : this(sceneName)
        {
            EVisionFrameType = eVisionFrame;
            VisionFrame = VisionFrameFactory.CreateInstance(eVisionFrame);
        }

        /// <summary>
        /// 创建Scene新实例
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="eVisionFrame">视觉框架枚举</param>
        /// <param name="visionOperaFile">算子文件路径</param>
        public Scene(string sceneName, EVisionFrameType eVisionFrame, string visionOperaFile) : this(sceneName, eVisionFrame)
        {
            if (VisionFrame == null)
            {
                throw new ArgumentException("VisionFrame cannot be null");
            }

            if (string.IsNullOrEmpty(visionOperaFile) || !File.Exists(visionOperaFile))
            {
                throw new FileNotFoundException("visionOperaFile invalid");
            }

            //还原视觉算子
            SetVisionOperaFile(visionOperaFile);
            
        }

        /// <summary>
        /// 创建Scene新实例
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="eVisionFrame">视觉框架枚举</param>
        /// <param name="visionOperaFile">算子文件路径</param>
        /// <param name="cameraSerial">相机序列号</param>
        public Scene(string sceneName, EVisionFrameType eVisionFrame, string visionOperaFile, string cameraSerial) : this(sceneName, eVisionFrame, visionOperaFile)
        {
            if (VisionFrame.IsEnableCamera)
            {
                CameraSerial = cameraSerial;

                //若相机无效,则不报异常
                try
                {
                    SetCamera(cameraSerial);
                }
                catch (InvalidOperationException)
                {
                    //若打开相机失败,不抛异常
                }
            }

        }

        #endregion

        #region 字段

        /// <summary>
        /// 线程锁
        /// </summary>
        private readonly object threadLock = new object();

        /// <summary>
        /// 视觉处理完成标志
        /// </summary>
        private bool isVisionProcessed = false;

        /// <summary>
        /// 采集超时计时器
        /// </summary>
        private readonly Stopwatch grapTimeoutStopwatch = new Stopwatch();

        /// <summary>
        /// 总处理时间计时器
        /// </summary>
        private readonly Stopwatch totalProcessStopwatch = new Stopwatch();

        /// <summary>
        /// 相机信息
        /// </summary>
        private ImageInfo imageInfo;

        #endregion

        #region 属性

        /// <summary>
        /// 场景名称
        /// </summary>
        public string Name { get; set; } = "EmptyScene";

        /// <summary>
        /// 初始化标志
        /// </summary>
        [JsonIgnore]
        public bool IsInit
        {
            get
            {
                //校验视觉框架状态
                if (!IsVisionFrameInit)
                {
                    return false;
                }

                //校验相机状态
                if ((VisionFrame.IsEnableCamera == true) && (!IsCameraInit))
                {
                    return false;
                }

                return true;
            }
        }

        #region 接口实例

        /// <summary>
        /// 视觉框架名
        /// </summary>
        public EVisionFrameType EVisionFrameType { get; set; }

        /// <summary>
        /// 视觉框架
        /// </summary>
        [JsonIgnore]
        public IVisionFrame VisionFrame { get; set; }

        /// <summary>
        /// 视觉框架初始化标志
        /// </summary>
        [JsonIgnore]
        public bool IsVisionFrameInit
        {
            get
            {
                if ((VisionFrame != null) && (VisionFrame.IsInit))
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 相机接口
        /// </summary>
        [JsonIgnore]
        public ICamera Camera { get; set; }

        /// <summary>
        /// 相机初始化标志
        /// </summary>
        [JsonIgnore]
        public bool IsCameraInit
        {
            get
            {
                if (VisionFrame?.IsEnableCamera == true)
                {
                    if ((Camera != null) && (Camera.IsOpen))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        #endregion

        #region 视觉框架参数

        /// <summary>
        /// 算子文件路径
        /// </summary>
        /// <remarks>
        /// 针对于不同的平台,算子文件可以使用不同格式的文件,例如,
        /// Halcon平台,算子文件为实现IVisionOperation接口的类库(*.dll);
        /// VisionPro平台,算子文件为ToolBlock工具保存的二进制文件(*.vpp);
        /// </remarks>
        public string VisionOperaFile { get; set; }

        /// <summary>
        /// 输入参数文件
        /// </summary>
        /// <remarks>
        /// 若
        /// </remarks>
        public string InputParamFile { get; set; }

        /// <summary>
        /// 输出参数文件
        /// </summary>
        public string OutputParamFile { get; set; }

        #endregion

        #region 相机参数

        /// <summary>
        /// 相机编号
        /// </summary>
        public string CameraSerial { get; set; }

        /// <summary>
        /// 相机配置文件
        /// </summary>
        public string CameraConfigFile { get; set; }

        /// <summary>
        /// 相机配置参数
        /// </summary>
        private CameraConfigParam cameraConfigParam;

        /// <summary>
        /// 标定文件
        /// </summary>
        public string CalibrationFile { get; set; }

        /// <summary>
        /// 标定参数
        /// </summary>
        private CalibParam calibrationParam = new CalibParam();

        #endregion

        #region 结果拼接

        /// <summary>
        /// 一级分隔符
        /// </summary>
        public char MainSeparatorChar { get; set; } = ':';

        /// <summary>
        /// 结束符
        /// </summary>
        public char SubSeparatorChar { get; set; } = ',';

        #endregion

        #endregion

        #region 基本接口

        /// <summary>
        /// 图像采集完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Camera_NewImageEvent(object sender, NewImageEventArgs e)
        {
            try
            {
                imageInfo = e.ImageInfo;

                lock (threadLock)
                {
                    isVisionProcessed = true;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                //注销事件回调
                var camera = sender as ICamera;
                camera.NewImageEvent -= Camera_NewImageEvent;
            }

        }

#if false
        /// <summary>
        /// 转换ItemCollection为字符串
        /// </summary>
        /// <param name="itemBases">ItemCollection 数据</param>
        /// <param name="separatorChar">分割符</param>
        /// <param name="terminatorChar">结束符</param>
        /// <returns></returns>
        private static string ConvertItemCollectionToString(ItemCollection itemBases, char separatorChar, char terminatorChar)
        {
            var visionResult = "";

            foreach (var item in itemBases ?? new ItemCollection())
            {
                if (item.IsAvailable)
                {
                    if (item.ValueType.IsArray)
                    {
                        var array = item.Value as Array;

                        //获取维度
                        int rank = array.Rank;
                        int[] rackCount = new int[array.Rank];
                        for (int i = 0; i < array.Rank; i++)
                        {
                            rackCount[i] = array.GetLength(i);
                        }

                        string arrayString = "";

                        if (rank == 1)
                        {
                            arrayString += '[';
                            for (int i = 0; i < rackCount[0]; i++)
                            {
                                arrayString += array.GetValue(i).ToString() + separatorChar;
                            }
                            arrayString = arrayString.TrimEnd(separatorChar);
                            arrayString += ']';

                        }
                        else if (rank == 2)
                        {
                            arrayString += '[';
                            for (int i = 0; i < rackCount[0]; i++)
                            {
                                arrayString += '[';
                                for (int j = 0; j < rackCount[1]; j++)
                                {
                                    arrayString += array.GetValue(i, j).ToString() + separatorChar;
                                }
                                arrayString = arrayString.TrimEnd(separatorChar);
                                arrayString += ']';
                                arrayString += separatorChar;
                            }
                            arrayString = arrayString.TrimEnd(separatorChar);
                            arrayString += ']';
                        }
                        else if (rank == 3)
                        {
                            arrayString += '[';
                            for (int i = 0; i < rackCount[0]; i++)
                            {
                                arrayString += '[';
                                for (int j = 0; j < rackCount[1]; j++)
                                {
                                    arrayString += '[';
                                    for (int k = 0; k < rackCount[2]; k++)
                                    {
                                        arrayString += array.GetValue(i, j, k).ToString() + separatorChar;
                                    }
                                    arrayString = arrayString.TrimEnd(separatorChar);
                                    arrayString += ']';
                                    arrayString += separatorChar;
                                }
                                arrayString = arrayString.TrimEnd(separatorChar);
                                arrayString += ']';
                                arrayString += separatorChar;
                            }
                            arrayString = arrayString.TrimEnd(separatorChar);
                            arrayString += ']';
                        }
                        visionResult += arrayString + separatorChar;

                    }
                    else
                    {
                        visionResult += item.Value.ToString() + separatorChar;
                    }

                }
            }

            //移除结尾的分隔符并替换成结束符
            visionResult = visionResult.TrimEnd(separatorChar);
            visionResult += terminatorChar;

            return visionResult;
        }

#endif

        /// <summary>
        /// 数值转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="subSeparatorChar">二级分隔符</param>
        /// <returns>转换的字符串</returns>
        private string ValueToString(object value, char subSeparatorChar)
        {
            if (value.GetType().Equals(typeof(float)) || value.GetType().Equals(typeof(double)))
            {
                return $"{value:0.###}";
            }
            else if (value is Location)
            {
                var location = (Location)value;
                double qx = location.X;
                double qy = location.Y;

                //进行标定转换
                if (calibrationParam.IsValid)
                {
                    SimpleVision.Calibration.Calibrate(calibrationParam.Matrix, location.X, location.Y, out qx, out qy);
                }

                return $"{qx:0.###}{subSeparatorChar}{qy:0.###}{subSeparatorChar}{location.Angle:0.###}";
            }
            else
            {
                return value.ToString();
            }
        }

        /// <summary>
        /// 转换ItemCollection为字符串
        /// </summary>
        /// <param name="itemBases">ItemCollection 数据</param>
        /// <param name="mainSeparatorChar">一级分割符</param>
        /// <param name="subSeparatorChar">二级分割符</param>
        /// <returns></returns>
        private string ConvertItemCollectionToString(ItemCollection itemBases, char mainSeparatorChar, char subSeparatorChar)
        {
            var visionResult = "";

            foreach (var item in itemBases ?? new ItemCollection())
            {
                if (item?.IsAvailable == true)
                {
                    //若value和valueType的类型不一致,则报错
                    if (!item.ValueType.IsAssignableFrom(item.Value?.GetType()))
                    {
                        throw new ArgumentException("valueType must be assignable from value");
                    }

                    if (item.ValueType.IsArray)
                    {
                        var array = item.Value as Array;

                        //获取维度
                        int rank = array.Rank;
                        int[] rackCount = new int[array.Rank];
                        for (int i = 0; i < array.Rank; i++)
                        {
                            rackCount[i] = array.GetLength(i);
                        }

                        string arrayString = "";

                        if (rank == 1)
                        {
                            for (int i = 0; i < rackCount[0]; i++)
                            {
                                arrayString += ValueToString(array.GetValue(i), subSeparatorChar) + mainSeparatorChar;
                            }
                            arrayString = arrayString.TrimEnd(mainSeparatorChar);
                        }
                        //else if (rank == 2)
                        //{
                        //    arrayString += '[';
                        //    for (int i = 0; i < rackCount[0]; i++)
                        //    {
                        //        arrayString += '[';
                        //        for (int j = 0; j < rackCount[1]; j++)
                        //        {
                        //            arrayString += array.GetValue(i, j).ToString() + mainSeparatorChar;
                        //        }
                        //        arrayString = arrayString.TrimEnd(mainSeparatorChar);
                        //        arrayString += ']';
                        //        arrayString += mainSeparatorChar;
                        //    }
                        //    arrayString = arrayString.TrimEnd(mainSeparatorChar);
                        //    arrayString += ']';
                        //}
                        visionResult += arrayString + mainSeparatorChar;

                    }
                    else
                    {
                        visionResult += ValueToString(item.Value, subSeparatorChar) + mainSeparatorChar;
                    }

                }
            }

            return visionResult;
        }

        /// <summary>
        /// 设置相机
        /// </summary>
        /// <param name="cameraSerial"></param>
        /// <exception cref="InvalidOperationException">
        /// 打开相机失败
        /// </exception>
        public void SetCamera(string cameraSerial)
        {
            //若相机无效,则不报异常
            try
            {
                if (VisionFrame.IsEnableCamera)
                {
                    CameraSerial = cameraSerial;

                    Camera = CameraFactory.GetCamera(cameraSerial);

                    if (CameraFactory.DefaultCameraSdkType != ECameraSdkType.VirtualCamera)
                    {
                        //配置相机参数
                        if (CameraFactory.DefaultCameraSdkType != ECameraSdkType.VirtualCamera)
                        {
                            if (string.IsNullOrEmpty(CameraConfigFile))
                            {
                                CameraConfigFile = $"default.json";
                            }
                            SetCameraConfigFile(CameraConfigFile);
                        }

                        //if (string.IsNullOrEmpty(CalibrationFile))
                        //{
                        //    CalibrationFile = $"default.json";
                        //}
                        //配置标定信息
                        SetCameraCalibrationFile(CalibrationFile);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// 设置相机配置文件
        /// </summary>
        /// <param name="file">配置文件(不需包含目录)</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:不捕获常规异常类型", Justification = "<挂起>")]
        public void SetCameraConfigFile(string file)
        {
            CameraConfigFile = file;

            if (!string.IsNullOrEmpty(file))
            {
                //获取相机配置参数
                string configFile = $"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}/VisionPlatform/Camera/CameraConfig/{CameraSerial}/ConfigFile/{CameraConfigFile}";

                if (File.Exists(configFile))
                {
                    cameraConfigParam = JsonSerialization.DeserializeObjectFromFile<CameraConfigParam>(configFile);

                    //还原相机配置
                    try
                    {
                        CameraFactory.ConfigurateCamera(CameraSerial, cameraConfigParam);
                    }
                    catch (Exception)
                    {

                    }
                }
                else
                {
                    cameraConfigParam = CameraFactory.GetCameraConfigration(CameraSerial);
                    JsonSerialization.SerializeObjectToFile(cameraConfigParam, configFile);
                }
            }

        }

        /// <summary>
        /// 设置相机标定文件
        /// </summary>
        /// <param name="file">配置文件(不需包含目录)</param>
        public void SetCameraCalibrationFile(string file)
        {
            CalibrationFile = file;

            if (!string.IsNullOrEmpty(file))
            {
                //获取相机配置参数
                string calibrationFile = $"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}/VisionPlatform/Camera/CameraConfig/{CameraSerial}/CalibrationFile/{CalibrationFile}";

                if (File.Exists(calibrationFile))
                {
                    calibrationParam = JsonSerialization.DeserializeObjectFromFile<CalibParam>(calibrationFile);
                }
                else
                {
                    calibrationParam = new CalibParam();
                    JsonSerialization.SerializeObjectToFile(calibrationParam, calibrationFile);
                }
            }
            else
            {
                CalibrationFile = null;
                calibrationParam = new CalibParam();
            }

        }

        /// <summary>
        /// 还原输入参数文件
        /// </summary>
        /// <param name="isForceDefault">强制默认</param>
        private void RecoverInputFile(bool isForceDefault = false)
        {
            if (VisionFrame.IsEnableInput == true)
            {
                if (string.IsNullOrEmpty(InputParamFile))
                {
                    InputParamFile = "default.json";
                }

                //若文件不存在,则创建默认配置到此文件;若文件存在,则从配置文件中加载配置;
                string filePath = $"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}/VisionPlatform/Scene/{EVisionFrameType}/{Name}/InputParam/{InputParamFile}";

                if (isForceDefault)
                {
                    JsonSerialization.SerializeObjectToFile(VisionFrame.Inputs, filePath);
                }
                else
                {
                    if (!File.Exists(filePath))
                    {
                        JsonSerialization.SerializeObjectToFile(VisionFrame.Inputs, filePath);
                    }
                    else
                    {
                        VisionFrame.Inputs.Clear();
                        VisionFrame.Inputs.AddRange(JsonSerialization.DeserializeObjectFromFile<ItemCollection>(filePath));
                    }
                }
            }
        }

        /// <summary>
        /// 还原输出文件
        /// </summary>
        private void RecoverOutputFile(bool isForceDefault = false)
        {
            if (VisionFrame.IsEnableOutput == true)
            {
                if (string.IsNullOrEmpty(OutputParamFile))
                {
                    OutputParamFile = "default.json";
                }

                //若文件不存在,则创建默认配置到此文件;若文件存在,则从配置文件中加载配置;
                string filePath = $"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}/VisionPlatform/Scene/{EVisionFrameType}/{Name}/OutputParam/{OutputParamFile}";
                if (isForceDefault)
                {
                    JsonSerialization.SerializeObjectToFile(VisionFrame.Outputs, filePath);
                }
                else
                {
                    if (!File.Exists(filePath))
                    {
                        JsonSerialization.SerializeObjectToFile(VisionFrame.Outputs, filePath);
                    }
                    else
                    {
                        VisionFrame.Outputs.Clear();
                        VisionFrame.Outputs.AddRange(JsonSerialization.DeserializeObjectFromFile<ItemCollection>(filePath));
                    }
                }
            }
        }

        /// <summary>
        /// 设置视觉算子文件
        /// </summary>
        /// <param name="file">视觉算子文件</param>
        public void SetVisionOperaFile(string file)
        {
            if (VisionFrame == null)
            {
                throw new ArgumentException("VisionFrame cannot be null");
            }

            //还原视觉算子
            if (string.IsNullOrEmpty(file) || !File.Exists(file))
            {
                throw new FileNotFoundException("visionOperaFile invalid");
            }

            //确认文件是否本地路径,如果不是,则复制到本地路径下
            string dstDirectory = $"VisionPlatform/Scene/{EVisionFrameType}/{Name}/VisionOperation";
            string dstFile = $"{dstDirectory}/{Path.GetFileName(file)}";

            if (!Directory.Exists(dstDirectory))
            {
                Directory.CreateDirectory(dstDirectory);
            }

            if (Path.GetFullPath(dstFile) != file)
            {
                try
                {
                    //如果是dll,则将同级目录下所有的dll都拷贝过来
                    if (Path.GetExtension(file) == ".dll")
                    {
                        FileInfo[] fileList = new DirectoryInfo(Path.GetDirectoryName(file))?.GetFiles("*.dll", SearchOption.TopDirectoryOnly);

                        foreach (var item in fileList)
                        {
                            File.Copy(item.FullName, $"{dstDirectory}/{Path.GetFileName(item.Name)}", true);
                        }
                    }
                    else
                    {
                        File.Copy(file, dstFile, true);
                    }
                }
                catch (IOException)
                {
                    //如果是同一个文件,则会报IO异常,过滤掉此异常
                }
            }

            VisionOperaFile = Path.GetFileName(dstFile);
            VisionFrame.Init(dstFile);

            //创建默认输入输出参数文件
            RecoverInputFile(true);
            RecoverOutputFile(true);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <remarks>
        /// 这个方法的主要功能是在配置参数完善的前提下,进行接口的还原;
        /// 例如反序列化后,进行场景接口的还原;
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:不捕获常规异常类型", Justification = "<挂起>")]
        public void Init()
        {
            try
            {
                //还原视觉框架
                if (EVisionFrameType == EVisionFrameType.Unknown)
                {
                    throw new ArgumentException("VisionFrameName invalid");
                }

                if (VisionFrame == null)
                {
                    VisionFrame = VisionFrameFactory.CreateInstance(EVisionFrameType);
                }

                //还原视觉算子
                if (string.IsNullOrEmpty(VisionOperaFile))
                {
                    throw new ArgumentException("VisionOperaFile cannot be null");
                }

                string visionOperaFilePath = $"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}/VisionPlatform/Scene/{EVisionFrameType}/{Name}/VisionOperation/{VisionOperaFile}";
                VisionFrame.Init(visionOperaFilePath);

                //还原输入参数
                RecoverInputFile();

                //还原输出参数
                RecoverOutputFile();

                //还原相机
                if (VisionFrame.IsEnableCamera)
                {
                    if (!string.IsNullOrEmpty(CameraSerial))
                    {
                        //若相机无效,则不报异常
                        try
                        {
                            SetCamera(CameraSerial);
                        }
                        catch (InvalidOperationException)
                        {
                            //若打开相机失败,不抛异常
                        }

                        ////配置相机参数
                        //if (CameraFactory.DefaultCameraSdkType != ECameraSdkType.VirtualCamera)
                        //{
                        //    SetCameraConfigFile(CameraConfigFile);
                        //}

                        ////配置标定信息
                        //SetCameraCalibrationFile(CalibrationFile);
                    }

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
        /// <param name="visionResult">视觉结果(字符串格式)</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:不捕获常规异常类型", Justification = "<挂起>")]
        public RunStatus Execute(int timeout, out string visionResult)
        {
            bool isTimeout = false;
            var runStatus = new RunStatus();
            visionResult = "";

            try
            {
                //参数校验
                if (VisionFrame == null)
                {
                    throw new NullReferenceException("VisionFrame cannot be null");
                }

                if (!IsInit)
                {
                    throw new Exception($"scene is uninitialized({nameof(IsVisionFrameInit)}:{IsVisionFrameInit} {nameof(IsCameraInit)}:{IsCameraInit})");
                }

                ItemCollection outputs;

                //开始计时
                totalProcessStopwatch.Restart();

                //如果使能相机,则调用相机采集
                if (VisionFrame.IsEnableCamera && IsCameraInit)
                {
                    lock (threadLock)
                    {
                        isVisionProcessed = false;
                    }

                    //注册相机采集完成事件
                    Camera.NewImageEvent -= Camera_NewImageEvent;
                    Camera.NewImageEvent += Camera_NewImageEvent;

                    //配置相机参数
                    CameraFactory.ConfigurateCamera(CameraSerial, cameraConfigParam);

                    //触发拍照
                    Camera.TriggerSoftware();

                    //阻塞等待视觉处理完成
                    grapTimeoutStopwatch.Restart();
                    while (true)
                    {
                        lock (threadLock)
                        {
                            if (isVisionProcessed)
                            {
                                grapTimeoutStopwatch.Stop();
                                break;
                            }
                        }

                        if ((timeout > 0) && (grapTimeoutStopwatch.Elapsed.TotalMilliseconds > timeout))
                        {
                            isTimeout = true;
                            grapTimeoutStopwatch.Stop();
                            throw new TimeoutException("grab image timeout");
                        }

                        System.Threading.Thread.Sleep(2);
                    }

                    //执行视觉框架
                    VisionFrame.ExecuteByImageInfo(imageInfo, out outputs);

                    //结果拼接
                    visionResult = ConvertItemCollectionToString(outputs, MainSeparatorChar, SubSeparatorChar);
                }
                else
                {
                    //执行视觉框架
                    VisionFrame.Execute(timeout, out outputs);

                    //结果拼接
                    visionResult = ConvertItemCollectionToString(outputs, MainSeparatorChar, SubSeparatorChar);
                }
            }
            catch (Exception ex)
            {
                return new RunStatus(0, EResult.Error, ex.Message);
            }
            finally
            {
                //停止计时
                totalProcessStopwatch.Stop();
                VisionFrame.RunStatus.TotalTime = totalProcessStopwatch.Elapsed.TotalMilliseconds;

                if (!isTimeout)
                {
                    //释放图像资源,否则可能会导致资源泄露
                    imageInfo.DisposeImageIntPtr?.Invoke(imageInfo.ImagePtr);
                }
            }

            return VisionFrame.RunStatus;
        }

        /// <summary>
        /// 异步执行
        /// </summary>
        /// <param name="timeout">超时时间,单位:ms</param>
        /// <returns>异步任务</returns>
        public Task<string> ExecuteAsync(int timeout)
        {
            return Task.Run(() =>
            {
                string visionResult = "";
                try
                {
                    Execute(timeout, out visionResult);
                }
                catch (Exception)
                {
                    throw;
                }

                return visionResult;
            });
        }

        /// <summary>
        /// 通过本地图片执行
        /// </summary>
        /// <param name="file">本地图片路径</param>
        /// <param name="visionResult">视觉结果(字符串格式)</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:不捕获常规异常类型", Justification = "<挂起>")]
        public RunStatus ExecuteByFile(string file, out string visionResult)
        {
            visionResult = "";

            //开始计时
            totalProcessStopwatch.Restart();

            try
            {
                if ((VisionFrame == null) || (!VisionFrame.IsInit))
                {
                    throw new NullReferenceException("VisionFrame invaild");
                }

                //执行视觉框架
                ItemCollection outputs;
                VisionFrame.ExecuteByFile(file, out outputs);

                //结果拼接
                visionResult = ConvertItemCollectionToString(outputs, MainSeparatorChar, SubSeparatorChar);
            }
            catch (Exception ex)
            {
                return new RunStatus(0, EResult.Error, ex.Message);
            }
            finally
            {
                //停止计时
                totalProcessStopwatch.Stop();
                VisionFrame.RunStatus.TotalTime = totalProcessStopwatch.Elapsed.TotalMilliseconds;
            }

            return VisionFrame.RunStatus;
        }

        /// <summary>
        /// 保存参数到本地图片
        /// </summary>
        public void SaveParamToLocalFile()
        {
            if (VisionFrame == null)
            {
                VisionFrame = VisionFrameFactory.CreateInstance(EVisionFrameType);
            }

            //保存输入参数
            if (VisionFrame.IsEnableInput == true)
            {
                if (string.IsNullOrEmpty(InputParamFile))
                {
                    InputParamFile = "default.json";
                }

                //若文件不存在,则创建默认配置到此文件;若文件存在,则从配置文件中加载配置;
                string filePath = $"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}/VisionPlatform/Scene/{EVisionFrameType}/{Name}/InputParam/{InputParamFile}";
                JsonSerialization.SerializeObjectToFile(VisionFrame.Inputs, filePath);
            }

            //保存输出参数
            if (VisionFrame.IsEnableOutput == true)
            {
                if (string.IsNullOrEmpty(OutputParamFile))
                {
                    OutputParamFile = "default.json";
                }

                //若文件不存在,则创建默认配置到此文件;若文件存在,则从配置文件中加载配置;
                string filePath = $"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}/VisionPlatform/Scene/{EVisionFrameType}/{Name}/OutputParam/{OutputParamFile}";
                JsonSerialization.SerializeObjectToFile(VisionFrame.Outputs, filePath);
            }

        }

        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region 序列化/反序列化

        /// <summary>
        /// 反序列化场景
        /// </summary>
        /// <param name="file">序列化文件(*.json)</param>
        /// <returns>场景实例</returns>
        public static Scene Deserialize(string file)
        {
            var scene = JsonSerialization.DeserializeObjectFromFile<Scene>(file);

            try
            {
                scene?.Init();
            }
            catch (ArgumentException)
            {

            }
            
            return scene;
        }

        /// <summary>
        /// 序列化场景
        /// </summary>
        /// <param name="scene">场景实例</param>
        /// <param name="file">序列化文件(*.json)</param>
        public static void Serialize(Scene scene, string file)
        {
            if (scene == null)
            {
                throw new ArgumentNullException("scene cannot be null");
            }

            scene.SaveParamToLocalFile();
            JsonSerialization.SerializeObjectToFile(scene, file);
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
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

                VisionFrame?.Dispose();
                VisionFrame = null;

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~Scene()
        // {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}
