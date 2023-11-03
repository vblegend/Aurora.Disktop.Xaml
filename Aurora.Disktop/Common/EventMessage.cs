using Aurora.Disktop.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Security.Cryptography;

namespace Aurora.Disktop.Common
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

    }




    public enum MouseButtons
    {
        None = 0,
        Left = 1,
        Middle = 2,
        Right = 3
    }




    public class EventMessage
    {

        public EventMessage()
        {
                
        }

        public EventMessage(WM_MESSAGE msg)
        {
            this.Message = msg;
        }



        public static EventMessage MouseMessage(WM_MESSAGE msg, Point local, MouseButtons button)
        {
            var ret = new EventMessage();
            ret.Message = msg;
            ret.Location = local;
            ret.Button = button;
            return ret;
        }



        public static EventMessage MouseMessage(WM_MESSAGE msg, Point local)
        {
            var ret = new EventMessage();
            ret.Message = msg;
            ret.Location = local;
            return ret;
        }


        public static EventMessage WheelMessage(WM_MESSAGE msg, Point local,Int32 wheel)
        {
            var ret = new EventMessage();
            ret.Message = msg;
            ret.Location = local;
            ret.Wheel = wheel;
            return ret;
        }

        public WM_MESSAGE Message { get; private set; }
        public Point Location;
        public MouseButtons Button;
        public Int32 Wheel;
    }





}
