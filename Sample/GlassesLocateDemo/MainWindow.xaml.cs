using Framework.Infrastructure.Logging;
using Framework.Infrastructure.Logging.Log4Net;
using Framework.Infrastructure.Serialization;
using Framework.TcpSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using VisionPlatform.BaseType;
using VisionPlatform.Core;
using VisionPlatform.Wpf;

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

        public SystemParams SystemParams { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            SystemParams = new SystemParams();

            if (File.Exists("SystemParams.json"))
            {
                var obj = JsonSerialization.DeserializeObjectFromFile<SystemParams>("SystemParams.json");

                if (obj != null)
                {
                    SystemParams = obj;
                }
            }
            else
            {
                JsonSerialization.SerializeObjectToFile(SystemParams, "SystemParams.json");
            }


            //创建日志接口
            Logging.Init(new Log4Net());
            Logging.LoadConfigFile("LogConfig.xml");

            //更新视觉框架集合
            VisionFrameFactory.UpdateAssembly();
            VisionFrameFactory.DefaultVisionFrameType = SystemParams.DefaultVisionFrameType;

            //更新相机框架集合
            CameraFactory.UpdateAssembly();
            CameraFactory.DefaultCameraSdkType = SystemParams.DefaultCameraSdkType;

            if ((VisionFrameFactory.DefaultVisionFrameType != EVisionFrameType.VisionProVpp) && (CameraFactory.DefaultCameraSdkType == ECameraSdkType.VirtualCamera))
            {
                try
                {
                    CameraFactory.AddCamera(@"C:\Users\Public\Documents\MVTec\HALCON-17.12-Progress\examples\images");
                    CameraFactory.AddCamera(@"D:\20190904CXC\图片");
                    //CameraFactory.AddCamera(@"E:\测试图像\刹车片");
                    //CameraFactory.AddCamera(@"E:\测试图像\AGV标定板");
                    //CameraFactory.AddCamera(@"E:\测试图像\眼镜");
                    //CameraFactory.AddCamera(@"E:\测试图像\定位圆");
                    //CameraFactory.AddCamera(@"E:\测试图像\眼镜腿");
                }
                catch (Exception ex)
                {
                    Logging.Error(ex);
                    MessageBox.Show(ex.Message);
                }
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
            //加载训练集

            try
            {
                SrVision.SRV_Init(SystemParams.SrDetFilePath);
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CameraFactory.RemoveAllCameras();
            try
            {
                SrVision.SRV_Dispose();
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
            }

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

        /// <summary>
        /// 执行场景
        /// </summary>
        /// <param name="scene"></param>
        private RunStatus ExecuteScene(Scene scene, out string visionResult, out int glassesType)
        {
            visionResult = "";
            glassesType = -1;

            if (RunningWindow1.Content != scene.VisionFrame.RunningWindow)
            {
                RunningWindow1.Content = scene.VisionFrame.RunningWindow;
            }


            RunStatus runStatus = scene.Execute(2000, out visionResult);
            ShowResult(runStatus, visionResult);

            if (runStatus.Result == EResult.Accept)
            {
                if (File.Exists(SystemParams.DefaultImageFile))
                {
                    try
                    {
                        try
                        {
                            glassesType = SrVision.SRV_Run(SystemParams.DefaultImageFile);
                        }
                        catch (Exception ex)
                        {
                            Logging.Error(ex);
                        }
                        UpdateGlassesType(glassesType);
                    }
                    catch (Exception)
                    {
                        UpdateGlassesType(-1);
                    }
                }
            }
            else if(runStatus.Result == EResult.Error)
            {
                UpdateGlassesType(-1);
                MessageBox.Show(runStatus.Message);
            }
            else
            {
                UpdateGlassesType(-1);
            }

            return runStatus;
        }

        private void ExecuteSceneButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string result;
                int glassesType = -1;
                var scene1 = ScenesListView.SelectedItem as Scene;

                if (scene1 == null)
                {
                    return;
                }

                ExecuteScene(scene1, out result, out glassesType);
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
                    var tcpSocketClientConfiguration = new TcpSocketClientConfiguration();
                    tcpSocketClientConfiguration.FrameBuilder = new RawBufferFrameBuilder();
                    RobotTcpSocketClient = new TcpSocketClient(IPAddress.Parse(RobotIP.Text), int.Parse(RobotPort.Text), IPAddress.Parse(SystemParams.IP), localPort++, tcpSocketClientConfiguration);

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
        private void UpdateGlassesType(int glassesType)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (glassesType >= 0)
                {
                    GlassesTypeLabel.Content = glassesType.ToString();
                }
                else
                {
                    GlassesTypeLabel.Content = "无效结果";
                }
            });

        }
        private void RobotTcpSocketClient_ServerDataReceived(object sender, TcpServerDataReceivedEventArgs e)
        {
            //获取机器人消息
            string Message = Encoding.UTF8.GetString(e.Data, e.DataOffset, e.DataLength);
            Application.Current.Dispatcher.Invoke(() =>
            {
                RecvMsgTextBox.Text = $"{DateTime.Now}:{Message}";
            });
            if (SceneManager.Scenes.ContainsKey(Message.TrimEnd('$')))
            {

                Application.Current.Dispatcher.Invoke(() =>
                {
                    string visionResult;
                    int glassesType = -1;
                    RunStatus runStatus = ExecuteScene(SceneManager.Scenes[Message.TrimEnd('$')], out visionResult, out glassesType);

                    if (runStatus.Result == EResult.Accept)
                    {
                        //返回执行结果
                        e.Client.Send(Encoding.UTF8.GetBytes($"{glassesType},{visionResult}$"));
                        SendMsgTextBox.Text = $"{DateTime.Now}:{glassesType},{visionResult}$";
                    }
                    else
                    {
                        UpdateGlassesType(-1);
                        e.Client.Send(Encoding.UTF8.GetBytes("NG$"));
                        SendMsgTextBox.Text = $"{DateTime.Now}:NG$";
                    }
                });
            }
            else
            {
                //返回错误信息
                e.Client.Send(Encoding.UTF8.GetBytes("Invalid Command$"));
                
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SendMsgTextBox.Text = $"{DateTime.Now}:Invalid Command$";
                });
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
