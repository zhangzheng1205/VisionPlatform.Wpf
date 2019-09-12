using System;
using System.Collections.Generic;
using System.IO;

namespace VisionPlatform.Core
{
    /// <summary>
    /// 场景管理器类
    /// </summary>
    public class SceneManager
    {
        #region 单例模式

        /// <summary>
        /// 私有实例接口
        /// </summary>
        private static readonly SceneManager Instance = new SceneManager();

        /// <summary>
        /// 创建私有SceneManager新实例,保证外界无法通过new来创建新实例
        /// </summary>
        private SceneManager()
        {

        }

        /// <summary>
        /// 获取实例接口
        /// </summary>
        /// <returns></returns>
        public static SceneManager GetInstance()
        {

            return Instance;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 场景列表
        /// </summary>
        public Dictionary<string, Scene> Scenes { get; } = new Dictionary<string, Scene>();

        #endregion

        #region 索引器

        /// <summary>
        /// 场景索引器
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <returns>场景</returns>
        public Scene this[string sceneName]
        {
            get
            {
                if (Scenes.ContainsKey(sceneName))
                {
                    return Scenes[sceneName];
                }

                return null;
            }
        }

        #endregion

        #region 场景基础操作

        /// <summary>
        /// 注册场景
        /// </summary>
        /// <param name="scene">场景</param>
        public void RegisterScene(Scene scene)
        {
            if (scene == null)
            {
                throw new ArgumentNullException("scene cannot be null");
            }

            try
            {
                if (!Scenes.ContainsKey(scene.Name))
                {
                    //注册场景
                    Scenes.Add(scene.Name, scene);
                }

                //保存场景到本地
                string file = $"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}/VisionPlatform/Scene/{scene.EVisionFrameType}/{scene.Name}/Scene.json";
                Scene.Serialize(scene, file);
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// 删除场景
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <returns>执行结果</returns>
        public bool DeleteScene(string sceneName)
        {
            try
            {
                //若场景存在,则删除场景以及对应目录
                if (Scenes.ContainsKey(sceneName))
                {
                    //释放场景
                    Scenes[sceneName].Dispose();

                    string file = $"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}/VisionPlatform/Scene/{Scenes[sceneName].EVisionFrameType}/{Scenes[sceneName].Name}/Scene.json";
                    Scenes.Remove(sceneName);
                    File.Delete(file);

                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;
        }

        /// <summary>
        /// 判断是否包含对应的场景
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <returns>结果</returns>
        public bool Contains(string sceneName)
        {
            return Scenes.ContainsKey(sceneName);
        }

        /// <summary>
        /// 复位场景列表
        /// </summary>
        /// <returns>执行结果</returns>
        public void ResetScenes()
        {
            try
            {
                if ((Scenes != null) && (Scenes?.Count > 0))
                {
                    foreach (var item in Scenes.Values)
                    {
                        item.Dispose();
                    }
                }

                Scenes.Clear();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 保存场景到本地
        /// </summary>
        public void SaveScenes()
        {
            foreach (var item in Scenes.Values)
            {
                string file = $"VisionPaltform/Scene/{item.EVisionFrameType}/{item.Name}/{item.Name}.json";
            }
        }

        /// <summary>
        /// 恢复场景列表
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:不捕获常规异常类型", Justification = "<挂起>")]
        public void RecoverScenes()
        {
            Scenes.Clear();
            if (VisionFrameFactory.DefaultVisionFrameType != BaseType.EVisionFrameType.Unknown)
            {
                var baseSceneDirectory = $"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}/VisionPlatform/Scene/{VisionFrameFactory.DefaultVisionFrameType}";

                if (Directory.Exists(baseSceneDirectory))
                {
                    //场景目录
                    DirectoryInfo[] sceneDirectories = new DirectoryInfo(baseSceneDirectory)?.GetDirectories();

                    foreach (var item in sceneDirectories)
                    {
                        string file = $"{item.FullName}\\Scene.json";

                        if (File.Exists(file))
                        {
                            try
                            {
                                var scene = Scene.Deserialize(file);
                                if (scene != null)
                                {
                                    Scenes.Add(scene.Name, scene);
                                }
                            }
                            catch (ArgumentException)
                            {
                            }
                        }
                        else
                        {
                            try
                            {
                                //当前场景已被删除,移除该场景
                                item.Delete(true);
                            }
                            catch (Exception)
                            {

                            }
                            
                        }

                    }
                }
            }

        }

        #endregion

    }
}
