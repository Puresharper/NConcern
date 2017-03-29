using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace NConcern
{
    static public partial class Aspect
    {
        static private partial class Directory
        {
            private sealed partial class Entry : IEnumerable<IAspect>
            {
                static private string Identity(Type type)
                {
                    var _index = type.Name.IndexOf('`');
                    var _name = _index < 0 ? type.Name : type.Name.Substring(0, _index);
                    if (type.GetGenericArguments().Length == 0) { return string.Concat("<", _name, ">"); }
                    _name = string.Concat(_name, "<", type.GetGenericArguments().Length.ToString(), ">");
                    return string.Concat("<", _name, string.Concat("<", string.Concat(type.GetGenericArguments().Select(_argument => string.Concat("<", _argument.Name, ">"))), ">"), ">");
                }

                static private string Identity(MethodBase method)
                {
                    return string.Concat("<", method.IsConstructor ? method.DeclaringType.Name : method.Name, method.GetGenericArguments().Length > 0 ? string.Concat("<", method.GetGenericArguments().Length, ">") : string.Empty, method.GetParameters().Length > 0 ? string.Concat("<", string.Concat(method.GetParameters().Select(_parameter => Identity(_parameter.ParameterType))), ">") : string.Empty, ">");
                }

                static private FieldInfo Pointer(MethodBase method)
                {
                    if (method.IsGenericMethod)
                    {
                        var _method = (method as MethodInfo).GetGenericMethodDefinition();
                        return Type.GetType(string.Concat(_method.DeclaringType.FullName, "+<Neptune>+") + Identity(_method) + ", " + _method.DeclaringType.Assembly.FullName).MakeGenericType(method.GetGenericArguments()).GetField("<Pointer>");
                    }
                    return Type.GetType(string.Concat(method.DeclaringType.FullName, "+<Neptune>+") + Identity(method) + ", " + method.DeclaringType.Assembly.FullName).GetField("<Pointer>");
                }

                public readonly Type Type;
                public readonly MethodBase Method;
                public readonly Activity Activity;
                private readonly LinkedList<IAspect> m_Aspectization;
                private readonly Dictionary<IAspect, Activity> m_Dictionary;
                private readonly IntPtr m_Pointer;
                private readonly FieldInfo m_Field;

                unsafe internal Entry(Type type, MethodBase method, Activity activity)
                {
                    this.Type = type;
                    this.Method = method;
                    this.Activity = activity;
                    this.m_Aspectization = new LinkedList<IAspect>();
                    this.m_Dictionary = new Dictionary<IAspect, Activity>();
                    var _type = type.GetNestedType("<Neptune>", BindingFlags.NonPublic);
                    if (_type == null) { throw new NotSupportedException(string.Format("type '{0}' is not managed by CNeptune and cannot be supervised.", type.AssemblyQualifiedName)); }
                    this.m_Field = Pointer(method);
                    this.m_Pointer = (IntPtr)this.m_Field.GetValue(null);
                }

                private void Update()
                {
                    var _aspectization = this.m_Aspectization.SelectMany(_Aspect => _Aspect.Advise(this.Method)).ToArray();
                    var _pointer = this.m_Pointer;
                    foreach (var _advice in _aspectization)
                    {
                        if (_advice == null) { continue; }
                        var _method = _advice.Decorate(this.Method, _pointer);
                        if (_method != null) { _pointer = _method.Pointer(); }
                    }
                    this.m_Field.SetValue(null, _pointer);
                }

                public void Add(IAspect aspect)
                {
                    if (this.m_Dictionary.ContainsKey(aspect))
                    {
                        this.Update();
                        return;
                    }
                    this.m_Aspectization.AddFirst(aspect);
                    this.m_Dictionary.Add(aspect, null);
                    this.Update();
                }

                public void Remove(IAspect aspect)
                {
                    if (this.m_Dictionary.Remove(aspect))
                    {
                        this.m_Aspectization.Remove(aspect);
                        this.Update();
                    }
                }

                IEnumerator<IAspect> IEnumerable<IAspect>.GetEnumerator()
                {
                    return this.m_Aspectization.GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return this.m_Aspectization.GetEnumerator();
                }
            }

            [DebuggerDisplay("{Debugger.Display(this) , nq}")]
            [DebuggerTypeProxy(typeof(Entry.Debugger))]
            private sealed partial class Entry
            {
                private class Debugger
                {
                    static public string Display(Aspect.Directory.Entry map)
                    {
                        return string.Concat(map.Type.Declaration(), ".", map.Method.Declaration(), " = ", map.m_Aspectization.Count.ToString());
                    }

                    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
                    private Aspect.Directory.Entry m_Map;

                    public Debugger(Aspect.Directory.Entry map)
                    {
                        this.m_Map = map;
                    }

                    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
                    public IAspect[] View
                    {
                        get { return this.m_Map.m_Aspectization.ToArray(); }
                    }
                }
            }
        }
    }
}