using System;
using System.Collections.Generic;
using System.Text;

namespace core.Dto
{
    public  class CheckoutRequestDto
    {
        public int DeliveryMethodId { get; set; }

        public string FullName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
