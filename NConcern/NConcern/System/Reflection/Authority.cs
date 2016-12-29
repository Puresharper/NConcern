using System;
using System.Reflection.Emit;

namespace System.Reflection
{
    internal sealed class Authority
    {
        private delegate void Resolve(int token, out IntPtr type, out IntPtr method, out IntPtr field);
        private delegate Type GetTypeFromHandleUnsafe(IntPtr handle);

        private readonly MethodInfo method;
        private readonly Resolve tokenResolver;
        private readonly Func<int, string> stringResolver;
        private readonly Func<int, int, byte[]> signatureResolver;
        private readonly GetTypeFromHandleUnsafe getTypeFromHandleUnsafe;
        private readonly MethodInfo getMethodBase;
        private readonly ConstructorInfo runtimeMethodHandleInternalCtor;
        private readonly ConstructorInfo runtimeFieldHandleStubCtor;
        private readonly MethodInfo getFieldInfo;

        public Authority(MethodInfo method)
        {
            this.method = method;
            if (method is DynamicMethod)
            {
                var resolver = typeof(DynamicMethod).GetField("m_resolver", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(method);
                if (resolver == null) throw new ArgumentException("The dynamic method's IL has not been finalized.");

                tokenResolver = Delegate.CreateDelegate(typeof(Resolve), resolver, resolver.GetType().GetMethod("ResolveToken", BindingFlags.Instance | BindingFlags.NonPublic)) as Resolve;
                stringResolver = Delegate.CreateDelegate(typeof(Func<int, string>), resolver, resolver.GetType().GetMethod("GetStringLiteral", BindingFlags.Instance | BindingFlags.NonPublic)) as Func<int, string>;
                signatureResolver = Delegate.CreateDelegate(typeof(Func<int, int, byte[]>), resolver, resolver.GetType().GetMethod("ResolveSignature", BindingFlags.Instance | BindingFlags.NonPublic)) as Func<int, int, byte[]>;

                getTypeFromHandleUnsafe = Delegate.CreateDelegate(typeof(GetTypeFromHandleUnsafe), typeof(Type).GetMethod("GetTypeFromHandleUnsafe", BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(IntPtr) }, null)) as GetTypeFromHandleUnsafe;
                var runtimeType = typeof(RuntimeTypeHandle).Assembly.GetType("System.RuntimeType");

                var runtimeMethodHandleInternal = typeof(RuntimeTypeHandle).Assembly.GetType("System.RuntimeMethodHandleInternal");
                getMethodBase = runtimeType.GetMethod("GetMethodBase", BindingFlags.Static | BindingFlags.NonPublic, null, new[] { runtimeType, runtimeMethodHandleInternal }, null);
                runtimeMethodHandleInternalCtor = runtimeMethodHandleInternal.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(IntPtr) }, null);

                var runtimeFieldInfoStub = typeof(RuntimeTypeHandle).Assembly.GetType("System.RuntimeFieldInfoStub");
                runtimeFieldHandleStubCtor = runtimeFieldInfoStub.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(IntPtr), typeof(object) }, null);
                getFieldInfo = runtimeType.GetMethod("GetFieldInfo", BindingFlags.Static | BindingFlags.NonPublic, null, new[] { runtimeType, typeof(RuntimeTypeHandle).Assembly.GetType("System.IRuntimeFieldInfo") }, null);
            }
            else
            {

            }
        }

        public Type Type(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            if (method is DynamicMethod)
            {
                IntPtr typeHandle, methodHandle, fieldHandle;
                tokenResolver.Invoke(metadataToken, out typeHandle, out methodHandle, out fieldHandle);

                return getTypeFromHandleUnsafe.Invoke(typeHandle);
            }
            return method.DeclaringType.Module.ResolveType(metadataToken, genericTypeArguments, genericMethodArguments);
        }

        public MethodBase Method(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            if (method is DynamicMethod)
            {
                IntPtr typeHandle, methodHandle, fieldHandle;
                tokenResolver.Invoke(metadataToken, out typeHandle, out methodHandle, out fieldHandle);

                return (MethodBase)getMethodBase.Invoke(null, new[]
        {
            typeHandle == IntPtr.Zero ? null : getTypeFromHandleUnsafe.Invoke(typeHandle),
            runtimeMethodHandleInternalCtor.Invoke(new object[] { methodHandle })
        });
            }
            return method.DeclaringType.Module.ResolveMethod(metadataToken, genericTypeArguments, genericMethodArguments);
        }

        public FieldInfo Field(int identity, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            if (method is DynamicMethod)
            {
                IntPtr typeHandle, methodHandle, fieldHandle;
                tokenResolver.Invoke(identity, out typeHandle, out methodHandle, out fieldHandle);

                return (FieldInfo)getFieldInfo.Invoke(null, new[]
        {
            typeHandle == IntPtr.Zero ? null : getTypeFromHandleUnsafe.Invoke(typeHandle),
            runtimeFieldHandleStubCtor.Invoke(new object[] { fieldHandle, null })
        });
            }
            return method.DeclaringType.Module.ResolveField(identity, genericTypeArguments, genericMethodArguments);
        }

        public MemberInfo Member(int identity, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            if (method is DynamicMethod)
            {
                IntPtr typeHandle, methodHandle, fieldHandle;
                tokenResolver.Invoke(identity, out typeHandle, out methodHandle, out fieldHandle);

                if (methodHandle != IntPtr.Zero)
                {
                    return (MethodBase)getMethodBase.Invoke(null, new[]
            {
                typeHandle == IntPtr.Zero ? null : getTypeFromHandleUnsafe.Invoke(typeHandle),
                runtimeMethodHandleInternalCtor.Invoke(new object[] { methodHandle })
            });
                }

                if (fieldHandle != IntPtr.Zero)
                {
                    return (FieldInfo)getFieldInfo.Invoke(null, new[]
            {
                typeHandle == IntPtr.Zero ? null : getTypeFromHandleUnsafe.Invoke(typeHandle),
                runtimeFieldHandleStubCtor.Invoke(new object[] { fieldHandle, null })
            });
                }

                if (typeHandle != IntPtr.Zero)
                {
                    return getTypeFromHandleUnsafe.Invoke(typeHandle);
                }

                throw new NotImplementedException("DynamicMethods are not able to reference members by token other than types, methods and fields.");
            }
            return method.DeclaringType.Module.ResolveMember(identity, genericTypeArguments, genericMethodArguments);
        }

        public byte[] Signature(int identity)
        {
            if (method is DynamicMethod)
            {
                return signatureResolver.Invoke(identity, 0);
            }
            return method.DeclaringType.Module.ResolveSignature(identity);
        }

        public string String(int identity)
        {
            if (method is DynamicMethod)
            {
                return stringResolver.Invoke(identity);
            }
            return method.DeclaringType.Module.ResolveString(identity);
        }
    }
}
