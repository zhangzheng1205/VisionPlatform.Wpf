using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionPlatform.IRobot;

namespace RandomRobotLocation
{
    public class RobotComunication : IRobotCommunication
    {
        private Random random = new Random();

        /// <summary>
        /// 连接标志
        /// </summary>
        public bool IsConnect { get; private set; }

        /// <summary>
        /// 连接到机器人
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="port">端口</param>
        /// <returns>执行结果</returns>
        public bool Connect(string ip, int port)
        {
            IsConnect = true;
            return IsConnect;
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            IsConnect = false;
        }

        /// <summary>
        /// 获取机器人位置
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="z">Z</param>
        /// <returns>执行结果</returns>
        public bool GetRobotLocation(out double x, out double y, out double z)
        {
            x = -1;
            y = -1;
            z = -1;

            if (IsConnect)
            {
                x = random.Next(0, 500000) / 1000.0;
                y = random.Next(0, 500000) / 1000.0;
                z = random.Next(0, 500000) / 1000.0;
                return true;
            }

            return false;
        }

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
        public bool GetRobotLocation(out double x, out double y, out double z, out double yaw, out double pitch, out double roll)
        {
            x = -1;
            y = -1;
            z = -1;
            yaw = -1;
            pitch = -1;
            roll = -1;

            if (IsConnect)
            {
                x = random.Next(0, 500000) / 1000.0;
                y = random.Next(0, 500000) / 1000.0;
                z = random.Next(0, 500000) / 1000.0;
                yaw = random.Next(0, 500000) / 1000.0;
                pitch = random.Next(0, 500000) / 1000.0;
                roll = random.Next(0, 500000) / 1000.0;
                return true;
            }

            return false;
        }
    }
}
