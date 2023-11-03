using Aurora.Disktop.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora.Disktop.Common
{
    public interface IControlList
    {

        public T Add<T>(T control) where T: Control;

        public void Insert(Int32 index, Control control);

        public Boolean Remove(Control control);

        public Boolean Remove();

        public Int32 IndexOf(Control control);

    }
}
