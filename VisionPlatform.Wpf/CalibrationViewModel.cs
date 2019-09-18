using Caliburn.Micro;
using Framework.Vision;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VisionPlatform.Wpf
{
    /// <summary>
    /// 标定点位置
    /// </summary>
    public enum ECalibrationPointLocation
    {
        ///// <summary>
        ///// 未知
        ///// </summary>
        //[Description("未知")]
        //Unknown,

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
        public CalibrationPoint(ECalibrationPointLocation location, double px, double py, double qx, double qy) : this (px, py, qx, qy)
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
            CalibParam = calibParam;

            calibrationPointLocations.Clear();
            foreach (ECalibrationPointLocation item in Enum.GetValues(typeof(ECalibrationPointLocation)))
            {
                calibrationPointLocations.Add(item);
            }
            //foreach (ECalibrationPointLocation item in Enum.GetValues(typeof(ECalibrationPointLocation)))
            //{
            //    calibrationPointLocations.Add(item, (Attribute.GetCustomAttribute(item.GetType().GetField(item.ToString()), typeof(DescriptionAttribute)) as DescriptionAttribute).Description);
            //}
        }

        #endregion

        #region 属性

        #region 场景管理

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

        #endregion

        #region 输入栏参数

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

        private CalibParam calibParam;

        /// <summary>
        /// 标定参数
        /// </summary>
        public CalibParam CalibParam
        {
            get
            {
                return calibParam;
            }
            set
            {
                calibParam = value;
                CalibPointList = new ObservableCollection<CalibrationPoint>(CalibParam.CalibPointList.ConvertAll(x=> new CalibrationPoint(x)));
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

        /// <summary>
        /// 标定矩阵
        /// </summary>
        public double[] Matrix
        {
            get
            {
                if (CalibParam.IsValid)
                {
                    return CalibParam.Matrix;
                }
                else
                {
                    return new double[0];
                }

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
            CalibPointList.Add(new CalibrationPoint(CalibrationPointLocation, px, py, qx, qy));
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
            CalibPointList = new ObservableCollection<CalibrationPoint>();
            OnCalibrationPointListChanged(CalibPointList);
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

            CalibParam.CalibPointList.Clear();

            for (int i = 0; i < CalibPointList.Count; i++)
            {
                pxArray[i] = CalibPointList[i].Px;
                pyArray[i] = CalibPointList[i].Py;
                qxArray[i] = CalibPointList[i].Qx;
                qyArray[i] = CalibPointList[i].Qy;

            }
            CalibParam.CalibPointList = CalibPointList.ToList().ConvertAll(x=>x.GetCalibPointData());

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
            CalibParam.IsValid = result1 && result2;
            CalibParam.Matrix = posMatrix;
            CalibParam.InvMatrix = invMatrix;

            NotifyOfPropertyChange(() => Matrix);

            if (CalibParam.IsValid)
            {
                MessageRaised?.Invoke(this, new MessageRaisedEventArgs(MessageLevel.Message, "标定成功!"));
            }
            else
            {
                MessageRaised?.Invoke(this, new MessageRaisedEventArgs(MessageLevel.Warning, "标定失败!请检查相关的数据"));
            }
        }

        #endregion
    }
}
