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
    internal static class FilterStringParser
    {
        private static int typeIndex = 0;
        private static int propertyIndex = 1;
        private static int operationIndex = 2;
        private static int valueIndex = 3;

        private static int numberOfComponents = 4;
            //typeof(FilterStringParser)
            //.GetProperties(System.Reflection.BindingFlags.Static)
            //.Where(x => x.PropertyType == typeof(int))
            //.Count();

        public static FilterRule<TarEntry>[] Parse<TEntity>(string input)
            where TEntity : class
        {
            string[] filters = input.Split('/');
            FilterRule<TEntity>[] output = new FilterRule<TEntity>[filters.Length];
            for (int i = 0; i < output.Length; i++)
            {
                string filter = filters[i];
                string[] components = filter.Split('.');

                CheckIfAllComponentsArePresent(components);

                string typeString = components[0];
                string propertyString = components[1];
                string operationString = components[2];
                string valueString = components[3];

                Type entityType = typeof(TEntity);
                string[] propertiesOfEntity = entityType.GetProperties().Select(x => x.Name).ToArray();
                if (!propertiesOfEntity.Contains(propertyString))
                {
                    throw new ParsingFiltersFailedException(propertyString, $"Entity {entityType.Name} does not contain property with such name");
                }
                if (!Types.TryGetValue(typeString, out Type type))
                {
                    throw new ParsingFiltersFailedException(propertyString, $"Wrong type declaration: {typeString}");
                }
                if (!Operations.TryGetValue(operationString, out ExpressionType operation))
                {
                    throw new ParsingFiltersFailedException(propertyString, $"Wrong operation declaration: {operationString}");
                }
                object value = ConvertValueIfValid(valueString, type);
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

        private static void CheckIfAllComponentsArePresent(string[] components)
        {
            if (components is null 
                || components.Count() != numberOfComponents 
                || components.Any(x => string.IsNullOrEmpty(x)))
            {
                throw new ParsingFiltersFailedException("Not enough components in filter");
            }
        }
        private static object ConvertValueIfValid(string value, Type destinationType)
        {
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
