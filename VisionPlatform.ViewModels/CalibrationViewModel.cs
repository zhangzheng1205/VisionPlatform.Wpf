using Caliburn.Micro;
using Framework.Infrastructure.Serialization;
using Framework.Vision;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;


namespace VisionPlatform.ViewModels
{
    public class CalibrationViewModel : Screen
    {
        #region 属性

        /// <summary>
        /// 标定点位列表
        /// </summary>
        public CalibParam CalibParam { get; set; } = new CalibParam();

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
        private double[] matrix;

        /// <summary>
        /// 标定矩阵
        /// </summary>
        public double[] Matrix
        {
            get
            {
                return matrix;
            }

            private set
            {
                matrix = value;
                NotifyOfPropertyChange(() => Matrix);
            }
        }

        /// <summary>
        /// 文件路径
        /// </summary>
        private string filePath;

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

        /// <summary>
        /// 使能标志
        /// </summary>
        private bool isEnable = false;

        /// <summary>
        /// 使能标志
        /// </summary>
        public bool IsEnable
        {
            get
            {
                return isEnable;
            }
            set
            {
                isEnable = value;
                NotifyOfPropertyChange(() => IsEnable);
            }
        }

        /// <summary>
        /// 选择项索引
        /// </summary>
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

        #region 委托

        /// <summary>
        /// 创建标定矩阵委托
        /// </summary>
        /// <param name="px">原始点X</param>
        /// <param name="py">原始点Y</param>
        /// <param name="qx">目标点位X</param>
        /// <param name="qy">目标点位Y</param>
        /// <param name="matrix">标定矩阵</param>
        /// <returns>执行结果</returns>
        public delegate bool GetCalibMatrixDelegate(double[] px, double[] py, double[] qx, double[] qy, out double[] matrix);

        /// <summary>
        /// 创建标定矩阵委托
        /// </summary>
        public GetCalibMatrixDelegate GetCalibMatrixCallback { get; set; }

        /// <summary>
        /// 获取输入结果委托
        /// </summary>
        /// <param name="px">输入X</param>
        /// <param name="py">输入Y</param>
        /// <returns>执行结果</returns>
        public delegate bool GetInputResultDelegate(out double px, out double py);

        /// <summary>
        /// 获取输入结果委托
        /// </summary>
        public GetInputResultDelegate GetInputResultCallback { get; set; }

        /// <summary>
        /// 获取输出结果委托
        /// </summary>
        /// <param name="Px">输出</param>
        /// <param name="Py">图像Y</param>
        /// <returns>执行结果</returns>
        public delegate bool GetOutputResultDelegate(out double qx, out double qy);

        /// <summary>
        /// 获取输出结果委托
        /// </summary>
        public GetOutputResultDelegate GetOutputResultCallback { get; set; }

        #endregion

        #region 事件

        /// <summary>
        /// 创建标定文件事件
        /// </summary>
        public event EventHandler<CalibrationConfigurationCompletedEventArgs> CalibrationConfigurationCompleted;

        /// <summary>
        /// 取消事件
        /// </summary>
        public event EventHandler<EventArgs> CalibrationConfigurationCanceled;

        #endregion

        #region 方法

        /// <summary>
        /// 加载文件
        /// </summary>
        /// <param name="FilePath"></param>
        public void LoadFile(string filePath)
        {
            try
            {
                FilePath = filePath;
                if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                {
                    CalibParam = JsonSerialization.DeserializeObjectFromFile<CalibParam>(filePath);
                    if (CalibParam == null)
                    {
                        CalibParam = new CalibParam();
                    }

                    CalibPointList = new ObservableCollection<CalibPointData>(CalibParam.CalibPointList);

                    IsEnable = true;
                    Matrix = CalibParam.Matrix;
                }
            }
            catch (Exception)
            {
                throw;
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
                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    CalibParam = new CalibParam();
                    CalibPointList.Clear();

                    IsEnable = true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 复位
        /// </summary>
        public void Reset()
        {
            IsEnable = false;

            Px = 0;
            Py = 0;
            Qx = 0;
            Qy = 0;

            FilePath = "";
            Matrix = null;

            CalibPointList = new ObservableCollection<CalibPointData>();

        }

        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            Px = 0;
            Py = 0;
            Qx = 0;
            Qy = 0;
        }

        /// <summary>
        /// 获取输入结果
        /// </summary>
        public void GetInputResult()
        {
            double px = -1;
            double py = -1;

            GetInputResultCallback?.Invoke(out px, out py);

            Px = px;
            Py = py;
        }

        /// <summary>
        /// 获取输出结果
        /// </summary>
        public void GetOutputResult()
        {
            double qx = -1;
            double qy = -1;

            GetOutputResultCallback?.Invoke(out qx, out qy);

            Qx = qx;
            Qy = qy;
        }

        /// <summary>
        /// 增加
        /// </summary>
        public void Add(double px, double py, double qx, double qy)
        {
            CalibPointList.Add(new CalibPointData(px, py, qx, qy));
        }

        /// <summary>
        /// 覆盖
        /// </summary>
        public void Cover(int index, double px, double py, double qx, double qy)
        {
            if ((index >= 0) && (index < CalibPointList.Count))
            {
                CalibPointList[index] = new CalibPointData(px, py, qx, qy);
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

            Matrix = posMatrix;

            if (CalibParam.IsValid)
            {
                MetroDialog.ShowMessageDialog(this, "标定成功!", $"点击\"确定\"将保存当前配置到文件之中");
            }
            else
            {
                MetroDialog.ShowMessageDialog(this, "标定失败!", "请检查相关的数据");
            }
        }

        /// <summary>
        /// 选择项改变
        /// </summary>
        /// <param name="index"></param>
        public void CalibPointListSelectionChanged(int index)
        {
            if ((index >= 0) && (index < CalibPointList.Count))
            {
                Px = CalibPointList[index].Px;
                Py = CalibPointList[index].Py;
                Qx = CalibPointList[index].Qx;
                Qy = CalibPointList[index].Qy;
            }
        }

        /// <summary>
        /// 触发标定配置完成事件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="calibParam">标定参数</param>
        /// <param name="isSuccess">成功标志</param>
        protected void OnCalibrationConfigurationCompleted(string filePath, CalibParam calibParam, bool isSuccess)
        {
            CalibrationConfigurationCompleted?.Invoke(this, new CalibrationConfigurationCompletedEventArgs(filePath, calibParam, isSuccess));

        }

        /// <summary>
        /// 确认
        /// </summary>
        public void Accept()
        {
            //保存结果
            JsonSerialization.SerializeObjectToFile(CalibParam, FilePath);

            //触发"创建标定文件"事件
            OnCalibrationConfigurationCompleted(FilePath, CalibParam, CalibParam.IsValid);

        }

        /// <summary>
        /// 触发标定配置取消事件
        /// </summary>
        protected void OnCalibrationConfigurationCanceled()
        {
            CalibrationConfigurationCanceled?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// 取消
        /// </summary>
        public void Cancel()
        {
            OnCalibrationConfigurationCanceled();
        }

        #endregion

    }
}
