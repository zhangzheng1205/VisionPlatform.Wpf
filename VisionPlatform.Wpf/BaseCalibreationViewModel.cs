using Caliburn.Micro;
using Framework.Vision;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionPlatform.Wpf
{
    /// <summary>
    /// BaseCalibreationView模型
    /// </summary>
    public class BaseCalibreationViewModel : Screen
    {
        #region 构造函数

        /// <summary>
        /// 创建BaseCalibreationViewModel新实例
        /// </summary>
        public BaseCalibreationViewModel() : this(new CalibParam())
        {
            
        }

        /// <summary>
        /// 创建BaseCalibreationViewModel新实例
        /// </summary>
        /// <param name="calibParam">标定参数</param>
        public BaseCalibreationViewModel(CalibParam calibParam)
        {
            CalibParam = calibParam;
        }

        #endregion

        #region 属性

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

        private CalibParam calibParam;

        /// <summary>
        /// 标定参数
        /// </summary>
        public CalibParam CalibParam
        {
            get
            {
                return  calibParam;
            }
            set
            {
                calibParam = value;
                CalibPointList = new ObservableCollection<CalibPointData>(CalibParam.CalibPointList);
            }
        }

        /// <summary>
        /// 标定点位列表
        /// </summary>
        private ObservableCollection<CalibPointData> calibPointList = new ObservableCollection<CalibPointData>();

        /// <summary>
        /// 标定点位列表
        /// </summary>
        public ObservableCollection<CalibPointData> CalibPointList
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
        protected void OnCalibrationPointListChanged(ObservableCollection<CalibPointData> calibPointList)
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
        protected void OnCalibrationPointSelectionChanged(ObservableCollection<CalibPointData> calibPointList, int index, CalibPointData calibPointData)
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
            CalibPointList.Add(new CalibPointData(px, py, qx, qy));
            OnCalibrationPointListChanged(CalibPointList);
        }

        /// <summary>
        /// 覆盖
        /// </summary>
        public void Cover(int index, double px, double py, double qx, double qy)
        {
            if ((index >= 0) && (index < CalibPointList.Count))
            {
                CalibPointList[index] = new CalibPointData(px, py, qx, qy);
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
            CalibPointList = new ObservableCollection<CalibPointData>();
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
            CalibParam.CalibPointList = CalibPointList.ToList();

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
