using System;

namespace NConcern.Example
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple=false, Inherited=true)]
    public class Requiered : Attribute
    {
    }
}
