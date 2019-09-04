using Framework.TcpSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;


namespace GlassesLocateDemo
{
    public class ServerSocket
    {
        /// <summary>
        /// 服务器Socket
        /// </summary>
        public TcpSocketServer TcpSocketServer { get; private set; }

        /// <summary>
        /// 已连接的客户端列表
        /// </summary>
        public List<TcpSocketSession> ClientSessions { get; private set; }

        /// <summary>
        /// 创建服务器端口新实例
        /// </summary>
        /// <param name="LocalIP"></param>
        /// <param name="Port"></param>
        public ServerSocket(IPAddress LocalIP, int Port)
        {
            StartServer(LocalIP, Port);

        }

        /// <summary>
        /// 启动服务器
        /// </summary>
        /// <param name="LocalIP">本地服务器IP</param>
        /// <param name="Port">服务器端口</param>
        private bool StartServer(IPAddress LocalIP, int Port)
        {
            try
            {
                //获取本地IP名
                string hostName = Dns.GetHostName();
                IPHostEntry localhost = Dns.GetHostEntry(hostName);

                //校验参数
                foreach (var item in localhost.AddressList)
                {
                    if (Equals(item, LocalIP))
                    {
                        var ServerConfig = new TcpSocketServerConfiguration();
                        ServerConfig.FrameBuilder = new RawBufferFrameBuilder();

                        TcpSocketServer = new TcpSocketServer(LocalIP, Port, ServerConfig);
                        TcpSocketServer.ClientConnected += ClientConnectedCallback;
                        TcpSocketServer.ClientDisconnected += ClientDisconnectedCallback;
                        TcpSocketServer.ClientDataReceived += ClientDataReceivedCallback;
                        TcpSocketServer.Listen();

                        Console.WriteLine($"Start: {LocalIP}:{Port}");

                        return true;
                    }
                }
            }
            catch (Exception)
            {

            }

            return false;
        }

        /// <summary>
        /// 客户端连接回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientConnectedCallback(object sender, TcpClientConnectedEventArgs e)
        {
            try
            {
                ClientSessions.Add(e.Session);
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        /// <summary>
        /// 客户端断开连接回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientDisconnectedCallback(object sender, TcpClientDisconnectedEventArgs e)
        {
            try
            {
                ClientSessions.Remove(e.Session);
            }
            catch (Exception)
            {

            }
            
        }

        /// <summary>
        /// 数据接收回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientDataReceivedCallback(object sender, TcpClientDataReceivedEventArgs e)
        {
            try
            {
                //获取机器人消息
                string Message = Encoding.UTF8.GetString(e.Data, e.DataOffset, e.DataLength);

                //回发接收到的消息
                e.Session.Send(Encoding.UTF8.GetBytes(Message));

            }
            catch (Exception)
            {

            }

        }

    }
}
