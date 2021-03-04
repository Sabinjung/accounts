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
using Accounts.Projects;
using System.Linq;
using Accounts.Companies.Dto;
using Accounts.Invoicing.Dto;
using Abp.FluentValidation;
using System;
using Accounts.Intuit.Dto;
using Accounts.Intuit;
using Accounts.Expenses.Dto;

namespace Accounts
{
    [DependsOn(
        typeof(AbpFluentValidationModule),
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
                       .ForMember("Email", x => x.MapFrom(y => y.Consultant.Email))
                       .ForMember("PhoneNumber", x => x.MapFrom(y => y.Consultant.PhoneNumber))
                       .ForMember("CompanyName", x => x.MapFrom(y => y.Company.DisplayName))
                       .ForMember("EndClientName", x => x.MapFrom(y => y.EndClient.ClientName))
                       .ForMember("TotalHoursBilled", x => x.MapFrom(y => y.Invoices.Sum(z => z.TotalHours)))
                       .ForMember("TotalAmountBilled", x => x.MapFrom(y => y.Invoices.Sum(z => z.Total)))
                       .ForMember("InvoiceCycleName", x => x.MapFrom(y => y.InvoiceCycle.Name));

                    cfg.CreateMap<Project, ProjectListItemDto>()
                       .ForMember("ConsultantName", x => x.MapFrom(y => $"{y.Consultant.FirstName} {y.Consultant.LastName}"))
                       .ForMember("CompanyName", x => x.MapFrom(y => y.Company.DisplayName))
                       .ForMember("EndClientName", x => x.MapFrom(y => y.EndClient.ClientName))
                       .ForMember("TotalHoursBilled", x => x.MapFrom(y => y.Invoices.Sum(z => z.TotalHours)))
                       .ForMember("TotalAmountBilled", x => x.MapFrom(y => y.Invoices.Sum(z => z.Total)))
                        .ForMember("TermName", x => x.MapFrom(y => y.Term.Name))
                        .ForMember("InvoiceCycleName", x => x.MapFrom(y => y.InvoiceCycle.Name));

                    cfg.CreateMap<Invoice, IncoiceListItemDto>()
                       .ForMember(x => x.ConsultantName, y => y.MapFrom(z => $"{z.Consultant.FirstName} {z.Consultant.LastName}"))
                       .ForMember(x => x.CompanyEmail, y => y.MapFrom(z => z.Company.Email.Split(",", StringSplitOptions.None).ToList()))
                       .ForMember(x => x.CompanyPhoneNumber, y => y.MapFrom(z => z.Company.PhoneNumber))
                       .ForMember(x => x.CompanyName, y => y.MapFrom(z => $"{z.Company.DisplayName}"));

                    cfg.CreateMap<Invoice, Children>()
                       .ForMember(x => x.ConsultantName, y => y.MapFrom(z => $"{z.Consultant.FirstName} {z.Consultant.LastName}"))
                       .ForMember(x => x.CompanyName, y => y.MapFrom(z => $"{z.Company.DisplayName}"));

                    cfg.CreateMap<Project, UnsyncedProjectData>()
                        .ForMember("FirstName", x => x.MapFrom(y => y.Consultant.FirstName))
                        .ForMember("LastName", x => x.MapFrom(y => y.Consultant.LastName))
                        .ForMember("CompanyName", x => x.MapFrom(y => y.Company.DisplayName))
                        .ForMember("StartDate", x => x.MapFrom(y => y.StartDt))
                        .ForMember("EndDate", x => x.MapFrom(y => y.EndDt));

                    cfg.CreateMap<Project, IhrmsProjectDto>()
                        .ForMember("FirstName", x => x.MapFrom(y => y.Consultant.FirstName))
                        .ForMember("LastName", x => x.MapFrom(y => y.Consultant.LastName))
                        .ForMember("CompanyName", x => x.MapFrom(y => y.Company.DisplayName))
                        .ForMember("StartDate", x => x.MapFrom(y => y.StartDt))
                        .ForMember("EndDate", x => x.MapFrom(y => y.EndDt));



                    cfg.CreateMap<ProjectDto, Project>()
                        .ForMember("StartDt", x => x.MapFrom(y => y.StartDt.Date))
                        .ForMember("EndDt", x => x.MapFrom(y => y.EndDt.HasValue ? y.EndDt.Value.Date : y.EndDt))
                        .ForMember("InvoiceCycleStartDt", x => x.MapFrom(y => y.InvoiceCycleStartDt.Date));

                    cfg.CreateMap<IntuitData.Customer, Company>()
                        .ForMember("ExternalCustomerId", x => x.MapFrom(y => y.Id))
                        .ForMember("Id", x => x.Ignore())
                        .ForMember("Email", x => x.MapFrom(y => y.PrimaryEmailAddr != null ? y.PrimaryEmailAddr.Address : string.Empty))
                         .ForMember("PhoneNumber", x => x.MapFrom(y => y.PrimaryPhone != null ? y.PrimaryPhone.FreeFormNumber : string.Empty));

                    cfg.CreateMap<IntuitCompanyDto, IntuitData.Customer>()
                      .ConvertUsing<ConvertCompanyToCustomer>();

                    cfg.CreateMap<IntuitData.Term, Term>()
                        .ForMember("ExternalTermId", x => x.MapFrom(y => y.Id))
                        .ForMember("Id", x => x.Ignore());

                    cfg.CreateMap<IntuitData.PaymentMethod, PaymentMethod>()
                        .ForMember("ExternalPaymentId", x => x.MapFrom(y => y.Id))
                        .ForMember("Id", x => x.Ignore());

                    cfg.CreateMap<IntuitData.Invoice, Invoice>()
                        .ForMember("Id", x => x.Ignore())
                        .ForMember("TermId", x => x.Ignore())
                        .ForMember("Memo", x => x.Ignore())
                        .ForMember("Description", x => x.Ignore())
                        .ForMember("InvoiceDate", x => x.Ignore())
                        .ForMember("DueDate", x => x.Ignore())
                        .ForMember("TotalHours", x => x.Ignore())
                        .ForMember("Rate", x => x.Ignore())
                        .ForMember("ServiceTotal", x => x.Ignore())
                        .ForMember("SubTotal", x => x.Ignore())
                        .ForMember("DiscountType", x => x.Ignore())
                        .ForMember("DiscountValue", x => x.Ignore())
                        .ForMember("Total", x => x.Ignore())
                        .ForMember("QBOInvoiceId", x => x.Ignore())
                        .ForMember("ConsultantId", x => x.Ignore())
                        .ForMember("ProjectId", x => x.Ignore())
                        .ForMember("CompanyId", x => x.Ignore())
                        .ForMember("DiscountAmount", x => x.Ignore())
                        .ForMember("LastModificationTime", x => x.Ignore())
                        .ForMember("LastModifierUserId", x => x.Ignore())
                        .ForMember("DeletionTime", x => x.Ignore())
                        .ForMember("DeleterUserId", x => x.Ignore());

                    cfg.CreateMap<Timesheet, TimesheetDto>()
                        .ForMember("CreatedDt", x => x.MapFrom(y => y.CreationTime))
                        .ForMember("CreatedByUserName", x => x.MapFrom(y => y.CreatorUser.FullName))
                        .ForMember("ApprovedByUserName", x => x.MapFrom(y => y.ApprovedByUser.FullName))
                        .ForMember("QBInvoiceId", x => x.MapFrom(y => y.Invoice.QBOInvoiceId))
                        .ForMember("InvoiceGeneratedByUserName", x => x.MapFrom(y => y.InvoiceGeneratedByUser.FullName));

                    cfg.CreateMap<Timesheet, TimesheetListItemDto>()
                        .ForMember("CreatedDt", x => x.MapFrom(y => y.CreationTime))
                        .ForMember("CreatedByUserName", x => x.MapFrom(y => y.CreatorUser.FullName))
                        .ForMember("ApprovedByUserName", x => x.MapFrom(y => y.ApprovedByUser.FullName))
                        .ForMember("QBOInvoiceId", x => x.MapFrom(y => y.InvoiceId.HasValue ? y.Invoice.QBOInvoiceId : string.Empty))
                        .ForMember("EInvoiceId", x => x.MapFrom(y => y.InvoiceId.HasValue ? y.Invoice.EInvoiceId : string.Empty));

                    cfg.CreateMap<Invoice, InvoiceDto>()
                        .ForMember(x => x.TermName, y => y.MapFrom(z => z.Term.Name))
                        .ForMember(x => x.ConsultantName, y => y.MapFrom(z => $"{z.Consultant.FirstName} {z.Consultant.LastName}"))
                        .ForMember(x => x.CompanyName, y => y.MapFrom(z => $"{z.Company.DisplayName}"))
                        .ForMember(x => x.CompanyEmail, y => y.MapFrom(z => $"{z.Company.Email}"))
                        .ForMember(x => x.IsSendMail, y => y.MapFrom(z => $"{z.Project.IsSendMail}"));

                    cfg.CreateMap<HourLogEntryDto, HourLogEntry>()
                        .ForMember("Day", x => x.MapFrom(y => y.Day.Date));

                    cfg.CreateMap<TimesheetQueryParameters, TimesheetQueryParameters>()
                        .ForMember(d => d.Name, x => x.Ignore())
                        .ForMember(d => d.IsActive, x => x.Condition((source, dest) => source.Name == dest.Name))
                        .ForAllOtherMembers(opt => opt.Condition((source, dest, sourceMember, destmember) => destmember == null));

                    cfg.CreateMap<ProjectQueryParameters, ProjectQueryParameters>()
                        .ForMember(d => d.Name, x => x.Ignore())
                        .ForMember(d => d.IsActive, x => x.Condition((source, dest) => source.Name == dest.Name))
                        .ForAllOtherMembers(opt => opt.Condition((source, dest, sourceMember, destmember) => destmember == null));

                    cfg.CreateMap<InvoiceQueryParameter, InvoiceQueryParameter>()
                       .ForMember(d => d.Name, x => x.Ignore())
                       .ForMember(d => d.IsActive, x => x.Condition((source, dest) => source.Name == dest.Name))
                       .ForAllOtherMembers(opt => opt.Condition((source, dest, sourceMember, destmember) => destmember == null));

                    cfg.CreateMap<Company, CompanyDto>()
                        .ForMember(d => d.TermName, x => x.MapFrom(y => y.Term != null ? y.Term.Name : string.Empty));

                    cfg.CreateMap<LineItem, LineItemDto>()
                        .ForMember(x => x.ExpenseTypeName, x => x.MapFrom(y => y.ExpenseType.Name));
                    
                    cfg.CreateMap<Expense, ExpenseDto>()
                        .ForMember(x => x.ExpenseTypeName, x => x.MapFrom(y => y.ExpenseType.Name));

                    cfg.AddMaps(thisAssembly);
                }

            );
        }
    }
}