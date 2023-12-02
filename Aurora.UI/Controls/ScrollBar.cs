using System.Diagnostics;
using Aurora.UI.Animation;
using Aurora.UI.Common;
using Aurora.UI.Graphics;
using Microsoft.Xna.Framework;

namespace Aurora.UI.Controls
{




    public class ScrollBar : Panel, IAttachable
    {
        private Button _dec_button;
        private Button _sliding_button;
        private Button _inc_button;
        private Ticker slidingTicker;
        private Int32 SlideStart = 0;
        private Int32 SlideEnd = 0;



        public ScrollBar()
        {
            this._minValue = 0;
            this._maxValue = 100;
            this._value = 0;
            this.Step = 1;
            this._orientation = XamlOrientation.Vertical;
            this._dec_button = new Button() { Name = "#BUTTON_DEC", VerticalAlignment = XamlVerticalAlignment.Top, HorizontalAlignment = XamlHorizontalAlignment.Stretch };
            this._sliding_button = new Button() { Name = "#BUTTON_SLIDE", HorizontalAlignment = XamlHorizontalAlignment.Stretch };
            this._inc_button = new Button() { Name = "#BUTTON_INC", VerticalAlignment = XamlVerticalAlignment.Bottom, HorizontalAlignment = XamlHorizontalAlignment.Stretch };
            this._dec_button.MouseWheel += (s, e) => { e.Handled = false; };

            this._dec_button.MouseDown += _dec_button_MouseDown;
            this._dec_button.MouseUp += _dec_button_MouseUp;

            this._inc_button.MouseDown += _inc_button_MouseDown;
            this._inc_button.MouseUp += _dec_button_MouseUp;

            this._sliding_button.MouseWheel += (s, e) => { e.Handled = false; };
            this._inc_button.MouseWheel += (s, e) => { e.Handled = false; };
            this._sliding_button.MouseDown += sliding_button_MouseDown;
            this._sliding_button.MouseMove += sliding_button_MouseMove;
            this._sliding_button.MouseUp += sliding_button_MouseUp;


            this.slidingTicker = new Ticker()
            {
                Delay = new TimeSpan(0, 0, 0, 0, 300),
                Interval = new TimeSpan(0, 0, 0, 0, 100)
            };
            this.slidingTicker.Tick += slidingTicker_Tick;
        }

        #region Animation

        private void slidingTicker_Tick(Ticker sender)
        {
            this.Value += this.sliding;
        }

        private void _inc_button_MouseDown(Control sender, IMouseMessage args)
        {
            this.Value += this.Step;
            this.sliding = this.Step;
            this.slidingTicker.Start();
        }

        private Int32 sliding = 0;

        private void _dec_button_MouseDown(Control sender, IMouseMessage args)
        {
            this.Value -= this.Step;
            this.sliding = -this.Step;
            this.slidingTicker.Start();
        }

        private void _dec_button_MouseUp(Control sender, IMouseMessage args)
        {
            this.slidingTicker.Stop();
        }

        #endregion


        #region Sliding Button
        private void sliding_button_MouseUp(Control sender, IMouseMessage args)
        {
            this.dropPosition = null;
            args.Release();
        }

        private Point? dropPosition = null;
        private void sliding_button_MouseDown(Control control, IMouseMessage args)
        {
            this.dropPosition = args.GetLocation(control);
            args.Capture();
        }

        private void sliding_button_MouseMove(Control control, IMouseMessage args)
        {
            base.OnMouseMove(args);
            if (dropPosition.HasValue)
            {
                var offset = args.GetLocation(this).Sub(dropPosition.Value);
                var y = offset.Y;
                y = Math.Min(this.SlideEnd, y);
                y = Math.Max(this.SlideStart, y);
                var _p = ((Double)(y - this.SlideStart) / (Double)(this.SlideEnd - this.SlideStart));
                var v = this._minValue + (Int32)((this._maxValue - this._minValue) * _p);
                this.Value = v;
            }
        }
        #endregion



        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            (this.slidingTicker as IAnimationUpdateable).Update(gameTime);
        }




        protected override void OnMouseDown(IMouseMessage args)
        {
            base.OnMouseDown(args);
            var pos = args.GetLocation(this);
            if (pos.Y < this._sliding_button.Margin.Top)
            {
                this.Value -= this.Step;
            }
            else
            {
                this.Value += this.Step;
            }
        }

        protected override void OnMouseWheel(IMouseMessage args)
        {
            this.Value += args.Wheel > 0 ? -this.Step : this.Step;
            base.OnMouseWheel(args);
        }


        private void Updates()
        {
            var _percent = ((Double)(this._value - this._minValue) / (Double)(this._maxValue - this._minValue));
            this.SlideStart = this._dec_button.Height;
            var padding = (this.SlideEnd - this.SlideStart) * _percent;
            this._sliding_button.Margin = new Thickness(0, SlideStart + (Int32)padding, 0, 0);
        }

        void IAttachable.OnAttached()
        {
            if (this.Count == 0)
            {
                this.Add(this._dec_button);
                this.Add(this._sliding_button);
                this.Add(this._inc_button);
            }
        }



        protected override void OnLayoutUpdate()
        {
            this.SlideStart = this._dec_button.Height;
            this.SlideEnd = this.Height - this._inc_button.Height - this._sliding_button.Height;
            this._sliding_button.Margin = new Thickness(0, SlideStart, 0, 0);
            Updates();
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
                this.UpdateSkin();
            }

        }
        private ITexture texture;

        private void UpdateSkin()
        {
            if (this.texture == null) return;
            var rect = this.texture.SourceRect;
            var eleHeight = rect.Height / 3;
            var dec = new Rectangle(0, 0, rect.Width, eleHeight);
            var inc = new Rectangle(0, eleHeight, rect.Width, eleHeight);
            var sliding = new Rectangle(0, eleHeight * 2, rect.Width, eleHeight);
            if (this._dec_button != null) this._dec_button.SetTexture(this.texture, dec, 1, 3);
            if (this._inc_button != null) this._inc_button.SetTexture(this.texture, inc, 1, 3);
            if (this._sliding_button != null) this._sliding_button.SetTexture(this.texture, sliding, 1, 3);
            OnLayoutUpdate();
        }



        #region Properties
        public XamlOrientation Orientation
        {
            get
            {
                return _orientation;
            }
            set
            {
                _orientation = value;
            }

        }
        private XamlOrientation _orientation;
        public Int32 Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = Math.Min(Math.Max(value, this._minValue), this._maxValue);
                Updates();
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


        public Int32 Step;



        #endregion

    }
}
