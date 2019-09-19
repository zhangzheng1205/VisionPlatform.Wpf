using Caliburn.Micro;
using Framework.Infrastructure.Serialization;
using Framework.Vision;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using VisionPlatform.BaseType;
using VisionPlatform.Core;

namespace VisionPlatform.Wpf
{
    /// <summary>
    /// 标定点位置
    /// </summary>
    public enum ECalibrationPointLocation
    {
        /// <summary>
        /// 上左
        /// </summary>
        [Description("上左")]
        TopLeft,

        /// <summary>
        /// 上中
        /// </summary>
        [Description("上中")]
        TopCenter,

        /// <summary>
        /// 上右
        /// </summary>
        [Description("上右")]
        TopRight,

        /// <summary>
        /// 正左
        /// </summary>
        [Description("正左")]
        Left,

        /// <summary>
        /// 正中
        /// </summary>
        [Description("正中")]
        Center,

        /// <summary>
        /// 正右
        /// </summary>
        [Description("正右")]
        Right,

        /// <summary>
        /// 下左
        /// </summary>
        [Description("下左")]
        BottomLeft,

        /// <summary>
        /// 下中
        /// </summary>
        [Description("下中")]
        BottomCenter,

        /// <summary>
        /// 下右
        /// </summary>
        [Description("下右")]
        BottomRight,

    }

    /// <summary>
    /// CalibrationPointLocation到字符串转换器
    /// </summary>
    public class CalibrationPointLocationListToStringListConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var locations = value as ObservableCollection<ECalibrationPointLocation>;
            CalibrationPointLocationsToStringConverter converter = new CalibrationPointLocationsToStringConverter();

            if (locations != null)
            {
                ObservableCollection<string> locationDescriptions = new ObservableCollection<string>();

                foreach (var item in locations)
                {
                    locationDescriptions.Add(converter.Convert(item, targetType, parameter, culture) as string);
                }

                return locationDescriptions;
            }

            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// 字符串到CalibrationPointLocation转换器
    /// </summary>
    public class CalibrationPointLocationsToStringConverter : IValueConverter
    {
        /// <summary>
        /// 枚举变量到字符串描述
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ECalibrationPointLocation location = (ECalibrationPointLocation)value;
            return (Attribute.GetCustomAttribute(location.GetType().GetField(location.ToString()), typeof(DescriptionAttribute)) as DescriptionAttribute).Description;
        }

        /// <summary>
        /// 字符串描述到枚举变量
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string description = value as string;

            if (!string.IsNullOrEmpty(description))
            {
                foreach (ECalibrationPointLocation item in Enum.GetValues(typeof(ECalibrationPointLocation)))
                {
                    if ((Attribute.GetCustomAttribute(item.GetType().GetField(item.ToString()), typeof(DescriptionAttribute)) as DescriptionAttribute).Description.Equals(description))
                    {
                        return item;
                    }
                }
            }

