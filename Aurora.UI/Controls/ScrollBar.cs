using Aurora.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            this._dec_button = new Button() { Name = "BUTTON_DEC" , VerticalAlignment = XamlVerticalAlignment.Top, HorizontalAlignment = XamlHorizontalAlignment.Stretch };
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
