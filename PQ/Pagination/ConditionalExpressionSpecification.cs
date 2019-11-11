using Abp.Specifications;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PQ.Pagination
{
    public class ConditionalExpressionSpecification<TDataModel> : Specification<TDataModel>
    {
        public bool Condition { get; }

        public Expression<Func<TDataModel, bool>> Predicate { get; }


        public ConditionalExpressionSpecification(bool condition, Expression<Func<TDataModel, bool>> predicate)
        {
            Condition = condition;
            Predicate = predicate;

        }

        public override Expression<Func<TDataModel, bool>> ToExpression()
        {
           
            return Condition ? Predicate : x => true;
        }
    }
}
