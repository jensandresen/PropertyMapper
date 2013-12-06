using System;
using System.Linq.Expressions;

namespace PropertyMapper
{
    public interface IConfiguration<TSource, TDestination>
    {
        /// <summary>
        /// Specify a property on the destination instance that should be ignored when coping property values.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="selector">Expression for selecting a property on the destination.</param>
        void Ignore<TValue>(Expression<Func<TDestination, TValue>> selector);
    }
}