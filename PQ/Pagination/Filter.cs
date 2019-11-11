using Abp.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PQ.Pagination
{
    internal class TrueSpecification<T> : ISpecification<T>
    {

        public bool IsSatisfiedBy(T obj)
        {
            return true;
        }

        public Expression<Func<T, bool>> ToExpression()
        {
            return x => true;
        }
    }
}
