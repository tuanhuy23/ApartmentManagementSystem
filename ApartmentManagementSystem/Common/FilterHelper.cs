using ApartmentManagementSystem.Dtos.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ApartmentManagementSystem.Common
{
    public static class FilterHelper
    {
        public static IQueryable<T> ApplyFilters<T>(IQueryable<T> query, List<FilterQuery> filters) where T : class
        {
            if (filters == null || filters.Count == 0)
            {
                return query;
            }

            foreach (var filter in filters)
            {
                PropertyInfo property = typeof(T).GetProperty(filter.Code, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (property == null)
                {
                    continue;
                }

                query = ApplyFilter(query, property, filter.Operator, filter.Value);
            }

            return query;
        }

        private static IQueryable<T> ApplyFilter<T>(IQueryable<T> query, PropertyInfo property, FilterOperator filterOperator, dynamic filterValue) where T : class
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            MemberExpression member = Expression.Property(parameter, property);
            ConstantExpression constant = Expression.Constant(filterValue);

            Expression body = GetExpression(member, filterOperator, constant);

            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
            return query.Where(lambda);
        }

        private static Expression GetExpression(MemberExpression member, FilterOperator filterOperator, object filterValue)
        {
            Expression convertedValue = null;

            if (filterValue == null)
            {
                throw new ArgumentException("FilterQuery value cannot be null");
            }

            if (filterValue.GetType() == member.Type)
            {
                convertedValue = Expression.Constant(filterValue);
            }
            else
            {
                var constantExpression = filterValue as ConstantExpression;
                if (constantExpression != null)
                {
                    if (member.Type == typeof(string))
                    {
                        if (filterOperator == FilterOperator.In)
                        {
                            if (filterValue == null)
                            {
                                throw new ArgumentException("Invalid filter value for FilterOperator.In");
                            }

                            var jsonArray = JsonConvert.DeserializeObject<JArray>(filterValue.ToString());

                            if (jsonArray == null)
                            {
                                throw new ArgumentException("Invalid filter value for FilterOperator.In");
                            }

                            var values = jsonArray.Select(j => j.ToString());

                            var parameter = Expression.Parameter(member.Type, "x");

                            Expression orExpression = null;

                            foreach (var value in values)
                            {
                                var equalsExpression = Expression.Equal(member, Expression.Constant(value));

                                orExpression = orExpression == null
                                    ? equalsExpression
                                    : Expression.OrElse(orExpression, equalsExpression);
                            }

                            return orExpression;
                        }
                        else
                        {
                            if (constantExpression.Value is String stringValue)
                            {
                                convertedValue = Expression.Constant(stringValue);
                            }
                            else if (constantExpression.Value == null)
                            {
                                convertedValue = Expression.Constant(null, member.Type);
                            }
                            else
                            {
                                throw new ArgumentException("Cannot convert filter value to a string");
                            }
                        }
                    }

                    else if (member.Type == typeof(Guid) || (Nullable.GetUnderlyingType(member.Type) != null &&
                                                             Nullable.GetUnderlyingType(member.Type) == typeof(Guid)))
                    {
                        if (filterOperator == FilterOperator.In)
                        {
                            if (filterValue == null)
                            {
                                throw new ArgumentException("Invalid filter value for FilterOperator.In");
                            }

                            var jsonArray = JsonConvert.DeserializeObject<JArray>(filterValue.ToString());

                            if (jsonArray == null)
                            {
                                throw new ArgumentException("Invalid filter value for FilterOperator.In");
                            }

                            var values = jsonArray.Select(j => j.ToString());

                            var parameter = Expression.Parameter(member.Type, "x");

                            Expression orExpression = null;

                            foreach (var value in values)
                            {
                                if (Guid.TryParse(value, out Guid parsedValue))
                                {
                                    Expression equalsExpression;

                                    if (member.Type.IsGenericType &&
                                        member.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                                    {
                                        // If member is Nullable<Guid>, use HasValue and Value properties
                                        var hasValueExpression = Expression.Property(member, "HasValue");
                                        var valueExpression = Expression.Property(member, "Value");

                                        equalsExpression = Expression.AndAlso(
                                            hasValueExpression,
                                            Expression.Call(
                                                valueExpression,
                                                typeof(Guid).GetMethod("Equals", new[] { typeof(Guid) }),
                                                Expression.Constant(parsedValue)
                                            )
                                        );
                                    }
                                    else
                                    {
                                        // Otherwise, use direct Equals call
                                        equalsExpression = Expression.Call(
                                            member,
                                            typeof(Guid).GetMethod("Equals", new[] { typeof(Guid) }),
                                            Expression.Constant(parsedValue)
                                        );
                                    }

                                    orExpression = orExpression == null
                                        ? equalsExpression
                                        : Expression.OrElse(orExpression, equalsExpression);
                                }
                                else
                                {
                                    throw new ArgumentException($"Cannot convert filter value '{value}' to Guid");
                                }
                            }

                            return orExpression;
                        }
                        else
                        {
                            if (constantExpression.Value is Guid guidValue)
                            {
                                convertedValue = Expression.Constant(guidValue);
                            }
                            else if (constantExpression.Value == null)
                            {
                                convertedValue = Expression.Constant(null, member.Type);
                            }
                            else
                            {
                                // Thử kiểm tra nếu giá trị có thể được chuyển đổi thành Guid
                                if (Guid.TryParse(constantExpression.Value.ToString(), out Guid nullableGuidValue))
                                {
                                    // Nếu thành công, tạo biểu thức constant
                                    convertedValue = Expression.Constant(nullableGuidValue, member.Type);
                                }
                                else
                                {
                                    throw new ArgumentException("Cannot convert filter value to a Nullable<Guid>");
                                }
                            }
                        }
                    }

                    else if (member.Type == typeof(bool))
                    {
                        if (constantExpression.Value is bool boolValue)
                        {
                            convertedValue = Expression.Constant(boolValue);
                        }
                        else
                        {
                            throw new ArgumentException("Cannot convert filter value to a boolean");
                        }
                    }

                    else if (member.Type == typeof(int) || (Nullable.GetUnderlyingType(member.Type) != null &&
                                                           Nullable.GetUnderlyingType(member.Type) == typeof(int)))
                    {
                        if (filterOperator == FilterOperator.In)
                        {
                            if (filterValue == null)
                            {
                                throw new ArgumentException("Invalid filter value for FilterOperator.In");
                            }

                            var jsonArray = JsonConvert.DeserializeObject<JArray>(filterValue.ToString());

                            if (jsonArray == null)
                            {
                                throw new ArgumentException("Invalid filter value for FilterOperator.In");
                            }

                            var values = jsonArray.Select(j => j.ToString());

                            var parameter = Expression.Parameter(member.Type, "x");

                            Expression orExpression = null;

                            foreach (var value in values)
                            {
                                if (int.TryParse(value, out int parsedValue))
                                {
                                    var equalsExpression = Expression.Equal(member, Expression.Constant(parsedValue));
                                    orExpression = orExpression == null
                                        ? equalsExpression
                                        : Expression.OrElse(orExpression, equalsExpression);
                                }
                                else
                                {
                                    throw new ArgumentException($"Cannot convert filter value '{value}' to int");
                                }
                            }

                            return orExpression;
                        }
                        else
                        {
                            if (constantExpression.Value is int intValue)
                            {
                                convertedValue = Expression.Constant(intValue);
                            }
                            else if (constantExpression.Value is long longValue)
                            {
                                if (longValue >= int.MinValue && longValue <= int.MaxValue)
                                {
                                    convertedValue = Expression.Constant((int)longValue);
                                }
                                else
                                {
                                    throw new ArgumentException(
                                        "Cannot convert filter value to an integer, value is out of range for int");
                                }
                            }
                            else if (constantExpression.Value == null && member.Type.IsGenericType &&
                                     member.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                // Handle nullable int with null value
                                convertedValue = Expression.Constant(null, member.Type);
                            }
                            else
                            {
                                throw new ArgumentException("Cannot convert filter value to an integer");
                            }
                        }
                    }

                    else if (member.Type == typeof(long) || (Nullable.GetUnderlyingType(member.Type) != null &&
                                                             Nullable.GetUnderlyingType(member.Type) == typeof(long)))
                    {
                        if (filterOperator == FilterOperator.In)
                        {
                            if (filterValue == null)
                            {
                                throw new ArgumentException("Invalid filter value for FilterOperator.In");
                            }

                            var jsonArray = JsonConvert.DeserializeObject<JArray>(filterValue.ToString());

                            if (jsonArray == null)
                            {
                                throw new ArgumentException("Invalid filter value for FilterOperator.In");
                            }

                            var values = jsonArray.Select(j => j.ToString());

                            var parameter = Expression.Parameter(member.Type, "x");

                            Expression orExpression = null;

                            foreach (var value in values)
                            {
                                if (long.TryParse(value, out long parsedValue))
                                {
                                    var equalsExpression = Expression.Equal(member, Expression.Constant(parsedValue));
                                    orExpression = orExpression == null
                                        ? equalsExpression
                                        : Expression.OrElse(orExpression, equalsExpression);
                                }
                                else
                                {
                                    throw new ArgumentException($"Cannot convert filter value '{value}' to long");
                                }
                            }

                            return orExpression;
                        }
                        else
                        {
                            if (constantExpression.Value is long longValue)
                            {
                                convertedValue = Expression.Constant(longValue);
                            }
                            else if (constantExpression.Value == null && member.Type.IsGenericType &&
                                     member.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                // Handle nullable long with null value
                                convertedValue = Expression.Constant(null, member.Type);
                            }
                            else
                            {
                                throw new ArgumentException("Cannot convert filter value to a long");
                            }
                        }
                    }

                    else if (member.Type == typeof(float) || (Nullable.GetUnderlyingType(member.Type) != null &&
                                                              Nullable.GetUnderlyingType(member.Type) == typeof(float)))
                    {
                        if (filterOperator == FilterOperator.In)
                        {
                            if (filterValue == null)
                            {
                                throw new ArgumentException("Invalid filter value for FilterOperator.In");
                            }

                            var jsonArray = JsonConvert.DeserializeObject<JArray>(filterValue.ToString());

                            if (jsonArray == null)
                            {
                                throw new ArgumentException("Invalid filter value for FilterOperator.In");
                            }

                            var values = jsonArray.Select(j => j.ToString());

                            var parameter = Expression.Parameter(member.Type, "x");

                            Expression orExpression = null;

                            foreach (var value in values)
                            {
                                if (float.TryParse(value, out float parsedValue))
                                {
                                    var equalsExpression = Expression.Equal(member, Expression.Constant(parsedValue));
                                    orExpression = orExpression == null
                                        ? equalsExpression
                                        : Expression.OrElse(orExpression, equalsExpression);
                                }
                                else
                                {
                                    throw new ArgumentException($"Cannot convert filter value '{value}' to float");
                                }
                            }

                            return orExpression;
                        }
                        else
                        {
                            if (constantExpression.Value is float floatValue)
                            {
                                convertedValue = Expression.Constant(floatValue);
                            }
                            else if (constantExpression.Value == null && member.Type.IsGenericType &&
                                     member.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                // Handle nullable float with null value
                                convertedValue = Expression.Constant(null, member.Type);
                            }
                            else
                            {
                                throw new ArgumentException("Cannot convert filter value to a float");
                            }
                        }
                    }

                    else if (member.Type == typeof(double) || (Nullable.GetUnderlyingType(member.Type) != null &&
                                                               Nullable.GetUnderlyingType(member.Type) ==
                                                               typeof(double)))
                    {
                        if (constantExpression.Value is double doubleValue)
                        {
                            convertedValue = Expression.Constant(doubleValue);
                        }
                        else if (constantExpression.Value is float floatValue)
                        {
                            convertedValue = Expression.Constant((double)floatValue);
                        }
                        else if (constantExpression.Value is int intValue)
                        {
                            convertedValue = Expression.Constant((double)intValue);
                        }
                        else if (constantExpression.Value is long longValue)
                        {
                            if (longValue >= double.MinValue && longValue <= double.MaxValue)
                            {
                                convertedValue = Expression.Constant((double)longValue);
                            }
                            else
                            {
                                throw new ArgumentException(
                                    "Cannot convert filter value to a double, value is out of range for double");
                            }
                        }
                        else if (constantExpression.Value == null && member.Type.IsGenericType &&
                                 member.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            // Handle nullable double with null value
                            convertedValue = Expression.Constant(null, member.Type);
                        }
                        else
                        {
                            throw new ArgumentException("Cannot convert filter value to a double");
                        }
                    }

                    else if (member.Type == typeof(DateTime) || (Nullable.GetUnderlyingType(member.Type) != null &&
                                                                 Nullable.GetUnderlyingType(member.Type) ==
                                                                 typeof(DateTime)))
                    {
                        if (DateTime.TryParse(constantExpression.Value.ToString(), out DateTime dateTimeValue))
                        {
                            Expression dateFilter;

                            if (Nullable.GetUnderlyingType(member.Type) != null)
                            {
                                // Nullable<DateTime> handling
                                var hasValueExpression = Expression.Property(member, "HasValue");
                                var valueExpression = Expression.Property(member, "Value");

                                if (filterOperator == FilterOperator.Equals)
                                {
                                    dateFilter = Expression.AndAlso(
                                        hasValueExpression,
                                        Expression.AndAlso(
                                            Expression.GreaterThanOrEqual(valueExpression,
                                                Expression.Constant(dateTimeValue)),
                                            Expression.LessThanOrEqual(valueExpression,
                                                Expression.Constant(dateTimeValue.AddDays(1).AddTicks(-1)))
                                        )
                                    );
                                }
                                else if (filterOperator == FilterOperator.GreaterThanOrEqual)
                                {
                                    dateFilter = Expression.AndAlso(
                                        hasValueExpression,
                                        Expression.GreaterThanOrEqual(valueExpression,
                                            Expression.Constant(dateTimeValue))
                                    );
                                }
                                else if (filterOperator == FilterOperator.LessThanOrEqual)
                                {
                                    dateFilter = Expression.AndAlso(
                                        hasValueExpression,
                                        Expression.LessThanOrEqual(valueExpression,
                                            Expression.Constant(dateTimeValue.AddDays(1).AddTicks(-1)))
                                    );
                                }
                                else if (filterOperator == FilterOperator.NotEquals)
                                {
                                    dateFilter = Expression.OrElse(
                                        Expression.Not(hasValueExpression),
                                        Expression.OrElse(
                                            Expression.LessThan(valueExpression, Expression.Constant(dateTimeValue)),
                                            Expression.GreaterThan(valueExpression,
                                                Expression.Constant(dateTimeValue.AddDays(1).AddTicks(-1)))
                                        )
                                    );
                                }
                                else if (filterOperator == FilterOperator.GreaterThan)
                                {
                                    dateFilter = Expression.AndAlso(
                                        hasValueExpression,
                                        Expression.GreaterThan(valueExpression,
                                            Expression.Constant(dateTimeValue.AddDays(1).AddTicks(-1)))
                                    );
                                }
                                else if (filterOperator == FilterOperator.LessThan)
                                {
                                    dateFilter = Expression.AndAlso(
                                        hasValueExpression,
                                        Expression.LessThan(valueExpression, Expression.Constant(dateTimeValue))
                                    );
                                }
                                else
                                {
                                    throw new NotSupportedException(
                                        $"FilterQuery operator {filterOperator} is not supported for Nullable<DateTime> type.");
                                }
                            }
                            else
                            {
                                // DateTime handling
                                if (filterOperator == FilterOperator.Equals)
                                {
                                    // createdAt >= 2023-12-05:00:00:00 and createdAt <= 2023-12-05:23:59:59
                                    DateTime filterEndDate = dateTimeValue.AddDays(1).AddTicks(-1);

                                    var greaterThanOrEqual =
                                        Expression.GreaterThanOrEqual(member, Expression.Constant(dateTimeValue));
                                    var lessThanOrEqual =
                                        Expression.LessThanOrEqual(member, Expression.Constant(filterEndDate));

                                    dateFilter = Expression.AndAlso(greaterThanOrEqual, lessThanOrEqual);
                                }
                                else if (filterOperator == FilterOperator.GreaterThanOrEqual)
                                {
                                    // createdAt >= 2023-12-05:00:00:00
                                    dateFilter =
                                        Expression.GreaterThanOrEqual(member, Expression.Constant(dateTimeValue));
                                }
                                else if (filterOperator == FilterOperator.LessThanOrEqual)
                                {
                                    // createdAt <= 2023-12-06:23:59:59
                                    DateTime filterEndDate = dateTimeValue.AddDays(1).AddTicks(-1);
                                    dateFilter = Expression.LessThanOrEqual(member, Expression.Constant(filterEndDate));
                                }
                                else if (filterOperator == FilterOperator.NotEquals)
                                {
                                    // createdAt < 2023-12-05:00:00:00 or createdAt > 2023-12-06:23:59:59
                                    DateTime filterEndDate = dateTimeValue.AddDays(1).AddTicks(-1);

                                    var lessThan = Expression.LessThan(member, Expression.Constant(dateTimeValue));
                                    var greaterThan =
                                        Expression.GreaterThan(member, Expression.Constant(filterEndDate));

                                    dateFilter = Expression.OrElse(lessThan, greaterThan);
                                }
                                else if (filterOperator == FilterOperator.GreaterThan)
                                {
                                    // createdAt > 2023-12-05:23:59:59
                                    DateTime filterEndDate = dateTimeValue.AddDays(1).AddTicks(-1);
                                    dateFilter = Expression.GreaterThan(member, Expression.Constant(filterEndDate));
                                }
                                else if (filterOperator == FilterOperator.LessThan)
                                {
                                    // createdAt < 2023-12-06:00:00:00
                                    dateFilter = Expression.LessThan(member, Expression.Constant(dateTimeValue));
                                }
                                else
                                {
                                    throw new NotSupportedException(
                                        $"FilterQuery operator {filterOperator} is not supported for DateTime type.");
                                }
                            }

                            return dateFilter;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid DateTime format");
                        }
                    }

                    else if (Nullable.GetUnderlyingType(member.Type) != null &&
                             Nullable.GetUnderlyingType(member.Type) == typeof(DateTime))
                    {
                        if (constantExpression.Value == null)
                        {
                            // Handle null case, if needed
                            // For example, you might want to create a condition like "dateField == null"
                            return Expression.Equal(member, Expression.Constant(null, member.Type));
                        }
                        else if (DateTime.TryParse(constantExpression.Value.ToString(), out DateTime dateTimeValue))
                        {
                            Expression dateFilter;

                            if (filterOperator == FilterOperator.Equals)
                            {
                                // Handle Equals condition for DateTime?
                                DateTime filterEndDate = dateTimeValue.AddDays(1).AddTicks(-1);
                                var greaterThanOrEqual =
                                    Expression.GreaterThanOrEqual(member, Expression.Constant(dateTimeValue));
                                var lessThanOrEqual =
                                    Expression.LessThanOrEqual(member, Expression.Constant(filterEndDate));
                                dateFilter = Expression.AndAlso(greaterThanOrEqual, lessThanOrEqual);
                            }
                            else if (filterOperator == FilterOperator.GreaterThanOrEqual)
                            {
                                // Handle GreaterThanOrEqual condition for DateTime?
                                dateFilter = Expression.GreaterThanOrEqual(member, Expression.Constant(dateTimeValue));
                            }
                            else if (filterOperator == FilterOperator.LessThanOrEqual)
                            {
                                // Handle LessThanOrEqual condition for DateTime?
                                DateTime filterEndDate = dateTimeValue.AddDays(1).AddTicks(-1);
                                dateFilter = Expression.LessThanOrEqual(member, Expression.Constant(filterEndDate));
                            }
                            else if (filterOperator == FilterOperator.NotEquals)
                            {
                                // createdAt < 2023-12-05:00:00:00 or createdAt > 2023-12-06:23:59:59
                                DateTime filterEndDate = dateTimeValue.AddDays(1).AddTicks(-1);

                                var lessThan = Expression.LessThan(member, Expression.Constant(dateTimeValue));
                                var greaterThan = Expression.GreaterThan(member, Expression.Constant(filterEndDate));

                                dateFilter = Expression.OrElse(lessThan, greaterThan);
                            }
                            else if (filterOperator == FilterOperator.GreaterThan)
                            {
                                // createdAt > 2023-12-05:23:59:59
                                DateTime filterEndDate = dateTimeValue.AddDays(1).AddTicks(-1);
                                dateFilter = Expression.GreaterThan(member, Expression.Constant(filterEndDate));
                            }
                            else if (filterOperator == FilterOperator.LessThan)
                            {
                                // createdAt < 2023-12-06:00:00:00
                                dateFilter = Expression.LessThan(member, Expression.Constant(dateTimeValue));
                            }
                            else
                            {
                                throw new NotSupportedException(
                                    $"FilterQuery operator {filterOperator} is not supported for DateTime type.");
                            }

                            return dateFilter;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid DateTime format");
                        }
                    }

                    else
                    {
                        throw new ArgumentException("Unsupported filter value type");
                    }
                }
                else
                {
                    throw new ArgumentException("Unsupported filter value type");
                }
            }

            if (convertedValue == null)
            {
                throw new ArgumentException("convertedValue not value ");
            }

            switch (filterOperator)
            {
                case FilterOperator.Equals:
                    return Expression.Equal(member, convertedValue);
                case FilterOperator.NotEquals:
                    return Expression.NotEqual(member, convertedValue);
                case FilterOperator.GreaterThan:
                    return Expression.GreaterThan(member, convertedValue);
                case FilterOperator.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(member, convertedValue);
                case FilterOperator.LessThan:
                    return Expression.LessThan(member, convertedValue);
                case FilterOperator.LessThanOrEqual:
                    return Expression.LessThanOrEqual(member, convertedValue);
                case FilterOperator.Contains:
                    if (member.Type == typeof(string))
                    {
                        MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                        return Expression.Call(member, containsMethod, convertedValue);
                    }
                    else
                    {
                        throw new ArgumentException("Unsupported filter operator for non-string type");
                    }
                case FilterOperator.StartsWith:
                    if (member.Type == typeof(string))
                    {
                        MethodInfo startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
                        return Expression.Call(member, startsWithMethod, convertedValue);
                    }
                    else
                    {
                        throw new ArgumentException("Unsupported filter operator for non-string type");
                    }
                case FilterOperator.EndsWith:
                    if (member.Type == typeof(string))
                    {
                        MethodInfo endsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
                        return Expression.Call(member, endsWithMethod, convertedValue);
                    }
                    else
                    {
                        throw new ArgumentException("Unsupported filter operator for non-string type");
                    }
                default:
                    throw new NotSupportedException($"FilterQuery operator {filterOperator} is not supported.");
            }
        }
    }
}
