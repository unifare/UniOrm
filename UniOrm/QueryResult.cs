using System;
using System.Collections.Generic;
using System.Text;

namespace UniOrm
{
    public class QueryResult
    {
        public IEnumerable<dynamic> DataList { get; set; }
        public int currentIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPage { get; set; }
    }
}
