using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq.Expressions;
using System.IO;

using ZondervanLibrary.SharedLibrary.Parsing.Records;
using ZondervanLibrary.SharedLibrary.Parsing.Fields;
using ZondervanLibrary.SharedLibrary.Parsing.Conversion;

namespace ZondervanLibrary.SharedLibrary.Parsing.Parsers
{
    public abstract class StreamParserBase<TRecord> : IStreamParser<TRecord>
        where TRecord : new()
    {
        protected readonly IRecordAttribute _recordAttribute;

        public StreamParserBase()
        {
            IEnumerable<IRecordAttribute> attributes = typeof(TRecord).GetCustomAttributes().Where(a => typeof(IRecordAttribute).IsAssignableFrom(a.GetType())).Cast<IRecordAttribute>();

            if (attributes.Count() == 0)
            {
                throw new ArgumentException("TRecord must have a record attribute in order to create a stream parser.");
            }
            else if (attributes.Count() > 1)
            {
                throw new ArgumentException("TRecord cannot have multiple record attributes.");
            }

            _recordAttribute = attributes.First();
        }

        public abstract IEnumerable<TRecord> ParseStream(Stream stream);

        public abstract void WriteStream(Stream stream, IEnumerable<TRecord> items);

        protected IEnumerable<Expression> CreateFieldExpression(PropertyInfo propertyInfo, IFieldAttribute fieldAttribute, ParameterExpression returnExpression, Expression valueExpression)
        {
            List<Expression> ret = new List<Expression>();

            Regex nullRegex = (fieldAttribute.NullPattern != null) ? new Regex(fieldAttribute.NullPattern) : new Regex(@"^\s*$");

            Expression isNullMatch = Expression.Call(Expression.Constant(nullRegex), typeof(Regex).GetMethod("IsMatch", new[] { typeof(String) }), valueExpression);
            Expression assignedExpression;

            if (fieldAttribute.IsRequired)
            {
                ret.Add(Expression.IfThen(isNullMatch, Expression.Throw(Expression.Constant(new Exception($"Field '{propertyInfo.Name}' cannot be null")))));
            }

            assignedExpression = valueExpression;

            List<Expression> temp = new List<Expression>();

            if (fieldAttribute.ValidationPattern != null)
            {
                Regex validationRegex = new Regex(fieldAttribute.ValidationPattern);

                Expression isValidationMatch = Expression.Call(Expression.Constant(validationRegex), typeof(Regex).GetMethod("IsMatch", new[] { typeof(String) }), valueExpression);

                Expression s = Expression.Call(typeof(String).GetMethod("Format", new[] { typeof(String), typeof(String) }), Expression.Constant($"Field '{propertyInfo.Name}' does not match the required format.\nValue: {{0}}"), valueExpression);

                Expression exception = Expression.New(typeof(Exception).GetConstructor(new[] { typeof(String) }), s);

                temp.Add(Expression.IfThen(Expression.Not(isValidationMatch), Expression.Throw(exception)));
            }

            IEnumerable<Attribute> conversionAttributes = propertyInfo.GetCustomAttributes().Where(attribute => attribute.GetType().GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConversionAttribute<>)));

            if (conversionAttributes.Count() > 1)
            {
                throw new Exception(String.Format("Property '{0}' cannot contain multiple conversion attributes."));
            }
            else if (conversionAttributes.Count() == 1)
            {
                assignedExpression = Expression.Convert(Expression.Call(Expression.Constant(conversionAttributes.First()), conversionAttributes.First().GetType().GetMethod("Convert", new[] { typeof(String) }), assignedExpression), propertyInfo.PropertyType);
            }
            else if (propertyInfo.PropertyType.IsEnum || (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) && propertyInfo.PropertyType.GetGenericArguments()[0].IsEnum))
            {
                Type enumType = (propertyInfo.PropertyType.IsEnum) ? propertyInfo.PropertyType : propertyInfo.PropertyType.GetGenericArguments()[0];

                assignedExpression = Expression.Call(assignedExpression, typeof(String).GetMethod("Replace", new[] { typeof(String), typeof(String) }), Expression.Constant(" "), Expression.Constant(""));
                assignedExpression = Expression.Call(assignedExpression, typeof(String).GetMethod("Replace", new[] { typeof(String), typeof(String) }), Expression.Constant("_"), Expression.Constant(""));
                assignedExpression = Expression.Call(assignedExpression, typeof(String).GetMethod("Replace", new[] { typeof(String), typeof(String) }), Expression.Constant("&"), Expression.Constant(""));

                assignedExpression = Expression.Convert(Expression.Call(typeof(Enum).GetMethod("Parse", new[] { typeof(Type), typeof(String), typeof(Boolean) }), Expression.Constant(enumType), assignedExpression, Expression.Constant(true)), propertyInfo.PropertyType);
            }
            else if (propertyInfo.PropertyType != typeof(String))
            {
                // Attempt to find a "Parse" method to convert
                MethodInfo parseMethod = propertyInfo.PropertyType.GetMethod("Parse", new[] { typeof(String) });

                if (parseMethod == null)
                {
                    throw new Exception($"No '{propertyInfo.PropertyType}.Parse(String)' method was found to use for conversion.");
                }

                assignedExpression = Expression.Call(parseMethod, assignedExpression);
            }

            temp.Add(Expression.Assign(Expression.Property(returnExpression, propertyInfo), assignedExpression));

            if (fieldAttribute.IsRequired)
            {
                ret.AddRange(temp);
            }
            else
            {
                ret.Add(Expression.IfThenElse(isNullMatch, Expression.Assign(Expression.Property(returnExpression, propertyInfo), Expression.Convert(Expression.Constant(null), propertyInfo.PropertyType)), Expression.Block(temp)));
            }

            return ret;
        }
    }
}
