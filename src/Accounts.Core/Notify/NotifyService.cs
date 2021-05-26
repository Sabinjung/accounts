using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using Accounts.Data;
using Accounts.Models;
using Accounts.Notify;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
        private readonly TeamNotification TeamNotification;

        public NotifyService (IRepository<HourLogEntry> hourlogRepository, IRepository<Consultant> consultantRepository, IRepository<Company> companyRepository, IRepository<Invoice> invoiceRepository, IOptions<TeamNotification> team, IRepository<Timesheet> timesheetRepository, IRepository<Config> configRepository, IConfiguration configuration)
        {
            TeamNotification = team.Value;
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
            var databaseInvoice = InvoiceRepository.FirstOrDefault(x => x.EInvoiceId == invoiceId);
            var companyName = CompanyRepository.FirstOrDefault(x => x.Id == databaseInvoice.CompanyId).DisplayName;
            var consultantName = ConsultantRepository.FirstOrDefault(x => x.Id == databaseInvoice.ConsultantId).DisplayName;
            ChannelNotifyParam notify = new ChannelNotifyParam
            {
                TeamId = "",
                TeamName = "",
                Message = $"Invoice has been {message}.\n" +
                $" Date: {DateTime.UtcNow.Date.ToString(" MM/dd/yyyy")}\n" +
                $" Customer Name: {companyName}\n" +
                $" Consultant Name: {consultantName}\n" +
                $" eInvoice ID:{invoiceId}\n" +
                $" Invoice link to Accounts application: {invoiceUrl + "invoices/" + databaseInvoice.Id + "\n"}\n"
            };
            await SendNotification(notify, (int)ConfigTypes.RCBot);

            return "User Notified";
        }
        public async Task<string> NotifyPayment(decimal? balance, string customerName, string invoiceId, string date,decimal remainingBalance)
        {
            var invoiceUrl = Configuration.GetSection("App:ServerRootAddress").Value;
            var databaseInvoice = InvoiceRepository.FirstOrDefault(x => x.EInvoiceId == invoiceId);
            ChannelNotifyParam notify = new ChannelNotifyParam
            {
                TeamId = "",
                TeamName = "",
                Message = "Amount has been paid by vendor.\n" +
                $" Payment Date: {date}\n" +
                $" Customer Name: {customerName}\n" +
                $" Amount Received: ${balance}\n" +
                $" eInvoice ID:{invoiceId}\n" +
                $" Remaining Balance: ${remainingBalance}\n" +
                $" Invoice link to Accounts application: {invoiceUrl + "invoices/" + databaseInvoice.Id + "\n"}\n" 
            };
            await SendNotification(notify, (int)ConfigTypes.RCChannel);
            return "User Notified";
        }

        public async Task<string> SendNotification(ChannelNotifyParam param,int channelId)
        {
            var client = new HttpClient();
            var emailAddress = ConfigRepository.GetAllList().Where(x => x.ConfigTypeId == (int)ConfigTypes.NotificationEmail).Select(x => x.Data).ToList();
            var baseUrl = ConfigRepository.GetAllList().Where(x => x.ConfigTypeId == (int)ConfigTypes.BaseUrl).Select(x => x.Data).FirstOrDefault();
            var channelName = ConfigRepository.GetAllList().Where(x => x.ConfigTypeId == channelId).Select(x => x.Data).FirstOrDefault();

            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage getTeams = await client.GetAsync("api/GetAllTeams");
            var jsonData = await getTeams.Content.ReadAsStringAsync();
            var teams = JsonConvert.DeserializeObject<List<TeamNotification>>(jsonData);
            var query = teams.Where(x => x.TeamName.ToLower() == channelName.ToLower()).Select(x => (x.TeamId, x.TeamName)).FirstOrDefault();
            param.TeamId = query.TeamId;
            param.TeamName = query.TeamName;
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
            var unassociatedTimesheets = TimesheetRepository.GetAllList().Where(x => x.InvoiceId == null).Select(x => x.Id).ToList();
            var unassociatedHoursProjects = HourlogRepository.GetAllList().Where(x => (x.TimesheetId == null || unassociatedTimesheets.Contains(x.TimesheetId.Value))&& x.Day < DateTime.Now.AddDays(-45)).GroupBy(y => y.ProjectId).Select(z => z.Key);
            var emailAddress = ConfigRepository.GetAllList().Where(x => x.ConfigTypeId == (int)ConfigTypes.NotificationEmail).Select(x => x.Data).ToList();
            var baseUrl = ConfigRepository.GetAllList().Where(x => x.ConfigTypeId == (int)ConfigTypes.BaseUrl).Select(x => x.Data).FirstOrDefault();
            var projectUrl = Configuration.GetSection("App:ServerRootAddress").Value;
            if (unassociatedHoursProjects.Count() != 0)
            {
                string messageBody = "";
                foreach (var project in unassociatedHoursProjects)
                {
                    messageBody = messageBody + projectUrl + "projects/" + project + "/detail\n";
                }
                notify = new NotifyParam
                {
                    EmailAddress = emailAddress,
                    Message = "Projects with unassociated hour logs: " + unassociatedHoursProjects.Count() + " \n\nProject Details: \n" + messageBody,
                    ImType = 1
                };
                notify.Message = notify.Message.Replace("\n", Environment.NewLine);
            }
            else
            {
                notify = new NotifyParam
                {
                    EmailAddress = emailAddress,
                    Message = "Projects with unassociated hour logs: " + unassociatedHoursProjects.Count(),
                    ImType = 1
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
