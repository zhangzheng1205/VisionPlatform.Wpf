using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionPlatform.BaseType
{
    /// <summary>
    /// 基本Item类型集合
    /// </summary>
    public class ItemCollection : List<ItemBase>
    {
        #region 构造函数

        /// <summary>
        /// 创建ItemCollection新实例
        /// </summary>
        public ItemCollection() : base()
        {

        }

        /// <summary>
        /// 创建ItemCollection新实例
        /// </summary>
        /// <param name="capacity">容量</param>
        public ItemCollection(int capacity) : base(capacity)
        {

        }

        /// <summary>
        /// 创建ItemCollection新实例
        /// </summary>
        /// <param name="collection">集合</param>
        public ItemCollection(IEnumerable<ItemBase> collection) : base(collection)
        {

        }

        #endregion

        /// <summary>
        /// Item索引
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>Item</returns>
        public ItemBase this[string key]
        {
            get
            {
                foreach (var item in this)
                {
                    if (item.Name == key)
                    {
                        return item;
                    }
                }

                return default(ItemBase);
            }
        }
    }
}