            return ECalibrationPointLocation.TopCenter;
        }
    }

    /// <summary>
    /// 标定点
    /// </summary>
    public class CalibrationPoint : CalibPointData
    {
        /// <summary>
        /// 创建CalibrationPoint新实例
        /// </summary>
        public CalibrationPoint() : base(-1, -1, -1, -1)
        {
            Location = ECalibrationPointLocation.Center;
        }

        /// <summary>
        /// 创建CalibrationPoint新实例
        /// </summary>
        /// <param name="px">原始X点位</param>
        /// <param name="py">原始Y点位</param>
        /// <param name="qx">转换X点位</param>
        /// <param name="qy">转换Y点位</param>
        public CalibrationPoint(double px, double py, double qx, double qy) : base(px, py, qx, qy)
        {

        }

        /// <summary>
        /// 创建CalibrationPoint新实例
        /// </summary>
        /// <param name="calibPointData">标定点数据</param>
        public CalibrationPoint(CalibPointData calibPointData) : base(calibPointData.Px, calibPointData.Py, calibPointData.Qx, calibPointData.Qy)
        {

        }

        /// <summary>
        /// 创建CalibrationPoint新实例
        /// </summary>
        /// <param name="location">点位位置</param>
        /// <param name="px">原始X点位</param>
        /// <param name="py">原始Y点位</param>
        /// <param name="qx">转换X点位</param>
        /// <param name="qy">转换Y点位</param>
        public CalibrationPoint(ECalibrationPointLocation location, double px, double py, double qx, double qy) : this(px, py, qx, qy)
        {
            Location = location;
        }

        /// <summary>
        /// 位置
        /// </summary>
        public ECalibrationPointLocation Location { get; set; }

        /// <summary>
        /// 获取CalibPointData
        /// </summary>
        /// <returns>CalibPointData数据</returns>
        public CalibPointData GetCalibPointData()
        {
            return new CalibPointData(this.Px, this.Py, this.Qx, this.Qy);
        }
    }

    /// <summary>
    /// 标定参数
    /// </summary>
    public class CalibrationParam : CalibParam
    {
        /// <summary>
        /// 创建CalibrationParam新实例
        /// </summary>
        public CalibrationParam() : base()
        {

        }
        /// <summary>
        /// 标定点列表
        /// </summary>
        public new List<CalibrationPoint> CalibPointList { get; set; } = new List<CalibrationPoint>();
    }

    /// <summary>
    /// CalibreationView模型
    /// </summary>
    public class CalibrationViewModel : Screen
    {
        #region 构造函数

        /// <summary>
        /// 创建BaseCalibreationViewModel新实例
        /// </summary>
        public CalibrationViewModel() : this(new CalibParam())
        {

        }

        /// <summary>
        /// 创建BaseCalibreationViewModel新实例
        /// </summary>
        /// <param name="calibParam">标定参数</param>
        public CalibrationViewModel(CalibParam calibParam)
        {
            CalibrationParam = new CalibrationParam();

            if (calibParam != null)
            {
                foreach (var item in calibParam.CalibPointList)
                {
                    CalibrationParam.CalibPointList.Add(new CalibrationPoint(item));
                }

                CalibrationParam.InvMatrix = calibParam.InvMatrix;
                CalibrationParam.Matrix = calibParam.Matrix;
                CalibrationParam.IsValid = calibParam.IsValid;
            }

            calibrationPointLocations.Clear();
            foreach (ECalibrationPointLocation item in Enum.GetValues(typeof(ECalibrationPointLocation)))
            {
                calibrationPointLocations.Add(item);
            }

            sceneManager = SceneManager.GetInstance();
            UpdateScenes();

            //foreach (ECalibrationPointLocation item in Enum.GetValues(typeof(ECalibrationPointLocation)))
            //{
            //    calibrationPointLocations.Add(item, (Attribute.GetCustomAttribute(item.GetType().GetField(item.ToString()), typeof(DescriptionAttribute)) as DescriptionAttribute).Description);
            //}
        }

        #endregion

        #region 字段

        private SceneManager sceneManager;

        #endregion

        #region 属性

        #region 场景管理


        private ObservableCollection<Scene> scenes;

        /// <summary>
        /// 场景列表
        /// </summary>
        public ObservableCollection<Scene> Scenes
        {
            get
            {
                return scenes;
            }
            set
            {
                scenes = value;
                NotifyOfPropertyChange(() => Scenes);
            }
        }

        private Scene scene;

        /// <summary>
        /// 当前场景实例
        /// </summary>
        public Scene Scene
        {
            get
            {
                return scene;
            }
            set
            {
                scene = value;
                NotifyOfPropertyChange(() => Scene);
                NotifyOfPropertyChange(() => IsSceneValid);
                NotifyOfPropertyChange(() => CalibrationOperationViewWindow);
                NotifyOfPropertyChange(() => CameraSerial);
                NotifyOfPropertyChange(() => CameraName);

                UpdateCameraCalibrationFiles();
            }
        }

        /// <summary>
        /// 场景有效标志
        /// </summary>
        public bool IsSceneValid
        {
            get
            {
                return Scene?.IsInit ?? false;
            }
        }

        /// <summary>
        /// 标定算子显示窗口
        /// </summary>
        public object CalibrationOperationViewWindow
        {
            get
            {
                if (IsSceneValid)
                {
                    return Scene.VisionFrame.RunningWindow;
                }

                return new Grid();
            }
        }

        private string visionResult;

        /// <summary>
        /// 视觉结果
        /// </summary>
        public string VisionResult
        {
            get
            {
                return visionResult;
            }
            set
            {
                visionResult = value;
                NotifyOfPropertyChange(() => VisionResult);
            }
        }

        /// <summary>
        /// 相机序列号
        /// </summary>
        public string CameraSerial
        {
            get
            {
                if (IsSceneValid && (CameraFactory.DefaultCameraSdkType != ECameraSdkType.VirtualCamera))
                {
                    return Scene.CameraSerial;
                }

                return "";
            }
        }

        /// <summary>
        /// 相机名
        /// </summary>
        public string CameraName
        {
            get
            {
                if (IsSceneValid && (Scene?.VisionFrame.IsEnableCamera == true))
                {
                    return Scene.Camera.ToString();
                }
                return "";
            }
        }

        #endregion

        #region 输入栏参数

        private ObservableCollection<ECalibrationPointLocation> calibrationPointLocations = new ObservableCollection<ECalibrationPointLocation>();

        /// <summary>
        /// 标定点位位置
        /// </summary>
        public ObservableCollection<ECalibrationPointLocation> CalibrationPointLocations
        {
            get
            {
                return calibrationPointLocations;
            }
        }

        private ECalibrationPointLocation calibrationPointLocation;

        /// <summary>
        /// 标点点位位置
        /// </summary>
        public ECalibrationPointLocation CalibrationPointLocation
        {
            get
            {
                return calibrationPointLocation;
            }
            set
            {
                calibrationPointLocation = value;
                NotifyOfPropertyChange(() => CalibrationPointLocation);
            }
        }

        /// <summary>
        /// 原始点位X
        /// </summary>
        private double px;

        /// <summary>
        /// 原始点位X
        /// </summary>
        public double Px
        {
            get
            {
                return px;
            }
            set
            {
                px = value;
                NotifyOfPropertyChange(() => Px);
            }
        }

        /// <summary>
        /// 原始点位Y
        /// </summary>
        private double py;

        /// <summary>
        /// 原始点位Y
        /// </summary>
        public double Py
        {
            get
            {
                return py;
            }
            set
            {
                py = value;
                NotifyOfPropertyChange(() => Py);
            }
        }

        /// <summary>
        /// 目标点位X
        /// </summary>
        private double qx;

        /// <summary>
        /// 目标点位X
        /// </summary>
        public double Qx
        {
            get
            {
                return qx;
            }
            set
            {
                qx = value;
                NotifyOfPropertyChange(() => Qx);
            }
        }

        /// <summary>
        /// 目标点位Y
        /// </summary>
        private double qy;

        /// <summary>
        /// 目标点位Y
        /// </summary>
        public double Qy
        {
            get
            {
                return qy;
            }
            set
            {
                qy = value;
                NotifyOfPropertyChange(() => Qy);
            }
        }

        #endregion

        #region 标定点位

        private CalibrationParam calibrationParam;

        /// <summary>
        /// 标定参数
        /// </summary>
        public CalibrationParam CalibrationParam
        {
            get
            {
                return calibrationParam;
            }
            set
            {
                calibrationParam = value;
                CalibPointList = new ObservableCollection<CalibrationPoint>(CalibrationParam.CalibPointList);
            }
        }

        /// <summary>
        /// 标定点位列表
        /// </summary>
        private ObservableCollection<CalibrationPoint> calibPointList = new ObservableCollection<CalibrationPoint>();

        /// <summary>
        /// 标定点位列表
        /// </summary>
        public ObservableCollection<CalibrationPoint> CalibPointList
        {
            get
            {
                return calibPointList;
            }
            set
            {
                calibPointList = value;
                NotifyOfPropertyChange(() => CalibPointList);
            }
        }

        private int selectedIndex;

        /// <summary>
        /// 选择项索引
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                selectedIndex = value;
                NotifyOfPropertyChange(() => SelectedIndex);
            }
        }

        #endregion

        #endregion

        #region 委托

        /// <summary>
        /// 创建标定矩阵计算委托
        /// </summary>
        /// <param name="px">原始点X</param>
        /// <param name="py">原始点Y</param>
        /// <param name="qx">目标点位X</param>
        /// <param name="qy">目标点位Y</param>
        /// <param name="matrix">标定矩阵</param>
        /// <returns>执行结果</returns>
        public delegate bool GetCalibMatrixDelegate(double[] px, double[] py, double[] qx, double[] qy, out double[] matrix);

        /// <summary>
        /// 创建标定矩阵计算委托
        /// </summary>
        public GetCalibMatrixDelegate GetCalibMatrixCallback { get; set; }

        #endregion

        #region 事件

        internal void OnMessageRaised(MessageLevel messageLevel, string message, Exception exception = null)
        {
            MessageRaised?.Invoke(this, new MessageRaisedEventArgs(messageLevel, message, exception));
        }

        /// <summary>
        /// 消息触发事件
        /// </summary>
        internal event EventHandler<MessageRaisedEventArgs> MessageRaised;

        /// <summary>
        /// 触发标定点列表改变事件
        /// </summary>
        /// <param name="calibPointList"></param>
        protected void OnCalibrationPointListChanged(ObservableCollection<CalibrationPoint> calibPointList)
        {
            CalibrationPointListChanged?.Invoke(this, new CalibrationPointListChangedEventArgs(calibPointList));
        }

        /// <summary>
        /// 标定点列表改变事件
        /// </summary>
        public event EventHandler<CalibrationPointListChangedEventArgs> CalibrationPointListChanged;

        /// <summary>
        /// 触发标定点选择项改变事件
        /// </summary>
        /// <param name="calibPointList">标定点列表</param>
        /// <param name="index">点位索引</param>
        /// <param name="calibPointData">标定点点位数据</param>
        protected void OnCalibrationPointSelectionChanged(ObservableCollection<CalibrationPoint> calibPointList, int index, CalibPointData calibPointData)
        {
            CalibrationPointSelectionChanged?.Invoke(this, new CalibrationPointSelectionChangedEventArgs(calibPointList, index, calibPointData));
        }

        /// <summary>
        /// 选择的点位改变
        /// </summary>
        public event EventHandler<CalibrationPointSelectionChangedEventArgs> CalibrationPointSelectionChanged;

        #endregion

        #region 方法

        #region 标定交互相关

        /// <summary>
        /// 清除
        /// </summary>
        public void ClearInput()
        {
            Px = 0;
            Py = 0;
            Qx = 0;
            Qy = 0;
        }

        /// <summary>
        /// 设置在输入框当前显示的点位
        /// </summary>
        /// <param name="index">点位索引</param>
        public void SetCurrentDisplayPointInInputBox(int index)
        {
            if ((index >= 0) && (index < CalibPointList.Count))
            {
                CalibrationPointLocation = CalibPointList[index].Location;
                Px = CalibPointList[index].Px;
                Py = CalibPointList[index].Py;
                Qx = CalibPointList[index].Qx;
                Qy = CalibPointList[index].Qy;

                OnCalibrationPointSelectionChanged(CalibPointList, index, CalibPointList[index]);
            }
            else
            {
                OnCalibrationPointSelectionChanged(CalibPointList, -1, null);
            }
        }

        /// <summary>
        /// 增加
        /// </summary>
        public void Add(double px, double py, double qx, double qy)
        {
            Add(CalibrationPointLocation, px, py, qx, qy);

            if (Enum.IsDefined(typeof(ECalibrationPointLocation), CalibrationPointLocation + 1))
            {
                CalibrationPointLocation++;
            }
        }

        /// <summary>
        /// 增加点位
        /// </summary>
        /// <param name="location">点位位置</param>
        /// <param name="px">原始X点位</param>
        /// <param name="py">原始Y点位</param>
        /// <param name="qx">转换X点位</param>
        /// <param name="qy">转换Y点位</param>
        public void Add(ECalibrationPointLocation location, double px, double py, double qx, double qy)
        {
            CalibPointList.Add(new CalibrationPoint(location, px, py, qx, qy));
            OnCalibrationPointListChanged(CalibPointList);
        }

        /// <summary>
        /// 覆盖
        /// </summary>
        public void Cover(int index, double px, double py, double qx, double qy)
        {
            if ((index >= 0) && (index < CalibPointList.Count))
            {
                CalibPointList[index] = new CalibrationPoint(CalibrationPointLocation, px, py, qx, qy);
                OnCalibrationPointListChanged(CalibPointList);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Delete(int index)
        {
            if ((index >= 0) && (index < CalibPointList.Count))
            {
                CalibPointList.RemoveAt(index);
                OnCalibrationPointListChanged(CalibPointList);
            }
        }

        /// <summary>
        /// 清除所有的数据
        /// </summary>
        public void Clear()
        {
            if (MessageBox.Show("是否要清空列表?", "清空列表数据", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                CalibPointList = new ObservableCollection<CalibrationPoint>();
                OnCalibrationPointListChanged(CalibPointList);
            }

        }

        /// <summary>
        /// 标定
        /// </summary>
        public void GetCalibMatrix()
        {
            //拼接数据
            double[] pxArray = new double[CalibPointList.Count];
            double[] pyArray = new double[CalibPointList.Count];
            double[] qxArray = new double[CalibPointList.Count];
            double[] qyArray = new double[CalibPointList.Count];
            double[] posMatrix;
            double[] invMatrix;
            bool result1 = false;
            bool result2 = false;

            CalibrationParam.CalibPointList.Clear();

            for (int i = 0; i < CalibPointList.Count; i++)
            {
                pxArray[i] = CalibPointList[i].Px;
                pyArray[i] = CalibPointList[i].Py;
                qxArray[i] = CalibPointList[i].Qx;
                qyArray[i] = CalibPointList[i].Qy;

            }
            CalibrationParam.CalibPointList = CalibPointList.ToList();

            //计算标定矩阵
            if (GetCalibMatrixCallback != null)
            {
                result1 = GetCalibMatrixCallback.Invoke(pxArray, pyArray, qxArray, qyArray, out posMatrix);
                result2 = GetCalibMatrixCallback.Invoke(qxArray, qyArray, pxArray, pyArray, out invMatrix);
            }
            else
            {
                result1 = SimpleVision.Calibration.CreateCalibMatrix(pxArray, pyArray, qxArray, qyArray, out posMatrix);
                result2 = SimpleVision.Calibration.CreateCalibMatrix(qxArray, qyArray, pxArray, pyArray, out invMatrix);
            }

            //结果保存
            CalibrationParam.IsValid = result1 && result2;
            CalibrationParam.Matrix = posMatrix;
            CalibrationParam.InvMatrix = invMatrix;

            if (CalibrationParam.IsValid)
            {
                MessageRaised?.Invoke(this, new MessageRaisedEventArgs(MessageLevel.Message, "标定成功!"));
            }
            else
            {
                MessageRaised?.Invoke(this, new MessageRaisedEventArgs(MessageLevel.Warning, "标定失败!请检查相关的数据"));
            }
        }

        #endregion

        #region 文件管理

        private ObservableCollection<string> calibrationConfigFiles;

        /// <summary>
        /// 相机配置文件列表
        /// </summary>
        public ObservableCollection<string> CalibrationConfigFiles
        {
            get
            {
                return calibrationConfigFiles;
            }
            set
            {
                calibrationConfigFiles = value;
                NotifyOfPropertyChange(() => CalibrationConfigFiles);
                NotifyOfPropertyChange(() => CalibrationConfigFile);
            }
        }

        private string calibrationConfigFile;

        /// <summary>
        /// 相机配置文件列表
        /// </summary>
        public string CalibrationConfigFile
        {
            get
            {
                return calibrationConfigFile;
            }
            set
            {
                calibrationConfigFile = value;
                LoadFromCurrentFile(calibrationConfigFile);
                NotifyOfPropertyChange(() => CalibrationConfigFile);
            }
        }

        /// <summary>
        /// 更新相机标定文件
        /// </summary>
        public void UpdateCameraCalibrationFiles()
        {
            try
            {
                if (IsSceneValid && Scene.VisionFrame.IsEnableCamera)
                {
                    FileInfo[] files = CameraFactory.GetCameraCalibrationFiles(Scene.Camera.Info.SerialNumber);
                    CalibrationConfigFiles = new ObservableCollection<string>(files.ToList().ConvertAll(x => x.Name));

                    if ((CalibrationConfigFiles.Count > 0) && (string.IsNullOrEmpty(CalibrationConfigFile)))
                    {
                        CalibrationConfigFile = CalibrationConfigFiles[0];
                    }
                }
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
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

                if (Scene?.Camera?.IsOpen == true)
                {
                    string file = $"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}/VisionPlatform/Camera/CameraConfig/{Scene.Camera.Info.SerialNumber}/CalibrationFile/{Path.GetFileNameWithoutExtension(fileName)}.json";
                    SaveToFile(file);
                    OnMessageRaised(MessageLevel.Message, "保存成功");
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

                if (Scene?.Camera?.IsOpen == true)
                {

                    JsonSerialization.SerializeObjectToFile(CalibrationParam, file);
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
                    return;
                }

                if (Scene?.Camera?.IsOpen == true)
                {
                    string file = $"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}/VisionPlatform/Camera/CameraConfig/{Scene.Camera.Info.SerialNumber}/CalibrationFile/{Path.GetFileNameWithoutExtension(fileName)}.json";
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

                CalibrationParam calibrationParam = JsonSerialization.DeserializeObjectFromFile<CalibrationParam>(file);

                if (calibrationParam != null)
                {
                    CalibrationParam = calibrationParam;
                }
                else
                {
                    CalibrationParam = new CalibrationParam();
                }
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
            finally
            {

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

                if (Scene?.Camera?.IsOpen == true)
                {
                    string file = $"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}/VisionPlatform/Camera/CameraConfig/{Scene.Camera.Info.SerialNumber}/CalibrationFile/{Path.GetFileNameWithoutExtension(fileName)}.json";

                    if (File.Exists(file))
                    {
                        File.Delete(file);
                        UpdateCameraCalibrationFiles();
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

        /// <summary>
        /// 新增标定文件
        /// </summary>
        public void AddCalibrationFile()
        {
            try
            {
                if (Scene?.Camera?.IsOpen != true)
                {
                    throw new Exception("相机无效!无法绑定标定文件到相机");
                }

                var inputWindow = new InputWindow();
                inputWindow.InputAccepted += (sender, e) =>
                {
                    string file = $"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}/VisionPlatform/Camera/CameraConfig/{Scene.Camera.Info.SerialNumber}/CalibrationFile/{Path.GetFileNameWithoutExtension(e.Input)}.json";
                    SaveToFile(file);
                    UpdateCameraCalibrationFiles();
                };

                inputWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                inputWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
        }

        #endregion

        #region 场景封装

        /// <summary>
        /// 更新场景列表
        /// </summary>
        public void UpdateScenes()
        {
            try
            {
                Scenes = new ObservableCollection<Scene>(sceneManager.Scenes.Values);
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
        }

        /// <summary>
        /// 设置图像标定点
        /// </summary>
        /// <param name="px">图像坐标X</param>
        /// <param name="py">图像坐标Y</param>
        public void SetImageCalibrationPoint(double px, double py)
        {
            try
            {
                for (int i = 0; i < CalibPointList.Count; i++)
                {
                    if (CalibPointList[i].Location == CalibrationPointLocation)
                    {
                        Cover(i, px, py, 0, 0);

                        if (Enum.IsDefined(typeof(ECalibrationPointLocation), CalibrationPointLocation + 1))
                        {
                            CalibrationPointLocation++;
                        }
                        return;
                    }
                }
                Add(CalibrationPointLocation, px, py, 0, 0);

                if (Enum.IsDefined(typeof(ECalibrationPointLocation), CalibrationPointLocation + 1))
                {
                    CalibrationPointLocation++;
                }
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
        }

        /// <summary>
        /// 设置机器人标定点
        /// </summary>
        /// <param name="qx">机器人X</param>
        /// <param name="qy">机器人Y</param>
        public void SetRobotCalibrationPoint(double qx, double qy)
        {
            try
            {
                for (int i = 0; i < CalibPointList.Count; i++)
                {
                    if (CalibPointList[i].Location == CalibrationPointLocation)
                    {
                        Cover(i, 0, 0, qx, qy);

                        if (Enum.IsDefined(typeof(ECalibrationPointLocation), CalibrationPointLocation + 1))
                        {
                            CalibrationPointLocation++;
                        }
                        return;
                    }
                }
                Add(CalibrationPointLocation, 0, 0, qx, qy);

                if (Enum.IsDefined(typeof(ECalibrationPointLocation), CalibrationPointLocation + 1))
                {
                    CalibrationPointLocation++;
                }
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
        }

        /// <summary>
        /// 获取图像位置
        /// </summary>
        public void GetImageLocation()
        {
            try
            {
                if (IsSceneValid)
                {
                    string result;
                    RunStatus runStatus = Scene.Execute(1000, out result);

                    if (runStatus.Result == EResult.Accept)
                    {
                        VisionResult = result;

                        foreach (var item in Scene.VisionFrame.Outputs)
                        {
                            if (item.Value is Location[])
                            {
                                var locations = item.Value as Location[];

                                //只取第一位有效数据
                                if (locations.Length > 0)
                                {
                                    SetImageCalibrationPoint(locations[0].X, locations[0].Y);
                                }

                            }
                            else if (item.Value is Location)
                            {
                                var location = (Location)item.Value;
                                SetImageCalibrationPoint(location.X, location.Y);
                            }
                        }
                    }
                    else
                    {
                        VisionResult = $"{runStatus.Result}: {runStatus.Message}";
                    }
                }
                else
                {
                    throw new Exception("场景无效");
                }
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }

        }

        #endregion

        #endregion
    }
}
