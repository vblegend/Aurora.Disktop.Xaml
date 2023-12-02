using Aurora.UI.Animation;
using Aurora.UI.Animation.Tweens;
using Aurora.UI.Common;
using Aurora.UI.Graphics;
using Aurora.UI.Tweens;
using Aurora.UI.Tweens.Tweens;
using Microsoft.Xna.Framework;

namespace Aurora.UI.Controls
{
    public class ProgressBar : Control
    {

        private TweenDouble _percent = new TweenDouble();

        private Double? shaowValue = 00.0f;


        public ProgressBar()
        {
            this.DelayTime = 500;
            this.FillMode = FillMode.None;
            this.Direction = XamlDirection.TopToBottom;
            this._percent.ChangeTo(0);
            this._percent.Easing = Easing.Circular.InOut;
            this._percent.OnBegin += _percent_OnBegin;
            this._percent.OnComplete += _percent_OnComplete;
        }

        private void _percent_OnBegin(TweenDouble sender)
        {

        }

        private void _percent_OnComplete(TweenDouble sender)
        {
            this.shaowValue = null;
        }

        protected override void OnRender(GameTime gameTime)
        {
            base.OnRender(gameTime);



            this.DrawProgressBar(this._percent.Value, Color.White);

            if (this.shaowValue.HasValue)
            {
                //var state = this.Renderer.SetState(blendState: Microsoft.Xna.Framework.Graphics.BlendState.Additive);
                this.DrawProgressBar(this.shaowValue.Value, new Color(255, 255, 255, 128));
                //if (state.HasValue)
                //{
                //    this.Renderer.RestoreState(state.Value);
                //}
            }
        }



        private void DrawProgressBar(Double percent, Color color)
        {

            if (this.texture != null)
            {
                Rectangle tmpTarget;
                Rectangle tmpSource;
                var width = this.GlobalBounds.Width;
                var height = this.GlobalBounds.Height;
                switch (this.Direction)
                {
                    case XamlDirection.LeftToRight:
                        tmpSource = new Rectangle(0, 0, (int)(percent * this.texture.Width / 100), this.texture.Height);
                        tmpTarget = new Rectangle(this.GlobalBounds.X, this.GlobalBounds.Y, (int)(percent * width / 100), height);
                        break;
                    case XamlDirection.BottomToTop:
                        var val = (int)(percent * this.texture.Height / 100);
                        tmpSource = new Rectangle(0, this.texture.Height - val, this.texture.Width, val);
                        val = (int)(percent * height / 100);
                        tmpTarget = new Rectangle(this.GlobalBounds.X, this.GlobalBounds.Y + height - val, width, val);
                        break;
                    case XamlDirection.RightToLeft:
                        val = (int)(percent * this.texture.Width / 100);
                        tmpSource = new Rectangle(this.texture.Width - val, 0, val, this.texture.Height);
                        val = (int)(percent * width / 100);
                        tmpTarget = new Rectangle(this.GlobalBounds.X + width - val, this.GlobalBounds.Y, val, height);
                        break;
                    case XamlDirection.TopToBottom:
                        tmpSource = new Rectangle(0, 0, this.texture.Width, (int)(percent * this.texture.Height / 100));
                        tmpTarget = new Rectangle(this.GlobalBounds.X, this.GlobalBounds.Y, width, (int)(percent * height / 100));
                        break;
                    default:
                        return;
                }
                this.Renderer.Draw(this.texture, tmpTarget, tmpSource, color);
            }


        }





        protected override void OnUpdate(GameTime gameTime)
        {
            if (this._percent is IAnimationUpdateable tween) tween.Update(gameTime);
        }




        public ITexture Texture
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


        protected override void OnMouseUp(IMouseMessage args)
        {
            if (args.Button == MouseButtons.Left)
            {
                if (this.GlobalBounds.Contains(args.Location) && this.Enabled)
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

        }


        // Declare the event.
        public event XamlClickEventHandler<ProgressBar> Click;

        private ITexture texture;

        public FillMode FillMode { get; set; }



        private void CalcPercent(Boolean animation)
        {
            Double value = 0.0f;
            if (this._maxValue != this._minValue && this._value != this._minValue)
            {
                value = ((Double)(this._value - this._minValue) / (Double)(this._maxValue - this._minValue)) * 100.0f;
            }
            if (value != this._percent.ToValue)
            {
                var duration = (Int32)(Math.Abs(this._percent.Value - value) / 100.0f * DelayTime);
                this._percent.ChangeTo(value, new TimeSpan(0, 0, 0, 0, animation ? duration : 0));
                if (animation && duration > 0)
                {
                    this.shaowValue = Math.Max(value, this._percent.Value);
                }
            }
        }



        #region Properties

        public Int32 DelayTime;




        public XamlDirection Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
            }

        }
        private XamlDirection _direction;
        public Int32 Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                this.CalcPercent(true);
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
                this.CalcPercent(false);
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
                this.CalcPercent(false);
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
                return this._percent.Value;
            }
        }



        #endregion
    }
}
