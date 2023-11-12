﻿using Aurora.UI.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Aurora.UI.Common
{

    public enum WM_MESSAGE
    {

        GOTFOCUS = 10,
        LOSTFOCUS = 11,



        MOUSE_ENTER = 100,
        MOUSE_LEAVE = 101,
        MOUSE_UP = 102,
        MOUSE_DOWN = 103,
        MOUSE_MOVE = 104,
        MOUSE_WHEEL = 105,
        MOUSE_DRAGSTART = 106,
        MOUSE_DRAGMOVE = 107,
        MOUSE_DRAGOVER = 108,




        KEY_DOWN = 200,
        KEY_UP = 201,
        TEXT_INPUT = 202,



    }




    public enum MouseButtons
    {
        None = 0,
        Left = 1,
        Middle = 2,
        Right = 3
    }


    public interface IInputMessage
    {
        public WM_MESSAGE Message { get; }
        public Boolean Shift { get; }
        public Boolean Ctrl { get; }
    }


    public interface IMouseMessage : IInputMessage
    {
        public Point Location { get; }
        public MouseButtons Button { get; }
        public Int32 Wheel { get; }

        public Point GetLocation(Control control);
    }

    public interface IKeyboardMessage : IInputMessage
    {
        public Keys Key { get; }

    }

    public class IntenelMouseMessage : IMouseMessage
    {

        public IntenelMouseMessage()
        {

        }
        public IntenelMouseMessage(WM_MESSAGE Message)
        {
            this.Message = Message;
        }


        public void DoDrag(Control dataSource, Object data, Int32 dragEffect)
        {

            if (this.Message == WM_MESSAGE.MOUSE_MOVE && this.Button == MouseButtons.Left)
            {


            }
        }



        public Boolean Shift { get; set; }
        public Boolean Ctrl { get; set; }
        public WM_MESSAGE Message { get; set; }
        public Point Location { get; set; }
        public MouseButtons Button { get; set; }
        public Int32 Wheel { get; set; }

        public Point GetLocation(Control control)
        {
            var p1 = this.Location;
            var p2 = control.GlobalBounds.Location;
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }
    }

    public class IntenelKeyBoardMessage : IKeyboardMessage
    {

        public IntenelKeyBoardMessage()
        {

        }
        public IntenelKeyBoardMessage(WM_MESSAGE Message)
        {
            this.Message = Message;
        }

        public WM_MESSAGE Message { get; set; }
        public Keys Key { get; set; }
        public Boolean Shift { get; set; }
        public Boolean Ctrl { get; set; }
    }


    public interface IMessageHandler
    {
        void ProcessMessage(IInputMessage msg);
    }

}
