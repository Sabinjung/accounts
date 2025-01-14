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
    public interface IHourLogEntryRepository : IRepository<HourLogEntry>
    {
        IQueryable<HourLogEntry> GetHourLogEntriesByProjectIdAsync(int projectId, DateTime startDt, DateTime endDt);
        IQueryable<HourLogEntry> GetAllHourLogEntriesByProjectIdAsync(int projectId, DateTime startDt);
    }
    public class HourLogEntryRepository : AccountsRepositoryBase<HourLogEntry>, IHourLogEntryRepository
    {

        public HourLogEntryRepository(IDbContextProvider<AccountsDbContext> dbContextProvider) : base(dbContextProvider) { }

        public IQueryable<HourLogEntry> GetHourLogEntriesByProjectIdAsync(int projectId, DateTime startDt, DateTime endDt)
        {
            var startDateOnly = startDt.Date;
            var endDateOnly = endDt.Date;
            return GetAll().Where(p => p.ProjectId == projectId && p.Day >= startDateOnly && p.Day <= endDateOnly);
        }

        public IQueryable<HourLogEntry> GetAllHourLogEntriesByProjectIdAsync(int projectId, DateTime startDt)
        {
            var startDateOnly = startDt.Date;
            return GetAll().Where(p => p.ProjectId == projectId && p.Day >= startDateOnly);
        }
    }
}
