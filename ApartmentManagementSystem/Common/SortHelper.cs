using ApartmentManagementSystem.Dtos.Base;
using System.Linq.Expressions;
using System.Reflection;

namespace ApartmentManagementSystem.Common
{
    public static class SortHelper
    {
        public static IQueryable<T> ApplySort<T>(IQueryable<T> query, List<SortQuery> sorts) where T : class
        {
            if (sorts == null || sorts.Count == 0)
            {
                return query;
            }

            var type = typeof(T);
            var parameter = Expression.Parameter(type, "x");

            foreach (var sort in sorts)
            {
                PropertyInfo property = type.GetProperty(sort.Code, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (property == null)
                {
                    continue;
                }

                var propertyType = property.PropertyType;
                var propertyExpression = Expression.Property(parameter, property);
                var keySelector = Expression.Lambda(propertyExpression, parameter);

                if (sort.Direction == SortDirection.Ascending)
                {
                    if (propertyType == typeof(string))
                    {
                        query = query.OrderBy((Expression<Func<T, string>>)keySelector);
                    }
                    else if (propertyType == typeof(int))
                    {
                        query = query.OrderBy((Expression<Func<T, int>>)keySelector);
                    }
                    else if (propertyType == typeof(float))
                    {
                        query = query.OrderBy((Expression<Func<T, float>>)keySelector);
                    }
                    else if (propertyType == typeof(double))
                    {
                        query = query.OrderBy((Expression<Func<T, double>>)keySelector);
                    }
                    else if (propertyType == typeof(decimal))
                    {
                        query = query.OrderBy((Expression<Func<T, decimal>>)keySelector);
                    }
                    else if (propertyType == typeof(Guid))
                    {
                        query = query.OrderBy((Expression<Func<T, Guid>>)keySelector);
                    }
                    else if (propertyType == typeof(DateTime))
                    {
                        query = query.OrderBy((Expression<Func<T, DateTime>>)keySelector);
                    }
                    else if (propertyType == typeof(bool))
                    {
                        query = query.OrderBy((Expression<Func<T, bool>>)keySelector);
                    }
                    else if (propertyType == typeof(DateTime))
                    {
                        query = query.OrderBy((Expression<Func<T, DateTime>>)keySelector);
                    }
                    else if (Nullable.GetUnderlyingType(propertyType) != null && Nullable.GetUnderlyingType(propertyType) == typeof(DateTime))
                    {
                        query = query.OrderBy((Expression<Func<T, DateTime?>>)keySelector);
                    }
                }
                else if (sort.Direction == SortDirection.Descending)
                {
                    if (propertyType == typeof(string))
                    {
                        query = query.OrderByDescending((Expression<Func<T, string>>)keySelector);
                    }
                    else if (propertyType == typeof(int))
                    {
                        query = query.OrderByDescending((Expression<Func<T, int>>)keySelector);
                    }
                    else if (propertyType == typeof(float))
                    {
                        query = query.OrderByDescending((Expression<Func<T, float>>)keySelector);
                    }
                    else if (propertyType == typeof(double))
                    {
                        query = query.OrderByDescending((Expression<Func<T, double>>)keySelector);
                    }
                    else if (propertyType == typeof(decimal))
                    {
                        query = query.OrderByDescending((Expression<Func<T, decimal>>)keySelector);
                    }
                    else if (propertyType == typeof(Guid))
                    {
                        query = query.OrderByDescending((Expression<Func<T, Guid>>)keySelector);
                    }
                    else if (propertyType == typeof(DateTime))
                    {
                        query = query.OrderByDescending((Expression<Func<T, DateTime>>)keySelector);
                    }
                    else if (propertyType == typeof(bool))
                    {
                        query = query.OrderByDescending((Expression<Func<T, bool>>)keySelector);
                    }
                    else if (propertyType == typeof(DateTime))
                    {
                        query = query.OrderByDescending((Expression<Func<T, DateTime>>)keySelector);
                    }
                    else if (Nullable.GetUnderlyingType(propertyType) != null && Nullable.GetUnderlyingType(propertyType) == typeof(DateTime))
                    {
                        query = query.OrderByDescending((Expression<Func<T, DateTime?>>)keySelector);
                    }
                }
            }

            return query;
        }
    }
}
