using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VisionPlatform.Wpf.Converters
{
    /// <summary>
    /// 标定矩阵转字符串
    /// </summary>
    internal class MatrixConverter : IValueConverter
    {

        /// <summary>
        /// 获取标定矩阵字符串
        /// </summary>
        /// <param name="matrix">矩阵</param>
        private string GetDisplayMatrix(double[] matrix)
        {
            try
            {
                if ((matrix != null) && (matrix.Length > 0))
                {
                    //显示标定结果
                    string MatrixString = "";

                    for (int i = 0; i < matrix.Length - 1; i++)
                    {
                        MatrixString += matrix[i].ToString("F4") + ",";
                    }

                    MatrixString += matrix[matrix.Length - 1].ToString("F4");

                    return MatrixString;
                }
                else
                {

                }
            }
            catch (Exception)
            {

            }
            return "NaN,NaN,NaN,NaN,NaN,NaN";
        }

        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return GetDisplayMatrix(value as double[]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
