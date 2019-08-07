using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using VisionPlatform.BaseType;

namespace VisionPlatform.Core
{
    /// <summary>
    /// 视觉框架工厂
    /// </summary>
    /// <remarks>
    /// 主要是为了创建不同的视觉框架的实例
    /// </remarks>
    public static class VisionFrameFactory
    {
        /// <summary>
        /// 视觉框架DLL根目录
        /// </summary>
        public static string VisionFrameDllRootPath { get; } = "VisionPlatform/VisionFrame";

        ///// <summary>
        ///// Dll名
        ///// </summary>
        //public static List<string> DllNames { get; private set; } = new List<string>();

        /// <summary>
        /// 视觉平台集合字典
        /// </summary>
        public static Dictionary<string, Assembly> VisionFrameAssemblys { get; private set; } = new Dictionary<string, Assembly>();

        /// <summary>
        /// 静态构造
        /// </summary>
        static VisionFrameFactory()
        {
            UpdateAssembly();
        }

        /// <summary>
        /// 更新集合
        /// </summary>
        public static void UpdateAssembly()
        {
            //DllNames.Clear();
            VisionFrameAssemblys.Clear();

            //遍历目录
            if (Directory.Exists(VisionFrameDllRootPath))
            {
                var directoryInfo = new DirectoryInfo(VisionFrameDllRootPath);

                foreach (var item in directoryInfo.GetDirectories())
                {
                    //获取集合
                    var dllPath = $"{VisionFrameDllRootPath}/{item.Name}/VisionPlatform.{item.Name}.dll";

                    if (File.Exists(dllPath))
                    {
                        var assembly = Assembly.LoadFrom(dllPath);

                        //将dll添加到集合字典中
                        VisionFrameAssemblys.Add(item.Name, assembly);
                    }

                }
            }

        }

        /// <summary>
        /// 创建视觉框架实例
        /// </summary>
        /// <param name="name">视觉框架名</param>
        /// <returns>视觉框架实例</returns>
        public static IVisionFrame CreateInstance(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("name cannot be null");
            }

            try
            {
                if (VisionFrameAssemblys.ContainsKey(name))
                {
                    //创建视觉框架实例
                    foreach (var item in VisionFrameAssemblys[name].ExportedTypes)
                    {
                        if (item.Name == "VisionFrame")
                        {
                            object obj = VisionFrameAssemblys[name].CreateInstance(item.FullName);

                            if (obj is IVisionFrame)
                            {
                                return obj as IVisionFrame;
                            }
                        }
                    }
                }

                throw new FileNotFoundException($"{nameof(name)} is not found");
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
