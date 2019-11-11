namespace Accounts.Core.Invoicing.Intuit
{
    public class IntuitSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUrl { get; set; }
        public string Environment { get; set; }

        public string ApiUrl { get; set; }

        public string MinorVersion { get; set; }
    }
}