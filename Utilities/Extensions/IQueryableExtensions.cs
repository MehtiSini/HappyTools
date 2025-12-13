using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using HappyTools.Utilities.Extensions;

namespace HappyTools.Utilities.Extensions
{
    /// <summary>
    /// Represent a class to implements Paging functionality.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginatedList<T> : List<T>
    {
        /// <summary>
        /// Gets a value that indicate current page index (Starts by Zero).
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// Gets a value that indicate each page size.
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// Gets a value that indicate count of all rows in data source.
        /// </summary>
        public int TotalCount { get; private set; }

        /// <summary>
        /// Gets a value that indicate count of pages in data source.
        /// </summary>
        public int TotalPages { get; private set; }

        public PaginatedList(IQueryable<T> source, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = source.Count();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

            AddRange(source.Skip(PageIndex * PageSize).Take(PageSize));
        }

        /// <summary>
        /// Gets a value that indicate that does previous page exists or not.
        /// </summary>
        public bool HasPreviousPage {
            get {
                return PageIndex > 0;
            }
        }

        /// <summary>
        /// Gets a value that indicate that does next page exists or not.
        /// </summary>
        public bool HasNextPage {
            get {
                return PageIndex + 1 < TotalPages;
            }
        }
    }

    public static class PaginatedListExtensions
    {
        /// <summary>
        /// Returns a paginated list.
        /// </summary>
        /// <typeparam name="T">Type of items in list.</typeparam>
        /// <param name="q">A IQueryable instance to apply.</param>
        /// <param name="pageIndex">Page number that starts with zero.</param>
        /// <param name="pageSize">Size of each page.</param>
        /// <returns>Returns a paginated list.</returns>
        /// <remarks>This functionality may not work in SQL Compact 3.5</remarks>
        /// <example>
        ///     Following example shows how use this extension method in ASP.NET MVC web application.
        ///     <code>
        ///     public ActionResult Customers(int? page, int? size)
        ///     {
        ///         var q = from p in customers
        ///                 select p;
        ///     
        ///         return View(q.ToPaginatedList(page.HasValue ? page.Value : 1, size.HasValue ? size.Value : 15));
        ///     }
        ///     </code>
        /// </example>
        public static PaginatedList<T> ToPaginatedList<T>(this IQueryable<T> q, int pageIndex, int pageSize)
        {
            return new PaginatedList<T>(q, pageIndex, pageSize);
        }

        /// <summary>
        /// Returns a paginated list. This function returns 15 rows from specific pageIndex.
        /// </summary>
        /// <typeparam name="T">Type of items in list.</typeparam>
        /// <param name="q">A IQueryable instance to apply.</param>
        /// <param name="pageIndex">Page number that starts with zero.</param>    
        /// <returns>Returns a paginated list.</returns>
        /// <remarks>This functionality may not work in SQL Compact 3.5</remarks>
        public static PaginatedList<T> ToPaginatedList<T>(this IQueryable<T> q, int pageIndex)
        {
            return new PaginatedList<T>(q, pageIndex, 15);
        }

        /// <summary>
        /// Returns a paginated list. This function returns 15 rows from page one.
        /// </summary>
        /// <typeparam name="T">Type of items in list.</typeparam>
        /// <param name="q">A IQueryable instance to apply.</param>    
        /// <returns>Returns a paginated list.</returns>
        /// <remarks>This functionality may not work in SQL Compact 3.5</remarks>
        public static PaginatedList<T> ToPaginatedList<T>(this IQueryable<T> q)
        {
            return new PaginatedList<T>(q, 1, 15);
        }
    }

    public static class IQueryableExtensions
    {
        ///// <summary>Projects each element of a sequence into a new form.</summary>
        ///// <returns>An <see cref="T:System.Linq.IQueryable`1" /> whose elements are the result of invoking a projection function on each element of <paramref name="source" />.</returns>
        ///// <param name="source">A sequence of values to project.</param>
        ///// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        ///// <typeparam name="TResult">The type of the value returned />.</typeparam>
        //public static IQueryable<TResult> Select<TSource, TResult>(this IQueryable<TSource> source) where TResult : new()
        //{
        //    Type destType = typeof(TResult);
        //    Type srcType = typeof(TSource);

