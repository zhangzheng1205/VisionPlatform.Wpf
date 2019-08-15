using System;
using System.Collections.Generic;

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
                if (!Scenes.ContainsKey(sceneName))
                {
                    return Scenes.Remove(sceneName);
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

        #endregion

    }
}
