using System;
using System.Linq.Expressions;

namespace PropertyMapper
{
    public interface IConfiguration<TSource, TDestination>
    {
        void Ignore<TValue>(Expression<Func<TDestination, TValue>> selector);
    }
}