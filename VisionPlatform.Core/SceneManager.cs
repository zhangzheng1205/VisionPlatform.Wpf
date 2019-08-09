using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionPlatform.Core
{
    /// <summary>
    /// 场景管理器类
    /// </summary>
    public class SceneManager
    {
        /// <summary>
        /// 场景列表
        /// </summary>
        public Dictionary<string, Scene> Scenes { get; }

        /// <summary>
        /// 注册场景
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="scene">场景</param>
        public void RegisterScene(Scene scene)
        {
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
                //Logging.Error($"{ToString()} 注册场景{sceneName}失败", ex);
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
                //Logging.Error($"{ToString()} 删除场景{sceneName}失败", ex);
                throw;
            }

            return false;
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
                //Logging.Error($"{ToString()} 复位场景列表异常", ex);
                throw;
            }
        }


        public void 恢复场景列表(string configFile)
        {

        }

    }
}
