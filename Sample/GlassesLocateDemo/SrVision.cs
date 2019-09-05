using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GlassesLocateDemo
{
    public class SrVision
    {
        const string DLLPath = "SrVisionDll.dll";
        [DllImport(DLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SRV_Init(string file);

        [DllImport(DLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SRV_Run(string file);

        [DllImport(DLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SRV_Dispose();
    }

}
