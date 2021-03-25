using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Intuit.Dto
{
    public class IntuitCompanyDto
    {
        public string ExternalCustomerId { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string PaymentMethod { get; set; }
        public string Terms { get; set; }
        public int InvoiceCycleId { get; set; }
        public string BillingCountry { get; set; }
        public string BillingCity { get; set; }
        public string BillingZipCode { get; set; }
        public string BillingStreet { get; set; }
        public string BillingState { get; set; }
        public string ShippingCountry { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingZipCode { get; set; }
        public string ShippingStreet { get; set; }
        public string ShippingState { get; set; }
        public string Notes { get; set; }
    }
}
