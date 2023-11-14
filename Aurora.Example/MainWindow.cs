using Aurora.UI;
using Aurora.UI.Common;
 

namespace Aurora.Example
{
    internal class MainWindow : PlayWindow
    {
        public override void OnAfterInitialize()
        {
            this.Window.Title = "Aurora UI";
            _ = this.LoadScene<InitScene>();
        }
    }
}
