using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PropertyMapper
{
    public class Configuration<TSource, TDestination> : IConfiguration<TSource, TDestination>
    {
        private readonly List<PropertyInfo> _propertiesToIgnore = new List<PropertyInfo>();

        public void Ignore<TValue>(Expression<Func<TDestination, TValue>> selector)
        {
            var body = GetMemberExpressionFrom(selector);
            var propertyInfo = (PropertyInfo) body.Member;

            _propertiesToIgnore.Add(propertyInfo);
        }

        private static MemberExpression GetMemberExpressionFrom<TProperty, TResult>(Expression<Func<TProperty, TResult>> selector)
        {
            var expression = selector.Body as MemberExpression;
            if (expression != null)
            {
                return expression;
            }

            var unaryExpression = selector.Body as UnaryExpression;
            if (unaryExpression != null)
            {
                return unaryExpression.Operand as MemberExpression;
            }

            return null;
        }

        public bool ShouldBeIgnored(PropertyInfo propertyInfo)
        {
            return _propertiesToIgnore.Any(p => PropertyHelpers.IsMatch(p, propertyInfo));
        }
    }
}