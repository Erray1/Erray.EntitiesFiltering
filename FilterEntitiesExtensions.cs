
using System.Linq.Expressions;

namespace Erray.EntitiesFiltering
{
    public static class FilterEntitiesExtensions
    {
        public static IQueryable<T> Filter<T>(this IQueryable<T> query, FilterRule<T>[]? filters)
            where T : class
        {
            if (filters is null || filters.Count() == 0) return query;
            var parameter = Expression.Parameter(typeof(T));
            BinaryExpression binaryExpression = null!;
            foreach (var rule in filters)
            {
                var prop = Expression.Property(parameter, rule.PropertyName);
                var val = Expression.Constant(rule.Value);
                var newBinary = Expression.MakeBinary(rule.Operation, prop, val);

                binaryExpression = binaryExpression is null ? 
                    newBinary :
                    Expression.MakeBinary(ExpressionType.AndAlso, binaryExpression, newBinary);
            }
            var cookedExpression = Expression.Lambda<Func<T, bool>>(binaryExpression, parameter);
            return query.Where(cookedExpression);
        }
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> items, FilterRule<T>[]? filters)
            where T : class
        {
            if (filters is null || filters.Count() == 0) return items;
            var parameter = Expression.Parameter(typeof(T));
            BinaryExpression binaryExpression = null!;
            foreach (var rule in filters)
            {
                var prop = Expression.Property(parameter, rule.PropertyName);
                var val = Expression.Constant(rule.Value);
                var newBinary = Expression.MakeBinary(rule.Operation, prop, val);

                binaryExpression = binaryExpression is null ?
                    newBinary :
                    Expression.MakeBinary(ExpressionType.AndAlso, binaryExpression, newBinary);
            }
            var cookedExpression = Expression.Lambda<Func<T, bool>>(binaryExpression, parameter);
            var lambda = cookedExpression.Compile();
            return items.Where(lambda);
        }
    }
}
