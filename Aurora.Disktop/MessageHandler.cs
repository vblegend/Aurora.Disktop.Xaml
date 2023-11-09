using Aurora.Disktop.Common;
using Aurora.Disktop.Controls;


namespace Aurora.Disktop
{
    internal class MessageHandler
    {
        private readonly Control Root;


        /// <summary>
        /// 焦点所在控件
        /// </summary>
        internal Control Activeed { get; private set; }

        /// <summary>
        /// 鼠标悬停的控件
        /// </summary>
        internal Control Hovering { get; private set; }

        /// <summary>
        /// 鼠标按下的控件
        /// </summary>
        internal Control Pressed { get; private set; }


        internal List<Control> FocusPath { get; private set; }

        public MessageHandler(Control control)
        {
            this.Root = control;
            this.FocusPath = new List<Control>();
        }



        internal Control ProcessMessage(Control control, IInputMessage msg)
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


        internal Control HandleKeyboardMessage(Control control, IKeyboardMessage msg)
        {
            if (this.FocusPath.Count > 0 && this.FocusPath[0] == control) return null;
            var focus = this.FocusPath[0];
            (focus as IXamlEventHandler)?.MessageHandler(msg);
            return focus;
        }


        internal Control HandleMouseMessage(Control control, IMouseMessage msg)
        {
            if (control == null) return null;
            if (control.IgnoreMouseEvents) return null;
            var mouseInControl = control.HitTest(msg.Location);
            if (!mouseInControl)
            {
                if (!control.ExtendBounds.Contains(msg.Location)) return null;
            }
            var controlHandler = control as IXamlEventHandler;
            // 处理被标记为吃掉消息的控件
            if (this.Pressed != null)
            {
                if (msg.Message == WM_MESSAGE.MOUSE_UP)
                {
                }

                (this.Pressed as IXamlEventHandler)?.MessageHandler(msg);
                if (msg.Message == WM_MESSAGE.MOUSE_UP)
                {
                    var r = this.Pressed;
                    this.Pressed = null;
                    return r;
                }
                //return this.Pressed;
            }
            // Panel 控件 处理子控件消息
            if (control is IPanelControl panel)
            {
                for (int i = panel.Count - 1; i >= 0; i--)
                {
                    var item = panel[i];
                    if (item != null)
                    {
                        var result = this.ProcessMessage(item, msg);
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
                var result = this.ProcessMessage(contentControl, msg);
                if (result != null) return result;
            }
            // 鼠标未在控件本体区域内
            if (!mouseInControl) return null;
            // 处理当前控件消息
            if (msg.Message == WM_MESSAGE.MOUSE_DOWN)
            {
                this.focusControl(control);
                (control as IXamlEventHandler)?.MessageHandler(msg);
                this.Pressed = control;
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
                        (Hovering as IXamlEventHandler)?.MessageHandler(new IntenelMouseMessage(WM_MESSAGE.MOUSE_LEAVE));
                    }
                    this.Hovering = control;
                    if (this.Hovering != null)
                    {
                        (Hovering as IXamlEventHandler)?.MessageHandler(new IntenelMouseMessage(WM_MESSAGE.MOUSE_ENTER));
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
        private void focusControl(Control control)
        {
            if (this.FocusPath.Count > 0 && this.FocusPath[0] == control)
            {
                return;
            }
            var currentPath = this.GetControlPath(control);
            foreach (var lostFocus in this.FocusPath.Except(currentPath))
            {
                //Trace.WriteLine($"Focus Lost ({lostFocus.Name})");
                (lostFocus as IXamlEventHandler)?.MessageHandler(new IntenelMouseMessage(WM_MESSAGE.LOSTFOCUS));
            }
            foreach (var gotFocus in currentPath.Except(this.FocusPath).Reverse())
            {
                //Trace.WriteLine($"Focus Got ({gotFocus.Name})");
                (gotFocus as IXamlEventHandler)?.MessageHandler(new IntenelMouseMessage(WM_MESSAGE.GOTFOCUS));
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
