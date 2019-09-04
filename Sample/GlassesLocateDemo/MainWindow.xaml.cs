using Framework.Infrastructure.Logging;
using Framework.Infrastructure.Logging.Log4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisionPlatform.BaseType;
using VisionPlatform.Core;
using VisionPlatform.Wpf;
using Framework.TcpSocket;
using System.Net;

namespace GlassesLocateDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 场景管理器
        /// </summary>
        public SceneManager SceneManager { get; set; }

        /// <summary>
        /// 场景配置窗口
        /// </summary>
        public Window SceneConfigWindow { get; set; }

        /// <summary>
        /// TCPSock客户端
        /// </summary>
        public TcpSocketClient RobotTcpSocketClient { get; set; }

        /// <summary>
        /// 机器人通信连接标志
        /// </summary>
        public bool IsRobotConnect { get; set; } = false;

        private int localPort = 1000;

        public MainWindow()
        {
            InitializeComponent();

            //创建日志接口
            Logging.Init(new Log4Net());
            Logging.LoadConfigFile("LogConfig.xml");

            //更新视觉框架集合
            VisionFrameFactory.UpdateAssembly();
            VisionFrameFactory.DefaultVisionFrameType = EVisionFrameType.Halcon;

            //更新相机框架集合
            CameraFactory.UpdateAssembly();
            CameraFactory.DefaultCameraSdkType = ECameraSdkType.VirtualCamera;

            if ((VisionFrameFactory.DefaultVisionFrameType != EVisionFrameType.VisionPro) && (CameraFactory.DefaultCameraSdkType == ECameraSdkType.VirtualCamera))
            {
                CameraFactory.AddCamera(@"C:\Users\Public\Documents\MVTec\HALCON-17.12-Progress\examples\images");
                CameraFactory.AddCamera(@"E:\测试图像\刹车片");
                CameraFactory.AddCamera(@"E:\测试图像\AGV标定板");
                CameraFactory.AddCamera(@"E:\测试图像\眼镜");
                CameraFactory.AddCamera(@"E:\测试图像\定位圆");
                CameraFactory.AddCamera(@"E:\测试图像\眼镜腿");
            }

            //获取场景管理器实例(单例)
            SceneManager = SceneManager.GetInstance();

            //恢复场景
            SceneManager.RecoverScenes();

            ScenesListView.Items.Clear();
            foreach (var item in SceneManager.Scenes.Values)
            {
                ScenesListView.Items.Add(item);
            }

            //更新相关控件
            UpdateRobotConnectionControl(IsRobotConnect);
        }

        /// <summary>
        /// 客户端会话
        /// </summary>
        public List<TcpSocketSession> RobotClientSocketSession { get; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void AddSceneButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var view = new SceneView(new Scene("EmptyScene", VisionFrameFactory.DefaultVisionFrameType), false)
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };

                var viewModel = (view.DataContext as SceneViewModel);

                //注册场景配置完成事件
                viewModel.SceneConfigurationCompleted += ViewModel_SceneConfigurationCompleted;

                //将控件嵌入窗口之中
                SceneConfigWindow = new Window();
                SceneConfigWindow.MinWidth = 800;
                SceneConfigWindow.MinHeight = 500;
                SceneConfigWindow.Width = 800;
                SceneConfigWindow.Height = 500;
                SceneConfigWindow.Content = view;
                SceneConfigWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                SceneConfigWindow.Owner = Window.GetWindow(this);
                SceneConfigWindow.Title = "场景配置窗口";
                SceneConfigWindow.WindowState = WindowState.Maximized;

                SceneConfigWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// 场景配置完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewModel_SceneConfigurationCompleted(object sender, SceneConfigurationCompletedEventArgs e)
        {
            try
            {
                if (e.Scene != null)
                {
                    //注册场景
                    SceneManager.RegisterScene(e.Scene);

                    ScenesListView.Items.Clear();
                    foreach (var item in SceneManager.Scenes.Values)
                    {
                        ScenesListView.Items.Add(item);
                    }
                }

                SceneConfigWindow.Close();
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
                MessageBox.Show(ex.Message);
            }

        }

        private void ModifySceneButton_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (ScenesListView.SelectedIndex < 0)
                {
                    MessageBox.Show("无效场景");
                    return;
                }

                var view = new SceneView(ScenesListView.SelectedItem as Scene, true)
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };

                var viewModel = (view.DataContext as SceneViewModel);

                //注册场景配置完成事件
                viewModel.SceneConfigurationCompleted += ViewModel_SceneConfigurationCompleted;

                //将控件嵌入窗口之中
                SceneConfigWindow = new Window();
                SceneConfigWindow.MinWidth = 800;
                SceneConfigWindow.MinHeight = 500;
                SceneConfigWindow.Width = 800;
                SceneConfigWindow.Height = 500;
                SceneConfigWindow.Content = view;
                SceneConfigWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                SceneConfigWindow.Owner = Window.GetWindow(this);
                SceneConfigWindow.Title = "场景配置窗口";
                SceneConfigWindow.WindowState = WindowState.Maximized;

                SceneConfigWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
                MessageBox.Show(ex.Message);
            }

        }

        private void DeleteSceneButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ScenesListView.SelectedIndex < 0)
                {
                    MessageBox.Show("无效场景");
                    return;
                }

                if (MessageBox.Show("是否要删除当前场景?", "删除确认", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    var scene = ScenesListView.SelectedItem as Scene;
                    SceneManager.DeleteScene(scene.Name);

                    ScenesListView.Items.Clear();
                    foreach (var item in SceneManager.Scenes.Values)
                    {
                        ScenesListView.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
                MessageBox.Show(ex.Message);
            }

        }

        private void ShowResult(RunStatus runStatus, string result)
        {
            ProcessTimeLabel.Content = runStatus.ProcessingTime.ToString("0.### MS");
            TotalTimeLabel.Content = runStatus.TotalTime.ToString("0.### MS");
            ResultLabel.Content = runStatus.Result.ToString();
            MessageLabel.Content = runStatus.Message;
            VisionResultTextBox.Text = result;
        }

        private void ExecuteSceneButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string result;
                var scene1 = ScenesListView.SelectedItem as Scene;

                if (scene1 == null)
                {
                    return;
                }

                if (RunningWindow1.Content != scene1.VisionFrame.RunningWindow)
                {
                    RunningWindow1.Content = scene1.VisionFrame.RunningWindow;

                }
                RunStatus runStatus = scene1.Execute(1000, out result);
                ShowResult(runStatus, result);

                if (runStatus.Result == EResult.Error)
                {
                    MessageBox.Show(runStatus.Message);
                }

            }
            catch (Exception ex)
            {
                Logging.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void OpenCalibButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var view = new AdvanceCalibrationView()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };

                var viewModel = (view.DataContext as AdvanceCalibrationViewModel);

                //将控件嵌入窗口之中
                var window = new Window();
                window.MinWidth = view.MinWidth + 50;
                window.MinHeight = view.MinHeight + 50;
                window.MaxWidth = view.MaxWidth;
                window.MaxHeight = view.MaxHeight;
                window.Width = view.MinWidth + 50;
                window.Height = view.MinHeight + 50;
                window.Content = view;
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                window.Owner = Window.GetWindow(this);
                window.Title = "标定窗口";
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
                MessageBox.Show(ex.Message);
            }

        }

        private void UpdateRobotConnectionControl(bool isRobotConnect)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (isRobotConnect)
                {
                    ConnectRobotButton.Content = "断开";
                    RobotConnectionInputDockPanel.IsEnabled = false;
                }
                else
                {
                    ConnectRobotButton.Content = "连接";
                    RobotConnectionInputDockPanel.IsEnabled = true;
                }
            });

        }

        private void ConnectRobotButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsRobotConnect)
                {
                    //获取本地IP名
                    string hostName = Dns.GetHostName();
                    IPHostEntry localhost = Dns.GetHostEntry(hostName);

                    var tcpSocketClientConfiguration = new TcpSocketClientConfiguration();
                    tcpSocketClientConfiguration.FrameBuilder = new RawBufferFrameBuilder();
                    RobotTcpSocketClient = new TcpSocketClient(IPAddress.Parse(RobotIP.Text), int.Parse(RobotPort.Text), localhost.AddressList[localhost.AddressList.Length - 1], localPort++, tcpSocketClientConfiguration);

                    RobotTcpSocketClient.ServerConnected += RobotTcpSocketClient_ServerConnected;
                    RobotTcpSocketClient.ServerDisconnected += RobotTcpSocketClient_ServerDisconnected;
                    RobotTcpSocketClient.ServerDataReceived += RobotTcpSocketClient_ServerDataReceived;
                    RobotTcpSocketClient.Connect();
                }
                else
                {
                    RobotTcpSocketClient.Shutdown();
                    RobotTcpSocketClient.Dispose();
                    RobotTcpSocketClient = null;
                    IsRobotConnect = false;

                    UpdateRobotConnectionControl(IsRobotConnect);
                }

            }
            catch (Exception ex)
            {
                Logging.Error(ex);
                MessageBox.Show(ex.Message);
            }

        }

        private void RobotTcpSocketClient_ServerDataReceived(object sender, TcpServerDataReceivedEventArgs e)
        {
            //获取机器人消息
            string Message = Encoding.UTF8.GetString(e.Data, e.DataOffset, e.DataLength);
            
            if (SceneManager.Scenes.ContainsKey(Message.TrimEnd('$')))
            {

                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (RunningWindow1.Content != SceneManager.Scenes[Message.TrimEnd('$')].VisionFrame.RunningWindow)
                    {
                        RunningWindow1.Content = SceneManager.Scenes[Message.TrimEnd('$')].VisionFrame.RunningWindow;
                    }

                    string visionResult;
                    RunStatus runStatus = SceneManager.Scenes[Message.TrimEnd('$')].Execute(2000, out visionResult);
                    ShowResult(runStatus, visionResult);

                    if (runStatus.Result == EResult.Accept)
                    {
                        //返回执行结果
                        e.Client.Send(Encoding.UTF8.GetBytes($"{visionResult}$"));
                    }
                    else
                    {
                        e.Client.Send(Encoding.UTF8.GetBytes("NG$"));
                    }
                });
            }
            else
            { 
                //返回错误信息
                e.Client.Send(Encoding.UTF8.GetBytes("Invalid Command$"));
            }
        }

        private void RobotTcpSocketClient_ServerDisconnected(object sender, TcpServerDisconnectedEventArgs e)
        {
            IsRobotConnect = false;
            UpdateRobotConnectionControl(IsRobotConnect);

            RobotTcpSocketClient.ServerConnected -= RobotTcpSocketClient_ServerConnected;
            RobotTcpSocketClient.ServerDisconnected -= RobotTcpSocketClient_ServerDisconnected;
            RobotTcpSocketClient.ServerDataReceived -= RobotTcpSocketClient_ServerDataReceived;

            RobotTcpSocketClient.Shutdown();
            RobotTcpSocketClient.Dispose();
        }

        private void RobotTcpSocketClient_ServerConnected(object sender, TcpServerConnectedEventArgs e)
        {
            IsRobotConnect = true;
            UpdateRobotConnectionControl(IsRobotConnect);
        }
    }
}
