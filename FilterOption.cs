
using System.Linq.Expressions;

namespace Erray.EntitiesFiltering
{
    public partial class FilterRule<TEntity>
        where TEntity : class
    {
        public object Value { get; set; }
        public string PropertyName { get; set; }
        public ExpressionType Operation { get; set; }
        public static FilterRule<TEntity>[]? FromString(string? filterString)
        {
            if (string.IsNullOrEmpty(filterString)) return null;
            return new FilterStringParser<TEntity>().Parse(filterString);
        }
    }
}
