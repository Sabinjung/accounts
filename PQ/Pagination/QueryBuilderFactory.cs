using Abp.Dependency;
using Abp.Specifications;
using AutoMapper;
using PQ.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PQ
{
    public class QueryBuilderFactory : ISingletonDependency
    {
        private readonly IMapper Mapper;
        public QueryBuilderFactory(IMapper mapper)
        {
            Mapper = mapper;
        }
        public QueryBuilder<TDataModel, TQueryParameter> Create<TDataModel, TQueryParameter>(IQueryable<TDataModel> query)
            where TQueryParameter : NamedQueryParameter
        {
            return new QueryBuilder<TDataModel, TQueryParameter>(query, Mapper);
        }

        public QueryBuilder<TDataModel, TQueryParameter> Create<TDataModel, TQueryParameter>(IQueryable<TDataModel> query, ISpecification<TDataModel> specification)
             where TQueryParameter : NamedQueryParameter
        {
            return new QueryBuilder<TDataModel, TQueryParameter>(query, Mapper, specification);
        }
    }
}
