using Framework.Vision;
using System;
using System.Collections.ObjectModel;

namespace VisionPlatform.Wpf
{
    /// <summary>
    /// 标定定列表改变事件参数
    /// </summary>
    public class CalibrationPointListChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 创建CalibrationPointListChangedEventArgs新实例
        /// </summary>
        /// <param name="calibPointList">标定点列表</param>
        public CalibrationPointListChangedEventArgs(ObservableCollection<CalibPointData> calibPointList)
        {
            CalibPointList = calibPointList;
        }

        /// <summary>
        /// 标定点列表
        /// </summary>
        public ObservableCollection<CalibPointData> CalibPointList { get; }
    }
}
