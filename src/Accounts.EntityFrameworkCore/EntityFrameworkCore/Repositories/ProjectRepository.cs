using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using Accounts.Models;
using PQ;
using PQ.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.EntityFrameworkCore.Repositories
{
    public interface IProjectRepository : IRepository<Project>
    {
        IQueryable<Project> QueryActiveProjects(DateTime startDt, DateTime endDt);
        QueryBuilder<Project, ProjectHourLogsQueryParameter> PagedQueryActiveProjectsByDate(DateTime startDt, DateTime endDt, ProjectHourLogsQueryParameter hourLogsQueryParameter);
        IQueryable<Project> QueryActiveProjectsByDate(DateTime startDt, DateTime endDt);

    }

    public class ProjectRepository : AccountsRepositoryBase<Project>, IProjectRepository
    {
        private readonly QueryBuilderFactory QueryBuilderFactory;

        public ProjectRepository(IDbContextProvider<AccountsDbContext> dbContextProvider,QueryBuilderFactory queryBuilder) : base(dbContextProvider)
        {
            QueryBuilderFactory = queryBuilder;
        }

        public Task<IEnumerable<HourLogEntry>> GetHourLogsAsync(int id, DateTime startDt, DateTime endDt)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Project> QueryActiveProjects(DateTime startDt, DateTime endDt) //01/01/2020 -01/31/2020
        {
            return GetAll().Where(p => ((p.StartDt >= startDt && p.StartDt <= endDt)
                                || ((p.EndDt.HasValue && p.EndDt >= startDt && p.StartDt >= startDt))
                                || (p.StartDt <= startDt && (!p.EndDt.HasValue))
                                || (p.StartDt <= startDt && (p.EndDt >= startDt)))
                                && (p.EndDt.HasValue ? p.EndDt > DateTime.UtcNow : true) == true);//checks for only active projects
        }
        public QueryBuilder<Project, ProjectHourLogsQueryParameter> PagedQueryActiveProjectsByDate(DateTime startDt, DateTime endDt,
                                                                ProjectHourLogsQueryParameter hourLogsQueryParameter) //01/01/2020 -01/31/2020
        {
            var projects = GetAllIncluding(x=>x.Consultant).Where(p => p.StartDt <= endDt && (!p.EndDt.HasValue || p.EndDt >= startDt));
            var pagedQuery = QueryBuilderFactory.Create<Project, ProjectHourLogsQueryParameter>(projects)
                .WhereIf(p => !string.IsNullOrEmpty(p.SearchText), p => x => x.Consultant.DisplayName.ToLower().Contains(p.SearchText.ToLower()) 
                || x.Company.DisplayName.ToLower().Contains(p.SearchText.ToLower()))
                .WhereIf(x => x.ProjectId.HasValue, p => x => x.Id == p.ProjectId)
                .WhereIf(x => x.ConsultantId.HasValue, p => x => x.ConsultantId == p.ConsultantId);
            ;
            var sorts = new Sorts<Project>();
            sorts.Add(true, c => c.Consultant.DisplayName);
            return pagedQuery.ApplySorts(sorts);

        }

        public IQueryable<Project> QueryActiveProjectsByDate(DateTime startDt, DateTime endDt) //01/01/2020 -01/31/2020
        {
            return GetAll().Where(p => p.StartDt <= endDt && (!p.EndDt.HasValue || p.EndDt >= startDt));

        }
    }
}