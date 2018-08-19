using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;

namespace ZondervanLibrary.SharedLibrary.Equatable
{
    /// <summary>
    /// Provides a base class that supplies the <see cref="IEquatable{T}" /> implementation for derived classes using reflection.
    /// </summary>
    /// <typeparam name="T">The type of the derived class that is to be equatable.</typeparam>
    public abstract class EquatableBase<T> : IEquatable<T>
        where T : EquatableBase<T>
    {
        private static readonly Func<T, T, Boolean> _equals;
        private static readonly Func<T, Int32> _getHashCode;
        private static readonly Boolean _isImmutable;

        private Int32? _hashCode;

        public static Boolean EnumerableSequenceEquals<TSource>(IEnumerable<TSource> list1, IEnumerable<TSource> list2)
        {
            if (list1 == null)
            {
                return list2 == null;
            }
            else
            {
                return list2 != null && list1.SequenceEqual(list2);
            }
        }

        public static Boolean EnumerableHashSetEquals<TSource>(IEnumerable<TSource> list1, IEnumerable<TSource> list2)
        {
            if (list1 == null)
            {
                return list2 == null;
            }
            else
            {
                return list2 != null && (list1).SequenceEqual(list2);
            }
        }

        private class EqualityComparer : IEqualityComparer<object>
        {
            private readonly Func<object, object, bool> _compare;
            private readonly Func<object, int> _hashcode;

            public EqualityComparer(Type type)
            {
                ParameterExpression leftParam = Expression.Parameter(typeof(object), "left");
                ParameterExpression rightParam = Expression.Parameter(typeof(object), "right");

                UnaryExpression convertedLeftParam = Expression.Convert(leftParam, type);
                UnaryExpression convertedRightParam = Expression.Convert(rightParam, type);

                Expression equalityExpression;

                if (HasOperatorEquality(type))
                {
                    equalityExpression = Expression.Equal(convertedLeftParam, convertedRightParam);
                }
                else if (IsEquatable(type))
                {
                    equalityExpression = Expression.Condition(Expression.Equal(convertedLeftParam, Expression.Constant(null)),
                                                      Expression.Equal(convertedRightParam, Expression.Constant(null)),
                                                      Expression.Call(convertedLeftParam, type.GetMethod("Equals", new[] { type }), convertedRightParam));
                }
                else
                {
                    throw new NotImplementedException();
                }

                _compare = (Func<object, object, bool>)Expression.Lambda(equalityExpression, leftParam, rightParam).Compile();
            }

            public new bool Equals(object x, object y)
            {
                return _compare(x, y);
            }

            public int GetHashCode(object obj)
            {
                return 0;
            }
        }

        public static Boolean MultiTypedEnumerableHashSetEquals<TSource>(IEnumerable<TSource> list1, IEnumerable<TSource> list2)
        {
            Dictionary<Type, HashSet<Object>> dictionary = new Dictionary<Type, HashSet<object>>();

            foreach (TSource e in list1)
            {
                Type type = e.GetType();

                if (!dictionary.ContainsKey(type))
                    dictionary.Add(type, new HashSet<object>(new EqualityComparer(type)));

                if (!dictionary[type].Contains(e))
                    dictionary[type].Add(e);
            }

            foreach (TSource e in list2)
            {
                Type type = e.GetType();

                if (!dictionary.ContainsKey(type))
                    return false;

                if (!dictionary[type].Contains(e))
                    return false;
            }

            return true;
        }

        public static Boolean EnumerableDictionaryEquals<TSource>(IEnumerable<TSource> list1, IEnumerable<TSource> list2)
        {
            if (list1 == null)
            {
                return list2 == null;
            }
            else
            {
                if (list1.Count() != list2.Count())
                {
                    return false;
                }

                Dictionary<TSource, Int32> dictionary = new Dictionary<TSource, Int32>(list1.Count());

                foreach (TSource element in list1)
                {
                    if (dictionary.ContainsKey(element))
                    {
                        dictionary[element]++;
                    }
                    else
                    {
                        dictionary.Add(element, 1);
                    }
                }

                foreach (TSource element in list2)
                {
                    if (!dictionary.ContainsKey(element) || dictionary[element]-- < 0)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        static bool HasOperatorEquality(Type type)
        {
            return type.IsValueType ||
                   type.IsEnum ||
                   (type.GetMethod("op_Equality") != null && type.GetMethod("op_Equality").IsSpecialName);
        }

        static bool IsEquatable(Type type)
        {
            return type.GetInterfaces().Any(x => x.IsGenericType &&
                                         x.GetGenericTypeDefinition() == typeof(IEquatable<>) &&
                                         x.GetGenericArguments().First() == type);
        }

        static bool IsEnumerable(Type type)
        {
            return type.GetInterfaces().Any(x => x.IsGenericType &&
                                        x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        static bool IsCollection(Type type)
        {
            return type.GetInterfaces().Any(x => x.IsGenericType &&
                                    x.GetGenericTypeDefinition() == typeof(ICollection<>));
        }

        static EquatableBase()
        {
            // Static constructor

            // Burden of null checking the parameter

            ParameterExpression param1 = Expression.Parameter(typeof(T), "param1");
            ParameterExpression param2 = Expression.Parameter(typeof(T), "param2");

            //Expression<Func<object, object, Boolean>> comparison = (object p1, object p2) => (p1 == null) ? (p2 == null) : p1.Equals(p2);

            // Struct vs. Reference Type

            Expression expression = typeof(T).GetProperties().Select<PropertyInfo, Expression>(propertyInfo =>
            {
                MemberExpression param1Property = Expression.Property(param1, propertyInfo);
                MemberExpression param2Property = Expression.Property(param2, propertyInfo);
                Type propertyType = propertyInfo.PropertyType;

                if (HasOperatorEquality(propertyType))
                {
                    return Expression.Equal(param1Property, param2Property);
                }
                else if (IsEquatable(propertyType))
                {
                    return Expression.Condition(Expression.Equal(param1Property, Expression.Constant(null)),
                                                Expression.Equal(param2Property, Expression.Constant(null)),
                                                Expression.Call(param1Property, propertyType.GetMethod("Equals", new Type[] { propertyType }), param2Property));
                }
                else if (IsEnumerable(propertyType))
                {
                    Type enumerableType = (propertyType.GetGenericArguments().FirstOrDefault() ?? propertyType.GetElementType()) ??
                                         propertyType.GetInterfaces().First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)).GetGenericArguments().First();

                    if (!IsEquatable(enumerableType))
                    {
                        if (enumerableType == typeof(Object))
                        {
                            MethodInfo enumerableHashSetEquals = typeof(EquatableBase<T>).GetMethod("EnumerableHashSetEquals")?.MakeGenericMethod(enumerableType);

                            return Expression.Call(enumerableHashSetEquals, param1Property, param2Property);
                        }
                        else
                        {
                            throw new Exception($"An appropiate equality comparison could not be found for type {enumerableType.ToString()}.");
                        }
                    }

                    bool nestedEnumerable = enumerableType.GetProperties().Any(x => IsCollection(x.PropertyType));

                    Boolean ignoreDuplicates = true;
                    Boolean assumeOrdered = false;

                    if (nestedEnumerable)
                    {
                        MethodInfo MultiTypedEnumerableHashSetEquals = typeof(EquatableBase<T>).GetMethod("MultiTypedEnumerableHashSetEquals")?.MakeGenericMethod(enumerableType);

                        return Expression.Call(MultiTypedEnumerableHashSetEquals, param1Property, param2Property);
                    }
                    else if (assumeOrdered)
                    {
                        MethodInfo enumerableSequenceEquals = typeof(EquatableBase<T>).GetMethod("enumerableSequenceEquals").MakeGenericMethod(enumerableType);

                        return Expression.Call(enumerableSequenceEquals, param1Property, param2Property);
                    }
                    else if (ignoreDuplicates)
                    {
                        MethodInfo enumerableHashSetEquals = typeof(EquatableBase<T>).GetMethod("EnumerableHashSetEquals").MakeGenericMethod(enumerableType);

                        return Expression.Call(enumerableHashSetEquals, param1Property, param2Property);
                    }
                    else
                    {
                        MethodInfo enumerableDictionaryEquals = typeof(EquatableBase<T>).GetMethod("enumerableDictionaryEquals").MakeGenericMethod(enumerableType);

                        return Expression.Call(enumerableDictionaryEquals, param1Property, param2Property);
                    }
                }
                else
                {
                    throw new Exception($"An appropiate equality comparison could not be found for type {propertyType.ToString()}.");
                }
            }).Aggregate(Expression.AndAlso);

            LambdaExpression lambda = Expression.Lambda(expression, param1, param2);

            _equals = (Func<T, T, Boolean>)lambda.Compile();

            //var getHashCodeExpression = typeof(T).GetProperties().Select<PropertyInfo, Expression>(propertyInfo =>
            //{

            //});

            //------------------------------------------------------------------------------------------------------------------------------------------

            Boolean isImmutable = false;
            Boolean explicitMode = false;

            EquatableAttribute equatableAttribute = typeof(T).CustomAttributes.OfType<EquatableAttribute>().SingleOrDefault();

            if (equatableAttribute != null)
            {
                isImmutable = equatableAttribute.IsImmutable;
                explicitMode = equatableAttribute.ExplicitMode;
            }

            PropertyInfo[] properties = explicitMode ? typeof(T).GetProperties().Where(p => p.CustomAttributes.OfType<EquatablePropertyAttribute>().Any()).ToArray() : typeof(T).GetProperties();

            List<Expression> equalityExpressions = new List<Expression>(properties.Length);
            List<Tuple<Int32, Expression>> hashCodeExpressions = new List<Tuple<Int32, Expression>>(properties.Length);
            ParameterExpression hashVariable = Expression.Variable(typeof(Int32));
            //hashCodeExpressions.Add(Expression.Assign(hashVariable, Expression.Constant(17)));

            foreach (PropertyInfo propertyInfo in properties)
            {
                MemberExpression param1Property = Expression.Property(param1, propertyInfo);
                MemberExpression param2Property = Expression.Property(param2, propertyInfo);
                Type propertyType = propertyInfo.PropertyType;

                String dependency = null;
                Boolean ordered = false;
                Boolean unique = false;
                Int32 rank = 1;

                EquatablePropertyAttribute equatablePropertyAttribute = (EquatablePropertyAttribute)propertyType.GetCustomAttribute(typeof(EquatablePropertyAttribute));

                if (equatablePropertyAttribute != null)
                {
                    dependency = equatablePropertyAttribute.Dependency;
                    ordered = equatablePropertyAttribute.Ordered;
                    unique = equatablePropertyAttribute.Unique;
                    rank = equatablePropertyAttribute.Rank;
                }
                
                //var integralTypes = new Type[] { typeof(Int16), typeof(UInt16), typeof(Int32), typeof(UInt32), typeof(Int64), typeof(UInt64), typeof(Boolean) };

                //if (propertyType

                MethodInfo getHashCode = propertyType.GetMethod("GetHashCode", new Type[] { });

                hashCodeExpressions.Add(new Tuple<Int32, Expression>(rank, Expression.Assign(hashVariable, Expression.Add(Expression.Multiply(hashVariable, Expression.Constant(23)), Expression.Call(param1Property, getHashCode)))));
            }
        }

        protected EquatableBase()
        {
            _hashCode = null;
        }

        /// <inheritdoc/>
        public Boolean Equals(T other)
        {
            if ((object)other == null)
                return false;

            return _equals((T)this, other);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            T castObject = obj as T;
            if ((object)castObject == null)
            {
                return false;
            }

            return _equals((T)this, castObject);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            // If the derived class is immutable we assume the hash code will be the same for all time and cache it.
            if (_isImmutable)
            {
                if (_hashCode == null)
                {
                    _hashCode = _getHashCode((T)this);
                }

                return _hashCode.Value;
            }

            if (_getHashCode != null)
                return _getHashCode((T)this);
            else
                return 0;
        }

        /// <inheritdoc/>
        public static bool operator ==(EquatableBase<T> object1, EquatableBase<T> object2)
        {
            if (ReferenceEquals(object1, object2))
            {
                return true;
            }

            // Due to restriction on T if you are an EquatableBase<T> then you are a T
            T castObject1 = (T)object1;
            T castObject2 = (T)object2;

            if ((object)castObject1 == null)
            {
                return (object)castObject2 == null;
            }
            else
            {
                return castObject1.Equals(castObject2);
            }
        }

        /// <inheritdoc/>
        public static bool operator !=(EquatableBase<T> object1, EquatableBase<T> object2)
        {
            return !(object1 == object2);
        }
    }
}