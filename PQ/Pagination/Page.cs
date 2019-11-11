using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PQ.Pagination
{
    public class Page<T>
    {
        /// <summary>
        /// IEnumerable of the final sorted, filtered and paginated data.
        /// </summary>
        public IEnumerable<T> Results { get; set; }
        /// <summary>
        /// The current page number.
        /// </summary>
        public int CurrentPage { get; set; }

        public int TotalCount { get; set; }
        /// <summary>
        /// Total number of pages.
        /// </summary>
        public int PageCount { get; set; }
        /// <summary>
        /// Number or records in a page.
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Total number of records that exist for the query.
        /// </summary>
        public int RecordCount { get; set; }


        public IEnumerable<PreDefinedQueryCount> RecordCounts { get; set; }
    }


    public class PreDefinedQueryCount
    {
        public string Name { get; set; }

        public int Count { get; set; }
    }
}
