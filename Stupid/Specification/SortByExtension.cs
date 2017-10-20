using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Stupid.Specification
{
    /// <summary>
    /// 表示基于Entity Framework的排序扩展类型。该扩展解决了在Entity Framework上针对某些
    /// 原始数据类型执行排序操作时，出现Expression of type A cannot be used for return type B
    /// 错误的问题。
    /// </summary>
    /// <remarks>有关该功能扩展的更多信息，请参考：http://www.cnblogs.com/daxnet/archive/2012/07/23/2605695.html。</remarks>
    internal static class SortByExtension
    {
        internal static IOrderedQueryable<TAggregateRoot> SortBy<TAggregateRoot>(this IQueryable<TAggregateRoot> query, Expression<Func<TAggregateRoot, dynamic>> sortPredicate)
               where TAggregateRoot : class
        {
            return InvokeSortBy(query, sortPredicate, SortOrder.Ascending);
        }



        internal static IOrderedQueryable<TAggregateRoot> SortByDescending<TAggregateRoot>(this IQueryable<TAggregateRoot> query, Expression<Func<TAggregateRoot, dynamic>> sorPredicate)
            where TAggregateRoot : class
        {
            return InvokeSortBy(query, sorPredicate, SortOrder.Descending);
        }
        private static IOrderedQueryable<TAggregateRoot> InvokeSortBy<TAggregateRoot>(IQueryable<TAggregateRoot> query,
            Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder)
            where TAggregateRoot : class
        {
            var param = sortPredicate.Parameters[0];
            string propertyName = null;
            Type propertyType = null;
            Expression bodyExpression = null;

            //判断Body是否是运算符表达式
            if (sortPredicate.Body is UnaryExpression)
            {
                var unaryExpression = sortPredicate.Body as UnaryExpression;
                bodyExpression = unaryExpression;

                // Extend by Evan.
                // 在传入方法调用类(非调用对象属性)排序方式时候,强制转换为MemberExpression会抛异常.
                if (bodyExpression is MethodCallExpression)
                {
                    var _methodexp = bodyExpression as MethodCallExpression;
                    Type t = _methodexp.Method.ReturnType;

                    var _expression = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(TAggregateRoot), t), _methodexp, param);

                    object result = typeof(Queryable).GetMethods().Single(
                        method => method.Name == GetSortingMethodName(sortOrder)
                        && method.IsGenericMethodDefinition
                        && method.GetGenericArguments().Length == 2
                        && method.GetParameters().Length == 2)
                        .MakeGenericMethod(typeof(TAggregateRoot), t)
                        .Invoke(null, new object[] { query, _expression });

                    return (IOrderedQueryable<TAggregateRoot>)result;
                }
            }
            else if (sortPredicate.Body is MemberExpression)
                bodyExpression = sortPredicate.Body;
            else
                throw new ArgumentException(@"The body of the sort predicate expression should be 
                either UnaryExpression or MemberExpression.", "sortPredicate");

            LambdaExpression convertedExpression;

            MemberExpression memberExpression = (MemberExpression)bodyExpression;
            propertyName = memberExpression.Member.Name;
            if (memberExpression.Member.MemberType == MemberTypes.Property)
            {
                PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
                propertyType = propertyInfo.PropertyType;
            }
            else
                throw new InvalidOperationException(@"Cannot evaluate the type of property since the member expression 
            represented by the sort predicate expression does not contain a PropertyInfo object.");

            Type funcType = typeof(Func<,>).MakeGenericType(typeof(TAggregateRoot), propertyType);
            convertedExpression = Expression.Lambda(funcType, Expression.Convert(Expression.Property(param, propertyName), propertyType), param);

            var sortingMethods = typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static);
            var sortingMethodName = GetSortingMethodName(sortOrder);
            var sortingMethod = sortingMethods.First(sm => sm.Name == sortingMethodName && sm.GetParameters().Length == 2);

            return (IOrderedQueryable<TAggregateRoot>)sortingMethod
                .MakeGenericMethod(typeof(TAggregateRoot), propertyType)
                .Invoke(null, new object[] { query, convertedExpression });
        }

        private static string GetSortingMethodName(SortOrder sortOrder)
        {
            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    return "OrderBy";

                case SortOrder.Descending:
                    return "OrderByDescending";

                default:
                    throw new ArgumentException("Sort Order must be specified as either Ascending or Descending.",
"sortOrder");
            }
        }
    }
}
