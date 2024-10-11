using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Erray.EntitiesFiltering
{
    internal class FilterStringParser<TEntity>
        where TEntity : class
    {
        private static int propertyIndex = 0;
        private static int operationIndex = 1;
        private static int valueIndex = 2;

        private PropertyInfo[]? propertyInfos;

        public FilterRule<TEntity>[] Parse(string input)
        {
            Type entityType = typeof(TEntity);
            propertyInfos = entityType.GetProperties();
            string[] filters = input.Split('/');
            FilterRule<TEntity>[] output = new FilterRule<TEntity>[filters.Length];
            for (int i = 0; i < output.Length; i++)
            {
                string filter = filters[i];
                string[] components = filter.Split('.');

                string propertyString = components[propertyIndex];
                string operationString = components[operationIndex];
                string valueString = components[valueIndex];

                if (!propertyInfos.Any(x => x.Name == propertyString))
                {
                    throw new ParsingFiltersFailedException(propertyString, $"Entity {entityType.Name} does not contain property with such name");
                }
                if (!Operations.TryGetValue(operationString, out ExpressionType operation))
                {
                    throw new ParsingFiltersFailedException(propertyString, $"Wrong operation declaration: {operationString}");
                }
                object value = ConvertValueIfValid(valueString, propertyString);
                FilterRule<TEntity> rule = new FilterRule<TEntity>()
                {
                    Operation = operation,
                    PropertyName = propertyString,
                    Value = value
                };
                output[i] = rule;

            }
            return output;
        }

        private object ConvertValueIfValid(string value, string propertyName)
        {
            var destinationType = propertyInfos
                .Single(x => x.Name == propertyName)
                .PropertyType;
            object converter = typeof(Converter<,>)
                .MakeGenericType(typeof(string), destinationType);
            MethodInfo convertMethod = converter.GetType()
                .GetMethod("Convert")!;
            try
            {
                object convertedValue = convertMethod.Invoke(converter, [value])!;
                return convertedValue;
            }
            catch (Exception ex) // Какое имя??
            {
                throw new ParsingFiltersFailedException(ex.Message);
            }

        }
        private static readonly Dictionary<string, ExpressionType> Operations = new()
        {
            { ">=", ExpressionType.GreaterThanOrEqual },
            {"<=", ExpressionType.LessThanOrEqual },
            {">", ExpressionType.GreaterThan },
            {"<", ExpressionType.LessThan },
            {"==", ExpressionType.Equal },
            {"!=", ExpressionType.NotEqual }
        };

        private static readonly Dictionary<string, Type> Types = new()
        {
            {"int", typeof(int) },
            {"long", typeof(long) },
            {"float", typeof(float) },
            {"double", typeof(double) },
            {"bool", typeof(bool) },
            {"string", typeof(string) },
            {"money", typeof(decimal) }
        };
    }
}
