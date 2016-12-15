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
                static private readonly ModuleBuilder m_Module = AppDomain.CurrentDomain.DefineDynamicModule();

                private readonly IntPtr m_Pointer;
                public readonly Type Type;
                public readonly MethodInfo Method;
                public readonly Activity Activity;
                private readonly LinkedList<IAspect> m_Aspectization;
                private readonly Dictionary<IAspect, Activity> m_Dictionary;
                private readonly Action<IntPtr> m_Setup;

                unsafe internal Entry(Type type, MethodInfo method, IntPtr pointer, Activity activity)
                {
                    method.Prepare();
                    this.Type = type;
                    this.Method = method;
                    this.m_Pointer = pointer;
                    this.Activity = activity;
                    this.m_Aspectization = new LinkedList<IAspect>();
                    this.m_Dictionary = new Dictionary<IAspect, Activity>();
                    if (method.IsVirtual) { return; }

                    var _type = Entry.m_Module.DefineType(string.Concat(Metadata<Delegate>.Type.Name, Guid.NewGuid().ToString("N")), TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Abstract);
                    switch (IntPtr.Size)
                    {
                        case 4: _type.DefineField(Metadata<Entry>.Field(_Activity => _Activity.m_Pointer).Name, Metadata<int>.Type, FieldAttributes.Static | FieldAttributes.Public); break;
                        case 8: _type.DefineField(Metadata<Entry>.Field(_Activity => _Activity.m_Pointer).Name, Metadata<long>.Type, FieldAttributes.Static | FieldAttributes.Public); break;
                        default: throw new NotSupportedException();
                    }
                    var _field = _type.CreateType().GetFields(BindingFlags.Static | BindingFlags.Public).Single();
                    var _signature = this.Method.Signature();
                    var _method = new DynamicMethod(method.Name, this.Method.ReturnType, _signature, this.Method.DeclaringType, true);
                    var _body = _method.GetILGenerator();
                    for (var _index = 0; _index < _signature.Length; _index++)
                    {
                        switch (_index)
                        {
                            case 0: _body.Emit(OpCodes.Ldarg_0); break;
                            case 1: _body.Emit(OpCodes.Ldarg_1); break;
                            case 2: _body.Emit(OpCodes.Ldarg_2); break;
                            case 3: _body.Emit(OpCodes.Ldarg_3); break;
                            default: _body.Emit(OpCodes.Ldarg_S, _index); break;
                        }
                    }
                    _body.Emit(OpCodes.Ldsflda, _field);
                    _body.Emit(OpCodes.Volatile);
                    _body.Emit(OpCodes.Ldobj, _field.FieldType);
                    _body.EmitCalli(OpCodes.Calli, CallingConventions.Standard, method.ReturnType, _signature, null);
                    _body.Emit(OpCodes.Ret);
                    _method.Prepare();
                    System.Runtime.InteropServices.GCHandle.Alloc(_method);
                    switch (IntPtr.Size)
                    {
                        case 4:
                            *((int*)(this.m_Pointer)) = _method.Pointer().ToInt32();
                            this.m_Setup = Expression.Lambda<Action<IntPtr>>(Expression.Assign(Expression.Field(null, _field), Expression.Call(Parameter<IntPtr>.Expression, Metadata<IntPtr>.Method(_Pointer => _Pointer.ToInt32()))), Parameter<IntPtr>.Expression).Compile();
                            break;
                        case 8:
                            *((long*)(this.m_Pointer)) = _method.Pointer().ToInt64();
                            this.m_Setup = Expression.Lambda<Action<IntPtr>>(Expression.Assign(Expression.Field(null, _field), Expression.Call(Parameter<IntPtr>.Expression, Metadata<IntPtr>.Method(_Pointer => _Pointer.ToInt64()))), Parameter<IntPtr>.Expression).Compile();
                            break;
                        default: throw new NotSupportedException();
                    }
                    this.m_Setup(activity.Pointer);
                }

                unsafe private void Setup(IntPtr pointer)
                {
                    if (this.m_Setup == null)
                    {
                        switch (IntPtr.Size)
                        {
                            case 4: *((int*)(this.m_Pointer)) = pointer.ToInt32(); break;
                            case 8: *((long*)(this.m_Pointer)) = pointer.ToInt64(); break;
                            default: throw new NotSupportedException();
                        }
                    }
                    else { this.m_Setup(pointer); }
                }

                private void Update()
                {
                    var _activity = this.Activity;
                    foreach (var _aspect in this.m_Aspectization)
                    {
                        _activity = _activity.Incorporate(_aspect);
                        this.m_Dictionary[_aspect] = _activity;
                    }
                    this.Setup(_activity.Pointer);
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