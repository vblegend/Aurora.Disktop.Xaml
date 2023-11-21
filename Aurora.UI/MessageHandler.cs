using Aurora.UI.Common;
using Aurora.UI.Controls;
using Microsoft.Xna.Framework;


namespace Aurora.UI
{
    public class MessageHandler : IMessageHandler
    {
        private readonly Control Root;

        /// <summary>
        /// 焦点所在控件
        /// </summary>
        public Control Activeed { get; private set; }

        /// <summary>
        /// 鼠标悬停的控件
        /// </summary>
        public Control Hovering { get; private set; }

        /// <summary>
        /// 鼠标捕获的控件
        /// </summary>
        private Control Captured;

        /// <summary>
        /// 捕获中断，仅处理被捕获的控件消息
        /// </summary>
        private Boolean CaptureInterrupt;





        private List<Control> FocusPath;

        internal MessageHandler(Control control)
        {
            this.Root = control;
            this.FocusPath = new List<Control>();
        }

        void IMessageHandler.OnMessage(IInputMessage msg)
        {
            this.DistributeMessage(this.Root, msg);
        }


        /// <summary>
        /// 强制捕获鼠标消息给某个控件。
        /// </summary>
        /// <param name="control"></param>
        public void CaptureMouse(Control control)
        {
            this.Captured = control;
            this.CaptureInterrupt = true;
        }

        /// <summary>
        /// 释放被捕获的鼠标消息
        /// </summary>
        public void ReleaseMouse()
        {
            this.CaptureInterrupt = false;
            if (this.Captured != null)
            {
                this.Captured = null;
            }
        }




        private Control DistributeMessage(Control control, IInputMessage msg)
        {
            if (msg is IMouseMessage mouseMessage)
            {
               return this.HandleMouseMessage(control, mouseMessage);
            }
            else if (msg is IKeyboardMessage keyboardMessage)
            {
                return this.HandleKeyboardMessage(control, keyboardMessage);
            }
            return null;
        }


        private Control HandleKeyboardMessage(Control control, IKeyboardMessage msg)
        {
            var focus = this.Activeed;
            (focus as IXamlEventHandler)?.MessageHandler(msg);
            return focus;
        }


        private Control HandleMouseMessage(Control control, IMouseMessage msg)
        {
            if (control == null) return null;
            if (control.IgnoreMouseEvents) return null;
            var mouseInControl = control.HitTest(msg.Location);
            if (!mouseInControl && this.Captured == null)
            {
                if (!control.ExtendBounds.Contains(msg.Location)) return null;
            }
            var controlHandler = control as IXamlEventHandler;
            // 处理被标记为吃掉消息的控件
            if (this.Captured != null)
            {
                if (msg.Message == WM_MESSAGE.MOUSE_UP)
                {
                }

                (this.Captured as IXamlEventHandler)?.MessageHandler(msg);
                if (msg.Message == WM_MESSAGE.MOUSE_UP)
                {
                    var r = this.Captured;
                    this.Captured = null;
                    return r;
                }

                if (this.CaptureInterrupt)
                {
                    return this.Captured;
                }
            }
            // Panel 控件 处理子控件消息
            if (control is IPanelControl panel)
            {
                for (int i = panel.Count - 1; i >= 0; i--)
                {
                    var item = panel[i];
                    if (item != null)
                    {
                        var result = this.DistributeMessage(item, msg);
                        if (result != null && item is Dialog dialog && msg.Message == WM_MESSAGE.MOUSE_DOWN && dialog.AutoTop)
                        {
                            dialog.MoveTop();
                        }
                        if (result != null) return result;
                    }
                }
            }
            else if (control is ContentControl item && item.Content is Control contentControl)
            {
                var result = this.DistributeMessage(contentControl, msg);
                if (result != null) return result;
            }
            // 鼠标未在控件本体区域内
            if (!mouseInControl) return null;
            // 处理当前控件消息
            if (msg.Message == WM_MESSAGE.MOUSE_DOWN)
            {
                this.Captured = control;
                this.CaptureInterrupt = false;
                this.focusControl(control, msg);
                (control as IXamlEventHandler)?.MessageHandler(msg);

            }
            else if (msg.Message == WM_MESSAGE.MOUSE_WHEEL)
            {
                (control as IXamlEventHandler)?.MessageHandler(msg);
            }
            else if (msg.Message == WM_MESSAGE.MOUSE_MOVE)
            {
                if (this.Hovering != control)
                {
                    if (this.Hovering != null)
                    {
                        (Hovering as IXamlEventHandler)?.MessageHandler(new IntenelEventMessage(WM_MESSAGE.MOUSE_LEAVE, msg));
                    }
                    this.Hovering = control;
                    if (this.Hovering != null)
                    {
                        (Hovering as IXamlEventHandler)?.MessageHandler(new IntenelEventMessage(WM_MESSAGE.MOUSE_ENTER, msg));
                    }
                }
                controlHandler?.MessageHandler(msg);
            }
            return control;
        }


        private List<Control> GetControlPath(Control control)
        {
            var lst = new List<Control>();
            while (control != null)
            {
                lst.Add(control);
                control = control.Parent;
            }
            return lst;
        }



        /// <summary>
        /// 聚焦控件，转移焦点
        /// </summary>
        /// <param name="control"></param>
        private void focusControl(Control control, IMouseMessage msg)
        {
            if (this.FocusPath.Count > 0 && this.FocusPath[0] == control)
            {
                return;
            }
            var currentPath = this.GetControlPath(control);
            foreach (var lostFocus in this.FocusPath.Except(currentPath))
            {
                (lostFocus as IXamlEventHandler)?.MessageHandler(new IntenelEventMessage(WM_MESSAGE.LOSTFOCUS, msg));
            }
            foreach (var gotFocus in currentPath.Except(this.FocusPath).Reverse())
            {
                (gotFocus as IXamlEventHandler)?.MessageHandler(new IntenelEventMessage(WM_MESSAGE.GOTFOCUS, msg));
            }
            this.FocusPath = currentPath;


            if (this.FocusPath.Count > 0)
            {
                this.Activeed = this.FocusPath[0];
            }
            else
            {
                this.Activeed = null;
            }

        }

    }
}
