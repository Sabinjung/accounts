using Accounts.Data;
using Accounts.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounts.EntityFrameworkCore.Seed
{
    public class CodeValueBuilder
    {
        private readonly AccountsDbContext _context;

        public CodeValueBuilder(AccountsDbContext context)
        {
            _context = context;
        }

        public void Generate()
        {
            GenerateCodes<InvoiceCycles, InvoiceCycle>();
            GenerateCodes<TimesheetStatuses, TimesheetStatus>();

        }

        private void GenerateCodes<T, TEntity>() where T : struct, IConvertible
            where TEntity : CodeBaseEntity, new()
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            foreach (int id in Enum.GetValues(typeof(T)))
            {
                var set = _context.Set<TEntity>();
                var name = Enum.GetName(typeof(T), id);
                var code = set.IgnoreQueryFilters().FirstOrDefault(t => t.Name == name);
                if (code == null)
                {
                    code = new TEntity { Name = name };
                    set.Add(code);
                }
            }
            _context.SaveChanges();

        }
    }
}
