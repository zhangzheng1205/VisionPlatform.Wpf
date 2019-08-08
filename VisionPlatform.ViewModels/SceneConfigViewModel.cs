using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows;
using Caliburn.Micro;
using Framework.Camera;
using Framework.Infrastructure.Serialization;
using VisionPlatform.Base;
using VisionPlatform.Views;

namespace VisionPlatform.ViewModels
{
    public class SceneConfigViewModel : Screen
    {
        #region 构造函数

        /// <summary>
        /// 创建SceneConfigViewModel新实例
        /// </summary>
        public SceneConfigViewModel() : this(null)
        {

        }

        /// <summary>
        /// 场景名
        /// </summary>
        /// <param name="sceneName"></param>
        public SceneConfigViewModel(IScene scene)
        {
            Scene = scene;

            //刷新列表
            UpdateFileList();

        }

        #endregion

        #region 属性

        #region 文件列表

        /// <summary>
        /// 视觉算子DLL目录
        /// </summary>
        public string VisionOperaDllDirectory { get; set; } = "./";

        /// <summary>
        /// 标定文件目录
        /// </summary>
        public string CalibFileDirectory { get; set; } = "./";

        /// <summary>
        /// 视觉输入参数文件目录
        /// </summary>
        public string VisionInputFileDirectory { get; set; } = "./";

        /// <summary>
        /// 视觉输出参数文件目录
        /// </summary>
        public string VisionOutputFileDirectory { get; set; } = "./";

        /// <summary>
        /// 相机配置文件目录
        /// </summary>
        public string CameraConfigFileDirectory { get; set; } = "./";

        /// <summary>
        /// 标定文件列表
        /// </summary>
        private ObservableCollection<string> calibFileList = new ObservableCollection<string>();

        /// <summary>
        /// 标定文件列表
        /// </summary>
        public ObservableCollection<string> CalibFileList
        {
            get
            {
                return calibFileList;
            }
            set
            {
                calibFileList = value;
                NotifyOfPropertyChange(() => CalibFileList);
            }
        }

        /// <summary>
        /// 输入参数文件列表
        /// </summary>
        private ObservableCollection<string> inputParamFileList = new ObservableCollection<string>();

        /// <summary>
        /// 输入参数文件列表
        /// </summary>
        public ObservableCollection<string> InputParamFileList
        {
            get
            {
                return inputParamFileList;
            }
            set
            {
                inputParamFileList = value;
                NotifyOfPropertyChange(() => InputParamFileList);
            }
        }

        /// <summary>
        /// 输出参数文件列表
        /// </summary>
        private ObservableCollection<string> outputParamFileList = new ObservableCollection<string>();

        /// <summary>
        /// 输出参数文件列表
        /// </summary>
        public ObservableCollection<string> OutputParamFileList
        {
            get
            {
                return outputParamFileList;
            }
            set
            {
                outputParamFileList = value;
                NotifyOfPropertyChange(() => OutputParamFileList);
            }
        }

        /// <summary>
        /// 相机列表
        /// </summary>
        private ObservableCollection<DeviceInfo> cameraList = new ObservableCollection<DeviceInfo>();

        /// <summary>
        /// 相机列表
        /// </summary>
        public ObservableCollection<DeviceInfo> CameraList
        {
            get
            {
                return cameraList;
            }
            set
            {
                cameraList = value;
                NotifyOfPropertyChange(() => CameraList);
            }
        }

        /// <summary>
        /// 相机配置文件列表
        /// </summary>
        private ObservableCollection<string> cameraConfigFileList = new ObservableCollection<string>();

        /// <summary>
        /// 相机配置文件列表
        /// </summary>
        public ObservableCollection<string> CameraConfigFileList
        {
            get
            {
                return cameraConfigFileList;
            }
            set
            {
                cameraConfigFileList = value;
                NotifyOfPropertyChange(() => CameraConfigFileList);
            }
        }

        #endregion

        #region 场景配置参数

        /// <summary>
        /// 场景
        /// </summary>
        public IScene Scene { get; set; }

        /// <summary>
        /// 场景有效标志
        /// </summary>
        private bool isSceneValid = false;

        /// <summary>
        /// 场景有效标志
        /// </summary>
        public bool IsSceneValid
        {
            get
            {
                return isSceneValid;
            }
            set
            {
                isSceneValid = value;
                NotifyOfPropertyChange(() => IsSceneValid);
            }
        }

