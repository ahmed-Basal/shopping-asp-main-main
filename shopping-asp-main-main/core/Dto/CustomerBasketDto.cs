using System;
using System.Collections.Generic;
using System.Text;

namespace core.Dto
{
    public  class CustomerBasketDto
    {
        public string Id { get; set; } 
        public List<BasketItemDto> Items { get; set; } = new List<BasketItemDto>();
        public decimal Total => Items.Sum(i => i.SubTotal); 
    }
}
