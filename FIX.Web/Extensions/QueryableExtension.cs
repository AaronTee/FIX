using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace FIX.Web.Extensions
{
    public static class QueryableExtension
    {
        public static IQueryable<T> ProcessListing<T>(
        this IQueryable<T> source, string propertyName, string order, int offset, int limit, string defaultPropertyName)
        {
            if (propertyName == null) propertyName = defaultPropertyName;

            var param1 = Expression.Parameter(typeof(T), "x");
            Expression parent = param1;
            parent = Expression.Property(parent, propertyName);
            Expression conversion = Expression.Convert(parent, typeof(object));

            ParameterExpression param = Expression.Parameter(typeof(T), "x");

            Expression<Func<T, object>> sortExpression = Expression.Lambda<Func<T, object>>(
                        Expression.Convert(Expression.Property(param, propertyName), typeof(object)), param);

                return source.ApplySettingsWithExpression(sortExpression, order, offset, limit);
        }

        private static IQueryable<T> ApplySettingsWithExpression<T>(this IQueryable<T> source, Expression<Func<T, object>> sortExpression, string order, int offset, int limit)
        {
            var expression = sortExpression.Compile();
            
            if (order == "desc")
            {
                return source.OrderByDescending(x => x).Skip(offset).Take(limit).AsQueryable();
            }
            else
            {
                return source.OrderBy(expression).Skip(offset).Take(limit).AsQueryable();
            }
        }

        public static IQueryable SortBy(this IQueryable queryList, string field)
        {
            return new List<string>() as IQueryable;
        }

        public static IQueryable<TSource> PaginateList<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, string sort, string order, int offset, int limit)
        {
            if (sort.IsNullOrEmpty()) return source.OrderBy(selector).Skip(offset).Take(limit);
            var orderCommand = (order == "desc") ? "OrderByDescending" : "OrderBy";

            return ApplyOrder<TSource>(source, sort, orderCommand).Skip(offset).Take(limit);
        }

        public static IEnumerable<TSource> PaginateList<TSource, TResult>(this IEnumerable<TSource> source, string sort, string order, int offset, int limit, Func<TSource, TResult> selector)
        {
            if (sort.IsNullOrEmpty()) return source.OrderBy(selector).Skip(offset).Take(limit);
            var orderCommand = (order == "desc") ? "OrderByDescending" : "OrderBy";

            return ApplyOrder<TSource>(source, sort, orderCommand).Skip(offset).Take(limit);
        }

        static IOrderedQueryable<T> ApplyOrder<T>(IEnumerable<T> source, string property, string methodName)
        {
            string[] props = property.Split('.');
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (string prop in props)
            {
                PropertyInfo pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            object result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2
                            && method.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), type)
                    .Invoke(null, new object[] { source, lambda });
            return (IOrderedQueryable<T>)result;
        }
    }
}