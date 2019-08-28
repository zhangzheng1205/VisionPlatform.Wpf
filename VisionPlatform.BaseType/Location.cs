using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionPlatform.BaseType
{
    /// <summary>
    /// 位置
    /// </summary>
    public struct Location
    {
        /// <summary>
        /// X
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 角度
        /// </summary>
        public double Angle { get; set; }

        /// <summary>
        /// 创建Location新实例
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="angle">角度</param>
        public Location(double x, double y, double angle)
        {
            X = x;
            Y = y;
            Angle = angle;
        }

    }
}