        /// <summary>
        /// 场景名
        /// </summary>
        public string SceneName
        {
            get
            {
                return Scene?.Name;
            }

            set
            {
                Scene.Name = value;
                NotifyOfPropertyChange(() => SceneName);
            }
        }

        /// <summary>
        /// 视觉算子DLL名
        /// </summary>
        public string VisionOperaDllName
        {
            get
            {
                return Path.GetFileName(VisionOperaDLLPath);
            }
            set
            {
                NotifyOfPropertyChange(() => VisionOperaDllName);
            }
        }

        /// <summary>
        /// 视觉算子DLL路径
        /// </summary>
        public string VisionOperaDLLPath
        {
            get
            {
                return Scene?.VisionOperaDLLPath;
            }
            set
            {
                try
                {
                    Scene.VisionOperaDLLPath = value;
                    NotifyOfPropertyChange(() => VisionOperaDLLPath);
                    NotifyOfPropertyChange(() => VisionOperaDllName);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        /// <summary>
        /// 使能视觉
        /// </summary>
        private bool isEnableVision;

        /// <summary>
        /// 使能视觉
        /// </summary>
        public bool IsEnableVision
        {
            get
            {
                return isEnableVision;
            }
            set
            {
                isEnableVision = value;
                NotifyOfPropertyChange(() => IsEnableVision);
            }
        }

        /// <summary>
        /// 标定文件
        /// </summary>
        private string calibFile;

        /// <summary>
        /// 标定文件
        /// </summary>
        public string CalibFile
        {
            get
            {
                return calibFile;
            }
            set
            {
                calibFile = value;
                NotifyOfPropertyChange(() => CalibFile);
            }
        }

        /// <summary>
        /// 输入参数文件
        /// </summary>
        private string inputParamFile;

        /// <summary>
        /// 输入参数文件
        /// </summary>
        public string InputParamFile
        {
            get
            {
                return inputParamFile;
            }
            set
            {
                inputParamFile = value;
                NotifyOfPropertyChange(() => InputParamFile);
            }
        }

        /// <summary>
        /// 输出参数文件
        /// </summary>
        private string outputParamFile;

        /// <summary>
        /// 输出参数文件
        /// </summary>
        public string OutputParamFile
        {
            get
            {
                return outputParamFile;
            }
            set
            {
                outputParamFile = value;
                NotifyOfPropertyChange(() => OutputParamFile);
            }
        }

        /// <summary>
        /// 相机编号
        /// </summary>
        public DeviceInfo selectedCamera;

        /// <summary>
        /// 相机编号
        /// </summary>
        public DeviceInfo SelectedCamera
        {
            get
            {
                return selectedCamera;
            }
            set
            {
                selectedCamera = value;
                NotifyOfPropertyChange(() => SelectedCamera);
            }
        }

        /// <summary>
        /// 相机配置文件
        /// </summary>
        private string cameraConfigFile;

        /// <summary>
        /// 相机配置文件
        /// </summary>
        public string CameraConfigFile
        {
            get
            {
                return cameraConfigFile;
            }
            set
            {
                cameraConfigFile = value;
                NotifyOfPropertyChange(() => CameraConfigFile);
            }
        }

        #endregion

        #endregion

        #region 事件

        /// <summary>
        /// 完成场景配置事件
        /// </summary>
        public event EventHandler<CompletedSceneConfigEventArgs> CompletedSceneConfigEventHandler;

        #endregion

        #region 方法

        /// <summary>
        /// 显示标定窗口
        /// </summary>
        /// <param name="fileName"></param>
        private void ShowCalibrationWindow(string fileName)
        {
            //打开标定窗口
            Window window = new EmptyMetroWindow();
            var viewModel = new CalibrationViewModel();
            var control = new CalibrationView
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                DataContext = viewModel
            };
            window.MinWidth = control.MinWidth + 50;
            window.MinHeight = control.MinHeight + 50;
            window.Width = control.MinWidth + 50;
            window.Height = control.MinHeight + 50;
            window.Content = control;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //window.Owner = Window.GetWindow(this);
            window.Title = "标定窗口";

            if (File.Exists(fileName))
            {
                viewModel.LoadFile(fileName);
            }
            else
            {
                viewModel.CreateFile(fileName);
            }

            viewModel.CreateCalibFileEventHandler += delegate (object sender, CreateCalibFileEventArgs e)
            {
                //序列化标定参数
                JsonSerialization.SerializeObjectToFile(e.CalibParam, fileName);

                //刷新文件列表
                UpdateFileList();

                window.Close();
            };

            viewModel.CancelEventHandler += delegate (object sender, EventArgs e)
            {
                //刷新文件列表
                UpdateFileList();

                window.Close();
            };

            window.ShowDialog();
        }

        /// <summary>
        /// 显示参数列表
        /// </summary>
        /// <param name="configParamList">配置参数列表</param>
        private void ShowParamConfigWindow(string fileName, ItemCollection configParamList)
        {
            //打开标定窗口
            Window window = new EmptyMetroWindow();
            var viewModel = new ParamConfigViewModel(configParamList);
            var control = new ParamConfigView
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                DataContext = viewModel
            };
            window.MinWidth = control.MinWidth + 50;
            window.MinHeight = control.MinHeight + 50;
            window.Width = control.MinWidth + 50;
            window.Height = control.MinHeight + 50;
            window.Content = control;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //window.Owner = Window.GetWindow(this);
            window.Title = "参数配置窗口";

            viewModel.CompletedConfigurationEventHandler += delegate (object sender, CompletedConfigurationEventArgs e)
            {
                //序列化配置参数
                JsonSerialization.SerializeObjectToFile(e.ConfigParamList, fileName);

                //刷新文件列表
                UpdateFileList();

                window.Close();
            };

            viewModel.CancelEventHandler += delegate (object sender, EventArgs e)
            {
                //刷新文件列表
                UpdateFileList();

                window.Close();
            };

            window.ShowDialog();

        }

        /// <summary>
        /// 显示输入参数配置窗口
        /// </summary>
        /// <param name="configParamList">参数配置列表</param>
        private void ShowInputParamConfigWindow(string fileName)
        {
            ItemCollection configParamList = null;

            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            var filePath = $"{VisionInputFileDirectory}/{fileName}.json";

            if (File.Exists(filePath))
            {
                configParamList = JsonSerialization.DeserializeObjectFromFile<ItemCollection>(filePath);
            }

            if (configParamList == null)
            {
                //从视觉算子Dll中获取默认配置
                var visionOperaAssembly = Assembly.LoadFrom(Scene.VisionOperaDLLPath);
                var types = visionOperaAssembly.GetTypes();
                object obj = new object();
                foreach (var item in types)
                {
                    if (item.Name == "VisionOpera")
                    {
                        obj = visionOperaAssembly.CreateInstance(item.FullName);
                        break;
                    }
                }

                //若视觉算子DLL有效,则创建默认配置
                if (obj is IVisionOpera)
                {
                    //创建默认配置
                    var visionOpera = obj as IVisionOpera;
                    configParamList = visionOpera.Inputs;
                }
                else
                {
                    return;
                }
            }

            ShowParamConfigWindow(filePath, configParamList);

        }

        /// <summary>
        /// 显示输出参数配置窗口
        /// </summary>
        /// <param name="configParamList">参数配置列表</param>
        private void ShowOutputParamConfigWindow(string fileName)
        {
            ItemCollection configParamList = null;

            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            var filePath = $"{VisionOutputFileDirectory}/{fileName}.json";

            if (File.Exists(filePath))
            {
                configParamList = JsonSerialization.DeserializeObjectFromFile<ItemCollection>(filePath);
            }

            if (configParamList == null)
            {
                //从视觉算子Dll中获取默认配置
                var visionOperaAssembly = Assembly.LoadFrom(Scene.VisionOperaDLLPath);
                var types = visionOperaAssembly.GetTypes();
                object obj = new object();
                foreach (var item in types)
                {
                    if (item.Name == "VisionOpera")
                    {
                        obj = visionOperaAssembly.CreateInstance(item.FullName);
                        break;
                    }
                }

                //若视觉算子DLL有效,则创建默认配置
                if (obj is IVisionOpera)
                {
                    //创建默认配置
                    var visionOpera = obj as IVisionOpera;
                    configParamList = visionOpera.Outputs;
                }
                else
                {
                    return;
                }
            }

            ShowParamConfigWindow(filePath, configParamList);

        }

        /// <summary>
        /// 完成场景配置事件
        /// </summary>
        protected void OneCompletedSceneConfigEvent()
        {
            if (Scene != null)
            {
                Scene.IsEnableVision = IsEnableVision;
                Scene.CalibFile = CalibFile;
                Scene.InputParamFile = InputParamFile;
                Scene.OutputParamFile = OutputParamFile;
                Scene.CameraSerialNumber = SelectedCamera.SerialNumber;
                Scene.CameraConfigFile = CameraConfigFile;

                CompletedSceneConfigEventHandler?.Invoke(this, new CompletedSceneConfigEventArgs(Scene));
            }

        }

        /// <summary>
        /// 加载视觉算子DLL
        /// </summary>
        public void LoadVisionOperaDll(string dllPath)
        {
            if (string.IsNullOrWhiteSpace(dllPath))
            {
                return;
            }

            VisionOperaDLLPath = $"{VisionOperaDllDirectory}/{Path.GetFileName(dllPath)}";

            //校验,若选择的DLL并非本地路径,则拷贝
            if (Path.GetFullPath(VisionOperaDllDirectory).TrimEnd('\\') != Path.GetDirectoryName(dllPath))
            {
                System.IO.File.Copy(dllPath, VisionOperaDLLPath, true);
            }

            //实例化视觉算子
            var visionOperaAssembly = Assembly.LoadFrom(dllPath);
            var types = visionOperaAssembly.GetTypes();
            object obj = new object();
            foreach (var item in types)
            {
                if (item.Name == "VisionOpera")
                {
                    obj = visionOperaAssembly.CreateInstance(item.FullName);
                    break;
                }
            }

            //若视觉算子DLL有效,则创建默认配置
            if (obj is IVisionOpera)
            {
                //创建默认配置
                var visionOpera = obj as IVisionOpera;

                var cameraParam = new ItemCollection();
                cameraParam.Add(new ItemBase("ExposureTime", 2000.0));
                cameraParam.Add(new ItemBase("Gain", 0.0));

                JsonSerialization.SerializeObjectToFile(visionOpera.Inputs, $"{VisionInputFileDirectory}/DefaultInputParam.json");
                JsonSerialization.SerializeObjectToFile(visionOpera.Outputs, $"{VisionOutputFileDirectory}/DefaultOutputParam.json");
                JsonSerialization.SerializeObjectToFile(visionOpera.CalibParam, $"{CalibFileDirectory}/DefaultCalibParam.json");
                JsonSerialization.SerializeObjectToFile(cameraParam, $"{CameraConfigFileDirectory}/DefaultCameraParam.json");

                UpdateFileList();
            }

        }

        /// <summary>
        /// 更新
        /// </summary>
        public void UpdateFileList()
        {
            if (!string.IsNullOrWhiteSpace(SceneName))
            {
                //刷新场景名到UI
                SceneName = SceneName;
                VisionOperaDllName = VisionOperaDllName;

                //刷新目录参数
                VisionOperaDllDirectory = $"VisionPlatform/Scene/{SceneName}";
                CalibFileDirectory = $"VisionPlatform/Scene/{SceneName}/Calibration";
                VisionInputFileDirectory = $"VisionPlatform/Scene/{SceneName}/InputParam";
                VisionOutputFileDirectory = $"VisionPlatform/Scene/{SceneName}/OutputParam";
                CameraConfigFileDirectory = $"VisionPlatform/Scene/{SceneName}/CameraConfig";

                //备份当前配置
                var calibFileBackup = CalibFile;
                var inputParamFileBackup = InputParamFile;
                var outputParamFileBackup = OutputParamFile;
                var cameraConfigFileBackup = CameraConfigFile;

                //获取标定文件列表
                var fileList = new DirectoryInfo(CalibFileDirectory)?.GetFiles("*.json", SearchOption.TopDirectoryOnly);
                if (CalibFileList != null)
                {
                    CalibFileList.Clear();
                    foreach (var item in fileList)
                    {
                        CalibFileList.Add(item.Name);
                    }
                    CalibFileList.Add("创建新的参数文件...");

                    if (CalibFileList.Count > 1)
                    {
                        if (string.IsNullOrWhiteSpace(calibFileBackup))
                        {
                            CalibFile = CalibFileList[0];
                        }
                        else
                        {
                            CalibFile = calibFileBackup;
                        }
                    }

                }

                //获取视觉输入参数文件列表
                fileList = new DirectoryInfo(VisionInputFileDirectory)?.GetFiles("*.json", SearchOption.TopDirectoryOnly);
                if (InputParamFileList != null)
                {
                    InputParamFileList.Clear();
                    foreach (var item in fileList)
                    {
                        InputParamFileList.Add(item.Name);
                    }
                    InputParamFileList.Add("创建新的参数文件...");

                    if (InputParamFileList.Count > 1)
                    {
                        if (string.IsNullOrWhiteSpace(inputParamFileBackup))
                        {
                            InputParamFile = InputParamFileList[0];
                        }
                        else
                        {
                            InputParamFile = inputParamFileBackup;
                        }
                    }

                }

                //获取视觉输出参数文件列表
                fileList = new DirectoryInfo(VisionOutputFileDirectory)?.GetFiles("*.json", SearchOption.TopDirectoryOnly);
                if (OutputParamFileList != null)
                {
                    OutputParamFileList.Clear();
                    foreach (var item in fileList)
                    {
                        OutputParamFileList.Add(item.Name);
                    }
                    OutputParamFileList.Add("创建新的参数文件...");

                    if (OutputParamFileList.Count > 1)
                    {
                        if (string.IsNullOrWhiteSpace(outputParamFileBackup))
                        {
                            OutputParamFile = OutputParamFileList[0];
                        }
                        else
                        {
                            OutputParamFile = outputParamFileBackup;
                        }
                    }
                }

                //获取相机配置文件列表
                fileList = new DirectoryInfo(CameraConfigFileDirectory)?.GetFiles("*.json", SearchOption.TopDirectoryOnly);
                if (CameraConfigFileList != null)
                {
                    CameraConfigFileList.Clear();
                    foreach (var item in fileList)
                    {
                        CameraConfigFileList.Add(item.Name);
                    }
                    CameraConfigFileList.Add("创建新的参数文件...");

                    if (CameraConfigFileList.Count > 1)
                    {
                        if (string.IsNullOrWhiteSpace(cameraConfigFileBackup))
                        {
                            CameraConfigFile = CameraConfigFileList[0];
                        }
                        else
                        {
                            CameraConfigFile = cameraConfigFileBackup;
                        }
                    }
                }

            }
            else
            {
                CalibFileList.Clear();
                InputParamFileList.Clear();
                OutputParamFileList.Clear();
                CameraConfigFileList.Clear();
            }
        }

        /// <summary>
        /// 确认
        /// </summary>
        public void Accept()
        {
            OneCompletedSceneConfigEvent();
        }

        /// <summary>
        /// 取消
        /// </summary>
        public void Cancel()
        {

        }

        /// <summary>
        /// 更新场景
        /// </summary>
        /// <param name="scene"></param>
        public void UpdateScene(IScene scene)
        {
            if (scene == null)
            {
                IsSceneValid = false;
            }
            else
            {
                //更新场景
                Scene = scene;

                //刷新文件列表
                UpdateFileList();

                //设置有效选项
                IsEnableVision = Scene?.IsEnableVision ?? true;
                CalibFile = Scene?.CalibFile;
                InputParamFile = Scene?.InputParamFile;
                OutputParamFile = Scene?.OutputParamFile;
                //CameraSerialNumber = Scene?.CameraSerialNumber;
                CameraConfigFile = Scene?.CameraConfigFile;

                if (string.IsNullOrWhiteSpace(Scene?.CameraSerialNumber))
                {
                    SelectedCamera = CameraList[CameraList.Count - 1];
                }
                else
                {
                    foreach (var item in CameraList)
                    {
                        if (item.SerialNumber == Scene?.CameraSerialNumber)
                        {
                            SelectedCamera = item;
                            break;
                        }
                    }
                }


                IsSceneValid = true;
            }
        }

        /// <summary>
        /// 标定文件选择项改变
        /// </summary>
        /// <param name="index"></param>
        public async void CalibFileListSelectionChanged(int index)
        {
            if ((index == CalibFileList.Count - 1) && (index != -1))
            {
                if (CalibFileList.Count > 1)
                {
                    CalibFile = CalibFileList[0];
                }
                else
                {
                    CalibFile = null;
                }

                string fileName = await MetroDialog.ShowInputDialog(this, "请输入文件名称", "此配置将会以*.json配置文件的形式保存在对应目录下");
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    ShowCalibrationWindow($"{CalibFileDirectory}/{fileName}.json");
                }
            }
        }

