using System;
using System.Collections.Generic;
using System.Text;

namespace core.Dto
{
    public  class BasketItemDto
    {

        public string Name { get; set; }
        public decimal Price { get; set; }
        public string PictureUrl { get; set; } // الصورة تظهر للـ Front-End
        public int Quantity { get; set; }
        public decimal SubTotal => Price * Quantity; // مجموع المنتج
    }
}
