using Aurora.UI.Common;
using Aurora.UI.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;



namespace Aurora.UI.Components {

    public enum Cursors
    {
        /// <summary>
        /// 指针
        /// </summary>
        Pointer = 0,
        /// <summary>
        /// 文本
        /// </summary>
        Text = 1,
        /// <summary>
        /// 帮助
        /// </summary>
        Help = 2,
        /// <summary>
        /// 十字
        /// </summary>
        Corss = 3,
        /// <summary>
        /// 手（超链接）
        /// </summary>
        Hand = 4,
    }

    public enum CursorState
    {
        System = 0,
        Custom = 1,
    }


    public class CursorComponent : IXamlComponent
    {
        private CursorState state;
        private Vector2 position;
        private PlayWindow window { get; set; }
        private Int32 Index;

        public SpriteObject Skin;

        public Cursors Cursor;

        /// <summary>
        /// 光标类型
        /// </summary>
        public CursorState State
        {
            get
            {
                return this.state;
            }
            set
            {
                this.state = value;
                window.IsMouseVisible = value == CursorState.System;
            }
        }

        private Stack<Cursors> stack = new Stack<Cursors>();

        public CursorComponent(PlayWindow window)
        {
            this.window = window;
            this.State = CursorState.System;
        }


        public void Push(Cursors cursor)
        {
            this.stack.Push(this.Cursor);
            this.Cursor = cursor;
        }

        public void Pop()
        {
            if (this.stack.Count > 0)
            {
                var cursor = this.stack.Pop();
                this.Cursor = cursor;
            }
        }


        public void Update(GameTime gameTime)
        {
            if (this.State == CursorState.Custom)
            {
                MouseState mouseState = Mouse.GetState(window.Window);
                this.position.X = (float)mouseState.X;
                this.position.Y = (float)mouseState.Y;
                this.Index = this.Skin.Columns * (Int32)this.Cursor;
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (this.State == CursorState.Custom)
            {
                var sourceRect = this.Skin.GetFrameRectangle(this.Index);
                this.window.GraphicContext.Draw(this.Skin.Texture, this.position, sourceRect, Color.White);
            }
        }

    }
}