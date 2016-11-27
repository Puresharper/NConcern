using System;
using System.ComponentModel;

namespace System.Reflection
{
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static internal class __AssemblyName
    {
        static public string Name(this AssemblyName name)
        {
            var _name = name.FullName;
            var _index = _name.IndexOf(',');
            if (_index > 0) { return _name.Substring(0, _index).TrimEnd(); }
            return _name;
        }
    }
}
