using System;
using System.Linq.Expressions;

namespace PropertyMapper.Tests
{
    public interface IConfiguration<TSource, TDestination>
    {
        void Ignore<TValue>(Expression<Func<TDestination, TValue>> selector);
    }
}