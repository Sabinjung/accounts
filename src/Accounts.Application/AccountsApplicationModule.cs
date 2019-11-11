using Abp.AutoMapper;
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
                    cfg.CreateMap<IntuitData.Customer, Company>()
                        .ForMember("ExternalCustomerId", x => x.MapFrom(y => y.Id))
                        .ForMember("Id", x => x.Ignore());

                    cfg.CreateMap<IntuitData.Term, Term>()
                        .ForMember("ExternalTermId", x => x.MapFrom(y => y.Id))
                        .ForMember("Id", x => x.Ignore());

                    cfg.CreateMap<Timesheet, TimesheetDto>()
                        .ForMember("CreatedDt", x => x.MapFrom(y => y.CreationTime));

                    cfg.CreateMap<Timesheet, TimesheetListItemDto>()
                        .ForMember("CreatedDt", x => x.MapFrom(y => y.CreationTime));
   

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
