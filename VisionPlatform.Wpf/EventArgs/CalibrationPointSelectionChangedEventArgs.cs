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
    /// 标定点选择项改变事件
    /// </summary>
    public class CalibrationPointSelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 创建CalibrationPointSelectionChangedEventArgs新实例
        /// </summary>
        /// <param name="calibPointList">标定点列表</param>
        /// <param name="index">点位索引</param>
        /// <param name="calibPointData">标定点点位数据</param>
        public CalibrationPointSelectionChangedEventArgs(ObservableCollection<CalibPointData> calibPointList, int index, CalibPointData calibPointData)
        {
            CalibPointList = calibPointList;
            Index = index;
            CalibPointData = calibPointData;
        }

        /// <summary>
        /// 标定点列表
        /// </summary>
        public ObservableCollection<CalibPointData> CalibPointList { get; }

        /// <summary>
        /// 点位索引
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// 标定点点位数据
        /// </summary>
        public CalibPointData CalibPointData { get; }
    }
}
