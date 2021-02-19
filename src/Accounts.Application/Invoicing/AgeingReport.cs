using Accounts.Invoicing.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Invoicing
{
    public class AgeingReport
    {
        public async Task<IEnumerable<AgeingReportDto>> GetAgeingReport(List<Children> childrens)
        {
            foreach (var child in childrens)
            {
                if (child.DueDate < DateTime.Now.Date.AddDays(-120))
                    child.TotalBalance = childrens.GroupBy(x => new { x.CompanyName, x.Balance, x.DueDate }).Where(x => x.Key.CompanyName == child.CompanyName && x.Key.DueDate < DateTime.Now.Date.AddDays(-120)).Sum(x => x.Key.Balance);

                if (child.DueDate < DateTime.Now.Date.AddDays(-90) && child.DueDate > DateTime.Now.Date.AddDays(-120))
                    child.TotalBalance = childrens.GroupBy(x => new { x.CompanyName, x.Balance, x.DueDate }).Where(x => x.Key.CompanyName == child.CompanyName && x.Key.DueDate < DateTime.Now.Date.AddDays(-90) && x.Key.DueDate > DateTime.Now.Date.AddDays(-120)).Sum(x => x.Key.Balance);

                if (child.DueDate < DateTime.Now.Date.AddDays(-60) && child.DueDate > DateTime.Now.Date.AddDays(-90))
                    child.TotalBalance = childrens.GroupBy(x => new { x.CompanyName, x.Balance, x.DueDate }).Where(x => x.Key.CompanyName == child.CompanyName && x.Key.DueDate < DateTime.Now.Date.AddDays(-60) && x.Key.DueDate > DateTime.Now.Date.AddDays(-90)).Sum(x => x.Key.Balance);

                if (child.DueDate < DateTime.Now.Date.AddDays(-30) && child.DueDate > DateTime.Now.Date.AddDays(-60))
                    child.TotalBalance = childrens.GroupBy(x => new { x.CompanyName, x.Balance, x.DueDate }).Where(x => x.Key.CompanyName == child.CompanyName && x.Key.DueDate < DateTime.Now.Date.AddDays(-30) && x.Key.DueDate > DateTime.Now.Date.AddDays(-60)).Sum(x => x.Key.Balance);

                if (child.DueDate < DateTime.Now.Date.AddDays(-15) && child.DueDate > DateTime.Now.Date.AddDays(-30))
                    child.TotalBalance = childrens.GroupBy(x => new { x.CompanyName, x.Balance, x.DueDate }).Where(x => x.Key.CompanyName == child.CompanyName && x.Key.DueDate < DateTime.Now.Date.AddDays(-15) && x.Key.DueDate > DateTime.Now.Date.AddDays(-30)).Sum(x => x.Key.Balance);

                if (child.DueDate < DateTime.Now.Date && child.DueDate > DateTime.Now.Date.AddDays(-15))
                    child.TotalBalance = childrens.GroupBy(x => new { x.CompanyName, x.Balance, x.DueDate }).Where(x => x.Key.CompanyName == child.CompanyName && x.Key.DueDate < DateTime.Now.Date && x.Key.DueDate > DateTime.Now.Date.AddDays(-1)).Sum(x => x.Key.Balance);

            }
            var result = new List<AgeingReportDto>();

            var firstChild = new AgeingReportDto
            {
                Key = 1,
                Days = "120 days above",
                Children = childrens.Where(x => x.DueDate < DateTime.Now.Date.AddDays(-120) && x.Balance != 0).OrderBy(y => y.CompanyName).ToList()
            };
            result.Add(firstChild);

            var secondChild = new AgeingReportDto
            {
                Key = 2,
                Days = "91 - 120 days",
                Children = childrens.Where(x => x.DueDate < DateTime.Now.Date.AddDays(-90) && x.DueDate > DateTime.Now.Date.AddDays(-120) && x.Balance != 0).OrderBy(y => y.CompanyName).ToList()
            };
            result.Add(secondChild);

            var thirdChild = new AgeingReportDto
            {
                Key = 3,
                Days = "61 - 90 days",
                Children = childrens.Where(x => x.DueDate < DateTime.Now.Date.AddDays(-60) && x.DueDate > DateTime.Now.Date.AddDays(-90) && x.Balance != 0).OrderBy(y => y.CompanyName).ToList()
            };
            result.Add(thirdChild);

            var fourthChild = new AgeingReportDto
            {
                Key = 4,
                Days = "31 - 60 days",
                Children = childrens.Where(x => x.DueDate < DateTime.Now.Date.AddDays(-30) && x.DueDate > DateTime.Now.Date.AddDays(-60) && x.Balance != 0).OrderBy(y => y.CompanyName).ToList()
            };
            result.Add(fourthChild);
            var fifthChild = new AgeingReportDto
            {
                Key = 5,
                Days = "16 - 30 days",
                Children = childrens.Where(x => x.DueDate < DateTime.Now.Date.AddDays(-15) && x.DueDate > DateTime.Now.Date.AddDays(-30) && x.Balance != 0).OrderBy(y => y.CompanyName).ToList()
            };
            result.Add(fifthChild);
            
            var sixthChild = new AgeingReportDto
            {
                Key = 6,
                Days = "1 - 15 days",
                Children = childrens.Where(x => x.DueDate < DateTime.Now.Date && x.DueDate > DateTime.Now.Date.AddDays(-15) && x.Balance != 0).OrderBy(y => y.CompanyName).ToList()
            };
            result.Add(sixthChild);

            return result;
        }
    }
}
