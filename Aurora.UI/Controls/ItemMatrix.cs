using Aurora.UI.Common;
using Microsoft.Xna.Framework;


namespace Aurora.UI.Controls
{



    public class IconMaterixItem
    {
        public Int32 Index;
        public Rectangle Location;
        public Int32 Row;
        public Int32 Column;
    }





    public class ItemMatrix : Control
    {
        private IconMaterixItem[] Items { get; set; } = new IconMaterixItem[0];


        public ItemMatrix()
        {
            this.rows = 2;
            this.columns = 2;
            this.itemSize = new Point(32, 32);
            this.space = new Point(0, 0);
            this.UpdateLayout();
        }





        protected override void OnRender(GameTime gameTime)
        {
            if (this.Background != null)
            {
                this.Background.Draw(Renderer, this.GlobalBounds, Color.White);
            }
            for (int i = 0; i < Items.Length; i++)
            {
                var dest = Items[i].Location.Add(this.GlobalBounds.Location);
                //this.Renderer.DrawRectangle(dest, Color.Orange, 1);
                this.ItemDrawing?.Invoke(this, new ImageMatrixDrawEventArgs<IconMaterixItem>(i, Items[i], dest, this.Renderer, gameTime));
            }
        }



        protected override void OnMouseMove(IMouseMessage args)
        {
            var pos = args.GetLocation(this);
            var index = this.GetGridIndex(pos);

            if (index != this.HoverIndex)
            {
                if (this.HoverIndex.HasValue)
                {
                    this.ItemMouseLeave?.Invoke(this, new ItemEventArgs<IconMaterixItem>(this.HoverIndex.Value, this.Items[this.HoverIndex.Value], Point.Zero));
                }
                this.HoverIndex = index;
                if (this.HoverIndex.HasValue)
                {
                    this.ItemMouseEnter?.Invoke(this, new ItemEventArgs<IconMaterixItem>(this.HoverIndex.Value, this.Items[this.HoverIndex.Value], Point.Zero));
                }
            }
        }
        private Int32? RParssedIndex = null;
        private Int32? LParssedIndex = null;
        private Int32? HoverIndex = null;

        protected override void OnMouseLeave()
        {
            if (this.HoverIndex.HasValue)
            {
                this.ItemMouseLeave?.Invoke(this, new ItemEventArgs<IconMaterixItem>(this.HoverIndex.Value, this.Items[this.HoverIndex.Value], Point.Zero));
                this.HoverIndex = null;
            }
        }


        protected override void OnMouseDown(IMouseMessage args)
        {
            var pos = args.GetLocation(this);
            var index = this.GetGridIndex(pos);
            if (args.Button == MouseButtons.Left)
            {
                this.LParssedIndex = index;
            }
            else
            {
                this.RParssedIndex = index;
            }
        }

        protected override void OnMouseUp(IMouseMessage args)
        {
            var pos = args.GetLocation(this);
            var index = this.GetGridIndex(pos);
            if (args.Button == MouseButtons.Left && this.LParssedIndex.HasValue)
            {
                if (index == this.LParssedIndex)
                {
                    this.ItemClick?.Invoke(this, new ItemEventArgs<IconMaterixItem>(index.Value, this.Items[index.Value], args.Location));
                }
                this.LParssedIndex = null;
            }
            else if (args.Button == MouseButtons.Right && this.RParssedIndex.HasValue)
            {
                if (index == this.RParssedIndex)
                {
                    this.ItemMenu?.Invoke(this, new ItemEventArgs<IconMaterixItem>(index.Value, this.Items[index.Value], args.Location));
                }
                this.RParssedIndex = null;
            }
        }






