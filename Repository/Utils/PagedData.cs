using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Utils
{
    public class PagedData<T>
    {
        public int PageNo { get; set; }
        public int TotalPages { get; set; }
        public int TotalRows { get; set; }
        public IQueryable<T> Data { get; set; }
    }
}
