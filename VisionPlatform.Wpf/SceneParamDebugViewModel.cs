﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionPlatform.BaseType;
using VisionPlatform.Core;

namespace VisionPlatform.Wpf
{
    /// <summary>
    /// SceneParamDebugViewModel
    /// </summary>
    public class SceneParamDebugViewModel : Screen
    {
        #region 构造函数

        /// <summary>
        /// 创建SceneParamDebugViewModel新实例
        /// </summary>
        public SceneParamDebugViewModel()
        {

        }

        /// <summary>
        /// 创建SceneParamDebugViewModel新实例
        /// </summary>
        /// <param name="scene">场景实例</param>
        public SceneParamDebugViewModel(Scene scene)
        {
            Scene = scene;

        }

        #endregion

        #region 属性

        private Scene scene;

        /// <summary>
        /// 场景
        /// </summary>
        public Scene Scene
        {
            get
            {
                return scene;
            }
            set
            {
                scene = value;

                NotifyOfPropertyChange(() => Scene);
                NotifyOfPropertyChange(() => SceneName);
                NotifyOfPropertyChange(() => IsEnableCamera);
                NotifyOfPropertyChange(() => CameraName);
                NotifyOfPropertyChange(() => IsEnableInput);
                NotifyOfPropertyChange(() => Inputs);
                NotifyOfPropertyChange(() => IsEnableOutput);
                NotifyOfPropertyChange(() => Outputs);
                NotifyOfPropertyChange(() => ConfigWindow);
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
        }

        /// <summary>
        /// 使能相机
        /// </summary>
        public bool IsEnableCamera
        {
            get
            {
                return Scene?.VisionFrame?.IsEnableCamera ?? false;
            }
        }

        /// <summary>
        /// 相机名
        /// </summary>
        public string CameraName
        {
            get
            {
                return Scene?.Camera?.ToString() ?? "无效相机";
            }
        }

        /// <summary>
        /// 使能输入
        /// </summary>
        public bool IsEnableInput
        {
            get
            {
                return Scene?.VisionFrame?.IsEnableInput ?? false;
            }
        }

        /// <summary>
        /// 输入参数
        /// </summary>
        public ObservableCollection<ItemBase> Inputs
        {
            get
            {
                if (Scene?.VisionFrame?.Inputs == null)
                {
                    return new ObservableCollection<ItemBase>();
                }

                return new ObservableCollection<ItemBase>(Scene?.VisionFrame?.Inputs);
            }
        }

        /// <summary>
        /// 使能输出
        /// </summary>
        public bool IsEnableOutput
        {
            get
            {
                return Scene?.VisionFrame?.IsEnableOutput ?? false;
            }
        }

        /// <summary>
        /// 输出参数
        /// </summary>
        public ObservableCollection<ItemBase> Outputs
        {
            get
            {
                if (Scene?.VisionFrame?.Outputs == null)
                {
                    return new ObservableCollection<ItemBase>();
                }

                return new ObservableCollection<ItemBase>(Scene?.VisionFrame?.Outputs);
            }
        }

        /// <summary>
        /// 配置窗口
        /// </summary>
        public object ConfigWindow
        {
            get
            {
                return Scene?.VisionFrame?.ConfigWindow ?? new System.Windows.Controls.Grid();
            }
        }

        private ObservableCollection<RunStatus> runStatus = new ObservableCollection<RunStatus>();

        /// <summary>
        /// 运行状态
        /// </summary>
        public ObservableCollection<RunStatus> RunStatus
        {
            get
            {
                runStatus.Clear();
                if (Scene?.VisionFrame?.RunStatus != null)
                {
                    runStatus.Add(Scene?.VisionFrame?.RunStatus);
                }
                else
                {
                    runStatus.Add(new BaseType.RunStatus());
                }
                
                return runStatus;
            }
        }

        private string visionResult;

        /// <summary>
        /// 视觉结果
        /// </summary>
        public string VisionResult
        {
            get
            {
                return visionResult;
            }
            set
            {
                visionResult = value;
                NotifyOfPropertyChange(() => VisionResult);
                NotifyOfPropertyChange(() => RunStatus);
                NotifyOfPropertyChange(() => Outputs);
            }
        }

        #endregion

        #region 事件

        /// <summary>
        /// 场景配置完成事件
        /// </summary>
        public event EventHandler<SceneConfigurationCompletedEventArgs> SceneConfigurationCompleted;

        /// <summary>
        /// 消息触发事件
        /// </summary>
        internal event EventHandler<MessageRaisedEventArgs> MessageRaised;

        #endregion

        #region 方法

        /// <summary>
        /// 触发场景配置完成事件
        /// </summary>
        protected void OnSceneConfigurationCompleted(Scene scene)
        {
            SceneConfigurationCompleted?.Invoke(this, new SceneConfigurationCompletedEventArgs(scene));
        }

        internal void OnMessageRaised(MessageLevel messageLevel, string message, Exception exception = null)
        {
            MessageRaised?.Invoke(this, new MessageRaisedEventArgs(messageLevel, message, exception));
        }

        /// <summary>
        /// 通过本地图片执行
        /// </summary>
        /// <param name="file"></param>
        public void ExecuteByFile(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                return;
            }

            try
            {
                string visionResult = "";
                RunStatus runStatus = Scene?.ExecuteByFile(file, out visionResult);
                if ((runStatus.Result == EResult.Accept) || (runStatus.Result == EResult.Warning))
                {
                    VisionResult = visionResult;
                }
                else if ((runStatus.Result == EResult.Error) || (runStatus.Result == EResult.Reject))
                {
                    OnMessageRaised(MessageLevel.Err, runStatus.Message, runStatus.Exception);
                }
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
        }

        /// <summary>
        /// 执行场景
        /// </summary>
        public void Execute()
        {
            try
            {
                string visionResult = "";
                RunStatus runStatus = Scene?.Execute(2000, out visionResult);
                if ((runStatus.Result == EResult.Accept) || (runStatus.Result == EResult.Warning))
                {
                    VisionResult = visionResult;
                }
                else if ((runStatus.Result == EResult.Error) || (runStatus.Result == EResult.Reject))
                {
                    OnMessageRaised(MessageLevel.Err, runStatus.Message, runStatus.Exception);
                }
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
        }

        /// <summary>
        /// 异步执行
        /// </summary>
        public async void ExecuteAsync()
        {
            try
            {
                VisionResult = await Scene?.ExecuteAsync(2000);
            }
            catch (Exception ex)
            {
                OnMessageRaised(MessageLevel.Err, ex.Message, ex);
            }
        }

        /// <summary>
        /// 确认退出
        /// </summary>
        public void Accept()
        {
            //保存输入输出参数到本地文件
            Scene.SaveParamToLocalFile();

            OnSceneConfigurationCompleted(Scene);
        }

        #endregion
    }
}
