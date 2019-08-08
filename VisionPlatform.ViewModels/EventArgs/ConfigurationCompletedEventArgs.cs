using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionPlatform.BaseType;

namespace VisionPlatform.ViewModels
{
    /// <summary>
    /// 完成参数配置事件参数
    /// </summary>
    public class ConfigurationCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// 配置参数列表
        /// </summary>
        public ItemCollection Configurations { get; }

        /// <summary>
        /// 创建ConfigurationCompletedEventArgs新实例
        /// </summary>
        /// <param name="configParamList"></param>
        public ConfigurationCompletedEventArgs(ItemCollection configurations)
        {
            Configurations = configurations;
        }

    }
}
