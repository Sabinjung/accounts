using Microsoft.AspNetCore.Antiforgery;
using Accounts.Controllers;

namespace Accounts.Web.Host.Controllers
{
    public class AntiForgeryController : AccountsControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }
    }
}
