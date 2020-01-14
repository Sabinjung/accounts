using Abp.Specifications;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PQ.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PQ
{
    public enum SpecificationType
    {
        And,
        Or,
        Not
    }

    public class QueryBuilder<TDataModel, TQueryParameter> where TQueryParameter : NamedQueryParameter
    {
        private readonly IQueryable<TDataModel> OriginalQuery;

        private IQueryable<TDataModel> CompileQuery(TQueryParameter queryParameter)
        {
            ISpecification<TDataModel> ConsturctSpecification(TQueryParameter p)
            {
                return Specifications.Aggregate(RootSpecification, (s, t) =>
                {
                    switch (t.Item1)
                    {
                        case SpecificationType.And:
                            return s.And(t.Item2(queryParameter));

                        case SpecificationType.Or:
                            return s.Or(t.Item2(queryParameter));

                        default:
                            return s;
                    }
                });
            }

            var tempSpec = ConsturctSpecification(queryParameter);
            return Query.Where(tempSpec.ToExpression());
        }

        public readonly ISpecification<TDataModel> RootSpecification;

        public readonly List<(SpecificationType, Func<TQueryParameter, ISpecification<TDataModel>>)> Specifications;

        public readonly List<TQueryParameter> PreDefinedQuerParameters;

        private readonly IMapper Mapper;

        public IQueryable<TDataModel> Query { get; private set; }

        public QueryBuilder(IQueryable<TDataModel> query, IMapper mapper) : this(query, mapper, new AnySpecification<TDataModel>())
        {
        }

        public QueryBuilder(IQueryable<TDataModel> query, IMapper mapper, ISpecification<TDataModel> specification)
        {
            OriginalQuery = query;
            Query = query;
            RootSpecification = specification;
            Mapper = mapper;
            Specifications = new List<(SpecificationType, Func<TQueryParameter, ISpecification<TDataModel>>)>();
            PreDefinedQuerParameters = new List<TQueryParameter>();
        }

        public QueryBuilder<TDataModel, TQueryParameter> Where(Func<TQueryParameter, Expression<Func<TDataModel, bool>>> specFun)
        {
            return this.And(x => new ExpressionSpecification<TDataModel>(specFun(x)));
        }

        public QueryBuilder<TDataModel, TQueryParameter> WhereIf(Predicate<TQueryParameter> condition, Func<TQueryParameter, Expression<Func<TDataModel, bool>>> specFun)
        {
            return this.And(x => new ConditionalExpressionSpecification<TDataModel>(condition(x), specFun(x)));
        }

        public QueryBuilder<TDataModel, TQueryParameter> And(Func<TQueryParameter, ISpecification<TDataModel>> specFun)
        {
            Specifications.Add((SpecificationType.And, specFun));
            return this;
        }

        public QueryBuilder<TDataModel, TQueryParameter> Or(Func<TQueryParameter, ISpecification<TDataModel>> specFun)
        {
            Specifications.Add((SpecificationType.And, specFun));
            return this;
        }

        public QueryBuilder<TDataModel, TQueryParameter> Not(Func<TQueryParameter, ISpecification<TDataModel>> specFun)
        {
            Specifications.Add((SpecificationType.And, specFun));
            return this;
        }

        public QueryBuilder<TDataModel, TQueryParameter> ApplySorts(Sorts<TDataModel> sorts)
        {
            if (sorts.IsValid())
            {
                Query = Sorts<TDataModel>.ApplySorts(Query, sorts);
            }
            return this;
        }

        public async Task<Page<TDataTransferObject>> ExecuteAsync<TDataTransferObject>(params TQueryParameter[] queryParameters)
        {
            var primaryQueryParameter = queryParameters.Length == 1 ? queryParameters.First() : queryParameters.First(x => x.IsActive);
            var pageNumber = primaryQueryParameter.PageNumber;
            var pageSize = primaryQueryParameter.PageSize;

            if (!pageSize.HasValue) pageSize = 100;

            var compiledQuery = CompileQuery(primaryQueryParameter);
            var projectedQuery = Mapper.ProjectTo<TDataTransferObject>(compiledQuery);

            var recordCounts = await Task.WhenAll(queryParameters.Select(async x => new PreDefinedQueryCount
            {
                Name = x.Name,
                Count = await CompileQuery(x).CountAsync()
            }));
            var result = new Page<TDataTransferObject>
            {
                CurrentPage = pageNumber.HasValue ? pageNumber.Value : 0,
                PageSize = pageSize.Value,
                TotalCount = OriginalQuery.Count(),
                RecordCount = compiledQuery.Count(),
                Results = pageNumber.HasValue ? await projectedQuery.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value).ToListAsync() : await projectedQuery.ToListAsync(),
                RecordCounts = recordCounts
            };
            result.PageCount = pageNumber.HasValue ? (int)Math.Ceiling((double)result.RecordCount / pageSize.Value) : result.TotalCount;
            return result;
        }

        public async Task<IEnumerable<TDataTransferObject>> ExecuteAsync<TDataTransferObject>(
           Func<IQueryable<TDataModel>, IQueryable<TDataTransferObject>> mapQuery,
           params TQueryParameter[] queryParameters)
        {
            var primaryQueryParameter = queryParameters.Length == 1 ? queryParameters.First() : queryParameters.First(x => x.IsActive);
            var compiledQuery = CompileQuery(primaryQueryParameter);
            var finalQuery = mapQuery(compiledQuery);
            return await finalQuery.ToListAsync();
        }
    }
}