﻿using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using Accounts.Models;
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
    }

    public class ProjectRepository : AccountsRepositoryBase<Project>, IProjectRepository
    {
        public ProjectRepository(IDbContextProvider<AccountsDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public Task<IEnumerable<HourLogEntry>> GetHourLogsAsync(int id, DateTime startDt, DateTime endDt)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Project> QueryActiveProjects(DateTime startDt, DateTime endDt)
        {
            return GetAll().Where(p => ((p.StartDt >= startDt && p.StartDt <= endDt)
                                || ((p.EndDt.HasValue && p.EndDt >= startDt && p.StartDt >= startDt))
                                || (p.StartDt <= startDt && (!p.EndDt.HasValue))
                                || (p.StartDt <= startDt && (p.EndDt >= startDt)))
                                && (p.EndDt.HasValue ? p.EndDt > DateTime.UtcNow : true) == true);//checks for only active projects
        }
    }
}