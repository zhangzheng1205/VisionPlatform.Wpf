using System;
using System.Collections.Generic;
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

        /// <summary>
        /// 视觉平台集合字典
        /// </summary>
        public static Dictionary<EVisionFrameType, Assembly> VisionFrameAssemblys { get; private set; } = new Dictionary<EVisionFrameType, Assembly>();

        /// <summary>
        /// 默认场景框架
        /// </summary>
        public static EVisionFrameType DefaultVisionFrameType { get; set; } = EVisionFrameType.Unknown;

        /// <summary>
        /// 静态构造
        /// </summary>
        static VisionFrameFactory()
        {
            UpdateAssembly();
        }

        /// <summary>
        /// 目录名转换为ECameraSdkType
        /// </summary>
        /// <param name="directoryName">目录名</param>
        /// <returns>ECameraSdkType</returns>
        private static EVisionFrameType ConvertToEVisionFrameType(string directoryName)
        {
            switch (directoryName)
            {
                case "HalconVisionFrame": return EVisionFrameType.Halcon;
                case "VisionProVisionFrame": return EVisionFrameType.VisionPro;
                case "NiVisionVisionFrame": return EVisionFrameType.NIVision;
                default: return EVisionFrameType.Unknown;
            }
        }

        /// <summary>
        /// 更新集合
        /// </summary>
        public static void UpdateAssembly()
        {
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
                        VisionFrameAssemblys.Add(ConvertToEVisionFrameType(item.Name), assembly);
                    }

                }
            }

        }

        /// <summary>
        /// 创建视觉框架实例
        /// </summary>
        /// <param name="visionFrameType">视觉框架类型</param>
        /// <returns>视觉框架实例</returns>
        public static IVisionFrame CreateInstance(EVisionFrameType visionFrameType)
        {
            try
            {
                if (VisionFrameAssemblys.ContainsKey(visionFrameType))
                {
                    //创建视觉框架实例
                    foreach (var item in VisionFrameAssemblys[visionFrameType].ExportedTypes)
                    {
                        if (item.Name == "VisionFrame")
                        {
                            object obj = VisionFrameAssemblys[visionFrameType].CreateInstance(item.FullName);

                            if (obj is IVisionFrame)
                            {
                                return obj as IVisionFrame;
                            }
                        }
                    }
                }

                throw new FileNotFoundException($"{nameof(visionFrameType)} is not found");
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取视觉框架实例(按照默认的视觉框架类型进行实例化)
        /// </summary>
        /// <returns>视觉框架实例</returns>
        public static IVisionFrame CreateInstance()
        {
            if (DefaultVisionFrameType != EVisionFrameType.Unknown)
            {
                return CreateInstance(DefaultVisionFrameType);
            }

            return default(IVisionFrame);
        }

    }

}
