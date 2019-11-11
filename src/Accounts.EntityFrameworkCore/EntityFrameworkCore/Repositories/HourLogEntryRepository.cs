using Abp.Domain.Repositories;
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
    }
    public class HourLogEntryRepository : AccountsRepositoryBase<HourLogEntry>, IHourLogEntryRepository
    {

        public HourLogEntryRepository(IDbContextProvider<AccountsDbContext> dbContextProvider) : base(dbContextProvider) { }

        public IQueryable<HourLogEntry> GetHourLogEntriesByProjectIdAsync(int projectId, DateTime startDt, DateTime endDt)
        {
            return GetAll().Where(p => p.Day >= startDt && p.Day <= endDt);
        }


    }
}
