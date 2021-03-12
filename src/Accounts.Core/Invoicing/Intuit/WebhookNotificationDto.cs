using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Invoicing.Intuit
{
    public class WebhookNotificationDto
    {
        public class Entities
        {
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("id")]
            public string Id { get; set; }
            [JsonProperty("operation")]
            public string Operation { get; set; }
            [JsonProperty("lastUpdated")]
            public string LastUpdated { get; set; } //validate type
        }

        public class DataChangeEvents
        {
            [JsonProperty("entities")]
            public List<Entities> Entities { get; set; }

        }
        public class EventNotifications
        {
            [JsonProperty("realmId")]
            public string RealmId { get; set; }

            [JsonProperty("dataChangeEvent")]
            public DataChangeEvents DataEvents { get; set; }

        }
        public class WebhooksData
        {
            [JsonProperty("eventNotifications")]
            public List<EventNotifications> EventNotifications { get; set; }

        }
    }
}
