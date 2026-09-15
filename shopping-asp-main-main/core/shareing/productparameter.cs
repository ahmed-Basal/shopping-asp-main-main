using System;
using System.Collections.Generic;
using System.Text;

namespace core.shareing
{
    public class ProductParameters
    {
        public string? Sort { get; set; }
        public int? CategoryId { get; set; }
        public string? Search { get; set; }

        private const int MaxPageSize = 50;

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value <= 0 ? 10 :
                               value > MaxPageSize ? MaxPageSize : value;
        }

        private int _pageNumber = 1;
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value <= 0 ? 1 : value;
        }
    }
}
