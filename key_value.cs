using System;
using System.Data;
using System.Configuration;
using System.Collections;

namespace gbe
{
    [Serializable]
    public class key_value
    {
        key_value() { }

        public key_value(string k, object v) { key = k; value = v; }

        public string key = string.Empty;
        public object value = null;
    }

    [Serializable]
    public class key_value_container
    {
        public object[] container;

    }
}
