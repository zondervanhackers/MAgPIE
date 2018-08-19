using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

using ZondervanLibrary.SharedLibrary.Parsing.Records;
using ZondervanLibrary.SharedLibrary.Parsing.Fields;

namespace ZondervanLibrary.SharedLibrary.Parsing.Parsers
{
    public class DelimitedStreamParser<TRecord> : StreamParserBase<TRecord>
        where TRecord : new()
    {
        private readonly Func<String, TRecord> _lineParser;

        public DelimitedStreamParser()
        {
            if (_recordAttribute.GetType() != typeof(DelimitedRecordAttribute))
            {
                throw new ArgumentException("TRecord must have a delimited record attribute.");
            }

            _lineParser = CreateLineParser();
        }

        public override IEnumerable<TRecord> ParseStream(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                String line;
                Int32 lineNumber = 1;

                if (_recordAttribute.IgnoreFirstLine)
                {
                    reader.ReadLine();
                    lineNumber++;
                }

                while ((line = reader.ReadLine()) != null)
                {
                    TRecord temp;

                    try
                    {
                        temp = _lineParser(line);
                    }
                    catch (Exception exception)
                    {
                        throw new ParsingException(lineNumber, exception.Message, exception);
                    }

                    yield return temp;

                    lineNumber++;
                }
            }
        }

        public override void WriteStream(Stream stream, IEnumerable<TRecord> items)
        {
            throw new NotImplementedException();
        }

        private Func<String, TRecord> CreateLineParser()
        {
            ParameterExpression lineExpression = Expression.Parameter(typeof(String), "line");

            ParameterExpression returnExpression = Expression.Variable(typeof(TRecord), "ret");
            ParameterExpression stringsExpression = Expression.Variable(typeof(String[]), "strings");

            List<Expression> expressions =
                new List<Expression> { Expression.Assign(returnExpression, Expression.New(typeof(TRecord))) };


            // Create expression to split string
            DelimitedRecordAttribute delimitedRecordAttribute = (DelimitedRecordAttribute)_recordAttribute;

            Expression seperatorArray = Expression.NewArrayInit(typeof(String), Expression.Constant(delimitedRecordAttribute.Delimiter));
            expressions.Add(Expression.Assign(stringsExpression, Expression.Call(lineExpression, typeof(String).GetMethod("Split", new Type[] { typeof(String[]), typeof(StringSplitOptions) }), seperatorArray, Expression.Constant(StringSplitOptions.None))));

            int i = 0;
            foreach (PropertyInfo propertyInfo in typeof(TRecord).GetProperties())
            {
                DelimitedFieldAttribute delimitedFieldAttribute = propertyInfo.GetCustomAttribute<DelimitedFieldAttribute>(false);

                if (delimitedFieldAttribute == null)
                {
                    // If a property does not have a DelimitedFieldAttribute then we can simply skip it.
                    break;
                }

                expressions.AddRange(CreateFieldExpression(propertyInfo, delimitedFieldAttribute, returnExpression, Expression.ArrayIndex(stringsExpression, Expression.Constant(i))));
                
                i++;
            }

            expressions.Add(returnExpression);

            Expression body = Expression.Block(new List<ParameterExpression> { returnExpression, stringsExpression }, expressions);

            LambdaExpression lambda = Expression.Lambda<Func<String, TRecord>>(body, lineExpression);

            return (Func<String, TRecord>)lambda.Compile();
        }
    }
}
