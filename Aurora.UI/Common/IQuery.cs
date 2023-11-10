using Aurora.UI.Controls;


namespace Aurora.UI.Common
{
    /// <summary>
    /// GUI控件查询接口
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        /// 获取当前控件下指定名称的对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Control，未找到时返回 null</returns>
        Control this[String name] { get; }

        /// <summary>
        /// 查询当前控件下指定路径的控件
        /// </summary>
        /// <typeparam name="T">控件类型</typeparam>
        /// <param name="path">控件路径 control/control/</param>
        /// <returns>Control，未找到时返回 null</returns>
        T Query<T>(String path) where T : Control;
    }
}
