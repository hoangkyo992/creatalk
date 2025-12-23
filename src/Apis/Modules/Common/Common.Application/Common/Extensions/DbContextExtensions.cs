using System.Linq.Expressions;

namespace Common.Application.Common.Extensions;

public static class DbContextExtensions
{
    /// <summary>
    /// Projects each element of a sequence into a new form. (OrderByDescending CreatedDate)
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by the function represented by selector.</typeparam>
    /// <param name="source">A sequence of values to project.</param>
    /// <param name="selector">A projection function to apply to each element.</param>
    /// <returns>An System.Linq.IQueryable`1 whose elements are the result of invoking a projection function on each element of source.</returns>
    public static IQueryable<TResult> SelectWithDefaultOrder<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
        where TSource : BaseEntity
    {
        return source.OrderByDescending(c => c.Id).Select(selector);
    }

    /// <summary>
    /// Projects each element of a sequence into a new form by incorporating the element's index. (OrderByDescending CreatedDate)
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
    /// <param name="source">A sequence of values to invoke a transform function on.</param>
    /// <param name="selector">A transform function to apply to each source element; the second parameter of the function represents the index of the source element.</param>
    /// <returns>An System.Collections.Generic.IEnumerable`1 whose elements are the result of invoking the transform function on each element of source.</returns>
    public static IQueryable<TResult> SelectWithDefaultOrder<TSource, TResult>(this DbSet<TSource> source, Expression<Func<TSource, TResult>> selector)
        where TSource : BaseEntity
    {
        return source.OrderByDescending(c => c.Id).Select(selector);
    }
}