using Aurora.UI.Common;
using Microsoft.Xna.Framework;
using System;


namespace Aurora.UI.Controls
{


    public interface IMatrixItem
    {
        public Int32 Index { get; }
        public Rectangle Location { get; }
        public Int32 Row { get; }
        public Int32 Column { get; }
    }

    internal class PrivateMatrixItem : IMatrixItem
    {
        public Int32 Index { get; set; }
        public Rectangle Location { get; set; }
        public Int32 Row { get; set; }
        public Int32 Column { get; set; }
    }





    public class ItemMatrix : Control
    {
        private PrivateMatrixItem[] MatrixItems { get; set; } = new PrivateMatrixItem[0];

        public Object[] Items { get; set; }

        public ItemMatrix()
        {
            this.Items = new Object[65];
            for (int i = 0; i < Items.Length; i++)
            {
                this.Items[i] = new object();
            }
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
            var len = Math.Min(MatrixItems.Length, this.Items.Length);
            for (int i = 0; i < len; i++)
            {
                var dest = MatrixItems[i].Location.Add(this.GlobalBounds.Location);
                //this.Renderer.DrawRectangle(dest, Color.Orange, 1);
                this.ItemDrawing?.Invoke(this, new MatrixDrawEventArgs<IMatrixItem>(i, MatrixItems[i], dest, Items[i], this.Renderer, gameTime));
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
                    var obj = this.HoverIndex.Value < this.Items.Length ? Items[this.HoverIndex.Value] : null;
                    this.ItemMouseLeave?.Invoke(this, new MatrixEventArgs<IMatrixItem>(this.HoverIndex.Value, this.MatrixItems[this.HoverIndex.Value], obj, Point.Zero));
                }
                this.HoverIndex = index;
                if (this.HoverIndex.HasValue)
                {
                    var obj = this.HoverIndex.Value < this.Items.Length ? Items[this.HoverIndex.Value] : null;
                    this.ItemMouseEnter?.Invoke(this, new MatrixEventArgs<IMatrixItem>(this.HoverIndex.Value, this.MatrixItems[this.HoverIndex.Value], obj, Point.Zero));
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
                var obj = this.HoverIndex.Value < this.Items.Length ? Items[this.HoverIndex.Value] : null;
                this.ItemMouseLeave?.Invoke(this, new MatrixEventArgs<IMatrixItem>(this.HoverIndex.Value, this.MatrixItems[this.HoverIndex.Value], obj, Point.Zero));
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
            if (index.HasValue)
            {
                var obj = index.Value < this.Items.Length ? Items[index.Value] : null;
                this.ItemMouseDown?.Invoke(this, new MatrixEventArgs<IMatrixItem>(index.Value, this.MatrixItems[index.Value], obj, args.Location));
            }
        }

        protected override void OnMouseUp(IMouseMessage args)
        {
            var pos = args.GetLocation(this);
            var index = this.GetGridIndex(pos);

            if (index.HasValue)
            {
                var obj = index.Value < this.Items.Length ? Items[index.Value] : null;
                this.ItemMouseUp?.Invoke(this, new MatrixEventArgs<IMatrixItem>(index.Value, this.MatrixItems[index.Value], obj, args.Location));
            }

            if (args.Button == MouseButtons.Left && this.LParssedIndex.HasValue)
            {
                if (index == this.LParssedIndex)
                {
                    var obj = index.Value < this.Items.Length ? Items[index.Value] : null;
                    this.ItemClick?.Invoke(this, new MatrixEventArgs<IMatrixItem>(index.Value, this.MatrixItems[index.Value], obj, args.Location));
                }
                this.LParssedIndex = null;
            }
            else if (args.Button == MouseButtons.Right && this.RParssedIndex.HasValue)
            {
                if (index == this.RParssedIndex)
                {
                    var obj = index.Value < this.Items.Length ? Items[index.Value] : null;
                    this.ItemMenu?.Invoke(this, new MatrixEventArgs<IMatrixItem>(index.Value, this.MatrixItems[index.Value], obj, args.Location));
                }
                this.RParssedIndex = null;
            }
        }






        private void UpdateLayout()
        {
            var num = this.rows * this.columns;
            if (this.MatrixItems.Length != num)
            {
                this.MatrixItems = new PrivateMatrixItem[num];
            }
            var itemSpace = new Point(this.itemSize.X + this.space.X, this.itemSize.Y + this.space.Y);
            for (int i = 0; i < num; i++)
            {
                var row = (Int32)(i / this.columns);
                var column = (Int32)(i % this.columns);
                var left = this.padding.Left + column * itemSpace.X + column;
                var top = this.padding.Top + row * itemSpace.Y + row;
                this.MatrixItems[i] = new PrivateMatrixItem();
                this.MatrixItems[i].Index = i;
                this.MatrixItems[i].Row = row;
                this.MatrixItems[i].Column = column;
                this.MatrixItems[i].Location = new Rectangle(left, top, this.itemSize.X, this.itemSize.Y);
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

            if (x < 0 || y < 0) return null;

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
        public virtual event XamlItemEventHandler<ItemMatrix, IMatrixItem> ItemMouseEnter;
        public virtual event XamlItemEventHandler<ItemMatrix, IMatrixItem> ItemMouseLeave;


        public virtual event XamlItemEventHandler<ItemMatrix, IMatrixItem> ItemMouseDown;
        public virtual event XamlItemEventHandler<ItemMatrix, IMatrixItem> ItemMouseUp;

        public virtual event XamlItemEventHandler<ItemMatrix, IMatrixItem> ItemClick;
        public virtual event XamlItemEventHandler<ItemMatrix, IMatrixItem> ItemMenu;

        public virtual event ImageMatrixDrawEventHandler<ItemMatrix, IMatrixItem> ItemDrawing;

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
