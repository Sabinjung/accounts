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

                if (child.DueDate < DateTime.Now.Date.AddDays(-14) && child.DueDate > DateTime.Now.Date.AddDays(-15))
                    child.TotalBalance = childrens.GroupBy(x => new { x.CompanyName, x.Balance, x.DueDate }).Where(x => x.Key.CompanyName == child.CompanyName && x.Key.DueDate < DateTime.Now.Date.AddDays(-14) && x.Key.DueDate > DateTime.Now.Date.AddDays(-15)).Sum(x => x.Key.Balance);

                if (child.DueDate < DateTime.Now.Date.AddDays(-13) && child.DueDate > DateTime.Now.Date.AddDays(-14))
                    child.TotalBalance = childrens.GroupBy(x => new { x.CompanyName, x.Balance, x.DueDate }).Where(x => x.Key.CompanyName == child.CompanyName && x.Key.DueDate < DateTime.Now.Date.AddDays(-13) && x.Key.DueDate > DateTime.Now.Date.AddDays(-14)).Sum(x => x.Key.Balance);

                if (child.DueDate < DateTime.Now.Date.AddDays(-12) && child.DueDate > DateTime.Now.Date.AddDays(-13))
                    child.TotalBalance = childrens.GroupBy(x => new { x.CompanyName, x.Balance, x.DueDate }).Where(x => x.Key.CompanyName == child.CompanyName && x.Key.DueDate < DateTime.Now.Date.AddDays(-12) && x.Key.DueDate > DateTime.Now.Date.AddDays(-13)).Sum(x => x.Key.Balance);

                if (child.DueDate < DateTime.Now.Date.AddDays(-11) && child.DueDate > DateTime.Now.Date.AddDays(-12))
                    child.TotalBalance = childrens.GroupBy(x => new { x.CompanyName, x.Balance, x.DueDate }).Where(x => x.Key.CompanyName == child.CompanyName && x.Key.DueDate < DateTime.Now.Date.AddDays(-11) && x.Key.DueDate > DateTime.Now.Date.AddDays(-12)).Sum(x => x.Key.Balance);

                if (child.DueDate < DateTime.Now.Date.AddDays(-10) && child.DueDate > DateTime.Now.Date.AddDays(-11))
                    child.TotalBalance = childrens.GroupBy(x => new { x.CompanyName, x.Balance, x.DueDate }).Where(x => x.Key.CompanyName == child.CompanyName && x.Key.DueDate < DateTime.Now.Date.AddDays(-10) && x.Key.DueDate > DateTime.Now.Date.AddDays(-11)).Sum(x => x.Key.Balance);

                if (child.DueDate < DateTime.Now.Date.AddDays(-9) && child.DueDate > DateTime.Now.Date.AddDays(-10))
                    child.TotalBalance = childrens.GroupBy(x => new { x.CompanyName, x.Balance, x.DueDate }).Where(x => x.Key.CompanyName == child.CompanyName && x.Key.DueDate < DateTime.Now.Date.AddDays(-9) && x.Key.DueDate > DateTime.Now.Date.AddDays(-10)).Sum(x => x.Key.Balance);

                if (child.DueDate < DateTime.Now.Date.AddDays(-8) && child.DueDate > DateTime.Now.Date.AddDays(-9))
                    child.TotalBalance = childrens.GroupBy(x => new { x.CompanyName, x.Balance, x.DueDate }).Where(x => x.Key.CompanyName == child.CompanyName && x.Key.DueDate < DateTime.Now.Date.AddDays(-8) && x.Key.DueDate > DateTime.Now.Date.AddDays(-9)).Sum(x => x.Key.Balance);

                if (child.DueDate < DateTime.Now.Date.AddDays(-7) && child.DueDate > DateTime.Now.Date.AddDays(-8))
                    child.TotalBalance = childrens.GroupBy(x => new { x.CompanyName, x.Balance, x.DueDate }).Where(x => x.Key.CompanyName == child.CompanyName && x.Key.DueDate < DateTime.Now.Date.AddDays(-7) && x.Key.DueDate > DateTime.Now.Date.AddDays(-8)).Sum(x => x.Key.Balance);

                if (child.DueDate < DateTime.Now.Date.AddDays(-6) && child.DueDate > DateTime.Now.Date.AddDays(-7))
                    child.TotalBalance = childrens.GroupBy(x => new { x.CompanyName, x.Balance, x.DueDate }).Where(x => x.Key.CompanyName == child.CompanyName && x.Key.DueDate < DateTime.Now.Date.AddDays(-6) && x.Key.DueDate > DateTime.Now.Date.AddDays(-7)).Sum(x => x.Key.Balance);

                if (child.DueDate < DateTime.Now.Date.AddDays(-5) && child.DueDate > DateTime.Now.Date.AddDays(-6))
                    child.TotalBalance = childrens.GroupBy(x => new { x.CompanyName, x.Balance, x.DueDate }).Where(x => x.Key.CompanyName == child.CompanyName && x.Key.DueDate < DateTime.Now.Date.AddDays(-5) && x.Key.DueDate > DateTime.Now.Date.AddDays(-6)).Sum(x => x.Key.Balance);

                if (child.DueDate < DateTime.Now.Date.AddDays(-4) && child.DueDate > DateTime.Now.Date.AddDays(-5))
                    child.TotalBalance = childrens.GroupBy(x => new { x.CompanyName, x.Balance, x.DueDate }).Where(x => x.Key.CompanyName == child.CompanyName && x.Key.DueDate < DateTime.Now.Date.AddDays(-4) && x.Key.DueDate > DateTime.Now.Date.AddDays(-5)).Sum(x => x.Key.Balance);

                if (child.DueDate < DateTime.Now.Date.AddDays(-3) && child.DueDate > DateTime.Now.Date.AddDays(-4))
                    child.TotalBalance = childrens.GroupBy(x => new { x.CompanyName, x.Balance, x.DueDate }).Where(x => x.Key.CompanyName == child.CompanyName && x.Key.DueDate < DateTime.Now.Date.AddDays(-3) && x.Key.DueDate > DateTime.Now.Date.AddDays(-4)).Sum(x => x.Key.Balance);

                if (child.DueDate < DateTime.Now.Date.AddDays(-2) && child.DueDate > DateTime.Now.Date.AddDays(-3))
                    child.TotalBalance = childrens.GroupBy(x => new { x.CompanyName, x.Balance, x.DueDate }).Where(x => x.Key.CompanyName == child.CompanyName && x.Key.DueDate < DateTime.Now.Date.AddDays(-2) && x.Key.DueDate > DateTime.Now.Date.AddDays(-3)).Sum(x => x.Key.Balance);

                if (child.DueDate < DateTime.Now.Date.AddDays(-1) && child.DueDate > DateTime.Now.Date.AddDays(-2))
                    child.TotalBalance = childrens.GroupBy(x => new { x.CompanyName, x.Balance, x.DueDate }).Where(x => x.Key.CompanyName == child.CompanyName && x.Key.DueDate < DateTime.Now.Date.AddDays(-1) && x.Key.DueDate > DateTime.Now.Date.AddDays(-2)).Sum(x => x.Key.Balance);

                if (child.DueDate < DateTime.Now.Date && child.DueDate > DateTime.Now.Date.AddDays(-1))
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
                Days = "15 days",
                Children = childrens.Where(x => x.DueDate < DateTime.Now.Date.AddDays(-14) && x.DueDate > DateTime.Now.Date.AddDays(-15) && x.Balance != 0).OrderBy(y => y.CompanyName).ToList()
            };
            result.Add(sixthChild);
            var seventhChild = new AgeingReportDto
            {
                Key = 7,
                Days = "14 days",
                Children = childrens.Where(x => x.DueDate <= DateTime.Now.Date.AddDays(-13) && x.DueDate > DateTime.Now.Date.AddDays(-14) && x.Balance != 0).OrderBy(y => y.CompanyName).ToList()
            };
            result.Add(seventhChild);
            var eighthChild = new AgeingReportDto
            {
                Key = 8,
                Days = "13 days",
                Children = childrens.Where(x => x.DueDate < DateTime.Now.Date.AddDays(-12) && x.DueDate > DateTime.Now.Date.AddDays(-13) && x.Balance != 0).OrderBy(y => y.CompanyName).ToList()
            };
            result.Add(eighthChild);
            var ninthChild = new AgeingReportDto
            {
                Key = 9,
                Days = "12 days",
                Children = childrens.Where(x => x.DueDate < DateTime.Now.Date.AddDays(-11) && x.DueDate > DateTime.Now.Date.AddDays(-12) && x.Balance != 0).OrderBy(y => y.CompanyName).ToList()
            };
            result.Add(ninthChild);
            var tenthChild = new AgeingReportDto
            {
                Key = 10,
                Days = "11 days",
                Children = childrens.Where(x => x.DueDate < DateTime.Now.Date.AddDays(-10) && x.DueDate > DateTime.Now.Date.AddDays(-11) && x.Balance != 0).OrderBy(y => y.CompanyName).ToList()
            };
            result.Add(tenthChild);
            var eleventhChild = new AgeingReportDto
            {
                Key = 11,
                Days = "10 days",
                Children = childrens.Where(x => x.DueDate < DateTime.Now.Date.AddDays(-9) && x.DueDate > DateTime.Now.Date.AddDays(-10) && x.Balance != 0).OrderBy(y => y.CompanyName).ToList()
            };
            result.Add(eleventhChild);
            var twelvethChild = new AgeingReportDto
            {
                Key = 12,
                Days = "9 days",
                Children = childrens.Where(x => x.DueDate < DateTime.Now.Date.AddDays(-8) && x.DueDate > DateTime.Now.Date.AddDays(-9) && x.Balance != 0).OrderBy(y => y.CompanyName).ToList()
            };
            result.Add(twelvethChild);
            var thirteenthChild = new AgeingReportDto
            {
                Key = 13,
                Days = "8 days",
                Children = childrens.Where(x => x.DueDate < DateTime.Now.Date.AddDays(-7) && x.DueDate > DateTime.Now.Date.AddDays(-8) && x.Balance != 0).OrderBy(y => y.CompanyName).ToList()
            };
            result.Add(thirteenthChild);
            var fourteenthChild = new AgeingReportDto
            {
                Key = 14,
                Days = "7 days",
                Children = childrens.Where(x => x.DueDate < DateTime.Now.Date.AddDays(-6) && x.DueDate > DateTime.Now.Date.AddDays(-7) && x.Balance != 0).OrderBy(y => y.CompanyName).ToList()
            };
            result.Add(fourteenthChild);
            var fifteenthChild = new AgeingReportDto
            {
                Key = 15,
                Days = "6 days",
                Children = childrens.Where(x => x.DueDate < DateTime.Now.Date.AddDays(-5) && x.DueDate > DateTime.Now.Date.AddDays(-6) && x.Balance != 0).OrderBy(y => y.CompanyName).ToList()
            };
            result.Add(fifteenthChild);
            var sixteenthChild = new AgeingReportDto
            {
                Key = 16,
                Days = "5 days",
                Children = childrens.Where(x => x.DueDate < DateTime.Now.Date.AddDays(-4) && x.DueDate > DateTime.Now.Date.AddDays(-5) && x.Balance != 0).OrderBy(y => y.CompanyName).ToList()
            };
            result.Add(sixteenthChild);
            var seventeenthChild = new AgeingReportDto
            {
                Key = 17,
                Days = "4 days",
                Children = childrens.Where(x => x.DueDate < DateTime.Now.Date.AddDays(-3) && x.DueDate > DateTime.Now.Date.AddDays(-4) && x.Balance != 0).OrderBy(y => y.CompanyName).ToList()
            };
            result.Add(seventeenthChild);
            var eighteenthChild = new AgeingReportDto
            {
                Key = 18,
                Days = "3 days",
                Children = childrens.Where(x => x.DueDate < DateTime.Now.Date.AddDays(-2) && x.DueDate > DateTime.Now.Date.AddDays(-3) && x.Balance != 0).OrderBy(y => y.CompanyName).ToList()
            };
            result.Add(eighteenthChild);
            var nineteenthChild = new AgeingReportDto
            {
                Key = 19,
                Days = "2 days",
                Children = childrens.Where(x => x.DueDate < DateTime.Now.Date.AddDays(-1) && x.DueDate > DateTime.Now.Date.AddDays(-2) && x.Balance != 0).OrderBy(y => y.CompanyName).ToList()
            };
            result.Add(nineteenthChild);
            var twenthChild = new AgeingReportDto
            {
                Key = 20,
                Days = "1 day",
                Children = childrens.Where(x => x.DueDate < DateTime.Now.Date && x.DueDate > DateTime.Now.Date.AddDays(-1) && x.Balance != 0).OrderBy(y => y.CompanyName).ToList()
            };
            result.Add(twenthChild);

            return result;
        }
    }
}
