using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionPlatform.Robot
{
    /// <summary>
    /// 机器人通信接口
    /// </summary>
    public interface IRobotCommunication
    {
        /// <summary>
        /// 连接到机器人
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="port">端口</param>
        /// <returns>执行结果</returns>
        bool Connect(string ip, int port);

        /// <summary>
        /// 断开连接
        /// </summary>
        void Disconnect();

        /// <summary>
        /// 获取机器人位置
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="z">Z</param>
        /// <returns>执行结果</returns>
        bool GetRobotLocation(out double x, out double y, out double z);

        /// <summary>
        /// 获取机器人位置
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="z">Z</param>
        /// <param name="yaw">Yaw</param>
        /// <param name="pitch">Pitch</param>
        /// <param name="roll">Roll</param>
        /// <returns>执行结果</returns>
        bool GetRobotLocation(out double x, out double y, out double z, out double yaw, out double pitch, out double roll);

    }
}
