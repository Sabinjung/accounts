﻿using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using Accounts.Authorization.Roles;
using Accounts.Authorization.Users;
using Accounts.MultiTenancy;
using Accounts.Models;
using Accounts.Data.Models;

namespace Accounts.EntityFrameworkCore
{
    public class AccountsDbContext : AbpZeroDbContext<Tenant, Role, User, AccountsDbContext>
    {
        /* Define a DbSet for each entity of the application */

        public DbSet<Consultant> Consultants { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Company> Companies { get; set; }

        public DbSet<Term> Terms { get; set; }

        public DbSet<PaymentMethod> PaymentMethods { get; set; }

        public DbSet<HourLogEntry> HourLogEntries { get; set; }

        public DbSet<Timesheet> Timesheets { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<TimesheetPeriod> TimesheetPeriods { get; set; }

        public DbSet<TimesheetStatus> TimesheetStatuses { get; set; }

        public DbSet<Attachment> Attachments { get; set; }

        public DbSet<InvoiceCycle> InvoiceCycles { get; set; }

        public DbSet<LineItem> LineItems { get; set; }

        public DbSet<Note> Notes { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExpenseType> ExpenseTypes { get; set; }
        public DbSet<EndClient> EndClients { get; set; }
        public DbSet<Config> Configs { get; set; }
        public DbSet<ConfigType> ConfigTypes { get; set; }

        public DbSet<IntuitWebhookLog> IntuitWebhookLogs { get; set; }
        public DbSet<Fieldglass> Fieldglasses { get; set; }

        public AccountsDbContext(DbContextOptions<AccountsDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

    }
}
