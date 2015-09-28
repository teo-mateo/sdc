using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SDC.Library.Extensions
{
    public static class IQueryableExtensions
    {
        /// <summary>
        /// Orders the IQueryable ascending by the given property (as string)
        /// see http://stackoverflow.com/questions/307512/how-do-i-apply-orderby-on-an-iqueryable-using-a-string-column-name-within-a-gene
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="ordering"></param>
        /// <param name="values"></param>
        /// <returns>Ordered IQueryable<typeparamref name="T"/></returns>
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string orderByProperty, params object[] values)
        {
            return source.OrderByAnyDirection(orderByProperty, "OrderBy", values);
        }

        /// <summary>
        /// Orders the IQueryable descending by the given property (as string)
        /// see http://stackoverflow.com/questions/307512/how-do-i-apply-orderby-on-an-iqueryable-using-a-string-column-name-within-a-gene
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="orderByProperty">will order descending by this property</param>
        /// <param name="values">???</param>
        /// <returns>Ordered IQueryable<typeparamref name="T"/></returns>
        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string orderByProperty, params object[] values)
        {
            return source.OrderByAnyDirection(orderByProperty, "OrderByDescending", values);
        }


        /// <summary>
        /// Orders the given IQueryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="orderByProperty">Parent.Child1.Child2</param>
        /// <param name="direction">OrderBy/OrderByDescending</param>
        /// <param name="values">??? unused, remove.</param>
        /// <returns>Ordered IQueryable<typeparamref name="T"/></returns>
        public static IQueryable<T> OrderByAnyDirection<T>(this IQueryable<T> source, string orderByProperty, string direction, params object[] values)
        {
            var type = typeof(T);

            if (orderByProperty.Contains("."))
            {
                var allProperties = orderByProperty.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                var property = type.GetProperty(allProperties[0]);
                var parameter = Expression.Parameter(type, "p");
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);

                for (int i = 1; i < allProperties.Length; i++)
                {
                    property = property.PropertyType.GetProperty(allProperties[i]);
                    propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
                }

                //todo: move out.
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                var resultExp = Expression.Call(
                        typeof(Queryable),
                        direction,
                        new Type[] { type, property.PropertyType },
                        source.Expression,
                        Expression.Quote(orderByExp));
                return source.Provider.CreateQuery<T>(resultExp);

            }
            else
            {
                var property = type.GetProperty(orderByProperty);
                var parameter = Expression.Parameter(type, "p");
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                var resultExp = Expression.Call(
                        typeof(Queryable),
                        direction,
                        new Type[] { type, property.PropertyType },
                        source.Expression,
                        Expression.Quote(orderByExp));
                return source.Provider.CreateQuery<T>(resultExp);
            }


        }
    }
}