        //    var srcArg = Expression.Parameter(srcType, "s");
        //    // new statement "new Data()"
        //    var destNew = Expression.New(destType);
        //    var sourceProperties = srcType.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
        //    var destProperties = destType.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
        //    // create initializers
        //    var destBindings = destProperties.Where(p => sourceProperties.Exists(s => s.Name == p.Name && s.GetCustomAttribute<NotMappedAttribute>() == null && (s.PropertyType == p.PropertyType || p.PropertyType.IsAssignableFrom(s.PropertyType))) && p.CanWrite).Select(o =>
        //    {

        //        // property "Field1"
        //        var srcPropertyRef = srcType.GetProperty(o.Name);

        //        // property "Field1"
        //        var destPropertyRef = destType.GetProperty(o.Name);

        //        // original value "o.Field1"
        //        var xOriginal = Expression.Property(srcArg, srcPropertyRef);
        //        return Expression.Bind(destPropertyRef, xOriginal);


        //    }
        //    );
        //    // initialization "new TResult { Field1 = o.Field1, Field2 = o.Field2 }"
        //    var xInit = Expression.MemberInit(destNew, destBindings);

        //    // expression "s => new TResult { Field1 = o.Field1, Field2 = o.Field2 }"
        //    var lambda = Expression.Lambda<Func<TSource, TResult>>(xInit, srcArg);


        //    return source.Select<TSource, TResult>(lambda);
        //}
        /// <summary>Projects each element of a sequence into a new form.</summary>
        /// <returns>An <see cref="T:System.Linq.IQueryable`1" /> whose elements are the result of invoking a projection function on each element of <paramref name="source" />.</returns>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="additionalSelector">A projection function that help to map elements between source and target that are not in same name or same type</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <typeparam name="TResult">The type of the value returned />.</typeparam>
        public static IQueryable<TResult> SelectInto<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> additionalSelector) where TResult : new()
        {


            return source.SelectInto(additionalSelector, null);
        }

