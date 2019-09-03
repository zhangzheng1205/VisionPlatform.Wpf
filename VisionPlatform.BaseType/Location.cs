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

        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString()
        {
            return ToString(','); 
        }

        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <param name="separatorChar">分隔符</param>
        /// <returns>结果字符串</returns>
        public string ToString(char separatorChar)
        {
            return $"{X:0.###}{separatorChar}{Y:0.###}{separatorChar}{Angle:0.###}";
        }

    }
}
