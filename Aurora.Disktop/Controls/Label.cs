using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora.Disktop.Controls
{
    public class Label : Control, IPanelControl
    {
        public Label()
        {
            this.Children = new List<Control>();
        }

        public List<Control> Children { get; protected set; }

        public Control? this[int index] => this.Children[index];

        public int Count => this.Children.Count;


        public T Add<T>(T control) where T : Control
        {
            this.Children.Add(control);
            return control;
        }

        public int IndexOf(Control control)
        {
            return this.Children.IndexOf(control);
        }

        public void Remove(Control control)
        {
            this.Children.Remove(control);
        }

        protected override void OnRender(GameTime gameTime)
        {



        }


    }
}
