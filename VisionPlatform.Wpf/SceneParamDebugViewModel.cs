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
            this.scene = scene;
            
        }

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
                    return null;
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
                    return null;
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
                return Scene?.VisionFrame?.ConfigWindow;
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
                runStatus.Add(Scene?.VisionFrame?.RunStatus);
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

        #region 方法

        /// <summary>
        /// 执行场景
        /// </summary>
        public void Execute()
        {
            try
            {
                string visionResult = "";
                Scene?.Execute(2000, out visionResult);
                VisionResult = visionResult;
            }
            catch (Exception)
            {
                
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
            catch (Exception)
            {

            }
            
        }

        /// <summary>
        /// 保存场景
        /// </summary>
        public void SaveScene()
        {
            Scene.Serialize(Scene, "1.json");
        }

        #endregion
    }
}
