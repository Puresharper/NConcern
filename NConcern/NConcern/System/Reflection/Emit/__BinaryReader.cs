using System;
using System.IO;
using System.Linq.Expressions;

namespace System.Reflection.Emit
{
    static internal class __BinaryReader
    {
        static private readonly Func<int, Label> m_ConvertInt32ToLabel = Expression.Lambda<Func<int, Label>>(Expression.New(Metadata<Label>.Type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { Metadata<int>.Type }, null), Parameter<int>.Expression), Parameter<int>.Expression).Compile();

        static internal Label ReadLabel(this BinaryReader reader)
        {
            return __BinaryReader.m_ConvertInt32ToLabel(reader.ReadInt32());
        }

        static internal Label ReadShortLabel(this BinaryReader reader)
        {
            return __BinaryReader.m_ConvertInt32ToLabel(Convert.ToInt32(reader.ReadSByte()));
        }
    }
}
