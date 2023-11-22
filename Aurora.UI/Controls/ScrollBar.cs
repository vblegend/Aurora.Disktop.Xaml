using Aurora.UI.Common;
using Aurora.UI.Graphics;
using Microsoft.Xna.Framework;

namespace Aurora.UI.Controls
{




    public class ScrollBar : Panel, IAttachable
    {
        private Button _dec_button;
        private Button _slide_button;
        private Button _inc_button;



        public ScrollBar()
        {
            this._minValue = 0;
            this._maxValue = 100;
            this._value = 0;
            this._orientation = XamlOrientation.Vertical;

            this._dec_button = new Button() { Name = "BUTTON_DEC", VerticalAlignment = XamlVerticalAlignment.Top, HorizontalAlignment = XamlHorizontalAlignment.Stretch };
            this._slide_button = new Button() { Name = "BUTTON_SLIDE", HorizontalAlignment = XamlHorizontalAlignment.Stretch };
            this._inc_button = new Button() { Name = "BUTTON_INC", VerticalAlignment = XamlVerticalAlignment.Bottom, HorizontalAlignment = XamlHorizontalAlignment.Stretch };



        }

        private void Updates()
        {
            if (this.Count == 0)
            {
                this.Add(this._dec_button);
                this.Add(this._slide_button);
                this.Add(this._inc_button);
            }
        }

        void IAttachable.OnAttached()
        {
            Updates();
        }



        private void UpdateSkin()
        {
            if (this.skin == null) return;
            var rect = this.skin.SourceRect;
            var eleHeight = rect.Height / 3;
            var dec = new Rectangle(0, 0, rect.Width, eleHeight);
            var inc = new Rectangle(0, eleHeight, rect.Width, eleHeight);
            var slide = new Rectangle(0, eleHeight * 2, rect.Width, eleHeight);
            if (this._dec_button != null) this._dec_button.Image = new SpriteObject(this.skin, dec, 1, 3);
            if (this._inc_button != null) this._inc_button.Image = new SpriteObject(this.skin, inc, 1, 3);
            if (this._slide_button != null) this._slide_button.Image = new SpriteObject(this.skin, slide, 1, 3);

            this._slide_button.Margin = new Thickness(0,100,0,0);


        }







        public ITexture Skin
        {
            get
            {
                return this.skin;
            }
            set
            {
                this.skin = value;
                this.UpdateSkin();
            }

        }
        private ITexture skin;




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

        #endregion

    }
}
