using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VisionPlatform.BaseType;

namespace VisionPlatform.ViewModels
{
    public class ParamConfigViewModel : Screen
    {
        #region 构造函数

        /// <summary>
        /// 创建ParamConfigViewModel新实例
        /// </summary>
        public ParamConfigViewModel() : this(new ItemCollection())
        {

        }

        /// <summary>
        /// 创建ParamConfigViewModel新实例
        /// </summary>
        /// <param name="configParamList">配置参数列表</param>
        public ParamConfigViewModel(ItemCollection configParamList)
        {
            ParamList = new ObservableCollection<ItemBase>();
            ConfigParamList = configParamList;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 参数列表
        /// </summary>
        public ItemCollection ConfigParamList
        {
            get
            {
                return new ItemCollection(ParamList);
            }
            set
            {
                ParamList = new ObservableCollection<ItemBase>(value);
            }
        }

        /// <summary>
        /// 参数列表
        /// </summary>
        private ObservableCollection<ItemBase> paramList;

        /// <summary>
        /// 参数列表
        /// </summary>
        public ObservableCollection<ItemBase> ParamList
        {
            get
            {
                return paramList;
            }
            set
            {
                paramList = value;
                NotifyOfPropertyChange(() => ParamList);
            }
        }

        #endregion

        #region 事件

        /// <summary>
        /// 配置完成事件
        /// </summary>
        public event EventHandler<ConfigurationCompletedEventArgs> ConfigurationCompleted;

        /// <summary>
        /// 配置取消事件
        /// </summary>
        public event EventHandler<EventArgs> ConfigurationCanceled;

        #endregion

        #region 方法

        /// <summary>
        /// 配置完成事件
        /// </summary>
        /// <param name="configurations">配置参数</param>
        protected void OnConfigurationCompleted(ItemCollection configurations)
        {
            ConfigurationCompleted?.Invoke(this, new ConfigurationCompletedEventArgs(configurations));
        }

        /// <summary>
        /// 确认
        /// </summary>
        public void Accept()
        {
            OnConfigurationCompleted(ConfigParamList);
        }

        /// <summary>
        /// 配置取消事件
        /// </summary>
        protected void OnConfigurationCanceled()
        {
            ConfigurationCanceled?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// 取消
        /// </summary>
        public void Cancel()
        {
            OnConfigurationCanceled();
        }

        #endregion
    }
}
