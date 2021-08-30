using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using Accounts.Data;
using Accounts.Models;
using Accounts.Notify;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Accounts.Core.Notify
{
    public class NotifyService : DomainService, INotifyService
    {
        private readonly IRepository<HourLogEntry> HourlogRepository;
        private readonly IRepository<Consultant> ConsultantRepository;
        private readonly IRepository<Company> CompanyRepository;
        private readonly IRepository<Timesheet> TimesheetRepository;
        private readonly IRepository<Invoice> InvoiceRepository;
        private readonly IRepository<Config> ConfigRepository;
        private readonly IConfiguration Configuration;

        public NotifyService (IRepository<HourLogEntry> hourlogRepository,
            IRepository<Consultant> consultantRepository,
            IRepository<Company> companyRepository,
            IRepository<Invoice> invoiceRepository,
            IRepository<Timesheet> timesheetRepository,
            IRepository<Config> configRepository,
            IConfiguration configuration)
        {
            HourlogRepository = hourlogRepository;
            ConsultantRepository = consultantRepository;
            CompanyRepository = companyRepository;
            TimesheetRepository = timesheetRepository;
            InvoiceRepository = invoiceRepository;
            ConfigRepository = configRepository;
            Configuration = configuration;
        }
        public async Task<string> NotifyInvoice(string invoiceId , string message)
        {
            var invoiceUrl = Configuration.GetSection("App:ServerRootAddress").Value;
            var notificationSource = Configuration.GetSection("RingCentralNotification:NotificationSource").Value;
            var databaseInvoice = InvoiceRepository.FirstOrDefault(x => x.EInvoiceId == invoiceId);
            var companyName = CompanyRepository.FirstOrDefault(x => x.Id == databaseInvoice.CompanyId).DisplayName;
            var consultantName = ConsultantRepository.FirstOrDefault(x => x.Id == databaseInvoice.ConsultantId).DisplayName;
            var teamId = ConfigRepository.GetAllList().Where(x => x.ConfigTypeId == (int)ConfigTypes.RCBot).Select(x => x.Data).FirstOrDefault();
            TeamNotificationDto notify = new TeamNotificationDto
            {
                TeamId = teamId,
                TeamName = "SutraBot",
                Message = $"Invoice has been {message}.\n" +
                $" Date: {DateTime.UtcNow.Date.ToString(" MM/dd/yyyy")}\n" +
                $" Customer Name: {companyName}\n" +
                $" Consultant Name: {consultantName}\n" +
                $" eInvoice ID:{invoiceId}\n" +
                $" Invoice link to Accounts application: {invoiceUrl + "invoices/" + databaseInvoice.Id + "\n"}\n",
                From = notificationSource
            };
            await SendNotification(notify);

            return "User Notified";
        }
        public async Task<string> NotifyPayment(string message)
        {
            var teamId = ConfigRepository.GetAllList().Where(x => x.ConfigTypeId == (int)ConfigTypes.RCChannel).Select(x => x.Data).FirstOrDefault();
            var notificationSource = Configuration.GetSection("RingCentralNotification:NotificationSource").Value;
            TeamNotificationDto notify = new TeamNotificationDto
            {
                TeamId = teamId,
                TeamName = "ArFollowUp",
                Message = message,
                From = notificationSource
            };
            await SendNotification(notify);
            return "User Notified";
        }

        public async Task<string> SendNotification(TeamNotificationDto param)
        {
            var client = new HttpClient();
            var emailAddress = ConfigRepository.GetAllList().Where(x => x.ConfigTypeId == (int)ConfigTypes.NotificationEmail).Select(x => x.Data).ToList();
            var baseUrl = ConfigRepository.GetAllList().Where(x => x.ConfigTypeId == (int)ConfigTypes.BaseUrl).Select(x => x.Data).FirstOrDefault();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage res = await client.PostAsJsonAsync("api/CreateTeamNotification",param);

            if (!res.IsSuccessStatusCode)
            {
                throw new UserFriendlyException("User notify failed.");
            }

            return "User Notified";
        }
        public async Task<string> NotifyUser()
        {
            var client = new HttpClient();
            var notify = new NotifyParam();
            var notificationSource = Configuration.GetSection("RingCentralNotification:NotificationSource").Value;
            var unassociatedTimesheets = TimesheetRepository.GetAllList().Where(x => x.InvoiceId == null).Select(x => x.Id).ToList();
            var unassociatedHoursProjects = HourlogRepository.GetAllIncluding(x=>x.Project)
                                            .Where(x => (x.TimesheetId == null || unassociatedTimesheets
                                            .Contains(x.TimesheetId.Value))&& x.Day < DateTime.Now.AddDays(-45) && x.Day >= x.Project.InvoiceCycleStartDt)
                                            .ToList()
                                            .OrderBy(x => x.CreationTime)
                                            .GroupBy(y => y.ProjectId).Select(z => z.Key);
            var emailAddress = ConfigRepository.GetAllList().Where(x => x.ConfigTypeId == (int)ConfigTypes.NotificationEmail).Select(x => x.Data).ToList();
            var baseUrl = ConfigRepository.GetAllList().Where(x => x.ConfigTypeId == (int)ConfigTypes.BaseUrl).Select(x => x.Data).FirstOrDefault();
            var projectUrl = Configuration.GetSection("App:ServerRootAddress").Value;
            if (unassociatedHoursProjects.Count() != 0)
            {
                string messageBody = "";
                foreach (var project in unassociatedHoursProjects)
                {
                    messageBody = messageBody + projectUrl + "projects/" + project + "/detail/unassociatedHourLogs\n";
                }
                notify = new NotifyParam
                {
                    EmailAddress = emailAddress,
                    Message = "Projects with unassociated hour logs: " + unassociatedHoursProjects.Count() + " \n\nProject Details: \n" + messageBody,
                    ImType = 1,
                    From = notificationSource
                };
                notify.Message = notify.Message.Replace("\n", Environment.NewLine);
            }
            else
            {
                notify = new NotifyParam
                {
                    EmailAddress = emailAddress,
                    Message = "Projects with unassociated hour logs: " + unassociatedHoursProjects.Count(),
                    ImType = 1,
                    From = notificationSource
                };
            }
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage res = await client.PostAsJsonAsync("api/CreateNotification", notify);
            if (!res.IsSuccessStatusCode)
            {
                throw new UserFriendlyException("User notify failed.");
            }

            return "User Notified";
        }
    }
}
