using System;

namespace VisionPlatform.BaseType
{
    /// <summary>
    /// 基本Item类型
    /// </summary>
    public class ItemBase
    {
        /// <summary>
        /// 创建类型实例
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <returns>实例默认值</returns>
        private static object CallCreateInstance(Type type)
        {
            try
            {
                return Activator.CreateInstance(type);
            }
            catch (ArgumentException)
            {
                return null;
            }
            catch (MissingMethodException)
            {
                return null;
            }
        }

        #region 构造函数

        /// <summary>
        /// 创建ItemBase新实例
        /// </summary>
        public ItemBase()
        {
        }

        /// <summary>
        /// 创建ItemBase新实例
        /// </summary>
        /// <param name="name">Item名</param>
        public ItemBase(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("name cannot be null");
            }

            Name = name;
            Value = null;
        }

        /// <summary>
        /// 创建ItemBase新实例
        /// </summary>
        /// <param name="name">Item名</param>
        /// <param name="value">数据</param>
        public ItemBase(string name, object value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("name cannot be null");
            }
            if (value == null)
            {
                throw new ArgumentException("value cannot be null");
            }

            Name = name;
            Value = value;
            ValueType = Value.GetType();
        }

        /// <summary>
        /// 创建ItemBase新实例
        /// </summary>
        /// <param name="name">Item名</param>
        /// <param name="value">数据</param>
        /// <param name="description">描述</param>
        public ItemBase(string name, object value, string description) : this(name, value)
        {
            Description = description;
        }

        /// <summary>
        /// 创建ItemBase新实例
        /// </summary>
        /// <param name="name">Item名</param>
        /// <param name="valueType">数据类型</param>
        public ItemBase(string name, Type valueType) : this(name)
        {
            if (valueType == null)
            {
                throw new ArgumentNullException("type cannot be null");
            }
            ValueType = valueType;
        }

        /// <summary>
        /// 创建ItemBase新实例
        /// </summary>
        /// <param name="name">Item名</param>
        /// <param name="valueType">数据类型</param>
        /// <param name="description">描述</param>
        public ItemBase(string name, Type valueType, string description) : this(name, valueType)
        {
            Description = description;
        }

        /// <summary>
        /// 创建ItemBase新实例
        /// </summary>
        /// <param name="name">Item名</param>
        /// <param name="value">数据</param>
        /// <param name="valueType">数据类型</param>
        public ItemBase(string name, object value, Type valueType) : this(name, valueType)
        {
            Value = value;

            if (value != null && !valueType.IsAssignableFrom(value.GetType()))
            {
                throw new ArgumentException("valueType must be assignable from value");
            }
        }

        /// <summary>
        /// 创建ItemBase新实例
        /// </summary>
        /// <param name="name">Item名</param>
        /// <param name="value">数据</param>
        /// <param name="valueType">数据类型</param>
        /// <param name="description">描述</param>
        public ItemBase(string name, object value, Type valueType, string description) : this(name, value, valueType)
        {
            Description = description;
        }

        /// <summary>
        /// 创建ItemBase新实例
        /// </summary>
        /// <param name="name">Item名</param>
        /// <param name="valueType">数据类型</param>
        /// <param name="initialize">是否初始化标志</param>
        public ItemBase(string name, Type valueType, bool initialize) : this(name, initialize ? CallCreateInstance(valueType) : null, valueType)
        {
        }

        #endregion

        #region 属性

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        private object m_value;

        /// <summary>
        /// 数值
        /// </summary>
        public object Value
        {
            get
            {
                return m_value;
            }
            set
            {
                if (ValueType != null)
                {
                    try
                    {
                        m_value = Convert.ChangeType(value, ValueType);
                    }
                    catch (InvalidCastException)
                    {
                        m_value = CallCreateInstance(ValueType);
                    }
                    catch (OverflowException)
                    {
                        m_value = CallCreateInstance(ValueType);
                    }
                }
                else
                {
                    m_value = value;
                }

            }
        }

        /// <summary>
        /// 数值类型
        /// </summary>
        public Type ValueType { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// 有效标志
        /// </summary>
        public bool IsAvailable { get; set; } = true;

        #endregion

        /// <summary>
        /// 字符串转换
        /// </summary>
        /// <returns>数据类型字符串</returns>
        public override string ToString()
        {

            return Value?.ToString();
        }
    }
}
