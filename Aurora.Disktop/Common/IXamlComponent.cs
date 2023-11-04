using Aurora.Disktop.Controls;


namespace Aurora.Disktop.Common
{
    public interface IXamlComponent
    {
    }



    public interface IXamlControl : IXamlComponent
    {
    }



    public delegate void XamlClickEventHandler<T>(T sender) where T : Control;
}