        /// <summary>Projects each element of a sequence into a new form.</summary>
        /// <returns>An <see cref="T:System.Linq.IQueryable`1" /> whose elements are the result of invoking a projection function on each element of <paramref name="source" />.</returns>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="additionalSelector">A projection function that help to map elements between source and target that are not in same name or same type</param>
        /// <param name="excludedSelector">An array of excluded properties in <see cref="TResult"/> that will not map automatically</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <typeparam name="TResult">The type of the value returned />.</typeparam>
        public static IQueryable<TResult> SelectInto<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> additionalSelector, Expression<Func<TResult, object>> excludedSelector) where TResult : new()
        {
            var destType = typeof(TResult);
            var srcType = typeof(TSource);
            var excludeProperties = new List<string>();
            if (excludedSelector != null)
            {
                var newExpression = excludedSelector.Body as NewExpression;
                excludeProperties = newExpression != null
                    ? newExpression.Members.Select(m => m.Name).ToList()
                    : new List<string>() { ((MemberExpression)excludedSelector.Body).Member.Name };
            }

            var srcArg = Expression.Parameter(srcType, "s");
            // new statement "new Data()"
            var destNew = Expression.New(destType);
            var sourceProperties = srcType.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            var destProperties = destType.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            // create initializers
            var destBindings = destProperties.Where(p => sourceProperties.Exists(s => s.Name == p.Name && !excludeProperties.Contains(p.Name) && s.GetCustomAttribute<NotMappedAttribute>() == null && (s.PropertyType == p.PropertyType || p.PropertyType.IsAssignableFrom(s.PropertyType))) && p.CanWrite).Select(o =>
            {

                // property "Field1"
                var srcPropertyRef = srcType.GetProperty(o.Name);

                // property "Field1"
                var destPropertyRef = destType.GetProperty(o.Name);

                // original value "o.Field1"
                var xOriginal = Expression.Property(srcArg, srcPropertyRef);
                return Expression.Bind(destPropertyRef, xOriginal);


            }
            ).ToList();
            if (additionalSelector != null)
            {
                //firstly we update additonalSelector parameter with our srcArg parameter
                var additionalSelectorExpression = UpdateParameter(additionalSelector, srcArg);
                var additionalBody = additionalSelectorExpression.Body;
                //we iterate through each member init expression in additionalSelector expression
                if (additionalBody is MemberInitExpression)
                {
                    var additionalBindings = ((MemberInitExpression)additionalBody).Bindings;
                    additionalBindings.ForEach(b =>
                    {
                        if (b is MemberAssignment)
                        {
                            var memberAssignment = (MemberAssignment)b;


                            // property "Field1"
                            var srcPropertyExpression = memberAssignment.Expression;

                            // property "Field1"
                            var destPropertyRef = destType.GetProperty(memberAssignment.Member.Name);

                            if (destPropertyRef != null)
                            {
                                var binding = Expression.Bind(destPropertyRef, srcPropertyExpression);
                                if (destBindings.Exists(p => p.Member == destPropertyRef))
                                    destBindings.RemoveWhere(p => p.Member == destPropertyRef);
                                destBindings.Add(binding);
                            }
                        }
                    });
                }
            }
            // initialization "new TResult { Field1 = o.Field1, Field2 = o.Field2 }"
            var xInit = Expression.MemberInit(destNew, destBindings);

            // expression "s => new TResult { Field1 = o.Field1, Field2 = o.Field2 }"
            var lambda = Expression.Lambda<Func<TSource, TResult>>(xInit, srcArg);


            return source.Select(lambda);
        }
        /// <summary>Projects each element of a sequence into a new form a combined selector of <param name="selector1"></param> and <param name="selector2"></param>.</summary>
        /// <returns>An <see cref="T:System.Linq.IQueryable`1" /> whose elements are the result of invoking a projection function on each element of <paramref name="source" />.</returns>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="selector1">A projection function that help to map elements between source and target that are not in same name or same type</param>
        /// <param name="selector2">A projection function  that help to map elements between source and target that are not in same name or same type</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <typeparam name="TResult">The type of the value returned />.</typeparam>
        public static IQueryable<TResult> SelectCombine<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector1, Expression<Func<TSource, TResult>> selector2) where TResult : new()
        {
            var destType = typeof(TResult);
            var srcType = typeof(TSource);

            var srcArg = Expression.Parameter(srcType, "s");
            var destBindings = new List<MemberAssignment>();
            // new statement "new Data()"
            var destNew = Expression.New(destType);
            if (selector1 != null)
            {
                //firstly we update additonalSelector parameter with our srcArg parameter
                var selectorExpression = UpdateParameter(selector1, srcArg);
                var body = selectorExpression.Body;
                //we iterate through each member init expression in additionalSelector expression
                if (body is MemberInitExpression)
                {
                    var additionalBindings = ((MemberInitExpression)body).Bindings;
                    additionalBindings.ForEach(b =>
                    {
                        if (b is MemberAssignment)
                        {
                            var memberAssignment = (MemberAssignment)b;


                            // property "Field1"
                            var srcPropertyExpression = memberAssignment.Expression;

                            // property "Field1"
                            var destPropertyRef = destType.GetProperty(memberAssignment.Member.Name);

                            if (destPropertyRef != null)
                            {
                                var binding = Expression.Bind(destPropertyRef, srcPropertyExpression);
                                destBindings.Add(binding);
                            }
                        }
                    });
                }
            }
            if (selector2 != null)
            {
                //firstly we update additonalSelector parameter with our srcArg parameter
                var additionalSelectorExpression = UpdateParameter(selector2, srcArg);
                var additionalBody = additionalSelectorExpression.Body;
                //we iterate through each member init expression in additionalSelector expression
                if (additionalBody is MemberInitExpression)
                {
                    var additionalBindings = ((MemberInitExpression)additionalBody).Bindings;
                    additionalBindings.ForEach(b =>
                    {
                        if (b is MemberAssignment)
                        {
                            var memberAssignment = (MemberAssignment)b;


                            // property "Field1"
                            var srcPropertyExpression = memberAssignment.Expression;

                            // property "Field1"
                            var destPropertyRef = destType.GetProperty(memberAssignment.Member.Name);

                            if (destPropertyRef != null)
                            {
                                var binding = Expression.Bind(destPropertyRef, srcPropertyExpression);
                                destBindings.Add(binding);
                            }
                        }
                    });
                }
            }
            // initialization "new TResult { Field1 = o.Field1, Field2 = o.Field2 }"
            var xInit = Expression.MemberInit(destNew, destBindings);

            // expression "s => new TResult { Field1 = o.Field1, Field2 = o.Field2 }"
            var lambda = Expression.Lambda<Func<TSource, TResult>>(xInit, srcArg);


            return source.Select(lambda);
        }
        /// <summary>Projects each element of a sequence into a new form a combined selector of <param name="selector1"></param> and <param name="selector2"></param>.</summary>
        /// <returns>An <see cref="T:System.Linq.IQueryable`1" /> whose elements are the result of invoking a projection function on each element of <paramref name="source" />.</returns>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="selector1">dictionary of key value pair that map <see cref="TSource"/> property name to  <see cref="TResult"/> property name</param>
        /// <param name="selector2">A projection function  that help to map elements between source and target that are not in same name or same type</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <typeparam name="TResult">The type of the value returned />.</typeparam>
        public static IQueryable<TResult> SelectCombine<TSource, TResult>(this IQueryable<TSource> source, Dictionary<string, string> selector1, Expression<Func<TSource, TResult>> selector2) where TResult : new()
        {
            var destType = typeof(TResult);
            var srcType = typeof(TSource);

            var srcArg = Expression.Parameter(srcType, "s");
            var destBindings = new List<MemberAssignment>();
            // new statement "new Data()"
            var destNew = Expression.New(destType);
            if (selector1 != null)
            {
                // create initializers
                selector1.Select(o => new { source = o.Key.Trim(), target = o.Value.Trim() })
                    .ForEach(o =>
                        {

                            // property "Field1"
                            var srcPropertyRef = typeof(TSource).GetProperty(o.source);
                            // property "Field1"
                            var destPropertyRef = typeof(TResult).GetProperty(o.target);
                            if (srcPropertyRef == null)
                                throw new Exception(string.Format(
                                    "Cannot find a public property with the name {0} in {1}", o.source, typeof(TSource).Name));
                            if (destPropertyRef == null)
                                throw new Exception(string.Format(
                                    "Cannot find a public property with the name {0} in {1}", o.target, typeof(TResult).Name));
                            // original value "o.Field1"
                            var srcPropertyExpression = Expression.Property(srcArg, srcPropertyRef);

                            // set value "Field1 = o.Field1"
                            var binding = Expression.Bind(destPropertyRef, srcPropertyExpression);
                            destBindings.Add(binding);
                        }
                    );
            }
            if (selector2 != null)
            {
                //firstly we update additonalSelector parameter with our srcArg parameter
                var additionalSelectorExpression = UpdateParameter(selector2, srcArg);
                var additionalBody = additionalSelectorExpression.Body;
                //we iterate through each member init expression in additionalSelector expression
                if (additionalBody is MemberInitExpression)
                {
                    var additionalBindings = ((MemberInitExpression)additionalBody).Bindings;
                    additionalBindings.ForEach(b =>
                    {
                        if (b is MemberAssignment)
                        {
                            var memberAssignment = (MemberAssignment)b;


                            // property "Field1"
                            var srcPropertyExpression = memberAssignment.Expression;

                            // property "Field1"
                            var destPropertyRef = destType.GetProperty(memberAssignment.Member.Name);

                            if (destPropertyRef != null)
                            {
                                var binding = Expression.Bind(destPropertyRef, srcPropertyExpression);
                                destBindings.Add(binding);
                            }
                        }
                    });
                }
            }
            // initialization "new TResult { Field1 = o.Field1, Field2 = o.Field2 }"
            var xInit = Expression.MemberInit(destNew, destBindings);

            // expression "s => new TResult { Field1 = o.Field1, Field2 = o.Field2 }"
            var lambda = Expression.Lambda<Func<TSource, TResult>>(xInit, srcArg);


            return source.Select(lambda);
        }



        static Expression<Func<T, TResult>> UpdateParameter<T, TResult>(
            Expression<Func<T, TResult>> expr,
            ParameterExpression newParameter)
        {
            var visitor = new ParameterUpdateVisitor(expr.Parameters[0], newParameter);
            var body = visitor.Visit(expr.Body);

            return Expression.Lambda<Func<T, TResult>>(body, newParameter);
        }


    }
    class ParameterUpdateVisitor : ExpressionVisitor
    {
        private ParameterExpression _oldParameter;
        private ParameterExpression _newParameter;

        public ParameterUpdateVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            _oldParameter = oldParameter;
            _newParameter = newParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (ReferenceEquals(node, _oldParameter))
                return _newParameter;

            return base.VisitParameter(node);
        }
    }

}