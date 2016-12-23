using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NConcern.Qualification.Basic
{
    [TestClass]
    public class Before
    {
        private class Action : IAspect
        {
            public IEnumerable<IAdvice> Advise(MethodInfo method)
            {
                yield return Advice.Basic.Before(() =>
                {
                    Interception.Done = true;
                });
            }
        }

        private class ParameterizedAction : IAspect
        {
            public IEnumerable<IAdvice> Advise(MethodInfo method)
            {
                yield return Advice.Basic.Before((_Instance, _Arguments) =>
                {
                    Interception.Done = true;
                    Interception.Instance = _Instance;
                    Interception.Arguments = _Arguments;
                });
            }
        }

        [TestMethod]
        public void StaticMethodWithActionAdvice()
        {
            Interception.Initialize();
            var _method = Metadata.Method(() => Calculator.Multiply(Argument<int>.Value, Argument<int>.Value));
            Aspect.Weave<Before.Action>(_method);
            var _return = Calculator.Multiply(2, 3);
            Assert.AreEqual(_return, 6);
            Assert.AreEqual(Interception.Done, true);
            Aspect.Release<Before.Action>(_method);
        }

        [TestMethod]
        public void StaticMethodWithParameterizedActionAdvice()
        {
            Interception.Initialize();
            var _method = Metadata.Method(() => Calculator.Multiply(Argument<int>.Value, Argument<int>.Value));
            Aspect.Weave<Before.ParameterizedAction>(_method);
            var _return = Calculator.Multiply(2, 3);
            Assert.AreEqual(_return, 6);
            Assert.AreEqual(Interception.Done, true);
            Assert.AreEqual(Interception.Instance, null);
            Assert.AreEqual(Interception.Arguments.Length, 2);
            Assert.AreEqual(Interception.Arguments[0], 2);
            Assert.AreEqual(Interception.Arguments[1], 3);
            Aspect.Release<Before.Action>(_method);
        }

        [TestMethod]
        public void InstanceMethodWithActionAdvice()
        {
            Interception.Initialize();
        }

        [TestMethod]
        public void InstanceMethodWithParameterizedActionAdvice()
        {
            Interception.Initialize();
        }

        [TestMethod]
        public void VirtualMethodWithActionAdvice()
        {
            Interception.Initialize();
        }

        [TestMethod]
        public void VirtualMethodWithParameterizedActionAdvice()
        {
            Interception.Initialize();
        }

        [TestMethod]
        public void OverridenMethodWithActionAdvice()
        {
            Interception.Initialize();
        }

        [TestMethod]
        public void OverridenMethodWithParameterizedActionAdvice()
        {
            Interception.Initialize();
        }

        [TestMethod]
        public void SealedOverridenMethodWithActionAdvice()
        {
            Interception.Initialize();
        }

        [TestMethod]
        public void SealedOverridenMethodWithParameterizedActionAdvice()
        {
            Interception.Initialize();
        }
    }
}
