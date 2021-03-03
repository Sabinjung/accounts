using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using Accounts.Data;
using Accounts.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
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
        private readonly IRepository<Timesheet> TimesheetRepository;
        private readonly IRepository<Config> ConfigRepository;
        private readonly IConfiguration Configuration;
        public NotifyService (IRepository<HourLogEntry> hourlogRepository, IRepository<Timesheet> timesheetRepository, IRepository<Config> configRepository, IConfiguration configuration)
        {
            HourlogRepository = hourlogRepository;
            TimesheetRepository = timesheetRepository;
            ConfigRepository = configRepository;
            Configuration = configuration;
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
                    messageBody = messageBody + projectUrl + project + "/detail\n";
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
