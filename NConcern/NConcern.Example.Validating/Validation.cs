using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NConcern.Example.Validating
{
    public class Validation : Aspect
    {
        override protected IEnumerable<Advice> Advise<T>(MethodInfo method)
        {
            var _parameters = method.GetParameters();
            var _requiered = _parameters.Where(_Parameter => Attribute.IsDefined(_Parameter, typeof(Requiered))).Select(_Parameter => _Parameter.Position).ToArray();
            yield return new Before((_Instance, _Arguments) =>
            {
                foreach (var _index in _requiered)
                {
                    if (_Arguments[_index] == null)
                    {
                        throw new ArgumentNullException(_parameters[_index].Name);
                    }
                }
            });
        }
    }
}
