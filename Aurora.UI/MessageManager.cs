using Aurora.UI.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace Aurora.UI
{









    internal class MessageManager
    {
        internal IMessageHandler Handler;
        private Boolean CtrlPressed;
        private Boolean ShiftPressed;
        private IntenelKeyBoardMessage keyboardMessage = new IntenelKeyBoardMessage();
        private PlayWindow gameWindow;
        private IntenelMouseMessage mouseMessage = new IntenelMouseMessage();
        private GameTime gameTime;

        public MessageManager(PlayWindow window)
        {
            this.gameWindow = window;
            window.Window.KeyDown += Window_KeyDown;
            window.Window.KeyUp += Window_KeyUp;
            //window.Window.TextInput += Window_TextInput;
        }

        private void Window_TextInput(object sender, TextInputEventArgs e)
        {

        }

        private void Window_KeyUp(object sender, InputKeyEventArgs e)
        {
            //if (!this.gameWindow.IsActive) return;
            if (e.Key == Keys.LeftControl || e.Key == Keys.RightControl) this.CtrlPressed = false;
            if (e.Key == Keys.LeftShift || e.Key == Keys.RightShift) this.ShiftPressed = false;
            this.DispatchKeyBoardEvent(WM_MESSAGE.KEY_UP, e.Key);
        }

        private void Window_KeyDown(object sender, InputKeyEventArgs e)
        {
            //if (!this.gameWindow.IsActive) return;
            if (e.Key == Keys.LeftControl || e.Key == Keys.RightControl) this.CtrlPressed = true;
            if (e.Key == Keys.LeftShift || e.Key == Keys.RightShift) this.ShiftPressed = true;
            this.DispatchKeyBoardEvent(WM_MESSAGE.KEY_DOWN, e.Key);
        }


        internal void SetHandler(IMessageHandler handler)
        {
            this.Handler = handler;
            this.LeftButton = ButtonState.Released;
            this.MiddleButton = ButtonState.Released;
            this.RightButton = ButtonState.Released;
        }


        private void DispatchKeyBoardEvent(WM_MESSAGE msg, Keys key)
        {
            this.keyboardMessage.Message = msg;
            this.keyboardMessage.Ctrl = this.CtrlPressed;
            this.keyboardMessage.Shift = this.ShiftPressed;
            this.keyboardMessage.Key = key;
            Handler.OnMessage(this.keyboardMessage);
        }

        private void DispatchMouseEvent(WM_MESSAGE msg, Point position, MouseButtons? buttons = null, Int32? wheel = null)
        {
            this.mouseMessage.Message = msg;
            this.mouseMessage.Ctrl = this.CtrlPressed;
            this.mouseMessage.Shift = this.ShiftPressed;
            this.mouseMessage.Location = position;
            if (buttons.HasValue) this.mouseMessage.Button = buttons.Value;
            if (wheel.HasValue) this.mouseMessage.Wheel = wheel.Value;
            Handler.OnMessage(this.mouseMessage);
        }








        internal void Update(GameTime gameTime)
        {

            if (!this.gameWindow.IsActive) return;
            var state = Mouse.GetState(this.gameWindow.Window);
            if (!state.Position.Equals(this.MousePosition))
            {
                MousePosition = state.Position;
                this.DispatchMouseEvent(WM_MESSAGE.MOUSE_MOVE, state.Position);
            }
            if (state.ScrollWheelValue > ScrollWheelValue)
            {
                this.DispatchMouseEvent(WM_MESSAGE.MOUSE_WHEEL, state.Position, wheel: state.ScrollWheelValue - ScrollWheelValue);
            }
            else if (state.ScrollWheelValue < ScrollWheelValue)
            {
                this.DispatchMouseEvent(WM_MESSAGE.MOUSE_WHEEL, state.Position, wheel: state.ScrollWheelValue - ScrollWheelValue);
            }
            this.ScrollWheelValue = state.ScrollWheelValue;

            if (state.LeftButton != this.LeftButton)
            {
                if (state.LeftButton == ButtonState.Pressed)
                {
                    this.LeftButton = state.LeftButton;
                    this.DispatchMouseEvent(WM_MESSAGE.MOUSE_DOWN, state.Position, MouseButtons.Left);
                }
                else
                {
                    this.LeftButton = state.LeftButton;
                    this.DispatchMouseEvent(WM_MESSAGE.MOUSE_UP, state.Position, MouseButtons.Left);
                }
   
            }
            if (state.RightButton != this.RightButton)
            {
                if (state.RightButton == ButtonState.Pressed)
                {
                    this.RightButton = state.RightButton;
                    this.DispatchMouseEvent(WM_MESSAGE.MOUSE_DOWN, state.Position, MouseButtons.Right);
                }
                else
                {
                    this.RightButton = state.RightButton;
                    this.DispatchMouseEvent(WM_MESSAGE.MOUSE_UP, state.Position, MouseButtons.Right);
                }
 
            }
            if (state.MiddleButton != this.MiddleButton)
            {
                if (state.MiddleButton == ButtonState.Pressed)
                {
                    this.MiddleButton = state.MiddleButton;
                    this.DispatchMouseEvent(WM_MESSAGE.MOUSE_DOWN, state.Position, MouseButtons.Middle);
                }
                else
                {
                    this.MiddleButton = state.MiddleButton;
                    this.DispatchMouseEvent(WM_MESSAGE.MOUSE_UP, state.Position, MouseButtons.Middle);
                }

            }
        }


        private Int32 ScrollWheelValue;
        private ButtonState LeftButton;
        private ButtonState MiddleButton;
        private ButtonState RightButton;
        private Point MousePosition;

    }
}
