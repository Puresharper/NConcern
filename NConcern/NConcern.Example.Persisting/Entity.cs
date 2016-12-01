using System;

namespace NConcern.Example.Persisting
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public sealed class Entity : Attribute
    {
    }
}
