
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Application.Extensions
{
    public class PagedList<T> where T : class
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
            Items = items;
        }

    }
    public static class PagingSortingBuilder
    {
        public static async Task<PagedList<T>> 
            ToPagedAsync<T>(this IQueryable<T> src, int skip, int take, string orderBy = "") 
            where T : class
        {
            var queryExpression = src.Expression;
            queryExpression = queryExpression.OrderBy(orderBy);

            if (queryExpression.CanReduce)
                queryExpression = queryExpression.Reduce();

            src = src.Provider.CreateQuery<T>(queryExpression);
            int totalCount = await src.CountAsync();
            var items = await src.Skip((skip - 1) * take).Take(take).ToListAsync();
            var results = new PagedList<T>(items, totalCount, skip, take);
            return results;
        }

        private static Expression OrderBy(this Expression source, string orderBy)
        {
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                var orderBys = orderBy.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < orderBys.Length; i++)
                {
                    source = AddOrderBy(source, orderBys[i], i);
                }
            }

            return source;
        }

        private static Expression AddOrderBy(Expression source, string orderBy, int index)
        {
            var orderByParams = orderBy.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string orderByMethodName = index == 0 ? "OrderBy" : "ThenBy";
            string parameterPath = orderByParams[0];
            if (orderByParams.Length > 1 && orderByParams[1].Equals("desc", StringComparison.OrdinalIgnoreCase))
                orderByMethodName += "Descending";

            var sourceType = source.Type.GetGenericArguments().First();
            var parameterExpression = Expression.Parameter(sourceType, "p");
            var orderByExpression = BuildPropertyPathExpression(parameterExpression, parameterPath);
            var orderByFuncType = typeof(Func<,>).MakeGenericType(sourceType, orderByExpression.Type);
            var orderByLambda = Expression.Lambda(orderByFuncType, orderByExpression, new ParameterExpression[] { parameterExpression });

            source = Expression.Call(typeof(Queryable), orderByMethodName, new Type[] { sourceType, orderByExpression.Type }, source, orderByLambda);
            return source;
        }

        private static Expression BuildPropertyPathExpression(this Expression rootExpression, string propertyPath)
        {
            var parts = propertyPath.Split(new[] { '.' }, 2);
            var currentProperty = parts[0];
            var propertyDescription = rootExpression.Type.GetProperty(currentProperty, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (propertyDescription == null)
                throw new KeyNotFoundException($"Cannot find property {rootExpression.Type.Name}.{currentProperty}. The root expression is {rootExpression} and the full path would be {propertyPath}.");

            var propExpr = Expression.Property(rootExpression, propertyDescription);
            if (parts.Length > 1)
                return BuildPropertyPathExpression(propExpr, parts[1]);

            return propExpr;
        }

    }
}
