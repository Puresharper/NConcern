using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NConcern.Example
{
    static internal class Storage
    {
        static private class Partition<T>
            where T : class
        {
            static public readonly IList<T> List = new List<T>();
        }

        static public IQueryable<T> Query<T>()
            where T : class
        {
            return Storage.Partition<T>.List.AsQueryable<T>();
        }

        static public IQueryable<T> Query<T>(Expression<Func<T, bool>> predicate)
            where T : class
        {
            return Storage.Query<T>().Where(predicate);
        }

        static public void Add<T>(T item)
            where T : class
        {
            Storage.Partition<T>.List.Add(item);
        }

        static public void Remove<T>(T item)
            where T : class
        {
            Storage.Partition<T>.List.Remove(item);
        }
    }
}
