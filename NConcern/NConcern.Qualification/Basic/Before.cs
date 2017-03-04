using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace NConcern.Qualification.Basic
{
    [TestClass]
    public class Before
    {
        private class Interceptor : IAspect
        {
            public IEnumerable<IAdvice> Advise(MethodBase method)
            {
                yield return Advice.Basic.Before(() =>
                {
                    Interception.Sequence();
                    Interception.Done = true;
                });
            }

            public class Parameterized : IAspect
            {
                public IEnumerable<IAdvice> Advise(MethodBase method)
                {
                    yield return Advice.Basic.Before((_Instance, _Arguments) =>
                    {
                        Interception.Sequence();
                        Interception.Done = true;
                        Interception.Instance = _Instance;
                        Interception.Arguments = _Arguments;
                    });
                }
            }
        }

        [TestMethod]
        public void BasicBeforeStaticMethod()
        {
            lock (Interception.Handle)
            {
                var _method = Metadata.Method(() => Static.Method(Argument<int>.Value, Argument<int>.Value));
                Interception.Initialize();
                Static.Method(2, 3);
                Assert.AreEqual(Interception.Done, false);
                Aspect.Weave<Before.Interceptor>(_method);
                Interception.Initialize();
                var _return = Static.Method(2, 3);
                Assert.AreEqual(_return, 1);
                Assert.AreEqual(Interception.Done, true);
                Aspect.Release<Before.Interceptor>(_method);
                Interception.Initialize();
                Static.Method(2, 3);
                Assert.AreEqual(Interception.Done, false);
            }
        }

        [TestMethod]
        public void BasicBeforeStaticMethodStateful()
        {
            lock (Interception.Handle)
            {
                var _method = Metadata.Method(() => Static.Method(Argument<int>.Value, Argument<int>.Value));
                Interception.Initialize();
                Static.Method(2, 3);
                Assert.AreEqual(Interception.Done, false);
                Aspect.Weave<Before.Interceptor.Parameterized>(_method);
                Interception.Initialize();
                var _return = Static.Method(2, 3);
                Assert.AreEqual(_return, 1);
                Assert.AreEqual(Interception.Done, true);
                Assert.AreEqual(Interception.Instance, null);
                Assert.AreEqual(Interception.Arguments.Length, 2);
                Assert.AreEqual(Interception.Arguments[0], 2);
                Assert.AreEqual(Interception.Arguments[1], 3);
                Aspect.Release<Before.Interceptor.Parameterized>(_method);
                Interception.Initialize();
                Static.Method(2, 3);
                Assert.AreEqual(Interception.Done, false);
            }
        }

        [TestMethod]
        public void BasicBeforeSealedMethod()
        {
            lock (Interception.Handle)
            {
                var _method = Metadata<Sealed>.Method(_Instance => _Instance.Method(Argument<int>.Value, Argument<int>.Value));
                var _instance = new Sealed();
                Interception.Initialize();
                _instance.Method(2, 3);
                Assert.AreEqual(Interception.Done, false);
                Aspect.Weave<Before.Interceptor>(_method);
                Interception.Initialize();
                var _return = _instance.Method(2, 3);
                Assert.AreEqual(_return, 1);
                Assert.AreEqual(Interception.Done, true);
                Aspect.Release<Before.Interceptor>(_method);
                Interception.Initialize();
                _instance.Method(2, 3);
                Assert.AreEqual(Interception.Done, false);
            }
        }

        [TestMethod]
        public void BasicBeforeSealedMethodStateful()
        {
            lock (Interception.Handle)
            {
                var _method = Metadata<Sealed>.Method(_Instance => _Instance.Method(Argument<int>.Value, Argument<int>.Value));
                var _instance = new Sealed();
                Interception.Initialize();
                _instance.Method(2, 3);
                Assert.AreEqual(Interception.Done, false);
                Aspect.Weave<Before.Interceptor.Parameterized>(_method);
                Interception.Initialize();
                var _return = _instance.Method(2, 3);
                Assert.AreEqual(_return, 1);
                Assert.AreEqual(Interception.Done, true);
                Assert.AreEqual(Interception.Instance, _instance);
                Assert.AreEqual(Interception.Arguments.Length, 2);
                Assert.AreEqual(Interception.Arguments[0], 2);
                Assert.AreEqual(Interception.Arguments[1], 3);
                Aspect.Release<Before.Interceptor.Parameterized>(_method);
                Interception.Initialize();
                _instance.Method(2, 3);
                Assert.AreEqual(Interception.Done, false);
            }
        }

        [TestMethod]
        public void BasicBeforeVirtualMethod()
        {
            lock (Interception.Handle)
            {
                var _method = Metadata<Virtual>.Method(_Instance => _Instance.Method(Argument<int>.Value, Argument<int>.Value));
                var _instance = new Virtual();
                Interception.Initialize();
                _instance.Method(2, 3);
                Assert.AreEqual(Interception.Done, false);
                Aspect.Weave<Before.Interceptor>(_method);
                Interception.Initialize();
                var _return = _instance.Method(2, 3);
                Assert.AreEqual(_return, 1);
                Assert.AreEqual(Interception.Done, true);
                Aspect.Release<Before.Interceptor>(_method);
                Interception.Initialize();
                _instance.Method(2, 3);
                Assert.AreEqual(Interception.Done, false);
            }
        }

        [TestMethod]
        public void BasicBeforeVirtualMethodStateful()
        {
            lock (Interception.Handle)
            {
                var _method = Metadata<Virtual>.Method(_Virtual => _Virtual.Method(Argument<int>.Value, Argument<int>.Value));
                var _instance = new Virtual();
                Interception.Initialize();
                _instance.Method(2, 3);
                Assert.AreEqual(Interception.Done, false);
                Aspect.Weave<Before.Interceptor.Parameterized>(_method);
                Interception.Initialize();
                var _return = _instance.Method(2, 3);
                Assert.AreEqual(_return, 1);
                Assert.AreEqual(Interception.Done, true);
                Assert.AreEqual(Interception.Instance, _instance);
                Assert.AreEqual(Interception.Arguments.Length, 2);
                Assert.AreEqual(Interception.Arguments[0], 2);
                Assert.AreEqual(Interception.Arguments[1], 3);
                Aspect.Release<Before.Interceptor.Parameterized>(_method);
                Interception.Initialize();
                _instance.Method(2, 3);
                Assert.AreEqual(Interception.Done, false);
            }
        }

        [TestMethod]
        public void BasicBeforeOverridenMethod()
        {
            lock (Interception.Handle)
            {
                var _method = Metadata<Overriden>.Method(_Overriden => _Overriden.Method(Argument<int>.Value, Argument<int>.Value));
                var _instance = new Overriden();
                Interception.Initialize();
                _instance.Method(2, 3);
                Assert.AreEqual(Interception.Done, false);
                Aspect.Weave<Before.Interceptor>(_method);
                Interception.Initialize();
                var _return = _instance.Method(2, 3);
                Assert.AreEqual(_return, 1);
                Assert.AreEqual(Interception.Done, true);
                Aspect.Release<Before.Interceptor>(_method);
                Interception.Initialize();
                _instance.Method(2, 3);
                Assert.AreEqual(Interception.Done, false);
            }
        }

        [TestMethod]
        public void BasicBeforeOverridenMethodStateful()
        {
            lock (Interception.Handle)
            {
                var _method = Metadata<Overriden>.Method(_Overriden => _Overriden.Method(Argument<int>.Value, Argument<int>.Value));
                var _instance = new Overriden();
                Interception.Initialize();
                _instance.Method(2, 3);
                Assert.AreEqual(Interception.Done, false);
                Aspect.Weave<Before.Interceptor.Parameterized>(_method);
                Interception.Initialize();
                var _return = _instance.Method(2, 3);
                Assert.AreEqual(_return, 1);
                Assert.AreEqual(Interception.Done, true);
                Assert.AreEqual(Interception.Instance, _instance);
                Assert.AreEqual(Interception.Arguments.Length, 2);
                Assert.AreEqual(Interception.Arguments[0], 2);
                Assert.AreEqual(Interception.Arguments[1], 3);
                Aspect.Release<Before.Interceptor.Parameterized>(_method);
                Interception.Initialize();
                _instance.Method(2, 3);
                Assert.AreEqual(Interception.Done, false);
            }
        }
    }
}
