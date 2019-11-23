﻿using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Accounts.Authorization;
using Accounts.Projects.Dto;
using Accounts.HourLogEntries.Dto;
using Accounts.Models;
using IntuitData = Intuit.Ipp.Data;
using Accounts.Timesheets.Dto;
using PQ.Pagination;

namespace Accounts
{
    [DependsOn(
        typeof(AccountsCoreModule),
        typeof(AbpAutoMapperModule))]
    public class AccountsApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<AccountsAuthorizationProvider>();
            Configuration.Settings.Providers.Add<AccountsSettingsProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(AccountsApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg =>
                {
                    cfg.CreateMap<Project, ProjectDto>()
                        .ForMember("ConsultantName", x => x.MapFrom(y => $"{y.Consultant.FirstName} {y.Consultant.LastName}"))
                        .ForMember("CompanyName", x => x.MapFrom(y => y.Company.DisplayName));


                    cfg.CreateMap<ProjectDto, Project>()
                        .ForMember("StartDt", x => x.MapFrom(y => y.StartDt.Date))
                        .ForMember("EndDt", x => x.MapFrom(y => y.EndDt.HasValue ? y.EndDt.Value.Date : y.EndDt));

                    cfg.CreateMap<IntuitData.Customer, Company>()
                        .ForMember("ExternalCustomerId", x => x.MapFrom(y => y.Id))
                        .ForMember("Id", x => x.Ignore());

                    cfg.CreateMap<IntuitData.Term, Term>()
                        .ForMember("ExternalTermId", x => x.MapFrom(y => y.Id))
                        .ForMember("Id", x => x.Ignore());

                    cfg.CreateMap<Timesheet, TimesheetDto>()
                        .ForMember("CreatedDt", x => x.MapFrom(y => y.CreationTime))
                        .ForMember("CreatedByUserName", x => x.MapFrom(y => y.CreatorUser.FullName))
                        .ForMember("ApprovedByUserName", x => x.MapFrom(y => y.ApprovedByUser.FullName))
                        .ForMember("QBInvoiceId", x => x.MapFrom(y => y.Invoice.QBOInvoiceId))
                        .ForMember("InvoiceGeneratedByUserName", x => x.MapFrom(y => y.InvoiceGeneratedByUser.FullName));


                    cfg.CreateMap<Timesheet, TimesheetListItemDto>()
                        .ForMember("CreatedDt", x => x.MapFrom(y => y.CreationTime))
                        .ForMember("CreatedByUserName", x => x.MapFrom(y => y.CreatorUser.FullName))
                        .ForMember("ApprovedByUserName", x => x.MapFrom(y => y.ApprovedByUser.FullName));

                    cfg.CreateMap<HourLogEntryDto, HourLogEntry>()
                        .ForMember("Day", x => x.MapFrom(y => y.Day.Date));


                    cfg.CreateMap<TimesheetQueryParameters, TimesheetQueryParameters>()
                        .ForMember(d => d.Name, x => x.Ignore())
                        .ForMember(d => d.IsActive, x => x.Condition((source, dest) => source.Name == dest.Name))
                        .ForAllOtherMembers(opt => opt.Condition((source, dest, sourceMember, destmember) => destmember == null));


                    cfg.AddMaps(thisAssembly);

                }

            );
        }
    }
}
