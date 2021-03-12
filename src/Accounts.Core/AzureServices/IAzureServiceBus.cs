using Intuit.Ipp.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.AzureServices
{
    public interface IAzureServiceBus
    {
        void ServiceBusListener();
    }
}
