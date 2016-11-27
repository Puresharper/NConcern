using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace NConcern
{
    abstract public partial class Aspect
    {
        private sealed partial class Weaver
        {
            static private readonly ModuleBuilder m_Module = AppDomain.CurrentDomain.DefineDynamicModule();

            private readonly Resource m_Resource;
            private readonly IntPtr m_Pointer;
            public readonly Type Type;
            public readonly MethodInfo Method;
            public readonly Junction Junction;
            private readonly LinkedList<Aspect> m_Typology;
            private readonly Dictionary<Aspect, Junction> m_Dictionary;
            private readonly Action<IntPtr> m_Setup;

            unsafe internal Weaver(Type type, MethodInfo method, IntPtr pointer, Junction junction)
            {
                method.Prepare();
                this.m_Resource = new Resource();
                this.Type = type;
                this.Method = method;
                this.m_Pointer = pointer;
                this.Junction = junction;
                this.m_Typology = new LinkedList<Aspect>();
                this.m_Dictionary = new Dictionary<Aspect, Junction>();
                if (method.IsVirtual) { this.m_Setup = this.Setup; }
                else
                {
                    var _type = Weaver.m_Module.DefineType(string.Concat(Metadata<Delegate>.Type.Name, Guid.NewGuid().ToString("N")), TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Abstract);
                    switch (IntPtr.Size)
                    {
                        case 4: _type.DefineField(Metadata<Weaver>.Field(_Junction => _Junction.m_Pointer).Name, Metadata<int>.Type, FieldAttributes.Static | FieldAttributes.Public); break;
                        case 8: _type.DefineField(Metadata<Weaver>.Field(_Junction => _Junction.m_Pointer).Name, Metadata<long>.Type, FieldAttributes.Static | FieldAttributes.Public); break;
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
                    this.m_Setup(junction.Pointer);
                }
            }

            unsafe private void Setup(IntPtr pointer)
            {
                switch (IntPtr.Size)
                {
                    case 4: *((int*)(this.m_Pointer)) = pointer.ToInt32(); break;
                    case 8: *((long*)(this.m_Pointer)) = pointer.ToInt64(); break;
                    default: throw new NotSupportedException();
                }
            }

            public void Update()
            {
                lock (this.m_Resource)
                {
                    var _junction = this.Junction;
                    foreach (var _aspect in this.m_Typology)
                    {
                        var _advice = _aspect.Advise(_junction.Type, _junction.Method);
                        if (_advice == null) { this.m_Dictionary[_aspect] = _junction; }
                        else
                        {
                            _junction = _advice.Override(_junction) ?? _junction;
                            this.m_Dictionary[_aspect] = _junction;
                        }
                    }
                    this.m_Setup(_junction.Pointer);
                    this.Method.Prepare();
                }
            }

            public void Add(Aspect aspect)
            {
                lock (this.m_Resource)
                {
                    if (this.m_Dictionary.ContainsKey(aspect)) { return; }
                    this.m_Typology.AddFirst(aspect);
                    this.m_Dictionary.Add(aspect, null);
                    this.Update();
                }
            }

            public void Remove(Aspect aspect)
            {
                lock (this.m_Resource)
                {
                    if (this.m_Dictionary.Remove(aspect))
                    {
                        this.m_Typology.Remove(aspect);
                        this.Update();
                    }
                }
            }
        }

        [DebuggerDisplay("{Debugger.Display(this) , nq}")]
        [DebuggerTypeProxy(typeof(Weaver.Debugger))]
        private sealed partial class Weaver
        {
            private class Debugger
            {
                static public string Display(Aspect.Weaver weaver)
                {
                    return string.Concat(weaver.Type.Declaration(), ".", weaver.Method.Declaration(), " = ", weaver.m_Typology.Count.ToString());
                }

                [DebuggerBrowsable(DebuggerBrowsableState.Never)]
                private Aspect.Weaver m_Weaver;

                public Debugger(Aspect.Weaver weaver)
                {
                    this.m_Weaver = weaver;
                }

                [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
                public Aspect[] View
                {
                    get { return this.m_Weaver.m_Typology.ToArray(); }
                }
            }
        }

        static private class Weaver<T>
            where T : class
        {
            static private class Initialization
            {
                unsafe static internal Dictionary<MethodInfo, Weaver> Dictionary()
                {
                    //var _type = Metadata<T>.Type;
                    var _dictionary = new Dictionary<MethodInfo, Weaver>();
                    var _index = 0;
                    switch (IntPtr.Size)
                    {
                        case 4:
                            {
                                var _address = (int*)(*(((int*)Metadata<T>.Type.TypeHandle.Value.ToPointer()) + 10));
                                foreach (var _method in Metadata<T>.Type.Base().Reverse().SelectMany(_Type => _Type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly).Where(_Method => _Method.IsVirtual && _Method == _Method.GetBaseDefinition()).OrderBy(_Method => _Method.MetadataToken)))
                                {
                                    _method.Prepare();
                                    _dictionary.Add(_method, new Weaver(Metadata<T>.Type, _method, new IntPtr(_address + _index), new Junction(_method, _method.ReturnType, _method.Signature())));
                                    _index++;
                                }
                                foreach (var _method in Metadata<T>.Type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).Where(_Method => !_Method.IsVirtual))
                                {
                                    _method.Prepare();
                                    _dictionary.Add(_method, new Weaver(Metadata<T>.Type, _method, new IntPtr((int*)_method.Handle().Value.ToPointer() + 2), new Junction(_method, _method.ReturnType, _method.Signature())));
                                }
                                break;
                            }
                        case 8:
                            {
                                var _address = (long*)(*(((long*)Metadata<T>.Type.TypeHandle.Value.ToPointer()) + 8));
                                foreach (var _method in Metadata<T>.Type.Base().Reverse().SelectMany(_Type => _Type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly).Where(_Method => _Method.IsVirtual && _Method == _Method.GetBaseDefinition()).OrderBy(_Method => _Method.MetadataToken)))
                                {
                                    _method.Prepare();
                                    _dictionary.Add(_method, new Weaver(Metadata<T>.Type, _method, new IntPtr(_address + _index), new Junction(_method, _method.ReturnType, _method.Signature())));
                                    _index++;
                                }
                                foreach (var _method in Metadata<T>.Type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).Where(_Method => !_Method.IsVirtual))
                                {
                                    _method.Prepare();
                                    _dictionary.Add(_method, new Weaver(Metadata<T>.Type, _method, new IntPtr((long*)_method.Handle().Value.ToPointer() + 1), new Junction(_method, _method.ReturnType, _method.Signature())));
                                }
                                break;
                            }
                        default: throw new NotSupportedException();
                    }
                    return _dictionary;
                }
            }

            static public readonly Dictionary<MethodInfo, Weaver> Dictionary = Weaver<T>.Initialization.Dictionary();
        }
    }
}