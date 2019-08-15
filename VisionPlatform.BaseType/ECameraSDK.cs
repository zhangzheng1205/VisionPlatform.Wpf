namespace VisionPlatform.BaseType
{
    /// <summary>
    /// 相机SDK
    /// </summary>
    public enum ECameraSdkType
    {
        /// <summary>
        /// 支持巴斯勒相机
        /// </summary>
        Pylon,

        /// <summary>
        /// 支持IDS USB相机
        /// </summary>
        uEye,

        /// <summary>
        /// 支持海康相机
        /// </summary>
        Hik,

        /// <summary>
        /// 支持巴斯勒,康耐视,大恒,DALSA,海康,JAI,映美精(都已实际验证过)等Gige相机;
        /// </summary>
        /// <remarks>
        /// 1. 此SDK理论上兼容所有支持GENICAM协议的协议,但是需要实现相机节点适配文件./CameraParam/品牌名.xml;
        /// 2. 如上,使用这个这个SDK,需要根目录下面存在CameraParam文件,里面存放各个品牌相机的节点命名适配文件;
        /// 3. 采图效率与原生SDK一致,但是搜索相机的速度会比原生SDK慢一点(1~2S);
        /// 4. 存在一个BUG:当搜索相机时,有概率会导致先前已连接的相机断开,使用单个相机时没问题,但是使用多个相机时,
        ///    慎用这个SDK,或者是注意建立重连机制;
        /// </remarks>
        Common,

        /// <summary>
        /// 支持康耐视相机(尚未实现)
        /// </summary>
        Cognex,

        /// <summary>
        /// 支持大恒相机(尚未实现)
        /// </summary>
        Daheng,

        /// <summary>
        /// 支持DALSA相机(尚未实现)
        /// </summary>
        DALSA,

        /// <summary>
        /// 支持映美精相机(尚未实现)
        /// </summary>
        ImagingSource,

        /// <summary>
        /// 虚拟相机
        /// </summary>
        /// <remarks>
        /// 用于获取本地图片,接口的使用与ICamera保持一致,来传递图片路径或者;
        /// 设置图片路径: 通过调用bool Connect(string ImagePath), 支持图片路径或者目录;
        /// 获取图片: 通过TriggerSoftware()触发事件NewImageEvent,在事件中获取;
        /// </remarks>
        VirtualCamera,

        /// <summary>
        /// 未知
        /// </summary>
        Unknown,
    }
}
