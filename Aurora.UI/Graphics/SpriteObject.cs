
using Microsoft.Xna.Framework;


namespace Aurora.UI.Graphics
{
    /// <summary>
    /// 精灵对象
    /// </summary>
    public class SpriteObject
    {
        private Rectangle[] sheets;
        public ITexture Texture { get; private set; }
        public Int32 FrameCount { get; private set; }
        public Int32 Width { get; private set; }
        public Int32 Height { get; private set; }
        public Int32 Rows { get; private set; }
        public Int32 Columns { get; private set; }
        public Rectangle SourceRect { get; private set; }

        public SpriteObject(Int32 rows, Int32 columns)
        {
            this.sheets = new Rectangle[0];
            this.Rows = rows;
            this.Columns = columns;
            this.FrameCount = rows * columns;
        }

        public SpriteObject(ITexture texture, Rectangle sourceRect, Int32 rows, Int32 columns)
        {
            this.sheets = new Rectangle[0];
            if (rows < 1) throw new Exception("");
            if (columns < 1) throw new Exception("");
            this.Rows = rows;
            this.Columns = columns;
            this.FrameCount = rows * columns;
            this.SetTexture(texture, sourceRect);
        }

        public SpriteObject(ITexture texture, Int32 rows, Int32 columns) : this(texture, new Rectangle(0, 0, texture.Width, texture.Height), rows, columns)
        {
        }


        public void SetTexture(ITexture texture, Rectangle sourceRect)
        {
            this.Texture = texture;
            this.SourceRect = sourceRect;
            this.Width = sourceRect.Width / this.Columns;
            this.Height = sourceRect.Height / this.Rows;
            this.sheets = new Rectangle[this.Rows * this.Columns];
            var index = 0;
            for (int i = 0; i < this.Rows; i++)
            {
                for (int j = 0; j < this.Columns; j++)
                {
                    this.sheets[index] = new Rectangle(sourceRect.Left + j * this.Width, sourceRect.Top + i * this.Height, this.Width, this.Height);
                    index++;
                }
            }
        }



        public Rectangle GetFrameRectangle(Int32 index)
        {
            if (index < 0 || index >= this.FrameCount)
            {
                throw new Exception("数组越界");
            }
            return this.sheets[index];
        }

    }
}
