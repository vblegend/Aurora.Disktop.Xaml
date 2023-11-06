using Aurora.Disktop.Common;
using Aurora.Disktop.Common.Tweens;
using Aurora.Disktop.Graphics;
using Microsoft.Xna.Framework;


namespace Aurora.Disktop.Controls
{
    public class ProgressBar : Control
    {

        public TweenRectangle rectangle = new TweenRectangle();


        public ProgressBar()
        {
            this.FillMode = FillMode.None;
            this.Style = XamlProgressBarStyle.TopToBottom;
            this.rectangle.ChangeTo(new Rectangle(), new TimeSpan(0));
        }


        protected override void OnRender(GameTime gameTime)
        {
            base.OnRender(gameTime);

            if (this.texture != null)
            {
                Rectangle tmpTarget;
                Rectangle tmpSource;
                var width = this.GlobalBounds.Width;
                var height = this.GlobalBounds.Height;

                Double percent = this.Percent;
                switch (this.Style)
                {
                    case XamlProgressBarStyle.LeftToRight:
                        tmpSource = new Rectangle(0, 0, (int)(percent * this.texture.Width / 100), this.texture.Height);
                        tmpTarget = new Rectangle(this.GlobalBounds.X, this.GlobalBounds.Y, (int)(percent * width / 100), height);
                        break;
                    case XamlProgressBarStyle.BottomToTop:
                        var val = (int)(percent * this.texture.Height / 100);
                        tmpSource = new Rectangle(0, this.texture.Height - val, this.texture.Width, val);
                        val = (int)(percent * height / 100);
                        tmpTarget = new Rectangle(this.GlobalBounds.X, this.GlobalBounds.Y + height - val, width, val);
                        break;
                    case XamlProgressBarStyle.RightToLeft:
                        val = (int)(percent * this.texture.Width / 100);
                        tmpSource = new Rectangle(this.texture.Width - val, 0, val, this.texture.Height);
                        val = (int)(percent * width / 100);
                        tmpTarget = new Rectangle(this.GlobalBounds.X + width - val, this.GlobalBounds.Y, val, height);
                        break;
                    case XamlProgressBarStyle.TopToBottom:
                        tmpSource = new Rectangle(0, 0, this.texture.Width, (int)(percent * this.texture.Height / 100));
                        tmpTarget = new Rectangle(this.GlobalBounds.X, this.GlobalBounds.Y, width, (int)(percent * height / 100));
                        break;
                    default:
                        return;
                }

                this.Renderer.Draw(this.texture, tmpTarget, tmpSource, Color.White);
            }
        }



        protected override void OnUpdate(GameTime gameTime)
        {
            if (this.rectangle is ITweenUpdateable tween) tween.Update(gameTime);
        }


 

        public SimpleTexture Texture
        {
            get
            {
                return this.texture;
            }
            set
            {
                this.texture = value;
                if (this.Size.Equals(Point.Zero) && this.texture != null)
                {
                    this.Size = new Point(this.texture.Width, this.texture.Height);
                }
            }

        }


        protected override void OnMouseUp(MouseButtons button, Point point)
        {
            if (button == MouseButtons.Left)
            {
                if (this.GlobalBounds.Contains(point) && this.Enabled)
                {
                    this.Click?.Invoke(this);
                }

            }
        }


        protected override void CalcAutoSize()
        {
            if (this.NeedCalcAutoHeight && this.texture != null)
            {
                this.globalBounds.Height = texture.Height;
            }
            if (this.NeedCalcAutoWidth && this.texture != null)
            {
                this.globalBounds.Width = texture.Width;
            }
            this.rectangle.ChangeTo(this.globalBounds, new TimeSpan(0));
        }


        // Declare the event.
        public event XamlClickEventHandler<ProgressBar> Click;

        private SimpleTexture texture;

        public FillMode FillMode { get; set; }



        #region Properties
        public XamlProgressBarStyle Style
        {
            get
            {
                return _style;
            }
            set
            {
                _style = value;
            }

        }
        private XamlProgressBarStyle _style;
        public Int32 Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }

        }
        private Int32 _value;


        public Int32 MaxValue
        {
            get
            {
                return _maxValue;
            }
            set
            {
                _maxValue = value;
            }

        }
        private Int32 _maxValue;


        public Int32 MinValue
        {
            get
            {
                return _minValue;
            }
            set
            {
                _minValue = value;
            }

        }
        private Int32 _minValue;


        /// <summary>
        /// 当前百分比
        /// </summary>
        public Double Percent
        {
            get
            {
                if (this._value == this._minValue)
                {
                    return 0;
                }
                //计算比率
                return ((Double)(this._value - this._minValue) / (Double)(this._maxValue - this._minValue)) * 100.0f;
            }
        }



        #endregion
    }
}
