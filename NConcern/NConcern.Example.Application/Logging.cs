using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;

namespace NConcern.Example.Application
{
    static public class Logging
    {
        static public bool Managed(MethodInfo method)
        {
            var _type = method.DeclaringType;
            if (_type.IsInterface) { return false; }
            foreach (var _interface in _type.GetInterfaces())
            {
                if (Attribute.IsDefined(_interface, typeof(ServiceContractAttribute)))
                {
                    var _map = _type.GetInterfaceMap(_interface);
                    for (var _index = 0; _index < _map.InterfaceMethods.Length; _index++)
                    {
                        if (Attribute.IsDefined(_map.InterfaceMethods[_index], typeof(OperationContractAttribute)))
                        {
                            if (_map.TargetMethods[_index] == method)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
