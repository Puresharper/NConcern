using System;
using System.Reflection.Emit;

namespace System.Reflection
{
    internal sealed class Authority
    {
        private delegate void Resolve(int token, out IntPtr type, out IntPtr method, out IntPtr field);
        private delegate Type GetTypeFromHandleUnsafe(IntPtr handle);
        private readonly MethodBase m_Method;
        private readonly Resolve m_TokenResolver;
        private readonly Func<int, string> m_StringResolver;
        private readonly Func<int, int, byte[]> m_SignatureResolver;
        private readonly GetTypeFromHandleUnsafe m_GetTypeFromHandleUnsafe;
        private readonly MethodInfo m_GetMethodBase;
        private readonly ConstructorInfo m_RuntimeMethodHandleInternalCtor;
        private readonly ConstructorInfo m_RuntimeFieldHandleStubCtor;
        private readonly MethodInfo m_GetFieldInfo;

        public Authority(MethodBase method)
        {
            this.m_Method = method;
            if (method is DynamicMethod)
            {
                var _resolver = typeof(DynamicMethod).GetField("m_resolver", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(method);
                if (_resolver == null) throw new ArgumentException("The dynamic method's IL has not been finalized.");
                this.m_TokenResolver = Delegate.CreateDelegate(typeof(Resolve), _resolver, _resolver.GetType().GetMethod("ResolveToken", BindingFlags.Instance | BindingFlags.NonPublic)) as Resolve;
                this.m_StringResolver = Delegate.CreateDelegate(typeof(Func<int, string>), _resolver, _resolver.GetType().GetMethod("GetStringLiteral", BindingFlags.Instance | BindingFlags.NonPublic)) as Func<int, string>;
                this.m_SignatureResolver = Delegate.CreateDelegate(typeof(Func<int, int, byte[]>), _resolver, _resolver.GetType().GetMethod("ResolveSignature", BindingFlags.Instance | BindingFlags.NonPublic)) as Func<int, int, byte[]>;
                this.m_GetTypeFromHandleUnsafe = Delegate.CreateDelegate(typeof(GetTypeFromHandleUnsafe), typeof(Type).GetMethod("GetTypeFromHandleUnsafe", BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(IntPtr) }, null)) as GetTypeFromHandleUnsafe;
                var _type = typeof(RuntimeTypeHandle).Assembly.GetType("System.RuntimeType");
                var _method = typeof(RuntimeTypeHandle).Assembly.GetType("System.RuntimeMethodHandleInternal");
                this.m_GetMethodBase = _type.GetMethod("GetMethodBase", BindingFlags.Static | BindingFlags.NonPublic, null, new[] { _type, _method }, null);
                this.m_RuntimeMethodHandleInternalCtor = _method.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(IntPtr) }, null);
                var _stub = typeof(RuntimeTypeHandle).Assembly.GetType("System.RuntimeFieldInfoStub");
                this.m_RuntimeFieldHandleStubCtor = _stub.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(IntPtr), typeof(object) }, null);
                this.m_GetFieldInfo = _type.GetMethod("GetFieldInfo", BindingFlags.Static | BindingFlags.NonPublic, null, new[] { _type, typeof(RuntimeTypeHandle).Assembly.GetType("System.IRuntimeFieldInfo") }, null);
            }
        }

        public Type Type(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            if (this.m_Method is DynamicMethod)
            {
                IntPtr _type, _method, _field;
                this.m_TokenResolver(metadataToken, out _type, out _method, out _field);
                return this.m_GetTypeFromHandleUnsafe(_type);
            }
            return this.m_Method.DeclaringType.Module.ResolveType(metadataToken, genericTypeArguments, genericMethodArguments);
        }

        public MethodBase Method(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            if (this.m_Method is DynamicMethod)
            {
                IntPtr _type, _method, _field;
                this.m_TokenResolver(metadataToken, out _type, out _method, out _field);
                return (MethodBase)this.m_GetMethodBase.Invoke(null, new[] { _type == IntPtr.Zero ? null : this.m_GetTypeFromHandleUnsafe(_type), this.m_RuntimeMethodHandleInternalCtor.Invoke(new object[] { _method }) });
            }
            return this.m_Method.DeclaringType.Module.ResolveMethod(metadataToken, genericTypeArguments, genericMethodArguments);
        }

        public FieldInfo Field(int identity, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            if (this.m_Method is DynamicMethod)
            {
                IntPtr _type, _method, _field;
                this.m_TokenResolver(identity, out _type, out _method, out _field);
                return (FieldInfo)this.m_GetFieldInfo.Invoke(null, new[] { _type == IntPtr.Zero ? null : this.m_GetTypeFromHandleUnsafe(_type), this.m_RuntimeFieldHandleStubCtor.Invoke(new object[] { _field, null }) });
            }
            return m_Method.DeclaringType.Module.ResolveField(identity, genericTypeArguments, genericMethodArguments);
        }

        public MemberInfo Member(int identity, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            if (this.m_Method is DynamicMethod)
            {
                IntPtr _type, _method, _field;
                this.m_TokenResolver(identity, out _type, out _method, out _field);
                if (_method != IntPtr.Zero) { return (MethodBase)this.m_GetMethodBase.Invoke(null, new[] { _type == IntPtr.Zero ? null : this.m_GetTypeFromHandleUnsafe(_type), this.m_RuntimeMethodHandleInternalCtor.Invoke(new object[] { _method })}); }
                if (_field != IntPtr.Zero) { return (FieldInfo)this.m_GetFieldInfo.Invoke(null, new[] { _type == IntPtr.Zero ? null : this.m_GetTypeFromHandleUnsafe(_type), this.m_RuntimeFieldHandleStubCtor.Invoke(new object[] { _field, null }) }); }
                if (_type != IntPtr.Zero) { return this.m_GetTypeFromHandleUnsafe(_type); }
                throw new NotImplementedException("DynamicMethods are not able to reference members by token other than types, methods and fields.");
            }
            return this.m_Method.DeclaringType.Module.ResolveMember(identity, genericTypeArguments, genericMethodArguments);
        }

        public byte[] Signature(int identity)
        {
            if (this.m_Method is DynamicMethod) { return this.m_SignatureResolver(identity, 0); }
            return m_Method.DeclaringType.Module.ResolveSignature(identity);
        }

        public string String(int identity)
        {
            if (this.m_Method is DynamicMethod) { return this.m_StringResolver(identity); }
            return m_Method.DeclaringType.Module.ResolveString(identity);
        }
    }
}
