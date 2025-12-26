using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HappyTools.CrossCutting.Patterns.Specification
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; }
    }

    public abstract class Specification<T> : ISpecification<T>
    {
        public Expression<Func<T, bool>> Criteria { get; protected set; }
    }
    // And / Or combinators
    public static class SpecificationExtensions
    {
        public static Specification<T> And<T>(
            this Specification<T> left,
            Specification<T> right)
        {
            var param = Expression.Parameter(typeof(T));

            var body = Expression.AndAlso(
                Expression.Invoke(left.Criteria, param),
                Expression.Invoke(right.Criteria, param));

            return new InlineSpecification<T>(
                Expression.Lambda<Func<T, bool>>(body, param));
        }
    }

    public sealed class InlineSpecification<T> : Specification<T>
    {
        public InlineSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }
    }
}
