using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Core.Specs
{
    public class CatalogSpecParams
    {
        private const int MaxPageSize = 80;
        private int _pageSize = 10;

        public int PageIndex {  get; set; }

        public int PageSize { 
            get => _pageSize; 
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string? BrandId { set; get; }
        public string? TypeId { set; get; }
        public string? Sort { set; get; }
        public string? Search { set; get; }
    }
}
