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
        /// 清除所有的数据
        /// </summary>
        public void Clear()
        {

        }

        #endregion

    }
}
