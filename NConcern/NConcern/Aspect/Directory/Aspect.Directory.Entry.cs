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
                public readonly Type Type;
                public readonly MethodBase Method;
                public readonly Activity Activity;
                private readonly LinkedList<IAspect> m_Aspectization;
                private readonly Dictionary<IAspect, Activity> m_Dictionary;
                private readonly Action<MethodBase, Func<MethodInfo, MethodInfo>> m_Update;

                unsafe internal Entry(Type type, MethodBase method, Activity activity)
                {
                    method.Prepare();
                    this.Type = type;
                    this.Method = method;
                    this.Activity = activity;
                    this.m_Aspectization = new LinkedList<IAspect>();
                    this.m_Dictionary = new Dictionary<IAspect, Activity>();
                    var _type = type.GetNestedType("<Neptune>", BindingFlags.NonPublic);
                    if (_type == null) { throw new NotSupportedException(string.Format("type '{0}' is not managed by CNeptune and cannot be supervised.", type.AssemblyQualifiedName)); }
                    this.m_Update = _type.GetField("<Update>").GetValue(null) as Action<MethodBase, Func<MethodInfo, MethodInfo>>;
                }

                private void Update()
                {
                    var _aspectization = this.m_Aspectization.SelectMany(_Aspect => _Aspect.Advise(this.Method)).ToArray();
                    this.m_Update
                    (
                        this.Method,
                        _Method =>
                        {
                            var _method = _Method;
                            foreach (var _advice in _aspectization)
                            {
                                if (_advice == null) { continue; }
                                _method = _advice.Decorate(this.Method, _method.Pointer()) ?? _method;
                            }
                            return _method;
                        }
                    );
                    this.Method.Prepare();
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