        private void UpdateLayout()
        {
            var num = this.rows * this.columns;
            if (this.Items.Length != num)
            {
                this.Items = new IconMaterixItem[num];
            }
            var itemSpace = new Point(this.itemSize.X + this.space.X, this.itemSize.Y + this.space.Y);
            for (int i = 0; i < num; i++)
            {
                var row = (Int32)(i / this.columns);
                var column = (Int32)(i % this.columns);
                var left = this.padding.Left + column * itemSpace.X + column;
                var top = this.padding.Top + row * itemSpace.Y + row;
                this.Items[i] = new IconMaterixItem();
                this.Items[i].Index = i;
                this.Items[i].Row = row;
                this.Items[i].Column = column;
                this.Items[i].Location = new Rectangle(left, top, this.itemSize.X, this.itemSize.Y);
            }

        }



        /// <summary>
        /// 获取坐标所在格子索引
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Int32? GetGridIndex(Point point)
        {
            var spaceX = this.itemSize.X + this.space.X + 1;
            var spaceY = this.itemSize.Y + this.space.Y + 1;

            // 考虑 Padding
            var x = point.X - this.padding.Left;
            var y = point.Y - this.padding.Top;
            // 计算格子所在的列，考虑 spacex
            int column = x / spaceX;
            // 计算格子所在的行，考虑 spacey
            int row = y / spaceY;

            // 判断是否在有效范围内
            if (column >= Columns || row >= Rows)
            {
                return null; // 无效坐标
            }
            // 判断是否在格子内，而不是在格子之间的间隙内
            int xOffset = x % spaceX;
            int yOffset = y % spaceY;
            // 在格子间隙内
            if (xOffset >= this.itemSize.X || yOffset >= this.itemSize.Y)
            {
                return null;
            }
            return row * this.columns + column;
        }




        protected override void CalcAutoSize()
        {
            if (this.NeedCalcAutoHeight)
            {
                this.globalBounds.Height = this.padding.Top + this.padding.Bottom + this.rows * (this.itemSize.Y + this.space.Y + 1) - this.space.Y - 1;
            }
            if (this.NeedCalcAutoWidth)
            {
                this.globalBounds.Width = this.padding.Left + this.padding.Right + this.columns * (this.itemSize.X + this.space.X + 1) - this.space.X - 1;
            }
        }

        #region Events
        public virtual event XamlItemEventHandler<ItemMatrix, IconMaterixItem> ItemMouseEnter;
        public virtual event XamlItemEventHandler<ItemMatrix, IconMaterixItem> ItemMouseLeave;

        public virtual event XamlItemEventHandler<ItemMatrix, IconMaterixItem> ItemClick;
        public virtual event XamlItemEventHandler<ItemMatrix, IconMaterixItem> ItemMenu;

        public virtual event ImageMatrixDrawEventHandler<ItemMatrix, IconMaterixItem> ItemDrawing;

        #endregion


        #region Properties


        public Int32 Count
        {
            get
            {
                return this.rows * this.columns;
            }
        }


        public Int32 Columns
        {
            get
            {
                return columns;
            }
            set
            {
                if (value <= 0) throw new Exception();
                if (columns != value)
                {
                    columns = value;
                    this.UpdateLayout();
                }
            }
        }
        private Int32 columns;

        public Int32 Rows
        {
            get
            {
                return rows;
            }
            set
            {
                if (value <= 0) throw new Exception();
                if (rows != value)
                {
                    rows = value;
                    this.UpdateLayout();
                }

            }
        }
        private Int32 rows;






        public Point Space
        {
            get
            {
                return space;
            }
            set
            {
                if (value.X < 0 || value.Y < 0) throw new Exception();
                if (space != value)
                {
                    space = value;
                    this.UpdateLayout();
                }
            }
        }
        private Point space;




        public Point ItemSize
        {
            get
            {
                return itemSize;
            }
            set
            {
                if (value.X <= 0 || value.Y <= 0) throw new Exception();
                if (itemSize != value)
                {
                    itemSize = value;
                    this.UpdateLayout();
                }

            }
        }
        private Point itemSize;

        public Thickness Padding
        {
            get
            {
                return padding;
            }
            set
            {
                if (padding != value)
                {
                    padding = value;
                    this.UpdateLayout();
                }
            }
        }
        private Thickness padding;

        #endregion

    }
}
