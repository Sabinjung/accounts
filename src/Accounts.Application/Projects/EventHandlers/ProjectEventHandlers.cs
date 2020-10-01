using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Accounts.Models;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Projects.EventHandlers
{
    public class ProjectEventHandler : IAsyncEventHandler<EntityCreatedEventData<Project>>, IAsyncEventHandler<EntityUpdatedEventData<Project>>, IAsyncEventHandler<EntityDeletedEventData<Project>>, ITransientDependency
    {
        private readonly IRepository<Project> ProjectRepository;
        private readonly IRepository<Consultant> ConsultantRepository;
        private readonly IRepository<Company> CompanyRepository;
        private readonly IConfiguration Configuration;
        private readonly string WEBHOOK_URL;

        public ProjectEventHandler(IRepository<Project> _ProjectRepository,
             IRepository<Consultant> _ConsultantRepository,
             IRepository<Company> _CompanyRepository,
             IConfiguration _Configuration
          )
        {
            ProjectRepository = _ProjectRepository;
            ConsultantRepository = _ConsultantRepository;
            CompanyRepository = _CompanyRepository;
            Configuration = _Configuration;
            WEBHOOK_URL = Configuration.GetValue<string>("RingCentralNotification:ProjectNotificationWebhookUrl");
        }

        private void PushNotifaction(string WebhookUrl, object body)
        {
            RestClient client = new RestClient(WebhookUrl);
            RestRequest restRequest = new RestRequest(Method.POST);
            restRequest.AddHeader("Content-Type", "application/json");
            restRequest.RequestFormat = DataFormat.Json;
            restRequest.AddBody(body);
            var output = client.Execute(restRequest);
        }

        private object UpdateDetail(EntityUpdatedEventData<Project> eventdata)
        {
            var projectId = ProjectRepository.Get(eventdata.Entity.Id).Id;
            var project = ProjectRepository.Get(projectId);
            var createdBy = eventdata.Entity.CreatorUserId;
            var StartDate = eventdata.Entity.StartDt.ToString("MM/dd/yyyy");
            var CompanyName = CompanyRepository.Get(project.CompanyId).FullyQualifiedName;
            var Consultant = ConsultantRepository.Get(project.ConsultantId).DisplayName;
            var Rate = project.Rate;
            var projectdetail = new { activity = "Project", title = "**Account project change notification**", icon = "", body = "**The following projects have been changed/created in Accounts:**\n " + "**Company Name:** " + CompanyName + "\n**Consultant Name: **" + Consultant + "\n**Start Date : **" + StartDate };
            return projectdetail;
        }

        private object CreateDetail(EntityCreatedEventData<Project> eventdata)
        {
            var projectId = ProjectRepository.Get(eventdata.Entity.Id).Id;
            var project = ProjectRepository.Get(projectId);
            var createdBy = eventdata.Entity.CreatorUserId;
            var StartDate = eventdata.Entity.StartDt.ToString("MM/dd/yyyy");
            var CompanyName = CompanyRepository.Get(project.CompanyId).FullyQualifiedName;
            var Consultant = ConsultantRepository.Get(project.ConsultantId).DisplayName;
            var Rate = project.Rate;
            var projectdetail = new { activity = "Project", title = "**Account project change notification**", icon = "", body = "\n**The following projects have been changed/created in Accounts:** " + "\n**Company Name: **" + CompanyName + "\n**Consultant Name: **" + Consultant + " \n**Start Date : **" + StartDate };
            return projectdetail;
        }

        public async Task HandleEventAsync(EntityCreatedEventData<Project> eventdata)
        {
            var projectDetail = CreateDetail(eventdata);
            PushNotifaction(WEBHOOK_URL, projectDetail);
        }

        public async Task HandleEventAsync(EntityUpdatedEventData<Project> eventdata)
        {
            var projectDetail = UpdateDetail(eventdata);
            PushNotifaction(WEBHOOK_URL, projectDetail);
        }

        public async Task HandleEventAsync(EntityDeletedEventData<Project> eventdata)
        {
            var projectDetail = new { activity = "Project", title = "Account project change notification", icon = "", body = "The following projects have been changed/created in Accounts: " };
            PushNotifaction(WEBHOOK_URL, projectDetail);
        }
    }
}
