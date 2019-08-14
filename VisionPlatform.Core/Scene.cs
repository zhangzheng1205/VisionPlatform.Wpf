using Framework.Camera;
using Framework.Infrastructure.Serialization;
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
        public Scene(string sceneName, EVisionFrame eVisionFrame) : this(sceneName)
        {
            EVisionFrame = eVisionFrame;
            VisionFrame = VisionFrameFactory.CreateInstance(eVisionFrame);
        }

        /// <summary>
        /// 创建Scene新实例
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="eVisionFrame">视觉框架枚举</param>
        /// <param name="visionOperaFile">算子文件路径</param>
        public Scene(string sceneName, EVisionFrame eVisionFrame, string visionOperaFile) : this(sceneName, eVisionFrame)
        {
            if (VisionFrame == null)
            {
                throw new ArgumentException("VisionFrame cannot be null");
            }

            VisionOperaFile = visionOperaFile;
            VisionFrame.Init(visionOperaFile);

            //还原输入参数
            if (VisionFrame.IsEnableInput == true)
            {
                if (string.IsNullOrEmpty(InputParamFile))
                {
                    InputParamFile = "default.json";
                }

                string filePath = $"VisionPlatform/Scene/{EVisionFrame}/{Name}/InputParam/{InputParamFile}";
                if (!File.Exists(filePath))
                {
                    JsonSerialization.SerializeObjectToFile(VisionFrame.Inputs, filePath);
                }
            }

            //还原输出参数
            if (VisionFrame.IsEnableOutput == true)
            {
                if (string.IsNullOrEmpty(OutputParamFile))
                {
                    OutputParamFile = "default.json";
                }

                string filePath = $"VisionPlatform/Scene/{EVisionFrame}/{Name}/OutputParam/{OutputParamFile}";
                if (!File.Exists(filePath))
                {
                    JsonSerialization.SerializeObjectToFile(VisionFrame.Outputs, filePath);
                }
            }
        }

        /// <summary>
        /// 创建Scene新实例
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="eVisionFrame">视觉框架枚举</param>
        /// <param name="visionOperaFile">算子文件路径</param>
        /// <param name="cameraSerial">相机序列号</param>
        public Scene(string sceneName, EVisionFrame eVisionFrame, string visionOperaFile, string cameraSerial) : this(sceneName, eVisionFrame, visionOperaFile)
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
        public EVisionFrame EVisionFrame { get; set; }

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
        /// Halcon平台,算子文件为实现IVisionOpera接口的类库(*.dll);
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
        /// 标定文件
        /// </summary>
        public string CalibFile { get; set; }

        /// <summary>
        /// 相机配置文件
        /// </summary>
        public string CameraConfigFile { get; set; }

        #endregion

        #region 结果拼接

        /// <summary>
        /// 分隔符
        /// </summary>
        public char SeparatorChar { get; set; } = ',';

        /// <summary>
        /// 结束符
        /// </summary>
        public char TerminatorChar { get; set; } = ';';

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

                    if (CameraFactory.ECameraSDK != ECameraSDK.VirtualCamera)
                    {
                        if (string.IsNullOrEmpty(CalibFile))
                        {
                            CalibFile = $"VisionPlatform/Camera/CameraConfig/{CameraSerial}/Calibration/default.json";
                        }
                        if (string.IsNullOrEmpty(CameraConfigFile))
                        {
                            CameraConfigFile = $"VisionPlatform/Camera/CameraConfig/{CameraSerial}/Configuration/default.json";
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <remarks>
        /// 这个方法的主要功能是在配置参数完善的前提下,进行接口的还原;
        /// 例如反序列化后,进行场景接口的还原;
        /// </remarks>
        public void Init()
        {
            try
            {
                //还原视觉框架
                if (EVisionFrame == EVisionFrame.Unknown)
                {
                    throw new ArgumentException("VisionFrameName invalid");
                }

                if (VisionFrame == null)
                {
                    VisionFrame = VisionFrameFactory.CreateInstance(EVisionFrame);
                }

                //还原视觉算子
                if (string.IsNullOrEmpty(VisionOperaFile))
                {
                    throw new ArgumentException("VisionOperaFile cannot be null");
                }

                VisionFrame.Init(VisionOperaFile);

                //还原相机
                if (VisionFrame.IsEnableCamera)
                {
                    if (string.IsNullOrEmpty(CameraSerial))
                    {
                        throw new ArgumentException("CameraSerial cannot be null");
                    }

                    //若相机无效,则不报异常
                    try
                    {
                        SetCamera(CameraSerial);
                    }
                    catch (InvalidOperationException)
                    {
                        //若打开相机失败,不抛异常
                    }
                }

                //还原输入参数
                if (VisionFrame.IsEnableInput == true)
                {
                    if (string.IsNullOrEmpty(InputParamFile))
                    {
                        InputParamFile = "default.json";
                    }

                    //若文件不存在,则创建默认配置到此文件;若文件存在,则从配置文件中加载配置;
                    string filePath = $"VisionPlatform/Scene/{EVisionFrame}/{Name}/InputParam/{InputParamFile}";
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

                //还原输出参数
                if (VisionFrame.IsEnableOutput == true)
                {
                    if (string.IsNullOrEmpty(OutputParamFile))
                    {
                        OutputParamFile = "default.json";
                    }

                    //若文件不存在,则创建默认配置到此文件;若文件存在,则从配置文件中加载配置;
                    string filePath = $"VisionPlatform/Scene/{EVisionFrame}/{Name}/OutputParam/{OutputParamFile}";
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
        public void Execute(int timeout, out string visionResult)
        {
            //参数校验
            if (VisionFrame == null)
            {
                throw new NullReferenceException("VisionFrame cannot be null");
            }

            if (!IsInit)
            {
                throw new Exception("scene is uninitialized");
            }

            try
            {
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
                            grapTimeoutStopwatch.Stop();
                            throw new TimeoutException("grab image timeout");
                        }

                        System.Threading.Thread.Sleep(2);
                    }

                    //执行视觉框架
                    VisionFrame.ExecuteByImageInfo(imageInfo, out outputs);

                    //结果拼接
                    visionResult = ConvertItemCollectionToString(outputs, SeparatorChar, TerminatorChar);
                }
                else
                {
                    //执行视觉框架
                    VisionFrame.Execute(timeout, out outputs);

                    //结果拼接
                    visionResult = ConvertItemCollectionToString(outputs, SeparatorChar, TerminatorChar);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                //释放图像资源,否则可能会导致资源泄露
                imageInfo.DisposeImageIntPtr?.Invoke(imageInfo.ImagePtr);

                //停止计时
                totalProcessStopwatch.Stop();
                VisionFrame.RunStatus.TotalTime = totalProcessStopwatch.Elapsed.TotalMilliseconds;
            }

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
        public void ExecuteByFile(string file, out string visionResult)
        {
            if ((VisionFrame == null) || (!VisionFrame.IsInit))
            {
                throw new NullReferenceException("VisionFrame invaild");
            }

            try
            {
                //执行视觉框架
                ItemCollection outputs;
                VisionFrame.ExecuteByFile(file, out outputs);

                //结果拼接
                visionResult = ConvertItemCollectionToString(outputs, SeparatorChar, TerminatorChar);
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// 保存参数到本地图片
        /// </summary>
        public void SaveParamToLocalFile()
        {
            if (VisionFrame == null)
            {
                VisionFrame = VisionFrameFactory.CreateInstance(EVisionFrame);
            }

            //保存输入参数
            if (VisionFrame.IsEnableInput == true)
            {
                if (string.IsNullOrEmpty(InputParamFile))
                {
                    InputParamFile = "default.json";
                }

                //若文件不存在,则创建默认配置到此文件;若文件存在,则从配置文件中加载配置;
                string filePath = $"VisionPlatform/Scene/{EVisionFrame}/{Name}/InputParam/{InputParamFile}";
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
                string filePath = $"VisionPlatform/Scene/{EVisionFrame}/{Name}/OutputParam/{OutputParamFile}";
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

            scene.Init();

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
