

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Linq.Expressions;

namespace Erray.EntitiesFiltering
{
    public partial class FilterRule<TEntity>
        where TEntity : class
    {
        public object Value { get; set; }
        public string PropertyName { get; set; }
        public ExpressionType Operation { get; set; }
        public static FilterRule<TEntity>[]? FromQueryString(IQueryCollection query)
        {
            bool containsFilters = query.TryGetValue("filters", out StringValues value);
            if (!containsFilters) return null;
            return FromString(value.ToString());
        }
        public static FilterRule<TEntity>[]? FromString(string filterString)
        {
            return new FilterStringParser<TEntity>().Parse(filterString);
        }
    }
}