        /// <summary>
        /// 输入参数文件选择项改变
        /// </summary>
        /// <param name="index"></param>
        public async void InputParamFileListSelectionChanged(int index)
        {
            if ((index == InputParamFileList.Count - 1) && (index != -1))
            {
                if (InputParamFileList.Count > 1)
                {
                    InputParamFile = InputParamFileList[0];
                }
                else
                {
                    InputParamFile = null;
                }

                string fileName = await MetroDialog.ShowInputDialog(this, "请输入文件名称", "此配置将会以*.json配置文件的形式保存在对应目录下");
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    ShowInputParamConfigWindow(fileName);
                }
            }
        }

        /// <summary>
        /// 输出参数文件选择项改变
        /// </summary>
        /// <param name="index"></param>
        public async void OutputParamFileListSelectionChanged(int index)
        {
            if ((index == OutputParamFileList.Count - 1) && (index != -1))
            {
                if (OutputParamFileList.Count > 1)
                {
                    OutputParamFile = OutputParamFileList[0];
                }
                else
                {
                    OutputParamFile = null;
                }

                string fileName = await MetroDialog.ShowInputDialog(this, "请输入文件名称", "此配置将会以*.json配置文件的形式保存在对应目录下");
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    ShowOutputParamConfigWindow(fileName);
                }
            }
        }

        /// <summary>
        /// 相机选择项改变
        /// </summary>
        /// <param name="index"></param>
        public void CameraListSelectionChanged(int index)
        {
            //if ((index == CameraList.Count - 1) && (index != -1))
            //{
            //    if (CameraList.Count > 1)
            //    {
            //        CameraSerialNumber = CameraList[0];
            //    }
            //    else
            //    {
            //        CameraSerialNumber = null;
            //    }

            //    string fileName = await MetroDialog.ShowInputDialog(this, "请输入文件名称", "此配置将会以*.json配置文件的形式保存在对应目录下");
            //    if (!string.IsNullOrWhiteSpace(fileName))
            //    {

            //    }
            //}
        }

        /// <summary>
        /// 相机配置文件选择项改变
        /// </summary>
        /// <param name="index"></param>
        public async void CameraConfigFileListSelectionChanged(int index)
        {
            if ((index == CameraConfigFileList.Count - 1) && (index != -1))
            {
                if (CameraConfigFileList.Count > 1)
                {
                    CameraConfigFile = CameraConfigFileList[0];
                }
                else
                {
                    CameraConfigFile = null;
                }

                CameraConfigFile = null;

                string fileName = await MetroDialog.ShowInputDialog(this, "请输入文件名称", "此配置将会以*.json配置文件的形式保存在对应目录下");
                if (!string.IsNullOrWhiteSpace(fileName))
                {

                }
            }
        }

        /// <summary>
        /// 修改标定文件
        /// </summary>
        /// <param name="index"></param>
        public void ModifyCalibFile(int index)
        {
            if ((index >= 0) && (index < CalibFileList.Count))
            {
                ShowCalibrationWindow(CalibFileList[index]);
            }

        }

        /// <summary>
        /// 修改输入参数文件
        /// </summary>
        /// <param name="index"></param>
        public void ModifyInputParamFile(int index)
        {
            if ((index >= 0) && (index < InputParamFileList.Count))
            {
                ShowInputParamConfigWindow(Path.GetFileNameWithoutExtension(InputParamFileList[index]));
            }

        }

        /// <summary>
        /// 修改输出参数文件
        /// </summary>
        /// <param name="index"></param>
        public void ModifyOutputParamFile(int index)
        {
            if ((index >= 0) && (index < OutputParamFileList.Count))
            {
                ShowOutputParamConfigWindow(Path.GetFileNameWithoutExtension(OutputParamFileList[index]));
            }

        }

        /// <summary>
        /// 修改相机配置参数文件
        /// </summary>
        /// <param name="index"></param>
        public void ModifyCameraConfigFile(int index)
        {

        }

        /// <summary>
        /// 设置相机列表
        /// </summary>
        /// <param name="cameraList">相机列表</param>
        public void SetCameraList(Dictionary<string, DeviceInfo> deviceInfos)
        {
            CameraList.Clear();

            foreach (var item in deviceInfos.Values)
            {
                CameraList.Add(item);
            }
            CameraList.Add(new DeviceInfo() { IPAddress = "不选择相机"});

        }

        #endregion
    }
}
