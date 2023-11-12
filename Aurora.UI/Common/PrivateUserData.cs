using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora.UI.Common
{
    public interface IUserData
    {
        public void Clear();
        public void Set(String key, Object value);
        public T Get<T>(String key) where T : class;

        public Nullable<T> GetValue<T>(String key) where T : struct;

    }




    internal class PrivateUserData : IUserData
    {
        private Dictionary<String, Object> _data;


        private Dictionary<String, Object> getHashMap()
        {
            if (_data == null) _data = new Dictionary<String, Object>();
            return _data;
        }

        public T Get<T>(string key) where T : class
        {
            var map = getHashMap();
            if (map.TryGetValue(key, out var value))
            {
                return value as T;
            };
            return null;
        }

        public void Set(string key, object value)
        {
            var map = getHashMap();
            map.Add(key, value);    
        }


        public Nullable<T> GetValue<T>(string key) where T : struct
        {
            var map = getHashMap();
            if (map.TryGetValue(key, out var value))
            {
                return (T)value;
            };
            return null;
        }

        public void Clear()
        {
            if (this._data != null) 
            {
                this._data.Clear();
                this._data = null;
            }
        }
    }
}
