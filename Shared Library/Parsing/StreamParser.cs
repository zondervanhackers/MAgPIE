using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

using ZondervanLibrary.SharedLibrary.Parsing.Conversion;
using ZondervanLibrary.SharedLibrary.Parsing.Records;
using ZondervanLibrary.SharedLibrary.Parsing.Fields;

namespace ZondervanLibrary.SharedLibrary.Parsing
{
    public class StreamParser<TRecord> : IStreamParser<TRecord>
        where TRecord : new()
    {
        private static readonly Func<String, TRecord> _lineParser;
        private static readonly Boolean _ignoreFirstLine;

        static StreamParser()
        {
            // Do this in a static constructor?

            IEnumerable<IRecordAttribute> attributes = typeof(TRecord).GetCustomAttributes().Where(a => typeof(IRecordAttribute).IsAssignableFrom(a.GetType())).Cast<IRecordAttribute>().ToArray();

            if (!attributes.Any())
            {
                throw new ArgumentException("TRecord must have a record attribute in order to create a stream parser.");
            }

            if (attributes.Count() > 1)
            {
                throw new ArgumentException("TRecord cannot have multiple record attributes.");
            }

            IRecordAttribute attribute = attributes.First();

            _ignoreFirstLine = attribute.IgnoreFirstLine;

            _lineParser = CreateLineParser(attribute);
        }

        public IEnumerable<TRecord> ParseStream(Stream stream)
        {
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream))
            {
                String line;
                Int32 lineNumber = 1;

                if (_ignoreFirstLine)
                {
                    reader.ReadLine();
                    lineNumber++;
                }

                string fileName = ((FileStream)reader.BaseStream).Name;

                while ((line = reader.ReadLine()) != null)
                {
                    TRecord temp;
                    try
                    {
                        temp = _lineParser(line);
                    }
                    catch (Exception exception)
                    {
                        throw new ParsingException(lineNumber, exception.Message + " In file: " + fileName + " at line: " + lineNumber, exception);
                    }

                    yield return temp;

                    lineNumber++;
                }
            }
        }

        private static Func<String, TRecord> CreateLineParser(IRecordAttribute attribute)
        {
            ParameterExpression lineExpression = Expression.Parameter(typeof(String), "line");

            ParameterExpression returnExpression = Expression.Variable(typeof(TRecord), "ret");
            ParameterExpression stringsExpression = Expression.Variable(typeof(String[]), "strings");

            List<Expression> expressions =
                new List<Expression> { Expression.Assign(returnExpression, Expression.New(typeof(TRecord))) };

            if (attribute.GetType() == typeof(DelimitedRecordAttribute))
            {
                DelimitedRecordAttribute delimitedRecordAttribute = (DelimitedRecordAttribute)attribute;

                Expression seperatorArray = Expression.NewArrayInit(typeof(String), Expression.Constant(delimitedRecordAttribute.Delimiter));
                expressions.Add(Expression.Assign(stringsExpression, Expression.Call(lineExpression, typeof(String).GetMethod("Split", new[] { typeof(String[]), typeof(StringSplitOptions) }), seperatorArray, Expression.Constant(StringSplitOptions.None))));
            }

            int i = 0;
            foreach (PropertyInfo propertyInfo in typeof(TRecord).GetProperties())
            {
                DelimitedFieldAttribute delimitedFieldAttribute = propertyInfo.GetCustomAttribute<DelimitedFieldAttribute>(false);
                FixedWidthFieldAttribute fixedWidthFieldAttribute = propertyInfo.GetCustomAttribute<FixedWidthFieldAttribute>(false);

                if (delimitedFieldAttribute == null && fixedWidthFieldAttribute == null)
                {
                    // If property has no derivative of FieldAttribute then we do not include it in parsing.
                    break;
                }
                else if (delimitedFieldAttribute != null && attribute.GetType() != typeof(DelimitedRecordAttribute))
                {
                    throw new Exception($"{propertyInfo.Name} cannot have a DelimitedField attribute.");
                }
                else if (fixedWidthFieldAttribute != null && attribute.GetType() != typeof(FixedWidthRecordAttribute))
                {
                    throw new Exception($"{propertyInfo.Name} cannot have a FixedWidthField attribute.");
                }

                if (delimitedFieldAttribute != null)
                {
                    //If we are missing delimiters then we assume that it is missing.
                    Expression valueExpression = Expression.Condition(Expression.GreaterThan(Expression.ArrayLength(stringsExpression), Expression.Constant(i)), Expression.ArrayIndex(stringsExpression, Expression.Constant(i)), Expression.Constant(""));
                    expressions.AddRange(CreateFieldExpression(propertyInfo, delimitedFieldAttribute, returnExpression, valueExpression));

                    i++;
                }
                else
                {
                    Expression substring = Expression.Call(lineExpression, typeof(string).GetMethod("Substring", new[] { typeof(int), typeof(int) }) ?? throw new InvalidOperationException(), Expression.Constant(i), Expression.Constant(fixedWidthFieldAttribute.FieldSize));
                    expressions.AddRange(CreateFieldExpression(propertyInfo, fixedWidthFieldAttribute, returnExpression, substring));

                    i += fixedWidthFieldAttribute.FieldSize;
                }
            }

            expressions.Add(returnExpression);

            Expression body = Expression.Block(new List<ParameterExpression> { returnExpression, stringsExpression }, expressions);

            LambdaExpression lambda = Expression.Lambda<Func<String, TRecord>>(body, lineExpression);

            return (Func<String, TRecord>)lambda.Compile();
        }

        private static List<Expression> CreateFieldExpression(PropertyInfo propertyInfo, IFieldAttribute fieldAttribute, ParameterExpression returnExpression, Expression valueExpression)
        {
            List<Expression> ret = new List<Expression>();

            Regex nullRegex = (fieldAttribute.NullPattern != null) ? new Regex(fieldAttribute.NullPattern) : new Regex(@"^\s*$");

            Expression isNullMatch = Expression.Call(Expression.Constant(nullRegex), typeof(Regex).GetMethod("IsMatch", new[] { typeof(string) }) ?? throw new InvalidOperationException(), valueExpression);
            Expression assignedExpression = valueExpression;

            if (fieldAttribute.IsRequired)
            {
                ret.Add(Expression.IfThen(isNullMatch, Expression.Throw(Expression.Constant(new Exception($"Field '{propertyInfo.Name}' cannot be null")))));
            }

            List<Expression> temp = new List<Expression>();

            if (fieldAttribute.ValidationPattern != null)
            {
                Regex validationRegex = new Regex(fieldAttribute.ValidationPattern);

                Expression isValidationMatch = Expression.Call(Expression.Constant(validationRegex), typeof(Regex).GetMethod("IsMatch", new[] { typeof(String) }) ?? throw new InvalidOperationException(), valueExpression);

                Expression s = Expression.Call(typeof(String).GetMethod("Format", new[] { typeof(String), typeof(String) }) ?? throw new InvalidOperationException(), Expression.Constant($"Field '{propertyInfo.Name}' does not match the required format.\nValue: {{0}}"), valueExpression);

                Expression exception = Expression.New(typeof(Exception).GetConstructor(new[] { typeof(String) }) ?? throw new InvalidOperationException(), s);

                temp.Add(Expression.IfThen(Expression.Not(isValidationMatch), Expression.Throw(exception)));
            }

            IEnumerable<Attribute> conversionAttributes = propertyInfo.GetCustomAttributes().Where(attribute => attribute.GetType().GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConversionAttribute<>))).ToArray();


            if (conversionAttributes.Count() > 1)
            {
                throw new Exception($"Property '{propertyInfo}' cannot contain multiple conversion attributes.");
            }
            else if (conversionAttributes.Count() == 1)
            {
                assignedExpression = Expression.Convert(Expression.Call(Expression.Constant(conversionAttributes.First()), conversionAttributes.First().GetType().GetMethod("Convert", new[] { typeof(String) }) ?? throw new InvalidOperationException(), assignedExpression), propertyInfo.PropertyType);
            }
            else if (propertyInfo.PropertyType.IsEnum || (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) && propertyInfo.PropertyType.GetGenericArguments()[0].IsEnum))
            {
                Type enumType = (propertyInfo.PropertyType.IsEnum) ? propertyInfo.PropertyType : propertyInfo.PropertyType.GetGenericArguments()[0];

                assignedExpression = Expression.Call(assignedExpression, typeof(String).GetMethod("Replace", new[] { typeof(String), typeof(String) }) ?? throw new InvalidOperationException(), Expression.Constant(" "), Expression.Constant(""));
                assignedExpression = Expression.Call(assignedExpression, typeof(String).GetMethod("Replace", new[] { typeof(String), typeof(String) }) ?? throw new InvalidOperationException(), Expression.Constant("_"), Expression.Constant(""));
                assignedExpression = Expression.Call(assignedExpression, typeof(String).GetMethod("Replace", new[] { typeof(String), typeof(String) }) ?? throw new InvalidOperationException(), Expression.Constant("&"), Expression.Constant(""));
                assignedExpression = Expression.Call(assignedExpression, typeof(String).GetMethod("Replace", new[] { typeof(String), typeof(String) }) ?? throw new InvalidOperationException(), Expression.Constant("-"), Expression.Constant(""));

                if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    assignedExpression = Expression.Convert(Expression.Call(typeof(StreamParser<TRecord>).GetMethod("ParseEnumCaseIgnored").MakeGenericMethod(Nullable.GetUnderlyingType(propertyInfo.PropertyType)), Expression.Constant(propertyInfo.Name), assignedExpression), propertyInfo.PropertyType);
                }
                else
                {
                    assignedExpression = Expression.Call(typeof(StreamParser<TRecord>).GetMethod("ParseEnumCaseIgnored").MakeGenericMethod(propertyInfo.PropertyType), Expression.Constant(propertyInfo.Name), assignedExpression);
                }
            }
            else if (propertyInfo.PropertyType != typeof(String))
            {
                Type conversionType = propertyInfo.PropertyType;

                // If this is a Nullable<?> type then we need to extract the inner type to find the parse method;
                if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    conversionType = propertyInfo.PropertyType.GetGenericArguments()[0];
                }
                
                // Attempt to find a "Parse" method to convert
                MethodInfo parseMethod = conversionType.GetMethod("Parse", new[] { typeof(String) });

                if (parseMethod == null)
                {
                    throw new Exception($"No '{propertyInfo.PropertyType}.Parse(String)' method was found to use for conversion.");
                }

                assignedExpression = Expression.Call(parseMethod, assignedExpression);

                if (conversionType != propertyInfo.PropertyType)
                {
                    assignedExpression = Expression.Convert(assignedExpression, propertyInfo.PropertyType);
                }
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

        public void WriteStream(Stream stream, IEnumerable<TRecord> items)
        {
            throw new NotImplementedException();
        }

        public static T ParseEnumCaseIgnored<T>(string parameterName, string value) where T : struct, IConvertible
        {
            if (Enum.TryParse(value, true, out T result))
                return result;

            throw new ArgumentException($" {parameterName} has unrecognized value: {value}");
        }
    }
}
