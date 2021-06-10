using Abp.Application.Services;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Core.Notify
{
    public interface INotifyService
    {
        Task<string> NotifyUser();
        Task<string> NotifyInvoice(string invoiceId,string message);
        Task<string> NotifyPayment(decimal? balance, string customerName, string invoiceId, string date,decimal remainingBalance);

    }
}
