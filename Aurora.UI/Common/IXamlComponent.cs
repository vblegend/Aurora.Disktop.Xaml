using Aurora.UI.Controls;


namespace Aurora.UI.Common
{

    /// <summary>
    /// Xaml Component
    /// </summary>
    public interface IXamlComponent
    {
    }



    /// <summary>
    /// Xaml UI Control
    /// </summary>
    public interface IXamlControl : IXamlComponent
    {
    }



    public delegate void XamlClickEventHandler<T>(T sender) where T : Control;
    public delegate void XamlCheckedEventHandler<T>(T sender,Boolean value) where T : Control;
}
